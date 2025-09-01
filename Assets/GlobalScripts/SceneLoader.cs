using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneState
{
    // 예시들
    /*None,
    Metaverse,
    TopDown,
    FlappyPlane,
    TheStack*/
}
// 씬 전환을 관리하는 스크립트, 게임 시작 시 자동으로 생성되며, 씬 전환을 담당
public class SceneLoader : MonoBehaviour 
{
    private static SceneLoader instance;

    // 여기에 씬들 한번에 다 추가
    private readonly Dictionary<SceneState, string> sceneNames = new()
    {
        // 예시들
        /*{ SceneState.Metaverse,   "MetaverseScene" },
        { SceneState.TopDown,     "TopDownScene" },
        { SceneState.FlappyPlane, "FlappyPlaneScene" },
        { SceneState.TheStack,    "TheStackScene" }*/
    };
    
    // 키 모아두기 예시
    public const string SelCharSKey = "SelectedCharacter";

    public static bool IsChange { get; private set; } = false; // 씬 전환 시 그 후 상호작용 작동 안하도록

    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] // 하이어아키 창에 게임오브젝트를 만들지 않아도 자동 생성
    private static void Init()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("SceneLoader");
            // FadeManager와 다른 진행 방식, 두 방식 다 이해 필요
            go.AddComponent<SceneLoader>(); // 컴포넌트 추가 시 Awake()가 호출되어 instance가 설정됨
        }
    }
    private void Awake()
    {
        if (instance == null) // 처음 AddComponent로 호출 시 아직 null, 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            // BeforeSceneLoad에서도 설정 가능하나 안정적으로 Awake()에서 설정
            // 또한 OnSceneLoaded은 static이 아니므로, 현재 구조로는 Initialize()에서 설정 불가
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 중복 방지
        }
    }
    public static void Load(SceneState state)
    {
        IsChange = true; // 씬 전환 시작
        SceneManager.LoadScene(instance.sceneNames[state]);
    }

    public static string GetSceneName(SceneState state)
    {
        return instance.sceneNames.TryGetValue(state, out var name) ? name : null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        //Debug.Log("씬 전환 완료");
        IsChange = false;
    }
    public static IEnumerator NextSceneSequence(SceneState nextScene)
    {
        yield return FadeManager.Instance.FadeOut(); // 페이드 아웃 끝나고
        Debug.Log("다음 씬으로");
        SceneLoader.Load(nextScene);
    }
}