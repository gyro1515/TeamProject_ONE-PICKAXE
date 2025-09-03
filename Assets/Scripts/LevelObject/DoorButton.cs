using UnityEngine;

public class DoorButton : InteractableObject
{
    // 버튼과 연결된 문 오브젝트
    public Door targetDoor;

    // 상호작용할 레이어를 설정합니다.
    public LayerMask validInteractionLayers;

    // Interaction 메서드를 오버라이드. 충돌한 GameObject를 받습니다.
    public override void Interaction(GameObject other)
    {
        // 충돌한 오브젝트의 레이어가 유효한 레이어에 포함되는지 확인
        if (IsInLayerMask(other.layer, validInteractionLayers))
        {
            Debug.Log("DoorButton: 유효한 레이어의 오브젝트가 닿았습니다. 문을 엽니다.");
            targetDoor.Toggle();
        }
        else
        {
            Debug.LogWarning("DoorButton: 유효하지 않은 레이어의 오브젝트가 닿았습니다. 문이 열리지 않습니다.");
        }
    }

    // 레이어가 LayerMask에 포함되는지 확인하는 헬퍼 메서드
    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        // 비트 연산을 통해 레이어가 LayerMask에 포함되어 있는지 확인
        return ((layerMask.value & (1 << layer)) > 0);
    }
}