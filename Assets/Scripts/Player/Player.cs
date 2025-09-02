using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : BaseCharacter
{
    [field: Header("플레이어 세팅")]
    [field: SerializeField] public float JumpForce { get; private set; } = 7f;
    

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
