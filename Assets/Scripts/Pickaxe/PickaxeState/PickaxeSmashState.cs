using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeSmashState : PickaxeBaseState
{
    private float stateEnterTime; // 이 상태에 진입한 시간

    // 휘두르기 애니메이션 타이밍 (VFX 기준)
    private const float VFX_FRAME_RATE = 12f;
    private const float HITBOX_START_TIME = 1.0f / VFX_FRAME_RATE * 1; // 2번째 프레임 시작 시점
    private const float HITBOX_END_TIME = HITBOX_START_TIME + (1.0f / VFX_FRAME_RATE * 4); //판정 시작 후 4프레임 뒤 종료
    private const float SMASH_TOTAL_DURATION = 1.0f / VFX_FRAME_RATE * 5; // 전체 VFX 길이(5프레임)

    private bool isHitboxActive = false;

    public override void EnterState(PickaxeStateMachine stateMachine)
    {
        Debug.Log("곡괭이 상태: 휘두르기(Smash)");

        stateEnterTime = Time.time;
        stateMachine.PickaxeController.LastSmashTime = Time.time; // 쿨타임 계산을 위해 시간 기록
        isHitboxActive = false;

        // 곡괭이 휘두르기 애니메이션 재생
        
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

    public override void HandleInput(PickaxeStateMachine stateMachine)
    {

    }

    public override void HandleTrigger(PickaxeStateMachine stateMachine, Collider2D other)
    {

    }

    public override void UpdateState(PickaxeStateMachine stateMachine)
    {

    }
}
