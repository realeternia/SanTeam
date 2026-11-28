﻿﻿﻿---
name: combat-skill-design
description: 《三国卡牌》(金铲铲式) SkillConfig 表的配置方法与战斗技能设计规范。当需要新增/修改一条技能配置（SkillConfig_s.cs 的 43 列）、分配技能 Id 段、按 Lv1~5 展开、写 Descript 动态引用、配伤害/治疗/护盾/MP/Buff/特效/召唤/连线字段、维护技能模板表与模板组件表（SkillTemplateConfig / SkillTemplateComponentConfig，Template + Modules 组合），或排查配置不生效时使用。纯 Buff/BuffConfig、羁绊、物品掉落配置请另用对应技能。
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
### 脚本命名规范（强制）

| 前缀 | 用途 | 判定方式 |
| --- | --- | --- |
| `SkillAidXxx` | 英雄专属技能 | 一个英雄（或少数英雄）一套实现，含专属机制/演出；重写 `Skill.CheckAidSkill` 或挂专属钩子 |
| `SkillComXxx` | 通用模板技能 | 行为由 `SkillConfig` 字段 + `TargetType` 标记驱动，一份脚本服务多个英雄/多行配置 |
| `SkillSoldierXxx` | 兵种/召唤物专属技能 | 只被 `SoldierConfig.Skill` 引用（如喷火兽的 `SkillSoldierHitWall`） |
| `SkillBondXxx` | 羁绊/连锁技能 | `SkillConfig.Type` 为 `职业` 或 `连接` |
| `SkillEquipXxx` | 道具技能 | `SkillConfig.Type` 为 `道具` |

- `ScriptName`、类名、文件名三者必须一致；新增脚本先按上表选前缀，禁止 `SkillTaunt` 这类无分类名。
- `SkillComXxx` 内部再按「组名 + 形态」命名：组名固定为 `Damage`（伤害，含带伤害的嘲讽 `SkillComDamageTaunt`）/ `Heal`（治疗）/ `Buff`（增益、减益、控制等一切通过 Buff 生效且不造成伤害的效果），形态为 `Single` / `Area` / `ShockWave` / `JumpChain` / `HitRegion` / `Aura` / `Casting` / `Taunt` 等；
  如 `SkillComDamageSingle`、`SkillComDamageArea`、`SkillComHealAreaCasting`、`SkillComBuffSingle`、`SkillComDamageTaunt`。禁止把形态写在组名前面（`SkillComSingleDamage`、`SkillComAreaBuff` 均已废弃）。
- 同一行为被多类技能共用时拆成两个完整脚本：英雄侧保留 `SkillAidXxx` / `SkillComXxx` 实现，羁绊侧完整复制 `SkillBondXxx`、道具侧完整复制 `SkillEquipXxx`；羁绊+道具共用时 Bond 侧保留实现，Equip 侧完整复制。
- 复制脚本必须完整保留原实现的业务逻辑，不得只写空壳或通过继承复用；`SkillDumb` 为特殊占位脚本，保持原名 `SkillDumb`，不加 `SkillBond` 前缀。
- 新增/改名脚本必须同步 `Assembly-CSharp.csproj`，并全量更新 `SkillConfig` 与 `SkillBaseConfig` 的 `ScriptName`。

> 构造函数**无默认参数**：新增列必须插到 `SkillConfig` 构造签名对应位置，并同步为**全部数据行**补值，否则编译即失败。

## 二、43 列逐列配置方法

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
| `AuraMpRate` | float | 光环耗蓝倍率（仅 `SkillComBuffAura` 生效）：开启期间每秒消耗 MP = 1×该值，默认 20（组件 150~154 可改 15/20/25/30/40） |
| `TriggerCondition` | string | 满足才触发，空 = 无条件。格式 `key<数值`，多条件 `;` 分隔且需**全部满足**；key 走 `Chess.GetAttr`（如 `hprate<50` = 自身生命低于 50%）。解析失败会 `GameLog.Warn` 并**判定为不触发** |
| `HurtTag` | string | 伤害标签，`AntiShield` = 绕过护盾直接打血（破盾类）；空 = 无 |

### 2.3 选目标与范围

| 列 | 类型 | 配置方法 |
| --- | --- | --- |
| `Range` | float | 射程/施法距离：单目标 `CheckInRange`、直线技能长度、走廊宽度的判定用；目标中心类范围技能 `<=0` 视为不限射程 |
| `Area` | float | AoE 半径 + **所有列表选人**（`GetEnemyInRange` 等）的搜索半径；`0` = 全图 |
| `SelectOnly` | string | **目标硬过滤**（`;` 分隔，全部满足才可选，空=不过滤）。词表：`ally`/`enemy` 候选阵营（一般必配其一，都未配时用脚本默认阵营）、`hero`/`soldier` 仅英雄/仅士兵、`notSelf` 排除施法者、`inFight` 仅交战中、`notFullHp` 排除满血、`hpBelow:50` 生命百分比低于 N、`job:盾|士` 指定职业、`hasBuff:短名` 携带指定 Buff |
| `SelectFirst` | string | **目标优先级链**（`;` 分隔，按顺序逐条比较，前一条分出胜负即返回，平手看下一条；空=随机）。词表：`lowestHp`/`highestHp`/`highestArmor`/`lowestArmor`/`highestAp`/`highestAtk`/`highestMagicRes`/`lowestMagicRes`/`highestMp`（蓝耗技能法力和最高）/`biggestHpGap`（生命缺口最大）/`nearest`/`farthest`/`rangedFirst`/`meleeFirst`/`heroFirst`/`nearestToEnemy`（站位最靠前）/`random` |
| `TargetType` | string | **仅保留脚本私有行为标记**（`self`/`missile`/`noDedupe` 等），目标筛选已全部迁到 `SelectOnly`/`SelectFirst`；`HasSkillFlag(flag)` 逐个匹配，配之前先读脚本类头部的标记注释 |
| `TargetCount` | int | 最大命中数：**有 `SelectFirst` 时按优先级降序取前 N，没有则随机取 N**；`<=0` 表示范围内全部（禁止再调用 `RandomSelect`，限数 0 会清空列表） |

