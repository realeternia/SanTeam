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
        // bindPlayer.cards[100003] = 1;
        // bindPlayer.cards[100005] = 1;
        // bindPlayer.cards[100006] = 3;
        // for(int i = 0; i < 6; i++)
        //     bindPlayer.cards[101001 + i + 1] = 1;
        // for(int i = 0; i < 13; i++)
        //     bindPlayer.cards[400001 + i + 1] = 1;            
        // OnShow();

        closeBtn.onClick.AddListener(() =>
        {      
            DestroyAllCells();
            PanelManager.Instance.HideBag();
            CardShopManager.Instance.OnShow();
        });  
        fieldAutoBtn.onClick.AddListener(() =>
        {
            var p1 = GameManager.Instance.GetPlayer(0);
            p1.AutoSetBattleCard();
            UpdateFieldView();
        });

        for (int i = 0; i < 6; i++)
        {
            GameObject fieldUnit = Instantiate(Resources.Load<GameObject>("Prefabs/FieldUnit"), fieldRegion.transform);
            var fieldUnitControl = fieldUnit.GetComponent<BagFieldUnitControl>();
            fieldUnitControl.SetInfo(i, 0);
            fieldUnitControl.bagControl = this;

            int xOff = 135 * (i % 3);
            int yOff = 135 * (i / 3);
            if (i == 1)
                yOff -= 30;
            else if (i == 4)
                yOff += 30;

            fieldUnit.transform.localPosition = new Vector3(121 + xOff, -186 - yOff, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnShow()
    {
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
            bagRecycler.gameObject.SetActive(p.pid == 0);
        if (fieldAutoBtn != null)
            fieldAutoBtn.gameObject.SetActive(p.pid == 0);

        heroDetail.gameObject.SetActive(false);
        itemDetail.gameObject.SetActive(false);

        UpdateFieldView();

        var soldierCfg = SoldierConfig.GetConfig(500001);
        var textAtk = (soldierCfg.Atk + p.sodatk).ToString();
        var textHp = (soldierCfg.Hp + p.sodhp * 5).ToString();
        infoText.text = "<color=yellow>战斗力-</color>" + bindPlayer.lastFightMark + " <color=red>士兵攻击-</color>" + textAtk + " <color=green>士兵生命值-</color>" + textHp;
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
            bagCell.cardId = item.Key;
            bagCell.level = HeroSelectionTool.GetCardLevel(item.Value);
            UpdateHeroInfo(bagCell);

            bagCell.bagControl = this;

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
            bagCell.cardId = itemCell.Key;
            bagCell.level = HeroSelectionTool.GetCardLevel(itemCell.Value);            
            UpdatItemInfo(bagCell); 

            bagCell.bagControl = this;

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
            UpdateHeroInfo(bagCell);
        }
        foreach (Transform child in bagItemRegion.transform)
        {
            var bagCell = child.GetComponent<BagCell>();
            UpdatItemInfo(bagCell);
        }
    }

    public void UpdateFieldView()
    {
        foreach (Transform child in fieldRegion.transform)
        {
            var fieldUnit = child.GetComponent<BagFieldUnitControl>();
            fieldUnit.SetInfo(fieldUnit.posId, bindPlayer.battleCards.Length > fieldUnit.posId ? bindPlayer.battleCards[fieldUnit.posId] : 0);
        }
    }

    private void UpdatItemInfo(BagCell bagCell)
    {
        bagCell.textItemCount.text = bagCell.level.ToString();
        var itemCfg = ItemConfig.GetConfig(bagCell.cardId);
        bagCell.itemImage.sprite = Resources.Load<Sprite>("ItemPic/" + itemCfg.Icon);

        if (bindPlayer.itemEquips.ContainsValue(bagCell.cardId))
        {
            bagCell.shieldImage.gameObject.SetActive(true);
        }
        else
        {
            bagCell.shieldImage.gameObject.SetActive(false);
        }

    }

    private void UpdateHeroInfo(BagCell bagCell)
    {
        var heroCfg = HeroConfig.GetConfig(bagCell.cardId);
        bagCell.textItemCount.text = bagCell.level.ToString();
        bagCell.textItemName.text = heroCfg.Name;
        if (bindPlayer.itemEquips.ContainsKey(bagCell.cardId))
        {
            bagCell.equipImage.gameObject.SetActive(true);
            bagCell.equipImage.sprite = Resources.Load<Sprite>("ItemPic/" + ItemConfig.GetConfig(bindPlayer.itemEquips[bagCell.cardId]).Icon);
        }
        else
        {
            bagCell.equipImage.gameObject.SetActive(false);
        }
        bagCell.itemImage.sprite = Resources.Load<Sprite>("SkinsBig/" + heroCfg.Icon);
    }

    // 将物品装备到英雄的方法，供拖拽功能使用
    public void EquipItemToHero(int itemCardId, int heroCardId)
    {
        if(itemCardId == 0 || heroCardId == 0)
            return;
            
        var p1 = GameManager.Instance.GetPlayer(0);
        p1.Equip(heroCardId, itemCardId);

        heroDetail.gameObject.SetActive(true);
        itemDetail.gameObject.SetActive(true);
        heroDetail.UpdateInfo(heroCardId, HeroSelectionTool.GetCardLevel(p1.cards[heroCardId]));
        itemDetail.UpdateInfo(itemCardId, HeroSelectionTool.GetCardLevel(p1.cards[itemCardId]));

        GameManager.Instance.PlaySound("Sounds/equip");
        UpdateEquips();
    }

    public void SetHeroForBattle(int heroId, int pos)
    {
        var p1 = GameManager.Instance.GetPlayer(0);
        p1.SetBattlePos(heroId, pos);

        GameManager.Instance.PlaySound("Sounds/equip");
        UpdateFieldView();
    }

    public void SellCard(int cardId)
    {
        var p1 = GameManager.Instance.GetPlayer(0);
        p1.SellCard(cardId);
        var cell = cellCache.Find(x => x.GetComponent<BagCell>().cardId == cardId);
        if(cell != null)
        {
            cellCache.Remove(cell);
            Destroy(cell);
        }
        if(CardShopManager.Instance != null)
            CardShopManager.Instance.OnPlayerSellCard();
        heroDetail.Clear();
        itemDetail.Clear();
        heroDetail.gameObject.SetActive(false);
        itemDetail.gameObject.SetActive(false);
        UpdateFieldView();
    }

    public void OnCellClick(BagCell cell)
    {
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
