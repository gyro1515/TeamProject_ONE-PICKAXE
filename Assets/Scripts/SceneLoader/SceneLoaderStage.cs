using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class SceneLoaderStage : MonoBehaviour
{
    // 테스트, 추후 플레이어/적 소환 등에 쓰일 수도...?
    [Header("스테이지 씬 설정")]
    [SerializeField] int stageNum = 999;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] AudioClip bgSoundClip;
    [SerializeField] Vector3 playerSpawnPos;
    private void Awake()
    {
        GameManager.Instance.Player = null;
        GameManager.Instance.Player = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity).GetComponent<Player>();
        if(bgSoundClip) SoundManager.Instance.ChangeBackGroundMusic(bgSoundClip);
    }
    private void Start()
    {
        GameManager.Instance.Player.HUD.SetStageText(stageNum);
    }
    private void Update()
    {
        // 테스트
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
        }*/
    }
}
