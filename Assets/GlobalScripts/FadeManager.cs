using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 페이드 인 아웃을 관리하는 매니저, 게임 시작 시 자동으로 생성, 씬 전환 시 페이드 효과 적용
public class FadeManager : SingletonMono<FadeManager>
{
    private Image fadeImage;
    private const float fadeDuration = 0.3f; // 페이드 시간

    // 페이드 인 아웃 체크용
    [HideInInspector] public bool isFadeOut = false;

    // 자동 초기화
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance != null)
        {
            if (Instance.fadeImage == null)
                Instance.SetupFadeImage();
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }
    private void SetupFadeImage()
    {
        // 캔버스 생성
        GameObject canvasGO = new GameObject("FadeCanvas");
        canvasGO.transform.SetParent(this.transform); // FadeManager의 자식으로 설정
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // UI 맨 위에 표시

        // 이미지 생성
        GameObject imgGO = new GameObject("FadeImage");
        imgGO.transform.SetParent(canvasGO.transform);
        fadeImage = imgGO.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.raycastTarget = true; // 클릭 이벤트를 받도록 설정하여, 
                                        // 페이드 아웃 중에는 다른 UI 상호작용을 막기

        // 풀스크린 설정
        RectTransform rt = fadeImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // 아래 함수들도 static으로 설정 가능하나, 다 instance.을 붙여 사용해야 함
    // 따라서 인스턴스 메소드로 설정 -> FadeManager.Instance.FadeOut()으로 사용
    public IEnumerator FadeOut()
    {
        //Debug.Log("FadeOut");
        fadeImage.raycastTarget = true;
        isFadeOut = true;
        float time = 0f;
        Color c = fadeImage.color;
        c.a = 0f;
        while (time < fadeDuration)
        {
            // 렉 걸렸을때 값 튐 방지
            float tmpDelta = Time.unscaledDeltaTime;
            tmpDelta = Mathf.Min(tmpDelta, 0.015f);
            time += tmpDelta;
            //Debug.Log($"Out :{time}");
            c.a = Mathf.Lerp(0f, 1f, time / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;

    }
    public IEnumerator FadeIn()
    {
        //Debug.Log("FadeIn");
        float time = 0f;
        Color c = fadeImage.color;
        c.a = 1f;
        while (time < fadeDuration)
        {
            // 렉 걸렸을때 값 튐 방지
            float tmpDelta = Time.unscaledDeltaTime;
            tmpDelta = Mathf.Min(tmpDelta, 0.015f);
            time += tmpDelta;
            //Debug.Log($"In :{time}");

            c.a = Mathf.Lerp(1f, 0f, time / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 0f;
        fadeImage.color = c;
        fadeImage.raycastTarget = false;
        isFadeOut = false;

    }

}
