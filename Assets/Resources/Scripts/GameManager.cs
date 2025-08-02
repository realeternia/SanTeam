using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerInfo[] players;   

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        var p1 = PlayerBook.playerWang;
        players[0].Init(p1.name, p1.imgPath, 40);
        var pls = PlayerBook.GetRandomN(3);
        for(int i = 0; i < 3; i++)
            players[i + 1].Init(pls[i].name, pls[i].imgPath, 40);           
    }
  

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearTurn()
    {
        foreach(var p in players)
            p.isOnTurn = false;    
    }

    public void OnPlayerTurn(int pid)
    {
        foreach(var p in players)
            p.isOnTurn = false;
        players[pid].isOnTurn = true;
    }

    public PlayerInfo GetPlayer(int pid)
    {
        return players[pid];
    }

}
