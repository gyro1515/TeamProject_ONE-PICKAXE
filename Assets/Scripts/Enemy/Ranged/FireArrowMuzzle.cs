using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrowMuzzle : MonoBehaviour
{
    float xPos = 1f;
    private void Awake()
    {
        xPos = gameObject.transform.localPosition.x; // 초기 x값
    }
    public void SetFlipX(bool isFlipX)
    {
        if (isFlipX)
        {
            gameObject.transform.localPosition = new Vector3(-xPos, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
        }
        else gameObject.transform.localPosition = new Vector3(xPos, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
    }
}
