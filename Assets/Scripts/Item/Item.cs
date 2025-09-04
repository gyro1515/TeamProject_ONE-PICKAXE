
using UnityEngine;

public abstract class Item : MonoBehaviour, IItem
{
    [SerializeField] protected GameObject parentGO;
    [SerializeField] protected AudioClip getSound;
    // 이 변수를 클래스 레벨에서 선언해야 상태가 유지됩니다.
    private bool hasBeenPickedUp = false;

    // 자식 클래스들이 반드시 구현해야 할 추상 메서드
    public abstract void OnPlayerCollide(Player player);

    // 플레이어와의 충돌을 감지하는 공통 로직
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(">>> OnTriggerEnter2D 메서드가 호출되었습니다. 충돌한 오브젝트: " + other.gameObject.name);
        // 이미 획득된 상태면 더 이상 진행하지 않음

        //Debug.Log("OnTriggerEnter2D 진입. hasBeenPickedUp 상태: " + hasBeenPickedUp);
        if (hasBeenPickedUp)
        {
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = other.GetComponentInParent<Player>();

            if (player != null)
            {
                hasBeenPickedUp = true;
                OnPlayerCollide(player);
                //Destroy(gameObject);
                if (getSound) SoundManager.PlayClip(getSound);
                if (parentGO) Destroy(parentGO);
            }
        }
    }

    /*protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        // 충돌한 오브젝트의 태그가 "Enemy"인지 확인
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // 이 아이템의 콜라이더와 적의 콜라이더 간의 물리적 충돌을 영구적으로 무시
            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), other.collider);
        }
    }*/
}
