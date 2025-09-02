using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    BaseCharacter baseCharacter;
    protected Animator animator;
    protected virtual void Awake()
    {
        baseCharacter = GetComponent<BaseCharacter>();
        animator = GetComponent<Animator>();
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
        if (baseCharacter.CurrentHP <= 0) Dead();
    }
    protected virtual void Dead()
    {
        baseCharacter.IsDead = true;
    }
}
