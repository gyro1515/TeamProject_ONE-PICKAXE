
using UnityEngine;

public class Ore : Item
{
    public int oreValue = 1;

    public override void OnPlayerCollide(Player player)
    {
        // 플레이어의 광물 점수 추가
        player.OreCount += oreValue;
        Debug.Log("광물 획득! 현재 광물 점수: " + player.OreCount);
    }
}