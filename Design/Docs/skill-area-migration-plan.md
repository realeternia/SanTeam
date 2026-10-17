# skillCfg.Range → skillCfg.Area 选人半径迁移（收尾与验证）

## Context（背景）

用户指出 [SkillAidBuffArea.cs](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/Combat/Skill/SkillAidBuffArea.cs#L22) 中列表选人用了 `skillCfg.Range`（射程），而选人半径语义应为 `skillCfg.Area`（AoE 影响半径），并要求"所有代码都过一遍"。

经 AskUserQuestion 确认口径：
- Q1（奇才/国色/暴虐/一统）：**代码+配置一起迁移** —— 选取半径从配置 `Range` 迁到 `Area`（`Range` 置 0），代码改读 `Area`。
- Q2（其余射程类）：**全部改成 Area** —— 所有 `Get*InRange + skillCfg.Range` 的列表选人一并改用 `Area`。

字段语义依据：`Range`=射程/施法距离（`CheckInRange` 单目标判定用）；`Area`=AoE/影响半径（`Skill.PlayAreaEffect` 按 `Area` 缩放，见 [Skill.cs](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/Combat/Skill/Skill.cs#L399-L408)）。`WorldManager` 列表查询 `range==0` 表示全图。

## 当前状态（已完成，磁盘已落地）

### 代码侧（列表选人半径统一改 `skillCfg.Area`）
Grep 复核结论：全工程 `GetMySideInRange / GetEnemyInRange / GetUnitsInRangeAll / GetUnitsInRangeByTag / GetUnitsMySide / GetUnitsInRange` 传参**已全部为 `skillCfg.Area`**，唯一例外是 `SkillAidChargeImpale.cs:36`（有意保留，见下）。

代表性文件：
- [SkillAidBuffArea.cs](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/Combat/Skill/SkillAidBuffArea.cs#L22)（任务触发点）
- [SkillHitBuffArea.cs](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/Combat/Skill/SkillHitBuffArea.cs#L19)（真 bug：`连锁` 原 Range=0=全图、Area=40~80）
- [BuffMultiShot.cs](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/Combat/Buff/BuffMultiShot.cs#L17)
- 其余 20+ 个 `Combat/Skill/Skill*.cs`（AidBuff/Balance/BurstShot/CurseHeal/NobleBless/RaidCamp/RangeHeal/JumpHeal/...）。

### 有意保留 `skillCfg.Range` 的调用点（非列表选人，语义正确）
- 所有 `CheckInRange(...)`（单目标施法距离判定）。
- [SkillAidChargeImpale.cs](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/Combat/Skill/SkillAidChargeImpale.cs#L36)（`冲阵`：`Range`=直线长度、`Area`=走廊宽度，改 `Area` 会导致几乎打不到人；已在第 35 行加中文注释说明）。
- 几何/生成类：`SkillAidBurstShot.cs:33`、`SkillAidShadowSplit.cs:29`、`SkillAttackedTeleport.cs:20`、`SkillAttackedShadow.cs:20`、`SkillCastingAidScorchedEarth.cs:37-38`、`SkillCastingAidLastStrategy.cs:46`、`SkillDefFeedback.cs:22-25`、`SkillAidSuddenArrow.cs:15`。

### 配置侧（[SkillConfig_s.cs](file:///d:/U3dPrj/SanTeam/Assets/Resources/Scripts/Configs/SkillConfig_s.cs)）
迁移规则：`Area==0f` 的行 → 把该行 `Range` 值写入 `Area`（`Range` 不动）；`Area` 已非 0 的行不动；`AidBuffArea`（AidBuffArea 脚本）行额外把 `Range` 置 0。
已抽样核对正确：`2010141-45` 奇才=`0/50`、`2021221-25` 国色=`0/100`、`2020016-20` 暴虐=`0/80`、`2020021-25` 一统=`0/90`、`2021501-05` 旋风斩=`25/25`。

`git diff --numstat` 显示 `SkillConfig_s.cs` 111/111（含任务前既有未提交改动约 16 行），27 个 Skill 文件各 1~2 行，`BuffMultiShot.cs` 2/2。

## 剩余工作（收尾与验证）

1. **配置表安全性复核**：确认 `SkillConfig_s.cs` 无正则误改到非目标行、无语法/多重 BOM 破坏（可编译或至少人工抽查改动行 Id 集合）。
2. **异常项说明（汇报用，不需改代码）**：
   - `AidSwapStrike / HelpAidBuff / HitAround / AttackReboundArrow` 仅类文件与 `SkillManager` 有 case，配置表无对应数据行 → 配置侧无需迁移。
   - `BuffMultiShot`（多重箭）未找到施加该 Buff 的 SkillConfig 行（`BuffConfig` 300104 NameS="箭"，无 Skill 行 `BuffId="箭"`），其 `skillCfg.Area` 来源未解析。
   - `AidRangeHeal/AidJumpHeal/HitBuffArea` 等 `Area` 本已非 0 的行：其"第一步查询"改读 `Area` 会带来轻微半径变化（规则"Area 已非 0 不动"）。
3. **编译验证**：在 Unity/VS 侧编译确认无报错。

## 验证方式

- `Grep`：`(GetMySideInRange|GetEnemyInRange|GetUnitsInRangeAll|GetUnitsInRangeByTag)\s*\([^)]*skillCfg\.Range` → 期望仅 `SkillAidChargeImpale.cs:36`。
- 编译工程（Unity Editor 或 `dotnet build`），确认无错误。
- 战斗内实测 `奇才/国色/暴虐/一统/连锁` 技能选取范围与特效可见性正常。
