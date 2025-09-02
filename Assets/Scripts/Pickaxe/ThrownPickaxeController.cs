using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownPickaxeController : MonoBehaviour
{
    public float Damage = 50f;
    public float StickingOffset = 0.2f; // 땅에 박히는 정도
    public bool isPlayerFacingRight { get; private set; } // 플레이어의 방향을 저장할 변수 -> 곡괭이가 박히는 방향 결정

    public Rigidbody2D Rb2D { get; private set; }
    public Collision2D LastCollision { get; private set; }
    private Animator Animator;

    // Animation Hash
    private static readonly int ThrowHash = Animator.StringToHash("Throw");

    // 던져진 곡괭이의 상태 머신
    private ThrownPickaxeStateMachine stateMachine;

    void Awake()
    {
        Rb2D = GetComponent<Rigidbody2D>();
        Animator = GetComponentInChildren<Animator>();

        stateMachine = new ThrownPickaxeStateMachine(this);
    }

    private void Start()
    {
        stateMachine.Initialize(stateMachine.FlyingState);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌 정보 저장
        LastCollision = collision;

        // 충돌 이벤트 처리를 현재 상태로 위임
        stateMachine.HandleCollision(collision);
    }

    // ThrownPickaxe를 생성할 때 플레이어의 방향을 전달받는 메서드 추가
    public void InitializeThrownPickaxe(bool playerFacingRight)
    {
        isPlayerFacingRight = playerFacingRight;
    }

    // 던지기 애니메이션 재생
    public void PlayThrowAnimation()
    {
        Animator.SetBool(ThrowHash, true);
    }

    public void StopThrowAnimation()
    {
        Animator.SetBool(ThrowHash, false);
    }
}
