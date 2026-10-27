---
name: combat-skill-design
description: 《三国卡牌》(金铲铲式) SkillConfig 表的配置方法与战斗技能设计规范。当需要新增/修改一条技能配置（SkillConfig_s.cs 的 38 列）、分配技能 Id 段、按 Lv1~5 展开、写 Descript 动态引用、配伤害/治疗/护盾/MP/Buff/特效/召唤/连线字段，或排查配置不生效时使用。纯 Buff/BuffConfig、羁绊、物品掉落配置请另用对应技能。
---

# SkillConfig 表配置方法

本工程技能配置是**手写维护的唯一数据源**：`Assets/Resources/Scripts/Configs/SkillConfig_s.cs`。改技能 = 改该文件的 `new SkillConfig(...)` 数据行，**没有 Excel/源表/导入工具**，不需要考虑同步。

## 一、落地流程（新增一个技能）

1. **定形态**：单伤 / 群伤 / 单控 / 群控 / 单疗 / 群疗 / 自增 / 群增 / 光环 / 召唤 / 兵增 / 连击。
2. **选脚本**：先找现成通用脚本（见第八节），形态一致直接复用；确实没有才新建 `Combat/Skill/SkillXxx.cs`（英文类名，一技能一文件，继承 `Skill`）。
3. **取 Id**：按第三节段位取下一个空号，同一技能的 5 个等级必须**连号**。
4. **写 5 行数据**：`Sname` 相同、`Lv` 1~5，除数值字段外其余列**逐行保持一致**。
5. **写 Descript**：只在 Lv1 写模板，其余 4 行留空。
6. **注册**：新建 `.cs` 必须在 `Assembly-CSharp.csproj` 加 `<Compile Include>`（Editor 脚本加 `Assembly-CSharp-Editor.csproj`）。
7. **自检**：跑 `scripts/dump_skill.py <Sname>`（见第十节），确认 5 行齐全、Id 连号、脚本/Buff/特效存在。

> 构造函数**无默认参数**：新增列必须插到 `SkillConfig` 构造签名对应位置，并同步为**全部数据行**补值，否则编译即失败。

## 二、38 列逐列配置方法

### 2.1 身份与分类

| 列 | 类型 | 配置方法 |
| --- | --- | --- |
| `Id` | int | 唯一，按第三节段位分配；同 Sname 五个等级连号 |
| `Name` | string | 技能显示名，如 `烈火`；好友/职业连锁直接写技能名 |
| `Sname` | string | 技能缩写，是 `GetSkillConfig(sname, lv)`、`SetLevel`、好友/职业连锁的索引键。**英雄专属技能 Sname = 英雄名**（110 个英雄全部如此，`HeroConfig.Skill1` 挂它）；**Sname 全局唯一归属，禁止两个英雄共用**（`ConfigManager.PostModify` 会按英雄重复放大数值） |
| `Type` | string | 分类标签，只影响展示/统计，不驱动逻辑。现用值：`单伤 群伤 单控 群控 单增 群增 自增 单疗 群疗 光环 召唤 兵增 职业 连接 道具` |
| `Lv` | int | 1~5；同 Sname 每级一行 |
| `ScriptName` | string | 业务脚本类名（反射创建，构造签名固定 `(int skillId, Chess owner)`）；必须与 `Combat/Skill/` 下的类名完全一致 |

> 第 4 列 `Descript` 是技能说明文案，写法和动态引用规则见第四节。

### 2.2 触发与资源

| 列 | 类型 | 配置方法 |
| --- | --- | --- |
| `Rate` | float | 发动概率 0~1。**`<=0` = 必然发动**；`(0,1)` 时 `SysRandom.Value < Rate` |
| `CD` | float | 发动冷却（秒），`0` = 无冷却；与 `MpCost` 二选一或叠加，不要在脚本里另写死冷却 |
| `MpCost` | int | 技能 MP 上限。`0` = 不用蓝随时可发；`>0` 靠棋子 `mpRegen` 每秒充能，满才发动，发动后清空。见 [MP法力系统说明](../../Design/Docs/MP法力系统说明.md) |
| `StartMpRate` | float | 0~1 开局法力比例：`0` 从零充能（默认），`1` 开局满蓝 |
| `TriggerCondition` | string | 满足才触发，空 = 无条件。格式 `key<数值`，多条件 `;` 分隔且需**全部满足**；key 走 `Chess.GetAttr`（如 `hprate<50` = 自身生命低于 50%）。解析失败会 `GameLog.Warn` 并**判定为不触发** |
| `HurtTag` | string | 伤害标签，`AntiShield` = 绕过护盾直接打血（破盾类）；空 = 无 |

