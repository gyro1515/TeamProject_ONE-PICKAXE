using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour, IDamageable
{
    BaseCharacter baseCharacter;
    protected Animator animator;

    protected virtual void Awake()
    {
        baseCharacter = GetComponent<BaseCharacter>();
        animator = GetComponentInChildren<Animator>();
    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {
    }
    protected virtual void FixedUpdate()
    {
        Move();
    }
    protected virtual void Move()
    {
        // abstract로?
    }
    public virtual void TakeDamage(int damage)
    {
        // 예시 입니다.
        baseCharacter.CurrentHP -= damage;
        if (baseCharacter.CurrentHP <= 0 && !baseCharacter.IsDead) Dead();
    }
    protected virtual void Dead()
    {
        if (baseCharacter.IsDead) return; // 죽었으면 호출 안되게
        baseCharacter.IsDead = true;
    }
}
