using UnityEngine;

public class PickaxeStuckState : PickaxeBaseState<ThrownPickaxeStateMachine>
{
    public override void EnterState(ThrownPickaxeStateMachine stateMachine)
    {
        Debug.Log("곡괭이 상태: 박힌 상태");

        stateMachine.ThrownPickaxeController.StopThrowAnimation();

        // 물리 비활성화 (곡괭이를 정지시킴)
        stateMachine.ThrownPickaxeController.Rb2D.gravityScale = 0;
        stateMachine.ThrownPickaxeController.Rb2D.velocity = Vector2.zero;
        stateMachine.ThrownPickaxeController.Rb2D.isKinematic = true;

        // 위치 및 회전 조정
        AdjustPositionAndRotation(stateMachine);
    }

    private void AdjustPositionAndRotation(ThrownPickaxeStateMachine stateMachine)
    {
        // ThrownPickaxeController에 저장된 마지막 충돌 정보 가져오기
        Collision2D lastCollision = stateMachine.ThrownPickaxeController.LastCollision;

        if (lastCollision != null && lastCollision.contacts.Length > 0)
        {
            // 충돌 지점(contact point)과 법선 벡터(normal vector)를 사용
            Vector2 contactPoint = lastCollision.contacts[0].point;
            Vector2 contactNormal = lastCollision.contacts[0].normal;

            // 법선 벡터의 반대 방향을 사용해 곡괭이의 날이 충돌 지형을 파고들도록 함
            Vector2 flippedNormal = -contactNormal;

            // 곡괭이의 Z축 회전을 반전된 법선 벡터의 각도로 설정
            // Vector2.SignedAngle(Vector2.up, flippedNormal)은 곡괭이의 위(Y) 방향이
            // 충돌 면의 안쪽을 향하도록 회전시켜 줍니다.
            float rotationAngle = Vector2.SignedAngle(Vector2.up, flippedNormal);
            stateMachine.ThrownPickaxeController.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

            // 곡괭이 이미지 파일의 기본 45도 기울기를 고려하여 조정
            // 이제 곡괭이의 날이 충돌 면을 향하게 되었으므로, 45도 회전을 더해주면 됩니다.
            stateMachine.ThrownPickaxeController.transform.Rotate(0, 0, 45);

            // 곡괭이의 새 위치를 계산
            // 충돌 지점에서 법선 벡터 방향으로, 곡괭이의 절반 길이만큼 후퇴시켜 박힌 것처럼 보이게 함
            Vector2 newPosition = contactPoint + contactNormal * stateMachine.ThrownPickaxeController.StickingOffset;
            stateMachine.ThrownPickaxeController.transform.position = newPosition;
        }
    }

    public override void HandleTrigger(ThrownPickaxeStateMachine stateMachine, Collider2D other)
    {
        // 박힌 상태에서 다른 충돌을 무시하거나 회수 로직 처리
        if (other.CompareTag("Player"))
        {
            // 플레이어가 박힌 곡괭이에 닿았을 때 회수
            // 이 로직은 플레이어 컨트롤러에서 처리?
        }
    }

    public override void ExitState(ThrownPickaxeStateMachine stateMachine)
    {
        throw new System.NotImplementedException();
    }

    public override void FixedUpdateState(ThrownPickaxeStateMachine stateMachine)
    {
        throw new System.NotImplementedException();
    }

    public override void HandleCollision(ThrownPickaxeStateMachine stateMachine, Collision2D collision)
    {
        throw new System.NotImplementedException();
    }

    public override void HandleInput(ThrownPickaxeStateMachine stateMachine)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(ThrownPickaxeStateMachine stateMachine)
    {
        throw new System.NotImplementedException();
    }
}
