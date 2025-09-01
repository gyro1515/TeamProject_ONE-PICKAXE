using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManage : SingletonMono<SoundManage>
{
    [field: Header("사운드 설정")]
    [field: SerializeField][field: Range(0f, 1f)] public float SoundEffectVolume { get; set; }
    [field: SerializeField][field: Range(0f, 1f)] public float MusicVolume { get; set; }
    private AudioSource musicAudioSource;
    public AudioClip musicClip;

    //public SoundSource soundSourcePrefab;


}
