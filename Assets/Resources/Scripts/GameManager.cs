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
    private StreamWriter logWriter;  // 日志写入器
    public int year;

    private int tempFriendIdIdx = 1000;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        // 初始化日志文件
        string logPath = Application.persistentDataPath + "/game_log.txt";
        logWriter = new StreamWriter(logPath, false, System.Text.Encoding.UTF8); 
        logWriter.WriteLine("Game started at: " + System.DateTime.Now);

        ConfigManager.Init();
        
        // 注册日志事件
        Application.logMessageReceived += LogMessageReceived;

        players[0].Init(0, PlayerBook.GetWang());
        var pls = PlayerBook.GetRandomN(7);
        for (int i = 0; i < 7; i++)
            players[i + 1].Init(i + 1, pls[i]);

        UnityEngine.Debug.Log("GameManager Start");
    }

    // 日志处理函数
    private void LogMessageReceived(string logString, string stackTrace, LogType type)
    {
        if (logWriter != null)
        {
            if(logString.Contains("font asset"))
                return;
            string logType = type.ToString();
            logWriter.WriteLine($"[{System.DateTime.Now}] [{logType}] {logString}");
            if (!string.IsNullOrEmpty(stackTrace))
            {
                logWriter.WriteLine($"Stack Trace: {stackTrace}");
            }
            logWriter.Flush();  // 立即写入文件
        }
    }

    private void OnDestroy()
    {
        // 取消注册日志事件
        Application.logMessageReceived -= LogMessageReceived;
        
        // 关闭日志文件
        if (logWriter != null)
        {
            logWriter.WriteLine("Game ended at: " + System.DateTime.Now);
            logWriter.Close();
            logWriter = null;
        }
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

            Debug.Log("游戏数据加载成功 year=" + year);
        }
        catch (System.Exception e)
        {
            Debug.LogError("加载游戏数据失败: " + e.Message);
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
            
            Debug.Log("游戏数据保存成功: " + savePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("保存游戏数据失败: " + e.Message);
        }
    }

    public void InitFriend(bool loadSave)
    {
        if (!loadSave)
        {
            friendRdData = new List<FriendRandomData>();

            for (int i = 0; i < 6; i++)
            {
                // 筛选side=i且FriendCount<4的英雄
                var heroList2 = HeroConfig.ConfigList.Where(x => x.Side == i && heroIds.Contains(x.Id) && x.FriendCount < 4).ToList();

                // 智力分组
                var inteHighList = heroList2.Where(x => x.Inte > 80).ToList();
                var inteLowList = heroList2.Where(x => x.Inte < 60).ToList();

                // 随机2v2匹配
                CreateRandomFriendPairs(inteHighList, inteLowList, "智力辅佐");

                // 武力分组和匹配
                var strHighList = heroList2.Where(x => x.Str > 80).ToList();
                var strLowList = heroList2.Where(x => x.Str < 60).ToList();

                CreateRandomFriendPairs(strHighList, strLowList, "武力指导");

                heroList2 = HeroConfig.ConfigList.Where(x => x.Side == i && heroIds.Contains(x.Id) && x.FriendCount < 2).ToList();
                if (heroList2.Count > 2)
                {
                    // 随机打乱heroList，然后分成前一半和后一半
                    var random = new System.Random();
                    var shuffledList = heroList2.OrderBy(x => random.Next()).ToList();
                    int halfCount = shuffledList.Count / 2;
                    var firstHalf = shuffledList.Take(halfCount).ToList();
                    var secondHalf = shuffledList.Skip(halfCount).ToList();

                    CreateRandomFriendPairs(firstHalf, secondHalf, "协作如坚");
                }
            }

            var heroList = HeroConfig.ConfigList.Where(x => heroIds.Contains(x.Id) && x.FriendCount < 1).ToList();
            if (heroList.Count > 2)
            {
                // 随机打乱heroList，然后分成前一半和后一半
                var random = new System.Random();
                var shuffledList = heroList.OrderBy(x => random.Next()).ToList();
                int halfCount = shuffledList.Count / 2;
                var firstHalf = shuffledList.Take(halfCount).ToList();
                var secondHalf = shuffledList.Skip(halfCount).ToList();

                CreateRandomFriendPairs(firstHalf, secondHalf, "儿时好友");
            }
        }

        ConfigManager.InitFriend();
    }

    private void CreateRandomFriendPairs(List<HeroConfig> inteHighList, List<HeroConfig> inteLowList, string name)
    {
        if (inteHighList.Count < 1 || inteLowList.Count < 1)
            return;

        var rnd = new System.Random();
        var shuffledHigh = inteHighList.OrderBy(x => rnd.Next()).ToList();
        var shuffledLow = inteLowList.OrderBy(x => rnd.Next()).ToList();

        // 循环配对直到消耗完所有英雄
        while (shuffledHigh.Count > 0 && shuffledLow.Count > 0)
        {
            // 每次取2个高智力和2个低智力
            var highPair = shuffledHigh.Take(UnityEngine.Random.Range(1, 3)).ToList();
            var lowPair = shuffledLow.Take(UnityEngine.Random.Range(1, 3)).ToList();
            
            var heroes = highPair.Concat(lowPair).Select(x => x.Id).ToArray();

            var friendPair = new FriendRandomData();
            friendPair.id = tempFriendIdIdx++;
            friendPair.name = name;
            friendPair.friendIds = heroes;
            friendRdData.Add(friendPair);

            // 移除已配对的英雄
            shuffledHigh.RemoveRange(0, highPair.Count);
            shuffledLow.RemoveRange(0, lowPair.Count);
        }
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
        // 先对allHeroes遍历，1-100随机，如果大于RateAbs，加入返回队列
        List<HeroConfig> tempHeroes = new List<HeroConfig>(allHeroes);
        foreach (var hero in tempHeroes)
        {
            if(hero.RateAbs <= 0)
                continue;
            int randomValue = UnityEngine.Random.Range(1, 101);
            if (randomValue <= hero.RateAbs)
            {
                heroIds.Add(hero.Id);
                sideCounts[hero.Side - 1]++;
            }
            allHeroes.Remove(hero);
        }

        // 先随机选择5-7张Side=4的卡牌
        int[] sides = {4, 5, 6, 10};
        for (int i = 0; i < 2; i++)
        {
            var side = sides[UnityEngine.Random.Range(0, sides.Length)];
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
                    // 计算当前阵营总权重
                    float totalRate = 0;
                    foreach (var hero in tempSide4Heroes)
                    {
                        totalRate += hero.RateWeight;
                    }

                    HeroConfig heroCfg = null;
                    if (totalRate > 0)
                    {
                        float randomValue = UnityEngine.Random.Range(0, totalRate);
                        float accumulatedRate = 0;
                        HeroConfig selectedHero = null;

                        foreach (var hero in tempSide4Heroes)
                        {
                            if (hero.RateWeight <= 0)
                                continue;
                            accumulatedRate += hero.RateWeight;
                            if (accumulatedRate >= randomValue)
                            {
                                selectedHero = hero;
                                break;
                            }
                        }
                        heroCfg = selectedHero;
                    }
                    else
                    {
                        // 如果总权重为0，随机选一张
                        int randomIndex = UnityEngine.Random.Range(0, tempSide4Heroes.Count);
                        heroCfg = tempSide4Heroes[randomIndex];
                    }
                    heroIds.Add((int)heroCfg.Id);
                    allHeroes.Remove(heroCfg);
                    tempSide4Heroes.Remove(heroCfg);
                    sideCounts[side - 1]++;
                }
            }
        }

        // 准备按阵营1-3选择卡牌，保证各阵营相差最多一张
        List<List<HeroConfig>> sideHeroes = new List<List<HeroConfig>>
        {
            allHeroes.FindAll(hero => hero.Side == 1),
            allHeroes.FindAll(hero => hero.Side == 2),
            allHeroes.FindAll(hero => hero.Side == 3)
        };

        int targetCount = Mathf.Min(91, allHeroes.Count);

        while (heroIds.Count < targetCount)
        {
            // 找出当前数量最少的阵营
            int minIndex = 0;
            for (int i = 1; i < 3; i++)
            {
                if (sideCounts[i] < sideCounts[minIndex])
                {
                    minIndex = i;
                }
            }

            // 从最少的阵营中按权重选择一张卡牌
            List<HeroConfig> currentSideHeroes = sideHeroes[minIndex];
            if (currentSideHeroes.Count > 0)
            {
                // 计算当前阵营总权重
                float totalRate = 0;
                foreach (var hero in currentSideHeroes)
                {
                    totalRate += hero.RateWeight;
                }

                if (totalRate > 0)
                {
                    float randomValue = UnityEngine.Random.Range(0, totalRate);
                    float accumulatedRate = 0;
                    HeroConfig selectedHero = null;

                    foreach (var hero in currentSideHeroes)
                    {
                        if (hero.RateWeight <= 0)
                            continue;
                        accumulatedRate += hero.RateWeight;
                        if (accumulatedRate >= randomValue)
                        {
                            selectedHero = hero;
                            break;
                        }
                    }

                    if (selectedHero != null)
                    {
                        heroIds.Add((int)selectedHero.Id);
                        allHeroes.Remove(selectedHero);
                        sideHeroes[minIndex].Remove(selectedHero);
                        sideCounts[minIndex]++;
                    }
                }
                else
                {
                    // 如果总权重为0，随机选一张
                    int randomIndex = UnityEngine.Random.Range(0, currentSideHeroes.Count);
                    heroIds.Add((int)currentSideHeroes[randomIndex].Id);
                    allHeroes.Remove(currentSideHeroes[randomIndex]);
                    sideHeroes[minIndex].RemoveAt(randomIndex);
                    sideCounts[minIndex]++;
                }
            }
            else
            {
                // 如果当前阵营没有卡牌了，跳过该阵营
                // 找到下一个还有卡牌的阵营
                for (int i = 0; i < 3; i++)
                {
                    if (sideHeroes[i].Count > 0)
                    {
                        minIndex = i;
                        break;
                    }
                }
            }
        }
    }
}
