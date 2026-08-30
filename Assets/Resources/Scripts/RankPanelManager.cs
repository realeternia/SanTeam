using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using System.Linq;
using TMPro;

public class RankPanelManager : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject rankParent;
    public GameObject rankCellPrefab; // RankCell预制体引用

    public Button btnLeadShip;
    public Button btnStr;
    public Button btnInte;
    public Button btnHp;
    public Button btnPrice;

    public Button closeBtn;

    private LoopScrollRect loopScroll; // 循环列表，只实例化可见单元格，避免一次性创建全部条目导致卡顿


    // Start is called before the first frame update
    void Start()
    {
        ConfigManager.Init();

        loopScroll = new LoopScrollRect(scrollRect);

        // 加载所有英雄配置
        LoadHeroRankings();

        btnLeadShip.onClick.AddListener(() =>
        {
            GameLog.Debug("点击了btnLeadShip，开始按领导力排序");
            SortItems("LeadShip");
        });
        btnStr.onClick.AddListener(() =>
        {
            GameLog.Debug("点击了btnStr，开始按力量排序");
            SortItems("Str");
        });
        btnInte.onClick.AddListener(() =>
        {
            GameLog.Debug("点击了btnInte，开始按智力排序");
            SortItems("Inte");
        });
        btnHp.onClick.AddListener(() =>
        {
            GameLog.Debug("点击了btnHp，开始按生命值排序");
            SortItems("Hp");
        });
        btnPrice.onClick.AddListener(() =>
        {
            GameLog.Debug("点击了btnPrice，开始按价格排序");
            SortItems("Price");
        });
        closeBtn.onClick.AddListener(() =>
        {      
            PanelManager.Instance.HideRank();
            CardShopManager.Instance.OnShow();
        });

    }

    private void SortItems(string rankType)
    {
        // 循环列表模式：直接对数据源排序并刷新可见单元格
        if (loopScroll != null && loopScroll.IsInitialized)
        {
            loopScroll.SortItems((a, b) =>
                GetHeroVal(b as HeroConfig, rankType).CompareTo(GetHeroVal(a as HeroConfig, rankType)));
            scrollRect.normalizedPosition = new Vector2(0, 1);
            return;
        }

        List<RankCellInfo> cellInfos = new List<RankCellInfo>();
        foreach (Transform child in rankParent.transform)
        {
            cellInfos.Add(child.GetComponent<RankCellInfo>());
        }

        cellInfos.Sort((a, b) =>
        {
            if(rankType == "LeadShip")
                return b.leadShip.CompareTo(a.leadShip);
            else if(rankType == "Str")
                return b.str.CompareTo(a.str);
            else if(rankType == "Inte")
                return b.inte.CompareTo(a.inte);
            else if(rankType == "Hp")
                return b.hp.CompareTo(a.hp);
            else if(rankType == "Price")
                return b.price.CompareTo(a.price);
            return 0;
        });

        for(int i = 0; i < cellInfos.Count; i++)
        {
            cellInfos[i].gameObject.transform.SetSiblingIndex(i);
        }
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }

    private int GetHeroVal(HeroConfig h, string rankType)
    {
        if (h == null) return 0;
        switch (rankType)
        {
            case "LeadShip": return h.Atk;
            case "Str": return h.Might;
            case "Inte": return h.Ap;
            case "Hp": return h.Hp;
            case "Price": return HeroSelectionTool.GetPrice(h);
            default: return 0;
        }
    }

    // 加载英雄排名
    private void LoadHeroRankings()
    {
        // 清除现有的子物体
        foreach (Transform child in rankParent.transform)
        {
            Destroy(child.gameObject);
        }

        // 切换前清理循环列表（若已启用）
        if (loopScroll != null && loopScroll.IsInitialized)
        {
            loopScroll.Clear();
        }

        // 获取所有英雄配置
        var heroConfigs = HeroConfig.ConfigList;
        float cellHeight = rankCellPrefab.GetComponent<RectTransform>().sizeDelta.y;

        // 使用循环列表加载：只实例化视口内可见的单元格
        List<object> dataSource = heroConfigs.Cast<object>().ToList();
        loopScroll.Initialize(dataSource, rankCellPrefab, cellHeight);

        // 确保scrollRect不为空，然后滚动到最前面
        if (scrollRect != null)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }
    }

    public void OnShow()
    {
        // 重新显示时刷新可见单元格（卡池、收藏状态可能已变化）
        if (loopScroll != null && loopScroll.IsInitialized)
        {
            loopScroll.ForceRefresh();
        }
    }

    public void OnHide()
    {
    }


    // Update is called once per frame
    void Update()
    {

    }
}
