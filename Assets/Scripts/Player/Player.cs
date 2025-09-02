using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : BaseCharacter
{
    [field: Header("플레이어 세팅")]
    [field: SerializeField] public float JumpForce { get; private set; } = 7f;
    int mineralCnt = 0;
    public int MineralCnt { get { return mineralCnt; }
        set 
        {
            mineralCnt = value;
            mineralCnt = Mathf.Clamp(mineralCnt, 0, 999);
            OnMineralCntChage?.Invoke(mineralCnt);
        } 
    }
    public event Action<int> OnMineralCntChage;

    // 테스트로 AnimationData를 모노비헤이비어로 바꾸고 플레이어 게임 오브젝트에 스크립트로 추가해봤습니다.
    /*[field: Header("애니메이션 데이터")]
    [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; } = new PlayerAnimationData();*/
    public PlayerController Controller { get; private set; }
    public PlayerAnimationData AnimationData { get; private set; }
    public UIHUD HUD { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        AnimationData = GetComponent<PlayerAnimationData>();
        AnimationData.Initialize();
        Controller = GetComponent<PlayerController>();
        HUD = UIManager.Instance.GetUI<UIHUD>();
        OnCurHpChange += HUD.SetHp; // 이벤트 바인드해주기, 현재 체력 변화할 때마다 자동 호출
        OnMineralCntChage += HUD.SetMineralCount;
        Init(); // 초기화 할 것들, 현재는 UI만
    }
    void Init()
    {
        // 테스트로 다르게 세팅
        CurrentHP = 3; // 체력 UI, 테스트 3으로 세팅
        MineralCnt = 5;  
        // UI 세팅
        //CurrentHP = curHP; // 체력 UI
        //MineralCnt = mineralCnt; // 보유 광물 수 UI 세팅
        HUD.SetPickaxeOwn(true); // 곡괭이 보유여부에 따라 세팅하기
        HUD.SetStageText(1);
    }
    protected override void Update()
    {
        base.Update();
        // 체력 UI 테스트
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentHP -= 1;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentHP += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            HUD.SetPickaxeOwn(true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            HUD.SetPickaxeOwn(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            MineralCnt++;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            MineralCnt--;
        }

    }

}
