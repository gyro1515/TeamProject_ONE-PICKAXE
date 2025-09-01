// IAttack 인터페이스와 InteractableObject를 모두 상속/구현
using System;

public class SpikeTrap : InteractableObject, IAttack
{
    // 함정이 주는 데미지 값
    public int attackDamage = 10;

    // 플레이어와 충돌했을 때 Interaction()을 호출한다고 가정
    public override void Interaction()
    {
        // 충돌한 객체가 IDamageable 인터페이스를 구현했는지 확인
        IDamageable player = GetComponent<IDamageable>();
        if (player != null)
        {
            // Attack 메서드 호출
            Attack(player);
        }
    }

    private T GetComponent<T>()
    {
        throw new NotImplementedException();
    }

    // IAttack 인터페이스의 Attack 메서드 구현
    public void Attack(IDamageable target)
    {
        // 대상에게 데미지를 주는 메서드 호출
        target.TakeDamage(attackDamage);
    }
}