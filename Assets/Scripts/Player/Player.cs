using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : BaseCharacter
{
    [field: Header("플레이어 세팅")]
    [field: SerializeField] public float JumpForce { get; private set; } = 7f;

    // 테스트로 AnimationData를 모노비헤이비어로 바꾸고 플레이어 게임 오브젝트에 스크립트로 추가해봤습니다.
    /*[field: Header("애니메이션 데이터")]
    [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; } = new PlayerAnimationData();*/
    public PlayerController Controller { get; private set; }
    public PlayerAnimationData AnimationData { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        AnimationData = GetComponent<PlayerAnimationData>();
        AnimationData.Initialize();
        Controller = GetComponent<PlayerController>();
    }
    
}
