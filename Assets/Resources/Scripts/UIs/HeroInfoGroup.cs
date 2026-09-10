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

    // 每行英雄行距(与 AddHero 摆放间距保持一致)
    private const int RowSpacing = 102;
    // 面板除英雄行外的固定留白：原 5 人行面板高度 610 = 100 + 102 * 5
    private const int RectFixedHeight = 100;
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
        UpdateRectHeight(heroInfoRectSide1, countSide1);
        UpdateRectHeight(heroInfoRectSide2, countSide2);
        GameLog.Debug("Reset " + heroInfoRectSide1.transform.childCount + " " + heroInfoRectSide2.transform.childCount);
    }

    public HeroInfo AddHero(int side, int heroId, int level)
    {
        int count = side == 1 ? countSide1 : countSide2;
        GameObject heroInfoRect = side == 1 ? heroInfoRectSide1 : heroInfoRectSide2;
        HeroInfo heroInfo = Instantiate(heroPrefab, heroInfoRect.transform).GetComponent<HeroInfo>();
        heroInfo.transform.localPosition = new Vector3(105, -53 - RowSpacing * count, 0);
        var heroCfg = HeroConfig.GetConfig(heroId);

        heroInfo.heroImage.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);

        heroInfo.heroName.text = heroCfg.Name;
        heroInfo.heroLevelTxt.text = level.ToString();

        if(side == 1)
        {
            countSide1++;
            UpdateRectHeight(heroInfoRectSide1, countSide1);
        }
        else
        {
            countSide2++;
            UpdateRectHeight(heroInfoRectSide2, countSide2);
        }

        return heroInfo;
    }

    // 面板高度随该侧英雄数量自适应：顶部锚点(左上)不动，向下扩展/收缩
    private void UpdateRectHeight(GameObject sideRect, int count)
    {
        var rect = sideRect.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, RowSpacing * count);
    }
}
