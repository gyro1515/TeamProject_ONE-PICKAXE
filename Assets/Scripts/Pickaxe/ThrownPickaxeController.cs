using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownPickaxeController : MonoBehaviour
{
    [Header("Stuck Settings")]
    public float Damage = 50f;
    public float StickingOffset = 0.2f; // 땅에 박히는 정도
    public bool isPlayerFacingRight { get; private set; } // 플레이어의 방향을 저장할 변수 -> 곡괭이가 박히는 방향 결정

    [Header("Retrieve Settings")]
    public float RetrieveHoldDuration = 1.5f; // 원거리 회수 키 홀딩 시간
    public float RetrieveSpeed = 10f; // 회수 속도

    public Rigidbody2D Rb2D { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public RaycastHit2D LastHitInfo { get; private set; } // Raycast를 통해 얻은 충돌 정보를 저장할 변수
    private Animator Animator;

    // Animation Hash
    private static readonly int ThrowHash = Animator.StringToHash("Throw");
    private static readonly int RetrieveHash = Animator.StringToHash("Retrieve");

    // 던져진 곡괭이의 상태 머신
    private ThrownPickaxeStateMachine stateMachine;

    void Awake()
    {
        Rb2D = GetComponent<Rigidbody2D>();
        Animator = GetComponentInChildren<Animator>();

        stateMachine = new ThrownPickaxeStateMachine(this);
    }

    private void Start()
    {
        stateMachine.Initialize(stateMachine.FlyingState);
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

    // ThrownPickaxe를 생성할 때 플레이어의 방향을 전달받는 메서드 추가
    public void InitializePlayerFacing(bool playerFacingRight)
    {
        isPlayerFacingRight = playerFacingRight;
    }

    public void SetPlayerTransform(Transform transform)
    {
        PlayerTransform = transform;
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
}
