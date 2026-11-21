using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

public class RankPlayerPanelManager : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject rankParent;
    public GameObject rankCellPrefab; // RankCell预制体引用

    public Button btnGold;
    public Button btnPower;
    public Button btnMark;

    public Image winRateHeaderIcon; // 胜率列表头
    public Image recentHeaderIcon;  // 最近10场战绩列表头

    public Button closeBtn;


    // Start is called before the first frame update
    void Start()
    {
        btnGold.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            GameLog.Debug("点击了btnGold，开始按金币排序");
            SortItems("gold");
        });
        btnPower.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            GameLog.Debug("点击了btnPower，开始按战力排序");
            SortItems("power");
        });
        btnMark.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            GameLog.Debug("点击了btnMark，开始按玩家生命排序");
            SortItems("mark");
        });
        closeBtn.onClick.AddListener(() =>
        {      
            PanelManager.Instance.HideRankPlayer();
            CardShopManager.Instance.OnShow();
        });

        // 新增图标没有预制体资源引用，运行时按 SystemAttrConfig 加载
        if (winRateHeaderIcon != null)
            winRateHeaderIcon.sprite = LoadSystemIcon("pvpwinrate");
        if (recentHeaderIcon != null)
            recentHeaderIcon.sprite = LoadSystemIcon("pvpwin");
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
            if(rankType == "gold")
                return b.gold.CompareTo(a.gold);
            else if(rankType == "power")
                return b.power.CompareTo(a.power);
            else if(rankType == "mark")
                return b.life.CompareTo(a.life);
            return 0;
        });

        for(int i = 0; i < cellInfos.Count; i++)
        {
            cellInfos[i].gameObject.transform.SetSiblingIndex(i);
        }
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }

    // 加载玩家排名
    private void LoadPlayerRankings()
    {
        // 清除现有的子物体
        foreach (Transform child in rankParent.transform)
        {
            Destroy(child.gameObject);
        }

        // 为每个存活玩家创建一个RankCell（已淘汰玩家不再显示）
        int aliveCount = 0;
        foreach (var playerInfo in GameManager.Instance.players)
        {
            if (playerInfo.isEliminated)
                continue;

            // 实例化RankCell
            GameObject cell = Instantiate(rankCellPrefab, rankParent.transform);
            cell.transform.localScale = Vector3.one;

            // 获取RankPlayerCellInfo组件
            var cellInfo = cell.GetComponent<RankPlayerCellInfo>();
            if (cellInfo != null)
            {
                cellInfo.Init(playerInfo);
            }
            aliveCount++;
        }
        // Get the RectTransform components
         RectTransform rankParentRect = rankParent.GetComponent<RectTransform>();
         RectTransform cellRect = rankCellPrefab.GetComponent<RectTransform>();
          
         if (rankParentRect != null && cellRect != null)
         {
             // Set the height of rankParent based on the number of cells
             rankParentRect.sizeDelta = new Vector2(rankParentRect.sizeDelta.x, cellRect.sizeDelta.y * aliveCount);
         }
        // 确保scrollRect不为空，然后滚动到最前面
        if (scrollRect != null)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }
    }

    public void OnShow()
    {
        // 加载所有玩家数据
        LoadPlayerRankings();
    }

    public void OnHide()
    {
    }

    private static Sprite LoadSystemIcon(string name)
    {
        SystemAttrConfig cfg = SystemAttrConfig.GetConfigByname(name);
        if (cfg == null || string.IsNullOrEmpty(cfg.Icon))
        {
            GameLog.Error("RankPlayerPanelManager LoadSystemIcon: SystemAttrConfig 缺少图标 name=" + name);
            return null;
        }
        return IconLoader.LoadSprite(cfg.Icon);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
