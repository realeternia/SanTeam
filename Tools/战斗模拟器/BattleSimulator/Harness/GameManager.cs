// ============================================================
// 战斗模拟器 · Harness —— GameManager 无头替身
// 替代场景单例：players/GetPlayer/year/friendRdData/PlaySound
// ============================================================
using System.Collections.Generic;

public class GameManager
{
    [System.Serializable]
    public class FriendRandomData
    {
        public int id;
        public string name;
        public int[] friendIds;
    }

    public static GameManager Instance;

    // 2 玩家对战：players[0] = 甲(side1)，players[1] = 乙(side2)
    public PlayerInfo[] players;
    public List<FriendRandomData> friendRdData = new List<FriendRandomData>();
    public int year = 1;

    public PlayerInfo GetPlayer(int pid)
    {
        if (players == null || pid < 0 || pid >= players.Length)
            return null;
        return players[pid];
    }

    public void PlaySound(string path, int priority = 3)
    {
        // 无音效
    }
}
