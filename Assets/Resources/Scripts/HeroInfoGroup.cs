using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConfig;

public class HeroInfoGroup : MonoBehaviour
{
    public GameObject heroInfoRectSide1;
    public GameObject heroInfoRectSide2;
    private int countSide1;
    private int countSide2;
    public GameObject heroPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        countSide1 = 0;
        countSide2 = 0;
        foreach (Transform child in heroInfoRectSide1.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in heroInfoRectSide2.transform)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Reset " + heroInfoRectSide1.transform.childCount + " " + heroInfoRectSide2.transform.childCount);
    }

    public HeroInfo AddHero(int side, int heroId, int level)
    {
        int count = side == 1 ? countSide1 : countSide2;
        GameObject heroInfoRect = side == 1 ? heroInfoRectSide1 : heroInfoRectSide2;
        HeroInfo heroInfo = Instantiate(heroPrefab, heroInfoRect.transform).GetComponent<HeroInfo>();
        heroInfo.transform.localPosition = new Vector3(105, 50 - 102 * count, 0);
        var heroCfg = HeroConfig.GetConfig((uint)heroId);

        // 确定英雄的最高属性
        float inte = heroCfg.Inte;
        float leadShip = heroCfg.LeadShip;
        float str = heroCfg.Str;

        string highestAttr = "";
        if (inte >= leadShip && inte >= str && inte >= 90 )
        {
            highestAttr = "attrinte";
        }
        else if (leadShip >= inte && leadShip >= str && leadShip >= 90)
        {
            highestAttr = "attrlead";
        }
        else if (str >= inte && str >= leadShip && str >= 90)
        {
            highestAttr = "attrstr";
        }     
        else if (heroCfg.Total >= 230)
        {
            highestAttr = "attrshield";
        }


        heroInfo.heroImage.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
        if(highestAttr != "")
        {       
             // 根据最高属性加载对应图片
            heroInfo.classImg.sprite = Resources.Load<Sprite>(highestAttr);
            if(heroCfg.Total >= 250)
            {
                heroInfo.classImg.color = Color.red;
            }
            else if (heroCfg.Total >= 240)
            {
                heroInfo.classImg.color = Color.yellow;
            }
        }

        heroInfo.heroName.text = heroCfg.Name;
        heroInfo.heroLevelTxt.text = "等级 " + level;
        if(side == 1)
        {
            countSide1++;
        }
        else
        {
            countSide2++;
        }

        return heroInfo;
    }
}