> 选人统一走 `Combat/TargetSelector.cs`：先按 `SelectOnly` 过滤，再按 `SelectFirst` 排序，最后按 `TargetCount` 截断；单体技能同样适用（当前索敌目标仍满足规则时沿用）。
> 非范围查询的命中列表（矩形/光束等几何命中、跳跃弹道逐跳、通信 Buff 的额外箭矢）走 `TargetSelector.Filter`（基类 `SelectTargets(候选列表, 中心, limit, ...)`）。
> **一个技能行只能有一组规则**：若同一技能存在两套选取（如"治疗最低血友军 + 随机给敌人挂负面"、"主目标最远 + 落点最近全打"），主场景用配置两列，次要场景在脚本里用 `SelectTargetsWithRules(..., "enemy", "nearest")` 显式传规则，并在类头注释标注。

> 硬约束：列表选人半径一律传 `skillCfg.Area`，不得传 `Range`。唯一例外是方向/直线型技能（`Range`=直线长度、`Area`=走廊宽度），需注释说明。

**TargetType 行为标记（目标筛选已迁到 SelectOnly/SelectFirst，此处只剩脚本私有行为）**

| 脚本 | 标记 | 含义 |
| --- | --- | --- |
| `SkillComBuffArea`（群增） | `selfAlways` | 施法者必定获得（其余成员按 `TargetCount` 选） |
| | `noDedupe` | 不按首段 Buff 去重 |
| | `targetCenter` | 以选中的首个友军目标为中心，再在 `Area` 内扩散；缺省为自身中心 |
| `SkillComDamageArea`（群伤） | `self` / `missile` / `highHp` / `mpBurn` | 自身中心（不做射程校验）/ 走导弹 / 目标高血增伤 / 燃烧 `Strength2[mpBurn]` 蓝量 |
| `SkillComHealArea`（群疗） | `overflowToArmor` | 溢出治疗转护甲 |
| `SkillComBuffSingle`（单增） | `permanent` / `noStack` | 永久 / 不叠加 |
| `SkillBondBacklineStrike` | `smart` | 智能选后排 |
| `SkillComHealJumpChain` | （无） | 治疗弹射专用脚本，固定对友军英雄跳跃治疗；伤害弹射见 `SkillComDamageJumpChain` |
| `SkillComDamageSingle` | `execute` / `antiShield` 等 | 见脚本头部注释 |

**选人类技能推荐配置**

| 场景 | SelectOnly | SelectFirst |
| --- | --- | --- |
| 群体增益（低血优先） | `ally;hero;notSelf;notFullHp` | `lowestHp` |
| 群体增益（高血/远程） | `ally;hero;notSelf` | `rangedFirst;highestHp` |
| 群体治疗 | `ally;hero;notFullHp` | `lowestHp` |
| 范围伤害 / 群体减益 | `enemy` | 空（随机）或 `lowestHp` |
| 后排打击 | `enemy` | `farthest` |
| 切入/连击（优先英雄） | `enemy` | `heroFirst;farthest` |
| 抽蓝 / 打高蓝目标 | `enemy;inFight` | `highestMp` |
| 群体治疗（按缺口） | `ally;notFullHp` | `biggestHpGap` |
| 单体伤害 | `enemy` | 空（沿用当前索敌目标） |

- 单体类脚本（`SkillComDamageSingle`、`SkillComDamageShockWave*`、`SkillAidDamageLineLaser` 等）统一走基类 `SelectSingleTarget(Range)`：未配 `SelectFirst` 时行为与原来一致（沿用 `owner.targetChess` 并校验射程）。
- 不走筛选的类型：冲刺/直线路径扫掠（`SkillAidDamageLineCharge`、`SkillBondAttackRunCrossPlus`）、全员光环/全体友军加成（`SkillComBuffAura`、`SkillBondFactionShield`、`SkillAidSoldier*` / `SkillBondInitSoldierBuff` 的小兵加成）、嘲讽强制、导弹逐帧探测。这些不是"选目标"，`SelectOnly`/`SelectFirst` 不参与逻辑。
- 远程技能以自身为中心会全程打空（站位远离敌阵）→ 用目标中心；近战贴脸 AoE 用 `self`。
- 已废弃词不要复用：同一标记在不同脚本里含义必须一致，新增标记前先全局 `Grep HasSkillFlag`。
- **标记实现位置**：`TargetType` 标记与组件 `Strength2` 键只在 3 个体系基类里读取（见第八节表格），子类（`SkillComDamageArea` / `SkillComDamageShockWave` / `SkillComBuffArea` / `SkillComDamageTaunt` 等）只声明形态，不得再写 `HasSkillFlag` / `GetStr2`，保证同一组件在所有形态下行为一致（脚本自有形态参数如 `moveDist` 除外）。

### 2.4 数值列

| 列 | 类型 | 配置方法 |
| --- | --- | --- |
| `DamageType` | int | 伤害类型：`0`=`DamageTypeMagic` 法术（受魔抗，`×(100+ap)/100`）、`1`=`DamageTypeAttack` 物理（受护甲，`+atk`）、`2`=`DamageTypeReal` 真实（无视抗性与护盾）。默认 `0` |
| `DamageStrength` | float | 伤害基值。公式见第五节；DOT 类也走它 |
| `HealStrength` | float | 治疗基值，与伤害完全解耦，禁止用伤害公式当治疗 |
| `Strength2` | string | 通用数值槽 `key:value,key:value`。key 由脚本/描述约定，缺 key 读到 0。常用 key：`shield`（护盾）、`reduceRate`（减伤）、`dodge`、`mpRegen`、`mpShareRate`、`mpRestore`、`mpTransferRange`、`soldierAtk`… |
| `StrengthBuff1` | string | Buff 数值槽，由各 `BuffXxx` 按固定 key 读取；多 Buff 组合时按 `BuffId` 段位顺序取对应 key（如 `攻,甲,抗` 对应 `atk,armor,magicRes`） |

数值写法约定：

- 比例类写小数（`0.13` = 13%），由描述 `/字段%` 或 Buff 侧 `×100` 输出。
- 护盾走 `Strength2[shield]`，按法强成长 `×(100+ap)/100`；**国家护盾除外**（走最大生命比例）。
- 不要预先乘品质系数：英雄专属技能的品质/耗蓝/法系修正由 `PostModify` 统一写回（见第五节）。

### 2.5 Buff 列

