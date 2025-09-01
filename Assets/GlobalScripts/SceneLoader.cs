using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneState
{
    // 예시들
    None,
    Stage01,

}
// 씬 전환을 관리하는 스크립트, 게임 시작 시 자동으로 생성되며, 씬 전환을 담당
public class SceneLoader : SingletonMono<SceneLoader>
{
    // 여기에 씬들 한번에 다 추가
    private readonly Dictionary<SceneState, string> sceneNames = new()
    {
        // 예시들
        /*{ SceneState.TopDown,     "TopDownScene" },
        { SceneState.FlappyPlane, "FlappyPlaneScene" },
        { SceneState.TheStack,    "TheStackScene" }*/

        { SceneState.Stage01,   "Stage01" },
    };
    
    // 키 모아두기 예시
    public const string SelCharSKey = "SelectedCharacter";

    public static bool IsChange { get; private set; } = false; // 씬 전환 시 그 후 상호작용 작동 안하도록

    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] // 하이어아키 창에 게임오브젝트를 만들지 않아도 자동 생성
    private static void Init()
    {
        if (Instance != null)
        {
            SceneManager.sceneLoaded += Instance.OnSceneLoaded;
        }
    }
    protected override void Awake()
    {
        base.Awake();
    }
    static void Load(SceneState state)
    {
        IsChange = true; // 씬 전환 시작

        SceneManager.LoadScene(Instance.sceneNames[state]);
    }

    public static string GetSceneName(SceneState state)
    {
        return Instance.sceneNames.TryGetValue(state, out var name) ? name : null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        StartCoroutine(SceneLoaded());
    }
    static IEnumerator NextSceneSequence(SceneState nextScene)
    {
        yield return FadeManager.Instance.FadeOut(); // 페이드 아웃 끝나고
        Debug.Log("다음 씬으로");
        SceneLoader.Load(nextScene);
    }
    public void StartLoadScene(SceneState nextScene)
    {
        StartCoroutine(NextSceneSequence(nextScene));
    }
    static IEnumerator SceneLoaded()
    {
        yield return FadeManager.Instance.FadeIn(); // 페이드 인
        //Debug.Log("씬 전환 완료");
        IsChange = false;
    }
}