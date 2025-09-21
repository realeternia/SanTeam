using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerInfo[] players;
    private StreamWriter logWriter;  // 日志写入器

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
        
        // 注册日志事件
        Application.logMessageReceived += LogMessageReceived;

        var p1 = PlayerBook.GetWang();
        players[0].Init(0, p1.Name, p1.Imgpath, p1.Colorstr, 0);
        players[0].playerConfig = p1;
        var pls = PlayerBook.GetRandomN(5);
        for (int i = 0; i < 5; i++)
        {
            players[i + 1].Init(i + 1, pls[i].Name, pls[i].Imgpath, pls[i].Colorstr, 0);
            players[i + 1].playerConfig = pls[i];
        }
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
        return File.Exists(savePath);
    }

    public void LoadFromSave()
    {
        string savePath = Application.persistentDataPath + "/game_save.json";
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                
                // 确保players数组不为null且长度足够
                if (players != null && players.Length >= saveData.players.Count)
                {
                    for (int i = 0; i < saveData.players.Count; i++)
                    {
                        // 直接使用PlayerInfo的Deserialize方法
                        players[i].Deserialize(saveData.players[i]);
                    }
                    
                    Debug.Log("游戏数据加载成功");
                }
                else
                {
                    Debug.LogError("players数组未初始化或长度不足");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("加载游戏数据失败: " + e.Message);
            }
        }
        else
        {
            Debug.Log("没有找到存档文件");
        }
    }

    public void SaveToFile()
    {
        string savePath = Application.persistentDataPath + "/game_save.json";
        try
        {
            SaveData saveData = new SaveData();
            
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
    
    // 用于Unity JsonUtility序列化的辅助类
    [System.Serializable]
    private class SaveData
    {
        public List<string> players = new List<string>();
    }
}
