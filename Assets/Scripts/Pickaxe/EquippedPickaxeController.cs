using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquippedPickaxeController : MonoBehaviour
{
    [Header("Smash Settings")]
    public float SmashCooldown = 0.2f;
    public float LastSmashTime = 0f;
    public GameObject SmashArea;
    public Collider2D SmashHitBox { get; private set; } // 휘두르기 판정 영역의 콜라이더
    public int SmashDamage { get; private set; }

    [Header("Throw Settings")]
    public GameObject EquippedPickaxeObject; // 플레이어가 들고 있는 곡괭이 오브젝트
    public GameObject ThrowablePickaxePrefab; // 던지는 곡괭이 프리팹
    public float ThrowForce = 15f; // 던지는 힘
    public float ThrowRadius = 1.0f; // 플레이어로부터 생성될 위치의 반지름

    public LayerMask StuckableLayer; // Raycast가 감지할 박힐 수 있는 지형의 레이어

    [Header("SFX")]
    public AudioClip SmashSFX;
    public AudioClip SmashHitSFX;
    public AudioClip ThrowingSFX;
    public AudioClip CatchSFX;
    public AudioClip RetrieveSFX;

    public SoundSource RetrieveSoundSource;

    // 컴포넌트 및 오브젝트 참조
    private Animator Animator;

    private Collider2D playerCollider;
    private ContactFilter2D castContactFilter;
    private RaycastHit2D[] castResults = new RaycastHit2D[1];

    // Animation Hash
    private static readonly int SmashHash = Animator.StringToHash("Smash");
    private static readonly int CatchHash = Animator.StringToHash("Catch");

    // 장착 곡괭이의 상태머신
    private EquippedPickaxeStateMachine stateMachine;

    // 플레이어 입력 액션
    private PlayerInput.PlayerActions playerActions;
    private Player player;

    void Awake()
    {
        SmashHitBox = SmashArea.GetComponent<Collider2D>();
        Animator = GetComponentInChildren<Animator>();
        player = GetComponentInParent<Player>();

        stateMachine = new EquippedPickaxeStateMachine(this);

        playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogError("Player's Collider2D not found!");
        }

        castContactFilter = new ContactFilter2D();
        castContactFilter.SetLayerMask(StuckableLayer);
        castContactFilter.useTriggers = false;
    }

    void Start()
    {
        playerActions = GameManager.Instance.Player.Controller.PlayerActions;
        // Smash와 Throw 액션이 시작될 때 각각의 함수를 호출하도록 등록
        playerActions.SmashPickaxe.started += OnSmash;
        playerActions.ThrowPickaxe.started += OnThrow;

        stateMachine.Initialize(stateMachine.EquipState);

        // 스탯 설정
        SmashDamage = player.AttackPower;
    }

    private void OnDisable()
    {
        // 오브젝트가 비활성화될 때 등록했던 함수들 해제
        playerActions.SmashPickaxe.started -= OnSmash;
        playerActions.ThrowPickaxe.started -= OnThrow;
    }

    void Update()
    {
        // 입력은 Update에서 처리
        stateMachine.HandleInput();
        stateMachine.UpdateState();
    }

    void FixedUpdate()
    {
        // 물리 연산은 FixedUpdate에서 처리
        stateMachine.FixedUpdateState();
    }

    // OnCollisionEnter2D 같은 이벤트는 여기서 받아서 StateMachine에 전달
    private void OnCollisionEnter2D(Collision2D collision)
    {
        stateMachine.HandleCollision(collision);
    }

    // 트리거 충돌 이벤트
    private void OnTriggerEnter2D(Collider2D other)
    {
        stateMachine.HandleTrigger(other);
    }

    // EquippedPickaxeObject의 활성/비활성화 제어
    public void SetEquippedPickaxeActive(bool isActive)
    {
        EquippedPickaxeObject.SetActive(isActive);
    }

    // 휘두르기 애니메이션 재생
    public void PlaySmashAnimation()
    {
        Animator.SetTrigger(SmashHash);
    }

    // 캐치 애니메이션 재생
    public void PlayCatchAnimation()
    {
        Animator.SetTrigger(CatchHash);
    }

    // 곡괭이 회수하고 상태 초기화
    public void RetrievePickaxe(bool isCatch)
    {
        if(RetrieveSoundSource)
        {
            RetrieveSoundSource.Stop();
        }

        // 회수 UI 닫기
        UIRecallPickaxe uIRecallPickaxe = GameManager.Instance.Player.UIRecallPickaxe;
        uIRecallPickaxe?.CloseUI();

        // 플레이어의 곡괭이 소유 상태를 true로 변경
        player.HasPickaxe = true;

        // 장착된 곡괭이 오브젝트 다시 활성화
        SetEquippedPickaxeActive(true);

        // 캐치로 회수되었다면 Catch 애니메이션 재생
        if (isCatch)
        {
            if (CatchSFX)
            {
                SoundManager.PlayClip(CatchSFX);
            }

            PlayCatchAnimation();

            // CatchTextUI
            StartCoroutine(ShowCatchTextCoroutine());
        }

        // 상태를 기본 장착 상태(EquipState)로 강제 전환
        stateMachine.ChangeState(stateMachine.EquipState);
    }

    private IEnumerator ShowCatchTextCoroutine()
    {
        UICatchPickaxe uiCatchPickaxe = GameManager.Instance.Player.UICatchPickaxe;
        uiCatchPickaxe?.OpenUI();

        yield return new WaitForSeconds(0.7f);

        uiCatchPickaxe?.CloseUI();
    }

    private void OnSmash(InputAction.CallbackContext context)
    {
        // 현재 상태가 EquipState일 때만 반응
        if (stateMachine.CurrentState == stateMachine.EquipState)
        {
            // 쿨타임 확인
            if (Time.time >= LastSmashTime + SmashCooldown)
            {
                if (SmashSFX)
                {
                    SoundManager.PlayClip(SmashSFX);
                }

                // 휘두르기 상태로 전환
                stateMachine.ChangeState(stateMachine.SmashState);
            }
        }
    }

    private void OnThrow(InputAction.CallbackContext context)
    {
        player.Controller.FlipTowardsMouse();

        // 현재 상태가 EquipState일 때만 반응
        if (stateMachine.CurrentState == stateMachine.EquipState)
        {
            // 마우스 위치를 기반으로 던지는 방향 계산
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 playerPos = player.transform.position;
            Vector2 throwDirection = (mousePos - playerPos).normalized;

            // 플레이어 콜라이더를 이용해 전방 0.1f 거리에 장애물이 있는지 확인(Cast)
            int hitCount = playerCollider.Cast(throwDirection, castContactFilter, castResults, 0.1f);

            // 충돌이 감지되었고 그 대상이 박을 수 있는 태그라면 던지기 자체 취소
            if (hitCount > 0 && castResults[0].collider.CompareTag("CanStuck"))
            {
                Debug.Log("벽에 막혀 던질 수 없습니다.");
                return;
            }

            // 플레이어의 곡괭이 소유 상태를 false로 변경
            player.HasPickaxe = false;

            if (ThrowingSFX)
            {
                SoundManager.PlayClip(ThrowingSFX);
            }

            // 던지기 상태로 전환
            stateMachine.ChangeState(stateMachine.ThrowState);
        }
    }
}