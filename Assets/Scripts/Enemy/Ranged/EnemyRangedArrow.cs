using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedArrow : MonoBehaviour
{
    [Header("적 화살 설정")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float speed;
    [SerializeField] float range;
    [SerializeField] protected AudioClip hitSoundClip;
    [SerializeField] protected ParticleSystem hitParticle;

    Rigidbody2D rb2D;
    BoxCollider2D col;
    int damage = 0;
    float dir = 0;
    float startPosX = 0f;
    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        SetDeactive();
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
        startPosX = spawnPos.x;
        SetActive();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어만 맞도록 콜라이더 설정
        IDamageable damageable = collision.GetComponent<IDamageable>();
        damageable?.TakeDamage(damage);
        ArrowHit();
    }
    void SetActive()
    {
        col.enabled = true;
        spriteRenderer.gameObject.SetActive(true);
        gameObject.SetActive(true);
        enabled = true;
    }
    void SetDeactive()
    {
        gameObject.SetActive(false);
        enabled = false;
        col.enabled = false;
        dir = 0f;
    }
    void ArrowHit()
    {
        // 히트 사운드 출력
        if (hitSoundClip) SoundManager.PlayClip(hitSoundClip);
        // 히트 파티클 출력
        hitParticle?.Play();
        // 이동 멈추기
        dir = 0f;
        // 화살 이미지 끄기
        spriteRenderer.gameObject.SetActive(false);
        col.enabled = false;
        StartCoroutine(StartDeacive());
    }
    IEnumerator StartDeacive()
    {
        // 파티클 끝나면 비활성화 하기
        yield return new WaitForSeconds(hitParticle.main.duration);
        SetDeactive();
    }
}
