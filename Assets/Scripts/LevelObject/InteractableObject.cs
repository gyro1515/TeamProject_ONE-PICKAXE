using UnityEngine;

// InteractableObject는 충돌한 대상을 받도록 Interaction 메서드를 수정합니다.
public abstract class InteractableObject : MonoBehaviour
{
    // OnTriggerEnter2D를 사용하면 isTrigger 설정으로 더 유연하게 충돌을 처리할 수 있습니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트에서 InteractableObject의 상호작용을 실행합니다.
        // 플레이어에 한정하지 않고 모든 IDamageable 대상과 상호작용할 수 있게 설계합니다.
        Interaction(other.gameObject);
    }

    // 모든 자식 클래스가 반드시 구현해야 할 추상 메서드. 충돌한 대상을 매개변수로 받습니다.
    public abstract void Interaction(GameObject other);
}