| 列 | 类型 | 配置方法 |
| --- | --- | --- |
| `BuffId` | string | 填 `BuffConfig.NameS` **短名**（不是 Id）；空 = 无 Buff；多个用 `,`/`，`/`;`/`；` 分隔 |
| `BuffTime` | float | Buff 持续秒数；常规 1~10 秒。**永久类效果不要靠超大 BuffTime 模拟**，用脚本直接改属性（`SkillBondKillGain` / `SkillComBuffSingle.permanent`） |

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
| `AreaEffect` | string | 范围特效，按 `Area / EffectManager.AreaEffectBaseRadius(15)` 缩放；空 = 不播 |
| `Icon` | string | 技能图标资源名（`SkillPic/`） |

### 2.9 连锁与光环列

| 列 | 类型 | 配置方法 |
| --- | --- | --- |
| `LinkSelf` | string | 只给**连接英雄本人**的加成串（`atk+0.2,armor+0.3` 或 `mpRegen+5`，mpRegen 为百分比） |
| `LinkTeam` | string | 给我方其他英雄（不含本人）的加成，**配置即总量，不乘人数** |

- 子字段可被描述引用：`/linkself-atk`、`/linkteam-mpRegen`。
- 连锁数值改动只改配置，禁止写进 `JobLinkManager` 等代码；机制档位常量放 `CombatConst`。

### 2.10 技能模板 / 组件（Template / Modules）

两张表：`SkillTemplateConfig_s.cs`（模板，标准形态 + 主数值）、`SkillTemplateComponentConfig_s.cs`（组件，机制与形态修饰）。引用一律用**名字**，不用 Id。

- `SkillConfig.Template` = 模板名（对应 `SkillTemplateConfig.Name`，空=不用模板）；`SkillConfig.Modules` = 组件名数组（对应 `SkillTemplateComponentConfig.Name`，空=用模板默认 `Modules`）。基于模板时其余效果列可留空。

**模板 ↔ 脚本一一绑定（硬规则）**：绑定关系写在模板表 `Script` 列（技能类名），配表工具切换模板时按该列回写 `SkillConfig.ScriptName`；脚本不在绑定内的一律按**特化**处理（不套模板、保留原脚本与原数值）：

| 模板 | 唯一脚本 |
| --- | --- |
| 单体伤害大 / 中 / 小 | `SkillComDamageSingle` |
| 群体伤害近距离大 / 中 / 小、群体伤害自身中心大 / 中 / 小 | `SkillComDamageArea` |
| 单体友方增益 / 群体友方增益 / 群体友方增益-目标小 | `SkillComBuffArea` |
| 自身强化 | `SkillComBuffSingle` |
| 单体治疗大 / 中 / 小 | `SkillComHealSingle` |
| 范围治疗大 / 中 / 小 | `SkillComHealArea` |
| 持续治疗大 / 中 / 小 | `SkillComHealAreaCasting` |
| 持续伤害大 / 中 / 小 | `SkillComDamageHitRegion` |
| 光环中 | `SkillComBuffAura` |
| 冲击波伤害大 / 中 / 小 | `SkillComDamageShockWave` |
| 跳跃弹射-伤害大 / 中 / 小 | `SkillComDamageJumpChain` |
| 跳跃弹射-治疗大 / 中 / 小 | `SkillComHealJumpChain` |
| 群体嘲讽 | `SkillComDamageTaunt` |

**归模板 vs 归组件（判定顺序）**

| 内容 | 归属 | 说明 |
| --- | --- | --- |
| 伤害档位 / 治疗量 / 护盾量等主数值 | **模板 `Values`** | 模板 `Values` 就是 Lv1~5 主数值，**不要再建“伤害档位/治疗量”组件** |
| 控制/减益（减速、减防、减攻、减疗、增伤、眩晕、定身、混乱） | **组件** | 只管效果本身，**不写作用范围**；单控 = 单体伤害模板 + 眩晕/定身/混乱，群控 = 群体模板 + 同批控制组件，**不建“单体控制”模板** |
| 属性增益（加攻/加护甲/加法强/加魔抗/加攻速/加移速） | **组件** | 每条属性一个组件，可挂 `光环中`、`自身强化`、`单体治疗`、`范围治疗`；`光环` 只是模板（只保留一档中半径），不建“光环”组件 |
| 施法射程 / 施放中心（目标中心、自身中心） | **模板 `Range` 默认 40** | 施放中心是形态，按“目标中心 AOE / 自身中心 AOE”分模板；需要更远/更近时用 `射程小` / `射程中` / `射程大` 组件**加差值**（+20/+40/+70，默认 40 → 60/80/110） |
| AoE 半径 | **模板 `Area`（默认小范围=20）** | **模板只保留最小半径档**（光环中=40、群体友方增益-目标小=25）；扩大范围一律用 `小范围` / `中范围` / `大范围` 组件加差值（+15/+30/+50，默认 20 → 35/50/70），不再新建“中/大范围”模板。光环因需覆盖全队，固定用 `光环中`（半径 40、`TargetCount=0` 不限人数） |
| 目标筛选、特殊机制 | 组件 | 组件表**只服务模板技能**：特化脚本专用的机制（连击/溅射/穿透/爆炸/召唤克隆等）直接写进脚本，不入组件表 |
| 持续伤害（败 DOT） | **buff 组件** | 走伤害类 buff「败」（`StrengthBuff1=dot:N`），组件「持续伤害大/中/小」 |
| 中毒（毒 DOT） | **buff 组件** | 走伤害类 buff「毒」（`StrengthBuff1=dot:N`），组件「中毒」（1100，单形态，数值贴合低伤毒箭） |
| 永久成长（每施放一次叠自身属性） | **组件（可挂任意模板）** | 现有组件 9050「永久成长攻速」，Effect 固定 `TargetType:permanent;Strength2:selfAtkSpeed=...`，`Templates` 留空表示三大类通用；运行期由 `Skill.ApplyPermanentGrowth` 在施放成功后统一叠加，不依赖具体脚本。其他属性（selfAtk/selfAp/selfArmor/selfMagicRes/selfHp/selfMoveSpeed/selfRange）脚本已支持，有用到的技能时再按同样格式建组件 |
| 持续施法、持续区域、落点持续场（引导/落点区域/召唤投放） | 特化类，默认不建模板也不建组件；**已模板化的持续场例外**：用 `持续伤害大/中/小`/`持续治疗大/中/小` 模板承载 `SummonTime`/`SummonHitInterval` 与单次伤害/治疗；召唤物标签、范围仍由技能行配置 | 依赖专用脚本形态；持续参数放模板比放组件更好维护 |
| 跳跃弹射（多目标间跳跃结算、每跳递减） | **模板（伤害/治疗分开）** | 是形态而非插件，不建“弹射”组件；输出端用 `跳跃弹射-伤害`，治疗端用 `跳跃弹射-治疗` |
| 冲击波（导弹沿路径命中多名敌人） | **模板** | 固定形态，用 `冲击波伤害`；击退/减速等用组件或技能行 `Strength2`/`BuffId` 叠加 |
| MP消耗、CD、概率、触发条件 | 组件（施放节奏） | 作为频率修正项，不与单次效果成本混为一项 |

