using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private AudioClip footstepSFX;
    [SerializeField] PlayerController playerController;

    public void OnPlayFootStepSFX()
    {
        if(footstepSFX && playerController && playerController.IsGrounded)
        {
            SoundManager.PlayClip(footstepSFX);
        }
    }
}
