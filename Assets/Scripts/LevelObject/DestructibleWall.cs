using UnityEngine;
using System.Collections;
using System.Collections.Generic; // List 사용을 위해 추가

public class DestructibleWall : InteractableObject, IDamageable
{
    public int currentHealth = 30;
    public LayerMask weaponLayer;

    // SpriteRenderer 컴포넌트들을 담을 리스트
    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    // 원래 색상 저장
    private Color originalColor;
    // 깜빡임 효과가 진행 중인지 확인하는 플래그
    private bool isFlashing = false;

    private void Awake()
    {
        // 1. 이 스크립트가 부착된 오브젝트의 부모에서 모든 SpriteRenderer를 찾습니다.
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
            // 부모가 없다면, 현재 오브젝트에서 SpriteRenderer를 찾습니다.
            SpriteRenderer foundRenderer = GetComponent<SpriteRenderer>();
            if (foundRenderer != null)
            {
                spriteRenderers.Add(foundRenderer);
            }
        }

        // 2. 찾은 SpriteRenderer가 있는지 확인하고, 첫 번째 것의 원래 색상을 저장합니다.
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
        // 데미지가 적용될 때 실행되는 로그
        Debug.Log("TakeDamage 메서드 호출됨.");

        currentHealth -= damage;
        Debug.Log("벽이 데미지를 입었습니다. 남은 체력: " + currentHealth);

        // SpriteRenderer가 있고, 현재 깜빡임 중이 아니라면 깜빡임 코루틴 시작
        if (spriteRenderers.Count > 0 && !isFlashing)
        {
            StartCoroutine(FlashRed(0.1f, originalColor));
        }

        if (currentHealth <= 0)
        {
            Debug.Log("벽의 체력이 0이 되어 파괴됩니다.");
            DestroyWall();
        }
    }

    private void DestroyWall()
    {
        Debug.Log("부모 오브젝트를 포함한 전체 오브젝트를 파괴합니다.");

        // 현재 오브젝트의 부모 Transform을 가져옵니다.
        Transform parentTransform = transform.parent;

        // 부모가 있다면 부모를 파괴합니다.
        if (parentTransform != null)
        {
            Destroy(parentTransform.gameObject);
        }
        else // 부모가 없다면 자기 자신을 파괴합니다.
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashRed(float duration, Color restoreColor)
    {
        isFlashing = true;

        // 찾은 모든 SpriteRenderer의 색상을 변경합니다.
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = Color.red;
        }

        yield return new WaitForSeconds(duration);

        // 모든 SpriteRenderer의 색상을 원래대로 되돌립니다.
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = restoreColor;
        }

        isFlashing = false;
    }
}