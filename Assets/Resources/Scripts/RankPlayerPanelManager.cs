using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;

public class RankPlayerPanelManager : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject rankParent;
    public GameObject rankCellPrefab; // RankCell预制体引用

    public Button btnSdAtk;
    public Button btnSdHp;
    public Button btnFood;
    public Button btnGold;
    public Button btnPower;
    public Button btnMark;

    public Button closeBtn;


    // Start is called before the first frame update
    void Start()
    {


        btnSdAtk.onClick.AddListener(() =>
        {
            UnityEngine.Debug.Log("点击了btnSdAtk，开始按攻击力排序");
            SortItems("sdatk");
        });
        btnSdHp.onClick.AddListener(() =>
        {
            UnityEngine.Debug.Log("点击了btnSdHp，开始按生命值排序");
            SortItems("sdhp");
        });
        btnFood.onClick.AddListener(() =>
        {
            UnityEngine.Debug.Log("点击了btnFood，开始按食物排序");
            SortItems("food");
        });
        btnGold.onClick.AddListener(() =>
        {
            UnityEngine.Debug.Log("点击了btnGold，开始按金币排序");
            SortItems("gold");
        });
        btnPower.onClick.AddListener(() =>
        {
            UnityEngine.Debug.Log("点击了btnPower，开始按战力排序");
            SortItems("power");
        });
        btnMark.onClick.AddListener(() =>
        {
            UnityEngine.Debug.Log("点击了btnMark，开始按标记排序");
            SortItems("mark");
        });
        closeBtn.onClick.AddListener(() =>
        {      
            PanelManager.Instance.HideRankPlayer();
            CardShopManager.Instance.OnShow();
        });

    }

    private void SortItems(string rankType)
    {
        var cellInfos = new List<RankPlayerCellInfo>();
        foreach (Transform child in rankParent.transform)
        {
            cellInfos.Add(child.GetComponent<RankPlayerCellInfo>());
        }

        cellInfos.Sort((a, b) =>
        {
            if(rankType == "sdatk")
                return b.soldierAtk.CompareTo(a.soldierAtk);
            else if(rankType == "sdhp")
                return b.soldierHp.CompareTo(a.soldierHp);
            else if(rankType == "food")
                return b.food.CompareTo(a.food);
            else if(rankType == "gold")
                return b.gold.CompareTo(a.gold);
            else if(rankType == "power")
                return b.power.CompareTo(a.power);
            else if(rankType == "mark")
                return b.mark.CompareTo(a.mark);
            return 0;
        });

        for(int i = 0; i < cellInfos.Count; i++)
        {
            cellInfos[i].gameObject.transform.SetSiblingIndex(i);
        }
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }

    // 加载英雄排名
    private void LoadPlayerRankings()
    {
        // 清除现有的子物体
        foreach (Transform child in rankParent.transform)
        {
            Destroy(child.gameObject);
        }

        // 为每个英雄配置创建一个RankCell
        foreach (var playerInfo in GameManager.Instance.players)
        {
            // 实例化RankCell
            GameObject cell = Instantiate(rankCellPrefab, rankParent.transform);
            cell.transform.localScale = Vector3.one;

            // 获取RankPlayerCellInfo组件
            var cellInfo = cell.GetComponent<RankPlayerCellInfo>();
            if (cellInfo != null)
            {
                cellInfo.Init(playerInfo);
            }
        }
        // Get the RectTransform components
         RectTransform rankParentRect = rankParent.GetComponent<RectTransform>();
         RectTransform cellRect = rankCellPrefab.GetComponent<RectTransform>();
          
         if (rankParentRect != null && cellRect != null)
         {
             // Set the height of rankParent based on the number of cells
             rankParentRect.sizeDelta = new Vector2(rankParentRect.sizeDelta.x, cellRect.sizeDelta.y * GameManager.Instance.players.Length);
         }
        // 确保scrollRect不为空，然后滚动到最前面
        if (scrollRect != null)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }
    }

    public void OnShow()
    {
        // 加载所有英雄配置
        LoadPlayerRankings();
    }

    public void OnHide()
    {
    }


    // Update is called once per frame
    void Update()
    {

    }
}
