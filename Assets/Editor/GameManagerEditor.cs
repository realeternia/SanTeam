using System.Collections.Generic;
using CommonConfig;
using UnityEditor;
using UnityEngine;

/// <summary>
/// GameManager 自定义 Inspector：在默认属性基础上，提供"调试阵容"可视化配置。
/// 配置任一方武将后，启动游戏将跳过选牌/商店直接开战；两方留空则走正常对局流程。
/// 数据存于 GameManager.debugHeroesSide1 / debugHeroesSide2（[HideInInspector]，不重复显示）。
/// </summary>
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private class HeroItem
    {
        public int Id;
        public string Name;
        public int Side;
        public string SideName;
    }

    private static readonly Color SectionBg = new Color(0.24f, 0.26f, 0.34f, 0.6f);
    private static readonly Color SectionAccent = new Color(1f, 0.75f, 0.2f);
    private static readonly Color Side1Accent = new Color(0.35f, 0.65f, 1.0f);
    private static readonly Color Side2Accent = new Color(1f, 0.45f, 0.45f);
    private static readonly Color HintColor = new Color(0.75f, 0.75f, 0.75f);

    private static List<HeroItem> heroes;

    private GUIStyle titleStyle;
    private GUIStyle hintStyle;
    private bool stylesInitialized;

    private void InitStyles()
    {
        if (stylesInitialized) return;
        stylesInitialized = true;

        titleStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleLeft };
        titleStyle.normal.textColor = new Color(0.85f, 0.88f, 1.0f);

        hintStyle = new GUIStyle(EditorStyles.miniLabel) { wordWrap = true };
        hintStyle.normal.textColor = HintColor;
    }

    // 编辑器下按需加载配置表（HeroConfig/ForceConfig 的静态表只有 Load 后才可用）
    private static List<HeroItem> GetHeroes()
    {
        if (heroes != null) return heroes;
        if (HeroConfig.ConfigList.Count == 0)
            HeroConfig.Load();
        if (ForceConfig.ConfigList.Count == 0)
            ForceConfig.Load();

        heroes = new List<HeroItem>();
        foreach (var cfg in HeroConfig.ConfigList)
        {
            if (cfg == null || cfg.Id <= 0) continue;
            var force = ConfigManager.GetForceConfig(cfg.Side);
            heroes.Add(new HeroItem
            {
                Id = cfg.Id,
                Name = cfg.Name,
                Side = cfg.Side,
                SideName = force != null ? force.Name : ("阵营" + cfg.Side)
            });
        }
        // 按阵营/ID排序，方便下拉菜单分组显示
        heroes.Sort((a, b) => a.Side != b.Side ? a.Side.CompareTo(b.Side) : a.Id.CompareTo(b.Id));
        return heroes;
    }

    public override void OnInspectorGUI()
    {
        InitStyles();
        serializedObject.Update();

        DrawDefaultInspector();

        GUILayout.Space(10);
        DrawDebugSection();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDebugSection()
    {
        Rect header = GUILayoutUtility.GetRect(0, 24);
        EditorGUI.DrawRect(header, SectionBg);
        EditorGUI.DrawRect(new Rect(header.x, header.y, 3, header.height), SectionAccent);
        GUI.Label(new Rect(header.x + 8, header.y, header.width - 8, header.height),
            "调试阵容（配置后启动直接开战，不走选牌/商店）", titleStyle);

        GUILayout.Space(2);
        GUILayout.Label("任一方填入武将即视为调试开局；两方留空则走正常对局流程。武将按 1 级生成，按顺序摆入布阵格。", hintStyle);
        GUILayout.Space(4);

        DrawHeroList("左侧（1号位）", "debugHeroesSide1", Side1Accent);
        GUILayout.Space(4);
        DrawHeroList("右侧（2号位）", "debugHeroesSide2", Side2Accent);
    }

    private void DrawHeroList(string title, string propName, Color accent)
    {
        SerializedProperty listProp = serializedObject.FindProperty(propName);
        if (listProp == null)
        {
            GameLog.Error($"GameManagerEditor 未找到字段 {propName}");
            return;
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        Rect titleRect = GUILayoutUtility.GetRect(0, 20);
        EditorGUI.DrawRect(new Rect(titleRect.x, titleRect.y, 3, titleRect.height), accent);
        GUI.Label(new Rect(titleRect.x + 8, titleRect.y, titleRect.width - 8, titleRect.height),
            string.Format("{0}   共 {1} 名", title, listProp.arraySize), titleStyle);

        for (int i = 0; i < listProp.arraySize; i++)
        {
            SerializedProperty elem = listProp.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();
            DrawSideSwatch(elem.intValue);
            if (GUILayout.Button(HeroLabel(elem.intValue), EditorStyles.popup))
            {
                int index = i;
                ShowHeroMenu(id =>
                {
                    var p = serializedObject.FindProperty(propName);
                    if (index < p.arraySize)
                        p.GetArrayElementAtIndex(index).intValue = id;
                    serializedObject.ApplyModifiedProperties();
                });
            }
            if (GUILayout.Button("×", GUILayout.Width(22)))
            {
                listProp.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("＋ 添加英雄", GUILayout.Height(20)))
        {
            ShowHeroMenu(id =>
            {
                var p = serializedObject.FindProperty(propName);
                int n = p.arraySize;
                p.InsertArrayElementAtIndex(n);
                p.GetArrayElementAtIndex(n).intValue = id;
                serializedObject.ApplyModifiedProperties();
            });
        }

        EditorGUILayout.EndVertical();
    }

    // 阵营色块（颜色统一取自 SysColor）
    private void DrawSideSwatch(int heroId)
    {
        int side = 0;
        foreach (var h in GetHeroes())
            if (h.Id == heroId) { side = h.Side; break; }
        Rect r = GUILayoutUtility.GetRect(12, 18, GUILayout.Width(12));
        EditorGUI.DrawRect(new Rect(r.x + 1, r.y + 3, 10, 12), SysColor.GetSideColor(side));
    }

    private static string HeroLabel(int heroId)
    {
        foreach (var h in GetHeroes())
            if (h.Id == heroId)
                return string.Format("{0}·{1}  ({2})", h.SideName, h.Name, h.Id);
        return heroId > 0 ? ("未知英雄 (" + heroId + ")") : "（未选择）";
    }

    // 弹出按阵营分组的英雄菜单（GenericMenu 的 "/" 路径自动形成子菜单）
    private static void ShowHeroMenu(System.Action<int> onPick)
    {
        var menu = new GenericMenu();
        foreach (var h in GetHeroes())
        {
            int id = h.Id;
            menu.AddItem(new GUIContent(string.Format("{0}/{1} ({2})", h.SideName, h.Name, id)), false, () => onPick(id));
        }
        menu.ShowAsContext();
    }
}