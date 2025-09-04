using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : Enemy
{
    public RangedController RangedController { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        RangedController = GetComponent<RangedController>();
    }

}
