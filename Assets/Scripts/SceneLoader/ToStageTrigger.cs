using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToStageTrigger : MonoBehaviour
{
    [SerializeField] SceneState sceneState = SceneState.None;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (sceneState == SceneState.None) return;
        SceneLoader.Instance.StartLoadScene(sceneState);
    }
}
