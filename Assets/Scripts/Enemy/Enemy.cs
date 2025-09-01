using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : BaseCharacter
{
    // Enemy를 상속받은 원거리/근거리를 만들 것인가?
    // 적 상태 추가 필요

    [Header("적 세팅")]
    [SerializeField] float attackRange = 3f;
    [SerializeField] float chaseRange = 5f;
    [field: Header("애니메이션 데이터")]
    [field: SerializeField] public EnemyAnimationData AnimationData { get; private set; } = new EnemyAnimationData();

    public EnemyController Controller { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        AnimationData.Initialize();
        Controller = GetComponent<EnemyController>();
    }
}