**模板集合（当前 38 条）**：伤害段——单体伤害大 / 中 / 小、群体伤害近距离大 / 中 / 小（目标中心 AOE）、群体伤害自身中心大 / 中 / 小（自身中心 AOE）、跳跃弹射-伤害大 / 中 / 小、冲击波伤害大 / 中 / 小、持续伤害大 / 中 / 小（落点持续伤害场）；治疗段——单体治疗大 / 中 / 小、范围治疗大 / 中 / 小、跳跃弹射-治疗大 / 中 / 小、持续治疗大 / 中 / 小（落点持续治疗场）；强化/减益段——自身强化 / 光环中 / 单体友方增益 / 群体友方增益 / 群体友方增益-目标小 / 单体减益 / 群体减益（`SelectOnly=enemy`，不造成伤害）/ 群体嘲讽（自身中心嘲讽 + 范围伤害，可附自身减伤）。**伤害/治疗段每个形态固定拆 大 / 中 / 小 三档**：大=形态基准值，中=大×80%，小=大×60%，评分 10/8/6 等比递减（跳跃弹射、范围治疗等原高评分档按 0.8/0.6 折算并取 0.5 整数倍）。**治疗档位值 = 同类型伤害 ×1.25**（单体治疗←单体伤害、范围治疗←群体伤害近距离/自身中心、跳跃弹射-治疗←跳跃弹射-伤害）；**持续伤害/持续治疗的单次值 = 同档直接值的 30%，四舍五入取整**（持续伤害←同档群体伤害、持续治疗←同档范围治疗，等价于同档持续伤害 ×1.25）。形态内的范围/射程/目标数差异仍靠组件叠加，不再为半径新增模板。**跳跃弹射主数值 = 同档常规群体伤害/治疗 ×1.1**（补偿逐跳衰减），衰减系数写在模板 `Effect` 的 `Strength2:jumpDecay=0.85`，可用组件 `无衰减`(1，正价) / `衰减中`(0.7，负价) / `衰减大`(0.5，负价) 覆盖。**3000 段强化增益模板只计形态分（Score 2~5）**，实际价值由所挂 Buff 组件承担，避免模板与组件对同一 Buff 重复计价；`群体友方增益-自身小` 与 `群体友方增益` 同为自身中心、仅半径 20/25 之差，已删除。

**模板字段口径**：`Script`=唯一绑定的技能类名（配表工具据此联动 `SkillConfig.ScriptName`）；`Effect`=效果 k-v（单字段承载“写哪列 + 五级数值”）；`Quality`=品质 1~4（紧邻 `Score` 之前，配表按品质染色：1 `#666666`、2 `#33CC33`、3 `#3333FF`、4 `#FFD700`）；`Score`=标准态总分（标准态=10 分，降档按数值比例下调）；`Range`=射程（默认 40，0=自身中心/全图等无施法距离形态）；`Area`=范围半径（默认小范围 20，0=非范围技能）；`TargetCount`=标准态目标数（**群体默认 3，单体/自身 = 1**，生成 SkillBase 时写入 `TargetCount` 列）；`MpCost`/`CD`=标准态节奏。组件同样带 `Quality`(1~4，置于 `Score` 前)，`Effect`/`Score` 为单项值，与模板分累加时注意不要对同一项重复计价。

**组件 `Templates` 口径（大类 + 形态白名单）**：
- **三大类**：`Damage`（伤害）/ `Heal`（治疗）/ `Buff`（增益，含减益、控制等一切走 Buff 的效果）；`null`/空数组 = 通用，三大类全部适用。工具按模板 `Script` 类名含 `Damage`/`Heal`/`Buff` 推断大类后比对，不匹配只给告警。
- **形态白名单**：只对某几种技能形态生效的机制组件，直接列**模板名**（`弹道` 只列单体/群体 9 个模板 —— 跳跃弹射/冲击波/持续伤害场本身已有飞行或落点表现；`自损` 只列单体 3 个 —— 按“每次施放自伤一次”实现，多目标形态会变成按目标数多次支付；`无衰减`/`衰减中`/`衰减大` 只列跳跃弹射 6 个；`持续场移动` 只列持续伤害 3 个；`周围友军加盾` 只列群体嘲讽）；当前模板名不在列表内时工具告警“只适用于指定形态模板”。
- **伤害修正类组件统一为 `Damage` 大类**：`斩杀`/`护盾加成`/`残血增伤`/`兵种克制`/`高血增伤`/`燃烧法力`/`法力转移`/`驱散`/`概率双击`/`破盾`/`位移`/`暴击率加成` 在单体、群体、跳跃弹射、冲击波、持续伤害场五条形态上都走统一伤害路径（`ResolveDamage` 或导弹命中回调），因此写 `Damage` 即可，不再逐个列模板名。
- 组件表**只服务模板技能**：任何模板技能都用不上的机制不建组件，直接在特化脚本里实现（连击/溅射/穿透/爆炸/召唤克隆）。
- 新增组件时：跨类通用留空 → 只属某大类写大类 → 只在部分形态生效写模板名。

**纯减益技能**：模板 `单体减益`/`群体减益`（`SelectOnly=enemy`、不造成伤害，Type 分别映射 `单控`/`群控`）属于 Buff 大类；状态效果组件 `减速`/`减防`/`减攻`/`减疗`/`增伤`/`眩晕`/`定身`/`混乱`/`叛逃`/`中毒`/`持续伤害` 的 `Templates` 均已含 `Buff`，`连锁扩散` 限定两个减益模板，可直接给纯减益技能选用；依赖伤害结算的形态专属机制（`弹道`/`自损`/`持续场移动`）仍按实际实现限定模板白名单。

