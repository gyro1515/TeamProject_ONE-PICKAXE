using UnityEngine;

public class PickaxeFlyingState : PickaxeBaseState<ThrownPickaxeStateMachine>
{
    public override void EnterState(ThrownPickaxeStateMachine stateMachine)
    {
        Debug.Log("곡괭이 상태: 날아가는 중");
    }

    public override void HandleTrigger(ThrownPickaxeStateMachine stateMachine, Collider2D other)
    {

    }

    public override void HandleCollision(ThrownPickaxeStateMachine stateMachine, Collision2D collision)
    {
        // 박히는 타일과 충돌했는지 확인
        if (collision.collider.CompareTag("CanStuck"))
        {
            stateMachine.ChangeState(stateMachine.StuckState);
        }
        // 튕기는 타일과 충돌했는지 확인
        else if (collision.collider.CompareTag("BouncingTile"))
        {
            stateMachine.ChangeState(stateMachine.BounceState);
        }
        // 기타 오브젝트(적, 플레이어 등)와의 충돌 처리
        else
        {
            // 이외의 충돌은 여기서 처리하거나 BouncingState로 전환
        }
    }

    public override void ExitState(ThrownPickaxeStateMachine stateMachine) { }
    public override void FixedUpdateState(ThrownPickaxeStateMachine stateMachine) { }
    public override void HandleInput(ThrownPickaxeStateMachine stateMachine) { }
    public override void UpdateState(ThrownPickaxeStateMachine stateMachine) { }
}
