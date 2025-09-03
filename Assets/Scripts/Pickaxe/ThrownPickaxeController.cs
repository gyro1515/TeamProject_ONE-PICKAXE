using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrownPickaxeController : MonoBehaviour
{
    [Header("Stuck Settings")]
    public float Damage = 50f;
    public float StickingOffset = 0.2f; // 땅에 박히는 정도
    public bool IsPlayerFacingRight { get; private set; } // 플레이어의 방향을 저장할 변수 -> 곡괭이가 박히는 방향 결정

    [Header("Retrieve Settings")]
    public float RetrieveHoldDuration = 1.5f; // 원거리 회수 키 홀딩 시간
    public float RetrieveSpeed = 10f; // 회수 속도

    [Header("Bounce Settings")]
    public float BounceMoveDuration = 1f; // 튕겨나가는 이동 총 시간
    public AnimationCurve BounceCurve; // 튕겨나가는 궤도를 정의할 AnimationCurve
    public float BounceHeight = 5f; // 곡선이 도달할 최대 높이
    public GameObject ArrivalMark; // 착지 지점 표시 오브젝트(익스펙터에서 할당)

    [Header("Catch Settings")]
    public float CatchTime = 1f; // 캐치 가능 시간
    public bool WasBounced { get; set; } = false; // 튕김 상태에서 왔는지 확인하는 플래그
    public bool IsReadyToStick { get; set; } = false; // 착지 준비 플래그

    public Rigidbody2D Rb2D { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public RaycastHit2D LastHitInfo { get; private set; } // Raycast를 통해 얻은 충돌 정보를 저장할 변수
    private Animator Animator;
    private LayerMask groundLayerMask;
    private PlayerInput.PlayerActions playerActions;

    // 현재 회수 키(R)가 눌리고 있는지 여부
    public bool IsRetrieveHeld { get; private set; }

    // Animation Hash
    private static readonly int ThrowHash = Animator.StringToHash("Throw");
    private static readonly int RetrieveHash = Animator.StringToHash("Retrieve");

    // 던져진 곡괭이의 상태 머신
    private ThrownPickaxeStateMachine stateMachine;

    // 자신을 생성한 EquippedPickaxeController의 참조를 저장할 변수
    public EquippedPickaxeController Owner { get; private set; }

    void Awake()
    {
        Rb2D = GetComponent<Rigidbody2D>();
        Animator = GetComponentInChildren<Animator>();
        groundLayerMask = LayerMask.GetMask("Cave");

        stateMachine = new ThrownPickaxeStateMachine(this);
    }

    private void Start()
    {
        stateMachine.Initialize(stateMachine.FlyingState);
    }

    private void OnDisable()
    {
        // Retrieve 액션 연결 해제
        playerActions.RetrievePickaxe.started -= OnRetrieve;
        playerActions.RetrievePickaxe.canceled -= OnRetrieveCanceled;
    }

    void Update()
    {
        // 입력은 Update에서 처리
        stateMachine.HandleInput();
        stateMachine.UpdateState();
    }

    // 트리거 충돌 이벤트
    private void OnTriggerEnter2D(Collider2D other)
    {
        stateMachine.HandleTrigger(other);
    }

    // Owner를 설정하기 위한 메소드
    public void Initialize(EquippedPickaxeController owner, Transform playerTransform, bool isPlayerFacingRight, PlayerInput.PlayerActions actions)
    {
        Owner = owner;
        PlayerTransform = playerTransform;
        IsPlayerFacingRight = isPlayerFacingRight;
        playerActions = actions; // 전달받은 actions 저장

        // Retrieve 액션이 시작/취소될 때 함수 연결
        playerActions.RetrievePickaxe.started += OnRetrieve;
        playerActions.RetrievePickaxe.canceled += OnRetrieveCanceled;
    }

    public void SetLastHitInfo(RaycastHit2D hitInfo)
    {
        LastHitInfo = hitInfo;
    }

    // 던지기 애니메이션 재생
    public void PlayThrowAnimation()
    {
        Animator.SetBool(ThrowHash, true);
    }

    public void StopThrowAnimation()
    {
        Animator.SetBool(ThrowHash, false);
    }

    // 회수 애니메이션 재생
    public void PlayRetrieveAnimation()
    {
        Animator.SetTrigger(RetrieveHash);
    }

    // AnimationCurve 기반으로 튕기기 로직을 시작하는 메서드
    public void Bounce(Vector2 startPoint, Transform playerTransform)
    {
        // 타격 이펙트 재생 (TODO: 이펙트 재생 로직 호출)

        // 착지 지점 계산 (새로운 곡괭이의 컨트롤러를 사용하지 않고 바로 계산)
        Vector2 landingPoint = GetPlayerGroundPosition(playerTransform);

        // 새 곡괭이의 상태를 BounceState로 전환하고 필요한 정보 전달
        stateMachine.BounceState.SetBounceData(startPoint, landingPoint);
        stateMachine.ChangeState(stateMachine.BounceState);
    }

    // 플레이어 발밑의 바닥 지점을 찾는 메서드
    private Vector2 GetPlayerGroundPosition(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            return Vector2.zero;
        }

        // 플레이어의 콜라이더 컴포넌트 가져옴
        Collider2D playerCollider = playerTransform.GetComponent<Collider2D>();

        if (playerCollider == null)
        {
            // 콜라이더가 없으면 플레이어의 위치를 그대로 반환
            Debug.LogWarning("Player's Collider2D not found. Returning player position.");
            return playerTransform.position;
        }

        // 콜라이더의 바닥 위치 계산
        Vector2 playerBottom = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y);

        // 플레이어의 바닥에서 아래로 레이캐스트 쏜다(지형 레이어에만 충돌)
        RaycastHit2D hit = Physics2D.Raycast(playerBottom, Vector2.down, Mathf.Infinity, groundLayerMask);

        // 레이캐스트 디버그용 시각화
        Debug.DrawRay(playerBottom, Vector2.down * 10f, Color.red, 1f);

        if (hit.collider != null)
        {
            // 충돌한 지점의 위치 반환
            return hit.point;
        }
        else
        {
            // 바닥을 찾지 못하면 플레이어의 바닥 위치를 반환
            return playerBottom;
        }
    }

    // R키가 눌렸을 때 호출
    private void OnRetrieve(InputAction.CallbackContext context)
    {
        IsRetrieveHeld = true;
    }

    // R키에서 손을 뗐을 때 호출
    private void OnRetrieveCanceled(InputAction.CallbackContext context)
    {
        IsRetrieveHeld = false;
    }
}
