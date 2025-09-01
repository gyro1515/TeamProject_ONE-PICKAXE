using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class SceneLoaderStage2 : MonoBehaviour
{
    UIPause uiPause;

    private void Awake()
    {
        uiPause = UIManager.Instance.GetUI<UIPause>();
        uiPause.CloseUI();
    }
    private void Update()
    {
        // 테스트
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiPause.OpenUI();
        }
    }
}
