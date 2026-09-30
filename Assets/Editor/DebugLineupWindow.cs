using System.Collections.Generic;
using System.IO;
using CommonConfig;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 调试阵容配置窗口（菜单：Tools/调试阵容配置窗口）。
/// 在窗口里配置当前场景 GameManager 的左侧(1号位)/右侧(2号位)上阵武将：
/// 任一方填入武将即视为调试开局，启动游戏后跳过选牌/商店流程直接开战（见 WorldManager.SpawnDebugHeroes）；
/// 两方留空则走正常对局流程。点「开始战斗」会先保存场景再进入 Play。
/// 武将列表按 1 级生成，按顺序摆入布阵格。
/// </summary>
public class DebugLineupWindow : EditorWindow
{
    private const string MenuPath = "Tools/调试阵容配置窗口";
    private const string GameScenePath = "Assets/Scenes/MainGame.unity";
    private const string PropSide1 = "debugHeroesSide1";
    private const string PropSide2 = "debugHeroesSide2";

    // 面板配色
    private static readonly Color SectionBg = new Color(0.24f, 0.26f, 0.34f, 0.6f);
    private static readonly Color SectionAccent = new Color(1f, 0.75f, 0.2f);
    private static readonly Color Side1Accent = new Color(0.35f, 0.65f, 1.0f);
    private static readonly Color Side2Accent = new Color(1f, 0.45f, 0.45f);
    private static readonly Color HintColor = new Color(0.75f, 0.75f, 0.75f);

    private class HeroItem
    {
        public int Id;
        public string Name;
        public int Side;
        public string SideName;
    }

    private static List<HeroItem> heroes;

    private GUIStyle titleStyle;
    private GUIStyle hintStyle;
    private bool stylesInitialized;

    private GameManager gm;
    private SerializedObject so;

    [MenuItem(MenuPath)]
    private static void Open()
    {
        var win = GetWindow<DebugLineupWindow>("调试阵容");
        win.minSize = new Vector2(460f, 380f);
        win.RefreshTarget();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChangedInEditMode;
        RefreshTarget();
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorSceneManager.activeSceneChangedInEditMode -= OnActiveSceneChangedInEditMode;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        Repaint();
    }

    private void OnActiveSceneChangedInEditMode(Scene oldScene, Scene newScene)
    {
        RefreshTarget();
        Repaint();
    }

