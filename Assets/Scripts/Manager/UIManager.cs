using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;

public class UIManager : SingletonMono<UIManager>
{
    public const string UIPrefabPath = "Prefabs/UI/";

    private bool _isCleaning;
    private Dictionary<string, BaseUI> _uiDictionary = new Dictionary<string, BaseUI>();
    protected override void Awake()
    {
        base.Awake();
    }
    private void OnEnable()
    {
        // 씬 언로드 이벤트 구독
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    
    private void Start()
    {
        
    }
    private void OnDisable()
    {
        // 씬 언로드 이벤트 해제 (메모리 누수 방지)
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // ================================
    // UI 관리
    // ================================
    public void OpenUI<T>() where T : BaseUI
    {
        var ui = GetUI<T>();
        ui?.OpenUI();
    }

    public void CloseUI<T>() where T : BaseUI
    {
        if (IsExistUI<T>())
        {
            var ui = GetUI<T>();
            ui?.CloseUI();
        }
    }

    public T GetUI<T>() where T : BaseUI
    {
        if (_isCleaning) return null;

        string uiName = GetUIName<T>();

        BaseUI ui;
        if (IsExistUI<T>())
            ui = _uiDictionary[uiName];
        else
            ui = CreateUI<T>();

        return ui as T;
    }

    private T CreateUI<T>() where T : BaseUI
    {
        if (_isCleaning) return null;

        string uiName = GetUIName<T>();
        if (_uiDictionary.TryGetValue(uiName, out var prevUi) && prevUi != null)
        {
            Destroy(prevUi.gameObject);
            _uiDictionary.Remove(uiName);
        }

        // 1. 프리팹 로드
        string path = GetPath<T>();
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"[UIManager] Prefab not found: {path}");
            return null;
        }

        // 2. 인스턴스 생성
        GameObject go = Instantiate(prefab, gameObject.transform);

        // 3. 컴포넌트 획득
        T ui = go.GetComponent<T>();
        if (ui == null)
        {
            Debug.LogError($"[UIManager] Prefab has no component : {uiName}");
            Destroy(go);
            return null;
        }

        // 4. Dictionary 등록
        _uiDictionary[uiName] = ui;

        return ui;
    }

    public bool IsExistUI<T>() where T : BaseUI
    {
        string uiName = GetUIName<T>();
        return _uiDictionary.TryGetValue(uiName, out var ui) && ui != null;
    }


    // ================================
    // path 헬퍼
    // ================================
    private string GetPath<T>() where T : BaseUI
    {
        return UIPrefabPath + GetUIName<T>();
    }

    private string GetUIName<T>() where T : BaseUI
    {
        return typeof(T).Name;
    }


    // ================================
    // 리소스 정리
    // ================================
    private void OnSceneUnloaded(Scene scene)
    {
        //Debug.Log("OnSceneUnloaded 호출");
        CleanAllUIs();
        StartCoroutine(CoUnloadUnusedAssets());
    }

    private void CleanAllUIs()
    {
        if (_isCleaning) return;
        _isCleaning = true;

        try
        {
            foreach (var ui in _uiDictionary.Values)
            {
                if (ui == null) continue;
                // Close 프로세스 추가 가능
                Destroy(ui.gameObject);
            }
            _uiDictionary.Clear();
        }
        finally
        {
            _isCleaning = false;
        }
    }

    // UI 뿐만 아니라 전체 오브젝트 관리 시스템측면에서도 있으면 좋음
    private IEnumerator CoUnloadUnusedAssets()
    {
        yield return Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
