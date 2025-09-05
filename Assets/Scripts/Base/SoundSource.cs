using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour
{
    private AudioSource _audioSource;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    public void Play(AudioClip clip, float soundEffectVolume, float soundEffectPitchVariance = 0)
    {
        gameObject.SetActive(true);
        CancelInvoke();
        _audioSource.Stop();
        _audioSource.clip = clip; ;
        _audioSource.volume = soundEffectVolume;
        _audioSource.Play();
        // 랜덤 속도할 거면 추가하기
        _audioSource.pitch = 1f + Random.Range(-soundEffectPitchVariance, soundEffectPitchVariance);

        Invoke("Disable", clip.length + 2);
    }
    public void Stop()
    {
        CancelInvoke();
        _audioSource.Stop();
        gameObject.SetActive(false);
    }
    public void Disable()
    {
        _audioSource.Stop();
        gameObject.SetActive(false);
    }
}
