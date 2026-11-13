---
name: combat-skill-design
description: 《三国卡牌》(金铲铲式) SkillConfig 表的配置方法与战斗技能设计规范。当需要新增/修改一条技能配置（SkillConfig_s.cs 的 41 列）、分配技能 Id 段、按 Lv1~5 展开、写 Descript 动态引用、配伤害/治疗/护盾/MP/Buff/特效/召唤/连线字段、维护技能模板表与模板组件表（SkillTemplateConfig / SkillTemplateComponentConfig，Template + Modules 组合），或排查配置不生效时使用。纯 Buff/BuffConfig、羁绊、物品掉落配置请另用对应技能。
---

# SkillConfig 表配置方法

本工程技能配置是**手写维护的唯一数据源**：`Assets/Resources/Scripts/Configs/SkillConfig_s.cs`。改技能 = 改该文件的 `new SkillConfig(...)` 数据行，**没有 Excel/源表/导入工具**，不需要考虑同步。

## 一、落地流程（新增一个技能）

1. **定形态**：单伤 / 群伤 / 单控 / 群控 / 单疗 / 群疗 / 自增 / 群增 / 光环 / 召唤 / 兵增 / 连击。
2. **套模板**：形态由「模板 + 组件」可表达时（见 2.10），配 `Template` + `Modules` 即可，不要为一组变体新建模板。
3. **选脚本**：先找现成通用脚本（见第八节），形态一致直接复用；确实没有才新建 `Combat/Skill/SkillXxx.cs`（英文类名，一技能一文件，继承 `Skill`）。
4. **取 Id**：按第三节段位取下一个空号，同一技能的 5 个等级必须**连号**。
5. **写 5 行数据**：`Sname` 相同、`Lv` 1~5，除数值字段外其余列**逐行保持一致**。
6. **写 Descript**：只在 Lv1 写模板，其余 4 行留空。
7. **注册**：新建 `.cs` 必须在 `Assembly-CSharp.csproj` 加 `<Compile Include>`（Editor 脚本加 `Assembly-CSharp-Editor.csproj`）。
8. **自检**：跑 `scripts/dump_skill.py <Sname>`（见第十节），确认 5 行齐全、Id 连号、脚本/Buff/特效存在。

> 构造函数**无默认参数**：新增列必须插到 `SkillConfig` 构造签名对应位置，并同步为**全部数据行**补值，否则编译即失败。

## 二、41 列逐列配置方法

### 2.1 身份与分类

| 列 | 类型 | 配置方法 |
| --- | --- | --- |
| `Id` | int | 唯一，按第三节段位分配；同 Sname 五个等级连号 |
| `Name` | string | 技能显示名，如 `烈火`；好友/职业连锁直接写技能名 |
| `Sname` | string | 技能缩写，是 `GetSkillConfig(sname, lv)`、`SetLevel`、好友/职业连锁的索引键。**英雄专属技能 Sname = 英雄名**（110 个英雄全部如此，`HeroConfig.Skill1` 挂它）；**Sname 全局唯一归属，禁止两个英雄共用**（`ConfigManager.PostModify` 会按英雄重复放大数值） |
| `Version` | float | 数据版本：老技能 `1.0f`，新增武将批次 `1.1f`。只作批次标记，不参与战斗逻辑 |
| `Type` | string | 分类标签，只影响展示/统计，不驱动逻辑。现用值：`单伤 群伤 单控 群控 单增 群增 自增 单疗 群疗 光环 召唤 兵增 职业 连接 道具` |
| `Template` | string | 技能模板名（对应 `SkillTemplateConfig.Name`；空=不使用模板、走原有各列；非空=基于模板+组件初始化，其余效果列可不配） |
| `Modules` | string[] | 组件名列表（对应 `SkillTemplateComponentConfig.Name`；空=使用模板默认组件） |
| `Lv` | int | 1~5；同 Sname 每级一行 |
| `ScriptName` | string | 业务脚本类名（反射创建，构造签名固定 `(int skillId, Chess owner)`）；必须与 `Combat/Skill/` 下的类名完全一致 |

> 第 5 列 `Descript` 是技能说明文案，写法和动态引用规则见第四节。

### 2.2 触发与资源

