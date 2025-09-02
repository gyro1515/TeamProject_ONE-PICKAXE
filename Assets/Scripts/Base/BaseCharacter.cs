using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour
{
    [field: Header("캐릭터 능력치 설정")]
    [field: SerializeField] public int AttackPower { get; private set; }
    [field: SerializeField] public int DefensePower { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;
    [field: SerializeField] public int MaxHP { get; private set; }
    protected int curHP;
    public bool IsDead { get; set; }
    public int CurrentHP { get { return curHP; }
        set 
        {
            curHP = value;
            curHP = Mathf.Clamp(curHP, 0, MaxHP);
            OnCurHpChange?.Invoke(curHP, MaxHP);
        } 
    }
    public event Action<float, float> OnCurHpChange; // UI에 바인드할 이벤트 델리게이트
    
    protected virtual void Awake()
    {
        CurrentHP = MaxHP;
    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {

    }
    protected virtual void FixedUpdate()
    {

    }
}
