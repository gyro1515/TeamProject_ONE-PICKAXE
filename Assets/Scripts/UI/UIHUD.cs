using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHUD : BaseUI
{
    [Header("HUD 세팅")]
    [SerializeField] Transform hpSlotTransform;
    [SerializeField] Transform slotPool;
    [SerializeField] Image pickaxeOnImg;
    [SerializeField] Image PickaxeOffImg;
    [SerializeField] TextMeshProUGUI pickaxeOwnText;
    [SerializeField] GameObject slotHpOnPrefabs;
    [SerializeField] GameObject slotHpOffPrefabs;
    [SerializeField] TextMeshProUGUI stageText;
    [SerializeField] TextMeshProUGUI mineralCountText;
    // 체력 아이콘 오브젝트 풀 용도
    List<GameObject> hpOnslots = new List<GameObject>();
    List<GameObject> hpOffslots = new List<GameObject>();

    public void SetPickaxeOwn(bool active) // 곡괭이 소유여부 세팅
    {
        if(active)
        {
            pickaxeOnImg.gameObject.SetActive(true);
            PickaxeOffImg.gameObject.SetActive(false);
            pickaxeOwnText.text = "보유";
        }
        else
        {
            pickaxeOnImg.gameObject.SetActive(false);
            PickaxeOffImg.gameObject.SetActive(true);
            pickaxeOwnText.text = "미보유";
        }
    }
    public void SetHp(int curHp, int maxHp)
    {
        ResetHpSlot();
        int hpOffCnt = maxHp - curHp;
        AddOnHp(curHp); // 체력on 아이콘 추가
        AddOffHp(hpOffCnt); // 체력off 아이콘 추가
    }
    public void SetStageText(int curStage)
    {
        stageText.text = "Stage " + curStage.ToString();
    }
    public void SetMineralCount(int cnt)
    {
        mineralCountText.text = cnt.ToString();
    }
    void ResetHpSlot()
    {
        // 앞에서부터 하면 자식이 계속 빠져서 로직이 꼬인다...
        for (int i = hpSlotTransform.childCount - 1; i >= 0; i--) 
        {
            Transform child = hpSlotTransform.GetChild(i);
            child.SetParent(slotPool);
            child.gameObject.SetActive(false);
        }
    }
    void AddOnHp(int cnt)
    {
        for(int i = 0; i < cnt; i++)
        {
            bool isAdded = false;
            for (int j = 0; j < hpOnslots.Count; j++)
            {
                if (hpOnslots[j].activeSelf) continue;
                hpOnslots[j].SetActive(true);
                hpOnslots[j].transform.SetParent(hpSlotTransform);
                isAdded = true;
                //Debug.Log($"{i}번째 on 활성화");
                break;
            }
            if (isAdded) continue;
            // 활성화 시킬 슬롯 없으면 만들기
            GameObject hpOnSlot = Instantiate(slotHpOnPrefabs, hpSlotTransform);
            hpOnSlot.SetActive(true);
            hpOnslots.Add(hpOnSlot);
            //Debug.Log($"{i}번째 on 생성/ hpOnslotsCnt: {hpOnslots.Count}");

        }
    }
    void AddOffHp(int cnt)
    {
        for (int i = 0; i < cnt; i++)
        {
            bool isAdded = false;
            for (int j = 0; j < hpOffslots.Count; j++)
            {
                if (hpOffslots[j].activeSelf) continue;
                hpOffslots[j].SetActive(true);
                hpOffslots[j].transform.SetParent(hpSlotTransform);
                isAdded = true;
                //Debug.Log($"{i}번째 off 활성화");
                break;
            }
            if (isAdded) continue;
            // 활성화 시킬 슬롯 없으면 만들기
            GameObject hpOffSlot = Instantiate(slotHpOffPrefabs, hpSlotTransform);
            hpOffSlot.SetActive(true);
            hpOffslots.Add(hpOffSlot);
            //Debug.Log($"{i}번째 off 생성 / hpOffslots: {hpOffslots.Count}");
        }
    }
    
}
