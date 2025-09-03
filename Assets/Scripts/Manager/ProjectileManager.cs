using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EProjectileType
{
    None, EnemyRangedArrow
}
public class ProjectileManager : SingletonMono<ProjectileManager>
{
    [Header("발사체 프리팹")]
    [SerializeField] Dictionary<EProjectileType, GameObject> projectilePrefabs = new Dictionary<EProjectileType, GameObject>(); // 안 보이네요...
    Dictionary<EProjectileType, List<GameObject>> projectilsObjectPool
        = new Dictionary<EProjectileType, List<GameObject>> ();// 프로젝타일 오브젝트 풀링용

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
    public GameObject GetObject(EProjectileType projectileType)
    {
        if (!projectilsObjectPool.ContainsKey(projectileType))
        {
            Debug.LogError($"프리팹 인덱스 {projectileType}에 대한 풀이 존재하지 않습니다.");
            return null;
        }

        
        foreach(var projectile in projectilsObjectPool[projectileType])
        {
            if (projectile.activeSelf) continue; // 활성화된 것은 스킵
            return projectile; 
        }

        GameObject obj = Instantiate(projectilePrefabs[projectileType], gameObject.transform);
        projectilsObjectPool[projectileType].Add(obj); // 풀에 추가
        return obj;
    }
}