**Effect 单字段 k-v（模板 / 组件同构，紧邻 `Modules`/`Templates` 之后）**：一个字段同时表达“写入目标 + 五级数值 + Buff 时长”，替代原来的 `Values` + `EffectKey` + `ValueScale` + `BuffTime` 四列。

- 语法：分号分隔多条绑定，单条为 `Column[:sub][=v1,v2,v3,v4,v5]`；`None`/空 = 只计分不写列。数值是**写入后的最终值**，不再有换算倍率（百分比直接写小数）。单值绑定（如 `BuffTime=4`）在 Lv2~5 沿用同一值。**Buff 属性数值统一只填一级（取现 Lv3 值），不写 5 级数组**，例如 `Buff:速:moveSpeed=0.07`、`Buff:缓:slow=0.18;Buff:缓:atkSpeed=0.18`，展开后 5 级同值。
- 差值绑定：列名后加 `+` / `-` 表示**在模板已写入值上叠加/扣减**（如 `Range+=20`、`Area+=30`、`TargetCount+=2`、`MpCost-=4`），不加符号仍是绝对值覆盖。`MpCost` 差值按表内单位×10（`MpCost+=2` = MP 消耗 +20）；其余列差值按列原单位。射程/范围/目标数/MP 这类调整类组件一律用差值。
- 例：`DamageStrength=70,105,175,280,420`、`Area=30`、`SelectFirst:lowestHp`、`SelectOnly:hero`、`Buff:缓:slow=0.18;Buff:缓:atkSpeed=0.18;BuffTime=3`、`BuffTime=1;Buff:乱`、`Buff:疫;Strength2:healDown=0.24;BuffTime=4`。
- 支持的 `Column`：`DamageStrength` / `HealStrength` / `CD` / `MpCost` / `Range` / `Area` / `TargetCount` / `Rate` / `BuffTime` / `SummonCount` / `SummonTime` / `SummonHitInterval` / `SummonSpeed` / `SummonTag:标签`（仅当技能行为空时写入，标签本身仍按技能手配）/ `SelectOnly:规则` / `SelectFirst:规则` / `TargetType:标记` / `TriggerCondition:属性<`（数值即阈值，如 `TriggerCondition:hprate<=70,50,30,20,15`）/ `Strength2:键`（`shield`、`healDown`、`splashRate`、`pierceRate`、`critRateBonus`、`explodeRate`、`reflect`、`moveDist`、`selfHurt`、`mpBurn`、`doubleChance`、`shieldBonusRate`、`vsJob`、`jumpDecay` 等）/ `Buff:短名[:槽键]`（短名取 `BuffConfig.NameS`；只写 `Buff:短名` 表示只加 buff 不给值）/ `BuffTimeRate=倍率` / `BuffEffectRate=倍率`。
- 值可以是数字（逐级）也可以是字面串，如 `Strength2:vsJob=盾|士`；字面串绑定不做数值校验，按手配优先写入。
- 槽位归属看脚本实际读哪一列：绝大多数 Buff 读 `StrengthBuff1`（如 `缓`/`破`/`慑`/`攻`/`快`），**`疫`（减疗）与 `盾`（护盾）读 `Strength2`**（`healDown`/`shield`，比例值或护盾点数），所以「减疗」写成 `Buff:疫;Strength2:healDown=0.24`、「护盾」写成 `Buff:盾;Strength2:shield=90`，避免同一机制既加 buff 又加数值造成重复生效。
- `BuffTime` 绑定：组件提供 Buff 时同一条 `Effect` 里写 `BuffTime=秒数`；**所有状态效果组件的 BuffTime 统一填单值（取现 Lv3 值），控制类同样固定**（如 `BuffTime=1`、`BuffTime=0.7`），不再写逐级数组。同一技能多个来源时长不同时取**最大值**并告警，再乘 `BuffTimeRate`（延长组件，多个取最大值）；没有 `BuffTime` 绑定时沿用技能行手配值。
- Buff 效果字段不再由组件声明：`BuffEffectRate=1.3/1.5`（效果提升 30%/50%）直接从 `BuffConfig.BuffAttrDynamic` 读取该技能引用的 Buff 所需动态槽键并乘算（如 `攻`→`atk`、`缓`→`slow,atkSpeed`、`守`→`armor`，`perStack/maxStack` 属 `BuffAttrConst` 不参与），多个提升组件取最大值；只作用于组件写入值（手配同名键不参与乘算）。`BuffTimeRate`/`BuffEffectRate` 只影响模板展开，不写入 SkillConfig 列。
- Buff 数值品质加成：`BuffConfig.IsQualityBonus=true` 的 Buff 在运行时按**施加者英雄品质**乘 `CombatConst.QualityBuffFactors`（Q1~Q4 = ×1/×1.25/×1.5/×2），读取入口统一在 `Buff.GetStrBuff`/`GetStr2`，模板展开表不用改；**仅英雄专属技能（施法技能的 Sname 属于某个 `HeroConfig.Skill1`）施加的 Buff 才吃该系数**，职业/连接/道具技能（如 `国` 护盾、`冲` 增伤、`玄盾` 减伤盾）不参与。`BuffConfig.IsApBonus=true` 的 Buff 额外按施法者法强乘 `(100+ap)/100`（同样仅英雄专属技能、仅 `BuffAttrDynamic` 键；`ap`/`evade`/`critRate` 黑名单除外），与品质系数叠乘；`术`/`闪`/`暴`（ap/evade/critRate）在表中直接勾 false，新增 Buff 时按“是否受品质加成”同步勾选 `IsApBonus`。比率/层数类机制（焰/链/锁/箭/震/守）保持 false，只给直接数值属性类（攻/甲/抗/术/速/快/暴/血/蓝/闪/吸/反/盾/愈/伤/慑/败/疫/缓/破/毒/望/兼/重/硬）勾 true。

