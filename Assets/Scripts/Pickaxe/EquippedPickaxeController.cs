using UnityEngine;

public class EquippedPickaxeController : MonoBehaviour
{
    public PickaxeStateMachine stateMachine;

    [Header("Smash Settings")]
    public float SmashCooldown = 0.2f;
    public float LastSmashTime = 0f;
    public GameObject SmashArea;

    [Header("Throw Settings")]
    public GameObject EquippedPickaxeObject; // 플레이어가 들고 있는 곡괭이 오브젝트
    public GameObject ThrowablePickaxePrefab; // 던지는 곡괭이 프리팹
    public float ThrowForce = 15f; // 던지는 힘
    public float ThrowRadius = 1.0f; // 플레이어로부터 생성될 위치의 반지름

    // 컴포넌트 및 오브젝트 참조
    private Transform PlayerTransform;
    private Rigidbody2D Rb2D;
    private Animator Animator;

    // Animation Hash
    private static readonly int SmashHash = Animator.StringToHash("Smash");

    void Awake()
    {
        PlayerTransform = GetComponentInParent<Transform>();
        Rb2D = GetComponent<Rigidbody2D>();
        Animator = GetComponentInChildren<Animator>();

        stateMachine = new PickaxeStateMachine(this);
    }

    void Start()
    {
        stateMachine.Initialize(stateMachine.EquipState);
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
}