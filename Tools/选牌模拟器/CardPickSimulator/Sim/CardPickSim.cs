// ============================================================
// 选牌模拟器 · 驱动器
// 复用游戏真实源码（CardShopManager / ShopMode / PlayerAI / PlayerInfo /
// CardViewControl），以无头方式驱动 N 个 AI 玩家在随机商店中逐回合买牌，
// 并记录每名 AI 的买入/卖出行为。
//
// 与真机的对应关系：
//   - 真机：CardShopManager.Start() → ShopBegin() → 协程 Mode.Drive() 驱动回合
//   - 模拟器：手工构建 8 名玩家/商店宿主，调用 ShopBegin()，用 CoroutineRunner 推进协程；
//             商店结束（WorldManager.BattleBegin）后不开战，直接进入下一回合商店。
// ============================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class CardPickSim
{
    /// <summary>模拟回合数</summary>
    public int Rounds = 30;
    /// <summary>每步推进的逻辑时间（与批量战斗模拟器一致的 0.05s 步长）</summary>
    private const float StepDt = 0.05f;
    /// <summary>单回合最大推进步数（防死循环）</summary>
    private const int MaxStepsPerRound = 20000;
    /// <summary>独立买卡模式下 pid0 代打的最大购买次数</summary>
    private const int MaxHumanShopBuys = 30;

    /// <summary>日志回调（逐行输出到界面）</summary>
    public Action<string> OnLog;
    /// <summary>运行结束回调（success, message）</summary>
    public Action<bool, string> OnFinished;

    private CardShopManager shop;
    private readonly List<PlayerState> states = new List<PlayerState>();
    private string logFilePath;

    // 每名玩家的上一帧快照：gold + 卡牌数量 + 等级经验，用于差分出买入/卖出/刷新/买经验
    private class PlayerState
    {
        public PlayerInfo info;
        public int lastGold;
        public Dictionary<int, int> lastCards = new Dictionary<int, int>();
        public int lastExp;
        public int lastLevel;
        public int buyCount;
        public int sellCount;
        public int refreshCount;
        public int buyExpCount;
    }

    // 统计
    private int totalBuys;
    private int totalSells;
    private int totalRefreshes;
    private int totalBuyExps;

    public void Run()
    {
        try
        {
            Log("========== 选牌模拟器开始 ==========");
            Log($"模拟回合数：{Rounds}，玩家数：8（全部 AI）");

            // 1) 配置初始化（与游戏启动一致）
            ConfigManager.Init();
            Log("配置加载完成");

            // 2) 好友/英雄池（与游戏 GameManager 启动一致）
            BuildGameManager();
            Log($"英雄池构建完成，本局可出英雄 {GameManager.Instance.heroIds.Count} 名");

            // 3) 构建无头场景：8 名玩家 + 世界 + 面板 + 商店宿主
            BuildScene();

            // 4) 逐回合驱动商店
            for (int round = 1; round <= Rounds; round++)
            {
                RunOneRound(round);
            }

            // 5) 结算
            PrintSummary();
            Log("========== 选牌模拟器结束 ==========");
            OnFinished?.Invoke(true, $"完成 {Rounds} 回合模拟，买入 {totalBuys} 次 / 卖出 {totalSells} 次 / 刷新 {totalRefreshes} 次 / 买经验 {totalBuyExps} 次");
        }
        catch (Exception e)
        {
            Log("[异常] " + e);
            OnFinished?.Invoke(false, "模拟异常：" + e.Message);
        }
    }

    // ---------------- 场景构建 ----------------

    private void BuildGameManager()
    {
        GameManager.Instance = new GameManager();
        GameManager.Instance.InitFriend(false);
        GameManager.Instance.InitHeros(false);
    }

    private void BuildScene()
    {
        // 世界管理器：SkipBattle=true，BattleBegin 只作为"回合结束"信号
        var worldGo = new GameObject("WorldManager");
        var world = worldGo.AddComponent<WorldManager>();
        world.uiCamera = worldGo.AddComponent<Camera>();
        WorldManager.Instance = world;
        WorldManager.SkipBattle = true;
        WorldManager.BattleBeginCount = 0;

        // 面板管理器：提供 Tooltip/面板开关空实现
        var panelGo = new GameObject("PanelManager");
        var panel = panelGo.AddComponent<PanelManager>();
        panel.cardShopPanel = new GameObject("CardShopPanel");
        PanelManager.Instance = panel;

        // 商店宿主：预制体/按钮/文本/选择区全部用哑对象占位
        var shopGo = new GameObject("CardShopManager");
        shop = shopGo.AddComponent<CardShopManager>();
        shop.cardViewPrefab = BuildCardViewPrefab();
        shop.passBtn = BuildButton(shopGo, "PassBtn");
        shop.refreshBtn = BuildButton(shopGo, "RefreshBtn");
        shop.bagBtn = BuildButton(shopGo, "BagBtn");
        shop.rankBtn = BuildButton(shopGo, "RankBtn");
        shop.rankPlayerBtn = BuildButton(shopGo, "RankPlayerBtn");
        shop.eraText = BuildText(shopGo, "EraText");
        shop.rateText = BuildText(shopGo, "RateText");
        shop.mySelect = shopGo.AddComponent<MySelectControl>();
        CardShopManager.Instance = shop;

        // 8 名玩家：与真机 GameManager.Start 一致 —— pid0 固定为王（GetWang），其余 7 人用
        // PlayerBook.GetRandomN(7) 随机抽取（4 个 CanPlay + 3 个非 CanPlay），保证每局 AI 性格/偏好随机
        var playersRoot = new GameObject("Players");
        var players = new PlayerInfo[8];
        players[0] = BuildPlayer(playersRoot, 0, PlayerBook.GetWang());
        var pls = PlayerBook.GetRandomN(7);
        for (int i = 0; i < 7; i++)
            players[i + 1] = BuildPlayer(playersRoot, i + 1, pls[i]);
        GameManager.Instance.players = players;

        // 差分快照初始化
        states.Clear();
        foreach (var p in players)
        {
            var st = new PlayerState { info = p, lastGold = p.gold };
            st.lastCards = new Dictionary<int, int>(p.cards);
            states.Add(st);
        }
    }

    private static GameObject BuildChild(GameObject parent, string name)
    {
        var go = new GameObject(name);
        go.AddComponent<RectTransform>();
        if (parent != null)
            go.transform.SetParent(parent.transform);
        return go;
    }

    private static TMP_Text BuildText(GameObject parent, string name)
    {
        var go = BuildChild(parent, name);
        return go.AddComponent<TextMeshProUGUI>();
    }

    private static Image BuildImage(GameObject parent, string name)
    {
        var go = BuildChild(parent, name);
        return go.AddComponent<Image>();
    }

    // 技能图标：图标 Image 外套一层容器（CardViewControl.Init 通过 transform.parent 控制整格显隐）
    private static Image BuildSkillIcon(GameObject parent, string name)
    {
        var container = BuildChild(parent, name + "Root");
        return BuildImage(container, name);
    }

    private static Button BuildButton(GameObject parent, string name)
    {
        var go = BuildChild(parent, name);
        go.AddComponent<TMP_Text>(); // SetPassButtonLabel 会取子文本
        return go.AddComponent<Button>();
    }

    // 构造一张"卡位"哑预制体：填满 CardViewControl.Init/OnSold/UpdateEffects 会用到的字段
    private static GameObject BuildCardViewPrefab()
    {
        var go = new GameObject("CardViewPrefab");
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>(); // Init 会 GetComponent<Image>().color = 阵营色
        var cv = go.AddComponent<CardViewControl>();

        cv.soldImage = BuildImage(go, "SoldImage");
        cv.cardName = BuildText(go, "CardName");
        cv.price = BuildText(go, "PriceText");
        cv.roundLeftText = null;      // 无倒计时节点：Init 会跳过 roundLeft 计算
        cv.roundLeftIconNode = null;
        cv.buyButton = BuildButton(go, "BuyBtn");
        cv.isHeroCardNode = BuildChild(go, "HeroNode");
        cv.isItemCardNode = BuildChild(go, "ItemNode");
        cv.heroImage = BuildImage(go, "HeroImage");
        // 技能图标槽位（4 个）：slot0=职业兵种 / slot1=技能分类(stXXXX) / slot2·3=好友组技能。
        // CardViewControl.Init 用 heroSkillImage[i].transform.parent 控制整格显隐，故每个图标各套一层容器，
        // 避免 SetActive 误命中预制体根节点
        cv.heroSkillImage = new Image[]
        {
            BuildSkillIcon(go, "SkillIcon0"),
            BuildSkillIcon(go, "SkillIcon1"),
            BuildSkillIcon(go, "SkillIcon2"),
            BuildSkillIcon(go, "SkillIcon3"),
        };
        cv.itemImage = BuildImage(go, "ItemImage");
        cv.effectGreen = BuildChild(go, "EffectGreen");
        cv.effectYellow = BuildChild(go, "EffectYellow");
        cv.effectGray = BuildChild(go, "EffectGray");
        cv.effectLayer = BuildChild(go, "EffectLayer");
        return go;
    }

    private static PlayerInfo BuildPlayer(GameObject parent, int pid, int playerConfigId)
    {
        var go = BuildChild(parent, "Player" + pid);
        var info = go.AddComponent<PlayerInfo>();

        // UpdateView 需要用到这些文本/图（全部哑对象）
        info.playerNameText = BuildText(go, "NameText");
        info.playerLevelText = BuildText(go, "LevelText");
        info.goldText = BuildText(go, "GoldText");
        info.resultText = BuildText(go, "ResultText");
        info.playerImage = BuildImage(go, "PlayerImage");
        info.playerBgImg = BuildImage(go, "PlayerBg");
        info.roundOverImg = BuildImage(go, "RoundOver");

        // Init(pid, playerId)：playerId 决定 PlayerConfig（AI 性格/偏好）
        info.Init(pid, playerConfigId);
        // 全部作为 AI 参与（模拟 8 个 AI 玩家）
        info.isAI = true;
        return info;
    }

    // ---------------- 回合驱动 ----------------

    private void RunOneRound(int round)
    {
        int before = WorldManager.BattleBeginCount;

        // 记录买经验前各玩家的总经验：ShopBegin 内 AI 会做经验检查并可能买经验。
        // 该阶段不发生战斗，总经验增量只能来自 PlayerInfo.BuyExp（4金=4经验，花费=总经验增量）
        var preTotalExp = new int[states.Count];
        for (int i = 0; i < states.Count; i++)
            preTotalExp[i] = states[i].info.GetTotalExp();

        // 开始本回合商店（内部完成加钱/排序/year++/AI经验检查/生成商店/启动 Mode.Drive 协程）
        shop.ShopBegin();

        // 差分出 AI 买经验事件
        for (int i = 0; i < states.Count; i++)
        {
            var st = states[i];
            int after = st.info.GetTotalExp();
            int gained = after - preTotalExp[i];
            if (gained <= 0)
                continue;
            int times = Math.Max(1, gained / CombatConst.ExpBuyAmount);
            Log($"  [第{round}回合] P{st.info.pid} 买经验x{times}（花费{gained}金，总经验{preTotalExp[i]}→{after}）");
            st.buyExpCount += times;
            totalBuyExps += times;
        }

        // 先快照：否则随后同步执行的"人类代打"买卖会被这份快照吞掉，导致 P0 买卖统计漏计
        CaptureSnapshots();

        // 独立买卡模式：pid0 是"人类"，用同一套 AI 评分代打后点"结束"
        if (shop.Mode is IndependentShopMode)
        {
            DriveHumanByAI();
            DiffAndLog(round, -1); // 立即结算人类代打阶段的买卖
        }

        // 推进协程，直到本回合结束（WorldManager.BattleBegin 被调用）
        int steps = 0;
        while (WorldManager.BattleBeginCount == before)
        {
            UnityEngine.Time.Advance(StepDt);
            CoroutineRunner.Step();
            DiffAndLog(round, steps);

            if (++steps > MaxStepsPerRound)
            {
                Log($"[警告] 第 {round} 回合推进超过 {MaxStepsPerRound} 步仍未结束，强制中断本回合");
                break;
            }
        }

        // 让 ShopEnd 协程的收尾代码（hasEnterBattle 等）有机会跑完
        for (int i = 0; i < 20; i++)
        {
            UnityEngine.Time.Advance(StepDt);
            CoroutineRunner.Step();
        }

        Log($"[第 {round} 回合] 商店阶段结束");

        // 商店结束后的战斗结算：按真机规则近似推进等级（复用游戏真实经验曲线）
        AdvanceLevelOneRound(round);
    }

    // ---------------- 等级近似推进 ----------------

    // 真机规则：每回合商店后进入一场战斗，胜+3经验/负+2经验（CombatConst.BattleWinExp/LoseExp），
    // 经验曲线与升级逻辑完全复用 PlayerInfo.AddExp / onBattleResult，仅"胜负"为近似：
    // 8 名玩家洗牌后两两配对，按阵容战力(价格×卡等级)占比决定胜率（收敛到 0.3~0.7 避免碾压）。
    private void AdvanceLevelOneRound(int round)
    {
        int n = states.Count;
        var order = new List<int>(n);
        var power = new int[n];
        for (int i = 0; i < n; i++)
        {
            order.Add(i);
            power[i] = CalcPlayerPower(states[i].info);
        }

        // 洗牌（SysRandom，保证可复现）
        for (int i = n - 1; i > 0; i--)
        {
            int j = SysRandom.Range(0, i + 1);
            int tmp = order[i]; order[i] = order[j]; order[j] = tmp;
        }

        for (int k = 0; k + 1 < n; k += 2)
        {
            int a = order[k];
            int b = order[k + 1];
            float winP = 0.5f;
            int total = power[a] + power[b];
            if (total > 0)
                winP = (float)power[a] / total;
            winP = Mathf.Clamp(winP, 0.3f, 0.7f);

            bool aWin = SysRandom.Range(0f, 1f) < winP;
            SettleBattle(states[a].info, aWin, round);
            SettleBattle(states[b].info, !aWin, round);
        }
    }

    private void SettleBattle(PlayerInfo p, bool isWin, int round)
    {
        int lvBefore = p.level;
        // mark 为名次分（胜者10，负者按死亡顺序1~7），仅影响展示，不参与经验
        int mark = isWin ? 10 : SysRandom.Range(1, 6);
        p.onBattleResult(isWin, mark); // 复用游戏逻辑：结算 + 经验（胜+3/负+2）+ 升级
        if (p.level != lvBefore)
            Log($"  [第{round}回合] P{p.pid} 战斗{(isWin ? "胜利" : "失败")} 升级 {lvBefore}→{p.level}级（上阵格 {p.GetSlotCount()}）");
    }

    // 阵容战力：复用 PlayerInfo.GetStrongCardList（价格×卡等级），与游戏自动上阵口径一致
    private static int CalcPlayerPower(PlayerInfo p)
    {
        int sum = 0;
        foreach (var t in p.GetStrongCardList(p.GetSlotCount()))
        {
            var cfg = HeroConfig.GetConfig(t.Item1);
            if (cfg == null)
                continue;
            sum += HeroSelectionTool.GetPrice(cfg) * t.Item2;
        }
        return sum;
    }

    // 独立买卡模式：pid0 用 AI 评分逻辑买牌，然后结束（等价于人类点"结束"）
    // 刷新次数与 AI(pid1~7) 对齐：买不动且还有余钱时刷新自己 5 格再重试，保证 P0 与 AI 购买机会对等
    private void DriveHumanByAI()
    {
        var human = GameManager.Instance.GetPlayer(0);
        int refreshes = 0;
        int guard = 0;
        while (guard++ < MaxHumanShopBuys)
        {
            var offers = shop.Mode.GetOffers(0);
            // 复用游戏 AI 选牌/购买（返回真实购买结果，买得动就继续买）
            if (offers != null && offers.Count > 0 && PlayerAI.AiCheckBuyCard(human, shop.Era, offers))
                continue;

            // 买不动且还有余钱：刷新自己的 5 格后重试（与 IndependentShopMode 中 AI 的处理一致）
            if (refreshes < CombatConst.AiMaxShopRefreshPerShop
                && human.gold >= CardShopManager.RefreshGoldCost * 2)
            {
                shop.Mode.OnHumanRefresh();
                refreshes++;
                continue;
            }
            break;
        }
        shop.Mode.OnHumanPass();
    }

    private void CaptureSnapshots()
    {
        foreach (var st in states)
        {
            st.lastGold = st.info.gold;
            st.lastCards = new Dictionary<int, int>(st.info.cards);
            st.lastExp = st.info.exp;
            st.lastLevel = st.info.level;
        }
    }

    // 逐帧比对玩家金币/卡牌变化，差分出买入、卖出、刷新（买经验在 RunOneRound 开场单独记账）
    // 说明：AI 一个 Step 内会连续买卡并刷新（协程中间不 yield），无法逐次拦截，
    // 故"刷新"用金币收支反推：净支出 - 买卡支出 + 卖卡退款 = 刷新次数 × RefreshGoldCost
    private void DiffAndLog(int round, int step)
    {
        foreach (var st in states)
        {
            var info = st.info;
            var now = info.cards;
            int pid = info.pid;

            // 滚动金币：每笔操作后更新，逐行展示该笔操作自身的金币收支；末行应收敛到 info.gold
            int cur = st.lastGold;
            int buySpent = 0;    // 买卡支出（按单价×数量）
            int sellRefund = 0;  // 卖卡退款（按游戏卖卡公式）

            // 卖出：上次有、现在变少或消失
            foreach (var kv in st.lastCards)
            {
                int cardId = kv.Key;
                int oldCount = kv.Value;
                int newCount = now.TryGetValue(cardId, out var c) ? c : 0;
                if (newCount < oldCount)
                {
                    int sold = oldCount - newCount;
                    int refund = CalcSellRefund(GetCardPrice(cardId), sold);
                    int prev = cur;
                    cur += refund;
                    Log($"  [第{round}回合] P{pid} 卖出 {CardName(cardId)}x{sold}（原有{oldCount} → 现{newCount}，+{refund}金）金币 {prev}→{cur}");
                    st.sellCount += sold;
                    totalSells += sold;
                    sellRefund += refund;
                }
            }

            // 买入：现在比上次多
            foreach (var kv in now)
            {
                int cardId = kv.Key;
                int newCount = kv.Value;
                int oldCount = st.lastCards.TryGetValue(cardId, out var c) ? c : 0;
                if (newCount > oldCount)
                {
                    int bought = newCount - oldCount;
                    int cost = GetCardPrice(cardId) * bought;
                    int prev = cur;
                    cur -= cost;
                    Log($"  [第{round}回合] P{pid} 买入 {CardName(cardId)}x{bought}（原有{oldCount} → 现{newCount}，-{cost}金）金币 {prev}→{cur}");
                    st.buyCount += bought;
                    totalBuys += bought;
                    buySpent += cost;
                }
            }

            // 买经验：AI 买经验发生在 ShopBegin（经验检查）内，已在 RunOneRound 开场单独差分记录，
            // 此处的逐帧比对阶段（Mode.Drive）不会再产生经验变化

            // 刷新：由金币收支反推（扣掉买卡/卖卡后的净支出即为刷新花费），
            // 用实际净支出滚动以保证收支连续、末行等于 info.gold
            int spent = st.lastGold - info.gold;
            int refreshGold = spent - (buySpent - sellRefund);
            if (refreshGold > 0)
            {
                int refreshes = (int)Math.Round(refreshGold / (double)CardShopManager.RefreshGoldCost);
                if (refreshes > 0)
                {
                    int prev = cur;
                    cur -= refreshGold;
                    Log($"  [第{round}回合] P{pid} 刷新商店x{refreshes}（消耗 {refreshGold} 金，金币 {prev}→{cur}）");
                    st.refreshCount += refreshes;
                    totalRefreshes += refreshes;
                }
            }

            st.lastGold = info.gold;
            st.lastCards = new Dictionary<int, int>(now);
            st.lastExp = info.exp;
            st.lastLevel = info.level;
        }
    }

    // 卡牌单价（与商店出价口径一致）
    private static int GetCardPrice(int cardId)
    {
        var cfg = HeroConfig.GetConfig(cardId);
        return cfg != null ? HeroSelectionTool.GetPrice(cfg) : 0;
    }

    // 游戏卖卡退款公式：总价<=5 全额返还，否则 totalValue - (totalValue-1)/5
    private static int CalcSellRefund(int price, int count)
    {
        int totalValue = price * count;
        return totalValue <= 5 ? totalValue : totalValue - (totalValue - 1) / 5;
    }

    private static string CardName(int cardId)
    {
        if (ConfigManager.IsHeroCard(cardId))
        {
            var cfg = HeroConfig.GetConfig(cardId);
            if (cfg != null)
                return $"{cfg.Name}({cardId})";
        }
        return cardId.ToString();
    }

    // 仅英雄名（结算展示用，不带卡ID）
    private static string HeroName(int cardId)
    {
        var cfg = HeroConfig.GetConfig(cardId);
        return cfg != null ? cfg.Name : cardId.ToString();
    }

    // 英雄名 + 卡等级与升级进度：如 孙坚Lv3(3/5) —— 3级、本档内经验3、升到4级还需5
    // cards[cardId] 存的是累计卡数(经验)，等级阈值见 HeroSelectionTool（1/4/8/13/20）
    private static string HeroNameWithLevel(PlayerInfo p, int cardId)
    {
        int exp = p.cards.TryGetValue(cardId, out var e) ? e : 0;
        int level = HeroSelectionTool.GetCardLevel(exp, true);
        // 命中玩家喜好（LikeForce 势力 / LikeJob 职业 / LikeFriend 好友组）的英雄名前置 ★
        string name = (IsLikedByPlayer(p, cardId) ? "★" : "") + HeroName(cardId);
        if (level >= HeroSelectionTool.MaxHeroCardLevel)
            return $"{name}Lv{level}(MAX)";
        if (level <= 0)
            return $"{name}Lv1(0/{HeroSelectionTool.GetCardExpByLevel(2) - HeroSelectionTool.GetCardExpByLevel(1)})";
        int start = HeroSelectionTool.GetCardExpByLevel(level);      // 本档起始经验
        int next = HeroSelectionTool.GetCardExpByLevel(level + 1);   // 下一档所需累计经验
        return $"{name}Lv{level}({exp - start}/{next - start})";
    }

    // 该英雄是否命中玩家喜好：LikeForce(势力) / LikeJob(职业) / LikeFriend(好友组)，判定口径同 PlayerAI.LikeHas*
    private static bool IsLikedByPlayer(PlayerInfo p, int cardId)
    {
        var cfg = HeroConfig.GetConfig(cardId);
        if (cfg == null)
            return false;

        var likeForce = p.playerConfig.LikeForce;
        if (likeForce != null && likeForce.Length > 0 && Array.IndexOf(likeForce, cfg.Side) >= 0)
            return true;

        // 职业：HeroConfig.Job 是 JobConfig.NameS，需映射回 JobConfig.Id 再比对 LikeJob
        var likeJob = p.playerConfig.LikeJob;
        if (likeJob != null && likeJob.Length > 0)
        {
            foreach (var jobCfg in JobConfig.ConfigList)
            {
                if (jobCfg.NameS == cfg.Job && Array.IndexOf(likeJob, jobCfg.Id) >= 0)
                    return true;
            }
        }

        // 好友组：英雄所属的任一好友组 Id 命中 LikeFriend 即算
        var likeFriend = p.playerConfig.LikeFriend;
        if (likeFriend != null && likeFriend.Length > 0)
        {
            var relIds = ConfigManager.GetHeroFriendInfo(cardId);
            if (relIds != null)
            {
                foreach (var relId in relIds)
                    if (Array.IndexOf(likeFriend, relId) >= 0)
                        return true;
            }
        }
        return false;
    }

    // 上阵阵容羁绊（仅列 Lv2 及以上），口径与游戏 MySelectControl 一致：
    //   职业：等级 = 上阵同职业人数（1人=Lv1）
    //   国家：等级 = 上阵同阵营人数 - 1（1人=0级，仅参与同阵营护盾的阵营计入）
    //   好友：等级 = 好友组上阵成员数 - 1（1人=0级）
    private static string BuildBondSummary(List<int> lineupIds)
    {
        var parts = new List<string>();

        // 职业（同职业人数）
        var jobCounts = new Dictionary<string, int>();
        foreach (var id in lineupIds)
        {
            var cfg = HeroConfig.GetConfig(id);
            if (cfg == null)
                continue;
            jobCounts[cfg.Job] = jobCounts.TryGetValue(cfg.Job, out var jc) ? jc + 1 : 1;
        }
        foreach (var kv in jobCounts)
        {
            if (kv.Value < 2)
                continue; // 职业等级=人数，Lv2 起列
            var jobCfg = ConfigManager.GetJobConfig(kv.Key);
            parts.Add($"职业·{(jobCfg != null ? jobCfg.Name : kv.Key)}Lv{kv.Value}");
        }

        // 国家（同阵营人数 - 1，仅参与同阵营护盾的阵营）
        var sideCounts = new Dictionary<int, int>();
        foreach (var id in lineupIds)
        {
            var cfg = HeroConfig.GetConfig(id);
            if (cfg == null)
                continue;
            sideCounts[cfg.Side] = sideCounts.TryGetValue(cfg.Side, out var sc) ? sc + 1 : 1;
        }
        foreach (var kv in sideCounts)
        {
            var forceCfg = ConfigManager.GetForceConfig(kv.Key);
            if (forceCfg == null || !forceCfg.JoinFactionShield)
                continue;
            int lv = kv.Value - 1;
            if (lv < 2)
                continue;
            parts.Add($"国家·{forceCfg.Name}Lv{lv}");
        }

        // 好友（好友组上阵成员数 - 1）
        foreach (var friendCfg in HeroFriendConfig.ConfigList)
        {
            int present = 0;
            foreach (var mid in friendCfg.Heros)
                if (lineupIds.Contains(mid))
                    present++;
            int lv = present - 1;
            if (lv < 2)
                continue;
            parts.Add($"好友·{friendCfg.Name}Lv{lv}");
        }

        return parts.Count > 0 ? string.Join(" / ", parts) : null;
    }

    // ---------------- 结算与日志 ----------------

    private void PrintSummary()
    {
        Log("");
        Log("---------- 玩家结算 ----------");
        foreach (var st in states)
        {
            var p = st.info;
            int heroCards = p.GetHeroCardList().Count;
            var lineup = p.GetStrongCardList(p.GetSlotCount());

            // 上阵阵容（按战力取前 上阵格 张）
            var lineupIds = new List<int>();
            var lineupNames = new List<string>();
            foreach (var t in lineup)
            {
                lineupIds.Add(t.Item1);
                lineupNames.Add(HeroNameWithLevel(p, t.Item1));
            }

            // 未上阵阵容（拥有的英雄卡 - 上阵）
            var benchNames = new List<string>();
            foreach (int cardId in p.GetHeroCardList())
            {
                if (!lineupIds.Contains(cardId))
                    benchNames.Add(HeroNameWithLevel(p, cardId));
            }

            int expToNext = p.GetExpToNext();
            string levelText = expToNext > 0 ? $"Lv{p.level} {p.exp}/{expToNext}" : $"Lv{p.level} MAX";

            Log($"P{p.pid}（{p.playerConfig.Name}） {levelText} 上阵格={p.GetSlotCount()} 金币={p.gold} 英雄卡={heroCards}张 " +
                $"买入={st.buyCount} 卖出={st.sellCount} 刷新={st.refreshCount} 买经验={st.buyExpCount}");
            Log($"    上阵阵容({lineupNames.Count}/{p.GetSlotCount()})：{(lineupNames.Count > 0 ? string.Join("、", lineupNames) : "无")}");
            Log($"    未上阵阵容({benchNames.Count})：{(benchNames.Count > 0 ? string.Join("、", benchNames) : "无")}");

            // 上阵阵容羁绊（仅列 Lv2 及以上）：职业 / 国家 / 好友关系
            string bondText = BuildBondSummary(lineupIds);
            if (bondText != null)
                Log($"    羁绊(Lv≥2)：{bondText}");
        }
        Log("------------------------------");
    }

    private void Log(string line)
    {
        OnLog?.Invoke(line);
        if (logFilePath != null)
        {
            try { File.AppendAllText(logFilePath, line + Environment.NewLine, Encoding.UTF8); }
            catch { }
        }
    }

    /// <summary>日志文件路径（在模拟器目录 Logs/ 下，随模拟生成）</summary>
    public string LogFilePath => logFilePath;

    public void InitLogFile(string dir)
    {
        try
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            logFilePath = Path.Combine(dir, $"CardPickSim_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            File.WriteAllText(logFilePath, "", Encoding.UTF8);
        }
        catch
        {
            logFilePath = null;
        }
    }
}
