using UnityEngine;

public class PickaxeEquipState : PickaxeBaseState<EquippedPickaxeStateMachine>
{
    public override void EnterState(EquippedPickaxeStateMachine stateMachine)
    {
        Debug.Log($"곡괭이 상태: 착용");

        stateMachine.EquippedPickaxeController.SmashArea.SetActive(false);
    }

    public override void HandleInput(EquippedPickaxeStateMachine stateMachine)
    {
        if(Input.GetMouseButtonDown(0))
        {
            // 쿨타임 지났는지 확인
            if(Time.time >= stateMachine.EquippedPickaxeController.LastSmashTime + stateMachine.EquippedPickaxeController.SmashCooldown)
            {
                // 휘두르기 상태로 전환
                stateMachine.ChangeState(stateMachine.SmashState);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // 던지기 상태로 전환
            stateMachine.ChangeState(stateMachine.ThrowState);
        }
    }

    public override void ExitState(EquippedPickaxeStateMachine stateMachine) { }
    public override void FixedUpdateState(EquippedPickaxeStateMachine stateMachine) { }
    public override void HandleCollision(EquippedPickaxeStateMachine stateMachine, Collision2D collision) { }
    public override void HandleTrigger(EquippedPickaxeStateMachine stateMachine, Collider2D other) { }
    public override void UpdateState(EquippedPickaxeStateMachine stateMachine) { }
}
