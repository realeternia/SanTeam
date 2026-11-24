using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

/// <summary>
/// Project 窗口选中 prefab 的右键菜单扩展：
/// 「添加到Effect」把 prefab 拷贝到 Prefabs/Effect，并在 EffectConfig 的 11090 之后插入特效配置行；
/// 「添加到Missile」把 prefab 拷贝到 Prefabs/Missile，并在 EffectConfig 的 10140 之后插入导弹配置行；
/// 新 Id 在对应段位内按 10 递增（特效 11xxx、导弹 10xxx），Name 取 prefab 文件名。
/// </summary>
public static class EffectPrefabContextMenu
{
    private const string EffectMenu = "Assets/添加到Effect";
    private const string MissileMenu = "Assets/添加到Missile";
    private const string EffectFolder = "Assets/Resources/Prefabs/Effect";
    private const string MissileFolder = "Assets/Resources/Prefabs/Missile";
    private const string EffectConfigPath = "Assets/Resources/Scripts/Configs/EffectConfig_s.cs";

    /// <summary>导弹段位（10xxx）</summary>
    private const int MissileIdMin = 10000;
    private const int MissileIdMax = 10999;
    /// <summary>特效段位（11xxx）</summary>
    private const int EffectIdMin = 11000;
    private const int EffectIdMax = 11999;
    /// <summary>兜底锚点：特效接 11090，导弹接 10140（段内无数据行时使用）</summary>
    private const int EffectFallbackAnchorId = 11090;
    private const int MissileFallbackAnchorId = 10140;
    /// <summary>新 Id 递增步长</summary>
    private const int IdStep = 10;

    // 匹配 Load() 中的数据行：config[10000] = new EffectConfig(10000, "Name", ...
    private static readonly Regex RowRegex = new Regex(
        @"^\s*config\[(?<id>\d+)\]\s*=\s*new EffectConfig\(\s*\d+\s*,\s*""(?<name>[^""]*)""",
        RegexOptions.Compiled);

    private sealed class RowInfo
    {
        public int Id;
        public string Name;
    }

    [MenuItem(EffectMenu, false, 2000)]
    private static void AddToEffect()
    {
        AddToProject(true);
    }

    [MenuItem(EffectMenu, true)]
    private static bool ValidateAddToEffect()
    {
        return GetSelectedPrefabPath() != null;
    }

    [MenuItem(MissileMenu, false, 2001)]
    private static void AddToMissile()
    {
        AddToProject(false);
    }

    [MenuItem(MissileMenu, true)]
    private static bool ValidateAddToMissile()
    {
        return GetSelectedPrefabPath() != null;
    }

