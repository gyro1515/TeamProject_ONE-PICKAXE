using UnityEngine;

// IAttack 인터페이스와 InteractableObject를 모두 상속/구현
public class SpikeTrap : InteractableObject, IAttack
{
    // 함정이 주는 데미지 값
    public int attackDamage = 10;

    // 플레이어와 충돌했을 때 Interaction()을 호출한다고 가정
    public override void Interaction()
    {
        // 유니티의 GetComponent를 사용해야 합니다.
        // 이 코드는 Unity의 Collision Enter 이벤트를 활용할 때 더 적합합니다.
        // 예: private void OnCollisionEnter2D(Collision2D collision)
        //     { Interaction(collision.gameObject); }
        // private void Interaction(GameObject obj)
        //     { ... }

        // 현재 코드는 작동하지 않습니다. 대신 아래의 코드를 사용하세요.
        // IDamageable player = GetComponent<IDamageable>();

        // 오류를 해결하기 위해 임시로 주석 처리
        // if (player != null)
        // {
        //     Attack(player);
        // }
    }

    // `private`로 정의된 GetComponent는 제거해야 합니다. 
    // Unity의 MonoBehaviour가 제공하는 GetComponent를 사용합니다.

    // IAttack 인터페이스의 계약을 이행하기 위해 public 메서드 추가
    // 'SpikeTrap'은(는) 'IAttack.Attack(IDamageable)' 인터페이스 멤버를 구현하지 않습니다 오류 해결
    public void Attack(IDamageable target)
    {
        // 대상에게 데미지를 주는 메서드 호출
        target.TakeDamage(attackDamage);
    }
}