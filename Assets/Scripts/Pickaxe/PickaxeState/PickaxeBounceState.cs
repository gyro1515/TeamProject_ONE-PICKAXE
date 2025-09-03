using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class PickaxeBounceState : PickaxeBaseState<ThrownPickaxeStateMachine>
{
    private Rigidbody2D rb;
    private Vector2 startPoint;
    private Vector2 endPoint;
    private float elapsedTime = 0f;

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
    }

    public override void UpdateState(ThrownPickaxeStateMachine stateMachine)
    {
        elapsedTime += Time.deltaTime;
        // 진행률 계산(0에서 시작해서 1에서 끝남)
        float progress = elapsedTime / stateMachine.ThrownPickaxeController.BounceMoveDuration;

        if (progress >= 1f)
        {
            // 목표 지점에 도달
            stateMachine.ThrownPickaxeController.transform.position = endPoint;

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

    public override void ExitState(ThrownPickaxeStateMachine stateMachine)
    {
        
    }

    public override void FixedUpdateState(ThrownPickaxeStateMachine stateMachine)
    {
        
    }

    public override void HandleCollision(ThrownPickaxeStateMachine stateMachine, Collision2D collision)
    {
        
    }

    public override void HandleInput(ThrownPickaxeStateMachine stateMachine)
    {
        
    }

    public override void HandleTrigger(ThrownPickaxeStateMachine stateMachine, Collider2D other)
    {
        // 박히는 타일과 충돌했는지 확인
        if (other.CompareTag("CanStuck"))
        {
            // 곡괭이의 속도 방향으로 레이캐스트를 쏴서 충돌 정보를 얻습니다.
            Vector2 rayDirection = stateMachine.ThrownPickaxeController.Rb2D.velocity.normalized;
            float rayDistance = 0.5f; // 곡괭이 크기에 맞춰 적절한 값 설정

            var lastHitInfo = Physics2D.Raycast(stateMachine.ThrownPickaxeController.transform.position, rayDirection, rayDistance, other.gameObject.layer);
            stateMachine.ThrownPickaxeController.SetLastHitInfo(lastHitInfo);

            stateMachine.ChangeState(stateMachine.StuckState);
        }
        // 튕기는 오브젝트와 충돌했는지 확인? or 박히는 오브젝트가 아니면 모두 튕기는 오브젝트로 간주?
        else if (other.CompareTag("CanBounce"))
        {
            // 충돌 지점을 추정하여 BounceState로 전달
            Vector2 hitPoint = stateMachine.ThrownPickaxeController.transform.position;
            stateMachine.ThrownPickaxeController.Bounce(hitPoint, stateMachine.ThrownPickaxeController.PlayerTransform);
        }
        // 기타 오브젝트(적, 플레이어 등)와의 충돌 처리
        else
        {
            // 이외의 충돌은 여기서 처리하거나 BouncingState로 전환
        }
    }
}
