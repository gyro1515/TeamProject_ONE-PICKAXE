using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedController : EnemyController
{
    EnemyRanged ranged;
    protected override void Awake()
    {
        base.Awake();
        ranged = GetComponent<EnemyRanged>();
    }
    protected override void Update()
    {
        base.Update();
        float normalizedTime = GetNormalizedTime("Attack"); // 0~1만 반환하도록 설정
                                                            // 여전히 공격 범위 안이라면 계속 공격하기
        if (ranged.Target && CanAttack())
        {
            if (normalizedTime >= 0f && normalizedTime <= 0.05f)
            { FlipSpriteByTarget();} // 공격 초반 회전
            
            if (animator.GetBool(ranged.AnimationData.AttackParameterHash)) return;

            animator.SetBool(ranged.AnimationData.AttackParameterHash, true);
            animator.SetBool(ranged.AnimationData.IdleParameterHash, false);
            return;
        }
        // 공격 끝나고 회전이나 아이들 세팅하기
        if (normalizedTime <= 0.95f) return;

        if (ranged.Target && !CanAttack())
        {
            FlipSpriteByTarget(); // 포착 됐을땐 회전만
        }
                                
        // 포착이 안됐거나, 포착은 됐지만 공격할 수 없다면 Idle로 세팅
        if (animator.GetBool(ranged.AnimationData.IdleParameterHash)) return;

        animator.SetBool(ranged.AnimationData.AttackParameterHash, false);
        animator.SetBool(ranged.AnimationData.IdleParameterHash, true);
    }
    public void FireArrow(bool isFlipX, Vector3 spawnPos)
    {
        EnemyRangedArrow arrow = ProjectileManager.Instance.GetObject(EProjectileType.EnemyRangedArrow).GetComponent<EnemyRangedArrow>();
        arrow?.Init(isFlipX, spawnPos, enemy.AttackPower);
    }
}
