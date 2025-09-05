using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MeleeController : EnemyController
{
    // 테스트용
    enum ETestState
    { None, Death, Attack, WaitToAttack, Chase, Ptrol, Hit }
    [SerializeField] ETestState state = ETestState.None; // 디버그용..., 상태 체크
    [SerializeField] EnemyMeleeSprite meleeSpriteScript;
    //[Header("근접 유닛 컨트롤러 설정")]
    EnemyMelee melee;
    INode behaviorTreeRoot;
    float horizontalInput = 0f;
    //public Vector2 patrolPos; // x: 왼쪽, y: 오른쪽, 디버그 용
    float patrolTime;
    float patrolTimer;
    float preTime;

    protected override void Awake()
    {
        base.Awake();
        melee = GetComponent<EnemyMelee>();
        behaviorTreeRoot = SetBehaviorTree();
        /*patrolPos.x = gameObject.transform.position.x - melee.PatrolData / 2; // PatrolData는 지름
        patrolPos.y = gameObject.transform.position.x + melee.PatrolData / 2;*/
        patrolTime = melee.PatrolData / melee.MoveSpeed; // 순찰 거리에 따른 이동 시간
        patrolTimer = patrolTime / 2; // 중간에서 시작
    }

    protected override void Start()
    {
        base.Start();
        animator.SetBool(melee.AnimationData.AttackParameterHash, false);
        animator.SetBool(melee.AnimationData.WalkParameterHash, true);
        animator.SetBool(melee.AnimationData.IdleParameterHash, false);
    }

    protected override void Update()
    {
        base.Update();
        if (GetNormalizedTime("Hurt") != -1f) { horizontalInput = 0f; state = ETestState.Hit; return; }
        state = ETestState.None;
        //Debug.Log("behaviorTreeRoot");
        behaviorTreeRoot.Evaluate();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        horizontalInput = 0f;
    }

    protected override void Move()
    {
        rb2D.velocity = new Vector2(horizontalInput * melee.MoveSpeed, rb2D.velocity.y);
        if (rb2D.velocity.x < 0f)
        {
            spriteRenderer.flipX = true;
        }
        else if (rb2D.velocity.x > 0f)
        {
            spriteRenderer.flipX = false;
        }
    }
    INode SetBehaviorTree()
    {
        // 죽음 여부부터 체크
        ActionNode checkIsDeadAc = new ActionNode(CheckIsDead);  

        // 안 죽었다면 타겟이 설정되어 있는가
        // 설정되어 있다면 공격 가능한가
        ActionNode checkCanAttackAc = new ActionNode(CheckCanAttack);
        ActionNode attackAc = new ActionNode(Attack);
        SequenceNode canAttackSeq = new SequenceNode(new List<INode>() { checkCanAttackAc, attackAc });
        // 공격 불가능하다면 공격 범위 안에는 있는가
        ActionNode checkAttackRangeAc = new ActionNode(CheckAttackRange);
        // 공격 범위 밖이라면 쫓기
        ActionNode chaseAc = new ActionNode(Chase);
        SelectorNode actionSel = new SelectorNode(new List<INode>() { canAttackSeq, checkAttackRangeAc, chaseAc });
        // melee.Target이 null이 아니면 actionSel실행되도록
        ConditionNode checkTargetCon = new ConditionNode(melee.HasTarget, actionSel); 

        // 타겟 설정이 안되어 있다면 순찰
        ActionNode patrolAc = new ActionNode(Patrol);

        SelectorNode rootSelector = new SelectorNode(new List<INode>() { checkIsDeadAc, checkTargetCon, patrolAc }); // 루트에 해당
        return rootSelector;
    }
    INode.ENodeState CheckIsDead()
    {
        if (melee.IsDead)
        {
            horizontalInput = 0f;
            rb2D.velocity = Vector2.zero;
            enabled = false; // 죽으면 업데이트 중지
            animator.SetTrigger(melee.AnimationData.DeathParameterHash); // 애니메이션 죽음으로
            state = ETestState.Death;
            return INode.ENodeState.Success;
        }
        return INode.ENodeState.Failure;
    }
    INode.ENodeState CheckCanAttack()
    {
        //Debug.Log("CheckCanAttack");
        // 우선 이동 멈추고 체크하기
        horizontalInput = 0f;
        
        if (CanAttack())
        {
            // 공격 시작
            animator.SetBool(melee.AnimationData.AttackParameterHash, true);
            animator.SetBool(melee.AnimationData.WalkParameterHash, false);
            animator.SetBool(melee.AnimationData.IdleParameterHash, false);
            // 공격 방향 설정
            FlipSpriteByTarget();
            return INode.ENodeState.Success;
        }
        
        // 공격 불가라면 일단 Idle세팅
        animator.SetBool(melee.AnimationData.AttackParameterHash, false);
        animator.SetBool(melee.AnimationData.WalkParameterHash, false);
        animator.SetBool(melee.AnimationData.IdleParameterHash, true);
        return INode.ENodeState.Failure;
    }
    INode.ENodeState Attack()
    {
        float normalizedTime = GetNormalizedTime("Attack"); // 0~1만 반환하도록 설정
        //Debug.Log($"AttackNode {normalizedTime}");

        // 여전히 공격 가능이라면 계속 공격하기
        // 특정 상황에 의해 공격이 해제되었다면
        if (!animator.GetBool(melee.AnimationData.AttackParameterHash))
        {
            meleeSpriteScript?.DeactiveCol();
            return INode.ENodeState.Failure; 
        }

        if(melee.CanAttack) melee.CanAttack = false;
        state = ETestState.Attack;
        // 공격이 재생중이라면
        if (normalizedTime <= 0.95f) { /*Debug.Log("Attacking");*/ return INode.ENodeState.Running; }

        // 공격 재생이 끝나고
        if (CanAttack()) // 여전히 공격히 가능하다면
        {
            FlipSpriteByTarget(); // 재공격이 가능하면 타겟 위치에 따라 회전
            return INode.ENodeState.Running;
        }
        else // 공격할 수 없다면
        {
            //Debug.Log($"AttackFnish, {normalizedTime}");
            // 재생이 끝나고 Idle로
            animator.SetBool(melee.AnimationData.AttackParameterHash, false);
            animator.SetBool(melee.AnimationData.WalkParameterHash, false);
            animator.SetBool(melee.AnimationData.IdleParameterHash, true);
            return INode.ENodeState.Failure;
        }
    }
    INode.ENodeState CheckAttackRange()
    {
        //Debug.Log("CheckAttackRange");
        if (IsInAttackRange())
        {
            state = ETestState.WaitToAttack;

            FlipSpriteByTarget();
            return INode.ENodeState.Success;
        }
        // Target이 없어지더라도 현재 위치에서 패트롤이 시작 되도록
        patrolTimer = patrolTime / 2; // 중간에서 시작
        // 범위 밖이라면 쫓기
        animator.SetBool(melee.AnimationData.AttackParameterHash, false);
        animator.SetBool(melee.AnimationData.WalkParameterHash, true);
        animator.SetBool(melee.AnimationData.IdleParameterHash, false);
        return INode.ENodeState.Failure;
    }
    INode.ENodeState Chase()
    {
        state = ETestState.Chase;

        if (melee.Target.gameObject.transform.position.x - melee.gameObject.transform.position.x < -0.05f)
        {
            horizontalInput = -1.0f;
            if (!animator.GetBool(melee.AnimationData.WalkParameterHash))
            {
                animator.SetBool(melee.AnimationData.AttackParameterHash, false);
                animator.SetBool(melee.AnimationData.WalkParameterHash, true);
                animator.SetBool(melee.AnimationData.IdleParameterHash, false);
            }
        }
        else if (melee.Target.gameObject.transform.position.x - melee.gameObject.transform.position.x > 0.05f)
        {
            horizontalInput = 1.0f;
            if (!animator.GetBool(melee.AnimationData.WalkParameterHash))
            {
                animator.SetBool(melee.AnimationData.AttackParameterHash, false);
                animator.SetBool(melee.AnimationData.WalkParameterHash, true);
                animator.SetBool(melee.AnimationData.IdleParameterHash, false);
            }
        }
        else
        {
            horizontalInput = 0f;
            if(!animator.GetBool(melee.AnimationData.IdleParameterHash))
            {
                animator.SetBool(melee.AnimationData.AttackParameterHash, false);
                animator.SetBool(melee.AnimationData.WalkParameterHash, false);
                animator.SetBool(melee.AnimationData.IdleParameterHash, true);
            }
        }
        return INode.ENodeState.Success;
    }
    INode.ENodeState Patrol()
    {
        state = ETestState.Ptrol;
        if (!animator.GetBool(melee.AnimationData.WalkParameterHash))
        {
            
            animator.SetBool(melee.AnimationData.AttackParameterHash, false);
            animator.SetBool(melee.AnimationData.WalkParameterHash, true);
            animator.SetBool(melee.AnimationData.IdleParameterHash, false);
        }
        if(meleeSpriteScript.gameObject.activeSelf)
        {
            // 공격 중 갑자기 타겟이 없어져서 공격 콜라이더 계속 활성화 되는 경우 있음
            meleeSpriteScript?.DeactiveCol();
        }
        // 움직이기
        float preT = Mathf.PingPong(patrolTimer / patrolTime, 1.0f);
        patrolTimer += Time.deltaTime;
        float t = Mathf.PingPong(patrolTimer / patrolTime, 1.0f);

        //float preInput = horizontalInput; // 방향 전환 디버그용
        if (preT > t)
        {
            horizontalInput = -1.0f;
        }
        else if(preT < t)
        {
            horizontalInput = 1.0f;
        }
        else horizontalInput = 0f;

        //if (preInput != horizontalInput) Debug.Log("방향전환"); // 방향 전환 디버그용

        // 위치 기반은 오차때문에 방향 전환이 순간 여러 번 될때가 있음...
        /*float moveTargetPosX = Mathf.Lerp(patrolPos.x, patrolPos.y, t);
        float clampX = Mathf.Clamp(gameObject.transform.position.x, patrolPos.x, patrolPos.y);
        if (moveTargetPosX > clampX)
        {
            horizontalInput = 1.0f;
        }
        else if (moveTargetPosX < clampX)
        {
            horizontalInput = -1.0f;
        }
        else horizontalInput = 0f;*/
        //Debug.Log($"TargetPos: {moveTargetPosX}, clampX: {clampX}, MoveDir: {horizontalInput}");

        return INode.ENodeState.Success;
    }
}