    /// <summary>当前选中的 prefab 资源路径，非 prefab 返回 null</summary>
    private static string GetSelectedPrefabPath()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path) || !path.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase))
            return null;
        return path;
    }

    private static void AddToProject(bool isEffect)
    {
        string sourcePath = GetSelectedPrefabPath();
        if (string.IsNullOrEmpty(sourcePath))
        {
            GameLog.Warn("EffectPrefabContextMenu 当前选中的不是 prefab");
            return;
        }

        string prefabName = Path.GetFileNameWithoutExtension(sourcePath);
        string targetFolder = isEffect ? EffectFolder : MissileFolder;
        string destPath = targetFolder + "/" + prefabName + ".prefab";

        if (!AssetDatabase.IsValidFolder(targetFolder))
        {
            GameLog.Error("EffectPrefabContextMenu 目录不存在: " + targetFolder);
            EditorUtility.DisplayDialog("添加失败", "目录不存在：" + targetFolder, "确定");
            return;
        }

        List<RowInfo> rows = ParseRows(EffectConfigPath);
        if (rows == null)
        {
            GameLog.Error("EffectPrefabContextMenu 配置表文件不存在: " + EffectConfigPath);
            EditorUtility.DisplayDialog("添加失败", "配置表文件不存在：" + EffectConfigPath, "确定");
            return;
        }
        if (rows.Exists(r => r.Name == prefabName))
        {
            GameLog.Warn("EffectPrefabContextMenu EffectConfig 已存在同名配置: " + prefabName);
            EditorUtility.DisplayDialog("添加失败", "EffectConfig 已存在 Name 为 “" + prefabName + "” 的配置，请先重命名 prefab。", "确定");
            return;
        }

        // 目标目录已有同名 prefab 时直接提示并终止，不覆盖、不写配置
        if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(destPath) != null)
        {
            GameLog.Warn("EffectPrefabContextMenu 目标已存在同名文件，未做任何操作: " + destPath);
            EditorUtility.DisplayDialog("目标已存在", "目标目录已存在同名文件：\n" + destPath + "\n未做任何操作。", "确定");
            return;
        }

        if (!AssetDatabase.CopyAsset(sourcePath, destPath))
        {
            GameLog.Error("EffectPrefabContextMenu 拷贝 prefab 失败: " + sourcePath + " -> " + destPath);
            EditorUtility.DisplayDialog("添加失败", "拷贝 prefab 失败，详见控制台日志。", "确定");
            return;
        }
        AssetDatabase.ImportAsset(destPath, ImportAssetOptions.ForceSynchronousImport);

        int newId = InsertConfigRow(EffectConfigPath, isEffect, prefabName, rows);
        if (newId < 0)
        {
            EditorUtility.DisplayDialog("添加失败", "写入 EffectConfig 失败，详见控制台日志。", "确定");
            return;
        }

        AssetDatabase.SaveAssets();
        GameLog.Info("EffectPrefabContextMenu 已添加" + (isEffect ? "特效" : "导弹") + ": " + prefabName
                     + " -> " + destPath + "，EffectConfig Id=" + newId);
    }

    /// <summary>解析 EffectConfig_s.cs 中 Load() 的全部数据行，文件不存在返回 null</summary>
    private static List<RowInfo> ParseRows(string configPath)
    {
        if (!File.Exists(configPath))
            return null;

        var rows = new List<RowInfo>();
        foreach (string line in File.ReadAllLines(configPath))
        {
            Match m = RowRegex.Match(line);
            if (!m.Success)
                continue;

            int id;
            if (!int.TryParse(m.Groups["id"].Value, out id))
                continue;

            rows.Add(new RowInfo { Id = id, Name = m.Groups["name"].Value });
        }
        return rows;
    }

    /// <summary>
    /// 在 Load() 中段位最大 Id 行之后插入一行配置，返回新 Id；失败返回 -1。
    /// 特效段为 11xxx（初始锚点 11090），导弹段为 10xxx（初始锚点 10140）。
    /// </summary>
    private static int InsertConfigRow(string configPath, bool isEffect, string prefabName, List<RowInfo> rows)
    {
        int minId = isEffect ? EffectIdMin : MissileIdMin;
        int maxId = isEffect ? EffectIdMax : MissileIdMax;

        int anchorId = -1;
        foreach (RowInfo row in rows)
        {
            if (row.Id >= minId && row.Id <= maxId && row.Id > anchorId)
                anchorId = row.Id;
        }
        if (anchorId < 0)
        {
            anchorId = isEffect ? EffectFallbackAnchorId : MissileFallbackAnchorId;
            GameLog.Warn("EffectPrefabContextMenu 段内无配置行，使用兜底锚点 Id=" + anchorId);
        }

        int newId = anchorId + IdStep;
        while (rows.Exists(r => r.Id == newId))
            newId += IdStep;

        byte[] raw = File.ReadAllBytes(configPath);
        bool hasBom = raw.Length >= 3 && raw[0] == 0xEF && raw[1] == 0xBB && raw[2] == 0xBF;
        string text = File.ReadAllText(configPath);
        string[] lines = text.Split('\n');

        int anchorIndex = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            Match m = RowRegex.Match(lines[i]);
            if (!m.Success)
                continue;

            int id;
            if (int.TryParse(m.Groups["id"].Value, out id) && id == anchorId)
            {
                anchorIndex = i;
                break;
            }
        }
        if (anchorIndex < 0)
        {
            GameLog.Error("EffectPrefabContextMenu 未找到锚点配置行 Id=" + anchorId);
            return -1;
        }

        string anchorLine = lines[anchorIndex];
        string lineEnding = anchorLine.EndsWith("\r") ? "\r" : "";
        string indent = Regex.Match(anchorLine, @"^\s*").Value;
        string nameLiteral = Escape(prefabName);

        string rowText = isEffect
            ? indent + "config[" + newId + "] = new EffectConfig(" + newId + ", \"" + nameLiteral + "\", \"" + nameLiteral
              + "\", \"伤害\", \"目标\", true, \"Effect/" + nameLiteral + "\", \"\", 1f, 1f, 1f, 1f, \"\");" + lineEnding
            : indent + "config[" + newId + "] = new EffectConfig(" + newId + ", \"" + nameLiteral + "\", \"" + nameLiteral
              + "\", \"伤害\", \"目标\", true, \"\", \"Missile/" + nameLiteral + "\", 1f, 1f, 1f, 1f, \"\");" + lineEnding;

        var newLines = new List<string>(lines.Length + 1);
        newLines.AddRange(lines);
        newLines.Insert(anchorIndex + 1, rowText);

        File.WriteAllText(configPath, string.Join("\n", newLines), new UTF8Encoding(hasBom));
        AssetDatabase.ImportAsset(configPath, ImportAssetOptions.ForceUpdate);
        return newId;
    }

    /// <summary>转义写入 C# 字符串字面量的反斜杠与双引号</summary>
    private static string Escape(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
