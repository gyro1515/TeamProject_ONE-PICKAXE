using UnityEngine;

// InteractableObject는 충돌한 대상을 받도록 Interaction 메서드를 수정합니다.
public abstract class InteractableObject : MonoBehaviour
{
    // Interaction()은 충돌 시 호출될 메서드
    public abstract void Interaction(GameObject other);

    // OnTriggerEnter 대신 OnCollisionEnter를 사용
    // ********** OnCollisionEnter는 호출이 안될겁니다?************
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
        Interaction(collision.gameObject);
    }
    // OnTriggerEnter2D를 사용하면 isTrigger 설정으로 더 유연하게 충돌을 처리할 수 있습니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter2D");

        // 충돌한 오브젝트에서 InteractableObject의 상호작용을 실행합니다.
        // 플레이어에 한정하지 않고 모든 IDamageable 대상과 상호작용할 수 있게 설계합니다.
        Interaction(other.gameObject);
    }
}

