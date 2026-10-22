using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

/// <summary>
/// 卡片商店“宿主”：只保留各商店模式共用的能力（预制体/容器/布局/购买入口/开场收尾），
/// 具体商店流程（共享轮换 / 独立买卡）由 <see cref="ShopMode"/> 派生类实现，按 GameRoundConfig.ShopType 选择。
/// </summary>
public class CardShopManager : MonoBehaviour
{
    public static CardShopManager Instance;
    public List<CardViewControl> cardViews = new List<CardViewControl>();

    public GameObject cardViewPrefab; // 拖拽CardView预制体到此处
    public GameObject cardItemViewPrefab; // 拖拽CardView预制体到此处

    /// <summary>刷新商店的金币消耗（共享=刷新6张未售卡；独立=重掷自己5格）</summary>
    public const int RefreshGoldCost = 2;
    /// <summary>共享商店：卡售出后维持的round数</summary>
    public const int SOLD_REMAIN_ROUNDS = 5;

    public Button passBtn;
    public Button refreshBtn;
    public Button bagBtn;
    public Button rankBtn;
    public Button rankPlayerBtn;

    public TMP_Text eraText;
    public MySelectControl mySelect;
    public TMP_Text rateText;

    private int era = 0;
    public bool hasEnterBattle = false;

    public int[] playerStartGold = new int[8]; // 记录每个玩家开局金币（用于AI跳过判定）

    /// <summary>本轮商店配置（ShopBegin 内确定）</summary>
    public GameRoundConfig ShopCfg { get; private set; }
    /// <summary>共享商店回合顺序：按积分低→高排序（仅共享模式使用）；非序列化属性</summary>
    public int[] TurnOrder { get; private set; } = new int[8];
    /// <summary>本商店阶段是否已结束</summary>
    public bool IsShopEnd { get; set; }
    /// <summary>当前商店阶段计数（供 AI 评分）</summary>
    public int Era => era;

    /// <summary>当前激活的商店模式</summary>
    public ShopMode Mode { get; private set; }

