using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestructibleWall : InteractableObject, IDamageable
{
    public int currentHealth = 30;
    public LayerMask weaponLayer;

    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private Color originalColor;
    private bool isFlashing = false;

    // 벽이 서서히 사라지는 데 걸리는 시간 (2초)
    [SerializeField] private float fadeDuration = 2.0f;

    private void Awake()
    {
        if (transform.parent != null)
        {
            SpriteRenderer[] foundRenderers = transform.parent.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in foundRenderers)
            {
                spriteRenderers.Add(sr);
            }
        }
        else
        {
            SpriteRenderer foundRenderer = GetComponent<SpriteRenderer>();
            if (foundRenderer != null)
            {
                spriteRenderers.Add(foundRenderer);
            }
        }

        if (spriteRenderers.Count > 0)
        {
            originalColor = spriteRenderers[0].color;
            Debug.Log($"벽에서 {spriteRenderers.Count}개의 SpriteRenderer를 찾았습니다.");
        }
        else
        {
            Debug.LogError("SpriteRenderer를 찾을 수 없습니다. 스크립트가 올바른 오브젝트에 부착되었는지 확인하세요.");
        }
    }

    public override void Interaction(GameObject other)
    {
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("TakeDamage 메서드 호출됨.");

        currentHealth -= damage;
        Debug.Log("벽이 데미지를 입었습니다. 남은 체력: " + currentHealth);

        if (spriteRenderers.Count > 0 && !isFlashing)
        {
            StartCoroutine(FlashRed(0.1f, originalColor));
        }

        if (currentHealth <= 0)
        {
            Debug.Log("벽의 체력이 0이 되어 파괴됩니다. 서서히 사라집니다.");

            // 벽의 모든 콜라이더를 비활성화하여 적이 더 이상 통과할 수 있게 합니다.
            Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D col in colliders)
            {
                col.enabled = false;
            }

            if (spriteRenderers.Count > 0)
            {
                StartCoroutine(FadeAndDestroy());
            }
            else
            {
                DestroyParentOrSelf();
            }
        }
    }

    private IEnumerator FlashRed(float duration, Color restoreColor)
    {
        isFlashing = true;
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = Color.red;
        }
        yield return new WaitForSeconds(duration);
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = restoreColor;
        }
        isFlashing = false;
    }

    // 벽을 서서히 투명하게 만들고 파괴하는 코루틴
    private IEnumerator FadeAndDestroy()
    {
        List<Color> startColors = new List<Color>();
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            startColors.Add(sr.color);
        }

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                Color currentColor = startColors[i];
                currentColor.a = Mathf.Lerp(startColors[i].a, 0, progress);
                spriteRenderers[i].color = currentColor;
            }

            yield return null;
        }

        DestroyParentOrSelf();
    }

    // 부모 오브젝트 또는 자기 자신을 파괴하는 헬퍼 메서드
    private void DestroyParentOrSelf()
    {
        Debug.Log("부모 오브젝트를 포함한 전체 오브젝트를 파괴합니다.");
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            Destroy(parentTransform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // OnCollisionStay2D 메서드 (이전 버전에서 사용)
    // 현재는 충돌을 감지하는 코드를 TakeDamage에 바로 넣지 않았으므로 이 부분은 삭제했습니다.
}