| 列 | 类型 | 配置方法 |
| --- | --- | --- |
| `Rate` | float | 发动概率 0~1。**`<=0` = 必然发动**；`(0,1)` 时 `SysRandom.Value < Rate` |
| `CD` | float | 发动冷却（秒），`0` = 无冷却；与 `MpCost` 二选一或叠加，不要在脚本里另写死冷却 |
| `MpCost` | int | 技能 MP 上限。`0` = 不用蓝随时可发；`>0` 靠普攻命中/受伤获取法力（`mpRegen` 为百分比加成），满才发动，发动后清空。见 [MP法力系统说明](../../Design/Docs/MP法力系统说明.md) |
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
| `LinkSelf` | string | 只给**连接英雄本人**的加成串（`atk+0.2,armor+0.3` 或 `mpRegen+5`，mpRegen 为百分比） |
| `LinkTeam` | string | 给我方其他英雄（不含本人）的加成，**配置即总量，不乘人数** |
| `AuroAttrs` | string | 光环技能的持续光环属性串，由光环脚本按帧/秒施加 |

- 子字段可被描述引用：`/linkself-atk`、`/linkteam-mpRegen`、`/auroattrs-atk`。
- 连锁数值改动只改配置，禁止写进 `JobLinkManager` 等代码；机制档位常量放 `CombatConst`。

### 2.10 技能模板 / 组件（Template / Modules）

两张表：`SkillTemplateConfig_s.cs`（模板，标准形态 + 主数值）、`SkillTemplateComponentConfig_s.cs`（组件，机制与形态修饰）。引用一律用**名字**，不用 Id。

- `SkillConfig.Template` = 模板名（对应 `SkillTemplateConfig.Name`，空=不用模板）；`SkillConfig.Modules` = 组件名数组（对应 `SkillTemplateComponentConfig.Name`，空=用模板默认 `Modules`）。基于模板时其余效果列可留空。

**归模板 vs 归组件（判定顺序）**

| 内容 | 归属 | 说明 |
| --- | --- | --- |
| 伤害档位 / 治疗量 / 护盾量等主数值 | **模板 `Values`** | 模板 `Values` 就是 Lv1~5 主数值，**不要再建“伤害档位/治疗量”组件** |
| 伤害倍率（按属性成长） | 组件 | 与固定基值分列，避免重复计价 |
| 控制/减益（减速、减防、减攻、减疗、增伤、眩晕、定身、混乱） | **组件** | 只管效果本身，**不写作用范围**；单控 = 单体伤害模板 + 眩晕/定身/混乱，群控 = 群体模板 + 同批控制组件，**不建“单体控制”模板** |
| 属性增益（加攻/加护甲/加法强/加魔抗/加攻速/加移速） | **组件** | 每条属性一个组件，可挂 `光环`、`自身强化`、`单体治疗`、`范围治疗`；`光环` 只是模板，不建“光环”组件 |
| 施法射程 / 施放中心（目标中心、自身中心） | **模板**（或模板 `Range`） | 施放中心是形态，按“目标中心 AOE / 自身中心 AOE”分模板；射程写模板 `Range` |
| AoE 半径 | **模板 `Area`（默认小范围=20）** | 默认小范围已含在模板里，不建“小范围”组件；扩大范围用 `中范围`(30) / `大范围`(40) 组件 |
| 连击、目标筛选、特殊机制 | 组件 | — |
| 持续伤害（毒/败 DOT） | **buff 组件** | 走伤害类 buff（`StrengthBuff1=dot:N`），不建“持续伤害”组件 |
| 持续施法、持续区域、召唤技能场（引导/落点区域/召唤投放） | **特化，不建模板也不建组件** | 依赖专用脚本形态，由脚本自身处理，不纳入模板+组件通用体系 |
| 跳跃弹射（多目标间跳跃结算、每跳递减） | **模板** | 是形态而非插件，不建“弹射”组件 |
| MP消耗、CD、概率、触发条件 | 组件（施放节奏） | 作为频率修正项，不与单次效果成本混为一项 |

**模板集合保持最小（当前 13 条）**：单体伤害大 / 单体伤害中 / 单体伤害小 / 群体伤害近距离（目标中心 AOE）/ 群体伤害自身中心（自身中心 AOE）/ 单体治疗 / 范围治疗 / 自身强化 / 光环 / 单体友方增益 / 群体友方增益 / 兵种增益 / 跳跃弹射。变体一律靠组件叠加，新增模板前先确认“不能由现有模板 + 组件表达”。

**模板字段口径**：`Effect`=效果 k-v（单字段承载“写哪列 + 五级数值”）；`Quality`=品质 1~4（紧邻 `Score` 之前，配表按品质染色：1 `#666666`、2 `#33CC33`、3 `#3333FF`、4 `#FFD700`）；`Score`=标准态总分（标准态=10 分，降档按数值比例下调）；`Range`=射程（0=默认攻击距离）；`Area`=范围半径（默认小范围 20，0=非范围技能）；`MpCost`/`CD`=标准态节奏。组件同样带 `Quality`(1~4，置于 `Score` 前)，`Effect`/`Score` 为单项值，与模板分累加时注意不要对同一项重复计价。

