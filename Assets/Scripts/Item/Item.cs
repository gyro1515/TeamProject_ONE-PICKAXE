
using UnityEngine;

public abstract class Item : MonoBehaviour ,IItem
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 객체가 플레이어인지 확인
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            // 인터페이스 메서드 호출
            OnPlayerCollide(player);

            // 충돌한 아이템을 제거
            Destroy(gameObject);
        }
    }

    public abstract void OnPlayerCollide(Player player);
}
