using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePickaxeStateMachine<T> where T : BasePickaxeStateMachine<T>
{
    // 현재 상태
    public PickaxeBaseState<T> CurrentState;

    // 초기 상태 설정
    public void Initialize(PickaxeBaseState<T> startingState)
    {
        CurrentState = startingState;
        startingState.EnterState((T)this);
    }

    // 상태 전환 로직
    public void ChangeState(PickaxeBaseState<T> newState)
    {
        if (CurrentState != null)
        {
            CurrentState.ExitState((T)this);
        }

        CurrentState = newState;
        CurrentState.EnterState((T)this);
    }

    public void HandleInput()
    {
        CurrentState?.HandleInput((T)this);
    }

    public void UpdateState()
    {
        CurrentState?.UpdateState((T)this);
    }

    public void FixedUpdateState()
    {
        CurrentState?.FixedUpdateState((T)this);
    }

    public void HandleCollision(Collision2D collision)
    {
        // 충돌 이벤트 처리를 현재 상태로 위임
        CurrentState?.HandleCollision((T)this, collision);
    }

    public void HandleTrigger(Collider2D other)
    {
        // 트리거 이벤트 처리를 현재 상태로 위임
        CurrentState?.HandleTrigger((T)this, other);
    }
}
