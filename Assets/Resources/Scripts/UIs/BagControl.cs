using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BagControl : MonoBehaviour, IPanelEvent
{
    public Button closeBtn;

    public Button fieldAutoBtn;
    public Button aiSwitchBtn;


    // Start is called before the first frame update
    // 声明一个列表用于缓存 cell 对象
    private List<GameObject> cellCache = new List<GameObject>();
    public GameObject bagHeroRegion;
    public GameObject bagItemRegion;
    public GameObject fieldRegion;
    public BagRecycler bagRecycler;
    public BagRecycler bagUnwear; // 卸装区：拖英雄过来脱下所有装备
    public BagRecycler bagCompose;
    public TMP_Text infoText;
    public TMP_Text expText;
    public Image expBar;
    public Button buyExpBtn;
    public TMP_Text sodInfoText;
    public Button sodLvupBtn;

    public PlayerInfo bindPlayer;
    public MySelectControl mySelect;

    void Start()
    {
        // bindPlayer = GameManager.Instance.GetPlayer(0);
        // bindPlayer.cards[ 101003 ] = 1;         
        // bindPlayer.cards[  101011 ] = 1;         
        //   bindPlayer.cards[103003] = 1;      

        infoText.raycastTarget = false;
        OnShow();

        closeBtn.onClick.AddListener(() =>
        {      
            PanelManager.Instance.GetTooltip<BaseTooltip>()?.HideTooltip();
            DestroyAllCells();
            PanelManager.Instance.HideBag();
            CardShopManager.Instance.OnShow();
        });
        fieldAutoBtn.onClick.AddListener(() =>
        {
            var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);
            p1.AutoSetBattleCard();
            UpdateFieldView();
            UpdateExpView();

            // 调试辅助：上阵英雄少于5时，直接把补齐 job/force/friend 羁绊的英雄卡放进背包（各到2~3级）
            // 仅编辑器/开发版生效（Debug.isDebugBuild），正式包不触发
            if (Debug.isDebugBuild)
            {
                var heroOnField = p1.battleCards.Count(c => c > 0 && ConfigManager.IsHeroCard(c));
                if (heroOnField < 5)
                    DebugAddCardsForBonds(p1);
            }

            GameManager.Instance.PlaySound("Sounds/equip");
        });
        aiSwitchBtn.onClick.AddListener(() =>
        {
            bindPlayer.isAI = !bindPlayer.isAI;
            aiSwitchBtn.GetComponentInChildren<TMP_Text>().text = bindPlayer.isAI ? "AI模式" : "玩家模式";
            if (bagRecycler != null)
                bagRecycler.gameObject.SetActive(!bindPlayer.isAI);
            if (bagUnwear != null)
                bagUnwear.gameObject.SetActive(!bindPlayer.isAI);
            if (fieldAutoBtn != null)
                fieldAutoBtn.gameObject.SetActive(!bindPlayer.isAI);
            if (buyExpBtn != null)
                buyExpBtn.gameObject.SetActive(!bindPlayer.isAI);
            if (sodLvupBtn != null)
                sodLvupBtn.gameObject.SetActive(!bindPlayer.isAI);
        });
        buyExpBtn.onClick.AddListener(() =>
        {
            if (bindPlayer == null || bindPlayer.isAI)
                return;
            if (!bindPlayer.BuyExp())
            {
                SystemTip.Show($"金币不足，购买经验需要{CombatConst.ExpBuyGoldCost}金币");
                return;
            }
            UpdateExpView();
            GameManager.Instance.PlaySound("Sounds/equip");
        });
        sodLvupBtn.onClick.AddListener(() =>
        {
            if (bindPlayer == null || bindPlayer.isAI)
                return;
            if (!bindPlayer.SodLvup())
            {
                SystemTip.Show(bindPlayer.soldierLevel >= CombatConst.SoldierMaxLevel
                    ? "士兵等级已满"
                    : $"金币不足，升级士兵需要{CombatConst.SodLvupGoldCost}金币");
                return;
            }
            UpdateSodView();
            UpdateFieldView(); // 升级后补足新解锁的士兵
            GameManager.Instance.PlaySound("Sounds/equip");
        });

        // 5x5布阵图：最上面一行前3格、最后面一行后2格为小兵格，其余可布阵英雄
        // FieldUnit 缩小为 80x80，格子间距调小为原来的 2/3
        float cellGap = 160f * 2f / 3f;
        float half = (CombatConst.FormationGridSize - 1) * cellGap * 0.5f;
        for (int i = 0; i < CombatConst.FormationCellCount; i++)
        {
            GameObject fieldUnit = Instantiate(Resources.Load<GameObject>("Prefabs/UIs/Cells/FieldUnit"), fieldRegion.transform);
            var fieldUnitControl = fieldUnit.GetComponent<BagFieldUnitControl>();
            fieldUnitControl.SetInfo(i, 0);
            fieldUnitControl.bagControl = this;

            fieldUnit.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            // 以原3x3网格中心(250,-331)为基准，向两侧扩展为5x5
            float xOff = cellGap * (i % CombatConst.FormationGridSize);
            float yOff = cellGap * (i / CombatConst.FormationGridSize);
            fieldUnit.transform.localPosition = new Vector3(260 - half + xOff, -261 + half - yOff, 0);
        }

        // 格子创建完成后刷新一次（OnShow 在格子创建前执行，需补刷）
        if (bindPlayer != null)
            UpdateFieldView();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnShow()
    {
        var currentPlayer = CardShopManager.Instance.GetCurrentPlayer();
        if (!currentPlayer.isAI)
            Bind(currentPlayer);
        else
            Bind(GameManager.Instance.GetPlayer(0));
    }

    public void OnHide()
    {
        PanelManager.Instance.GetTooltip<BaseTooltip>()?.HideTooltip();
    }

    public void Bind(PlayerInfo p)
    {
        bindPlayer = p;
        UpdateView();

        if (bagRecycler != null)
            bagRecycler.gameObject.SetActive(!p.isAI);
        if (bagUnwear != null)
            bagUnwear.gameObject.SetActive(!p.isAI);
        if (fieldAutoBtn != null)
            fieldAutoBtn.gameObject.SetActive(!p.isAI);
        if (buyExpBtn != null)
            buyExpBtn.gameObject.SetActive(!p.isAI);
        if (sodLvupBtn != null)
            sodLvupBtn.gameObject.SetActive(!p.isAI);

        UpdateFieldView();

        var soldierCfg = SoldierConfig.GetConfig(500001);
        var textAtk = (soldierCfg.Atk + bindPlayer.sodatk + bindPlayer.GetItemPAttr("satk") + bindPlayer.GetSoldierAtkAdd()).ToString();
        var textHp = (soldierCfg.Hp + bindPlayer.sodhp + bindPlayer.GetItemPAttr("shp") + bindPlayer.GetSoldierHpAdd()).ToString();
        UpdateExpView();
        UpdateSodView();

        var humanCount = GameManager.Instance.players.Count(x => !x.isAI);
        aiSwitchBtn.gameObject.SetActive(bindPlayer.pid != 0 && bindPlayer.playerConfig.CanPlay && (!bindPlayer.isAI || humanCount < 2));
        aiSwitchBtn.GetComponentInChildren<TMP_Text>().text = bindPlayer.isAI ? "AI模式" : "玩家模式";
    }

    public void SendSignal(string name, string parm1, int parm2)
    {
        if(name == "SelectPlayer")
            Bind(GameManager.Instance.GetPlayer(parm2));
    }

    // 刷新经验文本、经验条宽度、玩家名+等级文本
    private void UpdateExpView()
    {
        var expNext = bindPlayer.GetExpToNext();
        if (bindPlayer.level >= CombatConst.PlayerMaxLevel)
        {
            expText.text = "满级";
            expBar.rectTransform.sizeDelta = new Vector2(250, expBar.rectTransform.sizeDelta.y);
        }
        else
        {
            expText.text = bindPlayer.exp + "/" + expNext;
            var rate = expNext > 0 ? (float)bindPlayer.exp / expNext : 0f;
            expBar.rectTransform.sizeDelta = new Vector2(250 * rate, expBar.rectTransform.sizeDelta.y);
        }
        var heroOnField = bindPlayer.battleCards.Count(c => c > 0 && ConfigManager.IsHeroCard(c));
        infoText.text = bindPlayer.playerConfig.Name + " Lv." + bindPlayer.level + " 上阵" + heroOnField + "/" + bindPlayer.GetSlotCount() + "英雄";
    }

    // 刷新士兵等级显示：等级 + 当前/最大步兵/弓兵数量 + 攻防加成
    private void UpdateSodView()
    {
        sodInfoText.text = string.Format("士兵等级 {0} 攻+{1} 命+{2}",
            bindPlayer.soldierLevel,
            bindPlayer.GetSoldierAtkAdd(), bindPlayer.GetSoldierHpAdd());
    }

    public void UpdateView()
    {
        int index = 0;

        // 装备中的装备不在背包显示：持有数减去已装备数，没有多余副本则不显示（等级仍按持有总数计算）
        var itemCards = bindPlayer.items
            .Select(x => x.ItemId)
            .Distinct()
            .Select(id => new { Key = id, Owned = bindPlayer.GetItemCount(id), Value = bindPlayer.GetItemFreeCount(id) })
            .Where(x => x.Value > 0)
            .ToList();
        var heroCards = bindPlayer.cards.Where(x => ConfigManager.IsHeroCard(x.Key)).ToList();

        // Destroy all child objects in hero region
        foreach(Transform child in bagHeroRegion.transform)
            GameObject.Destroy(child.gameObject);
        // Destroy all child objects in item region
        foreach(Transform child in bagItemRegion.transform)
            GameObject.Destroy(child.gameObject);
        cellCache.Clear();

        foreach (var item in heroCards)
        {
            // 修改原代码，将新创建的 cell 加入缓存
            GameObject heroCell = Instantiate(Resources.Load<GameObject>("Prefabs/UIs/Cells/BagCellHero"), bagHeroRegion.transform);
            cellCache.Add(heroCell);
            int xOff = index % 5;
            int yOff = index / 5;

            heroCell.transform.localPosition = new Vector3(100 + 164 * xOff, -110 - 203 * yOff, 0);

            BagCell bagCell = heroCell.GetComponent<BagCell>();
            bagCell.bagControl = this;
            bagCell.cardId = item.Key;
            bagCell.count = item.Value;
            bagCell.level = HeroSelectionTool.GetCardLevel(item.Value, true);
            bagCell.UpdateHeroInfo();

            index++;
        }
        index = 0;
        foreach (var itemCell in itemCards)
        {
            int available = itemCell.Value;
            // 物品实例每件一格显示（堆叠上限列已删除，不再合并堆叠）
            for (int n = 0; n < available; n++)
            {
                // 修改原代码，将新创建的 cell 加入缓存
                GameObject cell = Instantiate(Resources.Load<GameObject>("Prefabs/UIs/Cells/BagCellItem"), bagItemRegion.transform);
                cellCache.Add(cell);
                int xOff = index % 9;
                int yOff = index / 9;
                cell.transform.localPosition = new Vector3(70 + 104 * xOff, -61 - 104 * yOff, 0);

                BagCell bagCell = cell.GetComponent<BagCell>();
                bagCell.bagControl = this;
                bagCell.cardId = itemCell.Key;
                bagCell.level = HeroSelectionTool.GetCardLevel(itemCell.Owned, false);
                bagCell.count = 1;
                bagCell.UpdateItemInfo();
                index++;
            }
        }
    }

    public void UpdateEquips()
    {
        foreach (Transform child in bagHeroRegion.transform)
        {
            var bagCell = child.GetComponent<BagCell>();
            bagCell.UpdateHeroInfo();
        }
        foreach (Transform child in bagItemRegion.transform)
        {
            var bagCell = child.GetComponent<BagCell>();
            bagCell.UpdateItemInfo();
        }
    }

    // 调试辅助：把最多10张能补齐羁绊的英雄卡直接放进玩家背包
    // 按当前上阵阵容的缺口挑选：职业同职业3人=3级、势力同阵营4人=3级、好友同组4人=3级（2级起步）
    private void DebugAddCardsForBonds(PlayerInfo p1)
    {
        var heroIds = p1.battleCards.Where(c => c > 0 && ConfigManager.IsHeroCard(c)).ToList();
        var used = new HashSet<int>(heroIds);

        // 统计当前各羁绊在场人数
        var jobCounts = new Dictionary<string, int>();
        var forceCounts = new Dictionary<int, int>();
        var friendPresent = new Dictionary<int, int>();
        foreach (var id in heroIds)
        {
            var cfg = HeroConfig.GetConfig(id);
            jobCounts.TryGetValue(cfg.Job, out var jc);
            jobCounts[cfg.Job] = jc + 1;
            forceCounts.TryGetValue(cfg.Side, out var fc);
            forceCounts[cfg.Side] = fc + 1;
        }
        foreach (var friendCfg in HeroFriendConfig.ConfigList)
        {
            var present = friendCfg.Heros.Count(m => heroIds.Contains(m));
            if (present > 0)
                friendPresent[friendCfg.Id] = present;
        }

        const int JOB_GOAL = 3;
        const int FORCE_GOAL = 4;
        const int FRIEND_GOAL = 4;
        const int MAX_CARDS = 10;

        var candidates = new List<int>();
        while (candidates.Count < MAX_CARDS)
        {
            var before = candidates.Count;

            // 1. 职业：优先补人数最多的同职业英雄
            foreach (var kv in jobCounts.OrderByDescending(x => x.Value))
            {
                if (candidates.Count >= MAX_CARDS)
                    break;
                if (kv.Value >= JOB_GOAL)
                    continue;
                var cand = HeroConfig.ConfigList.FirstOrDefault(h => h.Job == kv.Key && !used.Contains(h.Id));
                if (cand == null)
                    continue;
                candidates.Add(cand.Id);
                used.Add(cand.Id);
            }

            // 2. 势力：补同阵营英雄（排除不参与同阵营护盾的野）
            foreach (var kv in forceCounts.OrderByDescending(x => x.Value))
            {
                if (candidates.Count >= MAX_CARDS)
                    break;
                if (kv.Value >= FORCE_GOAL)
                    continue;
                var forceCfg = ConfigManager.GetForceConfig(kv.Key);
                if (forceCfg == null || !forceCfg.JoinFactionShield)
                    continue;
                var cand = HeroConfig.ConfigList.FirstOrDefault(h => h.Side == kv.Key && !used.Contains(h.Id));
                if (cand == null)
                    continue;
                candidates.Add(cand.Id);
                used.Add(cand.Id);
            }

            // 3. 好友：补同组中不在场的英雄
            foreach (var kv in friendPresent.OrderByDescending(x => x.Value))
            {
                if (candidates.Count >= MAX_CARDS)
                    break;
                if (kv.Value >= FRIEND_GOAL)
                    continue;
                var friendCfg = HeroFriendConfig.GetConfig(kv.Key);
                if (friendCfg == null)
                    continue;
                var cand = friendCfg.Heros.FirstOrDefault(h => !used.Contains(h));
                if (cand == 0)
                    continue;
                candidates.Add(cand);
                used.Add(cand);
            }

            if (candidates.Count == before)
                break; // 无可补英雄，结束
        }

        if (candidates.Count == 0)
            return;

        // 直接加入玩家背包（累计经验提升卡等级）；达到英雄卡上限(15张)后不再加新英雄
        var heroList = p1.GetHeroCardList();
        foreach (var heroId in candidates)
        {
            var isNew = !heroList.Contains(heroId);
            if (isNew && heroList.Count >= CombatConst.PlayerMaxHeroCards)
                break;
            p1.cards.TryGetValue(heroId, out var exp);
            p1.cards[heroId] = exp + 1;
            if (isNew)
                heroList.Add(heroId);
        }
        UpdateView(); // 背包区域重建，显示新加入的卡
    }

    private List<GameObject> connectionLines = new List<GameObject>();
    
    public void UpdateFieldView()
    {
        // 清除之前的连接线
        ClearConnectionLines();
        
        // 背包的羁绊列表默认停在"羁绊"模式，并随阵容调整实时刷新（上阵变化后调用方统一走 UpdateFieldView）
        RefreshMySelect();
        
        // 更新所有fieldUnit的信息
        foreach (Transform child in fieldRegion.transform)
        {
            var fieldUnit = child.GetComponent<BagFieldUnitControl>();
            if(fieldUnit != null)
                fieldUnit.SetInfo(fieldUnit.posId, bindPlayer.battleCards.Length > fieldUnit.posId ? bindPlayer.battleCards[fieldUnit.posId] : 0);
        }
        
        // 获取所有有英雄的fieldUnit
        BagFieldUnitControl[] heroUnits = new BagFieldUnitControl[bindPlayer.battleCards.Length];
        foreach (Transform child in fieldRegion.transform)
        {
            var fieldUnit = child.GetComponent<BagFieldUnitControl>();
            if(fieldUnit != null)
                heroUnits[fieldUnit.posId] = fieldUnit;
        }

        // 遍历任意两个节点，检查是否是好友关系
        for (int i = 0; i < heroUnits.Length; i++)
        {
            for (int j = i + 1; j < heroUnits.Length; j++)
            {
                if(heroUnits[i] == null || heroUnits[j] == null)
                    continue;

                int heroId1 = heroUnits[i].myHeroId;
                int heroId2 = heroUnits[j].myHeroId;

                if(heroId1 == 0 || heroId2 == 0 || !ConfigManager.IsHeroCard(heroId1) || !ConfigManager.IsHeroCard(heroId2))
                    continue;
                
                // 检查是否是好友关系
                if (ConfigManager.GetFriendLevel(heroId1, heroId2) > 0)
                {
                    // 线颜色取该武将所在关系行配置的 LineColor（未配置默认暗灰），与战场 FriendLineManager 一致
                    var lineColor = SysColor.FriendLine.DefaultLine;
                    var lineColorStr = ConfigManager.GetFriendLineColor(heroId1, heroId2);
                    if (!string.IsNullOrEmpty(lineColorStr))
                        ColorUtility.TryParseHtmlString(lineColorStr, out lineColor);
                    // 创建连接线
                    CreateConnectionLine(heroUnits[i].transform, heroUnits[j].transform, lineColor, Vector2.zero);
                }

                var helpSkillId = ConfigManager.GetShowHelpSkillId(heroId1, heroId2, i, j);
                if(helpSkillId > 0)
                {
                    var skillCfg = SkillConfig.GetConfig(helpSkillId);
                    var color = SysColor.GetSkillAttrColor(skillCfg.IsMagic);
                    // 创建连接线
                    CreateConnectionLine(heroUnits[i].transform, heroUnits[j].transform, color, new Vector2(-25, -25), Resources.Load<Sprite>("Textures/SkillPic/" + skillCfg.Icon));
                }

                helpSkillId = ConfigManager.GetShowHelpSkillId(heroId2, heroId1, j, i);
                if(helpSkillId > 0)
                {
                    var skillCfg = SkillConfig.GetConfig(helpSkillId);
                    var color = SysColor.GetSkillAttrColor(skillCfg.IsMagic);
                    // 创建连接线
                    CreateConnectionLine(heroUnits[i].transform, heroUnits[j].transform, color, new Vector2(25, 25), Resources.Load<Sprite>("Textures/SkillPic/" + skillCfg.Icon));
                }
            }
        }
    }
    
    // 创建连接线
    private void CreateConnectionLine(Transform startTransform, Transform endTransform, Color color, Vector2 offset, Sprite sprite = null)
    {
        // 创建一个新的GameObject作为连接线
        GameObject lineObject = new GameObject("ConnectionLine");
        lineObject.transform.SetParent(fieldRegion.transform, false);
        
        // 添加Image组件
        Image lineImage = lineObject.AddComponent<Image>();
        lineImage.color = color; // 设置为半透明的蓝色
        
        // 获取两个点的RectTransform
        RectTransform startRect = startTransform.GetComponent<RectTransform>();
        RectTransform endRect = endTransform.GetComponent<RectTransform>();
        
        // 获取两个点在父容器中的锚点位置（使用anchoredPosition而不是position，更适合UI元素）
        Vector2 startPos = startRect.anchoredPosition;
        Vector2 endPos = endRect.anchoredPosition;
        
        // 计算线段的中点、长度和角度
        Vector2 midPoint = (startPos + endPos) / 2;
        float distance = Vector2.Distance(startPos, endPos);
        float angle = Mathf.Atan2(endPos.y - startPos.y, endPos.x - startPos.x) * Mathf.Rad2Deg;
        
        // 设置线条的位置、大小和旋转
        RectTransform rectTransform = lineObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = midPoint + new Vector2(-250, 250) + offset;
        rectTransform.sizeDelta = new Vector2(distance, 10f); // 线条宽度为20
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);

        // 如果提供了sprite，则在线条中间位置创建一个image
        if (sprite != null)
        {
            GameObject spriteObject = new GameObject("LineSprite");
            spriteObject.transform.SetParent(fieldRegion.transform, false);
            
            Image spriteImage = spriteObject.AddComponent<Image>();
            spriteImage.sprite = sprite;
            spriteImage.color = Color.white;
            
            RectTransform spriteRectTransform = spriteObject.GetComponent<RectTransform>();
            spriteRectTransform.anchoredPosition = midPoint + new Vector2(-250, 250) + offset;
            spriteRectTransform.sizeDelta = new Vector2(45, 45); // 设置精灵大小
            
            // 将精灵放在线条上方
            spriteObject.transform.SetSiblingIndex(lineObject.transform.GetSiblingIndex() + 1);
            
            // 保存精灵引用，以便后续清除
            connectionLines.Add(spriteObject);
        }

        // 将线条放在所有UI元素的底层
        lineObject.transform.SetSiblingIndex(0);
        
        // 保存线条引用，以便后续清除
        connectionLines.Add(lineObject);
    }
    
    // 刷新羁绊列表：固定停在"羁绊"模式，按当前上阵阵容重算职业/好友/国家羁绊
    private void RefreshMySelect()
    {
        if (mySelect == null || bindPlayer == null)
            return;
        mySelect.SetMode(MySelectControl.ViewMode.Bond);
        mySelect.UpdateCards(bindPlayer);
    }

    // 清除所有连接线
    private void ClearConnectionLines()
    {
        foreach (GameObject line in connectionLines)
        {
            if (line != null)
            {
                Destroy(line);
            }
        }
        connectionLines.Clear();
    }

    // 将物品装备到英雄的方法，供拖拽功能使用
    public void EquipItemToHero(int itemCardId, int heroCardId)
    {
        if(itemCardId == 0 || heroCardId == 0)
            return;

        var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);
        var itemCfg = ItemConfig.GetConfig(itemCardId);

        // 使用限制：目标英雄需属于该技能的好友羁绊组（如万民书限定「仁」）
        if (!p1.CanUseItemToHero(heroCardId, itemCardId))
        {
            SystemTip.Show($"只能对拥有「{itemCfg.LimitSkillSname}」的英雄使用");
            return;
        }

        if(itemCfg.RemoveWhenUse)
        {
            p1.UseItemToHero(heroCardId, itemCardId);

            GameManager.Instance.PlaySound("Sounds/eat");

            RemoveCell(itemCardId);
        }
        else
        {
            // 装备到空槽：没有空槽或没有多余副本时失败
            if (!p1.Equip(heroCardId, itemCardId))
            {
                SystemTip.Show(p1.GetEquippedCount(itemCardId) >= p1.GetItemCount(itemCardId)
                    ? "没有多余副本可装备" : "装备槽已满，无法装备");
                return;
            }

            GameManager.Instance.PlaySound("Sounds/equip");

            UpdateView(); // 装备后背包不再显示该装备，需整体刷新（内部会重建格子）
        }
    }

    // 卸装区：拖英雄过来脱下其所有装备进背包
    public void UnwearHeroEquips(int heroCardId)
    {
        if (heroCardId == 0 || !ConfigManager.IsHeroCard(heroCardId))
            return;

        var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);
        int count = p1.UnwearAllEquips(heroCardId);
        if (count == 0)
        {
            SystemTip.Show("该英雄没有装备");
            return;
        }

        GameManager.Instance.PlaySound("Sounds/equip");
        UpdateView(); // 卸下的装备回到背包，整体刷新
    }

    // 合成区：拖物品过来 → 侧边栏列出与该物品相关的合成配方（材料不足的置灰排后）
    public void OpenComposePanel(int itemCardId)
    {
        if (itemCardId == 0 || ConfigManager.IsHeroCard(itemCardId))
        {
            SystemTip.Show("英雄不可合成");
            return;
        }

        SideComposeSelector.SetContext(bindPlayer.pid, itemCardId, OnComposeConfirm);
        PanelManager.Instance.ShowSideBar("SideComposeSelector");
    }

    // 确认合成：先收起侧边栏，等侧边栏滑出后在背包里播合成动画，动画结束才真正合成并刷新
    private void OnComposeConfirm(ItemCombineConfig recipe)
    {
        if (recipe == null)
        {
            GameLog.Error("BagControl.OnComposeConfirm: 合成配方为空");
            return;
        }

        PanelManager.Instance.HideSideBar(() => PlayComposeAnim(recipe, () =>
        {
            var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);
            if (!p1.CombineTwoItems(recipe.ItemA, recipe.ItemB))
            {
                SystemTip.Show("材料不足，无法合成");
                return;
            }

            GameManager.Instance.PlaySound("Sounds/equip");
            UpdateView(); // 材料消耗、产物进背包，整体刷新
        }));
    }

    // 合成动画：两件材料道具的图标跳出来碰到一起，合并成一个结果道具图标（在背包里播）
    private void PlayComposeAnim(ItemCombineConfig recipe, System.Action onComplete)
    {
        if (recipe == null || !ItemConfig.HasConfig(recipe.ResultId) || bagItemRegion == null)
        {
            GameLog.Error($"BagControl.PlayComposeAnim: 配方/结果道具配置缺失或物品区未就绪，跳过动画 resultId={recipe?.ResultId}");
            onComplete?.Invoke();
            return;
        }

        const float iconSize = 80f;
        const float jumpDuration = 0.3f;  // 材料图标弹起来碰到一起
        const float mergeDuration = 0.2f; // 合并出结果图标
        const float flyDuration = 0.35f;  // 结果道具飞向背包
        const float jumpPower = 90f;      // 跳跃高度
        const float collideRise = 70f;    // 碰撞点相对两材料中点再上抬的高度
        const float touchGap = 14f;       // 两图标"碰到一起"时的间距

        // 临时图标容器挂在物品区同层，动画结束后整体销毁
        GameObject containerObj = new GameObject("ComposeAnim", typeof(RectTransform));
        RectTransform container = containerObj.GetComponent<RectTransform>();
        container.SetParent(bagItemRegion.transform.parent, false);
        container.anchoredPosition = Vector2.zero;
        container.SetAsLastSibling();

        // 结果道具的落脚点 = 背包物品区中心
        Vector2 bagPos = ToContainerPos(container, bagItemRegion.transform as RectTransform);

        Vector2 startA = GetItemStartAnchored(container, recipe.ItemA, 0, bagPos - new Vector2(120f, 0f));
        Vector2 startB = GetItemStartAnchored(container, recipe.ItemB, recipe.ItemA == recipe.ItemB ? 1 : 0, bagPos + new Vector2(120f, 0f));

        // 碰撞点 = 两个材料的中点再上抬一点（弹起来在空中互相碰撞）
        Vector2 meetPos = (startA + startB) * 0.5f + new Vector2(0f, collideRise);

        Image iconA = CreateTempItemIcon(container, recipe.ItemA, startA, iconSize);
        Image iconB = CreateTempItemIcon(container, recipe.ItemB, startB, iconSize);
        Image iconResult = CreateTempItemIcon(container, recipe.ResultId, meetPos, iconSize);
        if (iconA == null || iconB == null || iconResult == null)
        {
            GameLog.Error("BagControl.PlayComposeAnim: 临时图标创建失败，跳过动画");
            Destroy(containerObj);
            onComplete?.Invoke();
            return;
        }

        iconResult.rectTransform.localScale = Vector3.zero; // 结果图标先收起，等两个材料碰到一起再弹出

        // 各自朝对方靠拢，中间留出 touchGap 表示"碰到"
        float dir = startA.x <= startB.x ? 1f : -1f;
        Vector2 endA = meetPos - new Vector2(touchGap, 0f) * dir;
        Vector2 endB = meetPos + new Vector2(touchGap, 0f) * dir;

        Sequence seq = DOTween.Sequence().SetUpdate(true);
        seq.Join(iconA.rectTransform.DOJumpAnchorPos(endA, jumpPower, 1, jumpDuration));
        seq.Join(iconB.rectTransform.DOJumpAnchorPos(endB, jumpPower, 1, jumpDuration));

        // 碰到一起后：材料图标缩小淡出，结果道具在碰撞点弹出
        seq.Append(iconResult.rectTransform.DOScale(Vector3.one, mergeDuration).SetEase(Ease.OutBack));
        seq.Join(iconA.DOFade(0f, mergeDuration));
        seq.Join(iconB.DOFade(0f, mergeDuration));
        seq.Join(iconA.rectTransform.DOScale(0f, mergeDuration));
        seq.Join(iconB.rectTransform.DOScale(0f, mergeDuration));

        // 新道具飞向背包
        seq.Append(iconResult.rectTransform.DOAnchorPos(bagPos, flyDuration).SetEase(Ease.InQuad));
        seq.Join(iconResult.rectTransform.DOScale(0.7f, flyDuration));

        seq.OnComplete(() =>
        {
            Destroy(containerObj);
            onComplete?.Invoke();
        });
    }

    // 取背包里该道具对应格子的位置（第 index 个副本，找不到则用兜底位置）
    private Vector2 GetItemStartAnchored(RectTransform container, int itemId, int index, Vector2 fallback)
    {
        List<RectTransform> cells = new List<RectTransform>();
        foreach (Transform child in bagItemRegion.transform)
        {
            var cell = child.GetComponent<BagCell>();
            if (cell != null && cell.cardId == itemId && child is RectTransform rt)
                cells.Add(rt);
        }

        if (cells.Count == 0)
            return fallback;

        return ToContainerPos(container, cells[Mathf.Min(index, cells.Count - 1)]);
    }

    // 目标世界的中心点 → 容器本地坐标（可直接当子图标的 anchoredPosition）
    private Vector2 ToContainerPos(RectTransform container, RectTransform target)
    {
        if (container == null || target == null)
            return Vector2.zero;
        return container.InverseTransformPoint(target.TransformPoint(target.rect.center));
    }

    // 创建一个临时道具图标（仅用于合成动画，无交互）
    private Image CreateTempItemIcon(RectTransform container, int itemId, Vector2 anchoredPos, float size)
    {
        if (!ItemConfig.HasConfig(itemId))
        {
            GameLog.Error($"BagControl.CreateTempItemIcon: ItemConfig 不存在 itemId={itemId}");
            return null;
        }

        GameObject go = new GameObject("TempItemIcon", typeof(RectTransform), typeof(Image));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(container, false);
        rt.sizeDelta = new Vector2(size, size);
        rt.anchoredPosition = anchoredPos;

        Image img = go.GetComponent<Image>();
        img.sprite = Resources.Load<Sprite>("Textures/ItemPic/" + ItemConfig.GetConfig(itemId).Icon);
        img.preserveAspect = true;
        img.raycastTarget = false;
        return img;
    }

    // 物品消耗/出售1个后的格子刷新：每件一格，消耗后直接移除该格
    private void RemoveCell(int itemCardId)
    {
        var cell = cellCache.Find(x => x != null && x.GetComponent<BagCell>().cardId == itemCardId);
        if (cell == null)
            return;
        cellCache.Remove(cell);
        Destroy(cell);
    }

    public void SetHeroForBattle(int heroId, int pos)
    {
        var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);
        if(p1.isAI)
            return;

        // 布阵失败（该格被小兵占用/超出上阵上限）给出提示
        if (!p1.SetBattlePos(heroId, pos, out string failReason))
        {
            SystemTip.Show(failReason);
            return;
        }

        GameManager.Instance.PlaySound("Sounds/equip");
        UpdateFieldView();
        UpdateExpView();
    }

    // 布阵格之间交换单位（英雄/小兵自由交换位置）
    public void SwapFieldUnit(int fromPos, int toPos)
    {
        var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);
        if(p1.isAI)
            return;

        p1.SwapBattleUnits(fromPos, toPos);

        GameManager.Instance.PlaySound("Sounds/equip");
        UpdateFieldView();
        UpdateExpView();
    }

    public void SellCard(int cardId)
    {
        var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);
        if(p1.isAI)
            return;

        // 物品仅掉落获得，不出售；仅英雄可出售（整组）
        if (!ConfigManager.IsHeroCard(cardId))
        {
            SystemTip.Show("物品不可出售");
            return;
        }
        p1.SellCard(cardId, 0);
        RemoveCell(cardId);

        GameManager.Instance.PlaySound("Sounds/gold");        
        UpdateFieldView();
        UpdateExpView();
    }

    public void OnCellClick(BagCell cell)
    {
        // 英雄查 cards，物品查 items
        if (ConfigManager.IsHeroCard(cell.cardId) ? !bindPlayer.cards.ContainsKey(cell.cardId) : !bindPlayer.HasCard(cell.cardId))
            return;
    }

    // 一次性销毁所有缓存的 cell 对象的函数
    public void DestroyAllCells()
    {
        foreach (var cell in cellCache)
        {
            if (cell != null)
            {
                Destroy(cell);
            }
        }
        cellCache.Clear();
    }    
}
