using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeSmashState : PickaxeBaseState<EquippedPickaxeStateMachine>
{
    private float stateEnterTime; // 이 상태에 진입한 시간

    // 휘두르기 애니메이션 타이밍 (VFX 기준)
    private const float VFX_FRAME_RATE = 12f;
    private const float HITBOX_START_TIME = 1.0f / VFX_FRAME_RATE * 1; // 2번째 프레임 시작 시점(히트박스 활성화)
    private const float HITBOX_END_TIME = HITBOX_START_TIME + (1.0f / VFX_FRAME_RATE * 4); //판정 시작 후 4프레임 뒤 종료(히트박스 비활성화)
    private const float SMASH_TOTAL_DURATION = 1.0f / VFX_FRAME_RATE * 5; // 전체 VFX 길이(5프레임)

    private bool isHitboxActive = false;

    public override void EnterState(EquippedPickaxeStateMachine stateMachine)
    {
        Debug.Log("곡괭이 상태: 휘두르기");

        stateEnterTime = Time.time;
        stateMachine.EquippedPickaxeController.LastSmashTime = Time.time; // 쿨타임 계산을 위해 시간 기록
        isHitboxActive = false;

        // 곡괭이 휘두르기 애니메이션 재생
        stateMachine.EquippedPickaxeController.PlaySmashAnimation();
    }

    public override void ExitState(EquippedPickaxeStateMachine stateMachine)
    {
        // 상태 나갈 때 히트박스가 켜져있다면 비활성화
        if (isHitboxActive)
        {
            stateMachine.EquippedPickaxeController.SmashArea.SetActive(false);
            isHitboxActive = false;
        }
    }

    public override void HandleTrigger(EquippedPickaxeStateMachine stateMachine, Collider2D other)
    {
        // 히트박스가 활성화된 동안 충돌이 발생하면 공격 판정
        if (isHitboxActive)
        {
            Debug.Log("히트박스 충돌 감지!");

            // 공격 대상이 맞는지 확인
            if (other.CompareTag("Enemy"))
            {
                Debug.Log($"{other.name}을 공격했습니다!");

                // 파괴 가능한 오브젝트에 데미지 적용
                //IDamageable damageable = other.GetComponent<IDamageable>();
                //if (damageable != null)
                //{
                //    damageable.TakeDamage(stateMachine.PickaxeController.SmashDamage);
                //}
            }
        }
    }

    public override void UpdateState(EquippedPickaxeStateMachine stateMachine)
    {
        float elapsedTime = Time.time - stateEnterTime;

        // 히트박스 활성화 로직
        if (elapsedTime >= HITBOX_START_TIME && !isHitboxActive)
        {
            stateMachine.EquippedPickaxeController.SmashArea.SetActive(true);
            isHitboxActive = true;
            Debug.Log("히트박스 활성화!");
        }

        // 히트박스 비활성화 로직
        if (elapsedTime >= HITBOX_END_TIME && isHitboxActive)
        {
            stateMachine.EquippedPickaxeController.SmashArea.SetActive(false);
            isHitboxActive = false;
            Debug.Log("히트박스 비활성화!");
        }

        // 상태 복귀 로직
        if (elapsedTime >= SMASH_TOTAL_DURATION)
        {
            stateMachine.ChangeState(stateMachine.EquipState);
        }
    }

    public override void FixedUpdateState(EquippedPickaxeStateMachine stateMachine) { }
    public override void HandleCollision(EquippedPickaxeStateMachine stateMachine, Collision2D collision) { }
    public override void HandleInput(EquippedPickaxeStateMachine stateMachine) { }
}