### 2.3 选目标与范围

| 列 | 类型 | 配置方法 |
| --- | --- | --- |
| `Range` | float | 射程/施法距离：单目标 `CheckInRange`、直线技能长度、走廊宽度的判定用；目标中心类范围技能 `<=0` 视为不限射程 |
| `Area` | float | AoE 半径 + **所有列表选人**（`GetEnemyInRange` 等）的搜索半径；`0` = 全图 |
| `TargetType` | string | **逗号分隔的标记串**（不是单选枚举），由所选 `ScriptName` 解释，`HasSkillFlag(flag)` 逐个匹配；空 = 脚本默认行为。**配之前必须先读该脚本类头部的标记目录注释** |
| `TargetCount` | int | 最大命中数；`>0` 时脚本会 `RandomSelect` 截断，`<=0` 时**禁止调用** `RandomSelect`（限数 0 会清空列表） |

> 硬约束：列表选人半径一律传 `skillCfg.Area`，不得传 `Range`。唯一例外是方向/直线型（`SkillAidChargeImpale` 冲阵：`Range`=直线长度、`Area`=走廊宽度），需注释说明。

**TargetType 常用标记（按脚本区分，可组合）**

| 脚本 | 标记 | 含义 |
| --- | --- | --- |
| `SkillAreaBuff`（群增） | `heroOnly` / `excludeSelf` / `selfAlways` | 只选英雄 / 排除施法者 / 施法者必得 |
| | `lowestHp` / `highestHp` / `highestArmor` | 按生命比例升 / 降序、按护甲降序取 `TargetCount` 名 |
| | `rangedOnly` / `meleeOnly` | 远程 / 近战优先排序（无匹配时回退其余单位） |
| | `fullHpSkip` / `noDedupe` | 跳过满血 / 不按首段 Buff 去重 |
| | `wholeMap` / `splitByRange` / `tauntAroundTarget` | 全图选人（`Area` 仅作嘲讽半径）/ 近战取第 1 段、远程取第 2 段 Buff / 挂完 Buff 嘲讽 `Area` 内敌人 |
| `SkillAreaDamageDeBuff`（群伤） | `self` / `missile` / `maxHpBonus` / `mpBurn` | 自身中心（不做射程校验）/ 走导弹 / 附加最大生命伤害 / 燃烧 `Strength2[mpBurn]` 蓝量 |
| `SkillAreaHeal`（群疗） | `overflowToArmor` | 溢出治疗转护甲 |
| `SkillSelfBuff`（自增） | `permanent` / `noStack` | 永久 / 不叠加 |
| `SkillBacklineStrike` | `smart` / `extra` | 智能选后排 / 额外效果 |
| `SkillJumpChain` | `heal` | 治疗弹射模式 |
| `SkillAttackedShield` | `ignoreWeakHit` | 忽略 ≤10 的低伤害 |

- 远程技能以自身为中心会全程打空（站位远离敌阵）→ 用目标中心；近战贴脸 AoE 用 `self`。
- 已废弃词不要复用：同一标记在不同脚本里含义必须一致，新增标记前先全局 `Grep HasSkillFlag`。

### 2.4 数值列

| 列 | 类型 | 配置方法 |
| --- | --- | --- |
| `DamageType` | int | 伤害类型：`0`=`DamageTypeMagic` 法术（受魔抗，`×(100+ap)/100`）、`1`=`DamageTypeAttack` 物理（受护甲，`+atk`）、`2`=`DamageTypeReal` 真实（无视抗性与护盾）。默认 `0` |
| `DamageStrength` | float | 伤害基值。公式见第五节；DOT 类也走它 |
| `HealStrength` | float | 治疗基值，与伤害完全解耦，禁止用伤害公式当治疗 |
| `Strength2` | string | 通用数值槽 `key:value,key:value`。key 由脚本/描述约定，缺 key 读到 0。常用 key：`shield`（护盾）、`reduceRate`（减伤）、`dodge`、`mpRegen`、`shareRate`、`soldierAtk`… |
| `StrengthBuff1` | string | Buff 数值槽，由各 `BuffXxx` 按固定 key 读取；多 Buff 组合时按 `BuffId` 段位顺序取对应 key（如 `攻,甲,抗` 对应 `atk,armor,magicRes`） |

