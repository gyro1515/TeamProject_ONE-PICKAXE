public abstract class PickaxeState
{
    // 상태 진입
    public abstract void EnterState(PickaxeController pickaxe);

    // 상태 업데이트
    public abstract void UpdateState(PickaxeController pickaxe);

    // 상태 종료
    public abstract void ExitState(PickaxeController pickaxe);
}
