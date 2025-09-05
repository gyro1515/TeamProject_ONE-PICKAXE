using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : Enemy
{
    [Header("근접 유닛 설정")]
    [SerializeField] float patrolData = 5f; // 기획은 20
    
    public float PatrolData { get { return patrolData; } }

    protected override void Start()
    {
        EnemyData enemyMeleeData = DataTableManager.Instance.GetCollectionData<EnemyData>(100);

        CurrentHP = enemyMeleeData.enemy_hp;
        AttackPower = enemyMeleeData.enemy_damage;
        MoveSpeed = enemyMeleeData.enemy_move;
        attackCoolTime = enemyMeleeData.enemy_cooldown;
        attackRange = enemyMeleeData.enemy_attackrange;
        detectionRange = enemyMeleeData.enemy_detectrange;
    }
}
