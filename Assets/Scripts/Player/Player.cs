using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BaseCharacter
{
    //[Header("플레이어 세팅")]
    // 곡괭이 추가 필요
    // 플레이어 상태 추가 필요
    // 플레이어 입력 필요
    [field: Header("애니메이션 데이터")]
    [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; } = new PlayerAnimationData();
    public PlayerController Controller { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        AnimationData.Initialize();
        Controller = GetComponent<PlayerController>();
    }
}
