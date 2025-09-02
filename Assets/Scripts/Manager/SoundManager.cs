using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMono<SoundManager>
{
    [field: Header("사운드 설정")]
    [field: SerializeField][field: Range(0f, 1f)] public float SoundEffectVolume { get; set; } = 1f;
    [field: SerializeField][field: Range(0f, 1f)] public float MusicVolume { get; set; } = 1f;
    private AudioSource musicAudioSource;
    public AudioClip musicClip;

    //public SoundSource soundSourcePrefab;


}
