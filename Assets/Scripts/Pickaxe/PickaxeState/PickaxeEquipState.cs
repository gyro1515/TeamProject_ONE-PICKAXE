using UnityEngine;

public class PickaxeEquipState : PickaxeBaseState<EquippedPickaxeStateMachine>
{
    public override void EnterState(EquippedPickaxeStateMachine stateMachine)
    {
        Debug.Log($"곡괭이 상태: 착용");

        stateMachine.EquippedPickaxeController.SmashArea.SetActive(false);
    }

    public override void HandleInput(EquippedPickaxeStateMachine stateMachine) { }
    public override void ExitState(EquippedPickaxeStateMachine stateMachine) { }
    public override void FixedUpdateState(EquippedPickaxeStateMachine stateMachine) { }
    public override void HandleCollision(EquippedPickaxeStateMachine stateMachine, Collision2D collision) { }
    public override void HandleTrigger(EquippedPickaxeStateMachine stateMachine, Collider2D other) { }
    public override void UpdateState(EquippedPickaxeStateMachine stateMachine) { }
}
