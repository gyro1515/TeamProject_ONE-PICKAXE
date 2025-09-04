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

    [Header("Throw Settings")]
    public GameObject EquippedPickaxeObject; // 플레이어가 들고 있는 곡괭이 오브젝트
    public GameObject ThrowablePickaxePrefab; // 던지는 곡괭이 프리팹
    public float ThrowForce = 15f; // 던지는 힘
    public float ThrowRadius = 1.0f; // 플레이어로부터 생성될 위치의 반지름

    // 컴포넌트 및 오브젝트 참조
    //private Transform PlayerTransform;
    //private Rigidbody2D Rb2D;
    private Animator Animator;

    // Animation Hash
    private static readonly int SmashHash = Animator.StringToHash("Smash");
    private static readonly int CatchHash = Animator.StringToHash("Catch");

    // 장착 곡괭이의 상태머신
    private EquippedPickaxeStateMachine stateMachine;

    // 플레이어 입력 액션
    private PlayerInput.PlayerActions playerActions;

    void Awake()
    {
        //Rb2D = GetComponent<Rigidbody2D>();
        SmashHitBox = SmashArea.GetComponent<Collider2D>();
        Animator = GetComponentInChildren<Animator>();

        stateMachine = new EquippedPickaxeStateMachine(this);
    }

    void Start()
    {
        playerActions = GameManager.Instance.Player.Controller.PlayerActions;
        // Smash와 Throw 액션이 시작될 때 각각의 함수를 호출하도록 등록
        playerActions.SmashPickaxe.started += OnSmash;
        playerActions.ThrowPickaxe.started += OnThrow;

        stateMachine.Initialize(stateMachine.EquipState);
    }

    private void OnEnable()
    {
        
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
        // 장착된 곡괭이 오브젝트 다시 활성화
        SetEquippedPickaxeActive(true);

        // 상태를 기본 장착 상태(EquipState)로 강제 전환
        stateMachine.ChangeState(stateMachine.EquipState);

        // 캐치로 회수되었다면 Catch 애니메이션 재생
        if (isCatch)
        {
            PlayCatchAnimation();
        }
    }

    private void OnSmash(InputAction.CallbackContext context)
    {
        // 현재 상태가 EquipState일 때만 반응
        if (stateMachine.CurrentState == stateMachine.EquipState)
        {
            // 쿨타임 확인
            if (Time.time >= LastSmashTime + SmashCooldown)
            {
                // 휘두르기 상태로 전환
                stateMachine.ChangeState(stateMachine.SmashState);
            }
        }
    }

    private void OnThrow(InputAction.CallbackContext context)
    {
        // 현재 상태가 EquipState일 때만 반응
        if (stateMachine.CurrentState == stateMachine.EquipState)
        {
            // 던지기 상태로 전환
            stateMachine.ChangeState(stateMachine.ThrowState);
        }
    }
}