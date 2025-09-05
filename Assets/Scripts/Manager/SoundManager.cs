using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SoundManager : SingletonMono<SoundManager>
{
    // 인스펙터 창에 드러난 것들은 다 확인용입니다.
    // ***자동으로 생성돼서, 인스펙터 창에서 설정 못합니다***
    [Header("사운드 설정")]
    [SerializeField][Range(0f, 1f)] float soundEffectVolume = 1f;
    [SerializeField][Range(0f, 1f)] float musicVolume = 1f;

    public float SoundEffectVolume { get { return soundEffectVolume; }
        set { soundEffectVolume = value; } }
    public float MusicVolume { get { return musicVolume; } 
        set { musicVolume = value; musicAudioSource.volume = musicVolume; } }
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
        musicAudioSource.volume = musicVolume;
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

    // 굳이 static이어야 하나 생각하지만 SoundManager.Instance보단 SoundManager.이 짧으니...
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
        SoundSource obj = Instantiate(Instance.soundSourcePrefab, Instance.gameObject.transform);
        obj.Play(clip, Instance.SoundEffectVolume);
        Instance.soundSources.Add(obj);
    }
    public static SoundSource PlayClipWithGetSoundSource(AudioClip clip)
    {
        for (int i = 0; i < Instance.soundSources.Count; i++)
        {
            if (Instance.soundSources[i].gameObject.activeSelf) continue;
            // 비활성화 찾아서 재생시키기
            SoundSource obj1 = Instance.soundSources[i];
            obj1.Play(clip, Instance.SoundEffectVolume);
            return obj1;
        }

        // 재생할 게 없다면 생성하기
        SoundSource obj2 = Instantiate(Instance.soundSourcePrefab, Instance.gameObject.transform);
        obj2.Play(clip, Instance.SoundEffectVolume);
        Instance.soundSources.Add(obj2);
        return obj2;
    }
    
}
