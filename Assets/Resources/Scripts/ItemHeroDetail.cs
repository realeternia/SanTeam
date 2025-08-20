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

    public TMP_Text nameText;
    public TMP_Text leadText;
    public TMP_Text inteText;
    public TMP_Text strText;
    public TMP_Text hpText;
    public Image skillImg;
    public TMP_Text skillText;


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

            if (heroConfig.Skills != null && heroConfig.Skills.Length > 0)
            {
                var skillConfig = SkillConfig.GetConfig(heroConfig.Skills[0]);

                skillImg.sprite = Resources.Load<Sprite>("SkillPic/" + skillConfig.Icon);
                skillText.text = skillConfig.Descript;
                skillImg.gameObject.SetActive(true);
                skillText.gameObject.SetActive(true);
            }
            else
            {
                skillImg.gameObject.SetActive(false);
                skillText.gameObject.SetActive(false);
            }

            if (player.itemEquips.ContainsKey(cardId))
            {
                var equipCardId = player.itemEquips[cardId];

                var equipName = ItemConfig.GetConfig(equipCardId).Name;
                equipText.text = equipName;
                var cardLevel = HeroSelectionTool.GetCardLevel(player.cards[equipCardId]);  
                attrEquip = HeroSelectionTool.GetCardAttr(equipCardId, cardLevel);
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
        skillImg.gameObject.SetActive(false);
        skillText.gameObject.SetActive(false);



    }
}
