using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MeleeController : EnemyController
{
    //[Header("근접 유닛 컨트롤러 설정")]
    EnemyMelee melee;
    INode behaviorTreeRoot;
    float horizontalInput = 0f;
    public Vector2 patrolPos; // x: 왼쪽, y: 오른쪽
    float patrolTime;
    float patrolTimer;
    float preTime;
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>(); // 추후 베이스 컨트롤러에서 
        melee = GetComponent<EnemyMelee>();
        behaviorTreeRoot = SetBehaviorTree();
        patrolPos.x = gameObject.transform.position.x - melee.PatrolData / 2; // PatrolData는 지름
        patrolPos.y = gameObject.transform.position.x + melee.PatrolData / 2;
        patrolTime = melee.PatrolData / melee.MoveSpeed; // 순찰 거리에 따른 이동 시간
        patrolTimer = patrolTime / 2; // 중간에서 시작
    }
    protected override void Update()
    {
        base.Update();
        behaviorTreeRoot.Evaluate();
    }
    protected override void Move()
    {
        rb2D.velocity = new Vector2(horizontalInput * melee.MoveSpeed, rb2D.velocity.y);
        if(rb2D.velocity.x < 0f)
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
        // 설정되어 있다면 공격 범위 안에 있는가
        ActionNode checkAttackRangeAc = new ActionNode(CheckAttackRange);
        ActionNode attackAc = new ActionNode(Attack);
        SequenceNode attackSeq = new SequenceNode(new List<INode>() { checkAttackRangeAc, attackAc });
        // 공격 범위 밖이라면 쫓기
        ActionNode chaseAc = new ActionNode(Chase);
        SelectorNode actionSel = new SelectorNode(new List<INode>() { attackSeq, chaseAc });
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
            enabled = false; // 죽으면 업데이트 중지
            animator.SetTrigger(melee.AnimationData.DeathParameterHash); // 애니메이션 죽음으로
            return INode.ENodeState.Success;
        }
        return INode.ENodeState.Failure;
    }
    INode.ENodeState CheckAttackRange()
    {
        if(CanAttack())
        {
            // 범위 안에 있다면, 이동 멈추고
            horizontalInput = 0f;
            // 공격 시작
            animator.SetBool(melee.AnimationData.AttackParameterHash, true);
            animator.SetBool(melee.AnimationData.WalkParameterHash, false);
            return INode.ENodeState.Success;
        }
        // 범위 밖이라면 쫓기
        animator.SetBool(melee.AnimationData.AttackParameterHash, false);
        animator.SetBool(melee.AnimationData.WalkParameterHash, true);
        return INode.ENodeState.Failure;
    }
    INode.ENodeState Attack()
    {
        float normalizedTime = GetNormalizedTime("Attack"); // 0~1만 반환하도록 설정
        // 여전히 공격 범위 안이라면 계속 공격하기
        if (CanAttack())
        {
            if (normalizedTime > 0.95f) 
                FlipSpriteByTarget(); // 재생이 끝나고 타겟 위치에 따라 회전
            return INode.ENodeState.Running;
        }

        // 범위 밖이라면 재생 체크
        if (normalizedTime < 0.95f) return INode.ENodeState.Running;

        // 재생이 끝나고 움직이기
        animator.SetBool(melee.AnimationData.AttackParameterHash, false);
        animator.SetBool(melee.AnimationData.WalkParameterHash, true);
        return INode.ENodeState.Failure;
    }
    INode.ENodeState Chase()
    {
        if ((melee.Target.gameObject.transform.position - melee.gameObject.transform.position).x <= 0)
        {
            horizontalInput = -1.0f;
        }
        else
        {
            horizontalInput = 1.0f;
        }
        return INode.ENodeState.Success;
    }
    INode.ENodeState Patrol()
    {
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
