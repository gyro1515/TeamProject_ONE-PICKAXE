using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack
{
    // 공격할 대상(IDamageable)을 매개변수로 받는 Attack 메서드로 수정
    void Attack(IDamageable target);
}