using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    [Header("시작 메뉴 설정")]
    [SerializeField] Button gameStartBtn;
    [SerializeField] Button gameOptionBtn;
    [SerializeField] Button gameEndBtn;

    private void Awake()
    {
        gameStartBtn.onClick.AddListener(GameStart);
        gameOptionBtn.onClick.AddListener(ExitGame);
        gameEndBtn.onClick.AddListener(ExitGame);
    }
    void GameStart()
    {
        SceneLoader.Instance.StartLoadScene(SceneState.Stage01);
    }
    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