    private Coroutine shopCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        passBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            Mode?.OnHumanPass();
        });

        refreshBtn.onClick.AddListener(() =>
        {
            Mode?.OnHumanRefresh();
        });

        bagBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowBag();
            for(int i = 0; i < cardViews.Count; i++)
                cardViews[i].ShowEffectLayer(false);
        });
        rankBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowRank();
            for(int i = 0; i < cardViews.Count; i++)
                cardViews[i].ShowEffectLayer(false);
        });
        rankPlayerBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowRankPlayer();
            for(int i = 0; i < cardViews.Count; i++)
                cardViews[i].ShowEffectLayer(false);
        });

        ShopBegin();
    }

    public void OnShow()
    {
        // 回到商店（背包/排行关闭等）：逐张同步特效层容器，亮起与背包卡重复/好友/同职业关联的商店卡
        for (int i = 0; i < cardViews.Count; i++)
            cardViews[i].UpdateEffectLayer();
    }

    // ---- 宿主工具方法（供各商店模式复用） ----

    /// <summary>创建一张可见的商店英雄卡（位置由 LayoutCards/ReplaceCardAt 指定）</summary>
    public CardViewControl CreateCardView(int heroId)
    {
        GameObject card = Instantiate(cardViewPrefab, transform);
        CardViewControl cardView = card.GetComponent<CardViewControl>();
        cardView.Init(heroId, true, GameManager.Instance.year);
        return cardView;
    }

    /// <summary>按网格排布卡位：perRow 为每行张数，centerRows 用于垂直居中（沿用原共享商店布局公式）</summary>
    public void LayoutCards(List<CardViewControl> views, int perRow, int centerRows)
    {
        if (perRow <= 0) perRow = 1;
        float cardWidth = 250f;
        float cardHeight = 318f;
        float spacing = 5f;

        float startX = -((perRow * cardWidth) + (perRow - 1) * spacing) / 2f + cardWidth / 2f - 0;
        float startY = (centerRows - 1) / 2f * (cardHeight + spacing) - 320;

        for (int i = 0; i < views.Count; i++)
        {
            int row = i / perRow;
            int col = i % perRow;
            RectTransform rectTransform = views[i].GetComponent<RectTransform>();
            if (rectTransform != null)
                rectTransform.anchoredPosition = new Vector2(startX + col * (cardWidth + spacing), startY - row * (cardHeight + spacing));
        }
    }

    /// <summary>用随机新卡替换指定卡位（保持原位置），并销毁旧卡</summary>
    public void ReplaceCardAt(int index, int heroId)
    {
        if (index < 0 || index >= cardViews.Count)
            return;

        var old = cardViews[index];
        Vector2 pos = old.GetComponent<RectTransform>().anchoredPosition;

        var newCtr = CreateCardView(heroId);
        newCtr.GetComponent<RectTransform>().anchoredPosition = pos;
        cardViews[index] = newCtr;

        Destroy(old.gameObject);
    }

    /// <summary>清空当前所有卡位（含飞卡动画对象）</summary>
    public void ClearCards()
    {
        var movingCardImages = GameObject.FindGameObjectsWithTag("MovingCard");
        foreach (var img in movingCardImages)
            Destroy(img);

        foreach (Transform child in transform)
            Destroy(child.gameObject);
        cardViews.Clear();
    }

    /// <summary>设置“跳过/结束”按钮文案</summary>
    public void SetPassButtonLabel(string label)
    {
        if (passBtn == null)
            return;
        var text = passBtn.GetComponentInChildren<TMP_Text>();
        if (text != null)
            text.text = label;
    }

    // 品质名称（品质1~4，与 SysColor.GetQualityColor 的档位一致）
    private static readonly string[] QualityNames = { "普通", "稀有", "优秀", "卓越" };

    /// <summary>
    /// 刷新稀有度概率文本：共享模式读回合配置(GameRoundConfig)，独立模式读玩家等级配置(PlayerLevelConfig)，
    /// 每段文本按对应品质颜色着色。触发时机：新回合开始 / 玩家(pid0)等级提升。
    /// </summary>
    public void RefreshRateText()
    {
        if (rateText == null)
            return;

        int q2, q3, q4;
        if (Mode != null && Mode.Type == ShopModeType.Independent)
        {
            int level = GameManager.Instance.GetPlayer(0).level;
            var cfg = PlayerLevelConfig.GetConfig(Mathf.Clamp(level, 1, CombatConst.PlayerMaxLevel));
            q2 = cfg.Quality2Rate;
            q3 = cfg.Quality3Rate;
            q4 = cfg.Quality4Rate;
        }
        else if (ShopCfg != null)
        {
            q2 = ShopCfg.Quality2Rate;
            q3 = ShopCfg.Quality3Rate;
            q4 = ShopCfg.Quality4Rate;
        }
        else
        {
            return;
        }

        int[] rates = { Mathf.Max(0, 100 - q2 - q3 - q4), q2, q3, q4 };

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < rates.Length; i++)
        {
            if (i > 0)
                sb.Append("\n");
            sb.Append(SysColor.ColorText($"{QualityNames[i]} {rates[i]}%", SysColor.GetQualityColor(i + 1)));
        }
        rateText.text = sb.ToString();
    }

    // 刷新卡位出卡：先按 LikeCardRefreshRate 概率从"该玩家自己"的收藏池随机出一张（收藏池=like阶段各玩家各自点赞的卡），
    // 未命中（或该玩家收藏池空）则按 GameRoundConfig 品质概率随机出一张
    public int GetRandomShopHeroId(GameRoundConfig shopCfg, int pid)
    {
        if (SysRandom.Range(0, 100) < CombatConst.LikeCardRefreshRate)
        {
            var likeId = HeroSelectionTool.GetRandomLikedHeroId(pid);
            if (likeId != 0)
                return likeId;
        }
        return HeroSelectionTool.GetRandomHeroIdByQuality(shopCfg);
    }

    // 独立买卡模式刷牌：先按 LikeCardRefreshRate 概率从"该玩家自己"的收藏池出一张，否则按玩家等级(PlayerLevelConfig)品质概率出一张
    public int GetRandomShopHeroIdByLevel(int level, int pid)
    {
        if (SysRandom.Range(0, 100) < CombatConst.LikeCardRefreshRate)
        {
            var likeId = HeroSelectionTool.GetRandomLikedHeroId(pid);
            if (likeId != 0)
                return likeId;
        }
        return HeroSelectionTool.GetRandomHeroIdByLevel(level);
    }

    // ---- 购买入口 ----

    // 人类购买入口：买卡成功后通知模式推进（共享=进入下一回合；独立=无）
    public bool RequestBuy(CardViewControl view, PlayerInfo player, int price)
    {
        if (!OnPlayerBuyCard(view, player, view.cardId, view.isHeroCard, price))
            return false;
        Mode?.OnHumanBought(player, view.ToOffer());
        return true;
    }

    // AI 购买入口：可见卡位买卡走 OnSold；隐藏商店（view==null）由此处把报价标记为已售出
    public bool BuyOffer(PlayerInfo player, ShopOffer offer)
    {
        if (offer == null)
            return false;

        if (!OnPlayerBuyCard(offer.view, player, offer.cardId, offer.isHero, offer.price))
            return false;

        if (offer.view == null)
            offer.sold = true;
        return true;
    }

    public bool OnPlayerBuyCard(CardViewControl ctr, PlayerInfo player, int cardId, bool isHero, int price)
    {
        // AI 买卡失败不弹提示，避免刷屏
        bool showTip = player != null && !player.isAI;

        if (player.gold < price)
        {
            if (showTip)
                SystemTip.Show("金币不足，无法购买");
            return false;
        }

        if (player.BuyCard(ctr, cardId, isHero, price))
        {
            mySelect.UpdateCards(player);
            // 通知模式：共享=触发售出倒计时/相邻刷新；独立=无
            Mode?.OnBought(new ShopOffer
            {
                cardId = cardId,
                isHero = isHero,
                price = price,
                view = ctr,
            }, player);
            return true;
        }

        // 金币足够时唯一的失败原因是新英雄卡超出背包上限
        if (showTip)
            SystemTip.Show($"英雄卡已满({CombatConst.PlayerMaxHeroCards}张)，无法购买新英雄");
        return false;
    }

    public PlayerInfo GetCurrentPlayer()
    {
        return Mode?.GetActingPlayer();
    }

    public void QuickView(int pid)
    {
        if (pid >= 0)
            mySelect.QuickView(GameManager.Instance.GetPlayer(pid));
        else
            mySelect.QuickViewFin();
    }

    // 商店开始时：按积分(mark)从低到高排序玩家，积分相同金币少的排前，再相同按pid排
    // 同时调整玩家位置（第1名排在最左边），回合顺序也按该排序
    private void SortPlayersByScore()
    {
        var players = GameManager.Instance.players;

        // 槽位位置：按当前X坐标从左到右排列，即第1名位置在最左
        var slotPos = players
            .Select(p => p.GetComponent<RectTransform>().anchoredPosition)
            .OrderBy(pos => pos.x)
            .ToArray();

        TurnOrder = players
            .OrderBy(p => p.mark)
            .ThenBy(p => p.gold)
            .ThenBy(p => p.pid)
            .Select(p => p.pid)
            .ToArray();

        for (int i = 0; i < TurnOrder.Length; i++)
        {
            players[TurnOrder[i]].GetComponent<RectTransform>().anchoredPosition = slotPos[i];
        }
    }

    private void CheckEraBonusGold()
    {
        if(GameManager.Instance.year <= 2)
            return;

        // 获取所有玩家
        var players = new List<(int id, int gold)>();
        foreach (var player in GameManager.Instance.players)
            players.Add((player.pid, player.gold));

        // 按金币数量升序排序
        players.Sort((a, b) => a.gold.CompareTo(b.gold));

        if(players[0].gold == players[1].gold && players[1].gold == players[2].gold)
        {
            
        }
        else
        {
            if (players[0].gold < players[1].gold)
            {
                GameManager.Instance.GetPlayer(players[0].id).AddGold(2);
                if (players[1].gold < players[2].gold)
                {
                    GameManager.Instance.GetPlayer(players[1].id).AddGold(2);
                    GameManager.Instance.GetPlayer(players[2].id).AddGold(1);
                }
                else
                {
                    GameManager.Instance.GetPlayer(players[1].id).AddGold(1);
                    GameManager.Instance.GetPlayer(players[2].id).AddGold(1);
                }
            }
            else
            {
                GameManager.Instance.GetPlayer(players[0].id).AddGold(2);
                GameManager.Instance.GetPlayer(players[1].id).AddGold(2);
                // 第三名(players[2])不加金币，原来AddGold(0)会触发异常
            }
        }
    }

    // 调试用：给人类玩家按掉落池（ItemConfig 的回合门槛 + 权重）随机发放 10 个道具，仅 Debug.isDebugBuild 生效
    public static void DebugGiveRandomItems()
    {
        if (!Debug.isDebugBuild)
            return;

        var p1 = GameManager.Instance.GetPlayer(0);
        // 调试：先清空背包中未装备的道具（保留已装备的），避免每轮累积把装备区塞满
        int cleared = p1.items.RemoveAll(slot => slot.HeroId == 0);
        if (cleared > 0)
            GameLog.Debug($"调试清理：玩家0 清除背包中 {cleared} 件未装备道具");
        int round = Math.Max(1, GameManager.Instance.year + 1); // 商店阶段对应的是即将开打的回合
        int given = 0;
        for (int i = 0; i < 6; i++)
        {
            int itemId = WorldManager.RollDropItem(round);
            if (itemId <= 0)
                continue;
            p1.AddItemCard(itemId);
            given++;
        }

        if (given > 0)
            GameLog.Debug($"调试随机道具：玩家0 按掉落规则获得 {given} 个（回合{round}）");

        // 调试：额外固定发放新增消耗品（经验书 / 分解锤），不受回合门槛限制，方便测试
        int[] extraItemIds = { 401010, 401011 };
        foreach (int extraItemId in extraItemIds)
            p1.AddItemCard(extraItemId);
        GameLog.Debug("调试固定道具：玩家0 获得 无字天书(401010) + 分解锤(401011)");
    }

    private void BeginEraCommon()
    {
        // 新商店阶段的公共开场：重置回合/era 状态、记录开局金币、落后补金
        foreach (var player in GameManager.Instance.players)
            player.OnEra(era);

        eraText.text = ShopCfg.Name;

        for (int i = 0; i < 8; i++)
            GameManager.Instance.GetPlayer(i).SetRoundOver(false);

        for (int i = 0; i < 8; i++)
            playerStartGold[i] = GameManager.Instance.GetPlayer(i).gold; // 记录开局金币

        CheckEraBonusGold();

        era++;

        GameManager.Instance.PlaySound("Sounds/page");
    }

    private ShopMode CreateMode(ShopModeType type)
    {
        switch (type)
        {
            case ShopModeType.Independent:
                return new IndependentShopMode();
            case ShopModeType.Shared:
            default:
                return new SharedShopMode();
        }
    }

    public void ShopBegin()
    {
        GameLog.Debug("ShopBegin");

        // 新商店阶段：把刷牌权重表重置为全 1（每张卡首次全随机），权重不参与存档，读档后同样回到全 1
        HeroSelectionTool.ResetShopWeights();

        // 调试：每次商店阶段开始（含开局与战后回商店）给人类玩家按掉落规则随机发 10 个道具
        DebugGiveRandomItems();

        if(hasEnterBattle) //存档拉起进入游戏，不会重复存储
            GameManager.Instance.SaveToFile();

        // 商店阶段 BGM：shop1~shop4 随机一首
        BGMPlayer.Instance.PlaySound("BGMs/shop" + (SysRandom.Range(0, 4) + 1));

        if (GameManager.Instance.year == 0)
        {
            GameLog.Debug("FirstRound ck");
            for(int i = 0; i < 8; i++)
                GameManager.Instance.GetPlayer(i).FirstRound();
        }

        var shopOpenIndex = GameManager.Instance.year; //第几场比赛
        ShopCfg = GameRoundConfig.GetConfig(Math.Min(100, shopOpenIndex + 1));
        var roundGold = ShopCfg.RoundGold;
        for(int i = 0; i < 8; i++)
            GameManager.Instance.GetPlayer(i).RoundGold(roundGold);

        // 每回合商店阶段重置"当回合已买入卡"记录：当回合买入的新卡当回合不卖
        for (int i = 0; i < 8; i++)
            GameManager.Instance.GetPlayer(i).boughtThisShopCards.Clear();

        SortPlayersByScore();

        // AI进商店：先把背包里的材料装备随机两两合成高级装备，再给英雄穿装备、自动使用消耗品（三方法内部只对 AI 生效）
        for (int i = 0; i < 8; i++)
        {
            var aiPlayer = GameManager.Instance.GetPlayer(i);
            aiPlayer.AutoCombineItems();
            aiPlayer.AutoEquipItems();
            aiPlayer.AutoUseItems();
        }

        GameManager.Instance.year++;
        era = 0;

        BeginEraCommon();

        // 依据本回合配置选择商店模式（0=共享轮换，1=独立买卡）
        Mode = CreateMode((ShopModeType)ShopCfg.ShopType);
        Mode.Bind(this);

        // 回合结束后、买卡前：AI 经验检查（总经验低于期望时按概率花金币买经验；先于商店生成，升级可影响本回合商店品质）
        for (int i = 1; i < 8; i++)
        {
            var aiP = GameManager.Instance.GetPlayer(i);
            if (aiP.isAI)
                PlayerAI.ShopBegin(aiP);
        }

        Mode.Begin();
        RefreshRateText(); // 新回合开始刷新稀有度概率文本
        shopCoroutine = StartCoroutine(Mode.Drive());
    }

    /// <summary>由商店模式在本阶段结束时调用：进入战斗</summary>
    public void RequestEnd()
    {
        if (IsShopEnd)
            return;
        IsShopEnd = true;
        StartCoroutine(ShopEnd());
    }

    private IEnumerator ShopEnd()
    {
        IsShopEnd = true;

        yield return new WaitForSeconds(0.5f);
        if(shopCoroutine != null)
            StopCoroutine(shopCoroutine);
        shopCoroutine = null;

        Mode?.End();

        GameManager.Instance.ClearTurn();

        var movingCardImages = GameObject.FindGameObjectsWithTag("MovingCard");
        foreach(var img in movingCardImages)
            Destroy(img);

        PanelManager.Instance.GetTooltip<BaseTooltip>()?.HideTooltip();
        PanelManager.Instance.HideShop();
        // 商店结束进入战斗
        GameManager.Instance.PlaySound("Sounds/biang");
        WorldManager.Instance.BattleBegin(); 
        hasEnterBattle = true;

        for(int i = 0; i < 8; i++)
            GameManager.Instance.GetPlayer(i).SetRoundOver(false);
    }
}
