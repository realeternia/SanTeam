using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;

public class RankPlayerCellInfo : MonoBehaviour
{
    public Image playerImg;
    public TMP_Text playerName;
    public TMP_Text textSoldierAtk;
    public TMP_Text textSoldierDef;
    public TMP_Text textFood;
    public TMP_Text textGold;
    public TMP_Text textPower;
    public TMP_Text textMark;

    public int playerId;
    public int soldierAtk;
    public int soldierHp;
    public int food;
    public int gold;
    public int power;
    public int mark;

    // Start is called before the first frame update
    void Start()
    {
        playerName.raycastTarget = false;
        textSoldierAtk.raycastTarget = false;
        textSoldierDef.raycastTarget = false;
        textFood.raycastTarget = false;
        textGold.raycastTarget = false;
        textPower.raycastTarget = false;
        textMark.raycastTarget = false;
    }

    public void Init(PlayerInfo playerInfo)
    {
        // 设置英雄信息
        playerImg.sprite = Resources.Load<Sprite>(playerInfo.imgPath);

        playerName.text = playerInfo.playerConfig.Name;
        playerId = playerInfo.playerId;

        var soldierCfg = SoldierConfig.GetConfig(500001);
        soldierAtk = playerInfo.sodatk + soldierCfg.Atk + playerInfo.GetItemPAttr("satk");
        soldierHp = playerInfo.sodhp + soldierCfg.Hp + playerInfo.GetItemPAttr("shp");
        food = playerInfo.maxFood;
        gold = playerInfo.gold;
        power = playerInfo.lastFightMark;
        mark = playerInfo.mark;
        textSoldierAtk.text = soldierAtk.ToString();
        textSoldierDef.text = soldierHp.ToString();
        textFood.text = food.ToString();
        textGold.text = gold.ToString();
        textPower.text = power.ToString();
        textMark.text = mark.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
