using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : BaseCharacter
{
    [field: Header("플레이어 세팅")]
    [field: SerializeField] public float MaxJumpForce { get; private set; } = 12.5f;
    [field: SerializeField] public float MinJumpForce { get; private set; } = 6f;
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

    private bool hasPickaxe;
    public bool HasPickaxe
    {
        get { return hasPickaxe; }
        set
        {
            // 상태가 실제로 변경될 때만 이벤트를 호출하도록 처리
            if (hasPickaxe != value)
            {
                hasPickaxe = value;
                // 상태 변경을 구독자(HUD)에게 알림
                OnPickaxeStateChange?.Invoke(hasPickaxe);
            }
        }
    }
    public event Action<bool> OnPickaxeStateChange;

    // 테스트로 AnimationData를 모노비헤이비어로 바꾸고 플레이어 게임 오브젝트에 스크립트로 추가해봤습니다.
    /*[field: Header("애니메이션 데이터")]
    [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; } = new PlayerAnimationData();*/
    public PlayerController Controller { get; private set; }
    public PlayerAnimationData AnimationData { get; private set; }
    public UIHUD HUD { get; private set; }
    public UIPause UIPause { get; private set; }
    public UIRecallPickaxe UIRecallPickaxe { get; private set; }
    public UICatchPickaxe UICatchPickaxe { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        AnimationData = GetComponent<PlayerAnimationData>();
        AnimationData?.Initialize();
        Controller = GetComponent<PlayerController>();
        HUD = UIManager.Instance.GetUI<UIHUD>();
        OnCurHpChange += HUD.SetHp; // 이벤트 바인드해주기, 현재 체력 변화할 때마다 자동 호출
        OnMineralCntChage += HUD.SetMineralCount;
        OnPickaxeStateChange += HUD.SetPickaxeOwn;

        Init(); // 초기화 할 것들, 현재는 UI만
        UIPause = UIManager.Instance.GetUI<UIPause>();
        UIRecallPickaxe = UIManager.Instance.GetUI<UIRecallPickaxe>();
        UICatchPickaxe = UIManager.Instance.GetUI<UICatchPickaxe>();
        UIRecallPickaxe.Player = this;
        UICatchPickaxe.Player = this;
    }

    void Init()
    {
        HasPickaxe = true;

        HUD.SetStageText(1);
    }

    protected override void Start()
    {
        base.Start();

        // 데이터 테이블에서 로드한 플레이어 데이터
        PlayerData playerData = DataTableManager.Instance.GetSingleData<PlayerData>();

        // UI 세팅
        CurrentHP = playerData.player_hp; // 체력 UI
        MineralCnt = mineralCnt; // 보유 광물 수 UI 세팅

        // 스탯 세팅
        AttackPower = playerData.player_dam;
        MoveSpeed = playerData.player_mov;
        //JumpForce = playerData.player_jump;
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
