using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : Enemy
{
    [Header("근접 유닛 설정")]
    float patrolData = 5f; // 기획은 20
    
    public float PatrolData { get { return patrolData; } } 

}
