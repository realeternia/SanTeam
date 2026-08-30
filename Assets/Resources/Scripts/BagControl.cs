using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
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
    public ItemHeroDetail heroDetail;
    public ItemDetail itemDetail;
    public GameObject bagHeroRegion;
    public GameObject bagItemRegion;
    public GameObject fieldRegion;
    public BagRecycler bagRecycler;
    public BagRecycler bagUnwear; // 卸装区：拖英雄过来脱下所有装备
    public TMP_Text infoText;
    public TMP_Text expText;
    public Image expBar;
    public Button buyExpBtn;
    public TMP_Text sodInfoText;
    public Button sodLvupBtn;

    public PlayerInfo bindPlayer;

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
            if (Tooltip.Instance != null)
                Tooltip.Instance.HideTooltip();
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
            if (bindPlayer.BuyExp())
            {
                UpdateExpView();
                GameManager.Instance.PlaySound("Sounds/equip");
            }
        });
        sodLvupBtn.onClick.AddListener(() =>
        {
            if (bindPlayer == null || bindPlayer.isAI)
                return;
            if (bindPlayer.SodLvup())
            {
                UpdateSodView();
                UpdateFieldView(); // 升级后补足新解锁的士兵
                GameManager.Instance.PlaySound("Sounds/equip");
            }
        });

        // 5x5布阵图：最上面一行前3格、最后面一行后2格为小兵格，其余可布阵英雄
        // FieldUnit 缩小为 80x80，格子间距调小为原来的 2/3
        float cellGap = 160f * 2f / 3f;
        float half = (CombatConst.FormationGridSize - 1) * cellGap * 0.5f;
        for (int i = 0; i < CombatConst.FormationCellCount; i++)
        {
            GameObject fieldUnit = Instantiate(Resources.Load<GameObject>("Prefabs/FieldUnit"), fieldRegion.transform);
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
        if (Tooltip.Instance != null)
            Tooltip.Instance.HideTooltip();
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

        heroDetail.gameObject.SetActive(false);
        itemDetail.gameObject.SetActive(false);

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
        var itemCards = bindPlayer.cards
            .Where(x => !ConfigManager.IsHeroCard(x.Key))
            .Select(x => new { Key = x.Key, Owned = x.Value, Value = x.Value - bindPlayer.GetEquippedCount(x.Key) })
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
            GameObject heroCell = Instantiate(Resources.Load<GameObject>("Prefabs/BagCellHero"), bagHeroRegion.transform);
            cellCache.Add(heroCell);
            int xOff = index % 6;
            int yOff = index / 6;

            heroCell.transform.localPosition = new Vector3(100 + 164 * xOff, -131 - 226 * yOff, 0);

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
            // 装备不叠加：每个未装备副本单独占一格
            for (int n = 0; n < itemCell.Value; n++)
            {
                // 修改原代码，将新创建的 cell 加入缓存
                GameObject cell = Instantiate(Resources.Load<GameObject>("Prefabs/BagCellItem"), bagItemRegion.transform);
                cellCache.Add(cell);
                int xOff = index % 9;
                int yOff = index / 9;
                cell.transform.localPosition = new Vector3(95 + 104 * xOff, -71 - 104 * yOff, 0);

                BagCell bagCell = cell.GetComponent<BagCell>();
                bagCell.bagControl = this;
                bagCell.cardId = itemCell.Key;
                bagCell.level = HeroSelectionTool.GetCardLevel(itemCell.Owned, false);
                bagCell.UpdateItemInfo();
                index++;
            }
        }
        itemDetail.Clear();
        heroDetail.Clear();
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
                    // 创建连接线
                    CreateConnectionLine(heroUnits[i].transform, heroUnits[j].transform, Color.white, Vector2.zero);
                }

                var helpSkillId = ConfigManager.GetShowHelpSkillId(heroId1, heroId2, i, j);
                if(helpSkillId > 0)
                {
                    var skillCfg = SkillConfig.GetConfig(helpSkillId);
                    var color = skillCfg.Attr == "ap" ? new Color(0.55f, 0.55f, 1f, 0.6f) : (skillCfg.Attr == "might" ? new Color(0.95f, 0.4f, 0.4f, 0.6f) : new Color(0.7f, 0.8f, 0.3f, 0.6f));
                    // 创建连接线
                    CreateConnectionLine(heroUnits[i].transform, heroUnits[j].transform, color, new Vector2(-25, -25), Resources.Load<Sprite>("SkillPic/" + skillCfg.Icon));
                }

                helpSkillId = ConfigManager.GetShowHelpSkillId(heroId2, heroId1, j, i);
                if(helpSkillId > 0)
                {
                    var skillCfg = SkillConfig.GetConfig(helpSkillId);
                    var color = skillCfg.Attr == "ap" ? new Color(0.55f, 0.55f, 1f, 0.6f) : (skillCfg.Attr == "might" ? new Color(0.95f, 0.4f, 0.4f, 0.6f) : new Color(0.7f, 0.8f, 0.3f, 0.6f));
                    // 创建连接线
                    CreateConnectionLine(heroUnits[i].transform, heroUnits[j].transform, color, new Vector2(25, 25), Resources.Load<Sprite>("SkillPic/" + skillCfg.Icon));
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
        if(itemCfg.RemoveWhenUse)
        {
            p1.UseItemToHero(heroCardId, itemCardId);

            itemDetail.gameObject.SetActive(false);
            GameManager.Instance.PlaySound("Sounds/eat");

            RemoveCell(itemCardId);
        }
        else
        {
            // 装备到空槽：没有空槽或没有多余副本时失败
            if (!p1.Equip(heroCardId, itemCardId))
            {
                ShowTipText(p1.GetEquippedCount(itemCardId) >= (p1.cards.TryGetValue(itemCardId, out var owned) ? owned : 0)
                    ? "没有多余副本可装备" : "装备槽已满，无法装备");
                return;
            }

            GameManager.Instance.PlaySound("Sounds/equip");

            UpdateView(); // 装备后背包不再显示该装备，需整体刷新（内部会重建格子）

            itemDetail.gameObject.SetActive(true);
            itemDetail.UpdateInfo(itemCardId, HeroSelectionTool.GetCardLevel(p1.cards[itemCardId], false));
        }

        heroDetail.gameObject.SetActive(true);
        heroDetail.UpdateInfo(heroCardId, HeroSelectionTool.GetCardLevel(p1.cards[heroCardId], true));
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
            ShowTipText("该英雄没有装备");
            return;
        }

        GameManager.Instance.PlaySound("Sounds/equip");
        UpdateView(); // 卸下的装备回到背包，整体刷新

        heroDetail.gameObject.SetActive(true);
        heroDetail.UpdateInfo(heroCardId, HeroSelectionTool.GetCardLevel(p1.cards[heroCardId], true));
    }

    private void RemoveCell(int itemCardId)
    {
        var cell = cellCache.Find(x => x.GetComponent<BagCell>().cardId == itemCardId);
        if (cell != null)
        {
            cellCache.Remove(cell);
            Destroy(cell);
        }
    }

    // 在信息栏短暂显示提示文字（2秒后恢复原信息）
    private Coroutine tipCoroutine;
    private void ShowTipText(string msg)
    {
        if (tipCoroutine != null)
            StopCoroutine(tipCoroutine);
        tipCoroutine = StartCoroutine(ShowTipTextCo(msg));
    }

    private IEnumerator ShowTipTextCo(string msg)
    {
        infoText.text = msg;
        infoText.color = Color.red;
        yield return new WaitForSeconds(2f);
        infoText.color = Color.white;
        UpdateExpView(); // 恢复原来的信息文本
    }

    public void SetHeroForBattle(int heroId, int pos)
    {
        var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);
        if(p1.isAI)
            return;

        p1.SetBattlePos(heroId, pos);

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

        // 物品每格一件只卖一件；英雄整组卖出
        p1.SellCard(cardId, ConfigManager.IsHeroCard(cardId) ? 0 : 1);
        RemoveCell(cardId);
        
        heroDetail.Clear();
        itemDetail.Clear();
        heroDetail.gameObject.SetActive(false);
        itemDetail.gameObject.SetActive(false);


        GameManager.Instance.PlaySound("Sounds/gold");        
        UpdateFieldView();
        UpdateExpView();
    }

    public void OnCellClick(BagCell cell)
    {
        if(!bindPlayer.cards.ContainsKey(cell.cardId))
            return;

        if (ConfigManager.IsHeroCard(cell.cardId))
        {
            heroDetail.UpdateInfo(cell.cardId, cell.level);
            heroDetail.gameObject.SetActive(true);
        }
        else
        {
            itemDetail.UpdateInfo(cell.cardId, cell.level);
            itemDetail.gameObject.SetActive(true);
        }
        foreach (var bagCell in cellCache)
        {
            var bagCellInfo = bagCell.GetComponent<BagCell>();
            if (bagCellInfo.cardId == heroDetail.cardId || bagCellInfo.cardId == itemDetail.cardId)
                bagCellInfo.OnSelect(true);
            else
                bagCellInfo.OnSelect(false);

        }
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
