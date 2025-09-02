using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class SceneLoaderStage2 : MonoBehaviour
{
    // 테스트, 추후 플레이어/적 소환 등에 쓰일 수도...?
    [SerializeField] GameObject playerPrefab;

    private void Awake()
    {
        Instantiate(playerPrefab);
        
    }
    private void Update()
    {
        // 테스트
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
        }*/
    }
}
