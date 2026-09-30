# 调试阵容配置改用 Unity 菜单启动

## Context

游戏内已有一条"直接开战"的调试链路：`GameManager.debugHeroesSide1/debugHeroesSide2` 填了武将后，`WorldManager` 会走 `SpawnDebugHeroes`，跳过选牌/商店流程直接进入战斗（相当于"进入模拟战斗"）。

但目前这个配置只能通过 `Assets/Editor/GameManagerEditor.cs` 的自定义 Inspector 来做（需要先在场景里选中挂着 GameManager 的物体），配置和开战是两件分离的事。本次改造把配置入口搬到 Unity 顶部菜单：菜单打开一个 EditorWindow，在窗口里配好左右两侧阵容并直接进入 Play 开战；旧的 Inspector 调试区块删除，避免同一套逻辑两处维护。

## 实现步骤

### 1. 新增 `Assets/Editor/DebugLineupWindow.cs`（`DebugLineupWindow : EditorWindow`）

菜单入口：`[MenuItem("Tools/调试阵容配置窗口")]`，与现有 [EffectPrefabScaleFixer.cs](file:///d:/U3dPrj/SanTeam/Assets/Editor/EffectPrefabScaleFixer.cs#L11) 的 `Tools/...` 扁平风格一致。

窗口内容：

- **状态行**：当前活动场景名 / 是否找到 GameManager / 是否已配置阵容（`gm.HasDebugLineup()`）。
- **找不到 GameManager 时**：提示文案 + 「打开 MainGame 场景」按钮（`EditorSceneManager.OpenScene("Assets/Scenes/MainGame.unity")`），其余控件禁用。
- **两组阵容列表**：左侧（1 号位）`debugHeroesSide1`、右侧（2 号位）`debugHeroesSide2`；每行 = 阵营色块 + 英雄下拉 + `×` 删除；底部「＋ 添加英雄」「清空两侧」「开始战斗」。
- **开始战斗**：先拦截 `EditorApplication.isCompiling || isUpdating || isPlayingOrWillChangePlaymode`（`GameLog.Warn` 中文提示并 return），要求至少一方非空；随后 `EditorSceneManager.SaveScene(...)` 落盘，再 `EditorApplication.EnterPlaymode()`（不用 `isPlaying = true`）。

复用的既有逻辑（从 `GameManagerEditor.cs` 整体搬过来，不重复造轮子）：

- 英雄缓存 `GetHeroes()`（按需 `HeroConfig.Load()` + `ForceConfig.Load()`，`ConfigManager.GetForceConfig(cfg.Side)` 取阵营名，`SysColor.GetSideColor(side)` 画色块）。
- `GenericMenu` 按阵营分组的英雄下拉（`ShowHeroMenu`）与 `HeroLabel`。

关键 Editor API 要点：

- **取实例**：`SceneManager.GetActiveScene().GetRootGameObjects()` 后 `GetComponentInChildren<GameManager>(true)`（可含未激活）；不要用 `GameManager.Instance`（只在运行时 `Awake` 赋值）。多场景时以活动场景为准，避免误改其它场景的实例。
- **编辑字段**：`new SerializedObject(gm)` + `FindProperty("debugHeroesSide1"/"debugHeroesSide2")`（字段是 `[HideInInspector]`，只能这样改），每次改动后 `ApplyModifiedProperties()`；实例失效时重建 SerializedObject。
- **落盘**：改动后 `EditorSceneManager.MarkSceneDirty(scene)`，否则场景不会保存。
- **刷新**：`OnEnable/OnDisable` 订阅/退订 `EditorApplication.playModeStateChanged`，进出 Play 时 `Repaint()` 刷新按钮可用态。
- 阵容超过 `CombatConst.FormationCellCount`(25) 时提示会被裁剪。

### 2. 删除旧入口

- 删除 [GameManagerEditor.cs](file:///d:/U3dPrj/SanTeam/Assets/Editor/GameManagerEditor.cs) 及其 `.cs.meta`。
- 从 [Assembly-CSharp-Editor.csproj](file:///d:/U3dPrj/SanTeam/Assembly-CSharp-Editor.csproj#L65) 移除对应的 `<Compile Include="Assets\Editor\GameManagerEditor.cs" />`。

### 3. csproj 登记新文件

在 `Assembly-CSharp-Editor.csproj` 增加 `<Compile Include="Assets\Editor\DebugLineupWindow.cs" />`（按项目规则，新增 .cs 必须登记；`.meta` 交 Unity 自动生成，不手工创建）。

## 不做的事

- **不改运行时脚本**：`GameManager.cs` 的字段/`HasDebugLineup()`、`WorldManager.SpawnDebugHeroes` 全部保持原样，窗口改的就是同一份场景序列化数据，Play 后链路不变。
- 不引入 EditorPrefs / 独立存档文件，配置随场景保存（与现状一致）。

## 验证

1. 打开 `Assets/Scenes/MainGame.unity`，菜单 `Tools/调试阵容配置窗口` 打开窗口，状态行显示找到 GameManager。
2. 左右各选 1~2 名武将（色块/阵营名/ID 正确），点「开始战斗」→ 编辑器进入 Play、无保存弹窗、跳过选牌与商店直接开战，武将按配置顺序生成且为 1 级。
3. 「清空两侧」后再跑 → 恢复选牌流程。
4. 切到不含 GameManager 的场景（如 EffectTest）→ 窗口给出提示且按钮禁用，不报错。