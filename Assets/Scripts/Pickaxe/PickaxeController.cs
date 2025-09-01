using UnityEngine;

public class PickaxeController : MonoBehaviour
{
    public PickaxeStateMachine stateMachine;

    public Transform PlayerTransform;
    public Rigidbody2D Rb2D;

    void Awake()
    {
        Rb2D = GetComponent<Rigidbody2D>();
        stateMachine = new PickaxeStateMachine(this);
    }

    void Start()
    {
        stateMachine.Initialize(stateMachine.equipState);
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
}