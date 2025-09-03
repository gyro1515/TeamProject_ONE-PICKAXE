using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EProjectileType
{
    None, EnemyRangedArrow
}
public class ProjectileManager : SingletonMono<ProjectileManager>
{
    [Header("발사체 프리팹")]
    [SerializeField] Dictionary<EProjectileType, GameObject> projectilePrefabs = new Dictionary<EProjectileType, GameObject>(); // 안 보이네요...
    Dictionary<EProjectileType, List<GameObject>> projectilsObjectPool
        = new Dictionary<EProjectileType, List<GameObject>>();// 프로젝타일 오브젝트 풀링용

    bool _isCleaning = false;

    protected override void Awake()
    {
        base.Awake();

        foreach (EProjectileType type in Enum.GetValues(typeof(EProjectileType)))
        {
            if (type == EProjectileType.None) continue;
            projectilsObjectPool[type] = new List<GameObject>();
            projectilePrefabs[type] = Resources.Load<GameObject>("Prefabs/Projectile/" + type.ToString());
        }
    }
    private void OnEnable()
    {
        // 씬 언로드 이벤트 구독
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    private void OnDisable()
    {
        // 씬 언로드 이벤트 해제 (메모리 누수 방지)
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
    public GameObject GetObject(EProjectileType projectileType)
    {
        if (!projectilsObjectPool.ContainsKey(projectileType))
        {
            Debug.LogError($"프리팹 인덱스 {projectileType}에 대한 풀이 존재하지 않습니다.");
            return null;
        }


        foreach (var projectile in projectilsObjectPool[projectileType])
        {
            if (projectile.activeSelf) continue; // 활성화된 것은 스킵
            return projectile;
        }

        GameObject obj = Instantiate(projectilePrefabs[projectileType], gameObject.transform);
        projectilsObjectPool[projectileType].Add(obj); // 풀에 추가
        return obj;
    }
    // 씬 전환 시 리소스 정리
    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("SoundManager: OnSceneUnloaded 호출");
        CleanAllProjectiles();
        StartCoroutine(CoUnloadUnusedAssets());
    }
    void CleanAllProjectiles()
    {
        if (_isCleaning) return;
        _isCleaning = true;

        try
        {
            foreach (var projecType in projectilsObjectPool)
            {
                foreach(var projec in projecType.Value)
                {
                    if (projec == null) continue;

                    Destroy(projec.gameObject);
                }
                projecType.Value.Clear();
            }
        }
        finally
        {
            _isCleaning = false;
        }
    }
    private IEnumerator CoUnloadUnusedAssets()
    {
        yield return Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