**Id 分段约定**：模板表按 **1000 段分大类**（1000 伤害、2000 治疗、3000 强化增益、4000 其他预留），段内仍以 **10 为步长**、同类相邻，子项用 +0~+2 表示 大/中/小（段首=大）。当前模板：**1000 段 伤害**——1000/1001/1002 单体伤害大/中/小、1010~1012 群体伤害近距离大/中/小（目标中心）、1020~1022 群体伤害自身中心大/中/小、1030~1032 跳跃弹射-伤害大/中/小、1040~1042 冲击波伤害大/中/小、1050~1052 持续伤害大/中/小；**2000 段 治疗**——2000~2002 单体治疗大/中/小、2010~2012 范围治疗大/中/小、2020~2022 跳跃弹射-治疗大/中/小、2030~2032 持续治疗大/中/小；**3000 段 强化增益**——3000 自身强化、3010 光环中（半径 40、`TargetCount=0` 不限人数、评分 2）、3020 单体友方增益、3030 群体友方增益、3032 群体友方增益-目标小、3050 群体嘲讽（自身中心、`Area=20`、`TargetCount=3` 默认伤害最多 3 人，可用目标增小/增大提升到 5/8、伤害 40/60/90/150/240）；**4000 段 其他**预留。组件表维持原分段：`30` 射程小（+20）/ `31` 射程中（+40）/ `32` 射程大（+70）、`40` 目标增小（+2）/ `41` 目标增大（+5）（以上一个概念占一段开头，不合并同类）、`50~59` 范围档（小=50 +15、中=51 +30、大=52 +50）、`61/62` 初始法力 50%/100%（`StartMpRate=0.5/1`）、`90` 段起：`91` 无衰减 / `92` 衰减中 / `93` 衰减大（跳跃链 jumpDecay）、`94~99` 空，持续伤害改走 buff、区域特化、`100~119` 预留、`120~125` 施放节奏（MP消耗 120~122 增加 +2/+4/+6、123~125 节省 -2/-4/-6，概率 140~142，光环耗蓝倍率 150~154 = 1.5/2/2.5/3/4x；CD 组件已删除，节奏差异改由模板 CD/MP 承担）、**增伤判定固定 `4000~4999`**（4000 斩杀、4010 残血增伤、4020 高血增伤、4030 兵种克制、4040 按护盾加成伤害、4060 暴击率加成、4070 概率双击、4080 随机增伤、4090 孤立增伤、4100 有盾增伤、4110 满血增伤、4120 逐次施法增伤，其余预留）；**士兵固定 `8000~8999`**（8000 士兵士气、8010 士兵增伤、8020 士兵回蓝、8030 周围友军加盾，其余预留）；**触发条件固定 `5000~5999`**（当前无预设组件，已删除未使用的阈值预设；需要时按条件键新建，key 必须来自 `Chess.GetAttr`：ap/atk/might/hp/hprate）；**目标筛选固定 `3000~3999`**（按 10 步长排列，3000 低血筛选起）；**负面/控制 buff 固定 `1000~1999`**（1002 减速=缓、1012 减防=破、1022 减攻=慑、1032 减疗=疫、1042 增伤=伤、1052 眩晕=乱、1062 定身=停、1072 混乱=惑、1090 自身易伤、**1100 中毒单形态**、1110 叛逃单形态、1120 连锁扩散单形态；每组同样不再分大/中/小，只保留原小档并去掉后缀）：数值型负面（减速/减防/减攻/减疗/增伤）的强度用 `效果提升30/50` 放大，控制型（眩晕/定身/混乱）用 `时长延长小/中` 拉长；控制型的时长同样填单值（取 Lv3），需要随等级延长时叠加 `2305 时长逐级延长20`；**正面 buff 固定 `2000~2999`**（2170 伤害反射、2180 生命链接、2190 受击叠防、2200 焰衣、2210 威震、2132 法术暴击为单形态特例），属性 buff **不再拆大/中/小**：每种只保留一档基础组件（取原小档数值、名称去掉“小”后缀，如 `加攻`/`加护甲`/`加攻速`/`护盾`），强度用 `2310 效果提升30`（`BuffEffectRate=1.3`）/ `2311 效果提升50`（`BuffEffectRate=1.5`）/ `2910 效果逐级提升20`（`BuffEffectRate=1.2,1.4,1.6,1.8,2`，Lv1~5 每级 +20%）乘算，时长用 `2300 时长延长小`（`BuffTimeRate=1.3`）/ `2301 时长延长中`（`BuffTimeRate=1.5`）/ `2305 时长逐级延长20`（`BuffTimeRate=1.2,1.4,1.6,1.8,2`，Lv1~5 每级 +20%）乘算；所有状态组件的 `BuffTime` 填单值（取 Lv3）；**属性增益必须引用具体属性组件**（如 `加攻`、`加护甲`），不存在“属性增益大/中/小”这类通用占位（已删除），原引用大/中档的技能行已统一折成「基础组件 + 效果提升30/50」；**特殊机制固定 `9000~9999`**（9010 驱散、9020 破盾、9030 位移、9050 永久成长攻速、9070 弹道 `TargetType:missile;SummonSpeed=20`、9081 自损、9082 燃烧法力、9083 法力转移、9190 持续场移动）。

**6000 段：伤害组·技能折射**（新增）：`6000` 弹射 / `6001` 三连 / `6010~6012` 范围 / `6090` 折射飞弹 / `6100~6102` 增幅。语义：主伤害落地后以主目标为中心、按 `refractRadius` 选 `refractCount` 名敌人（排除主目标），各造成 `GetSkillDamage()` × (`refractRate` + `refractAmp`，加算) 的派生伤害（`Refract` 标签，不再二次派生）；每次施放只折射一次；配 `TargetType:refractMissile` 时以主目标位置为起点发射飞弹，速度取 `refractSpeed`（默认 20），命中再结算。实现：`Skill.TryRefract`，直伤由 `SkillComDamageSingle.ResolveHit` 触发，导弹在命中回调触发。

**一致性自检**：模板 `Modules` 里的每个组件名必须存在于组件表；组件 `Templates` 只能是 `Damage`/`Heal`/`Buff`、模板名之一，或留空（通用）。

### 2.11 SkillBaseConfig（模板展开表）

`SkillBaseConfig_s.cs` 与 `SkillConfig` **列结构完全一致（43 列）**，存放「模板 + 组件」展开后的效果列；`ConfigManager.Init()` 依次 `SkillConfig.Load()` / `SkillBaseConfig.Load()`，并在 `PostModify()` 开头调用 `ApplySkillBase()`：

