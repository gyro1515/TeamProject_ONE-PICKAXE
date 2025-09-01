using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderStart : MonoBehaviour
{
    UIStartMenu uiStartMenu;
    private void Awake()
    {
        uiStartMenu = UIManager.Instance.GetUI<UIStartMenu>();
    }
}