**Effect 单字段 k-v（模板 / 组件同构，紧邻 `Modules`/`Templates` 之后）**：一个字段同时表达“写入目标 + 五级数值 + Buff 时长”，替代原来的 `Values` + `EffectKey` + `ValueScale` + `BuffTime` 四列。

- 语法：分号分隔多条绑定，单条为 `Column[:sub][=v1,v2,v3,v4,v5]`；`None`/空 = 只计分不写列。数值是**写入后的最终值**，不再有换算倍率（百分比直接写小数）。单值绑定（如 `BuffTime=4`）在 Lv2~5 沿用同一值。
- 例：`DamageStrength=70,105,175,280,420`、`Area=30`、`TargetType:lowestHp`、`Buff:缓:slow=0.1,0.12,0.15,0.2,0.25;BuffTime=3`、`BuffTime=0.9,1.1,1.4,1.8,2.4;Buff:乱`、`Buff:疫;Strength2:healDown=0.3,0.35,0.4,0.5,0.6;BuffTime=4`。
- 支持的 `Column`：`DamageStrength` / `HealStrength` / `CD` / `MpCost` / `Range` / `Area` / `TargetCount` / `Rate` / `BuffTime` / `TargetType:标记` / `TriggerCondition:属性<`（数值即阈值，如 `TriggerCondition:hprate<=70,50,30,20,15`）/ `Strength2:键`（`auraRate`、`selfGrow`、`shield`、`healDown`）/ `Buff:短名[:槽键]`（短名取 `BuffConfig.NameS`；只写 `Buff:短名` 表示只加 buff 不给值）。
- 槽位归属看脚本实际读哪一列：绝大多数 Buff 读 `StrengthBuff1`（如 `缓`/`破`/`慑`/`攻`/`快`），**`疫`（减疗）与 `盾`（护盾）读 `Strength2`**（`healDown`/`shield`，比例值或护盾点数），所以「减疗」写成 `Buff:疫;Strength2:healDown=0.3,...`、「护盾」写成 `Buff:盾;Strength2:shield=60,...`，避免同一机制既加 buff 又加数值造成重复生效。
- `BuffTime` 绑定：组件提供 Buff 时同一条 `Effect` 里写 `BuffTime=秒数`（减益 3~4s、增益/护盾 5s）；控制类写成逐级值（`BuffTime=0.9,1.1,1.4,1.8,2.4`）。同一技能多个来源时长不同时取**最大值**并告警；没有 `BuffTime` 绑定时沿用技能行手配值。
- 手配优先：`BuffId`/`StrengthBuff1`/`Strength2`/`TargetType` 里已手配的 token、槽键、标记不会被覆盖，只追加缺失项（同名槽键在 `Strength2` 与 `StrengthBuff1` 任一槽位手配都算已配）。

**Id 分段约定**：两张表 Id 以 **10 为步长**分段（10/20/30…），同类集中在一段内、相似项相邻 +1（如 `中范围`=50、`大范围`=51），段内空号留作扩展。模板：10~19 单体伤害（大/中/小=10/11/12）、20~29 群体伤害（目标中心=20、自身中心=21）、30~39 治疗、40~49 预留（持续区域已改特化）、50~59 自身强化、60~69 光环、70~79 单体友方增益、80~89 群体友方增益、90~99 兵种增益、100~109 预留（召唤技能场已改特化）、110~119 跳跃弹射。组件：`10` 伤害倍率、`20` 治疗量、`30` 施法射程、`40` 最大目标数（以上一个概念占一段开头，不合并同类）、`50~59` 范围档（中=50、大=51）、`90` 连击（`91~99` 空，持续伤害改走 buff、持续施法/区域特化）、`100~119` 预留、`120~122` 施放节奏（MP消耗/CD/概率）、**触发条件固定 `4000~4999`**（按条件键拆分：生命低于 4000~4002、攻击低于 4010、法强低于 4020、生命值低于 4030，key 必须来自 `Chess.GetAttr`：ap/atk/might/hp/hprate）、`130` 光环生效比例、`140` 光环自身成长；**目标筛选固定 `3000~3999`**（按 10 步长排列，3000 低血筛选起）；**负面 buff 固定 `1000~1999`**、**正面 buff 固定 `2000~2999`**，且**每个 buff 拆 强度大/中/小 三档**（同一 buff 占一个 10 段，大=段首 +0、中=+1、小=+2，数值约为中档的 1.5/1/0.6，评分同步 1.5/1/0.6 并取 0.5 整数倍）；模板 `Modules` 默认引用中档（如 `属性增益中`）；**特殊机制固定 `9000~9999`**（9000~9060 各占一段、多目标结算族 9070~9072、补充机制 9080~9087）。

