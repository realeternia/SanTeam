using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;

public class BagControl : MonoBehaviour
{
    public Button closeBtn;
    public Button sellBtn;
    // Start is called before the first frame update
    // 声明一个列表用于缓存 cell 对象
    private List<GameObject> cellCache = new List<GameObject>();
    public ItemDetail detail;
    public GameObject bagItemRegion;

    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {      
            DestroyAllCells();
            PanelManager.Instance.HideBag();
        });
        sellBtn.onClick.AddListener(() =>
        {
            var p1 = GameManager.Instance.GetPlayer(0);
            p1.SellCard(detail.cardId);
            var cell = cellCache.Find(x => x.GetComponent<BagCell>().cardId == detail.cardId);
            if(cell != null)
            {
                cellCache.Remove(cell);
                Destroy(cell);
            }
            CardShopManager.Instance.OnPlayerSellCard();
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
        foreach (var item in p1.cards)
        {
            // 修改原代码，将新创建的 cell 加入缓存
            GameObject cell = Instantiate(Resources.Load<GameObject>("Prefabs/BagCellItem"), bagItemRegion.transform);
            cellCache.Add(cell);
            int xOff = index % 8;
            int yOff = index / 8;
            cell.transform.localPosition = new Vector3(80 + 104 * xOff, -80 - 104 * yOff, 0);
            BagCell bagCell = cell.GetComponent<BagCell>();
            bagCell.cardId = item.Key;
            bagCell.level = HeroSelectionTool.GetCardLevel(item.Value);
            bagCell.itemNameText.text = bagCell.level.ToString();
            if(ConfigManager.IsHeroCard(bagCell.cardId))
            {       
                var heroCfg = HeroConfig.GetConfig(item.Key);
                bagCell.itemImage.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
            }
            else
            {
                var itemCfg = ItemConfig.GetConfig(item.Key);
                bagCell.itemImage.sprite = Resources.Load<Sprite>("ItemPic/" + itemCfg.Icon);
            }
          
            bagCell.bagControl = this;

            index++;
        }
    }

    public void OnHide()
    {
        
    }


    public void OnCellClick(BagCell cell)
    {
        foreach (var bagCell in cellCache)
        {
            bagCell.GetComponent<BagCell>().OnSelect(false);
        }
        cell.GetComponent<BagCell>().OnSelect(true);
        detail.UpdateInfo(cell.cardId, cell.level);
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
