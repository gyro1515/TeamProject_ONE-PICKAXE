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
        animator = GetComponentInChildren<Animator>();
    }
    protected override void Update()
    {
        base.Update();
        if (ranged.Target && CanAttack())
        {
            float normalizedTime = GetNormalizedTime("Attack"); // 0~1만 반환하도록 설정
                                                                // 여전히 공격 범위 안이라면 계속 공격하기
            if (normalizedTime >= 0f && normalizedTime <= 0.05f)
            { FlipSpriteByTarget();} // 공격 초반 회전
            if (animator.GetBool(ranged.AnimationData.AttackParameterHash)) return;

            animator.SetBool(ranged.AnimationData.AttackParameterHash, true);
            animator.SetBool(ranged.AnimationData.IdleParameterHash, false);

        }
        else if(ranged.Target && !CanAttack())
        {
            FlipSpriteByTarget(); // 포착 됐을땐 회전만
        }
        else
        {
            if (animator.GetBool(ranged.AnimationData.IdleParameterHash)) return;

            animator.SetBool(ranged.AnimationData.AttackParameterHash, false);
            animator.SetBool(ranged.AnimationData.IdleParameterHash, true);
        }
    }
    public void FireArrow(bool isFlipX, Vector3 spawnPos)
    {
        EnemyRangedArrow arrow = ProjectileManager.Instance.GetObject(EProjectileType.EnemyRangedArrow).GetComponent<EnemyRangedArrow>();
        arrow?.Init(isFlipX, spawnPos, enemy.AttackPower);
    }
}
