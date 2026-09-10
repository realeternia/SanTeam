using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class BattleResultHeroCellControl : MonoBehaviour
{
    public Image heroIcon;
    public TMP_Text playerName;
    public TMP_Text playerMark1;
    public TMP_Text playerMark2;
    public TMP_Text playerMark3;
    public TMP_Text playerRank;

    public Image playerIcon;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(BattleStatManager.BattleStat battleStat, int rank)
    {
        var player = GameManager.Instance.GetPlayer(battleStat.playerId);
        var heroLevel = HeroSelectionTool.GetCardLevel(player.cards[battleStat.heroId], true);
        var heroCfg = HeroConfig.GetConfig(battleStat.heroId);
        playerName.text = heroLevel.ToString() + heroCfg.Name;

        playerRank.text = rank.ToString(); // 假设按match顺序排列
        playerMark1.text = "总:" + battleStat.damage.ToString();
        playerMark2.text = "技:" + battleStat.skillDamage.ToString();
        playerMark3.text = "英:" + battleStat.heroDamage.ToString();

        playerIcon.sprite = player.playerImage.sprite;
        heroIcon.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
    }
}
