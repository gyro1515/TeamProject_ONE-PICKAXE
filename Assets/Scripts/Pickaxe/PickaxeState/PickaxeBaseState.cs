using UnityEngine;

public abstract class PickaxeBaseState<T> where T : BasePickaxeStateMachine<T>
{
    // 각 상태 진입 시 호출
    public abstract void EnterState(T stateMachine);

    // 사용자 입력 처리
    public abstract void HandleInput(T stateMachine);

    // 충돌 처리
    public abstract void HandleCollision(T stateMachine, Collision2D collision);

    // 트리거 처리
    public abstract void HandleTrigger(T stateMachine, Collider2D other);

    // 매 프레임마다 호출
    public abstract void UpdateState(T stateMachine);

    // 물리 연산 처리
    public abstract void FixedUpdateState(T stateMachine);

    // 각 상태 종료 시 호출
    public abstract void ExitState(T stateMachine);
}
