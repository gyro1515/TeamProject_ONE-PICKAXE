using UnityEngine;

public class PickaxeStateMachine
{
    // 현재 상태
    public PickaxeBaseState CurrentState;
    // 상태 전환 시 필요한 데이터
    public PickaxeController PickaxeController;

    // 미리 생성해둘 상태 인스턴스 (캐싱)
    public readonly PickaxeEquipState EquipState;
    public readonly PickaxeSmashState SmashState;
    public readonly PickaxeThrowState ThrowState;
    public readonly PickaxeStuckState StuckState;
    public readonly PickaxeRetrieveState ReturnState;
    public readonly PickaxeBounceState BounceState;

    public PickaxeStateMachine(PickaxeController controller)
    {
        this.PickaxeController = controller;

        // 게임 시작 시 모든 상태를 한 번씩만 생성 (캐싱)
        EquipState = new PickaxeEquipState();
        ThrowState = new PickaxeThrowState();
        StuckState = new PickaxeStuckState();
        ReturnState = new PickaxeRetrieveState();
        BounceState = new PickaxeBounceState();
    }

    // 초기 상태 설정
    public void Initialize(PickaxeBaseState startingState)
    {
        CurrentState = startingState;
        startingState.EnterState(this);
    }

    // 상태 전환 로직
    public void ChangeState(PickaxeBaseState newState)
    {
        if (CurrentState != null)
        {
            CurrentState.ExitState(this);
        }

        CurrentState = newState;
        CurrentState.EnterState(this);
    }

    public void HandleInput()
    {
        CurrentState?.HandleInput(this);
    }

    public void UpdateState()
    {
        CurrentState?.UpdateState(this);
    }

    public void FixedUpdateState()
    {
        CurrentState?.FixedUpdateState(this);
    }

    public void HandleCollision(Collision2D collision)
    {
        // 충돌 이벤트 처리를 현재 상태로 위임
        CurrentState?.HandleCollision(this, collision);
    }

    public void HandleTrigger(Collider2D other)
    {
        // 트리거 이벤트 처리를 현재 상태로 위임
        CurrentState?.HandleTrigger(this, other);
    }
}
