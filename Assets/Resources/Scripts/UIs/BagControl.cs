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

    // 背包区域切换按钮：英雄区 / 士兵区互斥显示
    public Button bagHeroBtn;
    public Button bagSoldierBtn;


    // Start is called before the first frame update
    // 声明一个列表用于缓存 cell 对象
    private List<GameObject> cellCache = new List<GameObject>();
    public GameObject bagHeroRegion;
    public GameObject bagSoldierRegion;
    public GameObject bagItemRegion;
    public GameObject fieldRegion;
    public BagRecycler bagInfo;

    public TMP_Text infoText;
    public TMP_Text expText;
    public Image expBar;
    public Button buyExpBtn;
    public TMP_Text sodInfoText;
    public Button sodLvupBtn;
    public Button[] loveBtn;

    public PlayerInfo bindPlayer;
    public MySelectControl mySelect;

    void Start()
    {
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

            GameManager.Instance.PlaySound("Sounds/foot");
        });
        if (bagHeroBtn != null)
        {
            bagHeroBtn.onClick.AddListener(() =>
            {
                GameManager.Instance.PlaySound("Sounds/click");
                SwitchBagRegion(true);
            });
        }
        if (bagSoldierBtn != null)
        {
            bagSoldierBtn.onClick.AddListener(() =>
            {
                GameManager.Instance.PlaySound("Sounds/click");
                SwitchBagRegion(false);
            });
        }
        SwitchBagRegion(true); // 默认显示英雄区

        // 许愿按钮：第10/25/40回合各解锁1个，点击打开许愿面板
        InitLoveButtons();

        aiSwitchBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/click");
            bindPlayer.isAI = !bindPlayer.isAI;
            aiSwitchBtn.GetComponentInChildren<TMP_Text>().text = bindPlayer.isAI ? "AI模式" : "玩家模式";
            if (bagInfo != null)
                bagInfo.gameObject.SetActive(!bindPlayer.isAI);
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

        if (bagInfo != null)
            bagInfo.gameObject.SetActive(!p.isAI);
        if (fieldAutoBtn != null)
            fieldAutoBtn.gameObject.SetActive(!p.isAI);
        if (buyExpBtn != null)
            buyExpBtn.gameObject.SetActive(!p.isAI);
        if (sodLvupBtn != null)
            sodLvupBtn.gameObject.SetActive(!p.isAI);

        UpdateFieldView();

        var soldierCfg = SoldierConfig.GetConfig(500001);
        var textAtk = (soldierCfg.Atk + bindPlayer.GetSoldierAtkAdd(soldierCfg)).ToString();
        var textHp = (soldierCfg.Hp + bindPlayer.GetItemPAttr("shp") + bindPlayer.GetSoldierHpAdd(soldierCfg)).ToString();
        UpdateExpView();
        UpdateSodView();

        var humanCount = GameManager.Instance.players.Count(x => !x.isAI);
        aiSwitchBtn.gameObject.SetActive(bindPlayer.pid != 0 && bindPlayer.playerConfig.CanPlay && (!bindPlayer.isAI || humanCount < 2));
        aiSwitchBtn.GetComponentInChildren<TMP_Text>().text = bindPlayer.isAI ? "AI模式" : "玩家模式";

        RefreshWishButtons();
    }

    // 初始化许愿按钮：注册点击
    private void InitLoveButtons()
    {
        if (loveBtn == null)
            return;

        for (int i = 0; i < loveBtn.Length; i++)
        {
            if (loveBtn[i] == null)
                continue;

            int slot = i;
            loveBtn[i].onClick.AddListener(() =>
            {
                GameManager.Instance.PlaySound("Sounds/click");
                OnLoveBtnClick(slot);
            });
        }
    }

    // 点击许愿按钮：未解锁/已选择给出提示，否则打开许愿面板
    private void OnLoveBtnClick(int slot)
    {
        if (slot < 0 || slot >= CombatConst.WishUnlockRounds.Length)
        {
            GameLog.Warn($"BagControl.OnLoveBtnClick: 许愿槽位{slot}超出配置范围，忽略");
            return;
        }

        if (bindPlayer == null)
        {
            GameLog.Warn("BagControl.OnLoveBtnClick: 未绑定玩家，忽略许愿");
            return;
        }

        if (!HeroSelectionTool.IsWishSlotUnlocked(slot))
        {
            int unlockRound = CombatConst.WishUnlockRounds[slot];
            int currentRound = GameManager.Instance.year;
            SystemTip.Show($"第{unlockRound}回合解锁许愿，还需{Mathf.Max(1, unlockRound - currentRound)}回合");
            return;
        }

        if (HeroSelectionTool.GetWishHeroId(bindPlayer, slot) != 0)
        {
            SystemTip.Show("该许愿已选择，无法修改");
            return;
        }

        PanelManager.Instance.ShowWish(slot, bindPlayer.pid, RefreshWishButtons);
    }

    // 刷新3个许愿按钮：已选择显示对应英雄头像；可点击显示 okk；未解锁显示 error
    private void RefreshWishButtons()
    {
        if (loveBtn == null || bindPlayer == null)
            return;

        for (int i = 0; i < loveBtn.Length; i++)
        {
            if (loveBtn[i] == null)
                continue;

            Image img = loveBtn[i].GetComponent<Image>();
            if (img == null)
                continue;

            int heroId = HeroSelectionTool.GetWishHeroId(bindPlayer, i);
            if (heroId != 0 && HeroConfig.HasConfig(heroId))
            {
                img.sprite = Resources.Load<Sprite>("Textures/Skins/" + HeroConfig.GetConfig(heroId).Icon);
            }
            else if (HeroSelectionTool.IsWishSlotUnlocked(i))
            {
                // 已到达许愿回合且尚未选择：可以点击
                img.sprite = LoadLoveBtnSprite("Textures/okk");
            }
            else
            {
                // 未到许愿回合：不可点击
                img.sprite = LoadLoveBtnSprite("Textures/error");
            }

            img.color = Color.white;
        }
    }

    private Sprite LoadLoveBtnSprite(string path)
    {
        var sprite = Resources.Load<Sprite>(path);
        if (sprite == null)
            GameLog.Error("BagControl 加载许愿按钮图标失败: " + path);
        return sprite;
    }

    // 切换背包显示区域：true=英雄区，false=士兵区（两个区域互斥占用同一块位置）
    private void SwitchBagRegion(bool showHero)
    {
        if (bagHeroRegion != null)
            bagHeroRegion.SetActive(showHero);
        if (bagSoldierRegion != null)
            bagSoldierRegion.SetActive(!showHero);

        // 选中的页签用金色高亮，未选中的置灰
        SetTabButtonColor(bagHeroBtn, showHero);
        SetTabButtonColor(bagSoldierBtn, !showHero);
    }

    private void SetTabButtonColor(Button button, bool selected)
    {
        if (button == null)
            return;
        Image img = (button.targetGraphic != null ? button.targetGraphic : button.GetComponent<Image>()) as Image;
        if (img != null)
            img.color = selected ? SysColor.UI.CheckBtnSelected : SysColor.UI.BtnClickedGray;
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
        var soldierCfg = SoldierConfig.GetConfig(500001);
        sodInfoText.text = string.Format("士兵等级 {0} 攻+{1} 命+{2}",
            bindPlayer.soldierLevel,
            bindPlayer.GetSoldierAtkAdd(soldierCfg), bindPlayer.GetSoldierHpAdd(soldierCfg));
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
                int xOff = index % 13;
                int yOff = index / 13;
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
                
                // 检查好友关系：真实所属组 + 装备的代友道具指代组，两组有交集即拉线
                var relIds1 = FriendLineManager.CollectFriendRelationIds(bindPlayer, heroId1);
                var relIds2 = FriendLineManager.CollectFriendRelationIds(bindPlayer, heroId2);
                HeroFriendConfig sharedRel = null;
                foreach (var relId in relIds1)
                {
                    if (!relIds2.Contains(relId))
                        continue;
                    var relCfg = HeroFriendConfig.GetConfig(relId);
                    if (relCfg != null)
                    {
                        sharedRel = relCfg;
                        break;
                    }
                }
                if (sharedRel != null)
                {
                    // 线颜色取该关系行配置的 LineColor（未配置默认暗灰），与战场 FriendLineManager 一致
                    var lineColor = SysColor.FriendLine.DefaultLine;
                    if (!string.IsNullOrEmpty(sharedRel.LineColor))
                        ColorUtility.TryParseHtmlString(sharedRel.LineColor, out lineColor);
                    // 创建连接线
                    CreateConnectionLine(heroUnits[i].transform, heroUnits[j].transform, lineColor, Vector2.zero);
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

        // 分解符只能在合成区使用，拖到英雄身上无效
        if (itemCfg.Effect == "tpsplit")
        {
            SystemTip.Show($"道具{itemCfg.Name}需拖到合成区使用");
            return;
        }

        // 使用限制：目标英雄需属于该技能的好友羁绊组（如万民书限定「仁」）；经验消耗品在英雄满级时不可使用
        if (!p1.CanUseItemToHero(heroCardId, itemCardId))
        {
            if (itemCfg.Effect == "tpexp")
                SystemTip.Show($"英雄已满级，无法使用道具{itemCfg.Name}");
            else
                SystemTip.Show($"只能对拥有{itemCfg.LimitSkillSname}羁绊的英雄使用");
            return;
        }

        // 代友道具：英雄已属于该羁绊组不可装备；同一英雄每种代友道具只能装备1件
        if (itemCfg.Effect == CombatConst.FriendProxyItemEffect)
        {
            if (ConfigManager.HeroHasFriendSkill(heroCardId, itemCfg.RefSkillSname))
            {
                SystemTip.Show($"该英雄已拥有此羁绊，无法装备道具{itemCfg.Name}");
                return;
            }
            if (p1.GetItemIdsOnHero(heroCardId).Contains(itemCardId))
            {
                SystemTip.Show("同一英雄只能装备一件该代友道具");
                return;
            }
        }

        if(itemCfg.RemoveWhenUse)
        {
            if (!p1.UseItemToHero(heroCardId, itemCardId))
                return;

            // 使用消耗场景：播放道具配置的使用音效
            PlayItemUseSound(itemCfg);

            // 消耗品使用后消失：整体刷新背包（经验消耗品会改变英雄卡等级显示）
            RemoveCell(itemCardId);
            UpdateView();
            UpdateFieldView();
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
            UpdateFieldView(); // 代友道具会影响羁绊连线与左侧羁绊人数，需同步刷新阵型区
        }
    }

    // 播放道具的使用音效：UseSound 为 Resources/Sounds 下的文件名（如 metalhit），空则不播放
    private void PlayItemUseSound(ItemConfig itemCfg)
    {
        if (itemCfg == null || string.IsNullOrEmpty(itemCfg.UseSound))
            return;
        GameManager.Instance.PlaySound("Sounds/" + itemCfg.UseSound);
    }

    // 卸装区：拖英雄过来脱下其所有装备进背包
    public void UnwearHeroEquips(int heroCardId)
    {
        if (heroCardId == 0 || !ConfigManager.IsHeroCard(heroCardId))
            return;

        var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);
        var equipIds = p1.GetItemIdsOnHero(heroCardId);
        if (equipIds.Count == 0)
        {
            SystemTip.Show("该英雄没有装备");
            return;
        }

        p1.UnwearAllEquips(heroCardId);
        GameManager.Instance.PlaySound("Sounds/equip");

        // 装备图标从英雄卡装备槽飞向物品区，落地后再刷新背包（卸下的装备此时才出现在物品区）
        PlayEquipsFlyToBag(heroCardId, equipIds, () => UpdateView());
    }

    // 卖出区：拖道具过来 → 右侧滑出道具操作面板，卖出/分解/改铸都在面板内执行
    public void OpenItemPanel(int itemCardId)
    {
        if (itemCardId == 0 || !ItemConfig.HasConfig(itemCardId) || ConfigManager.IsHeroCard(itemCardId))
        {
            SystemTip.Show("该道具无法打开操作面板");
            return;
        }

        SideItemControl.SetContext(this, itemCardId);
        PanelManager.Instance.ShowSideBar("SideItemControl");
    }

    // 合成按钮的显示条件：该道具是任意一条合成配方的材料（材料是否足够由配方面板里置灰提示）
    public bool CanComposeItem(int itemCardId)
    {
        if (bindPlayer == null || itemCardId == 0 || !ItemConfig.HasConfig(itemCardId) || ConfigManager.IsHeroCard(itemCardId))
            return false;

        foreach (var rcp in ItemCombineConfig.ConfigList)
        {
            if (rcp.ItemA == itemCardId || rcp.ItemB == itemCardId)
                return true;
        }
        return false;
    }

    // 打开合成配方面板：列出与该道具相关的配方（材料不足的置灰），确认后由 ComposeItem 执行
    public void OpenComposeSelector(int itemCardId)
    {
        if (bindPlayer == null)
            return;

        SideComposeSelector.SetContext(bindPlayer.pid, itemCardId, ComposeItem);
        PanelManager.Instance.ShowSideBar("SideComposeSelector");
    }

    // 确认合成：先收起配方面板，再消耗材料产出结果并刷新背包
    private void ComposeItem(ItemCombineConfig recipe)
    {
        if (recipe == null)
        {
            GameLog.Error("BagControl.ComposeItem: 合成配方为空");
            return;
        }

        PanelManager.Instance.HideSideBar(() =>
        {
            var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);

            // 两件材料合并特效，播完再真正消耗材料产出结果
            PlayMergeAnim(recipe.ItemA, recipe.ItemB, recipe.ResultId, () =>
            {
                if (!p1.CombineTwoItems(recipe.ItemA, recipe.ItemB))
                {
                    SystemTip.Show("材料不足，无法合成");
                    return;
                }

                GameManager.Instance.PlaySound("Sounds/compose");
                UpdateView();
                UpdateFieldView();
            });
        });
    }

    // 卖出区：拖英雄过来 → 右侧滑出英雄操作面板，卸装与卖出都在面板内执行
    public void OpenHeroPanel(int heroCardId)
    {
        if (heroCardId == 0 || !ConfigManager.IsHeroCard(heroCardId))
        {
            SystemTip.Show("只有英雄可以打开此面板");
            return;
        }

        SideHeroControl.SetContext(bindPlayer.pid, heroCardId, UnwearHeroEquips, SellCard);
        PanelManager.Instance.ShowSideBar("SideHeroControl");
    }

    // 分解：消耗1件该装备与1个分解锤(401011)，返还合成前的两件材料（由 SideItemControl 的分解按钮调用）
    public void DecomposeItem(int itemCardId)
    {
        if (bindPlayer == null)
            return;

        var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);

        var recipe = GetCombineRecipe(itemCardId);
        if (itemCardId == 0 || !ItemConfig.HasConfig(itemCardId) || ConfigManager.IsHeroCard(itemCardId) || recipe == null)
        {
            SystemTip.Show("该道具不是可分解的合成装备");
            return;
        }
        if (p1.GetItemFreeCount(itemCardId) <= 0)
        {
            SystemTip.Show(p1.GetEquippedCount(itemCardId) > 0 ? "该装备已全部装备，需先卸下" : "背包中没有该装备");
            return;
        }
        if (p1.GetItemCount(401011) <= 0)
        {
            SystemTip.Show("缺少分解锤，无法分解");
            return;
        }
        // 一分为二特效：装备弹起后碎裂成两件材料，播完再真正消耗与返还
        PlaySplitAnim(itemCardId, recipe.ItemA, recipe.ItemB, () =>
        {
            if (!p1.DecomposeItem(itemCardId, 401011))
                return;

            GameManager.Instance.PlaySound("Sounds/compose");
            PlayItemUseSound(ItemConfig.HasConfig(401011) ? ItemConfig.GetConfig(401011) : null); // 分解锤被消耗，播放其使用音效
            UpdateView();
            UpdateFieldView();
        });
    }

    // 是否可分解：该道具是合成装备、背包有多余副本、且持有分解锤(401011)
    public bool CanDecomposeItem(int itemCardId)
    {
        if (bindPlayer == null || itemCardId == 0 || !ItemConfig.HasConfig(itemCardId) || ConfigManager.IsHeroCard(itemCardId))
            return false;
        if (GetCombineRecipe(itemCardId) == null)
            return false;

        var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);
        return p1.GetItemFreeCount(itemCardId) > 0 && p1.GetItemCount(401011) > 0;
    }

    // 查找以该道具为结果的合成配方（即是否为合成装备、可分解）
    private ItemCombineConfig GetCombineRecipe(int itemId)
    {
        foreach (var rcp in ItemCombineConfig.ConfigList)
        {
            if (rcp.ResultId == itemId)
                return rcp;
        }
        return null;
    }

    // 改铸：消耗1件该装备与1个西洋镜(401009)，产出同 Id 段的随机装备（由 SideItemControl 的改铸按钮调用）
    public void RerollItem(int itemCardId)
    {
        if (bindPlayer == null)
            return;

        var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);

        if (itemCardId == 0 || !ItemConfig.HasConfig(itemCardId) || ConfigManager.IsHeroCard(itemCardId))
        {
            SystemTip.Show("该道具不能改铸");
            return;
        }
        int seg = itemCardId / 1000;
        if (seg != 400 && seg != 402 && seg != 403)
        {
            SystemTip.Show("只能改铸 400/402/403 系列装备");
            return;
        }
        if (p1.GetItemFreeCount(itemCardId) <= 0)
        {
            SystemTip.Show(p1.GetEquippedCount(itemCardId) > 0 ? "该装备已全部装备，需先卸下" : "背包中没有该装备");
            return;
        }
        if (p1.GetItemCount(401009) <= 0)
        {
            SystemTip.Show("缺少西洋镜，无法改铸");
            return;
        }
        var candidates = p1.GetRerollCandidates(itemCardId);
        if (candidates.Count == 0)
        {
            SystemTip.Show("没有可改铸的同系列道具");
            return;
        }
        int newItemId = candidates[SysRandom.Range(0, candidates.Count)];

        // 两合一特效：装备与西洋镜弹到一起合并，弹出改铸结果再飞回物品区；动画结束后再真正消耗与产出
        PlayMergeAnim(itemCardId, 401009, newItemId, () =>
        {
            if (!p1.RerollItem(itemCardId, 401009, newItemId))
                return;

            GameManager.Instance.PlaySound("Sounds/compose");
            PlayItemUseSound(ItemConfig.HasConfig(401009) ? ItemConfig.GetConfig(401009) : null); // 西洋镜被消耗，播放其使用音效
            UpdateView();
            UpdateFieldView();
        });
    }

    // 两合一动画（合成/改铸通用）：两个来源道具的图标弹起来碰到一起，合并成一个结果道具图标后飞回背包
    private void PlayMergeAnim(int itemA, int itemB, int resultId, System.Action onComplete)
    {
        if (bagItemRegion == null || !ItemConfig.HasConfig(itemA) || !ItemConfig.HasConfig(itemB) || !ItemConfig.HasConfig(resultId))
        {
            GameLog.Error($"BagControl.PlayMergeAnim: 物品区未就绪或配置缺失，跳过动画 itemA={itemA}, itemB={itemB}, resultId={resultId}");
            onComplete?.Invoke();
            return;
        }

        const float iconSize = 80f;
        const float jumpDuration = 0.4f;  // 两个图标从各自格子飞到屏幕中心
        const float mergeDuration = 0.2f; // 合并出结果图标
        const float flyDuration = 0.35f;  // 结果图标飞向背包
        const float jumpPower = 90f;      // 跳跃高度
        const float touchGap = 14f;       // 两个图标"碰到一起"时的间距

        // 临时图标容器挂在物品区同层，动画结束后整体销毁
        GameObject containerObj = new GameObject("MergeAnim", typeof(RectTransform));
        RectTransform container = containerObj.GetComponent<RectTransform>();
        container.SetParent(bagItemRegion.transform.parent, false);
        container.anchoredPosition = Vector2.zero;
        container.SetAsLastSibling();

        // 结果道具的落脚点 = 背包物品区中心
        Vector2 bagPos = ToContainerPos(container, bagItemRegion.transform as RectTransform);

        Vector2 startA = GetItemStartAnchored(container, itemA, 0, bagPos - new Vector2(120f, 0f));
        Vector2 startB = GetItemStartAnchored(container, itemB, itemA == itemB ? 1 : 0, bagPos + new Vector2(120f, 0f));

        // 碰撞点 = 屏幕中心
        Vector2 meetPos = GetScreenCenterPos(container);

        Image iconA = CreateTempItemIcon(container, itemA, startA, iconSize);
        Image iconB = CreateTempItemIcon(container, itemB, startB, iconSize);
        Image iconResult = CreateTempItemIcon(container, resultId, meetPos, iconSize);
        if (iconA == null || iconB == null || iconResult == null)
        {
            GameLog.Error("BagControl.PlayMergeAnim: 临时图标创建失败，跳过动画");
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

        // 碰到一起后：两个来源图标缩小淡出，结果道具在碰撞点弹出
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

    // 一分为二动画（分解）：装备图标弹起后碎裂消失，两件材料在碎裂点弹出并分开飞回背包
    private void PlaySplitAnim(int itemId, int outA, int outB, System.Action onComplete)
    {
        if (bagItemRegion == null || !ItemConfig.HasConfig(itemId) || !ItemConfig.HasConfig(outA) || !ItemConfig.HasConfig(outB))
        {
            GameLog.Error($"BagControl.PlaySplitAnim: 物品区未就绪或配置缺失，跳过动画 itemId={itemId}, outA={outA}, outB={outB}");
            onComplete?.Invoke();
            return;
        }

        const float iconSize = 80f;
        const float jumpDuration = 0.35f; // 装备飞到屏幕中心
        const float popDuration = 0.2f;    // 装备碎裂 + 材料弹出
        const float flyDuration = 0.35f;   // 材料飞向背包
        const float jumpPower = 70f;
        const float spread = 70f;          // 两件材料向左右分开的距离

        // 临时图标容器挂在物品区同层，动画结束后整体销毁
        GameObject containerObj = new GameObject("SplitAnim", typeof(RectTransform));
        RectTransform container = containerObj.GetComponent<RectTransform>();
        container.SetParent(bagItemRegion.transform.parent, false);
        container.anchoredPosition = Vector2.zero;
        container.SetAsLastSibling();

        Vector2 bagPos = ToContainerPos(container, bagItemRegion.transform as RectTransform);
        Vector2 start = GetItemStartAnchored(container, itemId, 0, bagPos);
        Vector2 breakPos = GetScreenCenterPos(container); // 碎裂点 = 屏幕中心

        Image iconSrc = CreateTempItemIcon(container, itemId, start, iconSize);
        Image iconA = CreateTempItemIcon(container, outA, breakPos, iconSize);
        Image iconB = CreateTempItemIcon(container, outB, breakPos, iconSize);
        if (iconSrc == null || iconA == null || iconB == null)
        {
            GameLog.Error("BagControl.PlaySplitAnim: 临时图标创建失败，跳过动画");
            Destroy(containerObj);
            onComplete?.Invoke();
            return;
        }

        iconA.rectTransform.localScale = Vector3.zero;
        iconB.rectTransform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence().SetUpdate(true);
        // 装备弹到碎裂点后消失
        seq.Append(iconSrc.rectTransform.DOJumpAnchorPos(breakPos, jumpPower, 1, jumpDuration));
        seq.Insert(jumpDuration, iconSrc.DOFade(0f, popDuration));
        seq.Insert(jumpDuration, iconSrc.rectTransform.DOScale(0f, popDuration));

        // 两件材料在碎裂点弹出
        seq.Insert(jumpDuration, iconA.rectTransform.DOScale(Vector3.one, popDuration).SetEase(Ease.OutBack));
        seq.Insert(jumpDuration, iconB.rectTransform.DOScale(Vector3.one, popDuration).SetEase(Ease.OutBack));

        // 材料向左右分开飞回物品区
        float flyStart = jumpDuration + popDuration;
        seq.Insert(flyStart, iconA.rectTransform.DOAnchorPos(bagPos - new Vector2(spread, 0f), flyDuration).SetEase(Ease.InQuad));
        seq.Insert(flyStart, iconB.rectTransform.DOAnchorPos(bagPos + new Vector2(spread, 0f), flyDuration).SetEase(Ease.InQuad));
        seq.Insert(flyStart, iconA.rectTransform.DOScale(0.6f, flyDuration));
        seq.Insert(flyStart, iconB.rectTransform.DOScale(0.6f, flyDuration));

        seq.OnComplete(() =>
        {
            Destroy(containerObj);
            onComplete?.Invoke();
        });
    }

    // 是否可改铸：400/402/403 系列装备、背包有多余副本、持有西洋镜(401009)且有同系列其它道具
    public bool CanRerollItem(int itemCardId)
    {
        if (bindPlayer == null || itemCardId == 0 || !ItemConfig.HasConfig(itemCardId) || ConfigManager.IsHeroCard(itemCardId))
            return false;
        int seg = itemCardId / 1000;
        if (seg != 400 && seg != 402 && seg != 403)
            return false;

        var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);
        if (p1.GetItemFreeCount(itemCardId) <= 0 || p1.GetItemCount(401009) <= 0)
            return false;
        return p1.GetRerollCandidates(itemCardId).Count > 0;
    }

    // 取屏幕中心（画布中心）在容器内的本地坐标，供合成/分解动画当作撞点
    private Vector2 GetScreenCenterPos(RectTransform container)
    {
        if (container == null)
            return Vector2.zero;

        Canvas canvas = container.GetComponentInParent<Canvas>();
        if (canvas == null || canvas.transform is not RectTransform canvasRect)
        {
            GameLog.Warn("BagControl.GetScreenCenterPos: 未找到父级 Canvas，退回容器原点");
            return Vector2.zero;
        }

        return ToContainerPos(container, canvasRect);
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
        img.sprite = ItemIconLoader.Load(ItemConfig.GetConfig(itemId).Icon);
        img.preserveAspect = true;
        img.raycastTarget = false;
        return img;
    }

    // 卸下/卖出英雄装备的飞行动画：装备图标从英雄卡装备槽依次飞向背包物品区（小跳一下再飞，落地后回调）
    private void PlayEquipsFlyToBag(int heroCardId, List<int> equipIds, System.Action onComplete)
    {
        if (equipIds == null || equipIds.Count == 0 || bagItemRegion == null)
        {
            onComplete?.Invoke();
            return;
        }

        const float iconSize = 60f;
        const float jumpDuration = 0.15f; // 装备从槽位先小跳起来
        const float flyDuration = 0.4f;   // 飞向物品区
        const float jumpPower = 40f;      // 小跳高度
        const float interval = 0.08f;     // 多件装备依次出发的间隔

        // 临时图标容器挂在物品区同层（UpdateView 只清物品区/英雄区子物体，不影响它），动画结束整体销毁
        GameObject containerObj = new GameObject("EquipFlyAnim", typeof(RectTransform));
        RectTransform container = containerObj.GetComponent<RectTransform>();
        container.SetParent(bagItemRegion.transform.parent, false);
        container.anchoredPosition = Vector2.zero;
        container.SetAsLastSibling();

        Vector2 bagPos = ToContainerPos(container, bagItemRegion.transform as RectTransform);

        // 起点：英雄卡的装备槽位（找不到英雄卡则退化到物品区中心，仅起点不同不影响飞行动画）
        RectTransform heroRt = null;
        foreach (Transform child in bagHeroRegion.transform)
        {
            var cell = child.GetComponent<BagCell>();
            if (cell != null && cell.cardId == heroCardId && child is RectTransform rt)
            {
                heroRt = rt;
                break;
            }
        }
        var heroCell = heroRt != null ? heroRt.GetComponent<BagCell>() : null;

        int done = 0;
        int total = 0;
        for (int i = 0; i < equipIds.Count; i++)
        {
            RectTransform slotRt = null;
            if (heroCell != null && heroCell.equipImages != null && i < heroCell.equipImages.Length && heroCell.equipImages[i] != null)
                slotRt = heroCell.equipImages[i].rectTransform;
            Vector2 start = slotRt != null
                ? ToContainerPos(container, slotRt)
                : (heroRt != null ? ToContainerPos(container, heroRt) : bagPos) + new Vector2(0, -30f * i);

            Image icon = CreateTempItemIcon(container, equipIds[i], start, iconSize);
            if (icon == null)
                continue;
            total++;

            // 每个装备：小跳起来 → 飞向物品区中心（同时缩小），多件装备依次延迟出发
            Sequence iconSeq = DOTween.Sequence().SetUpdate(true).SetDelay(interval * i);
            iconSeq.Append(icon.rectTransform.DOJumpAnchorPos(start + new Vector2(0f, jumpPower), jumpPower * 0.5f, 1, jumpDuration));
            iconSeq.Append(icon.rectTransform.DOAnchorPos(bagPos, flyDuration).SetEase(Ease.InQuad));
            iconSeq.Join(icon.rectTransform.DOScale(0.7f, flyDuration));
            iconSeq.OnComplete(() =>
            {
                done++;
                if (done >= total)
                {
                    Destroy(containerObj);
                    onComplete?.Invoke();
                }
            });
        }

        if (total == 0)
        {
            Destroy(containerObj);
            onComplete?.Invoke();
        }
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

        GameManager.Instance.PlaySound("Sounds/foot");
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

        GameManager.Instance.PlaySound("Sounds/foot");
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

        // 卖出前先脱下装备回背包（否则装备会挂在已卖出的英雄上而丢失），装备图标飞向物品区
        var equipIds = p1.GetItemIdsOnHero(cardId);
        if (equipIds.Count > 0)
            p1.UnwearAllEquips(cardId);

        p1.SellCard(cardId, 0);
        GameManager.Instance.PlaySound("Sounds/gold");

        if (equipIds.Count > 0)
        {
            // 等装备飞完再重建背包（卖出英雄消失、卸下的装备出现在物品区）；飞行起点取自英雄卡装备槽，不能提前销毁
            PlayEquipsFlyToBag(cardId, equipIds, () =>
            {
                UpdateView();
                UpdateFieldView();
                UpdateExpView();
            });
        }
        else
        {
            RemoveCell(cardId);
            UpdateFieldView();
            UpdateExpView();
        }
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
