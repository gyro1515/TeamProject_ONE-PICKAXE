using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private AudioClip footstepSFX;

    public void OnPlayFootStepSFX()
    {
        if(footstepSFX)
        {
            SoundManager.PlayClip(footstepSFX);
        }
    }
}