**一致性自检**：模板 `Modules` 里的每个组件名必须存在于组件表；组件 `Templates` 里的每个模板名必须存在于模板表；改模板名要同步改所有组件的 `Templates` 引用（改名会让引用悬空）。

### 2.11 SkillBaseConfig（模板展开表）

`SkillBaseConfig_s.cs` 与 `SkillConfig` **列结构完全一致（41 列）**，存放「模板 + 组件」展开后的效果列；`ConfigManager.Init()` 依次 `SkillConfig.Load()` / `SkillBaseConfig.Load()`，并在 `PostModify()` 开头调用 `ApplySkillBase()`：

- **触发条件**：`SkillConfig.Template` 非空即为模板化技能；同 Id 缺失 `SkillBaseConfig` 行时记 `GameLog.Warn` 并沿用原值。
- **覆盖列（32 列效果列）**：`Rate CD MpCost StartMpRate TriggerCondition DamageType HurtTag Range Area TargetType TargetCount DamageStrength HealStrength Strength2 StrengthBuff1 BuffId BuffTime SummonTag SummonCount SummonTime SummonHitInterval SummonSpeed ItemId ScriptName Action SelfEffect HitEffect AreaEffect Icon LinkSelf LinkTeam AuroAttrs`。
- **保留列（8 列身份列）**：`Id Name Sname Version Descript Type Template Modules Lv`。
- **展开规则（k-v 映射）**：模板先按 `EffectKey`/`ValueScale` 把 `Values[Lv-1]` 写入目标列/槽（伤害模板=`DamageStrength`、治疗模板=`HealStrength`，两者互斥清零）；随后 `Range`/`Area` 仅模板值 >0 时覆盖，`CD` 取模板，`MpCost` = 模板 `MpCost × 10`；再按**技能自己 `Modules`** 顺序展开每个组件的 `EffectKey`（列写入 / `Buff:短名:槽键` 追加 `BuffId`+`StrengthBuff1` / `TargetType:标记` / `TriggerCondition` / `Strength2:键`）。`BuffId`/`StrengthBuff1`/`Strength2`/`TargetType` 已手配的键不覆盖，只补缺失项。`BuffTime` 取模板与各组件提供值的最大值。模板默认 `Modules` 只参与估值与推荐校验，**不注入任何效果**（如「护盾中」不会给技能额外加盾）。
- **整体覆盖范围已扩大**：`EffectKey` 让 `Area`/`TargetCount`/`Rate`/`TriggerCondition`/`Strength2`/`BuffTime` 也能由组件写入；当前组件数据仍以 buff、目标筛选、触发条件为主，`伤害倍率`、`连击`、`后排筛选`、`友军筛选`、`光环成长` 等暂无对应列或标记（`EffectKey=None`，只计分）。
- **现状覆盖**：品质 1/2 英雄技能 80 个中 75 个模板化（375 行），5 个保留特化不模板化：曹叡·压制（SkillAidManaBurn）、王昶·屯田（SkillAidHitRegion）、高顺·破甲（SkillSingleDamage 按护盾加成）、蔡文姬·胡笳（SkillCastingAidAreaHeal）、黄月英·巧工（SkillAidCraftBuff）。品质 3/4 的专属机制不纳入模板体系。
- **数值口径提醒**：模板 `Values` 是「标准态」主数值，与手配 `DamageStrength`/`HealStrength`（受品质、MP 系数、法术倍率影响）不是同一量级，直接覆盖会显著改变战斗数值；需要标定时应在烘焙阶段按品质/MP 系数缩放模板值，或回调模板 `Values`，不要逐行改 `SkillBaseConfig`（它由脚本重新生成）。

## 三、Id 段（当前实际占用）

| 段 | 用途 | 分配规则 | 下一个空位 |
| --- | --- | --- | --- |
| `2000001~2000185` | 职业/国家连锁 | 每 Sname 5 连号，按 10 宽分块（国 2000001-5、王 2000011-15 … 工 2000181-185） | `2000191` |
| `2010001~2010165` | 好友连线技能 | 每组 5 连号，顺序递增 | `2010166` |
| `2020001~2021799` | 英雄个人技能 | 按职业分段：`202` + 段号(2位) + 序号(2位)。段号 = JobConfig 顺序（00王 01马 02车 03弓 04弩 05炮 06士 07盾 08扇 09相 10棋 11鼓 12琴 13医 14锤 15枪 16戟 17工）；每段 100 个 Id 号位（20 个英雄技能），每技能 5 连号，段内序号从 `…01` 起 | 工段 `2021736` |
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
