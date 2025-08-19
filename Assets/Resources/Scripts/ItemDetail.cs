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
        if(id == 0)
        {
            Clear();
            return;
        }

        cardId = id;
        level = lv;

        var maxHpBase = 0;
        var inteBase = 0;
        var strBase = 0;
        var leadShipBase = 0;

        HeroSelectionTool.AttrInfo attrFinal = new HeroSelectionTool.AttrInfo();
        HeroSelectionTool.AttrInfo attrEquip = new HeroSelectionTool.AttrInfo();

        equipText.text = "";        

        var player = GameManager.Instance.GetPlayer(0);

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
                var equipCardId = player.itemEquips[cardId];

                var equipName = ItemConfig.GetConfig(equipCardId).Name;
                equipText.text = equipName;
                var cardLevel = HeroSelectionTool.GetCardLevel(player.cards[equipCardId]);  
                attrEquip = HeroSelectionTool.GetCardAttr(equipCardId, cardLevel);
            } 
            
        }
        else
        {
            var itemConfig = ItemConfig.GetConfig(id);
            nameText.text = itemConfig.Name;
            goldText.text = (itemConfig.Price / 2).ToString();

            var attrBase = HeroSelectionTool.GetCardAttr(cardId, 1);
            maxHpBase = attrBase.Hp;
            inteBase = attrBase.Inte;
            strBase = attrBase.Str;
            leadShipBase = attrBase.Lead;

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

        attrFinal = HeroSelectionTool.GetCardAttr(cardId, lv);

        leadText.text = leadShipBase.ToString();
        if (attrFinal.Lead > leadShipBase)

            leadText.text += "<color=green>+" + (attrFinal.Lead - leadShipBase).ToString() + "</color>";
            if (attrEquip.Lead > 0)
            leadText.text += "<color=#FFB6C1>+" + attrEquip.Lead.ToString() + "</color>";

        inteText.text = inteBase.ToString();
        if (attrFinal.Inte > inteBase)
            inteText.text += "<color=green>+" + (attrFinal.Inte - inteBase).ToString() + "</color>";
        if (attrEquip.Inte > 0)
            inteText.text += "<color=#FFB6C1>+" + attrEquip.Inte.ToString() + "</color>";            
        strText.text = strBase.ToString();
        if (attrFinal.Str > strBase)
            strText.text += "<color=green>+" + (attrFinal.Str - strBase).ToString() + "</color>";
        if (attrEquip.Str > 0)
            strText.text += "<color=#FFB6C1>+" + attrEquip.Str.ToString() + "</color>";            
        hpText.text = maxHpBase.ToString();
        if (attrFinal.Hp > maxHpBase)
            hpText.text += "<color=green>+" + (attrFinal.Hp - maxHpBase).ToString() + "</color>";
        if (attrEquip.Hp > 0)
            hpText.text += "<color=#FFB6C1>+" + attrEquip.Hp.ToString() + "</color>";

    }

    public void UpdateSelf()
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
