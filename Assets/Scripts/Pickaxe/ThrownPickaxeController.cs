using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownPickaxeController : MonoBehaviour
{
    public float Damage = 50f;
    public float StickingOffset = 0.5f; // 땅에 박히는 정도 (절반 정도 파묻히기 위함)

    private Rigidbody2D Rb2D;
    private Animator Animator;

    // Animation Hash
    private static readonly int ThrowHash = Animator.StringToHash("Throw");

    // 던져진 곡괭이의 상태 머신
    private ThrownPickaxeStateMachine stateMachine;

    void Awake()
    {
        Rb2D = GetComponent<Rigidbody2D>();
        Animator = GetComponentInChildren<Animator>();
    }

    // 던지기 애니메이션 재생
    public void PlayThrowAnimation()
    {
        Animator.SetTrigger(ThrowHash);
    }
}
