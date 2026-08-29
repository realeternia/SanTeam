using System.Collections.Generic;
using CommonConfig;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IconLoader))]
public class IconLoaderEditor : Editor
{
    private int heroAttrIndex = -1;
    private int sysAttrIndex = -1;
    private List<int> heroAttrIds;
    private List<int> sysAttrIds;
    private string[] heroAttrDisplayNames;
    private string[] sysAttrDisplayNames;

    private static readonly Color HeaderBg = new Color(0.18f, 0.20f, 0.28f);
    private static readonly Color HeaderText = new Color(0.85f, 0.88f, 1.0f);
    private static readonly Color PathAccent = new Color(0.30f, 0.70f, 0.95f);
    private static readonly Color HeroAccent = new Color(0.75f, 0.30f, 0.90f);
    private static readonly Color SysAccent = new Color(0.20f, 0.80f, 0.55f);
    private static readonly Color IdBadgeBg = new Color(0.22f, 0.24f, 0.32f);
    private static readonly Color IdBadgeText = new Color(0.60f, 0.85f, 1.0f);
    private static readonly Color SectionBg = new Color(0.24f, 0.26f, 0.34f, 0.6f);
    private static readonly Color LimeLabel = new Color(0.55f, 1.0f, 0.35f);

    private GUIStyle headerStyle;
    private GUIStyle sectionLabelStyle;
    private GUIStyle idBadgeStyle;
    private GUIStyle limeLabelStyle;
    private bool stylesInitialized;

