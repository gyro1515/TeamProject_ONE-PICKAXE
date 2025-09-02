using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PickaxeFlyingState : PickaxeBaseState<ThrownPickaxeStateMachine>
{
    public override void EnterState(ThrownPickaxeStateMachine stateMachine)
    {
        Debug.Log("곡괭이 상태: 날아가는 중");
    }

    public override void HandleTrigger(ThrownPickaxeStateMachine stateMachine, Collider2D other)
    {
        // 박히는 타일과 충돌했는지 확인
        if (other.CompareTag("CanStuck"))
        {
            // 곡괭이의 속도 방향으로 레이캐스트를 쏴서 충돌 정보를 얻습니다.
            Vector2 rayDirection = stateMachine.ThrownPickaxeController.Rb2D.velocity.normalized;
            float rayDistance = 0.5f; // 곡괭이 크기에 맞춰 적절한 값 설정

            var lastHitInfo = Physics2D.Raycast(stateMachine.ThrownPickaxeController.transform.position, rayDirection, rayDistance, other.gameObject.layer);
            stateMachine.ThrownPickaxeController.SetLastHitInfo(lastHitInfo);

            stateMachine.ChangeState(stateMachine.StuckState);
        }
        // 튕기는 타일과 충돌했는지 확인
        else if (other.CompareTag("BouncingTile"))
        {
            stateMachine.ChangeState(stateMachine.BounceState);
        }
        // 기타 오브젝트(적, 플레이어 등)와의 충돌 처리
        else
        {
            // 이외의 충돌은 여기서 처리하거나 BouncingState로 전환
        }
    }

    public override void HandleCollision(ThrownPickaxeStateMachine stateMachine, Collision2D collision)
    {

    }

    public override void ExitState(ThrownPickaxeStateMachine stateMachine) { }
    public override void FixedUpdateState(ThrownPickaxeStateMachine stateMachine) { }
    public override void HandleInput(ThrownPickaxeStateMachine stateMachine) { }
    public override void UpdateState(ThrownPickaxeStateMachine stateMachine) { }
}
