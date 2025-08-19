using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConfig;
using TMPro;

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
        heroInfo.transform.localPosition = new Vector3(115, -63 - 122 * count, 0);
        var heroCfg = HeroConfig.GetConfig(heroId);

        heroInfo.heroImage.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);

        heroInfo.heroName.text = heroCfg.Name;
        heroInfo.heroLevelTxt.text = level.ToString();

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
