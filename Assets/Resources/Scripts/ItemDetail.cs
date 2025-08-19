using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CommonConfig;

public class ItemDetail : MonoBehaviour
{
    public int cardId;
    public TMP_Text nameText;
    public TMP_Text leadText;
    public TMP_Text inteText;
    public TMP_Text strText;
    public TMP_Text hpText;
    public TMP_Text goldText;
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

        var maxHpBase = 0;
        var inteBase = 0;
        var strBase = 0;
        var leadShipBase = 0;

        var maxHpFinal = 0;
        var inteFinal = 0;
        var strFinal = 0;
        var leadShipFinal = 0;

        if (ConfigManager.IsHeroCard(id))
        {
            var heroConfig = HeroConfig.GetConfig(id);

            maxHpBase = heroConfig.Hp;
            inteBase = heroConfig.Inte;
            strBase = heroConfig.Str;
            leadShipBase = heroConfig.LeadShip;

            nameText.text = heroConfig.Name;
            goldText.text = (HeroSelectionTool.GetPrice(heroConfig) * GameManager.Instance.GetPlayer(0).cards[cardId] / 2).ToString();
        }
        else
        {
            var itemConfig = ItemConfig.GetConfig(id);
            nameText.text = itemConfig.Name;
            goldText.text = itemConfig.Price.ToString();

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
        }

        maxHpFinal = maxHpBase * (9 + lv) / 10;
        inteFinal = inteBase + System.Math.Max(8 * (lv - 1), inteBase * (lv - 1) / 10);
        strFinal = strBase + System.Math.Max(8 * (lv - 1), strBase * (lv - 1) / 10);
        leadShipFinal = leadShipBase + System.Math.Max(8 * (lv - 1), leadShipBase * (lv - 1) / 10);

        leadText.text = leadShipBase.ToString();
        if (leadShipFinal > leadShipBase)
            leadText.text = leadShipBase.ToString() + "<color=green>+" + (leadShipFinal - leadShipBase).ToString() + "</color>";
        inteText.text = inteBase.ToString();
        if (inteFinal > inteBase)
            inteText.text = inteBase.ToString() + "<color=green>+" + (inteFinal - inteBase).ToString() + "</color>";
        strText.text = strBase.ToString();
        if (strFinal > strBase)
            strText.text = strBase.ToString() + "<color=green>+" + (strFinal - strBase).ToString() + "</color>";
        hpText.text = maxHpBase.ToString();
        if (maxHpFinal > maxHpBase)
            hpText.text = maxHpBase.ToString() + "<color=green>+" + (maxHpFinal - maxHpBase).ToString() + "</color>";

    }
}
