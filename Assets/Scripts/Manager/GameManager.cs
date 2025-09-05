using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    public Player Player {  get; set; }

    protected override void Awake()
    {
        base.Awake();

        // 데이터 테이블 로드
        DataTableManager.Instance.LoadSingleData<PlayerData>();
        DataTableManager.Instance.LoadSingleData<ObsaicleData>();
        DataTableManager.Instance.LoadSingleData<DropitemData>();
        DataTableManager.Instance.LoadSingleData<DoorData>();

        DataTableManager.Instance.LoadCollectionData<ItemDataTable>();
        DataTableManager.Instance.LoadCollectionData<EnemyDataTable>();
    }
}
