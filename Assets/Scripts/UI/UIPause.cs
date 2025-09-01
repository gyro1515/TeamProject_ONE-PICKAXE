using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPause : BaseUI
{
    [Header("일시정지 설정")]
    [SerializeField] Button btnContinue;
    [SerializeField] Button btnOption;
    [SerializeField] Button btnReturnToMain;
    UIGameOption uiGameOption;

    private void Awake()
    {
        btnContinue.onClick.AddListener(CloseUI);
        btnOption.onClick.AddListener(OpenOption);
        btnReturnToMain.onClick.AddListener(ReturnToMain);
        uiGameOption = UIManager.Instance.GetUI<UIGameOption>();
        CloseUI();
    }
    private void OnEnable()
    {
        Time.timeScale = 0.0f;
    }
    private void OnDisable()
    {
        Time.timeScale = 1.0f;
    }
    void OpenOption()
    {
        StartCoroutine(StartOpenUIGameOption());
        //uiGameOption?.OpenUI();
    }
    IEnumerator StartOpenUIGameOption()
    {
        yield return FadeManager.Instance.FadeOut();
        uiGameOption?.OpenUI();
        yield return FadeManager.Instance.FadeIn();
    }
    void ReturnToMain()
    {
        SceneLoader.Instance.StartLoadScene(SceneState.WonJinTestStart);
    }
}
