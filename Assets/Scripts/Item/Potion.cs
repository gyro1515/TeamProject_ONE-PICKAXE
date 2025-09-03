
using UnityEngine;

public class Potion : Item
{
    public int healthToRestore = 1;

    public override void OnPlayerCollide(Player player)
    {
        if (player.CurrentHealth < player.MaxHealth)
        {
            player.CurrentHealth += healthToRestore;
            player.CurrentHealth = Mathf.Min(player.CurrentHealth, player.MaxHealth);
            Debug.Log("플레이어 체력 회복! 현재 체력: " + player.CurrentHealth);
        }
        else
        {
            Debug.Log("플레이어 체력이 가득 차 있어 포션을 사용할 수 없습니다.");
        }
    }
}
