using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedController : EnemyController
{
    [Header("원거리 유닛 컨트롤러 세팅")]
    [SerializeField] FireArrowMuzzle fireMuzzle;
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
        // 여전히 공격 가능하면 공격하기
        // 타겟이 있고, 공격이 가능하며, 타겟과 원거리 유닛 발사지점 사이에 벽이 없다면 공격
        if (ranged.Target && CanAttack() && CanFire())
        {
            if(ranged.CanAttack) ranged.CanAttack = false;
            if (normalizedTime >= 0f && normalizedTime <= 0.05f)
            { FlipSpriteByTarget();} // 공격 초반 회전
            
            if (animator.GetBool(ranged.AnimationData.AttackParameterHash)) return;

            animator.SetBool(ranged.AnimationData.AttackParameterHash, true);
            animator.SetBool(ranged.AnimationData.IdleParameterHash, false);
            return;
        }
        // 공격 끝나고 회전이나 아이들 세팅하기
        if (animator.GetBool(ranged.AnimationData.AttackParameterHash) && normalizedTime <= 0.95f) return;

        if (ranged.Target)
        {
            FlipSpriteByTarget(); // 포착 됐을땐 회전만
        }
                                
        // 포착이 안됐거나, 포착은 됐지만 공격할 수 없다면 Idle로 세팅
        if (animator.GetBool(ranged.AnimationData.IdleParameterHash)) return;

        animator.SetBool(ranged.AnimationData.AttackParameterHash, false);
        animator.SetBool(ranged.AnimationData.IdleParameterHash, true);
    }
    public void FireArrow(bool isFlipX)
    {
        if (ranged.Target == null) return; // 발사할때 타겟이 사라졌다면 발사 취소
        fireMuzzle?.SetFlipX(isFlipX);
        /*if(!CanFire())
        {
            if (animator.GetBool(ranged.AnimationData.AttackParameterHash)) return;
            animator.SetBool(ranged.AnimationData.AttackParameterHash, false);
            animator.SetBool(ranged.AnimationData.IdleParameterHash, true);
            return;
        }*/
        if (enemy.AttackSoundClip) SoundManager.PlayClip(enemy.AttackSoundClip);
        EnemyRangedArrow arrow = ProjectileManager.Instance.GetObject(EProjectileType.EnemyRangedArrow).GetComponent<EnemyRangedArrow>();
        //arrow?.Init(isFlipX, fireMuzzle.gameObject.transform.position, enemy.AttackPower);
        Vector3 dir = ranged.Target.transform.position - fireMuzzle.gameObject.transform.position;
        arrow?.Init(dir.normalized, fireMuzzle.gameObject.transform.position, enemy.AttackPower);
    }
    bool CanFire()
    {
        fireMuzzle?.SetFlipX(spriteRenderer.flipX);
        // 적 발사 지점과 타겟 사이에 벽이 있는가
        Vector3 startPos = fireMuzzle.gameObject.transform.position;
        Vector3 dir = ranged.Target.transform.position - fireMuzzle.gameObject.transform.position;
        float distance = dir.magnitude;
        dir = dir.normalized;
        RaycastHit2D hit = Physics2D.BoxCast(startPos, 
            new Vector2(0.82f, 0.49f), 
            Mathf.Atan2(dir.y, dir.x), dir, distance, LayerMask.GetMask("Cave"));
        //RaycastHit2D hit = Physics2D.Raycast(startPos, dir, distance, LayerMask.GetMask("Cave"));
        /*float tmpDur = 1f;
        if(hit)
        {
            Debug.DrawLine(fireMuzzle.gameObject.transform.position, hit.point, Color.red, tmpDur);
            Debug.DrawLine(hit.point, ranged.Target.transform.position, Color.blue, tmpDur);
        }
        else
        {
            Debug.DrawLine(fireMuzzle.gameObject.transform.position, 
            ranged.Target.transform.position, Color.green, tmpDur);
        }*/
        if (hit) return false;

        return true;
    }
}
