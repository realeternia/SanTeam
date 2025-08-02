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
            p1.SellCard(detail.heroId);
            var cell = cellCache.Find(x => x.GetComponent<BagCell>().heroId == detail.heroId);
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
            var heroCfg = HeroConfig.GetConfig((uint)item.Key);

            // 修改原代码，将新创建的 cell 加入缓存
            GameObject cell = Instantiate(Resources.Load<GameObject>("Prefabs/BagCellItem"), transform);
            cellCache.Add(cell);
            int xOff = index % 8;
            int yOff = index / 8;
            cell.transform.localPosition = new Vector3(80 + 104 * xOff, -80 - 104 * yOff, 0);
            BagCell bagCell = cell.GetComponent<BagCell>();
            bagCell.heroId = item.Key;
            bagCell.level = item.Value;
            bagCell.itemNameText.text = item.Value.ToString();
            bagCell.itemImage.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
            bagCell.bagControl = this;

            index++;
        }
    }

    public void OnCellClick(BagCell cell)
    {
        foreach (var bagCell in cellCache)
        {
            bagCell.GetComponent<BagCell>().OnSelect(false);
        }
        cell.GetComponent<BagCell>().OnSelect(true);

        var heroConfig = HeroConfig.GetConfig((uint)cell.heroId);
        var lv = cell.level;
        var maxHp = heroConfig.Hp * (14 + lv) / 15;
       var inte = heroConfig.Inte + System.Math.Max(8 * (lv - 1), heroConfig.Inte * (lv - 1) / 10);
       var str = heroConfig.Str + System.Math.Max(8 * (lv - 1), heroConfig.Str * (lv - 1) / 10);
       var leadShip = heroConfig.LeadShip + System.Math.Max(8 * (lv - 1), heroConfig.LeadShip * (lv - 1) / 10);

        detail.heroId = cell.heroId;
        detail.leadText.text = heroConfig.LeadShip.ToString();
        if(leadShip > heroConfig.LeadShip)
            detail.leadText.text =  heroConfig.LeadShip.ToString() + "<color=green>+" + (leadShip - heroConfig.LeadShip).ToString() + "</color>";
        detail.inteText.text = heroConfig.Inte.ToString();
        if(inte > heroConfig.Inte)
            detail.inteText.text =  heroConfig.Inte.ToString() + "<color=green>+" + (inte - heroConfig.Inte).ToString() + "</color>";
        detail.strText.text = heroConfig.Str.ToString();
        if(str > heroConfig.Str)
            detail.strText.text =  heroConfig.Str.ToString() + "<color=green>+" + (str - heroConfig.Str).ToString() + "</color>";
        detail.hpText.text = heroConfig.Hp.ToString();
        if(maxHp > heroConfig.Hp)
            detail.hpText.text =  heroConfig.Hp.ToString() + "<color=green>+" + (maxHp - heroConfig.Hp).ToString() + "</color>";
        detail.goldText.text = (HeroSelectionTool.GetPrice(heroConfig)*lv/2).ToString();
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
