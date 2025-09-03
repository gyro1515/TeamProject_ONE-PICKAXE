using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationData : MonoBehaviour, IAnimationData
{
    // 예시 입니다.
    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string walkParameterName = "Walk";
    [SerializeField] private string hurtParameterName = "Hurt";
    [SerializeField] private string deathParameterName = "Death";
    [SerializeField] private string AttackParameterName = "Attack";

    // Hash 값으로 비교하기 위해 변환한 값을 저장할 변수들, 문자열 비교보다 훨씬 효율적
    public int IdleParameterHash { get; private set; }
    public int WalkParameterHash { get; private set; }
    public int HurtParameterHash { get; private set; }
    public int DeathParameterHash { get; private set; }
    public int AttackParameterHash { get; private set; }

    // Enemy Awake에서 호출 예정
    public void Initialize()
    {
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        WalkParameterHash = Animator.StringToHash(walkParameterName);
        HurtParameterHash = Animator.StringToHash(hurtParameterName);
        DeathParameterHash = Animator.StringToHash(deathParameterName);
        AttackParameterHash = Animator.StringToHash(AttackParameterName);

    }
}
