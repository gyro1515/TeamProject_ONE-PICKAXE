using UnityEngine;

public class PickaxeController : MonoBehaviour
{
    public PickaxeStateMachine stateMachine;

    [Header("Smash Settings")]
    public float SmashCooldown = 0.2f;
    public float LastSmashTime = 0f;
    public GameObject SmashArea;

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

    // 휘두르기 애니메이션 재생
    public void PlaySmashAnimation()
    {
        Animator.SetTrigger(SmashHash);
    }
}