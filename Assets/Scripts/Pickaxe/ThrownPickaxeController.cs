using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownPickaxeController : MonoBehaviour
{
    private Rigidbody2D Rb2D;
    private Animator Animator;

    // Animation Hash
    private static readonly int ThrowHash = Animator.StringToHash("Throw");

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
