using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using CommonConfig;
using System.Runtime.Versioning;

public class ItemDetail : MonoBehaviour
{
    public int cardId;
    public int level;
    public BagControl bagControl;

    public TMP_Text nameText;
    public Image attr1Icon;
    public TMP_Text attr1Val;
    public Image attr2Icon;
    public TMP_Text attr2Val;
    public TMP_Text goldText;
    public TMP_Text equipText;
    public TMP_Text descText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateInfo(int id, int lv)
    {
        if(id == 0)
        {
            Clear();
            return;
        }

        cardId = id;
        level = lv;

        AttrInfo attrFinal = new AttrInfo();
        AttrInfo attrEquip = new AttrInfo();

        equipText.text = "";        

        var player = bagControl.bindPlayer;

        if (!ConfigManager.IsHeroCard(id))
        {
            var itemConfig = ItemConfig.GetConfig(id);
            nameText.text = itemConfig.Name;
            var sellRate = player.GetSellRate();
            goldText.text = ((int)(itemConfig.Price * player.cards[cardId] * sellRate)).ToString();
            descText.text = itemConfig.Des;

            if(string.IsNullOrEmpty(itemConfig.Attr1))
            {
                attr1Icon.gameObject.SetActive(false);
                attr1Val.gameObject.SetActive(false);
            }
            else
            {
                attr1Icon.gameObject.SetActive(true);
                attr1Val.gameObject.SetActive(true);
                attr1Icon.sprite = Resources.Load<Sprite>("Textures/attr" + itemConfig.Attr1);
                attr1Val.text = (itemConfig.Attr1Val * lv).ToString();
            }

            if(string.IsNullOrEmpty(itemConfig.Attr2))
            {
                attr2Icon.gameObject.SetActive(false);
                attr2Val.gameObject.SetActive(false);
            }
            else
            {
                attr2Icon.gameObject.SetActive(true);
                attr2Val.gameObject.SetActive(true);
                attr2Icon.sprite = Resources.Load<Sprite>("Textures/attr" + itemConfig.Attr2);
                attr2Val.text = (itemConfig.Attr2Val * lv).ToString();
            }


            foreach (var item in player.itemEquips)
            {
                if (item.Value == cardId)
                {
                    var equipName = HeroConfig.GetConfig(item.Key).Name;
                    equipText.text = equipName;
                    break;
                }
            }            
        }
    }

    public void UpdateSelf()
    {
        UpdateInfo(cardId, level);
    }

    public void Clear()
    {
        cardId = 0;
        attr1Icon.gameObject.SetActive(false);
        attr1Val.gameObject.SetActive(false);
        attr2Icon.gameObject.SetActive(false);
        attr2Val.gameObject.SetActive(false);


    }
}
