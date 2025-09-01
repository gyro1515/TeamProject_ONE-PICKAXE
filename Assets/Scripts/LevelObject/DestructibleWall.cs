public class DestructibleWall : InteractableObject
{
    // Interaction()은 무기가 충돌했을 때 호출된다고 가정
    public override void Interaction()
    {
        // 콜라이더에 충돌한 무기의 타입을 확인
        // 예를 들어, 무기 타입이 "파괴 가능한" 타입인지 확인
        // if (collidingWeapon.weaponType == WeaponType.Destructive)
        // {
        //     // 벽을 파괴하는 로직 (예: 비활성화 또는 애니메이션 재생)
        //     this.gameObject.SetActive(false);
        // }
    }
}