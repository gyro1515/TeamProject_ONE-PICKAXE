using UnityEngine;

public class DestructibleWall : InteractableObject, IDamageable
{
    public int currentHealth = 3;

    // 매개변수를 받는 Interaction 메서드를 override하여 구현
    public override void Interaction(GameObject other)
    {

            // 이 벽 오브젝트에 데미지를 입힘
            TakeDamage(1);
        
    }

    // IDamageable 인터페이스 구현
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
        gameObject.SetActive(false);
    }
}