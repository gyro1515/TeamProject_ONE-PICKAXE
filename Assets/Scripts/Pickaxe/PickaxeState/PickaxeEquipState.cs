using UnityEngine;

public class PickaxeEquipState : PickaxeBaseState
{
    public override void EnterState(PickaxeStateMachine stateMachine)
    {
        Debug.Log($"곡괭이 상태: 착용");

        stateMachine.PickaxeController.SmashArea.SetActive(false);
    }

    public override void HandleInput(PickaxeStateMachine stateMachine)
    {
        if(Input.GetMouseButtonDown(0))
        {
            // 쿨타임 지났는지 확인
            if(Time.time >= stateMachine.PickaxeController.LastSmashTime + stateMachine.PickaxeController.SmashCooldown)
            {
                // 휘두르기 상태로 전환
                //stateMachine.ChangeState(stateMachine.smashState);
            }
        }

        //if (Input.GetMouseButtonDown(1))
        //{
        //    // 던지기 상태로 전환
        //    stateMachine.ChangeState(stateMachine.throwState);
        //}
    }

    public override void ExitState(PickaxeStateMachine stateMachine)
    {
        
    }

    public override void FixedUpdateState(PickaxeStateMachine stateMachine)
    {
        
    }

    public override void HandleCollision(PickaxeStateMachine stateMachine, Collision2D collision)
    {
        
    }

    public override void HandleTrigger(PickaxeStateMachine stateMachine, Collider2D other)
    {
        
    }

    public override void UpdateState(PickaxeStateMachine stateMachine)
    {

    }
}
