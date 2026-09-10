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

        playerRank.text = rank.ToString(); // 假设按match顺序排列
        playerMark.text = $"<color=white>{player.mark}</color> (<color=green>+{killMark}</color>)";


        playerIcon.sprite = player.playerImage.sprite;
    }
}
