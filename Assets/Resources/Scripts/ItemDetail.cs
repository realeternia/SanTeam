using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CommonConfig;

public class ItemDetail : MonoBehaviour
{
    public int cardId;
    public int level;

    public TMP_Text nameText;
    public TMP_Text leadText;
    public TMP_Text inteText;
    public TMP_Text strText;
    public TMP_Text hpText;
    public TMP_Text goldText;
    public TMP_Text equipText;

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
        cardId = id;
        level = lv;

        var maxHpBase = 0;
        var inteBase = 0;
        var strBase = 0;
        var leadShipBase = 0;

        var maxHpFinal = 0;
        var inteFinal = 0;
        var strFinal = 0;
        var leadShipFinal = 0;

        equipText.text = "";        

        if (ConfigManager.IsHeroCard(id))
        {
            var heroConfig = HeroConfig.GetConfig(id);

            maxHpBase = heroConfig.Hp;
            inteBase = heroConfig.Inte;
            strBase = heroConfig.Str;
            leadShipBase = heroConfig.LeadShip;

            nameText.text = heroConfig.Name;
            goldText.text = (HeroSelectionTool.GetPrice(heroConfig) * GameManager.Instance.GetPlayer(0).cards[cardId] / 2).ToString();

            if (player.itemEquips.ContainsKey(cardId))
            {
                var equipName = ItemConfig.GetConfig(player.itemEquips[cardId]).Name;
                equipText.text = equipName;
            }            
        }
        else
        {
            var itemConfig = ItemConfig.GetConfig(id);
            nameText.text = itemConfig.Name;
            goldText.text = (itemConfig.Price / 2).ToString();

            if (itemConfig.Attr1 == "str")
            {
                strBase = itemConfig.Attr1Val;
            }
            else if (itemConfig.Attr1 == "inte")
            {
                inteBase = itemConfig.Attr1Val;
            }
            else if (itemConfig.Attr1 == "lead")
            {
                leadShipBase = itemConfig.Attr1Val;
            }
            else if (itemConfig.Attr1 == "shield")
            {
                maxHpBase = itemConfig.Attr1Val;
            }
            if (itemConfig.Attr2 == "str")
            {
                strBase = itemConfig.Attr2Val;
            }
            else if (itemConfig.Attr2 == "inte")
            {
                inteBase = itemConfig.Attr2Val;
            }
            else if (itemConfig.Attr2 == "lead")
            {
                leadShipBase = itemConfig.Attr2Val;
            }
            else if (itemConfig.Attr2 == "hp")
            {
                maxHpBase = itemConfig.Attr2Val;
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

        maxHpFinal = maxHpBase * (9 + lv) / 10;
        inteFinal = inteBase + System.Math.Max(8 * (lv - 1), inteBase * (lv - 1) / 10);
        strFinal = strBase + System.Math.Max(8 * (lv - 1), strBase * (lv - 1) / 10);
        leadShipFinal = leadShipBase + System.Math.Max(8 * (lv - 1), leadShipBase * (lv - 1) / 10);

        leadText.text = leadShipBase.ToString();
        if (leadShipFinal > leadShipBase)
            leadText.text += "<color=green>+" + (leadShipFinal - leadShipBase).ToString() + "</color>";
        inteText.text = inteBase.ToString();
        if (inteFinal > inteBase)
            inteText.text += "<color=green>+" + (inteFinal - inteBase).ToString() + "</color>";
        strText.text = strBase.ToString();
        if (strFinal > strBase)
            strText.text += "<color=green>+" + (strFinal - strBase).ToString() + "</color>";
        hpText.text = maxHpBase.ToString();
        if (maxHpFinal > maxHpBase)
            hpText.text += "<color=green>+" + (maxHpFinal - maxHpBase).ToString() + "</color>";

    }

    private void UpdateSelf()
    {
        UpdateInfo(cardId, level);
    }

    public void Clear()
    {
        cardId = 0;
        nameText.text = "";
        leadText.text = "";
        inteText.text = "";
        strText.text = "";
        hpText.text = "";
        goldText.text = "";
        equipText.text = "";


    }
}
