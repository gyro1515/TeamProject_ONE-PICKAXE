using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIPause : BaseUI
{
    [Header("일시정지 설정")]
    [SerializeField] Button btnContinue;
    [SerializeField] Button btnOption;
    [SerializeField] Button btnReturnToMain;
    [SerializeField] AudioClip clickSoundClip;

    UIGameOption uiGameOption;
    PlayerInput.UIActions uiActions;
    private void Awake()
    {
        btnContinue.onClick.AddListener(CloseUI);
        btnOption.onClick.AddListener(OpenOption);
        btnReturnToMain.onClick.AddListener(ReturnToMain);
        uiGameOption = UIManager.Instance.GetUI<UIGameOption>();
        var input = new PlayerInput();   // 새 인스턴스
        uiActions = input.UI;            // UI 전용 액션맵
        uiActions.BackMenu.started += OnBackMenu;
        base.CloseUI(); // 사운드 출력 안되도록 부모 CloseUI사용
    }
    private void Start()
    {
    }
    private void OnEnable()
    {
        Time.timeScale = 0.0f;
        GameManager.Instance.Player?.Controller?.SetPlayerInput(false);
        uiActions.Enable();
    }
    private void OnDisable()
    {
        Time.timeScale = 1.0f;
        uiActions.Disable();
        GameManager.Instance.Player?.Controller?.SetPlayerInput(true);
    }
    public override void CloseUI()
    {
        base.CloseUI();
        if (clickSoundClip) SoundManager.PlayClip(clickSoundClip);
    }
    void OpenOption()
    {
        if (clickSoundClip) SoundManager.PlayClip(clickSoundClip);
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
        //SceneLoader.Instance.StartLoadScene(SceneState.StartScene); // 최종때는 이걸로
    }
    void OnBackMenu(InputAction.CallbackContext context)
    {
        //Debug.Log("OnBackMenu");
        if (uiGameOption.gameObject.activeSelf)
            uiGameOption.ReturnToStartMenu();
        else { CloseUI();} 
    }
}