数值写法约定：

- 比例类写小数（`0.13` = 13%），由描述 `/字段%` 或 Buff 侧 `×100` 输出。
- 护盾走 `Strength2[shield]`，按法强成长 `×(100+ap)/100`；**国家护盾除外**（走最大生命比例）。
- 不要预先乘品质系数：英雄专属技能的品质/耗蓝/法系修正由 `PostModify` 统一写回（见第五节）。

### 2.5 Buff 列

| 列 | 类型 | 配置方法 |
| --- | --- | --- |
| `BuffId` | string | 填 `BuffConfig.NameS` **短名**（不是 Id）；空 = 无 Buff；多个用 `,`/`，`/`;`/`；` 分隔 |
| `BuffTime` | float | Buff 持续秒数；常规 1~10 秒。**永久类效果不要靠超大 BuffTime 模拟**，用脚本直接改属性（`SkillKillGain` / `SkillAidCraftBuff` / `SkillSelfBuff.permanent`） |

- 一槽只能装一种语义：既要主动命中给目标上效果、又要开局给友军挂被动 Buff 时，冲突需拆（被动 Buff 配 `BuffId`，命中效果在脚本里用常量短名另引）。
- **BuffId 号段**：`300xxx` 正面通用、`3001xx` 正面特化、`301xxx` 负面通用、`3011xx` 负面特化。新增 Buff 先判正负面与是否特化，再在对应段顺延。

### 2.6 召唤列

| 列 | 类型 | 配置方法 |
| --- | --- | --- |
| `SummonTag` | string | 召唤物标签（对应单位/棋子标签）；空 = 非召唤技能 |
| `SummonCount` | int | 一次召唤数量 |
| `SummonTime` | float | 召唤物存活秒数 |
| `SummonHitInterval` | float | 召唤场伤害间隔（持续伤害类） |
| `SummonSpeed` | float | 召唤物移动速度 |

### 2.7 道具列

| 列 | 类型 | 配置方法 |
| --- | --- | --- |
| `ItemId` | int | 战斗开始时按 `Rate` 概率给所属玩家 1 个该道具；`0` = 无。对应 `ItemConfig.Id` |

### 2.8 表现列

| 列 | 类型 | 配置方法 |
| --- | --- | --- |
| `Action` | string | 施法动作名，脚本里 `owner.PlayerAnim(skillCfg.Action)` 播放；空 = 用默认动作 |
| `SelfEffect` | string | 施法/触发瞬间在**施法者身上**播的特效名（`EffectConfig.Name`），走 `PlaySelfEffect()` |
| `HitEffect` | string | 命中主目标时播的特效；单体技能必须有它或导弹表现 |
| `AreaEffect` | string | 范围特效，按 `Area / CombatConst.AreaEffectBaseRadius(15)` 缩放；空 = 不播 |
| `Icon` | string | 技能图标资源名（`SkillPic/`） |

### 2.9 连锁与光环列

| 列 | 类型 | 配置方法 |
| --- | --- | --- |
| `LinkSelf` | string | 只给**连接英雄本人**的加成串（`atk+0.2,armor+0.3` 或 `mpRegen+0.5`） |
| `LinkTeam` | string | 给我方其他英雄（不含本人）的加成，**配置即总量，不乘人数** |
| `AuroAttrs` | string | 光环技能的持续光环属性串，由光环脚本按帧/秒施加 |

- 子字段可被描述引用：`/linkself-atk`、`/linkteam-mpRegen`、`/auroattrs-atk`。
- 连锁数值改动只改配置，禁止写进 `JobLinkManager` 等代码；机制档位常量放 `CombatConst`。

## 三、Id 段（当前实际占用）

