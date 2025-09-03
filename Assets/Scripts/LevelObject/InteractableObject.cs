using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    // 콜라이더를 통해 플레이어와 충돌할 때 호출될 메서드
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트가 플레이어인지 확인하는 로직
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            // Interaction() 메서드를 호출하여 상호작용 시작
            Interaction();
        }
    }

    // 모든 자식 클래스가 반드시 구현해야 할 추상 메서드
    public abstract void Interaction();
}