- **触发条件**：`SkillConfig.Template` 非空即为模板化技能；同 Id 缺失 `SkillBaseConfig` 行时记 `GameLog.Warn` 并沿用原值。
- **覆盖列（23 列，仅模板/组件能改写的效果列）**：`Rate CD MpCost StartMpRate AuraMpRate TriggerCondition Range Area SelectOnly SelectFirst TargetType TargetCount DamageStrength HealStrength Strength2 StrengthBuff1 BuffId BuffTime SummonTag SummonCount SummonTime SummonHitInterval SummonSpeed`。身份列与表现列（`ScriptName Action SelfEffect HitEffect AreaEffect Icon LinkSelf LinkTeam DamageType HurtTag ItemId`）**不覆盖**，一律以 SkillConfig 为准；`StartMpRate` 仅当 SkillBase 值 >0（组件显式配置，如 61/62 初始法力）时覆盖，否则保留 SkillConfig 手配值；需要让某项参与覆盖时，把对应 key 加进工具的 `EffectColumns` 白名单并同步此列表。
- **保留列（9 列身份列）**：`Id Name Sname Version Descript Type Template Modules Lv`。
- **展开规则（k-v 映射）**：模板先按 `Effect` 的绑定把 `Lv` 对应数值写入目标列/槽（伤害模板=`DamageStrength`、治疗模板=`HealStrength`，两者互斥清零）；随后 `Range`/`Area`/`TargetCount` 仅在模板值 >0 时覆盖（Range 默认 40、Area 默认小范围 20、TargetCount 群体 3/单体 1），`CD` 取模板，`MpCost` = 模板 `MpCost × 10`；再按**技能自己 `Modules`** 顺序展开每个组件的 `Effect`（列写入 / `Buff:短名:槽键` 追加 `BuffId`+`StrengthBuff1` / `TargetType:标记` / `TriggerCondition` / `Strength2:键`）。`BuffId`/`StrengthBuff1`/`Strength2`/`TargetType` 已手配的键不覆盖，只补缺失项。`BuffTime` 取模板与各组件提供值的最大值，再乘 `BuffTimeRate`（多个取最大值）；`BuffEffectRate` 对 `BuffConfig.BuffAttrDynamic` 的槽键数值乘算（手配键不参与，`BuffAttrConst` 不参与）。模板默认 `Modules` 只参与估值与推荐校验，**不注入任何效果**（当前所有模板 `Modules` 均为空，需要组件请配到技能行，不要放模板）。
- **可被组件写入的列**：`EffectColumns` = `DamageStrength HealStrength CD MpCost StartMpRate AuraMpRate Range Area TargetCount Rate BuffTime BuffTimeRate BuffEffectRate SelectOnly SelectFirst TargetType TriggerCondition Strength2 Buff SummonCount SummonTime SummonHitInterval SummonSpeed SummonTag`。其中 `BuffTimeRate` 乘最终 `BuffTime`，`BuffEffectRate` 对 `BuffConfig.BuffAttrDynamic` 的键乘算（两者只参与展开，不落到技能表列）。目标筛选类组件（`3000~3999`）写 `SelectOnly:`/`SelectFirst:`，行为标记才写 `TargetType:`（如 `3090 施法者必含`→`TargetType:selfAlways`）。
- **现状覆盖**：品质 1~4 英雄技能 152 个（Q1 26 / Q2 54 / Q3 47 / Q4 25），模板化 130 个，22 个特化不模板化。Q4 共 14 个专属脚本：曹操·SkillAidDamageAreaTeamBuff、赵云·SkillAidDamageChainBlink、张飞·SkillAidSpinAttack、马超·SkillAidDamageLineCharge、关羽·SkillAidDamageSingleSummon、姜维·SkillAidDamageLineLaser、郭嘉·SkillAidDamageCastingLine、典韦·SkillAidDamageAreaFall、甘宁·SkillAidDamageSingleDashStealth、太史慈·SkillAidDamageLineVolley、周瑜·SkillAidDamageAreaWall、陆逊·SkillAidDamageAreaField、吕布·SkillAidDamageCastingDrain、邓艾·SkillAidDamageSingleJump。Q1~3 共 8 个：孙权·SkillAidSummonSingleClone、张昭·SkillAidSoldierFortify、孙尚香·SkillAidDamageShotBackstep、陈宫·SkillAidDamageCastingFrontal、蒋琬·SkillComHealArea（手配群疗，Template 留空）、吕蒙·SkillAidDamageSingleDashBuff、司马炎/陆抗·SkillAidBuffAreaRangeSplit。
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
3. **以基准技能对照**：单体法术基准（模板「单体伤害中」Lv1 `DamageStrength=60`），逐级递增；群体用较低基值 + 多目标。
4. 数值只进配置，逻辑里不写死；调平衡只改 `SkillConfig`。
5. 同一 Sname 五行的 `Range/Area/SelectOnly/SelectFirst/TargetCount` 语义必须一致。

## 七、Buff / 特效联动

- BuffId 填短名；组合按段位顺序读 `StrengthBuff1`（小键盘如 `攻,甲,抗` ↔ `atk,armor,magicRes`）。
- 特效名必须是 `EffectConfig.Name` 中存在的名字（见 `combat-effect-usage` 技能）。
- 范围技能的 `AreaEffect` 随 `Area` 缩放；普攻溅射类（`SkillBondHitArea`）只播主目标 `HitEffect`。

## 八、优先复用通用脚本

