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
    public TMP_Text infoText;

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
            DestroyAllCells();
            PanelManager.Instance.HideBag();
            CardShopManager.Instance.OnShow();
        });
        fieldAutoBtn.onClick.AddListener(() =>
        {
            var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);
            p1.AutoSetBattleCard();
            UpdateFieldView();

            GameManager.Instance.PlaySound("Sounds/equip");
        });
        aiSwitchBtn.onClick.AddListener(() =>
        {
            bindPlayer.isAI = !bindPlayer.isAI;
            aiSwitchBtn.GetComponentInChildren<TMP_Text>().text = bindPlayer.isAI ? "AI模式" : "玩家模式";
            if (bagRecycler != null)
                bagRecycler.gameObject.SetActive(!bindPlayer.isAI);
            if (fieldAutoBtn != null)
                fieldAutoBtn.gameObject.SetActive(!bindPlayer.isAI);
        });

        for (int i = 0; i < 6; i++)
        {
            GameObject fieldUnit = Instantiate(Resources.Load<GameObject>("Prefabs/FieldUnit"), fieldRegion.transform);
            var fieldUnitControl = fieldUnit.GetComponent<BagFieldUnitControl>();
            fieldUnitControl.SetInfo(i, 0);
            fieldUnitControl.bagControl = this;

            int xOff = 160 * (i % 3);
            int yOff = 160 * (i / 3);
            if (i == 1)
                yOff -= 60;
            else if (i == 4)
                yOff += 60;

            fieldUnit.transform.localPosition = new Vector3(90 + xOff, -171 - yOff, 0);
        }
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
        
    }

    public void Bind(PlayerInfo p)
    {
        bindPlayer = p;
        UpdateView();

        if (bagRecycler != null)
            bagRecycler.gameObject.SetActive(!p.isAI);
        if (fieldAutoBtn != null)
            fieldAutoBtn.gameObject.SetActive(!p.isAI);

        heroDetail.gameObject.SetActive(false);
        itemDetail.gameObject.SetActive(false);

        UpdateFieldView();

        var soldierCfg = SoldierConfig.GetConfig(500001);
        var textAtk = (soldierCfg.Atk + bindPlayer.sodatk + bindPlayer.GetItemPAttr("satk")).ToString();
        var textHp = (soldierCfg.Hp + bindPlayer.sodhp + bindPlayer.GetItemPAttr("shp")).ToString();
        infoText.text = bindPlayer.playerConfig.Name + "\n<color=yellow>战斗力 </color>" + bindPlayer.lastFightMark + " <color=red>兵攻-</color>" + textAtk + " <color=green>兵血-</color>" + textHp + " <color=#FF7F00>粮食-</color>" + bindPlayer.maxFood;

        var humanCount = GameManager.Instance.players.Count(x => !x.isAI);
        aiSwitchBtn.gameObject.SetActive(bindPlayer.pid != 0 && bindPlayer.playerConfig.CanPlay && (!bindPlayer.isAI || humanCount < 2));
        aiSwitchBtn.GetComponentInChildren<TMP_Text>().text = bindPlayer.isAI ? "AI模式" : "玩家模式";
    }

    public void SendSignal(string name, string parm1, int parm2)
    {
        if(name == "SelectPlayer")
            Bind(GameManager.Instance.GetPlayer(parm2));
    }

    public void UpdateView()
    {
        int index = 0;

        var itemCards = bindPlayer.cards.Where(x => !ConfigManager.IsHeroCard(x.Key)).ToList();
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
            // 修改原代码，将新创建的 cell 加入缓存
            GameObject cell = Instantiate(Resources.Load<GameObject>("Prefabs/BagCellItem"), bagItemRegion.transform);
            cellCache.Add(cell);
            int xOff = index % 9;
            int yOff = index / 9;
            cell.transform.localPosition = new Vector3(95 + 104 * xOff, -71 - 104 * yOff, 0);
            
            BagCell bagCell = cell.GetComponent<BagCell>();
            bagCell.bagControl = this;            
            bagCell.cardId = itemCell.Key;
            bagCell.count = itemCell.Value;
            bagCell.level = HeroSelectionTool.GetCardLevel(itemCell.Value, false);
            bagCell.UpdateItemInfo(); 
            index++;
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

                if(heroId1 == 0 || heroId2 == 0)
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
                    var color = skillCfg.Attr == "inte" ? new Color(0.55f, 0.55f, 1f, 0.6f) : (skillCfg.Attr == "str" ? new Color(0.95f, 0.4f, 0.4f, 0.6f) : new Color(0.7f, 0.8f, 0.3f, 0.6f));
                    // 创建连接线
                    CreateConnectionLine(heroUnits[i].transform, heroUnits[j].transform, color, new Vector2(-25, -25), Resources.Load<Sprite>("SkillPic/" + skillCfg.Icon));
                }

                helpSkillId = ConfigManager.GetShowHelpSkillId(heroId2, heroId1, j, i);
                if(helpSkillId > 0)
                {
                    var skillCfg = SkillConfig.GetConfig(helpSkillId);
                    var color = skillCfg.Attr == "inte" ? new Color(0.55f, 0.55f, 1f, 0.6f) : (skillCfg.Attr == "str" ? new Color(0.95f, 0.4f, 0.4f, 0.6f) : new Color(0.7f, 0.8f, 0.3f, 0.6f));
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
            p1.Equip(heroCardId, itemCardId);

            itemDetail.gameObject.SetActive(true);
            itemDetail.UpdateInfo(itemCardId, HeroSelectionTool.GetCardLevel(p1.cards[itemCardId], false));
            GameManager.Instance.PlaySound("Sounds/equip");

            UpdateEquips();
        }

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

    public void SetHeroForBattle(int heroId, int pos)
    {
        var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);
        if(p1.isAI)
            return;

        p1.SetBattlePos(heroId, pos);

        GameManager.Instance.PlaySound("Sounds/equip");
        UpdateFieldView();
    }

    public void SellCard(int cardId)
    {
        var p1 = GameManager.Instance.GetPlayer(bindPlayer.pid);
        if(p1.isAI)
            return;

        p1.SellCard(cardId);
        RemoveCell(cardId);
        
        heroDetail.Clear();
        itemDetail.Clear();
        heroDetail.gameObject.SetActive(false);
        itemDetail.gameObject.SetActive(false);


        GameManager.Instance.PlaySound("Sounds/gold");        
        UpdateFieldView();
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
