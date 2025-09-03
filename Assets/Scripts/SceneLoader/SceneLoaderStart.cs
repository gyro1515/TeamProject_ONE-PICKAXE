using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderStart : MonoBehaviour
{
    [Header("스타트 씬 설정")]
    [SerializeField] AudioClip bgSoundClip;
    UIStartMenu uiStartMenu;
    private void Awake()
    {
        uiStartMenu = UIManager.Instance.GetUI<UIStartMenu>();
        if(bgSoundClip) SoundManager.Instance.ChangeBackGroundMusic(bgSoundClip);
    }
}
