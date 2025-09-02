public class DoorButton : InteractableObject
{
    // 버튼과 연결된 문 오브젝트
    public Door targetDoor;

    public override void Interaction()
    {
        // 토글 방식으로 문 열기/닫기
        targetDoor.Toggle();
    }
}
