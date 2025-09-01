using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseController
{
    Enemy enemy;
    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
    }
}
