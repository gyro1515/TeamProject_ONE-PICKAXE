
public class ThrownPickaxeStateMachine : BasePickaxeStateMachine<ThrownPickaxeStateMachine>
{
    // 상태 전환 시 필요한 데이터
    public ThrownPickaxeController ThrownPickaxeController;

    // 미리 생성해둘 상태 인스턴스 (캐싱)
    public readonly PickaxeFlyingState FlyingState;
    public readonly PickaxeStuckState StuckState;
    public readonly PickaxeRetrieveState RetrieveState;
    public readonly PickaxeBounceState BounceState;

    public ThrownPickaxeStateMachine(ThrownPickaxeController controller)
    {
        ThrownPickaxeController = controller;

        FlyingState = new PickaxeFlyingState();
        StuckState = new PickaxeStuckState();
        RetrieveState = new PickaxeRetrieveState();
        BounceState = new PickaxeBounceState();
    }
}
