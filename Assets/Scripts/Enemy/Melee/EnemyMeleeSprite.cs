using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeSprite : MonoBehaviour
{
    [Header("근접 유닛 애니메이션 세팅")]
    [SerializeField] GameObject attackColGO;

    SpriteRenderer spriteRenderer;
    MeleeAttack attack;
    private void Awake()
    {
        attack = attackColGO.GetComponent<MeleeAttack>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void ActiveCol()
    {
        attack?.SetAttackOn(spriteRenderer.flipX);
    }
    public void DeactiveCol()
    {
        attackColGO?.SetActive(false);
    }
}
