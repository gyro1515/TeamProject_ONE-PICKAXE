using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownPickaxeController : MonoBehaviour
{
    public float Damage = 50f;
    public float StickingOffset = 0.5f; // 땅에 박히는 정도 (절반 정도 파묻히기 위함)

    public Rigidbody2D Rb2D { get; private set; }
    private Animator Animator;

    // 충돌 정보를 저장할 변수
    [HideInInspector] public Collision2D LastCollision;

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
