using UnityEngine;

public class PickaxeThrowState : PickaxeBaseState<EquippedPickaxeStateMachine>
{
    public override void EnterState(EquippedPickaxeStateMachine stateMachine)
    {
        Debug.Log("곡괭이 상태: 던지기");

        // 착용한 곡괭이 비활성화
        stateMachine.EquippedPickaxeController.SetEquippedPickaxeActive(false);

        // 곡괭이 투척 및 생성/설정
        ThrowPickaxe(stateMachine);
    }

    private void ThrowPickaxe(EquippedPickaxeStateMachine stateMachine)
    {
        // 마우스 커서의 월드 좌표
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 플레이어 위치
        Vector2 playerPos = stateMachine.EquippedPickaxeController.transform.position;

        // 플레이어와 마우스 커서 간의 방향 벡터 계산
        Vector2 throwDirection = (mousePos - playerPos).normalized;

        // 곡괭이가 생성될 위치를 계산(플레이어로부터 ThrowRadius만큼 떨어진 지점)
        Vector2 spawnPosition = playerPos + throwDirection * stateMachine.EquippedPickaxeController.ThrowRadius;

        // 투척 곡괭이 인스턴스화
        GameObject thrownPickaxeObject = GameObject.Instantiate(
            stateMachine.EquippedPickaxeController.ThrowablePickaxePrefab,
            spawnPosition,
            Quaternion.identity
        );

        // 투척 곡괭이 컨트롤러 저장
        ThrownPickaxeController thrownPickaxeController = thrownPickaxeObject.GetComponent<ThrownPickaxeController>();
        if (thrownPickaxeController != null)
        {
            // thrownPickaxeController에 플레이어의 방향 저장
            bool playerIsFacingRight = stateMachine.EquippedPickaxeController.transform.parent.localScale.x > 0;
            thrownPickaxeController.InitializeThrownPickaxe(playerIsFacingRight);
        }

        // 곡괭이에 힘을 가해 투척
        Rigidbody2D rb = thrownPickaxeObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1.0f; // 포물선 궤적을 위해 중력 적용
            rb.velocity = throwDirection * stateMachine.EquippedPickaxeController.ThrowForce;
        }

        // 곡괭이에 회전 애니메이션 적용
        thrownPickaxeController.PlayThrowAnimation();
    }

    public override void ExitState(EquippedPickaxeStateMachine stateMachine) { }

    public override void FixedUpdateState(EquippedPickaxeStateMachine stateMachine) { }

    public override void HandleCollision(EquippedPickaxeStateMachine stateMachine, Collision2D collision) { }

    public override void HandleInput(EquippedPickaxeStateMachine stateMachine) { }

    public override void HandleTrigger(EquippedPickaxeStateMachine stateMachine, Collider2D other) { }

    public override void UpdateState(EquippedPickaxeStateMachine stateMachine) { }
}
