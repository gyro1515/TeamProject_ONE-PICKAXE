using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRecallPickaxe : BaseUI
{
    [Header("곡괭이 회수 UI 세팅")]
    [SerializeField] GameObject keyIconGO;
    [SerializeField] GameObject gaugeBarGO;
    [SerializeField] Image gaugeBarImg;
    [SerializeField] Vector3 keyIconOffset;
    [SerializeField] Vector3 gaugeBarOffset;

    public Player Player {  get; set; }
    private void Awake()
    {
        CloseUI();
    }
    private void Update()
    {
        if (Player == null) return;
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(Player.gameObject.transform.position);
        keyIconGO.gameObject.transform.position = playerScreenPos + keyIconOffset;
        gaugeBarGO.gameObject.transform.position = playerScreenPos + gaugeBarOffset;
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
    public void SetGaugeBarValue(float cur, float max)
    {
        float ratio = Mathf.Clamp01(cur / max);
        gaugeBarImg.fillAmount = ratio;
    }


}
