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

    // 存档摘要解析用：镜像 PlayerInfo 序列化结构 { playerData:[{key,value}] }
    [System.Serializable]
    private class RawPlayerSave
    {
        public List<RawSavePair> playerData = new List<RawSavePair>();
    }

    [System.Serializable]
    private class RawSavePair
    {
        public string key;
        public string value;
    }

    // 存档摘要：存档列表展示用（年份/积分/金钱/上阵英雄）
    [System.Serializable]
    public struct SaveSummary
    {
        public int year;
        public int gold;
        public int mark;
        public List<int> lineHeroes;
    }

    /// <summary>最大存档槽位数</summary>
    public const int SaveSlotCount = 5;

    public static GameManager Instance;
    public PlayerInfo[] players; //不能new，都是配置好的
    public List<FriendRandomData> friendRdData;
    public List<int> heroIds;
    public int year;
    /// <summary>当前存档槽位（-1 表示尚未选择/新建）</summary>
    public int currentSaveSlot = -1;

    // 调试阵容：配置任一方武将后，进入游戏直接开战（跳过选牌/商店流程），列表留空则走正常对局流程。
    // 仅用于开发调试，通过菜单 Tools/调试阵容配置窗口 配置（见 Assets/Editor/DebugLineupWindow.cs），配置后可一键进入战斗。
    [HideInInspector] public List<int> debugHeroesSide1 = new List<int>(); // 左侧(1号位)武将ID，按顺序摆入布阵格
    [HideInInspector] public List<int> debugHeroesSide2 = new List<int>(); // 右侧(2号位)武将ID，按顺序摆入布阵格

    /// <summary>是否配置了调试阵容（任一方填入武将即视为调试开局）</summary>
    public bool HasDebugLineup()
    {
        return (debugHeroesSide1 != null && debugHeroesSide1.Count > 0)
            || (debugHeroesSide2 != null && debugHeroesSide2.Count > 0);
    }

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        ConfigManager.Init();

        MigrateLegacySave();

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

    // 存档文件路径（槽位 0 ~ SaveSlotCount-1）
    private string GetSavePath(int slot)
    {
        return Application.persistentDataPath + "/game_save_" + slot + ".json";
    }

    // 旧版单存档迁移：game_save.json → 槽位0（仅当槽位0不存在时）
    private void MigrateLegacySave()
    {
        string legacyPath = Application.persistentDataPath + "/game_save.json";
        if (File.Exists(legacyPath) && !File.Exists(GetSavePath(0)))
        {
            try
            {
                File.Move(legacyPath, GetSavePath(0));
                GameLog.Debug("旧版单存档已迁移到槽位0");
            }
            catch (System.Exception e)
            {
                GameLog.Error("旧版存档迁移失败: " + e.Message);
            }
        }
    }

    // 已使用的存档槽位（升序）
    public List<int> GetUsedSaveSlots()
    {
        var slots = new List<int>();
        for (int i = 0; i < SaveSlotCount; i++)
        {
            if (File.Exists(GetSavePath(i)))
                slots.Add(i);
        }
        return slots;
    }

    public bool IsGameSaveExist()
    {
        return GetUsedSaveSlots().Count > 0;
    }

    // 分配一个新的空存档槽位（取第一个未占用槽位），成功返回槽位号，槽位已满返回-1
    public int CreateNewSaveSlot()
    {
        for (int i = 0; i < SaveSlotCount; i++)
        {
            if (!File.Exists(GetSavePath(i)))
            {
                currentSaveSlot = i;
                GameLog.Debug("分配新存档槽位: " + i);
                return i;
            }
        }
        GameLog.Warn("存档槽位已满，无法创建新存档");
        return -1;
    }

    // 读取存档摘要（年份/积分/金钱/上阵英雄），供存档列表展示。
    // 从 0 号位(人类玩家)的序列化数据解析，兼容无独立摘要字段的旧存档。
    public SaveSummary GetSaveSummary(int slot)
    {
        var summary = new SaveSummary { lineHeroes = new List<int>() };
        string savePath = GetSavePath(slot);
        if (!File.Exists(savePath))
        {
            GameLog.Warn("读取存档摘要失败，文件不存在: " + savePath);
            return summary;
        }
        try
        {
            string json = File.ReadAllText(savePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            summary.year = saveData.year;

            if (saveData.players != null && saveData.players.Count > 0)
            {
                RawPlayerSave raw = JsonUtility.FromJson<RawPlayerSave>(saveData.players[0]);
                if (raw != null && raw.playerData != null)
                {
                    foreach (var pair in raw.playerData)
                    {
                        if (pair.key == "gold")
                            int.TryParse(pair.value, out summary.gold);
                        else if (pair.key == "mark")
                            int.TryParse(pair.value, out summary.mark);
                        else if (pair.key == "battleCards" && !string.IsNullOrEmpty(pair.value))
                        {
                            foreach (string s in pair.value.Split(','))
                            {
                                if (int.TryParse(s, out int cardId) && cardId > 0 && ConfigManager.IsHeroCard(cardId))
                                    summary.lineHeroes.Add(cardId);
                            }
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            GameLog.Error("读取存档摘要失败: " + e.Message);
        }
        return summary;
    }

    public bool LoadFromSave(int slot)
    {
        string savePath = GetSavePath(slot);
        if (!File.Exists(savePath))
        {
            GameLog.Warn("加载存档失败，文件不存在: " + savePath);
            return false;
        }
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

            currentSaveSlot = slot;
            GameLog.Debug("游戏数据加载成功 slot=" + slot + " year=" + year);
        }
        catch (System.Exception e)
        {
            GameLog.Error("加载游戏数据失败: " + e.Message);
            return false;
        }
        return true;
    }

    // 删除指定槽位存档
    public void DeleteSave(int slot)
    {
        string savePath = GetSavePath(slot);
        if (!File.Exists(savePath))
        {
            GameLog.Warn("删除存档失败，文件不存在: " + savePath);
            return;
        }
        try
        {
            File.Delete(savePath);
            if (currentSaveSlot == slot)
                currentSaveSlot = -1;
            GameLog.Debug("删除存档成功 slot=" + slot);
        }
        catch (System.Exception e)
        {
            GameLog.Error("删除存档失败: " + e.Message);
        }
    }

    public void SaveToFile()
    {
        // 未指定槽位（如调试阵容跳过选牌流程）时兜底写入槽位0
        int slot = currentSaveSlot >= 0 ? currentSaveSlot : 0;
        if (currentSaveSlot < 0)
            GameLog.Warn("当前未选择存档槽位，默认写入槽位0");

        string savePath = GetSavePath(slot);
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

        // 核心英雄（各势力主公 王）始终进入英雄池
        List<HeroConfig> tempHeroes = new List<HeroConfig>(allHeroes);
        foreach (var hero in tempHeroes)
        {
            if (ConfigManager.IsKingHero(hero.Id))
            {
                heroIds.Add(hero.Id);
                allHeroes.Remove(hero);
            }
        }

        // 魏蜀吴之外的4个阵营(4晋/5群/6神/10野)4选2，选中阵营的全部英雄进英雄池
        int[] sides = {4, 5, 6, 10};
        int[] pickedSides = new int[2];
        for (int i = 0; i < 2; i++)
        {
            var side = sides[SysRandom.Range(0, sides.Length)];
            sides = sides.Where(s => s != side).ToArray();
            pickedSides[i] = side;
        }
        foreach (var side in pickedSides)
        {
            foreach (var hero in allHeroes.FindAll(h => h.Side == side))
                heroIds.Add(hero.Id);
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
    }
}
