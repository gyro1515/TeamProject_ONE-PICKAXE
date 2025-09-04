using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedSprite : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    EnemyRanged enemyRanged;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyRanged = GetComponentInParent<EnemyRanged>();
    }
    public void FireArrow()
    {
        // 굳이 컨트롤러까지 가서...?
        enemyRanged.RangedController.FireArrow(spriteRenderer.flipX);
        
    }
}
