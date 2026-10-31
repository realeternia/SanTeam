using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleResultCellControl : MonoBehaviour
{
    public TMP_Text playerName;
    public TMP_Text playerMark;
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

    public void SetData(PlayerInfo player, int rank, int killMark)
    {
        playerName.text = player.playerNameText.text;

        playerRank.text = rank.ToString(); // 按生命值(mark)由高到低排序的名次
        // mark 现为生命值：失败扣血后直接显示剩余生命，本回合扣血量用红色标注（生命≤0 表示已淘汰）
        playerMark.text = player.lastDamage > 0
            ? $"HP <color=white>{player.mark}</color> <color=red>(-{player.lastDamage})</color>"
            : $"HP <color=white>{player.mark}</color>";


        playerIcon.sprite = player.playerImage.sprite;
    }
}
