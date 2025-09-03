using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseController
{
    [Header("적 유닛 컨트롤러 설정")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [Header("아이템 드랍 설정")]
    [SerializeField] private List<GameObject> droppableItems;

    [SerializeField] private float popUpForce = 5f;
    protected Enemy enemy;
    protected Rigidbody2D rb2D;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
        rb2D = GetComponent<Rigidbody2D>();
    }
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (enemy.CurrentHP <= 0) { animator.SetTrigger(enemy.AnimationData.DeathParameterHash); return; } // 데스는 테스트용
        
        // 아직 죽지 않았다면
        animator.SetTrigger(enemy.AnimationData.HurtParameterHash);
    }
    protected override void Update()
    {
        base.Update();
        // 테스트
        if(Input.GetKeyDown(KeyCode.Alpha7))
        {
            Debug.Log("테스트 TakeDamage");
            TakeDamage(10);
        }
    }
    protected float GetNormalizedTime(string tag = "Attack")
    {
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

        if (animator.IsInTransition(0) && nextInfo.IsTag(tag))
        {
            return Mathf.Repeat(nextInfo.normalizedTime, 1f);
        }
        else if (!animator.IsInTransition(0) && currentInfo.IsTag(tag))
        {
            return Mathf.Repeat(currentInfo.normalizedTime, 1f);
        }
        else
        {
            return 0f;
        }
    }
    protected bool CanAttack()
    {
        // 공격 범위 안에 있고, 공격 가능 시간이 되었는가
        if(IsInAttackRange() && enemy.canAttack)
        {
            enemy.canAttack = false;
            return true;
        }
        return false;
    }
    protected bool IsInAttackRange()
    {
        return (enemy.Target.gameObject.transform.position - enemy.gameObject.transform.position).sqrMagnitude
            <= enemy.AttackRange * enemy.AttackRange;
    }
    protected void FlipSpriteByTarget()
    {
        float dirX = enemy.Target.gameObject.transform.position.x - enemy.gameObject.transform.position.x;
        if (dirX < 0)
            spriteRenderer.flipX = true;
        else if(dirX > 0)
            spriteRenderer.flipX = false;
    }

     protected override void Dead()

    {

        base.Dead();

        if (droppableItems != null && droppableItems.Count > 0)
        {
            int randomIndex = Random.Range(0, droppableItems.Count);
            GameObject itemToDrop = droppableItems[randomIndex];

            // 아이템을 몬스터가 죽은 위치에 생성
            GameObject itemInstance = Instantiate(itemToDrop, transform.position, Quaternion.identity);

            // 생성된 아이템의 Rigidbody2D를 가져옴
            Rigidbody2D itemRb = itemInstance.GetComponent<Rigidbody2D>();

            // Rigidbody2D가 있다면 위로 힘을 가함

            if (itemRb != null)
            {
                // ForceMode2D.Impulse는 순간적인 힘을 가함
                itemRb.AddForce(Vector2.up * popUpForce, ForceMode2D.Impulse);
            }

            Debug.Log(">>> 아이템 " + itemToDrop.name + "을(를) 드랍했습니다.");
        }
        Destroy(gameObject);
    }
}

