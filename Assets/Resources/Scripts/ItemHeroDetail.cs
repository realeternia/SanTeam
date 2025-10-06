using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using CommonConfig;

public class ItemHeroDetail : MonoBehaviour
{
    public int cardId;
    public int level;
    public BagControl bagControl;

    public TMP_Text nameText;
    public TMP_Text leadText;
    public TMP_Text inteText;
    public TMP_Text strText;
    public TMP_Text hpText;
    public Image[] skillImg;

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

        AttrInfo attrFinal = new AttrInfo();
        AttrInfo attrEquip = new AttrInfo();

        equipText.text = "";        

        var player = bagControl.bindPlayer;

        if (ConfigManager.IsHeroCard(id))
        {
            var heroConfig = HeroConfig.GetConfig(id);

            maxHpBase = heroConfig.Hp;
            inteBase = heroConfig.Inte;
            strBase = heroConfig.Str;
            leadShipBase = heroConfig.LeadShip;

            nameText.text = heroConfig.Name;
            var sellRate = player.GetSellRate();
            goldText.text = ((int)(HeroSelectionTool.GetPrice(heroConfig) * player.cards[cardId] * sellRate)).ToString();

            for (int i = 0; i < 3; i++)
            {
                if (heroConfig.Skills != null && heroConfig.Skills.Length > i)
                {
                    var skillConfig = SkillConfig.GetConfig(heroConfig.Skills[i]);

                    skillImg[i].sprite = Resources.Load<Sprite>("SkillPic/" + skillConfig.Icon);
                    skillImg[i].gameObject.SetActive(true);
                }
                else
                {
                    skillImg[i].gameObject.SetActive(false);
                }
            }

            if (player.itemEquips.ContainsKey(cardId))
            {
                var equipCardId = player.itemEquips[cardId];

                var equipName = ItemConfig.GetConfig(equipCardId).Name;
                equipText.text = equipName;
                var cardLevel = HeroSelectionTool.GetCardLevel(player.cards[equipCardId], false);  
                attrEquip = HeroSelectionTool.GetCardAttr(player, equipCardId, cardLevel);
            } 
            
        }

        attrFinal = HeroSelectionTool.GetCardAttr(player, cardId, lv);

        leadText.text = attrFinal.Lead.ToString();
        if (attrEquip.Lead > 0)
            leadText.text += "\n<color=#FFB6C1>+" + attrEquip.Lead.ToString() + "</color>";

        inteText.text = attrFinal.Inte.ToString();
        if (attrEquip.Inte > 0)
            inteText.text += "\n<color=#FFB6C1>+" + attrEquip.Inte.ToString() + "</color>";            
        strText.text = attrFinal.Str.ToString();
        if (attrEquip.Str > 0)
            strText.text += "\n<color=#FFB6C1>+" + attrEquip.Str.ToString() + "</color>";            
        hpText.text = attrFinal.Hp.ToString();
        if (attrEquip.Hp > 0)
            hpText.text += "\n<color=#FFB6C1>+" + attrEquip.Hp.ToString() + "</color>";

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
        for(int i=0; i<skillImg.Length; i++)
            skillImg[i].gameObject.SetActive(false);

    }
}
