# 范围特效 AreaEffect 改造方案

## Context（为什么改）

盘查发现 `SkillConfig.EffectSize` 这一列**实际完全失效**：

- [EffectManager.GetScale](file:///d:/Codes/SanTeam2/Assets/Resources/Scripts/Combat/EffectManager.cs#L26-L31) 的逻辑是「`EffectConfig.Scale > 0` 就用 `Scale`，否则才回退到传入的 `size`」，而 [EffectConfig_s.cs](file:///d:/Codes/SanTeam2/Assets/Resources/Scripts/Configs/EffectConfig_s.cs) 每一行 `Scale` 都 > 0 → 844 行配置里的 `EffectSize`（火海 1.6、明断 30、震碎 5…）从不生效；导弹通路 [Missile.cs](file:///d:/Codes/SanTeam2/Assets/Resources/Scripts/Combat/Missile.cs#L66) 同理被 `MissileScale` 覆盖。
- 「范围/对地」特效与「单体命中」特效混用同一个 `HitEffect` 字段，导致范围特效的视觉大小与 `skillCfg.Area` 完全脱钩。
- 29 个按 `skillCfg.Area` 结算的范围脚本里，有 **13 个完全没播任何特效**（但配置里已填了 `HitEffect`）。
- `PlayPosSkillEffect` [L127](file:///d:/Codes/SanTeam2/Assets/Resources/Scripts/Combat/EffectManager.cs#L127) 无判空，特效资源缺失即 NRE。

目标：删除 `EffectSize` 列；把范围特效拆成独立字段 `AreaEffect`，以 `EffectConfig.Scale` 为基准按 `Area` 额外缩放；所有按 `Area` 结算的范围技能统一播 `AreaEffect`，缺视觉的补上。

## 已确认的设计决策

1. **缩放公式**：`最终缩放 = EffectConfig.Scale × (Area / 基准半径)`，基准半径作为常量放 `CombatConst`（默认 `10f`）。
2. **改造范围**：29 个按 `skillCfg.Area` 结算的脚本；**排除**普攻溅射类 [SkillHitArea](file:///d:/Codes/SanTeam2/Assets/Resources/Scripts/Combat/Skill/SkillHitArea.cs)、[SkillHitAround](file:///d:/Codes/SanTeam2/Assets/Resources/Scripts/Combat/Skill/SkillHitAround.cs)（它们保留打在主目标身上的 `HitEffect`，不播范围特效）。
3. **HitEffect 迁移**：按当前播法区分（见步骤 5）。

## 实施步骤

### 1. SkillConfig 列变更 —— [SkillConfig_s.cs](file:///d:/Codes/SanTeam2/Assets/Resources/Scripts/Configs/SkillConfig_s.cs)

`EffectSize` 的位置由 `AreaEffect` 顶替（仍夹在 `HitEffect` 与 `Icon` 之间），四处必须同步：

| 位置 | 现状 | 改为 |
|---|---|---|
| `fieldMeta` L75 | `{"EffectSize", ("size","float",60)}` | `{"AreaEffect", new FieldMetaInfo("范围特效(仅范围技能用,按Area缩放)","string",0)}` |
| 字段 L215-218 | `public float EffectSize;` | `public string AreaEffect;`（`/// <summary>` 同步） |
| 构造参数 L237 | `float EffectSize,` | `string AreaEffect,` |
| 构造赋值 L275 | `this.EffectSize = EffectSize;` | `this.AreaEffect = AreaEffect;` |

**844 行数据行**：每行尾部的 `<float>f`（EffectSize）替换为 `AreaEffect` 字符串 token。结构已校验：844 行全部满足「尾部 = `<float>f, "<Icon>", "<LinkSelf>", "<LinkTeam>", "<AuroAttrs>");`」，可用锚定 `$` 的正则 (`([\d.]+)f, ("[^"]*"), ("[^"]*"), ("[^"]*"), ("[^"]*"\);)$`) 唯一定位。

替换值按行的 `ScriptName` 决定：属于下表 29 个脚本的按规则取；其余脚本一律 `""`。用一条 PowerShell 逐行脚本（读行 → 正则捕获 ScriptName/Action/HitEffect → 查映射表 → 重建行）完成，不落盘临时文件；跑完用 Grep 校验 `new SkillConfig(` 仍为 844 行、无 `EffectSize` 残留。

### 2. CombatConst

新增常量（含中文注释「范围特效缩放基准半径：EffectConfig.Scale 对应的半径，实际缩放 = Scale × Area / 本值」）：

```csharp
public const float AreaEffectBaseRadius = 10f;
```

### 3. EffectManager

- `PlayPosSkillEffect(Chess parent, Vector3 pos, float area, string effect, float time)`：参数 `size` → `area`，缩放改为
  `baseScale = GetScale(effect, cfg, prefab?prefab.localScale.x:1f)`；`scale = baseScale × (area / CombatConst.AreaEffectBaseRadius)`。
- 补判空，与 `PlaySkillEffect` 对齐：`effect` 为空或资源加载失败 → `GameLog.Warn` 后返回 null（当前会 NRE）。
- `EffectConfig.GetConfigByName` 由 `idxName[val]`（未知名抛 `KeyNotFoundException`）改为 `TryGetValue`，使 [ResolveEffectPath](file:///d:/Codes/SanTeam2/Assets/Resources/Scripts/Combat/EffectManager.cs#L11-L23) 里「回退 Prefabs/Effect + Warn」的分支真正可达。

### 4. Skill 基类助手 —— [Skill.cs](file:///d:/Codes/SanTeam2/Assets/Resources/Scripts/Combat/Skill/Skill.cs)

新增统一入口，所有范围技能都用它，不再各自拼 `EffectManager` 参数：

```csharp
protected GameObject PlayAreaEffect(Vector3 pos, float time = 1.3f)
// AreaEffect 为空 → 静默返回（可选参数为空）
// time<=0 → 取 skillCfg.SummonTime>0 ? SummonTime : 1.3f
// 内部调 EffectManager.PlayPosSkillEffect(owner, pos, skillCfg.Area, skillCfg.AreaEffect, time)
```

### 5. 29 个脚本改造（按三类）

统一原则：**特效中心 = 该脚本 `GetUnitsInRange` 的第一参数**（自身或目标落点）；`SkillHitEffect` 的单体调用保持不变。

**A. 对地播（6 个）**——把 `PlayPosSkillEffect(..., skillCfg.EffectSize, skillCfg.HitEffect, ...)` 换成 `PlayAreaEffect(pos, summonTime)`：
`SkillAidScorchedEarth`、`SkillAidJudgement`、`SkillHitRegion`、`SkillHitWall`、`SkillHitFireArea`、`SkillAidAreaHeal`（AreaHeal 逐单位 `PlaySkillEffect(unit, HitEffect)` 保留）。

**B. 已在单位上播 HitEffect（10 个）**——保留原调用，新增一次 `PlayAreaEffect(中心)`：

| 脚本 | 中心 | 建议 AreaEffect |
|---|---|---|
| SkillAidTauntSlam / SkillAidBarbarianSlam | owner | SparkleAreaWhite / MagicFieldGreen |
| SkillAidSevenCharge / SkillAttackRunCrossPlus | owner | MagicFieldGreen / SwordWhirlwindWhite |
| SkillAidHexField / SkillAidQuietPlot | 目标 | MagicFieldGreen / ShadowExplosion |
| SkillAidMeteorHammer / SkillAidUsurpPressure | 目标 | ToolExplosion / MagicNovaBlue |
| SkillAidJumpHeal / SkillAidRangeHeal | 治疗落点 | MagicBuffGreen |

**C. 当前完全无特效（13 个）**——新增 `PlayAreaEffect(中心)`，`AreaEffect` 取现有 `HitEffect` 值（该行 `HitEffect` 为空时按语义补）：
`SkillAidSweepingAoe`、`SkillAidScorchingRain`、`SkillAidColossalSlam`、`SkillAidScatterShot`、`SkillAidIronGuard`、`SkillAidDiveCleave`、`SkillAidDecreeAoe`、`SkillAidBerserkCleave`、`SkillAidArmorBreak`、`SkillAidBashShield`、`SkillAidDualLaser`、`SkillAidDoubleShot`、`SkillAidQuake`。
已知示例：枪出如龙 `AuraSoftPurple`、恶来 `MagicChargeYellow`、震碎 `MagicNovaBlue`。中心逐文件确认（多为 owner 自身中心）。

**另外**：[SkillAidShockWave](file:///d:/Codes/SanTeam2/Assets/Resources/Scripts/Combat/Skill/SkillAidShockWave.cs#L29) 第 29 行传了 `skillCfg.EffectSize`（删列后编译不过），改为传 `skillCfg.Area`（该值只作导弹缩放兜底，实际被 `MissileScale` 覆盖）。

### 6. ConfigManager —— [ConfigManager.cs](file:///d:/Codes/SanTeam2/Assets/Resources/Scripts/Configs/ConfigManager.cs)

- 删 `case "effectsize"`（L440）与 `IsNumericField` 中的 `case "effectsize"`（L413）。
- `AreaEffect` 是字符串字段，当前无 Descript 引用，不新增 `/areaeffect`。

### 7. 同步项

- 已确认 `Tools/**`、`Assets/Editor/**` 无 `EffectSize` / SkillConfig 列引用（战斗模拟器只用 `SkillConfig.GetConfig`），无工具需同步。
- 本次不新增 `.cs` 文件，无需改 csproj。
- 不手工创建/修改 `.meta`。

## 验证

1. `msbuild Assembly-CSharp.csproj` 0 error。
2. `msbuild Tools/战斗模拟器/BattleSimulator/BattleSimulator.csproj` 0 error。
3. BattleSimulator headless 冒烟：多种子（seed 1~12）+ 覆盖火海 / 明断 / 枪出如龙 / 恶来 / 震碎 / 困敌 的阵容，确认 exit 0、stderr 无异常、技能伤害与 `PlayPosSkillEffect` 日志正常（重点看缩放值 = `Scale × Area / 10`）。
4. Grep 校验：全仓无 `EffectSize` / `effectsize` 残留；`new SkillConfig(` 仍为 844 行；抽查 5 行尾部结构为「4 个字符串 + `);`」。
5. 抽查 3 个代表技能（火海 Area=12 → 1.2×、明断 Area=25 → 2.5×、枪出如龙 Area=24 → 2.4×）在战斗内确认特效尺寸随 Area 缩放。

## 不在本次范围（待确认）

- 无配置行的孤儿脚本（`SkillHitFireArea`、`SkillHitRegion`、`SkillHitWall`、`SkillAidAreaHeal`、`SkillAidShockWave`）是否删除。
- `SkillHitWall`「单法术场驱动多落点特效」的结构问题、法术场/特效/伤害协程生命周期不同步问题。
- 明断现有 `HitEffect=MagicChargeYellow` 是「蓄力」造型而非雷阵，迁移后仍是该造型（如需换 `LightningExplosionBlue`/`MagicFieldGreen` 请告知）。