| 脚本 | 形态（配置行数） | 配置要点 |
| --- | --- | --- |
| `SkillComDamageSingle` | 单体伤害（125 行，最常用） | **单体伤害体系基类**：`DamageStrength` + `BuffId`（空=纯伤害），固定流程「选人 → 发动判定 → 施法表现 → 伤害结算 → 命中后处理」，派生类只覆写钩子（`SelectTarget`/`CanCast`/`PlayCast`/`CalcDamage`/`ResolveHit`/`ApplyDamage`/`AfterDamage`）即可继承全部通用组件。体系内派生：`SkillComDamageArea`、`SkillComDamageShockWave`、`SkillComDamageJumpChain`、`SkillComDamageHitRegion`（各自覆写 `CheckAidSkill`，继承通用能力）。注意：一次施放只结算 1 段，多段必须另写派生脚本 |
| `SkillComDamageArea` | 范围伤害 + 可选 Buff（60 行） | `SelectOnly=enemy`；`TargetType` 空=目标中心 / `self`=自身中心；`BuffId` 可 0~N 段（未配置即纯伤害） |
| `SkillComBuffSingle` | 单体增益（90 行，缺省自身） | **单体增益体系基类**：`BuffId` 挂 1~N 个 Buff，数值放 `Strength2`/`StrengthBuff1` 由 Buff 读。目标由 `SelectOnly`/`SelectFirst` 在 `Area` 内选（最多 `TargetCount` 名），**留空则默认给自己**；所以既能当"自身强化"，也能配 `ally;hero` 给友军加 Buff。钩子：`CanStart`/`SelectBuffTargets`/`CanCast`/`PlayCast`/`ApplyBuffs`；体系内派生：`SkillComBuffArea`、`SkillComBuffAura`、`SkillComDamageTaunt`。**反伤也走这里**：`BuffId=反`、`StrengthBuff1=reflect:比例`、`BuffTime=持续时间`，由 `BuffReflect` 在 Buff 期间结算；连接授予的常驻反伤（如 `刺`）加 `TargetType=noStack`，避免 CD 0 时每帧重放 |
| `SkillComBuffArea` | 群体增益（45 行） | `SelectOnly=ally;...` + `SelectFirst` 选人（见 2.3）；护盾类按统一护盾公式写 `Strength2[shield]`。施放中心分两种模板：`群体友方增益`（自身中心，Area 20）、`-目标小`（`TargetType:targetCenter`，Area 25）；要更大半径加 `中范围`/`大范围` 组件。近远分段（司马炎·一统 / 陆抗·西陵）走专属脚本 `SkillAidBuffAreaRangeSplit`，不再作为通用组件 |
| `SkillComBuffAura` | 光环（40 行） | 模板只有 `光环中`（半径 40、`TargetCount=0` 不限人数、评分 2），需要更大范围加 `中范围`/`大范围` 组件。每 1.4 秒对 `Area` 内按 `SelectOnly`/`SelectFirst` 选中的单位（可含敌方）挂 1.5 秒 `BuffId`（固定时长，`BuffTime` 列不参与）；`MpCost=0` 常开；`MpCost>0` 时 MP 满且 CD 就绪后通过施法开启（施法不清空 MP，MP 作为光环燃料），开启后每秒扣 `1×AuraMpRate` 点 MP（默认 20，组件 150~154 可调 15/20/25/30/40）、期间攻击/受击回蓝照常累计，MP<=0 自动关闭并移除 `SelfEffect`，再次开启需 MP 充满后重新施法 |
| `SkillComHealArea` | 范围治疗（25 行） | `HealStrength` + `Area`；单目标结算走基类 `HealUnit`，`overflowToArmor` 等组件自动生效 |
| `SkillComHealSingle` | 单体治疗（15 行） | **单体治疗体系基类**：只治 1 名友军（`Area` 内按 `SelectFirst`，典型 `lowestHp`），默认净化并挂 `BuffId`（支持多段/护盾），`HitEffect` 打在目标身上；钩子 `SelectTarget`/`CanCast`/`PlayCast`/`ResolveHeal`/`CleanseOnHeal`；体系内派生：`SkillComHealArea`、`SkillComHealAreaCasting`、`SkillComHealJumpChain` |
| `SkillComDamageJumpChain` | 伤害弹射（5 行） | 敌方跳跃伤害链；每跳衰减读 `Strength2[jumpDecay]`（模板默认 0.85，组件可改 1/0.7/0.5），命中可带 `BuffId` 减益 |
| `SkillComHealJumpChain` | 治疗弹射（5 行） | 友军跳跃治疗链；同样读 `Strength2[jumpDecay]`，固定选 `ally;hero;notFullHp + lowestHp` 语义 |
| `SkillBondBacklineStrike` | 打后排（15 行） | `smart` 智能选后排 / 默认最远 |
| `SkillAidHealSingleLowHp` / `SkillBondDefReduceDamageRate` | 低血触发（5 行各） | `TriggerCondition=hprate<50` + `Strength2[reduceRate]` |
| `SkillDumb` | 占位（65 行） | 无战斗行为，仅用于职业/道具等纯展示行；**不要给它配伤害字段** |

**组件语义只在体系基类结算（强制）**：`TargetType` 标记与 `Strength2` 键的读取一律写在 3 个体系基类里，派生类（含 `SkillComDamageShockWave` / `SkillComBuffArea` 等）不得再写 `HasSkillFlag` / `GetStr2`，否则同一组件在不同形态下表现不一致。当前归属：

| 体系基类 | 统一结算的组件语义 |
| --- | --- |
| `SkillComDamageSingle` | `self`（自身中心）/ `missile`（飞弹表现+飞行速度取 SummonSpeed）/ `highHp`·`randomAmp`·`isolatedAmp`·`shieldedAmp`·`fullHp`·`castStack`（增伤修正）/ `execute`·`shieldBonus`·`missingHp`·`vsJob`·`soldierDamage`·`critRateBonus`（伤害修正）/ `antiShield`·`mpBurn`·`pushMin/pushMax`·`selfHurt`·`selfBuff`·`dispel`（命中后处理）/ 命中特效 |
| `SkillComHealSingle` | `overflowToArmor`（溢出转护甲）/ 驱散负面 / 护盾值写入 / 多段 `BuffId` / 命中特效，统一入口 `HealUnit(unit, heal, withBuffs)` |
| `SkillComBuffSingle` | `noStack` / `permanent` / `selfAlways` / `noDedupe` / `targetCenter` / 护盾值写入；近远分段由专属脚本 `SkillAidBuffAreaRangeSplit` 实现 |

新增组件时：先在对应体系基类实现语义，再在组件表登记 `Effect`（`TargetType:xxx` 或 `Strength2:key=...`），派生类只需声明形态（单体/范围/弹射等）。

- 远程技能以自身为中心会全程打空（站位远离敌阵），必须目标中心；近战贴脸 AoE 用 `self`。
- 类名与 `ScriptName` 用英文，不用拼音；表里已有同名类时优先复用，别新建特化类。
- 英雄没有被动技能：受击/反伤/触发类效果不要写 `OnAttacked` 之类的被动脚本，一律改成「主动给自己挂 Buff + Buff 内实现」，如反伤 = `SkillComBuffSingle` + `BuffId=反`（`BuffReflect`）。

## 九、新建脚本要点

- `SkillCom*` 通用脚本一律继承 3 个体系基类（`SkillComDamageSingle` / `SkillComHealSingle` / `SkillComBuffSingle`），只覆写钩子或 `CheckAidSkill`；`SkillAid*` 特化脚本按需直接继承 `Skill`，不受体系约束。
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
