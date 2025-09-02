using UnityEngine;

public class PickaxeStuckState : PickaxeBaseState<ThrownPickaxeStateMachine>
{
    private Rigidbody2D rb;
    private float retrieveHoldTime = 0f;

    public override void EnterState(ThrownPickaxeStateMachine stateMachine)
    {
        Debug.Log("곡괭이 상태: 박힌 상태");

        stateMachine.ThrownPickaxeController.StopThrowAnimation();

        // 물리 비활성화 (곡괭이를 정지시킴)
        rb = stateMachine.ThrownPickaxeController.Rb2D;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        // 위치 및 회전 조정
        AdjustPositionAndRotation(stateMachine);

        // TODO: 회수 게이지바 UI 초기화?
    }

    private void AdjustPositionAndRotation(ThrownPickaxeStateMachine stateMachine)
    {
        // ThrownPickaxeController에 저장된 마지막 Raycast 정보 가져오기
        RaycastHit2D lastHit = stateMachine.ThrownPickaxeController.LastHitInfo;

        if (lastHit.collider != null)
        {
            // 충돌 지점(contact point)과 법선 벡터(normal vector)를 사용
            Vector2 contactPoint = lastHit.point;
            Vector2 contactNormal = lastHit.normal;

            // 법선 벡터의 반대 방향을 사용해 곡괭이의 날이 충돌 지형을 파고들도록 함
            Vector2 flippedNormal = -contactNormal;

            // 곡괭이의 Z축 회전을 반전된 법선 벡터의 각도로 설정
            // Vector2.SignedAngle(Vector2.up, flippedNormal)은 곡괭이의 위(Y) 방향이
            // 충돌 면의 안쪽을 향하도록 회전
            float rotationAngle = Vector2.SignedAngle(Vector2.up, flippedNormal);
            stateMachine.ThrownPickaxeController.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

            // 플레이어 방향에 따라 박힌 곡괭이 좌우 반전
            Vector3 newScale = stateMachine.ThrownPickaxeController.transform.localScale;
            if (stateMachine.ThrownPickaxeController.isPlayerFacingRight)
            {
                // 플레이어가 오른쪽을 바라보고 있을 때
                newScale.x = -1f;
            }
            else
            {
                // 플레이어가 왼쪽을 바라보고 있을 때
                newScale.x = 1f;
            }
            stateMachine.ThrownPickaxeController.transform.localScale = newScale;

            // 곡괭이의 새 위치 계산
            // 충돌 지점에서 법선 벡터 방향으로 곡괭이의 StickingOffset 길이만큼 후퇴시켜 박힌 것처럼 보이게 함
            Vector2 newPosition = contactPoint + contactNormal * (-stateMachine.ThrownPickaxeController.StickingOffset);
            stateMachine.ThrownPickaxeController.transform.position = newPosition;
        }
    }

    public override void HandleTrigger(ThrownPickaxeStateMachine stateMachine, Collider2D other)
    {
        // 박힌 상태에서 다른 충돌을 무시하거나 회수 로직 처리
        if (other.CompareTag("Player"))
        {
            Debug.Log("근거리 회수 완료!");
            // 플레이어가 박힌 곡괭이에 닿았을 때 회수
            // 이 로직은 플레이어 컨트롤러에서 처리?
            // player.GetComponent<PickaxeController>().RetrieveThrownPickaxe();

            // 곡괭이 오브젝트 파괴
            Object.Destroy(stateMachine.ThrownPickaxeController.gameObject);
        }
    }

    public override void HandleInput(ThrownPickaxeStateMachine stateMachine)
    {
        // 원거리 회수 입력 처리
        if (Input.GetKey(KeyCode.R))
        {
            retrieveHoldTime += Time.deltaTime;

            // TODO: UI 게이지바 업데이트
            Debug.Log($"원거리 회수 충전 중: {retrieveHoldTime:F2}초 / {stateMachine.ThrownPickaxeController.RetrieveHoldDuration}초");

            if (retrieveHoldTime >= stateMachine.ThrownPickaxeController.RetrieveHoldDuration)
            {
                Debug.Log("원거리 회수 충전 완료!");

                // 충전 완료 후 회수 애니메이션 재생(상태 전환 후 재생하면 딜레이 발생)
                stateMachine.ThrownPickaxeController.PlayRetrieveAnimation();

                stateMachine.ChangeState(stateMachine.RetrieveState);
            }
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            // 키를 놓으면 충전 취소
            retrieveHoldTime = 0f;
            Debug.Log("원거리 회수 충전 취소!");
            // TODO: UI 게이지 초기화
        }
    }

    public override void UpdateState(ThrownPickaxeStateMachine stateMachine) { }
    public override void ExitState(ThrownPickaxeStateMachine stateMachine) { }
    public override void FixedUpdateState(ThrownPickaxeStateMachine stateMachine) { }
    public override void HandleCollision(ThrownPickaxeStateMachine stateMachine, Collision2D collision) { }
}
