using UnityEngine;

public class SpikeTrap : InteractableObject, IAttack
{
    // 함정이 주는 데미지 값
    public int attackDamage = 10;

    // Interaction() 메서드 오버라이드. 충돌한 GameObject를 받습니다.
    public override void Interaction(GameObject other)
    {
        // 디버그 로그: 어떤 오브젝트가 트랩과 상호작용을 시작했는지 확인
        Debug.Log("SpikeTrap: " + other.name + " 오브젝트가 트랩에 닿았습니다.", this.gameObject);

        // 충돌한 객체가 IDamageable 인터페이스를 구현했는지 확인
        IDamageable damageableTarget = other.GetComponent<IDamageable>();

        if (damageableTarget != null)
        {
            // 디버그 로그: 데미지 적용 대상이 유효한지 확인
            Debug.Log("SpikeTrap: " + other.name + " 오브젝트가 IDamageable을 가지고 있습니다. 데미지를 가합니다.", this.gameObject);

            // Attack 메서드 호출
            Attack(damageableTarget);
        }
        else
        {
            // 디버그 로그: 데미지 적용 대상이 유효하지 않은 경우
            Debug.Log("SpikeTrap: " + other.name + " 오브젝트는 IDamageable을 가지고 있지 않습니다. 데미지를 가할 수 없습니다.", this.gameObject);
        }
    }

    // IAttack 인터페이스의 Attack 메서드 구현
    public void Attack(IDamageable target)
    {
        // 대상에게 데미지를 주는 메서드 호출
        target.TakeDamage(attackDamage);

        // 디버그 로그: 최종적으로 데미지 메서드가 호출되었는지 확인
        Debug.Log("SpikeTrap: " + target.GetType().Name + "에 " + attackDamage + "만큼의 데미지를 가했습니다.", this.gameObject);
    }
}
