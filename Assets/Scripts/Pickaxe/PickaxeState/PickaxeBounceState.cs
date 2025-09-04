using UnityEngine;

public class PickaxeBounceState : PickaxeBaseState<ThrownPickaxeStateMachine>
{
    private Rigidbody2D rb;
    private Vector2 startPoint;
    private Vector2 endPoint;
    private float elapsedTime = 0f;

    private GameObject arrivalMarkInstance; // 곡괭이가 착지할 마커

    // BounceState에 진입할 때 충돌 정보를 전달받는 메서드
    public void SetBounceData(Vector2 start, Vector2 end)
    {
        startPoint = start;
        endPoint = end;
        elapsedTime = 0f;
    }

    public override void EnterState(ThrownPickaxeStateMachine stateMachine)
    {
        Debug.Log("곡괭이 상태: 튕김");

        // 물리 비활성화 (AnimationCurve로 제어)
        rb = stateMachine.ThrownPickaxeController.Rb2D;
        rb.isKinematic = true;
        rb.gravityScale = 0;

        // 바운스 상태 진입 시 착지 준비 플래그를 초기화
        stateMachine.ThrownPickaxeController.IsReadyToStick = false;

        // 착지 마커 설정
        arrivalMarkInstance = stateMachine.ThrownPickaxeController.ArrivalMark;

        // 부모-자식 관계를 끊어 독립적인 오브젝트로 만들어서 곡괭이가 움직여도 마커가 고정되도록 함
        arrivalMarkInstance.transform.SetParent(null);

        stateMachine.ThrownPickaxeController.ArrivalMark.SetActive(true);
        stateMachine.ThrownPickaxeController.ArrivalMark.transform.position = endPoint;
    }

    public override void UpdateState(ThrownPickaxeStateMachine stateMachine)
    {
        elapsedTime += Time.deltaTime;
        // 진행률 계산(0에서 시작해서 1에서 끝남)
        float progress = elapsedTime / stateMachine.ThrownPickaxeController.BounceMoveDuration;

        // 궤도 이동이 끝나고 착지 준비가 되었을 때만 상태 전환
        if (progress >= 1f && stateMachine.ThrownPickaxeController.IsReadyToStick)
        {
            // 목표 지점에 도달
            stateMachine.ThrownPickaxeController.transform.position = endPoint;

            // 캐치를 위한 플래그를 true로 설정하고 StuckState로 전환
            stateMachine.ThrownPickaxeController.WasBounced = true;
            stateMachine.ChangeState(stateMachine.StuckState);

            return;
        }

        // 높이 값 계산(AnimationCurve를 사용하여 궤도 계산)
        // Evaluate(progress)는 현재 progress 값에 해당하는 커브의 y값을 반환
        // progress가 0.5(경로의 중간)이면 y값은 1(최고 높이)
        // 결과적으로 curveHeight는 0 -> 1 -> 0 순서로 변하는 값
        float curveHeight = stateMachine.ThrownPickaxeController.BounceCurve.Evaluate(progress);

        
        // 직선 경로 계산 (Lerp)
        Vector2 newPosition = Vector2.Lerp(startPoint, endPoint, progress);

        // 최종 위치 계산 및 적용
        // 직선 위치 + (높이 비율 * 최대 높이) 공식을 통한 최종 포물선상의 위치
        newPosition.y += curveHeight * stateMachine.ThrownPickaxeController.BounceHeight;

        stateMachine.ThrownPickaxeController.transform.position = newPosition;
    }

    public override void HandleTrigger(ThrownPickaxeStateMachine stateMachine, Collider2D other)
    {
        // 박히는 타일과 충돌했는지 확인
        if (other.CompareTag("CanStuck"))
        {
            // 상태를 직접 바꾸지 않고, 플래그만 true로 설정
            stateMachine.ThrownPickaxeController.IsReadyToStick = true;
        }
    }

    public override void ExitState(ThrownPickaxeStateMachine stateMachine)
    {
        if (arrivalMarkInstance != null)
        {
            Object.Destroy(arrivalMarkInstance);
        }
    }

    public override void FixedUpdateState(ThrownPickaxeStateMachine stateMachine) { }
    public override void HandleCollision(ThrownPickaxeStateMachine stateMachine, Collision2D collision) { }
    public override void HandleInput(ThrownPickaxeStateMachine stateMachine) { }
}
