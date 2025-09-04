using UnityEngine;

public class DoorButton : InteractableObject
{
    // 버튼과 연결된 문 오브젝트
    public Door targetDoor;

    // 상호작용할 레이어를 설정합니다.
    public LayerMask validInteractionLayers;

    public override void Interaction(GameObject other)
    {
        if (IsInLayerMask(other.layer, validInteractionLayers))
        {
            Debug.Log("DoorButton: 유효한 레이어의 오브젝트가 닿았습니다. 문을 엽니다.");
            targetDoor.Toggle();

            // 여기에 targetDoor의 부모를 설정하는 코드를 추가합니다.
            // targetDoor의 Transform을 이 스크립트가 붙어있는 오브젝트(this.transform)의 자식으로 설정합니다.
            
        }
    }

    // 이 메서드는 그대로 유지
    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << layer)) > 0);
    }
}