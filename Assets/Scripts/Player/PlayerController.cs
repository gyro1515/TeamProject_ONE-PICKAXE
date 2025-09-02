using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseController
{
    public PlayerInput Input { get; private set; }
    public PlayerInput.PlayerActions PlayerActions { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
        input = new PlayerInput();
    }
    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
