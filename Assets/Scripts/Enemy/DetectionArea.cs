using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionArea : MonoBehaviour
{
    CircleCollider2D col;
    Enemy enemy;
    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        enemy = GetComponentInParent<Enemy>();
        col.radius = enemy.DetectionRange;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        enemy.Target = collision.GetComponent<Player>();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("타겟 해제");
        enemy.Target = null;
    }
}
