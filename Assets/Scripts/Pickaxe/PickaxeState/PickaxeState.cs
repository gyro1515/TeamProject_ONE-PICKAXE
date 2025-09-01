using UnityEngine;

public abstract class PickaxeState
{
    // 각 상태 진입 시 호출
    public abstract void EnterState(PickaxeStateMachine stateMachine);

    // 사용자 입력 처리
    public abstract void HandleInput(PickaxeStateMachine stateMachine);

    // 충돌 처리
    public abstract void HandleCollision(PickaxeStateMachine stateMachine, Collision2D collision);

    // 트리거 처리
    public abstract void HandleTrigger(PickaxeStateMachine stateMachine, Collider2D other);

    // 매 프레임마다 호출
    public abstract void UpdateState(PickaxeStateMachine stateMachine);

    // 물리 연산 처리
    public abstract void FixedUpdateState(PickaxeStateMachine stateMachine);

    // 각 상태 종료 시 호출
    public abstract void ExitState(PickaxeStateMachine stateMachine);
}
