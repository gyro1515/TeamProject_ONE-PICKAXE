using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAttack : MonoBehaviour
{
    BoxCollider2D boxColForBodyAttackTrigger;
    private void Awake()
    {
        boxColForBodyAttackTrigger = GetComponent<BoxCollider2D>();
    }
    // 트리거에 들어온 플레이어에게 데미지 주기(트리거용 콜라이더에 include/exclude 설정해야 함)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 레이어 오버라이드로 충돌된 플레이어만 호출됨
        IDamageable damageable = collision.GetComponent<IDamageable>();
        damageable?.TakeDamage(1);
        if (damageable != null)
        {
            //Debug.Log($"몸박: {collision.gameObject.name}");
            // OnTriggerEnter초기화
            // -> 시간초로? 플레이어에게 무적? 기획에 따라 달라질듯 합니다.
            boxColForBodyAttackTrigger.enabled = false;
            boxColForBodyAttackTrigger.enabled = true;
        }
    }
}
