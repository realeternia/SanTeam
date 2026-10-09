// ============================================================
// 选牌模拟器 · Harness —— GameManager 无头替身
// 替代场景单例：8 名玩家、年份、英雄池、好友数据、回合推进与音效（空实现）。
// 方法签名与游戏源码保持一致，供零修改链接的商店/AI 代码调用。
// ============================================================
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

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

    public PlayerInfo[] players;
    public List<FriendRandomData> friendRdData = new List<FriendRandomData>();
    public List<int> heroIds = new List<int>();
    public int year = 0;

    public PlayerInfo GetPlayer(int pid)
    {
        if (players == null || pid < 0 || pid >= players.Length)
            return null;
        return players[pid];
    }

    public PlayerInfo GetFirstNoAiPlayer()
    {
        if (players == null)
            return null;
        foreach (var p in players)
            if (p != null && p.pid > 0 && !p.isAI)
                return p;
        return null;
    }

    public void ClearTurn()
    {
        if (players == null)
            return;
        foreach (var p in players)
            if (p != null)
                p.isOnTurn = false;
    }

    public void OnPlayerTurn(int pid)
    {
        if (players == null)
            return;
        foreach (var p in players)
            if (p != null)
                p.isOnTurn = false;
        if (pid >= 0 && pid < players.Length && players[pid] != null)
            players[pid].isOnTurn = true;
    }

    public void PlaySound(string path, int prioty = 3)
    {
        // 无音效
    }

    // 无存档系统：模拟器不需要落盘
    public void SaveToFile() { }

    public void InitFriend(bool loadSave)
    {
        if (!loadSave)
            friendRdData = new List<FriendRandomData>();
        ConfigManager.InitFriend();
    }

    public void InitHeros(bool loadSave)
    {
        if (!loadSave)
            BuildHeros();
        HeroSelectionTool.UpdateHeroPoolCache(heroIds);
    }

    // 与游戏 GameManager.BuildHeros 保持一致：主公全进池 + 4个非魏蜀吴阵营 4 选 2 + 魏蜀吴全部
    private void BuildHeros()
    {
        List<HeroConfig> allHeroes = new List<HeroConfig>(HeroConfig.ConfigList);
        heroIds = new List<int>();

        List<HeroConfig> tempHeroes = new List<HeroConfig>(allHeroes);
        foreach (var hero in tempHeroes)
        {
            if (ConfigManager.IsKingHero(hero.Id))
            {
                heroIds.Add(hero.Id);
                allHeroes.Remove(hero);
            }
        }

        int[] sides = { 4, 5, 6, 10 };
        for (int i = 0; i < 2; i++)
        {
            var side = sides[SysRandom.Range(0, sides.Length)];
            sides = sides.Where(s => s != side).ToArray();
            foreach (var hero in allHeroes.FindAll(h => h.Side == side))
                heroIds.Add(hero.Id);
        }

        foreach (int side in new[] { 1, 2, 3 })
        {
            foreach (var hero in allHeroes.FindAll(h => h.Side == side))
                heroIds.Add(hero.Id);
        }
    }
}
