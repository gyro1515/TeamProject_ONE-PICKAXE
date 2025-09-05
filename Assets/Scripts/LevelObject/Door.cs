using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableObject
{
    private bool isOpen = false;

    // SpriteRenderer 컴포넌트들을 담을 리스트
    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

    // 문이 서서히 사라지는 데 걸리는 시간
    [SerializeField] private float fadeDuration = 2.0f;

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

        // 2. 찾은 SpriteRenderer가 있는지 확인합니다.
        if (spriteRenderers.Count == 0)
        {
            Debug.LogError("Door 스크립트: SpriteRenderer 컴포넌트를 찾을 수 없습니다. 문 오브젝트나 그 부모-자식에 SpriteRenderer가 있는지 확인하세요.");
        }
    }

    public override void Interaction(GameObject other)
    {
        // 이 메서드는 Interaction이 직접 호출될 때 사용됩니다.
    }

    public void Toggle()
    {
        if (isOpen) return;

        isOpen = true;
        Debug.Log("문이 열렸습니다. " + fadeDuration + "초 동안 서서히 사라집니다.");

        // SpriteRenderer가 있으면 페이드 코루틴 시작, 없으면 즉시 파괴
        if (spriteRenderers.Count > 0)
        {
            StartCoroutine(FadeAndDestroy());
        }
        else
        {
            DestroyParentOrSelf();
        }
    }

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
}