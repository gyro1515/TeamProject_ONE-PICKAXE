using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SoundManager : SingletonMono<SoundManager>
{
    // 인스펙터 창에 드러난 것들은 다 확인용입니다.
    // ***자동으로 생성돼서, 인스펙터 창에서 설정 못합니다***
    [field: Header("사운드 설정")]
    [field: SerializeField][field: Range(0f, 1f)] public float SoundEffectVolume { get; set; } = 1f;
    [field: SerializeField][field: Range(0f, 1f)] public float MusicVolume { get; set; } = 1f;
    private AudioSource musicAudioSource;
    [Header("배경 음악")]
    [SerializeField] AudioClip musicClip;
    [Header("효과음용 프리팹")]
    [SerializeField] SoundSource soundSourcePrefab;
    List<SoundSource> soundSources = new List<SoundSource>(); // 사운드 소스 오브젝트 풀

    protected override void Awake()
    {
        base.Awake();
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.volume = MusicVolume;
        musicAudioSource.loop = true;
        soundSourcePrefab = Resources.Load<GameObject>("Prefabs/Sound/SoundSource").GetComponent<SoundSource>();
    }
    private void Start()
    {
        //아마 다른 곳에서 호출할 듯합니다. 씬로더라든지
        //ChangeBackGroundMusic(musicClip);
    }

    public void ChangeBackGroundMusic(AudioClip clip)
    {
        musicAudioSource.Stop();
        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    public static void PlayClip(AudioClip clip)
    {
        for(int i = 0; i < Instance.soundSources.Count; i++)
        {
            if (Instance.soundSources[i].gameObject.activeSelf) continue;
            // 비활성화 찾아서 재생시키기
            Instance.soundSources[i].Play(clip, Instance.SoundEffectVolume);
            return;
        }

        // 재생할 게 없다면 생성하기
        SoundSource obj = Instantiate(Instance.soundSourcePrefab);
        obj.Play(clip, Instance.SoundEffectVolume);
        Instance.soundSources.Add(obj);
    }
}
