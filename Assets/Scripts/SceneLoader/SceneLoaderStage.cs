using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class SceneLoaderStage : MonoBehaviour
{
    // 테스트, 추후 플레이어/적 소환 등에 쓰일 수도...?
    [Header("스테이지 씬 설정")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] AudioClip bgSoundClip;
    private void Awake()
    {
        GameManager.Instance.Player = null;
        GameManager.Instance.Player = Instantiate(playerPrefab).GetComponent<Player>();
        if(bgSoundClip) SoundManager.Instance.ChangeBackGroundMusic(bgSoundClip);
    }
    private void Update()
    {
        // 테스트
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
        }*/
    }
}
