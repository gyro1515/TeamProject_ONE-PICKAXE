using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BaseCharacter
{
    // Enemy를 상속받은 원거리/근거리를 만들 것인가?
    // 적 상태 추가 필요

    [Header("적 세팅")]
    [SerializeField] float attackRange = 3f;
    [SerializeField] float detectionRange = 5f;

    public EnemyAnimationData AnimationData { get; private set; }
    public Player Target { get; set; } // 타겟 설정하기

    public EnemyController Controller { get; private set; }

    public float AttackRange { get { return attackRange; } }
    public float DetectionRange { get { return detectionRange; } }
    protected override void Awake()
    {
        base.Awake();
        Controller = GetComponent<EnemyController>();
        AnimationData = GetComponent<EnemyAnimationData>();
        AnimationData?.Initialize();
    }
    public bool HasTarget()
    {
        return Target != null;
    }
}
