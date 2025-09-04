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
    private bool hasAttacked = false; // 이번 공격에서 공격 판정을 이미 실행했는지 여부

    public override void EnterState(EquippedPickaxeStateMachine stateMachine)
    {
        Debug.Log("곡괭이 상태: 휘두르기");

        stateEnterTime = Time.time;
        stateMachine.EquippedPickaxeController.LastSmashTime = Time.time; // 쿨타임 계산을 위해 시간 기록
        isHitboxActive = false;
        hasAttacked = false; // 상태에 진입할 때마다 공격 플래그 초기화


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
            hasAttacked = false;
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

        // 공격 판정 로직
        if (elapsedTime >= HITBOX_START_TIME && !hasAttacked)
        {
            hasAttacked = true; // 중복 실행 방지
            PerformSmashAttack(stateMachine);
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

    private void PerformSmashAttack(EquippedPickaxeStateMachine stateMachine)
    {
        Debug.Log("공격 판정 실행!");
        Collider2D smashAreaCollider = stateMachine.EquippedPickaxeController.SmashHitBox;
        if (smashAreaCollider == null)
        {
            Debug.LogError("SmashArea에 Collider2D가 없습니다!");
            return;
        }

        // smashAreaCollider 콜라이더와 겹쳐있는 모든 콜라이더를 리스트에 담는다
        List<Collider2D> hitTargets = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter(); // 특정 레이어만 거르려면 여기서 설정
        int hitCount = smashAreaCollider.OverlapCollider(filter, hitTargets);

        if (hitCount > 0)
        {
            foreach (var target in hitTargets)
            {
                if (target.CompareTag("Enemy"))
                {
                    Debug.Log($"{target.name}을 공격했습니다!");

                    IDamageable damageable = target.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(stateMachine.EquippedPickaxeController.SmashDamage);
                    }
                }
            }
        }
    }

    public override void HandleTrigger(EquippedPickaxeStateMachine stateMachine, Collider2D other) { }
    public override void FixedUpdateState(EquippedPickaxeStateMachine stateMachine) { }
    public override void HandleCollision(EquippedPickaxeStateMachine stateMachine, Collision2D collision) { }
    public override void HandleInput(EquippedPickaxeStateMachine stateMachine) { }
}
