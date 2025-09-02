using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableObject
{
    private bool isOpen = false;

    public override void Interaction()
    {
        // 문을 여는 로직
        Open();
    }

    // 버튼이 호출할 수 있도록 별도의 메서드 구현
    public void Toggle()
    {
        isOpen = !isOpen;
        // 문 상태에 따른 애니메이션 또는 위치 변경 로직
        if (isOpen)
        {
            // 문을 여는 애니메이션 또는 위치 변경
        }
        else
        {
            // 문을 닫는 애니메이션 또는 위치 변경
        }
    }

    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
            // 문을 여는 애니메이션 또는 위치 변경
        }
    }
}