    private void InitStyles()
    {
        if (stylesInitialized) return;
        stylesInitialized = true;

        headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = HeaderText },
            padding = new RectOffset(0, 0, 8, 8)
        };

        sectionLabelStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = Color.white },
            padding = new RectOffset(8, 0, 4, 4)
        };

        idBadgeStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            fontSize = 11,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = IdBadgeText },
            padding = new RectOffset(8, 8, 3, 3)
        };

        limeLabelStyle = new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = LimeLabel },
            hover = { textColor = LimeLabel },
            active = { textColor = LimeLabel },
            focused = { textColor = LimeLabel }
        };
    }

    private void RefreshAttrData()
    {
        heroAttrIds = null;
        heroAttrDisplayNames = null;
        heroAttrIndex = -1;
        sysAttrIds = null;
        sysAttrDisplayNames = null;
        sysAttrIndex = -1;
    }

    private void EnsureHeroAttrData()
    {
        if (heroAttrIds != null) return;
        HeroAttrConfig.Load();
        heroAttrIds = new List<int>();
        List<string> names = new List<string>();
        foreach (HeroAttrConfig cfg in HeroAttrConfig.ConfigList)
        {
            if (string.IsNullOrEmpty(cfg.Icon)) continue;
            heroAttrIds.Add(cfg.Id);
            names.Add(string.Format("{0}  [{1}]", cfg.Cname, cfg.Icon));
        }
        heroAttrDisplayNames = names.ToArray();
    }

    private void EnsureSysAttrData()
    {
        if (sysAttrIds != null) return;
        SystemAttrConfig.Load();
        sysAttrIds = new List<int>();
        List<string> names = new List<string>();
        foreach (SystemAttrConfig cfg in SystemAttrConfig.ConfigList)
        {
            if (string.IsNullOrEmpty(cfg.Icon)) continue;
            sysAttrIds.Add(cfg.Id);
            names.Add(string.Format("{0}  [{1}]", cfg.Cname, cfg.Icon));
        }
        sysAttrDisplayNames = names.ToArray();
    }

    private int FindIndex(List<int> ids, int targetId)
    {
        for (int i = 0; i < ids.Count; i++)
        {
            if (ids[i] == targetId) return i;
        }
        return -1;
    }

    private new void DrawHeader()
    {
        Rect rect = GUILayoutUtility.GetRect(0, 36, headerStyle);
        EditorGUI.DrawRect(rect, HeaderBg);
        Rect lineRect = new Rect(rect.x, rect.yMax - 2, rect.width, 2);
        EditorGUI.DrawRect(lineRect, PathAccent);
        GUI.Label(rect, "\u25C6  ICON LOADER", headerStyle);
    }

    private void DrawSectionHeader(string text, Color accent)
    {
        GUILayout.Space(6);
        Rect rect = GUILayoutUtility.GetRect(0, 24, sectionLabelStyle);
        EditorGUI.DrawRect(rect, SectionBg);
        Rect accentRect = new Rect(rect.x, rect.y, 3, rect.height);
        EditorGUI.DrawRect(accentRect, accent);
        GUI.Label(rect, "  " + text, sectionLabelStyle);
        GUILayout.Space(2);
    }

    private void DrawIdBadge(int id)
    {
        GUILayout.Space(4);
        Rect bgRect = GUILayoutUtility.GetRect(0, 22, idBadgeStyle);
        bgRect.x += EditorGUIUtility.labelWidth;
        bgRect.width = 80;
        EditorGUI.DrawRect(bgRect, IdBadgeBg);
        Rect borderRect = new Rect(bgRect.x, bgRect.y, bgRect.width, 1);
        EditorGUI.DrawRect(borderRect, IdBadgeText * 0.4f);
        GUI.Label(bgRect, "ID: " + id, idBadgeStyle);
    }

    public override void OnInspectorGUI()
    {
        InitStyles();
        serializedObject.Update();

        IconLoader loader = (IconLoader)target;

        DrawHeader();

        GUILayout.Space(2);
        Rect refreshRect = GUILayoutUtility.GetRect(0, 22);
        refreshRect.x += EditorGUIUtility.labelWidth;
        refreshRect.width -= EditorGUIUtility.labelWidth;
        if (GUI.Button(refreshRect, "\u21BB  \u5237\u65B0\u914D\u7F6E"))
        {
            RefreshAttrData();
        }

        SerializedProperty sourceTypeProp = serializedObject.FindProperty("sourceType");

        GUILayout.Space(4);
        Rect typeRect = GUILayoutUtility.GetRect(0, 20);
        Rect typeLabelRect = new Rect(typeRect.x, typeRect.y, EditorGUIUtility.labelWidth, typeRect.height);
        Rect typeFieldRect = new Rect(typeRect.x + EditorGUIUtility.labelWidth, typeRect.y, typeRect.width - EditorGUIUtility.labelWidth, typeRect.height);
        GUI.Label(typeLabelRect, "Source Type", limeLabelStyle);
        sourceTypeProp.enumValueIndex = EditorGUI.Popup(typeFieldRect, sourceTypeProp.enumValueIndex, sourceTypeProp.enumDisplayNames);

        IconSourceType currentType = (IconSourceType)sourceTypeProp.enumValueIndex;

        switch (currentType)
        {
            case IconSourceType.Path:
            {
                DrawSectionHeader("\u25B8 \u8DEF\u5F84\u6A21\u5F0F", PathAccent);
                SerializedProperty iconPathProp = serializedObject.FindProperty("iconPath");
                Rect pathRect = GUILayoutUtility.GetRect(0, 20);
                Rect pathLabelRect = new Rect(pathRect.x, pathRect.y, EditorGUIUtility.labelWidth, pathRect.height);
                Rect pathFieldRect = new Rect(pathRect.x + EditorGUIUtility.labelWidth, pathRect.y, pathRect.width - EditorGUIUtility.labelWidth, pathRect.height);
                GUI.Label(pathLabelRect, iconPathProp.displayName, limeLabelStyle);
                EditorGUI.PropertyField(pathFieldRect, iconPathProp, GUIContent.none);
                break;
            }
            case IconSourceType.HeroAttr:
            {
                DrawSectionHeader("\u25B8 \u82F1\u96C4\u5C5E\u6027", HeroAccent);
                EnsureHeroAttrData();
                if (heroAttrIds.Count == 0)
                {
                    EditorGUILayout.HelpBox("HeroAttrConfig \u65E0\u53EF\u7528\u6761\u76EE", MessageType.Warning);
                    break;
                }
                if (heroAttrIndex < 0)
                {
                    heroAttrIndex = FindIndex(heroAttrIds, loader.configId);
                }
                Rect popRect = GUILayoutUtility.GetRect(0, 20);
                Rect popLabelRect = new Rect(popRect.x, popRect.y, EditorGUIUtility.labelWidth, popRect.height);
                Rect popFieldRect = new Rect(popRect.x + EditorGUIUtility.labelWidth, popRect.y, popRect.width - EditorGUIUtility.labelWidth, popRect.height);
                GUI.Label(popLabelRect, "\u5C5E\u6027\u9009\u62E9", limeLabelStyle);
                int newIndex = EditorGUI.Popup(popFieldRect, heroAttrIndex, heroAttrDisplayNames);
                if (newIndex != heroAttrIndex && newIndex >= 0)
                {
                    heroAttrIndex = newIndex;
                    loader.configId = heroAttrIds[newIndex];
                    EditorUtility.SetDirty(loader);
                }
                if (heroAttrIndex >= 0 && heroAttrIndex < heroAttrDisplayNames.Length)
                {
                    DrawIdBadge(loader.configId);
                }
                break;
            }
            case IconSourceType.SysAttr:
            {
                DrawSectionHeader("\u25B8 \u7CFB\u7EDF\u5C5E\u6027", SysAccent);
                EnsureSysAttrData();
                if (sysAttrIds.Count == 0)
                {
                    EditorGUILayout.HelpBox("SystemAttrConfig \u65E0\u53EF\u7528\u6761\u76EE", MessageType.Warning);
                    break;
                }
                if (sysAttrIndex < 0)
                {
                    sysAttrIndex = FindIndex(sysAttrIds, loader.configId);
                }
                Rect sysPopRect = GUILayoutUtility.GetRect(0, 20);
                Rect sysPopLabelRect = new Rect(sysPopRect.x, sysPopRect.y, EditorGUIUtility.labelWidth, sysPopRect.height);
                Rect sysPopFieldRect = new Rect(sysPopRect.x + EditorGUIUtility.labelWidth, sysPopRect.y, sysPopRect.width - EditorGUIUtility.labelWidth, sysPopRect.height);
                GUI.Label(sysPopLabelRect, "\u5C5E\u6027\u9009\u62E9", limeLabelStyle);
                int sysNewIndex = EditorGUI.Popup(sysPopFieldRect, sysAttrIndex, sysAttrDisplayNames);
                if (sysNewIndex != sysAttrIndex && sysNewIndex >= 0)
                {
                    sysAttrIndex = sysNewIndex;
                    loader.configId = sysAttrIds[sysNewIndex];
                    EditorUtility.SetDirty(loader);
                }
                if (sysAttrIndex >= 0 && sysAttrIndex < sysAttrDisplayNames.Length)
                {
                    DrawIdBadge(loader.configId);
                }
                break;
            }
        }

        GUILayout.Space(4);
        serializedObject.ApplyModifiedProperties();
    }
}
