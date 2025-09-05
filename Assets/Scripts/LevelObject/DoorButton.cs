using UnityEngine;

public class DoorButton : InteractableObject
{
    [SerializeField] protected AudioClip interactionSound;
    public Door targetDoor;
    public LayerMask validInteractionLayers;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite activatedSprite;

    private void Awake()
    {
        // 1. 이 스크립트가 부착된 오브젝트의 부모에서 SpriteRenderer를 찾습니다.
        if (transform.parent != null)
        {
            spriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
        }
        else // 부모가 없다면, 현재 오브젝트에서 찾습니다.
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            Debug.LogError("DoorButton: SpriteRenderer 컴포넌트를 찾을 수 없습니다. 버튼 오브젝트나 그 부모에 SpriteRenderer가 있는지 확인하세요.");
        }
    }

    public override void Interaction(GameObject other)
    {
        if (IsInLayerMask(other.layer, validInteractionLayers))
        {
            if (interactionSound) SoundManager.PlayClip(interactionSound);
            Debug.Log("DoorButton: 유효한 레이어의 오브젝트가 닿았습니다. 문을 엽니다.");

            if (spriteRenderer != null && activatedSprite != null)
            {
                spriteRenderer.sprite = activatedSprite;
                Debug.Log("DoorButton: 버튼 이미지를 Lever_1로 변경합니다.");
            }

            targetDoor.Toggle();
        }
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << layer)) > 0);
    }
}