using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CommonConfig;

public class ItemDetail : MonoBehaviour
{
    public int heroId;
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
        heroId = id;
        var heroConfig = HeroConfig.GetConfig(id);
        var maxHp = heroConfig.Hp * (9 + lv) / 10;
        var inte = heroConfig.Inte + System.Math.Max(8 * (lv - 1), heroConfig.Inte * (lv - 1) / 10);
        var str = heroConfig.Str + System.Math.Max(8 * (lv - 1), heroConfig.Str * (lv - 1) / 10);
        var leadShip = heroConfig.LeadShip + System.Math.Max(8 * (lv - 1), heroConfig.LeadShip * (lv - 1) / 10);

        nameText.text = heroConfig.Name;
        leadText.text = heroConfig.LeadShip.ToString();
        if (leadShip > heroConfig.LeadShip)
            leadText.text = heroConfig.LeadShip.ToString() + "<color=green>+" + (leadShip - heroConfig.LeadShip).ToString() + "</color>";
        inteText.text = heroConfig.Inte.ToString();
        if (inte > heroConfig.Inte)
            inteText.text = heroConfig.Inte.ToString() + "<color=green>+" + (inte - heroConfig.Inte).ToString() + "</color>";
        strText.text = heroConfig.Str.ToString();
        if (str > heroConfig.Str)
            strText.text = heroConfig.Str.ToString() + "<color=green>+" + (str - heroConfig.Str).ToString() + "</color>";
        hpText.text = heroConfig.Hp.ToString();
        if (maxHp > heroConfig.Hp)
            hpText.text = heroConfig.Hp.ToString() + "<color=green>+" + (maxHp - heroConfig.Hp).ToString() + "</color>";
        goldText.text = (HeroSelectionTool.GetPrice(heroConfig) * GameManager.Instance.GetPlayer(0).cards[heroId] / 2).ToString();

    }
}
