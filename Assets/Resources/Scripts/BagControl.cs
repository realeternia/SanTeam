using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;

public class BagControl : MonoBehaviour
{
    public Button closeBtn;
    public Button sellHeroBtn;
    public Button sellItemBtn;

    public Button equipBtn;


    // Start is called before the first frame update
    // 声明一个列表用于缓存 cell 对象
    private List<GameObject> cellCache = new List<GameObject>();
    public ItemHeroDetail heroDetail;
    public ItemDetail itemDetail;
    public GameObject bagHeroRegion;
    public GameObject bagItemRegion;

    void Start()
    {
        var p1 = GameManager.Instance.GetPlayer(0);
        p1.cards[100003] = 1;
        p1.cards[100005] = 1;
        p1.cards[100006] = 3;
        for(int i = 0; i < 6; i++)
            p1.cards[101001 + i + 1] = 1;
        for(int i = 0; i < 13; i++)
            p1.cards[400001 + i + 1] = 1;            
        OnShow();

        closeBtn.onClick.AddListener(() =>
        {      
            DestroyAllCells();
            PanelManager.Instance.HideBag();
            CardShopManager.Instance.OnShow();
        });
        sellHeroBtn.onClick.AddListener(() =>
        {
            if(heroDetail.cardId == 0)
                return;
            var p1 = GameManager.Instance.GetPlayer(0);
            p1.SellCard(heroDetail.cardId);
            var cell = cellCache.Find(x => x.GetComponent<BagCell>().cardId == heroDetail.cardId);
            if(cell != null)
            {
                cellCache.Remove(cell);
                Destroy(cell);
            }
            CardShopManager.Instance.OnPlayerSellCard();
            heroDetail.Clear();
            itemDetail.UpdateSelf();

        });
        sellItemBtn.onClick.AddListener(() =>
        {          
            if(itemDetail.cardId == 0)
                return;
            var p1 = GameManager.Instance.GetPlayer(0);
            p1.SellCard(itemDetail.cardId);
            var cell = cellCache.Find(x => x.GetComponent<BagCell>().cardId == itemDetail.cardId);
            if(cell != null)
            {
                cellCache.Remove(cell);
                Destroy(cell);
            }
            itemDetail.Clear();
            heroDetail.UpdateSelf();

        });    
        equipBtn.onClick.AddListener(() =>
        {
            if(itemDetail.cardId == 0 || heroDetail.cardId == 0)
                return;
                
            var p1 = GameManager.Instance.GetPlayer(0);
            p1.Equip(heroDetail.cardId, itemDetail.cardId);

            heroDetail.UpdateSelf();
            itemDetail.UpdateSelf();

            GameManager.Instance.PlaySound("Sounds/equip");

        });    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnShow()
    {
        var p1 = GameManager.Instance.GetPlayer(0);
        int index = 0;

        var itemCards = p1.cards.Where(x => !ConfigManager.IsHeroCard(x.Key)).ToList();
        var heroCards = p1.cards.Where(x => ConfigManager.IsHeroCard(x.Key)).ToList();

        foreach (var item in heroCards)
        {
            // 修改原代码，将新创建的 cell 加入缓存
            GameObject cell = Instantiate(Resources.Load<GameObject>("Prefabs/BagCellHero"), bagHeroRegion.transform);
            cellCache.Add(cell);
            int xOff = index % 6;
            int yOff = index / 6;

            var heroCfg = HeroConfig.GetConfig(item.Key);            
            cell.transform.localPosition = new Vector3(100 + 164 * xOff, -131 - 226 * yOff, 0);
            BagCell bagCell = cell.GetComponent<BagCell>();
            bagCell.cardId = item.Key;
            bagCell.level = HeroSelectionTool.GetCardLevel(item.Value);
            bagCell.textItemCount.text = bagCell.level.ToString();
            bagCell.textItemName.text = heroCfg.Name;

            bagCell.itemImage.sprite = Resources.Load<Sprite>("SkinsBig/" + heroCfg.Icon);

            bagCell.bagControl = this;

            index++;
        }
        index = 0;
        foreach (var item in itemCards)
        {
            // 修改原代码，将新创建的 cell 加入缓存
            GameObject cell = Instantiate(Resources.Load<GameObject>("Prefabs/BagCellItem"), bagItemRegion.transform);
            cellCache.Add(cell);
            int xOff = index % 9;
            int yOff = index / 9;
            cell.transform.localPosition = new Vector3(95 + 104 * xOff, -71 - 104 * yOff, 0);
            BagCell bagCell = cell.GetComponent<BagCell>();
            bagCell.cardId = item.Key;
            bagCell.level = HeroSelectionTool.GetCardLevel(item.Value);
            bagCell.textItemCount.text = bagCell.level.ToString();
            var itemCfg = ItemConfig.GetConfig(item.Key);
            bagCell.itemImage.sprite = Resources.Load<Sprite>("ItemPic/" + itemCfg.Icon);

            bagCell.bagControl = this;

            index++;
        }
        itemDetail.Clear();
        heroDetail.Clear();
    }

    public void OnHide()
    {
        
    }


    public void OnCellClick(BagCell cell)
    {
        if (ConfigManager.IsHeroCard(cell.cardId))
        {
            heroDetail.UpdateInfo(cell.cardId, cell.level);
        }
        else
        {
            itemDetail.UpdateInfo(cell.cardId, cell.level);
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
