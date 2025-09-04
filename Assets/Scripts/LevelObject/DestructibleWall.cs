using UnityEngine;

// InteractableObject는 OnCollisionEnter를 사용하도록 변경해야 합니다.
public class DestructibleWall : InteractableObject, IDamageable
{
    public int currentHealth = 3;
    public LayerMask weaponLayer;

    public override void Interaction(GameObject other)
    {
        // OnCollisionEnter 이벤트는 콜라이더가 닿는 순간 호출됩니다.
        if (((1 << other.layer) & weaponLayer) != 0)
        {
            TakeDamage(1);
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
        Destroy(gameObject);
    }
}