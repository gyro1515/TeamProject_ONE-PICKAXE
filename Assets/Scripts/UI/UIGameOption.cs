using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOption : BaseUI
{
    [Header("옵션 메뉴 설정")]
    [SerializeField] Button btnApply;
    [SerializeField] Button btnReturn;
    [SerializeField] Slider sliderVolum;
    Canvas canvas;
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        btnApply.onClick.AddListener(SetVolum);
        btnReturn.onClick.AddListener(ReturnToStartMenu);
        sliderVolum.value = SoundManager.Instance.MusicVolume;
        CloseUI(); // 처음은 닫힌채로 시작
    }
    private void OnDisable()
    {
        canvas.sortingOrder = 2;
    }
    void SetVolum()
    {
        // 사운드 매니저 가져와서 볼륨설정하기
        SoundManager.Instance.MusicVolume = sliderVolum.value;
        SoundManager.Instance.SoundEffectVolume = sliderVolum.value;
    }
    void ReturnToStartMenu()
    {
        StartCoroutine(StartCloseUI());
    }
    IEnumerator StartCloseUI()
    {
        yield return FadeManager.Instance.FadeOut();
        canvas.sortingOrder = 0; // 바로 닫으면 코루틴 중지되기 때문에 눈속임으로...
        yield return FadeManager.Instance.FadeIn();
        CloseUI();
    }
}
