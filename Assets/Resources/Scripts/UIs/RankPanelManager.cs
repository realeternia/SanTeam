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

    public Button btnAtk;
    public Button btnArmor;
    public Button btnMagicRes;
    public Button btnHp;
    public Button btnPrice;

    public Button closeBtn;

    private LoopScrollRect loopScroll; // 循环列表，只实例化可见单元格，避免一次性创建全部条目导致卡顿
    private int loadedScenarioId = int.MinValue; // 当前数据源对应的剧本，切换剧本后需重建
    private bool inited; // ShowRank 会先激活面板再调 OnShow（早于 Start），用该标记保证只初始化一次


    // Start is called before the first frame update
    void Start()
    {
        EnsureInitialized();
    }

    // 面板初始化：首次显示（可能在 Start 之前）时创建循环列表、加载数据并绑定排序按钮
    private void EnsureInitialized()
    {
        if (inited)
            return;
        inited = true;

        ConfigManager.Init();

        loopScroll = new LoopScrollRect(scrollRect);

        // 加载当前剧本英雄排行
        LoadHeroRankings();

        btnAtk.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            GameLog.Debug("点击了btnAtk，开始按攻击排序");
            SortItems("Atk");
        });
        btnArmor.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            GameLog.Debug("点击了btnArmor，开始按护甲排序");
            SortItems("Armor");
        });
        btnMagicRes.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            GameLog.Debug("点击了btnMagicRes，开始按魔抗排序");
            SortItems("MagicRes");
        });
        btnHp.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            GameLog.Debug("点击了btnHp，开始按生命值排序");
            SortItems("Hp");
        });
        btnPrice.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
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
            if(rankType == "Atk")
                return b.atk.CompareTo(a.atk);
            else if(rankType == "Armor")
                return b.armor.CompareTo(a.armor);
            else if(rankType == "MagicRes")
                return b.magicRes.CompareTo(a.magicRes);
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
        // 排序与展示同口径：HeroConfig 数值经 PostModify/HeroScenario 写回为 1 星面板
        switch (rankType)
        {
            case "Atk": return h.Atk;
            case "Armor": return h.Armor;
            case "MagicRes": return h.MagicRes;
            case "Hp": return h.Hp;
            case "Price": return HeroSelectionTool.GetPrice(h);
            default: return 0;
        }
    }

    // 加载英雄排名
    private void LoadHeroRankings()
    {
        // 先清理循环列表（持有单元格引用），再清空可能残留的子物体
        if (loopScroll != null && loopScroll.IsInitialized)
        {
            loopScroll.Clear();
        }

        foreach (Transform child in rankParent.transform)
        {
            Destroy(child.gameObject);
        }

        // 只取当前剧本出场的英雄；默认按国家（阵营）分组，同国家内按品质从高到低
        var heroConfigs = HeroConfig.ConfigList
            .Where(cfg => HeroScenario.IsAvailable(cfg.Id))
            .OrderBy(cfg => HeroScenario.GetSide(cfg))
            .ThenByDescending(cfg => HeroScenario.GetQuality(cfg))
            .ThenBy(cfg => cfg.Id)
            .ToList();
        loadedScenarioId = HeroScenario.CurrentId;
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
        // 首次 ShowRank 时 OnShow 会早于 Start 执行，这里保证循环列表已初始化
        EnsureInitialized();

        // 剧本切换后按新剧本重建数据源，保证只显示本剧本英雄
        if (loadedScenarioId != HeroScenario.CurrentId)
        {
            LoadHeroRankings();
            return;
        }

        // 重新显示时刷新可见单元格（卡池、收藏状态可能已变化）
        if (loopScroll != null && loopScroll.IsInitialized)
        {
            loopScroll.ForceRefresh();
        }
    }

    public void OnHide()
    {
        // 面板关闭时隐藏按住显示的英雄 Tips，避免残留在界面上
        PanelManager.Instance.GetTooltip<BaseTooltip>()?.HideTooltip();
    }


    // Update is called once per frame
    void Update()
    {

    }
}
