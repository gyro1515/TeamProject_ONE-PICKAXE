using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    EnemyMelee enemy;
    float xPos = 1f;
    BoxCollider2D col;
    private void Awake()
    {
        enemy = GetComponentInParent<EnemyMelee>();
        xPos = gameObject.transform.localPosition.x; // 초기 x값
        col = GetComponent<BoxCollider2D>();
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        col.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 레이어 오버라이드로 충돌된 플레이어만 호출됨
        IDamageable damageable = collision.GetComponent<IDamageable>();
        damageable?.TakeDamage(enemy.AttackPower);
        //if (damageable != null) Debug.Log($"근접 유닛 공격: {collision.gameObject.name}");
    }
    public void SetAttackOn(bool isFlipX)
    {
        if(enemy.AttackSoundClip) SoundManager.PlayClip(enemy.AttackSoundClip);
        if (isFlipX)
        {
            gameObject.transform.localPosition = new Vector3(-xPos, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
        }
        else gameObject.transform.localPosition = new Vector3(xPos, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);

        gameObject.SetActive(true);
        col.enabled = true;
    }
}
