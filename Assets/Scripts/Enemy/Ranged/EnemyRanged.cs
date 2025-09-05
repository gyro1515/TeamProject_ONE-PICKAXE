using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : Enemy
{
    public RangedController RangedController { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        RangedController = GetComponent<RangedController>();
    }

    protected override void Start()
    {
        EnemyData enemyMeleeData = DataTableManager.Instance.GetCollectionData<EnemyData>(101);

        CurrentHP = enemyMeleeData.enemy_hp;
        AttackPower = enemyMeleeData.enemy_damage;
        MoveSpeed = enemyMeleeData.enemy_move;
        attackCoolTime = enemyMeleeData.enemy_cooldown;
        attackRange = enemyMeleeData.enemy_attackrange;
        detectionRange = enemyMeleeData.enemy_detectrange;
    }
}
