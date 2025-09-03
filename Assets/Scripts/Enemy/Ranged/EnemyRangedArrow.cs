using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedArrow : MonoBehaviour
{
    [Header("적 화살 설정")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float speed;
    [SerializeField] float range;
    Rigidbody2D rb2D;
    int damage = 0;
    float dir = 0;
    float startPosX = 0f;
    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        enabled = false;
        gameObject.SetActive(false);
    }
    private void FixedUpdate()
    {
        rb2D.velocity = new Vector3(dir * speed, 0f, 0f);
        if (Mathf.Abs(gameObject.transform.position.x - startPosX) < range) return;
        SetDeactive();
    }
    public void Init(bool isFlipX, Vector3 spawnPos, int damage)
    {
        spriteRenderer.flipX = isFlipX;
        if (isFlipX) dir = -1f; 
        else dir = 1f;
        gameObject.transform.position = spawnPos;
        this.damage = damage;
        gameObject.SetActive(true);
        enabled = true;
        startPosX = spawnPos.x;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어만 맞도록 콜라이더 설정
        IDamageable damageable = collision.GetComponent<IDamageable>();
        damageable?.TakeDamage(damage);
        SetDeactive();
    }
    void SetDeactive()
    {
        gameObject.SetActive(false);
        enabled = false;
        dir = 0f;
    }
}
