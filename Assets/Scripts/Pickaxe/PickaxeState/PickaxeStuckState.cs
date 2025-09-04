using UnityEngine;

public class PickaxeStuckState : PickaxeBaseState<ThrownPickaxeStateMachine>
{
    private Rigidbody2D rb;
    private float retrieveHoldTime = 0f; // 원거리 회수 충전 시간
    private float catchTimer = 0f; // 캐치 타이머

    // 회수 UI
    UIRecallPickaxe uIRecallPickaxe;

    public override void EnterState(ThrownPickaxeStateMachine stateMachine)
    {
        Debug.Log("곡괭이 상태: 박힌 상태");

        stateMachine.ThrownPickaxeController.StopThrowAnimation();

        // 물리 비활성화 (곡괭이를 정지시킴)
        rb = stateMachine.ThrownPickaxeController.Rb2D;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        // Bounce 상태에서 전환되었는지 확인하고 캐치 타이머 설정
        if (stateMachine.ThrownPickaxeController.WasBounced)
        {
            Debug.Log("캐치 가능 시간 시작!");
            catchTimer = stateMachine.ThrownPickaxeController.CatchTime;
            stateMachine.ThrownPickaxeController.WasBounced = false; // 플래그 초기화
        }
        else
        {
            catchTimer = 0f; // 튕겨서 온 게 아니면 캐치 시간 없음
        }

        // 위치 및 회전 조정
        AdjustPositionAndRotation(stateMachine);

        // 회수 게이지바 UI 초기화
        if(uIRecallPickaxe == null)
        {
            uIRecallPickaxe = GameManager.Instance.Player.UIRecallPickaxe;
        }

        uIRecallPickaxe?.OpenUI();
        uIRecallPickaxe?.SetGaugeBarValue(retrieveHoldTime, stateMachine.ThrownPickaxeController.RetrieveHoldDuration);
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
            if (stateMachine.ThrownPickaxeController.IsPlayerFacingRight)
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
            // 캐치 성공 여부 판정
            bool isCatchSuccess = catchTimer > 0f;

            // Owner(EquippedPickaxeController)에게 회수 신호 보내기
            if (stateMachine.ThrownPickaxeController.Owner != null)
            {
                stateMachine.ThrownPickaxeController.Owner.RetrievePickaxe(isCatchSuccess);
            }

            // 곡괭이 오브젝트 파괴
            Object.Destroy(stateMachine.ThrownPickaxeController.gameObject);
        }
    }

    public override void HandleInput(ThrownPickaxeStateMachine stateMachine)
    {
        
        // 원거리 회수 입력 처리
        if (stateMachine.ThrownPickaxeController.IsRetrieveHeld)
        {
            retrieveHoldTime += Time.deltaTime;
            uIRecallPickaxe?.SetGaugeBarValue(retrieveHoldTime, stateMachine.ThrownPickaxeController.RetrieveHoldDuration);
            // TODO: UI 게이지바 업데이트
            Debug.Log($"원거리 회수 충전 중: {retrieveHoldTime:F2}초 / {stateMachine.ThrownPickaxeController.RetrieveHoldDuration}초");

            if (retrieveHoldTime >= stateMachine.ThrownPickaxeController.RetrieveHoldDuration)
            {
                Debug.Log("원거리 회수 충전 완료!");

                stateMachine.ChangeState(stateMachine.RetrieveState);
                // 회수 UI 끄기? -> 확인 부탁드립니다.
                uIRecallPickaxe?.CloseUI();
            }
        }
        else
        {
            // 키를 놓으면 충전 취소
            if(retrieveHoldTime > 0f)
            {
                retrieveHoldTime = 0f;
                Debug.Log("원거리 회수 충전 취소!");
                // TODO: UI 게이지 초기화
                uIRecallPickaxe?.SetGaugeBarValue(retrieveHoldTime, stateMachine.ThrownPickaxeController.RetrieveHoldDuration);
            }
        }
    }

    public override void UpdateState(ThrownPickaxeStateMachine stateMachine)
    {
        // 캐치 타이머 감소 로직
        if (catchTimer > 0)
        {
            catchTimer -= Time.deltaTime;
            if (catchTimer <= 0)
            {
                Debug.Log("캐치 가능 시간 종료!");
            }
        }
    }
    public override void ExitState(ThrownPickaxeStateMachine stateMachine) { }
    public override void FixedUpdateState(ThrownPickaxeStateMachine stateMachine) { }
    public override void HandleCollision(ThrownPickaxeStateMachine stateMachine, Collision2D collision) { }
}
