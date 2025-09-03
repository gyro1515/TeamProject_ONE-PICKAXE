using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStartMenu : BaseUI
{
    [Header("시작 메뉴 설정")]
    [SerializeField] Button gameStartBtn;
    [SerializeField] Button gameOptionBtn;
    [SerializeField] Button gameEndBtn;

    UIGameOption uiGameOption;
    private void Awake()
    {
        gameStartBtn.onClick.AddListener(GameStart);
        gameOptionBtn.onClick.AddListener(OpenOption);
        gameEndBtn.onClick.AddListener(ExitGame);
        uiGameOption = UIManager.Instance.GetUI<UIGameOption>();
    }
    void GameStart()
    {
        SceneLoader.Instance.StartLoadScene(SceneState.Stage_WonJinTest);
    }
    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
    void OpenOption()
    {
        StartCoroutine(StartOpenUIGameOption());
    }
    IEnumerator StartOpenUIGameOption()
    {
        yield return FadeManager.Instance.FadeOut();
        uiGameOption.OpenUI();
        StartCoroutine(FadeManager.Instance.FadeIn());
    }
}
