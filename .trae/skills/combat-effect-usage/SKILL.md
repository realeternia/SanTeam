---
name: combat-effect-usage
description: 配置与校验战斗技能/Buff 特效(HitEffect/AreaEffect/BuffEffect)的规则与特效池。当需要给技能选特效、新增或调整 EffectConfig、统计高频复用、排查特效错配时使用。不要用于纯数值或伤害公式设计。
---

# 战斗特效使用规范

指导在本工程中为技能/Buff 选择与校验特效。特效名（`EffectConfig.Name`）是外部引用 key，技能行通过 `HitEffect`/`AreaEffect`、Buff 通过 `BuffEffect` 引用它。

## 一、数据源

- 特效表 `Assets/Resources/Scripts/Configs/EffectConfig_s.cs`：`Name`(引用key) / `Cname` / **`Type`(增益·负面·伤害)** / **`Area`(目标·范围)** / `EffPath` / `MissilePath` / `Scale` / `OffsetY` / `MissileScale` / `MissileOffsetY` / `TintColor`(hex 变色，可复用同形态特效)。
- 技能表 `Configs/SkillConfig_s.cs`：`HitEffect`（命中特效，挂目标/自身）、`AreaEffect`（范围特效，挂地面并按技能 `Area` 缩放到基准半径）。
- Buff 表 `Configs/BuffConfig_s.cs`：`BuffEffect`（挂单位身上，随 buff 存续）。
- 播放入口 `Combat/EffectManager.cs`：`PlaySkillEffect`(命中)、`Skill.PlayAreaEffect`→`PlayPosSkillEffect`(范围)、`PlayBuffEffect`(buff)。

## 二、选择规则（硬约束）

`HitEffect` 只能取 **Area=目标** 的特效；`AreaEffect` 只能取 **Area=范围** 的特效；且 `Type` 必须与技能语义一致（伤害技能配伤害特效、增益配增益、控制配负面）。

| 技能 Type | HitEffect 池 | AreaEffect 池 |
|---|---|---|
| 单伤 | 伤害/目标 | 空（单体无范围特效） |
| 群伤 | 伤害/目标 | 伤害/范围 |
| 召唤 | 伤害/目标 | 伤害/范围（技能场） |
| 自增·单增·光环·单疗 | 增益/目标 | 空 |
| 群增·兵增·群疗 | 增益/目标 | 增益/范围 |
| 单控 | 负面/目标 | 空 |
| 群控 | 负面/目标 | 负面/范围 |

- 不要为无范围语义的技能硬塞 `AreaEffect`，保持空串。
- 职业/连接/道具类技能不套用上表，按效果性质单独判定。
- 同一 `Sname` 的 5 个等级行必须共用同一 `HitEffect`/`AreaEffect`（只按 `Sname` 分配一次，再写回 5 行）。

## 三、定位与改写（SkillConfig 行格式）

行形如 `config[id] = new SkillConfig(...)`，构造器共 17 个字符串参数，按出现顺序：

```
Name, Sname, Descript, Type, TriggerCondition, HurtTag, TargetType, BuffId,
SummonTag, ScriptName, Action, HitEffect(第12), AreaEffect(第13),
Icon, LinkSelf, LinkTeam, AuroAttrs
```

用正则抓取该行全部 `"..."`，第 12、13 个即命中/范围特效。改名时按匹配的字符跨度**原位替换**（先改索引大的第 13 个，再改第 12 个），避免同名串误伤。

## 四、池内轮换打散（压制高频复用）

1. 先按 `Type/Area` 从 EffectConfig 筛出候选池。
2. 对同池技能做「最少使用优先」分配（计数取自全局，池间特效名天然不重叠）。
3. 处理顺序按 `Sname` 分组，每组分配一次后写到其 5 个等级行。
4. 目标是每个特效复用次数贴近理论下界 `ceil(池内技能数 / 池内特效数)`。
5. 现状瓶颈（不新增特效就无法继续下降）：**增益/范围 仅 3 个、负面/范围 仅 1 个、负面/目标 仅 3 个、增益/目标 8 个需承载 46 个技能**。

## 五、校验脚本

`scripts/check_effect_usage.ps1`：解析 EffectConfig 生成 Type/Area 池，按 `Sname` 扫描 SkillConfig 英雄技能，报告**规则违例**（HitEffect 非目标 / AreaEffect 非范围 / 类型不匹配）与**复用频次 Top**。

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .trae/skills/combat-effect-usage/scripts/check_effect_usage.ps1
```

可用 `-RepoRoot <仓库根>` 覆盖自动推导的工程根目录。

## 六、Buff 特效要点

- buff 特效随 buff 存续：`PlayBuffEffect` 内对子 `ParticleSystem` 置 `main.loop = true` 后再 `Play()`（`GetComponentsInChildren<ParticleSystem>(true)`）。
- 一次性特效（命中/范围）不加 loop。
- 世界缩放/偏移由配置决定，不受父节点缩放影响（按父级 `lossyScale` 的 X 轴做标量补偿）。

## 七、注意事项

- 改 `EffectConfig_s.cs` 表结构（增删列）须按表列顺序同步 `fieldMeta`、字段、构造器与全部数据行，构造器不得用默认参数。
- `SkillConfig_s.cs` 文本以 UTF-8 多 BOM 开头，脚本读写须原样保留 BOM，避免 Unity/MSBuild 解析异常。
- 新增特效：只需要在 `EffectConfig_s.cs` 追加行（`Name` 全局唯一），`.meta` 交由 Unity 自动生成。
