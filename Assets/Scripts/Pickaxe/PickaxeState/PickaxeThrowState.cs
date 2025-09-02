using UnityEngine;

public class PickaxeThrowState : PickaxeBaseState
{
    public override void EnterState(PickaxeStateMachine stateMachine)
    {
        Debug.Log("곡괭이 상태: 던지기");

        // 착용한 곡괭이 비활성화
        stateMachine.PickaxeController.SetEquippedPickaxeActive(false);

        // 곡괭이 투척 및 생성/설정
        ThrowPickaxe(stateMachine);
    }

    private void ThrowPickaxe(PickaxeStateMachine stateMachine)
    {
        // 마우스 커서의 월드 좌표
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 플레이어 위치
        Vector2 playerPos = stateMachine.PickaxeController.transform.position;

        // 플레이어와 마우스 커서 간의 방향 벡터 계산
        Vector2 throwDirection = (mousePos - playerPos).normalized;

        // 곡괭이가 생성될 위치를 계산(플레이어로부터 ThrowRadius만큼 떨어진 지점)
        Vector2 spawnPosition = playerPos + throwDirection * stateMachine.PickaxeController.ThrowRadius;

        // 투척 곡괭이 인스턴스화
        GameObject thrownPickaxeObject = GameObject.Instantiate(
            stateMachine.PickaxeController.ThrowablePickaxePrefab,
            spawnPosition,
            Quaternion.identity
        );

        // 투척 곡괭이 컨트롤러 저장
        ThrownPickaxeController thrownPickaxeController = thrownPickaxeObject.GetComponent<ThrownPickaxeController>();

        // 곡괭이에 힘을 가해 투척
        Rigidbody2D rb = thrownPickaxeObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1.0f; // 포물선 궤적을 위해 중력 적용
            rb.velocity = throwDirection * stateMachine.PickaxeController.ThrowForce;
        }

        // 곡괭이에 회전 애니메이션 적용
        thrownPickaxeController.PlayThrowAnimation();
    }

    public override void ExitState(PickaxeStateMachine stateMachine)
    {
    }

    public override void FixedUpdateState(PickaxeStateMachine stateMachine)
    {
    }

    public override void HandleCollision(PickaxeStateMachine stateMachine, Collision2D collision)
    {
    }

    public override void HandleInput(PickaxeStateMachine stateMachine)
    {
    }

    public override void HandleTrigger(PickaxeStateMachine stateMachine, Collider2D other)
    {
    }

    public override void UpdateState(PickaxeStateMachine stateMachine)
    {
    }
}
