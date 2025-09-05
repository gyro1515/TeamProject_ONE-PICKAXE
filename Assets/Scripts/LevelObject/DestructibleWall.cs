using UnityEngine;

public class DestructibleWall : InteractableObject, IDamageable
{
    public int currentHealth = 30;
    public LayerMask weaponLayer;

    private void Start()
    {
        DoorData wallData = DataTableManager.Instance.GetSingleData<DoorData>();
        currentHealth = wallData.door_hp;
    }

    public override void Interaction(GameObject other)
    {

    }

    public void TakeDamage(int damage)
    {
        // 데미지가 적용될 때 실행되는 로그
        Debug.Log("TakeDamage 메서드 호출됨.");

        currentHealth -= damage;
        Debug.Log("벽이 데미지를 입었습니다. 남은 체력: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("벽의 체력이 0이 되어 파괴됩니다.");
            DestroyWall();
        }
    }

    private void DestroyWall()
    {
        Debug.Log("부모 오브젝트를 포함한 전체 오브젝트를 파괴합니다.");

        // 현재 오브젝트의 부모 Transform을 가져옵니다.
        Transform parentTransform = transform.parent;

        // 부모가 있다면 부모를 파괴합니다.
        if (parentTransform != null)
        {
            Destroy(parentTransform.gameObject);
        }
        else // 부모가 없다면 자기 자신을 파괴합니다.
        {
            Destroy(gameObject);
        }
    }
}