using System.Collections;
using UnityEngine;

public class Door : InteractableObject
{
    private bool isOpen = false;

    // Interaction() 메서드는 버튼이 아닌 다른 오브젝트와 닿았을 때 호출되므로,
    // 이 로직은 유지하거나 다른 상호작용 방식으로 변경해야 합니다.
    public override void Interaction(GameObject other)
    {

    }

    public void Toggle()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            Debug.Log("문이 열렸습니다. 오브젝트를 즉시 파괴합니다.");

            // 현재 스크립트가 붙어있는 오브젝트의 부모를 찾습니다.
            Transform parentTransform = transform.parent;

            // 부모가 존재하면 부모를 파괴합니다. 부모와 함께 모든 자식도 파괴됩니다.
            if (parentTransform != null)
            {
                Destroy(parentTransform.gameObject);
            }

        }
    }
}