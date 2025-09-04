using UnityEngine;

public class DestructibleWall : InteractableObject, IDamageable
{
    public int currentHealth = 3;
    // 무기 레이어를 설정할 변수
    public LayerMask weaponLayer;

    public override void Interaction(GameObject other)
    {
        Debug.Log("벽에 " + other.name + "이(가) 닿았습니다. 레이어: " + other.layer);
        Debug.Log("유효한 레이어 마스크 값: " + weaponLayer.value);

        // 충돌한 오브젝트의 레이어가 'weaponLayer'에 포함되는지 확인
        if (((1 << other.layer) & weaponLayer) != 0)
        {
            Debug.Log("유효한 레이어입니다. 데미지를 입힙니다.");
            TakeDamage(1);
        }
        else
        {
            Debug.Log("유효하지 않은 레이어입니다. 데미지가 적용되지 않습니다.");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("벽이 데미지를 입었습니다. 남은 체력: " + currentHealth);

        if (currentHealth <= 0)
        {
            DestroyWall();
        }
    }

    private void DestroyWall()
    {
        // `Destroy(gameObject);`를 사용하여 오브젝트를 완전히 제거합니다.
        // `gameObject.SetActive(false);`는 오브젝트를 비활성화만 할 뿐 파괴하지 않습니다.
        Destroy(gameObject);
    }
}