// IAttack 인터페이스와 InteractableObject를 모두 상속/구현
public interface IDamageable
{
    // 이 메서드가 반드시 있어야 합니다.
    void TakeDamage(int damage);
}