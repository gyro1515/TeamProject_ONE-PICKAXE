using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICatchPickaxe : BaseUI
{
    [Header("곡괭이 캐치 UI 세팅")]
    [SerializeField] GameObject TextGO;
    [SerializeField] Vector3 TextOffset;

    public Player Player {  get; set; }

    private void Awake()
    {
        CloseUI();
    }

    private void Update()
    {
        if (Player == null)
        {
            return;
        }

        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(Player.gameObject.transform.position);
        TextGO.gameObject.transform.position = playerScreenPos + TextOffset;
    }

    public override void OpenUI()
    {
        enabled = true;
        base.OpenUI();
    }

    public override void CloseUI()
    {
        base.CloseUI();
        enabled = false;
    }
}
