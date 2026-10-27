# 技能范围字段与范围特效改造（已完成）

> 整理日期：2026-10-27
> 状态：已实施并收尾（`Range`/`Area` 语义分离 + `EffectSize` → `AreaEffect`）
> 合并来源：原 `范围特效AreaEffect改造方案.md` 与 `skill-area-migration-plan.md`

---

## 一、字段语义

| 字段 | 语义 | 使用场景 |
| --- | --- | --- |
| `Range` | 射程 / 施法距离 | `CheckInRange` 单目标判定；几何类技能（直线长度、走廊宽度等） |
| `Area` | AoE / 影响半径 | 列表选人（`GetMySideInRange` 等）与范围特效缩放 |
| `AreaEffect` | 范围特效名（`EffectConfig` 名） | 按 `Area` 缩放播放；空 = 该技能无范围视觉 |
| `HitEffect` | 单体命中特效 | 打在主目标身上 |

- 列表选人半径一律用 `skillCfg.Area`；`range==0` 表示全图。
- **唯一有意保留 `Range` 的列表类调用**：[SkillAidChargeImpale.cs](../../Assets/Resources/Scripts/Combat/Skill/SkillAidChargeImpale.cs)（`冲阵`：`Range`=直线长度、`Area`=走廊宽度，改 `Area` 会几乎打不到人）。

## 二、范围特效实现

- 统一入口 `Skill.PlayAreaEffect(pos, time)`（[Skill.cs](../../Assets/Resources/Scripts/Combat/Skill/Skill.cs)），内部调用 `EffectManager.PlayPosSkillEffect`。
- **缩放公式**：`最终缩放 = EffectConfig.Scale × (Area / CombatConst.AreaEffectBaseRadius)`，基准半径 `10`。
- 特效挂在战场场景容器（`WorldManager.Units`）而非施法者节点，落点固定、不随施法者移动/转向。
- `AreaEffect` 为空或资源加载失败：`GameLog.Warn` 后返回 null（不再 NRE）。
- 普攻溅射类 [SkillHitArea.cs](../../Assets/Resources/Scripts/Combat/Skill/SkillHitArea.cs)、[SkillHitAround.cs](../../Assets/Resources/Scripts/Combat/Skill/SkillHitAround.cs) 不播范围特效，保留主目标 `HitEffect`。

## 三、已完成的历史变更

1. 删除 `SkillConfig.EffectSize` 列（该列原本被 `EffectConfig.Scale > 0` 完全覆盖，实际从未生效），由 `AreaEffect` 顶替。
2. 所有按 `Area` 结算的范围技能改播 `AreaEffect`，配置里缺失视觉的补齐。
3. 列表选人半径从 `Range` 迁移到 `Area`；`AidBuffArea` 类行额外把 `Range` 置 0。
4. `EffectConfig.GetConfigByName` 改为 `TryGetValue`，使特效路径回退分支可达。

## 四、验证要点

- 全工程检索 `Get*InRange(... skillCfg.Range ...)`，期望只剩 `SkillAidChargeImpale`。
- 战斗内实测 `奇才 / 国色 / 暴虐 / 一统 / 连锁` 等范围技能的选取范围与特效大小随 `Area` 变化。
- `SkillConfig` 无 `EffectSize` 残留，`AreaEffect` 为空的行不播特效且不报错。

## 五、代码索引

| 内容 | 位置 |
| --- | --- |
| 范围特效入口 | [Skill.cs](../../Assets/Resources/Scripts/Combat/Skill/Skill.cs) `PlayAreaEffect` |
| 特效播放与缩放 | [EffectManager.cs](../../Assets/Resources/Scripts/Combat/EffectManager.cs) `PlayPosSkillEffect` |
| 缩放基准半径 | [CombatConst.cs](../../Assets/Resources/Scripts/Combat/CombatConst.cs) `AreaEffectBaseRadius` |
| 特效配置 | [EffectConfig_s.cs](../../Assets/Resources/Scripts/Configs/EffectConfig_s.cs) |
| 技能字段 | [SkillConfig_s.cs](../../Assets/Resources/Scripts/Configs/SkillConfig_s.cs) `Range` / `Area` / `AreaEffect` / `HitEffect` |