| 段 | 用途 | 分配规则 | 下一个空位 |
| --- | --- | --- | --- |
| `2000001~2000185` | 职业/国家连锁 | 每 Sname 5 连号，按 10 宽分块（国 2000001-5、王 2000011-15 … 工 2000181-185） | `2000191` |
| `2010001~2010165` | 好友连线技能 | 每组 5 连号，顺序递增 | `2010166` |
| `2020001~2021730` | 英雄个人技能 | 每英雄 5 连号；每百段约放 6~7 个英雄，块起点 `…01` | `2021731` |
| `2090001~2090030` | 道具技能 | 单行 `Lv=1`，稀疏递增（有跳号） | `2090031` |

- 新技能只取空段；**复用既有技能时不要改其 Id**。
- 改 Id 前先全工程 `Grep` 旧 Id，确认无脚本/`CombatConst` 硬引用。

## 四、等级展开与 Descript

- 同 `Sname` 展开 Lv1~5；`SetLevel(lv)` 切行，连锁/好友机制通过改 `Level` 生效。
- **Descript 只在 Lv1 写模板**，Lv2~5 留空：显示时自动回退 Lv1 模板并按本级字段值替换。
- 模板内联动态引用（斜杠**无需转义**）：`/damagestrength`、`/healstrength`、`/rate%`、`/cd`、`/range`、`/area`、`/bufftime`、`/targetcount`、`/mpcost`、`/strength2-<key>`、`/strengthbuff1-<key>`、`/linkself-<attr>`、`/linkteam-<attr>`、`/auroattrs-<attr>`。
  - 数字字段末尾加 `%` = 按百分比输出（`/rate%`：0.4 → "40%"）。
  - 比例槽 `strength2-<key>` 用 `%` 输出时按×100 处理。
- 无动态引用的整型文案（每级数字个数不同、必须逐级完整描述）才每级直接写满 Descript 兜底。
- 显示统一走 `ConfigManager.GetSkillDescript(cfg, withNext)`；`withNext=true` 附下一档数值（Tooltip 职业/好友档位用）。

## 五、数值公式（以 `Skill.cs` 现实现为准）

| 用途 | 公式 | 减免 |
| --- | --- | --- |
| 伤害 `DamageType=0` 法术 | `DamageStrength × (100 + ap) / 100` | 受魔抗 |
| 伤害 `DamageType=1` 物理 | `DamageStrength + atk` | 受护甲 |
| 伤害 `DamageType=2` 真实 | `DamageStrength + atk` | 无视抗性、护盾不吸收 |
| 治疗 | `HealStrength × (100 + ap) / 100` | — |
| 护盾（非国家） | `Strength2[shield] × (100 + ap) / 100` | 走护盾 Buff（`盾`） |

- 入口：`Skill.GetSkillDamage()` / `GetSkillHeal()` / `GetSkillShield()`，静态重载供 Buff 复用。
- **英雄专属技能（`HeroConfig.Skill1` 指向的 Sname）额外修正**（`ConfigManager.PostModify`，只作用于 `DamageStrength`/`HealStrength`）：
  `× QualitySkillFactor^(Q-1) × Clamp(MpCost / SkillMpStandard, 0.2, 2) × MagicRate`
  （`QualitySkillFactor=1.55`、`SkillMpStandard=100`、`MagicRate` 法系 1.5 / 物理 1.0）
  → 配置里写"Q1、耗蓝 100、非法系"的基准值，品质/耗蓝/法系收益自动生效，战场与 Tip 同口径。
- 职业/好友/道具技能的 Sname 不是英雄名，**不参与**上述修正。

## 六、数值设计原则

1. **单体 > 群体单段**：群体/弹射靠 `TargetCount`、`Area` 控制总输出，单段明显低于单体爆伤。
2. **MP 与 CD 是主要平衡轴**：高耗蓝/长 CD 换更高伤害或更强效果；不要把单体与群体拉到同一水平。
3. **以基准技能对照**：单体法术基准（`强击` Lv1 `DamageStrength=60`），逐级递增；群体用较低基值 + 多目标。
4. 数值只进配置，逻辑里不写死；调平衡只改 `SkillConfig`。
5. 同一 Sname 五行的 `Range/Area/TargetType/TargetCount` 语义必须一致。

## 七、Buff / 特效联动

- BuffId 填短名；组合按段位顺序读 `StrengthBuff1`（小键盘如 `攻,甲,抗` ↔ `atk,armor,magicRes`）。
- 特效名必须是 `EffectConfig.Name` 中存在的名字（见 `combat-effect-usage` 技能）。
- 范围技能的 `AreaEffect` 随 `Area` 缩放；普攻溅射类（`SkillHitArea`）只播主目标 `HitEffect`。

