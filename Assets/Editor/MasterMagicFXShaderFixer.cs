using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 把 MasterMagicFX 下引用了 ShaderGraph（工程未安装 com.unity.shadergraph 包）的材质，
/// 重新指向内置管线等价 Shader（MasterMagicFX/UberParticles、MasterMagicFX/CustomShader）。
/// 材质属性值原样保留，只替换 m_Shader 引用。
/// 首次打开工程时自动执行一次；也可手动执行菜单：Tools/MasterMagicFX/修复丢失的内置管线 Shader
/// </summary>
public static class MasterMagicFXShaderFixer
{
    private const string MenuPath = "Tools/MasterMagicFX/修复丢失的内置管线 Shader";
    private const string RootFolder = "Assets/MasterMagicFX";

    // 原 ShaderGraph 资源 guid（材质里 m_Shader 引用丢失的正是它们）
    private const string UberParticlesGraphGuid = "b85d6def27a1d1041b93e3045d7d6c0f";
    private const string CustomShaderGraphGuid = "ba4c9d58a4c51c34ba58f76d9963e1a5";

    private const string UberParticlesShaderName = "MasterMagicFX/UberParticles";
    private const string CustomShaderName = "MasterMagicFX/CustomShader";

    private const string DoneKey = "SanTeam.MasterMagicFX.ShaderFix.v1";

    [InitializeOnLoadMethod]
    private static void AutoFixOnLoad()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorPrefs.GetBool(DoneKey, false))
            {
                return;
            }

            if (FixAll())
            {
                EditorPrefs.SetBool(DoneKey, true);
            }
        };
    }

    [MenuItem(MenuPath)]
    private static void FixAllFromMenu()
    {
        if (FixAll())
        {
            EditorPrefs.SetBool(DoneKey, true);
        }
    }

    private static bool FixAll()
    {
        Shader uberParticles = Shader.Find(UberParticlesShaderName);
        Shader customShader = Shader.Find(CustomShaderName);
        if (uberParticles == null || customShader == null)
        {
            GameLog.Warn("MasterMagicFXShaderFixer 未找到替换 Shader："
                         + UberParticlesShaderName + " / " + CustomShaderName + "，稍后重试");
            return false;
        }

        int scanned = 0;
        int fixedCount = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { RootFolder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.EndsWith(".mat"))
            {
                continue;
            }
            scanned++;

            string text;
            try
            {
                text = File.ReadAllText(path);
            }
            catch (IOException e)
            {
                GameLog.Warn("MasterMagicFXShaderFixer 读取材质失败: " + path + " - " + e.Message);
                continue;
            }

            Shader target = null;
            if (text.Contains(UberParticlesGraphGuid))
            {
                target = uberParticles;
            }
            else if (text.Contains(CustomShaderGraphGuid))
            {
                target = customShader;
            }

            if (target == null)
            {
                continue;
            }

            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                GameLog.Warn("MasterMagicFXShaderFixer 加载材质失败: " + path);
                continue;
            }

            if (material.shader == target)
            {
                continue;
            }

            material.shader = target;
            EditorUtility.SetDirty(material);
            fixedCount++;
        }

        if (fixedCount > 0)
        {
            AssetDatabase.SaveAssets();
        }

        GameLog.Info("MasterMagicFXShaderFixer 完成，扫描 " + scanned + " 个材质，修复 " + fixedCount + " 个");
        return true;
    }
}
