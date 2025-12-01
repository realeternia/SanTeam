using System.Collections.Generic;
using UnityEngine;
using System.IO;

// 用于序列化的数据类
[System.Serializable]
public class ProfileData
{
    public List<int> cardLoves = new List<int>();
}

public class Profile : MonoBehaviour
{
    public static Profile Instance;

    // 收藏的卡牌列表
    public List<int> cardLoves = new List<int>();
    
    void Start()
    {
        // 读取文本文件
        string content = LoadTextFile();
        UnityEngine.Debug.Log("Profile Start Load " + content);        
        if (content != "")
        {
            // 解析文本文件到数据类
            ProfileData data = JsonUtility.FromJson<ProfileData>(content);
            // 赋值
            cardLoves = data.cardLoves;
        }
        Instance = this;
    }

    void OnApplicationQuit()
    {
        SaveTextFile();
    }


    // 保存文本文件
    public void SaveTextFile()
    {
        // 创建数据类实例并赋值
        ProfileData data = new ProfileData();
        data.cardLoves = cardLoves;
        
        // 转换为文本文件
        string content = JsonUtility.ToJson(data);
        string path = Path.Combine(Application.persistentDataPath, "profile.txt");
        File.WriteAllText(path, content);
    }

    // 读取文本文件
    public string LoadTextFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "profile.txt");
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        return "";
    }
}