using UnityEngine;

public class PickaxeRetrieveState : PickaxeBaseState<ThrownPickaxeStateMachine>
{
    private Rigidbody2D rb;
    private Transform playerTransform;

    public override void EnterState(ThrownPickaxeStateMachine stateMachine)
    {
        Debug.Log("곡괭이 상태: 회수 중");

        // 물리 활성화 및 초기화
        rb = stateMachine.ThrownPickaxeController.Rb2D;
        rb.isKinematic = false;
        rb.gravityScale = 0; // 중력 비활성화

        // 플레이어 Transform 가져오기
        playerTransform = stateMachine.ThrownPickaxeController.PlayerTransform;

        // 회수 애니메이션 재생
        stateMachine.ThrownPickaxeController.PlayRetrieveAnimation();
    }

    public override void HandleTrigger(ThrownPickaxeStateMachine stateMachine, Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("원거리 회수 완료!");

            // Owner(EquippedPickaxeController)에게 회수되었음을 알림
            if (stateMachine.ThrownPickaxeController.Owner != null)
            {
                stateMachine.ThrownPickaxeController.Owner.RetrievePickaxe();
            }

            // 곡괭이 오브젝트 파괴
            Object.Destroy(stateMachine.ThrownPickaxeController.gameObject);
        }
    }

    public override void UpdateState(ThrownPickaxeStateMachine stateMachine)
    {
        if (playerTransform == null)
        {
            // 플레이어가 사라진 경우(예: 사망) 회수 중단
            stateMachine.ChangeState(stateMachine.StuckState);
            return;
        }

        // 플레이어를 향해 직선 이동
        Vector2 directionToPlayer = (playerTransform.position - stateMachine.ThrownPickaxeController.transform.position).normalized;
        rb.velocity = directionToPlayer * stateMachine.ThrownPickaxeController.RetrieveSpeed;
    }

    public override void ExitState(ThrownPickaxeStateMachine stateMachine) { }
    public override void FixedUpdateState(ThrownPickaxeStateMachine stateMachine) { }
    public override void HandleCollision(ThrownPickaxeStateMachine stateMachine, Collision2D collision) { }
    public override void HandleInput(ThrownPickaxeStateMachine stateMachine) { }
}