    // 当前活动场景里的 GameManager（GameManager.Instance 只在运行时 Awake 赋值，编辑器下不可用）
    private static GameManager FindGameManager()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || !scene.isLoaded)
            return null;
        foreach (var root in scene.GetRootGameObjects())
        {
            var found = root.GetComponentInChildren<GameManager>(true);
            if (found != null)
                return found;
        }
        return null;
    }

    private void RefreshTarget()
    {
        gm = FindGameManager();
        so = gm != null ? new SerializedObject(gm) : null;
    }

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
        if (HeroConfig.ConfigList.Count == 0)
            HeroConfig.Load();
        if (ForceConfig.ConfigList.Count == 0)
            ForceConfig.Load();

        if (heroes != null)
            return heroes;

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

    private void OnGUI()
    {
        InitStyles();
        if (gm == null)
            RefreshTarget();

        DrawHeader();

        if (gm == null)
        {
            EditorGUILayout.HelpBox(
                "当前活动场景（" + SceneManager.GetActiveScene().name + "）没有找到 GameManager，无法配置调试阵容。",
                MessageType.Warning);
            if (File.Exists(GameScenePath)
                && GUILayout.Button("打开场景 " + GameScenePath, GUILayout.Height(24)))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    EditorSceneManager.OpenScene(GameScenePath);
            }
            return;
        }

        so.Update();
        DrawHeroList("左侧（1号位）", PropSide1, Side1Accent);
        GUILayout.Space(4);
        DrawHeroList("右侧（2号位）", PropSide2, Side2Accent);
        so.ApplyModifiedProperties();

        DrawLineupWarning();

        GUILayout.Space(4);
        GUILayout.Label("任一方填入武将即视为调试开局；两方留空则走正常对局流程。武将按 1 级生成，按顺序摆入布阵格。配置写入当前场景，点「开始战斗」时自动保存场景。", hintStyle);

        GUILayout.Space(6);
        GUILayout.BeginHorizontal();
        using (new EditorGUI.DisabledScope(!gm.HasDebugLineup() || EditorApplication.isPlayingOrWillChangePlaymode))
        {
            if (GUILayout.Button("开始战斗", GUILayout.Height(28)))
                StartBattle();
        }
        if (GUILayout.Button("清空两侧", GUILayout.Height(28), GUILayout.Width(90)))
            ClearBoth();
        GUILayout.EndHorizontal();
    }

    // 顶部状态：场景名 / GameManager 是否找到 / 是否已配置调试阵容
    private void DrawHeader()
    {
        Rect header = GUILayoutUtility.GetRect(0, 24);
        EditorGUI.DrawRect(header, SectionBg);
        EditorGUI.DrawRect(new Rect(header.x, header.y, 3, header.height), SectionAccent);
        GUI.Label(new Rect(header.x + 8, header.y, header.width - 8, header.height),
            "调试阵容（配置后启动直接开战，不走选牌/商店）", titleStyle);

        GUILayout.Space(2);
        string state = gm == null
            ? "未找到 GameManager"
            : (gm.HasDebugLineup()
                ? string.Format("已配置：甲 {0} 名 / 乙 {1} 名", gm.debugHeroesSide1.Count, gm.debugHeroesSide2.Count)
                : "未配置（启动走正常对局流程）");
        GUILayout.Label("场景：" + SceneManager.GetActiveScene().name + "    " + state, hintStyle);
        GUILayout.Space(2);
    }

    private void DrawHeroList(string title, string propName, Color accent)
    {
        SerializedProperty listProp = so.FindProperty(propName);
        if (listProp == null)
        {
            GameLog.Error($"DebugLineupWindow 未找到字段 {propName}");
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
                    if (so == null)
                        return;
                    var p = so.FindProperty(propName);
                    if (p != null && index < p.arraySize)
                        p.GetArrayElementAtIndex(index).intValue = id;
                    so.ApplyModifiedProperties();
                    MarkSceneDirty();
                });
            }
            if (GUILayout.Button("×", GUILayout.Width(22)))
            {
                EditorGUILayout.EndHorizontal();
                listProp.DeleteArrayElementAtIndex(i);
                so.ApplyModifiedProperties();
                MarkSceneDirty();
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("＋ 添加英雄", GUILayout.Height(20)))
        {
            ShowHeroMenu(id =>
            {
                if (so == null)
                    return;
                var p = so.FindProperty(propName);
                if (p == null)
                    return;
                int n = p.arraySize;
                p.InsertArrayElementAtIndex(n);
                p.GetArrayElementAtIndex(n).intValue = id;
                so.ApplyModifiedProperties();
                MarkSceneDirty();
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
                return string.Format("{0}·{1}  ({2})", h.SideName, h.Name, heroId);
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

    // 超出布阵格数量的武将不会被生成，提前提示
    private void DrawLineupWarning()
    {
        if (gm == null)
            return;
        int over = Mathf.Max(gm.debugHeroesSide1.Count, gm.debugHeroesSide2.Count) - CombatConst.FormationCellCount;
        if (over > 0)
            EditorGUILayout.HelpBox(
                "布阵格只有 " + CombatConst.FormationCellCount + " 格，超出的 " + over + " 名武将不会生成。",
                MessageType.Warning);
    }

    private void MarkSceneDirty()
    {
        if (gm == null)
            return;
        var scene = gm.gameObject.scene;
        if (scene.IsValid())
            EditorSceneManager.MarkSceneDirty(scene);
    }

    private void ClearBoth()
    {
        if (so == null)
            return;
        var p1 = so.FindProperty(PropSide1);
        var p2 = so.FindProperty(PropSide2);
        if (p1 != null) p1.ClearArray();
        if (p2 != null) p2.ClearArray();
        so.ApplyModifiedProperties();
        MarkSceneDirty();
    }

    // 保存场景后进入 Play：GameManager.HasDebugLineup 为真时 WorldManager 会跳过选牌/商店直接开战
    private void StartBattle()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            GameLog.Warn("调试阵容：编辑器正在编译/刷新或已处于运行状态，暂不能开始战斗");
            return;
        }
        if (gm == null)
        {
            GameLog.Warn("调试阵容：当前场景未找到 GameManager");
            return;
        }
        if (!gm.HasDebugLineup())
        {
            GameLog.Warn("调试阵容：左右两侧都没有武将，无法进入调试战斗");
            return;
        }

        so.ApplyModifiedProperties();
        var scene = gm.gameObject.scene;
        if (scene.IsValid() && !string.IsNullOrEmpty(scene.path))
            EditorSceneManager.SaveScene(scene);
        GameLog.Info("调试阵容：进入战斗（甲 " + gm.debugHeroesSide1.Count + " 名 / 乙 " + gm.debugHeroesSide2.Count + " 名）");
        EditorApplication.EnterPlaymode();
    }
}