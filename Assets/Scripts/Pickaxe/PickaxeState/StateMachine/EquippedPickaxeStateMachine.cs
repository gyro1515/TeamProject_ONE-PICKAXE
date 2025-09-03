using UnityEngine;

public class EquippedPickaxeStateMachine : BasePickaxeStateMachine<EquippedPickaxeStateMachine>
{
    // 상태 전환 시 필요한 데이터
    public EquippedPickaxeController EquippedPickaxeController;

    // 미리 생성해둘 상태 인스턴스 (캐싱)
    public readonly PickaxeEquipState EquipState;
    public readonly PickaxeSmashState SmashState;
    public readonly PickaxeThrowState ThrowState;

    public EquippedPickaxeStateMachine(EquippedPickaxeController controller)
    {
        EquippedPickaxeController = controller;

        // 게임 시작 시 모든 상태를 한 번씩만 생성 (캐싱)
        EquipState = new PickaxeEquipState();
        SmashState = new PickaxeSmashState();
        ThrowState = new PickaxeThrowState();
    }
}
