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
    [SerializeField] protected float attackCoolTime = 0f;
    [SerializeField] protected AudioClip attackSoundClip;
    [SerializeField] protected AudioClip deathSoundClip;
    protected float attackCoolTimer = 0f;
    public bool canAttack = true;
    public EnemyAnimationData AnimationData { get; private set; }
    public Player Target { get; set; } // 타겟 설정하기

    public EnemyController Controller { get; private set; }

    public float AttackRange { get { return attackRange; } }
    public float DetectionRange { get { return detectionRange; } }
    public AudioClip AttackSoundClip { get { return attackSoundClip; } }
    public AudioClip DeathSoundClip { get { return deathSoundClip; } }
    protected override void Awake()
    {
        base.Awake();
        Controller = GetComponent<EnemyController>();
        AnimationData = GetComponent<EnemyAnimationData>();
        AnimationData?.Initialize();
    }
    protected override void Update()
    {
        base.Update();

        CheckCoolTime();
    }
    public bool HasTarget()
    {
        return Target != null;
    }
    void CheckCoolTime()
    {
        // 공격 쿨타임 갱신하기
        if (canAttack) return;
        attackCoolTimer += Time.deltaTime;
        if (attackCoolTimer < attackCoolTime) return;
        attackCoolTimer -= attackCoolTime;
        canAttack = true;
    }
}
