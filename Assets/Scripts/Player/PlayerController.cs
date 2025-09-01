using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    Player player;
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }
}
