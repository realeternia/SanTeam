using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 批量刷 Prefabs/Effect 与 Prefabs/Missile 下所有 prefab：把根节点与第一层（直接）子节点缩放统一改为 (1,1,1)。
/// 修改直接持久化到 .prefab 文件。菜单：Tools/刷特效导弹Prefab子节点缩放
/// </summary>
public static class EffectPrefabScaleFixer
{
    private const string MenuPath = "Tools/刷特效导弹Prefab子节点缩放";
    private const string EffectFolder = "Assets/Resources/Prefabs/Effect";
    private const string MissileFolder = "Assets/Resources/Prefabs/Missile";

    [MenuItem(MenuPath)]
    private static void FixAll()
    {
        int modified = 0;
        int scanned = 0;
        modified += FixFolder(EffectFolder, ref scanned);
        modified += FixFolder(MissileFolder, ref scanned);
        AssetDatabase.SaveAssets();
        GameLog.Info("EffectPrefabScaleFixer 完成，共扫描 " + scanned + " 个 prefab，修改 " + modified + " 个");
    }

    private static int FixFolder(string folderPath, ref int scanned)
    {
        if (!Directory.Exists(folderPath))
        {
            GameLog.Warn("EffectPrefabScaleFixer 目录不存在: " + folderPath);
            return 0;
        }

        int count = 0;
        foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { folderPath }))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (root == null)
            {
                GameLog.Warn("EffectPrefabScaleFixer 加载失败: " + path);
                continue;
            }
            scanned++;

            bool changed = false;
            // 根节点缩放归一
            if (root.transform.localScale != Vector3.one)
            {
                root.transform.localScale = Vector3.one;
                changed = true;
            }
            // 第一层（直接）子节点缩放归一
            for (int i = 0; i < root.transform.childCount; i++)
            {
                var child = root.transform.GetChild(i);
                if (child.localScale != Vector3.one)
                {
                    child.localScale = Vector3.one;
                    changed = true;
                }
            }

            if (changed)
            {
                PrefabUtility.SavePrefabAsset(root);
                count++;
                GameLog.Info("EffectPrefabScaleFixer 已修改: " + path);
            }
        }
        return count;
    }
}
