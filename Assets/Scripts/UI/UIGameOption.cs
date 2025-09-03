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
    [SerializeField] AudioClip clickSoundClip;

    Canvas canvas;
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        btnApply.onClick.AddListener(SetVolum);
        btnReturn.onClick.AddListener(ReturnToStartMenu);
        CloseUI(); // 처음은 닫힌채로 시작
    }
    private void Start()
    {
        sliderVolum.value = SoundManager.Instance.MusicVolume;
    }
    private void OnDisable()
    {
        //Debug.Log("옵션 열림");
        canvas.sortingOrder = 2;
        canvas.enabled = true;
    }
    void SetVolum()
    {
        if (clickSoundClip) SoundManager.PlayClip(clickSoundClip);

        // 사운드 매니저 가져와서 볼륨설정하기
        SoundManager.Instance.MusicVolume = sliderVolum.value;
        SoundManager.Instance.SoundEffectVolume = sliderVolum.value;
    }
    public void ReturnToStartMenu()
    {
        if (clickSoundClip) SoundManager.PlayClip(clickSoundClip);
        //CloseUI();
        StartCoroutine(StartCloseUI());
    }
    IEnumerator StartCloseUI()
    {
        yield return FadeManager.Instance.FadeOut();
        canvas.sortingOrder = 0; // 바로 닫으면 코루틴 중지되기 때문에 눈속임으로...
        canvas.enabled = false; // 캔버스를 끄는 것도?
        yield return FadeManager.Instance.FadeIn();
        CloseUI();
    }
}
