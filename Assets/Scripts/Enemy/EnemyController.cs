using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseController
{
    [Header("적 유닛 컨트롤러 설정")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    protected Enemy enemy;
    protected Rigidbody2D rb2D;
    [Header("아이템 드랍 설정")]
    [SerializeField] private List<GameObject> droppableItems;

    [SerializeField][Range(0, 100)] private int itemDropChance = 100;
    [SerializeField] private float popUpForce = 5f;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
        rb2D = GetComponent<Rigidbody2D>();
    }
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (enemy.CurrentHP <= 0) return;
        
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
            TakeDamage(5);
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
        return enemy.CheckCanAttack();
    }
    protected bool IsInAttackRange()
    {
        return enemy.IsInAttackRange();
    }
    protected void FlipSpriteByTarget()
    {
        if(!enemy || !enemy.Target) { Debug.LogError("로직 오류"); return; }
        float dirX = enemy.Target.gameObject.transform.position.x - enemy.gameObject.transform.position.x;
        if (dirX < 0)
            spriteRenderer.flipX = true;
        else if(dirX > 0)
            spriteRenderer.flipX = false;
    }
    protected override void Dead()
    {
        base.Dead();
        animator.SetTrigger(enemy.AnimationData.DeathParameterHash);
        if (enemy.DeathSoundClip) SoundManager.PlayClip(enemy.DeathSoundClip);

        Debug.Log(">>> 'Dead()' 메서드 호출됨"); // 이 로그가 뜨는지 확인!

        // 0부터 100 사이의 임의의 숫자를 생성하여 드롭 확률을 결정

        if (Random.Range(0, 101) <= itemDropChance)
        {
            // 드롭 확률이 성공했을 때만 아이템을 드롭

            if (droppableItems != null && droppableItems.Count > 0)
            {
                int randomIndex = Random.Range(0, droppableItems.Count);
                GameObject itemToDrop = droppableItems[randomIndex];

                // 아이템을 몬스터가 죽은 위치에 생성

                GameObject itemInstance = Instantiate(itemToDrop, transform.position, Quaternion.identity);

                // 생성된 아이템에 위로 튀어 오르는 힘을 가함

                Rigidbody2D itemRb = itemInstance.GetComponent<Rigidbody2D>();

                if (itemRb != null)

                {
                    itemRb.AddForce(Vector2.up * popUpForce, ForceMode2D.Impulse);
                }

                Debug.Log(">>> 아이템 " + itemToDrop.name + "을(를) 드랍했습니다.");
            }
        }
        else
        {
            // 드롭에 실패했을 경우
            Debug.Log(">>> 아이템이 나오지 않았습니다.");
        }
        // 몬스터 오브젝트를 파괴
        //Destroy(gameObject);
        // 파괴하면 오류 발생 가능성 있어서 active만 끄겠습니다
        gameObject.SetActive(false);
    }
}