## 八、优先复用通用脚本

| 脚本 | 形态（配置行数） | 配置要点 |
| --- | --- | --- |
| `SkillSingleDamage` | 单体伤害（125 行，最常用） | `DamageStrength`/`HealStrength` + `BuffId`（空=纯伤害）；`TargetCount>0` = 对同一目标连打 N 段 |
| `SkillAreaDamageDeBuff` | 范围伤害 + 减益（60 行） | `TargetType` 空=目标中心 / `self`=自身中心；`BuffId` 可 0~N 段 |
| `SkillSelfBuff` | 自身增益（50 行） | `BuffId` 挂 1~N 个 Buff，数值放 `Strength2`/`StrengthBuff1` 由 Buff 读 |
| `SkillAreaBuff` | 群体增益（45 行） | `TargetType` 标记选人（见 2.3）；护盾类按统一护盾公式写 `Strength2[shield]` |
| `SkillAura` | 光环（30 行） | `AuroAttrs` 配持续光环属性 |
| `SkillAreaHeal` | 范围治疗（15 行） | `HealStrength` + `Area`，`overflowToArmor` 可转护甲 |
| `SkillJumpChain` | 弹射（10 行） | `TargetType=heal` 走治疗链，否则伤害链 |
| `SkillBacklineStrike` | 打后排（15 行） | `smart` 智能选后排 / 默认最远 |
| `SkillAidBuffLowHp` / `SkillDefReduceDamageRate` | 低血触发（5 行各） | `TriggerCondition=hprate<50` + `Strength2[reduceRate]` |
| `SkillAidChargeImpale` | 直线穿透 | `Range`=长度、`Area`=宽度（唯一合法用 `Range` 的列表技能） |
| `SkillDumb` | 占位（65 行） | 无战斗行为，仅用于职业/道具等纯展示行；**不要给它配伤害字段** |

- 远程技能以自身为中心会全程打空（站位远离敌阵），必须目标中心；近战贴脸 AoE 用 `self`。
- 类名与 `ScriptName` 用英文，不用拼音；表里已有同名类时优先复用，别新建特化类。

## 九、新建脚本要点

- 继承 `Skill`，构造 `(int skillId, Chess owner)`；生命周期钩子覆写签名与基类一致。
- 触发判定统一走基类 `CheckBurst`（CD / Rate / MpCost / TriggerCondition），**不要在脚本里重复判概率或蓝量**。
- 伤害/治疗/护盾一律用第五节公式，禁止自造系数；属性读取用 `owner.GetAttr(key)`。
- 技能专属常量就近放本文件 `public const`，跨文件共享用 `类名.常量`；通用 Id/机制数值放 `CombatConst`。
- 随机统一 `SysRandom`，日志统一 `GameLog`（中文）。
- 空判失败必须 `GameLog.Warn/Error`，禁止静默 return。

## 十、自检清单

- [ ] 5 行齐全、`Lv=1~5`、同 `Sname`、Id 连号、无 Id 冲突
- [ ] `ScriptName` 类存在且已加入 `.csproj`
- [ ] `BuffId` 短名在 `BuffConfig` 存在；`BuffTime` 合理（常规 1~10 秒；永久效果走脚本改属性，不用超大 BuffTime 模拟）
- [ ] `HitEffect`/`AreaEffect`/`SelfEffect` 特效名在 `EffectConfig` 存在
- [ ] 列表选人半径用 `Area`，范围技能 `Area ≠ 0`（除非确为全图）
- [ ] `TargetCount>0` 的随机截断与脚本 `RandomSelect` 匹配
- [ ] Lv2~5 的 Descript 为空，Lv1 模板引用的 key 在本行都有值
- [ ] 英雄专属技能 `Sname` = 英雄名、未被其他英雄占用
- [ ] 特殊计数：`hprate<50` 这类条件键能被 `Chess.GetAttr` 解析

**自动自检**：

```powershell
python .trae/skills/combat-skill-design/scripts/dump_skill.py 烈火
python .trae/skills/combat-skill-design/scripts/dump_skill.py 2020001
```

脚本按 `Sname` 或 `Id` 打印该技能全部等级行、关键字段，并检查行数/Id 连号/脚本文件/Buff 短名/特效名。
