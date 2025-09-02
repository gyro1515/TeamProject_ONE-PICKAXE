using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerAnimationData : MonoBehaviour, IAnimationData
{
    // 예시 입니다.
    [Header("애니메이션 데이터")]
    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string runParameterName = "Run";
    [SerializeField] private string AttackParameterName = "Attack";

    // Hash 값으로 비교하기 위해 변환한 값을 저장할 변수들, 문자열 비교보다 훨씬 효율적
    public int IdleParameterHash { get; private set; }
    public int RunParameterHash { get; private set; }
    public int AttackParameterHash { get; private set; }

    // Player의 Awake에서 호출 예정
    public void Initialize()
    {
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        RunParameterHash = Animator.StringToHash(runParameterName);
        AttackParameterHash = Animator.StringToHash(AttackParameterName);

    }
}