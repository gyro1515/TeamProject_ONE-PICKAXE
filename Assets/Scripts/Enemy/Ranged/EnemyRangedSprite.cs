using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedSprite : MonoBehaviour
{
    [Header("원거리 유닛 애니메이션 세팅")]
    [SerializeField] FireArrowMuzzle fireMuzzle;
    SpriteRenderer spriteRenderer;
    EnemyRanged enemyRanged;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyRanged = GetComponentInParent<EnemyRanged>();
    }
    public void FireArrow()
    {
        fireMuzzle?.SetFlipX(spriteRenderer.flipX);
        // 굳이 컨트롤러까지 가서...?
        enemyRanged.RangedController.FireArrow(spriteRenderer.flipX, fireMuzzle.gameObject.transform.position);
        
    }
}
