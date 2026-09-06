using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;
using CommonConfig;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class FriendRandomData
    {
        public int id;
        public string name;
        public int[] friendIds;
    }
    
    // 用于Unity JsonUtility序列化的辅助类
    [System.Serializable]
    private class SaveData
    {
        public List<string> players = new List<string>();
        public List<FriendRandomData> friendRdData = new List<FriendRandomData>();
        public List<int> heroIds = new List<int>();
        public int year;
    }

    public static GameManager Instance;
    public PlayerInfo[] players; //不能new，都是配置好的
    public List<FriendRandomData> friendRdData;
    public List<int> heroIds;
    public int year;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        ConfigManager.Init();

        players[0].Init(0, PlayerBook.GetWang());
        var pls = PlayerBook.GetRandomN(7);
        for (int i = 0; i < 7; i++)
            players[i + 1].Init(i + 1, pls[i]);

        GameLog.Debug("GameManager Start");
    }

    private void OnDestroy()
    {
        // 关闭统一日志系统
        GameLog.Shutdown();
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

    public PlayerInfo GetFirstNoAiPlayer()
    {
        foreach(var p in players)
            if(p.pid > 0 && !p.isAI)
                return p;
        return null;
    }

    // 静态变量记录上次播放路径和 clip
    string lastPath = "";
    AudioClip lastClip = null;

    private int lastSoundPriority = -1;
    private float lastSoundTime = 0f;

    public void PlaySound(string path, int prioty = 3)
    {
        float currentTime = Time.time;
        // 如果当前优先级低于上一次且时间间隔小于1秒，则跳过播放
        if (prioty < lastSoundPriority && currentTime - lastSoundTime < 1.5f)
        {
            return;
        }

        // 更新上次播放信息
        lastSoundPriority = prioty;
        lastSoundTime = currentTime;
    
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (lastPath != path)
        {
            lastPath = path;
            lastClip = Resources.Load<AudioClip>(path);
            if (lastClip != null)
            {
                audioSource.clip = lastClip;
            }
        }

        if (audioSource.clip != null)
        {
            audioSource.Stop();
            audioSource.Play();
        }
    }

    public bool IsGameSaveExist()
    {
        string savePath = Application.persistentDataPath + "/game_save.json";
        
        if(!File.Exists(savePath))
            return false;
        return true;
    }

    public bool LoadFromSave()
    {
        string savePath = Application.persistentDataPath + "/game_save.json";
        if (!File.Exists(savePath))
            return false;
        try
        {
            string json = File.ReadAllText(savePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            year = saveData.year;

            // 确保players数组不为null且长度足够
            if (saveData.players != null)
            {
                for (int i = 0; i < saveData.players.Count; i++)
                {
                    players[i].Deserialize(saveData.players[i]);
                    players[i].SetPlayerData();
                    players[i].UpdateView();
                }
            }

            // 加载friendRdData
            if (saveData.friendRdData != null)
            {
                friendRdData = new List<FriendRandomData>();
                friendRdData.AddRange(saveData.friendRdData);
            }

            // 加载heroIds
            if (saveData.heroIds != null)
            {
                heroIds = new List<int>();
                heroIds.AddRange(saveData.heroIds);
            }

            GameLog.Debug("游戏数据加载成功 year=" + year);
        }
        catch (System.Exception e)
        {
            GameLog.Error("加载游戏数据失败: " + e.Message);
            return false;
        }
        return true;
    }

    public void SaveToFile()
    {
        string savePath = Application.persistentDataPath + "/game_save.json";
        try
        {
            SaveData saveData = new SaveData();

            saveData.year = year;
            // 序列化每个PlayerInfo对象
            foreach (PlayerInfo player in players)
            {
                if (player != null)
                {
                    string playerJson = player.Serialize();
                    if (!string.IsNullOrEmpty(playerJson))
                    {
                        saveData.players.Add(playerJson);
                    }
                }
            }
            
            // 保存friendRdData
            if (friendRdData != null)
            {
                saveData.friendRdData.AddRange(friendRdData);
            }
            
            // 保存heroIds
            if (heroIds != null)
            {
                saveData.heroIds.AddRange(heroIds);
            }
            
            // 使用JsonUtility序列化数据
            string json = JsonUtility.ToJson(saveData);
            File.WriteAllText(savePath, json);
            
            GameLog.Debug("游戏数据保存成功: " + savePath);
        }
        catch (System.Exception e)
        {
            GameLog.Error("保存游戏数据失败: " + e.Message);
        }
    }

    public void InitFriend(bool loadSave)
    {
        if (!loadSave)
        {
            // 随机好友功能已移除，新游戏清空历史随机配对数据
            friendRdData = new List<FriendRandomData>();
        }

        ConfigManager.InitFriend();
    }

    public void InitHeros(bool loadSave)
    {
        if(!loadSave)
            BuildHeros();

        HeroSelectionTool.UpdateHeroPoolCache(heroIds);
    }

    private void BuildHeros()
    {
        List<HeroConfig> allHeroes = new List<HeroConfig>(HeroConfig.ConfigList);
        heroIds = new List<int>();

        int[] sideCounts = new int[10];
        // 核心英雄（Id<100100 的王）始终进入英雄池
        List<HeroConfig> tempHeroes = new List<HeroConfig>(allHeroes);
        foreach (var hero in tempHeroes)
        {
            if (hero.Id < 100100)
            {
                heroIds.Add(hero.Id);
                sideCounts[hero.Side - 1]++;
                allHeroes.Remove(hero);
            }
        }

        // 先随机选择5-7张Side=4的卡牌
        int[] sides = {4, 5, 6, 10};
        for (int i = 0; i < 2; i++)
        {
            var side = sides[SysRandom.Range(0, sides.Length)];
            sides = sides.Where(s => s != side).ToArray();

            List<HeroConfig> side4Heroes = allHeroes.FindAll(hero => hero.Side == side);
            if (side4Heroes.Count > 0)
            {
                int side4Count = i + 6;
                side4Count = Mathf.Min(side4Count, side4Heroes.Count);

                if(HeroConfig.HasConfig(100000 + side))
                {
                    var heroConfig = HeroConfig.GetConfig(100000 + side);
                    heroIds.Add((int)heroConfig.Id);
                    sideCounts[side - 1]++;
                    allHeroes.Remove(heroConfig);
                    side4Heroes.Remove(heroConfig);
                }
                
                List<HeroConfig> tempSide4Heroes = new List<HeroConfig>(side4Heroes);
                for (int j = sideCounts[side - 1]; j < side4Count; j++)
                {
                    // 该阵营内随机选一张
                    int randomIndex = SysRandom.Range(0, tempSide4Heroes.Count);
                    HeroConfig heroCfg = tempSide4Heroes[randomIndex];
                    heroIds.Add((int)heroCfg.Id);
                    allHeroes.Remove(heroCfg);
                    tempSide4Heroes.Remove(heroCfg);
                    sideCounts[side - 1]++;
                }
            }
        }

        // side 1/2/3 全部加入英雄池，不做随机筛选
        List<List<HeroConfig>> sideHeroes = new List<List<HeroConfig>>
        {
            allHeroes.FindAll(hero => hero.Side == 1),
            allHeroes.FindAll(hero => hero.Side == 2),
            allHeroes.FindAll(hero => hero.Side == 3)
        };

        foreach (var sideHeroList in sideHeroes)
        {
            foreach (var hero in sideHeroList)
                heroIds.Add(hero.Id);
        }

        // 心仪卡牌必定进入英雄池
        if (Profile.Instance.cardLoves != null)
        {
            foreach (var loveId in Profile.Instance.cardLoves)
            {
                if (!heroIds.Contains(loveId) && HeroConfig.HasConfig(loveId))
                    heroIds.Add(loveId);
            }
        }
    }
}
