using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class MySelectControl : MonoBehaviour
{
    public enum ViewMode
    {
        Hero,
        Bond,
    }

    public GameObject nodePrefab; // 拖拽CardView预制体到此处
    private List<int> playerCards;
    private PlayerInfo playerInfo;

    public TMP_Text playerNameText;
    public Button changeButton;

    private TMP_Text changeButtonText;
    private ViewMode mode = ViewMode.Hero;
    private PlayerInfo currentPlayer;

    void Start()
    {
        // 容错：背包等场景的 MySelectControl 未配置 changeButton 时不做切换绑定
        if (changeButton != null)
        {
            changeButton.onClick.AddListener(OnChangeMode);
            changeButtonText = changeButton.GetComponentInChildren<TMP_Text>();
            UpdateChangeButtonText();
        }
    }

    // 外部设置显示模式（背包默认停在羁绊模式，且无 changeButton 不可手动切换）
    public void SetMode(ViewMode newMode)
    {
        mode = newMode;
        UpdateChangeButtonText();
    }

    private void OnChangeMode()
    {
        mode = mode == ViewMode.Hero ? ViewMode.Bond : ViewMode.Hero;
        UpdateChangeButtonText();
        if (currentPlayer != null)
            ShowCards(currentPlayer, new List<int>(currentPlayer.cards.Keys));
    }

    // 按钮文字显示当前可切换到的目标模式
    private void UpdateChangeButtonText()
    {
        if (changeButtonText == null)
            return;
        changeButtonText.text = mode == ViewMode.Hero ? "英雄" : "羁绊";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuickView(PlayerInfo checkPlayer)
    {
        currentPlayer = checkPlayer;
        var tmpCards = new List<int>(checkPlayer.cards.Keys);
        ShowCards(checkPlayer, tmpCards);
        playerNameText.text = checkPlayer.playerConfig.Name;
    }

    public void QuickViewFin()
    {
        ShowCards(playerInfo, playerCards);
        playerNameText.text = playerInfo.playerConfig.Name;
    }

    public void UpdateCards(PlayerInfo playerInfo)
    {
        // 假设 playerInfo 中有 cards 列表
        playerCards = new List<int>(playerInfo.cards.Keys);
        this.playerInfo = playerInfo;
        currentPlayer = playerInfo;
        ShowCards(playerInfo, playerCards);
        playerNameText.text = playerInfo.playerConfig.Name;
        GameLog.Debug($"UpdateCards {playerInfo.name} {playerCards.Count}");
    }

    private void ShowCards(PlayerInfo checkPlayer, List<int> cards)
    {
        if (mode == ViewMode.Bond)
        {
            ShowBondCards(checkPlayer);
            return;
        }
        ShowHeroCards(checkPlayer, cards);
    }

    private void ShowHeroCards(PlayerInfo checkPlayer, List<int> cards)
    {
        // 英雄模式：经验从高到低排序
        cards.Sort((a, b) =>
        {
            var ea = checkPlayer.cards.TryGetValue(a, out int x) ? x : 0;
            var eb = checkPlayer.cards.TryGetValue(b, out int y) ? y : 0;
            return eb.CompareTo(ea);
        });

        // 获取当前已有的 TMP_Text 组件
        List<SelectCardNodeControl> existingTexts = new List<SelectCardNodeControl>(GetComponentsInChildren<SelectCardNodeControl>());

        // 遍历 cards 列表
        int i = 0;
        foreach (var cardId in cards)
        {
            if(!ConfigManager.IsHeroCard(cardId))
                continue;

            SelectCardNodeControl selectNode = GetOrCreateNode(existingTexts, i);

            // 更新文本内容，这里假设 CardInfo 有一个 GetDisplayText 方法
            var cardCfg = HeroConfig.GetConfig(cardId);
            var skillIcon = "";
            var skillCfgs = ConfigManager.GetHeroSkillConfigs(cardCfg);
            if (skillCfgs.Count > 0)
            {
                skillIcon = skillCfgs[0].Icon;
            }
            selectNode.cardId = cardId;
            selectNode.UpdateExp(checkPlayer.pid, cardCfg.Name, checkPlayer.cards[cardId], skillIcon);

            i++;
        }

        // 移除多余的 TMP_Text
        for (int j = i; j < existingTexts.Count; j++)
        {
            Destroy(existingTexts[j].gameObject);
        }
    }

    // 羁绊模式：显示职业羁绊、好友羁绊与国家羁绊，按羁绊人数（等级）倒序，格式"等级+名字"
    // 职业：等级=上阵人数（1人=Lv1）；好友/国家：等级=人数-1（1人=0级，2人=Lv1起，最多5级）
    private void ShowBondCards(PlayerInfo checkPlayer)
    {
        var entries = new List<BondEntry>();

        // 统计上阵英雄卡（羁绊按上阵阵容 battleCards 统计）
        var battleHeroes = new List<int>();
        foreach (var id in checkPlayer.battleCards)
        {
            if (id > 0 && ConfigManager.IsHeroCard(id))
                battleHeroes.Add(id);
        }

        // 职业羁绊：同职业人数即等级，名字取 JobConfig.NameS
        var jobCounts = new Dictionary<string, int>();
        foreach (var id in battleHeroes)
        {
            var job = HeroConfig.GetConfig(id).Job;
            jobCounts.TryGetValue(job, out var c);
            jobCounts[job] = c + 1;
        }
        foreach (var kv in jobCounts)
        {
            var jobCfg = ConfigManager.GetJobConfig(kv.Key);
            if (jobCfg == null)
                continue;
            entries.Add(new BondEntry { Name = jobCfg.Name, Count = kv.Value, Icon = GetSkillIcon(jobCfg.SkillId) });
        }

        // 好友羁绊：HeroFriendConfig.Heros 中拥有英雄数即人数，等级=人数-1（1人=0级）；
        // 普通组无特殊技能则不显示图标；配置了 LineColor 的组用该颜色作为文字前景色
        foreach (var friendCfg in HeroFriendConfig.ConfigList)
        {
            var present = 0;
            foreach (var memberId in friendCfg.Heros)
            {
                if (battleHeroes.Contains(memberId))
                    present++;
            }
            if (present <= 0)
                continue;

            Color? lineColor = null;
            if (!string.IsNullOrEmpty(friendCfg.LineColor))
            {
                if (ColorUtility.TryParseHtmlString(friendCfg.LineColor, out var parsed))
                    lineColor = parsed;
            }
            entries.Add(new BondEntry { Name = friendCfg.Name, Count = present - 1, Icon = GetSkillIcon(friendCfg.SkillId), Color = lineColor });
        }

        // 国家势力：按上阵英雄阵营计数；不参与同阵营护盾(野=10)或图标为空的国家跳过；
        // 等级=同阵营人数-1（1人=0级）
        var forceCounts = new Dictionary<int, int>();
        foreach (var id in battleHeroes)
        {
            var side = HeroConfig.GetConfig(id).Side;
            forceCounts.TryGetValue(side, out var c);
            forceCounts[side] = c + 1;
        }
        foreach (var kv in forceCounts)
        {
            var forceCfg = ConfigManager.GetForceConfig(kv.Key);
            if (forceCfg == null || !forceCfg.JoinFactionShield)
                continue;

            entries.Add(new BondEntry { Name = forceCfg.Name, Count = kv.Value - 1, Icon = "Textures/Icons/" + forceCfg.Icon });
        }

        // 羁绊人数倒序排序
        entries.Sort((a, b) => b.Count.CompareTo(a.Count));

        var existingTexts = new List<SelectCardNodeControl>(GetComponentsInChildren<SelectCardNodeControl>());
        int i = 0;
        foreach (var entry in entries)
        {
            var selectNode = GetOrCreateNode(existingTexts, i);
            selectNode.cardId = 0;
            selectNode.UpdateBond(entry.Count + entry.Name, entry.Icon, entry.Color);
            i++;
        }
        for (int j = i; j < existingTexts.Count; j++)
        {
            Destroy(existingTexts[j].gameObject);
        }
    }

    // 复用已有节点或实例化新节点
    private SelectCardNodeControl GetOrCreateNode(List<SelectCardNodeControl> existingTexts, int index)
    {
        if (index < existingTexts.Count)
            return existingTexts[index];

        GameObject textObject = Instantiate(nodePrefab, transform);
        textObject.name = $"CardText_{index}";
        var selectNode = textObject.GetComponent<SelectCardNodeControl>();
        if (selectNode == null)
        {
            GameLog.Error("Failed to get SelectCardNodeControl component from the instantiated prefab.");
        }

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 1);
            rectTransform.anchoredPosition = new Vector2(0, - index * 63); // 假设每个文本高度 30 单位
            rectTransform.sizeDelta = new Vector2(0, 60);
        }
        return selectNode;
    }

    // 按技能缩写取图标路径，未配置或不存在返回空串
    private string GetSkillIcon(string sname)
    {
        if (string.IsNullOrEmpty(sname))
            return "";
        var cfg = ConfigManager.GetSkillConfig(sname);
        return cfg != null ? cfg.Icon : "";
    }

    private struct BondEntry
    {
        public string Name;
        public int Count;
        public string Icon;
        public Color? Color;
    }
}