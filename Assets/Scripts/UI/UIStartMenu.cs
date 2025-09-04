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
    [SerializeField] AudioClip clickSoundClip;


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
        if(clickSoundClip) SoundManager.PlayClip(clickSoundClip);

        SceneLoader.Instance.StartLoadScene(SceneState.Stage1WonJinTest);
        //SceneLoader.Instance.StartLoadScene(SceneState.Stage1); // 최종때는 이걸로
    }
    void ExitGame()
    {
        if (clickSoundClip) SoundManager.PlayClip(clickSoundClip);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
    void OpenOption()
    {
        if (clickSoundClip) SoundManager.PlayClip(clickSoundClip);
        StartCoroutine(StartOpenUIGameOption());
    }
    IEnumerator StartOpenUIGameOption()
    {
        yield return FadeManager.Instance.FadeOut();
        uiGameOption.OpenUI();
        StartCoroutine(FadeManager.Instance.FadeIn());
    }
}
