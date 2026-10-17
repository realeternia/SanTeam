# Buff 归属判断规则 & Buff→技能引用统计

> 数据来源：`Assets/Resources/Scripts/Configs/BuffConfig_s.cs`（36 条）、`Assets/Resources/Scripts/Configs/SkillConfig_s.cs`（819 行技能）。
> BuffId 号段：`300xxx` 正面通用 / `3001xx` 正面特化 / `301xxx` 负面通用 / `3011xx` 负面特化。
> 口径：按 `new SkillConfig(...)` 构造参数位解析——第 22 个参数（索引 21）= `BuffId`，第 31 个参数（索引 30）= `ScriptName`。
> 结论可用下面的「判定口径」手工复核，无需跑工具。

---

## 一、Buff 归属判断规则（代码事实）

### 1. 数据结构归属

| 字段 | 含义 | 说明 |
| --- | --- | --- |
| `Buff.owner` | **被挂者（持有者）** | 效果数值、表现都加在它身上 |
| `Buff.caster` | **施加者（归属来源）** | 「谁给的 buff」，用于判定归属方 |
| `Buff.skillCfg` | 效果数值来源 | 由 `skillId` 决定取 SkillConfig 哪一行 |
| `Buff.id` | BuffConfig.Id | 由 `buffId` 决定取哪条 buff 配置 |

构造签名：`Buff(int id, int skillId, Chess caster, Chess unit, float lastTime)`（[Buff.cs](../../Assets/Resources/Scripts/Combat/Buff/Buff.cs#L20-L29)）。

### 2. 施加入口归属

`BuffManager.AddBuff(Chess target, Chess caster, int skillId, int buffId, float time)` → **target 获得 buff，归属来源记作 caster**（[BuffManager.cs](../../Assets/Resources/Scripts/Combat/Buff/BuffManager.cs#L12-L33)）。
按 `BuffConfig.ScriptName` 反射创建对应 `BuffXxx` 子类（约定 ScriptName 即类名）。

### 3. 配置侧归属

- `SkillConfig.BuffId` 是 **string**，填 `BuffConfig.NameS` 短名；**空串 = 无 buff**。
- **支持多段**：用 `,` `，` `;` `；` 分隔，如 `"护,盾"`、`"盾,愈"`、`"威,乱,惑"`。解析统一走 `Skill.GetSkillBuffIds()` / `Skill.GetSkillBuffIds(SkillConfig)`（[Skill.cs](../../Assets/Resources/Scripts/Combat/Skill/Skill.cs#L87-L120)）。
- **段位语义由各技能脚本自行约定**，唯一有书面约定的是 `SkillAidAtkDrain`（马腾·腾驾，`"慑,攻"`）：第 1 段=目标减攻 Buff，第 2 段=自身加攻 Buff。其余多段技能以各脚本内注释为准。

### 4. 正负面 & 驱散归属

| 字段 | 判定 |
| --- | --- |
| `BuffConfig.IsPositive` | true=正面，false=负面 |
| `BuffConfig.CanDispel` | true=可被驱散，false=英雄招牌/兵种 buff 不可驱散 |
| `BuffManager.DispelNegative` | 只清 **IsPositive=false 且 CanDispel=true** |
| `BuffManager.DispelPositive` | 只清 **IsPositive=true 且 CanDispel=true** |

### 5. 存在性归属（用「身上有没有这个 buff」驱动机制）

机制不再另存成员变量，而是**运行时判断某单位身上是否存在某 Buff**：

| 机制 | Buff | 判定位置 |
| --- | --- | --- |
| 嘲讽 | `BuffTaunt` 301101 | `Chess.FindTarget`：`GetBuff(CombatConst.TauntBuffId)` 存在且 `buff.caster` 存活 → 目标强制 = caster |
| 连锁 | `BuffLock` 301102 | 「带着 buff 才传递伤害」 |
| 生命链接 | `BuffLifeLink` 300106 | 「带着 buff 才共享伤害/回复」 |

**索敌优先级**：混乱（`chaosCount>0`）> 嘲讽（有 `BuffTaunt`）> 常规打分索敌（[Chess.cs](../../Assets/Resources/Scripts/Combat/Chess.cs#L422-L442)）。
嘲讽施加入口统一为 `Skill.TauntEnemies()`（[Skill.cs](../../Assets/Resources/Scripts/Combat/Skill/Skill.cs#L442-L469)），时长取技能自身 `BuffTime`。

### 6. 表现归属

`BuffConfig.BuffEffect`（特效）、`ColorStart`+`ColorEnd`（染色）、`Material`（切材质）全部作用在 **owner** 身上，在 `Buff.OnAdd/OnRemove` 中挂/摘，与 caster 无关。

---

## 二、统计总览

| 项 | 数量 |
| --- | --- |
| SkillConfig 技能行 | **819** 行（技能按 `Sname` 展开 Lv1~5，个别道具技能只有 1 行） |
| 配了 `BuffId` 的行 | **418** 行 |
| BuffConfig 条目 | **36** 条 |
| ├ 被配置引用（`SkillConfig.BuffId`） | **32** 条 |
| ├ 仅被代码引用（配置里看不到） | **1** 条：`301101 嘲讽`（`Skill.TauntEnemies`） |
| └ 完全未引用 | **3** 条：`300104 多重箭`、`300025 反伤`、`301005 陷阵` |

> **代码引用的例外**：`301101 嘲讽` 在 `SkillConfig.BuffId` 里查不到（配置引用数 = 0），实际由 [Skill.cs](../../Assets/Resources/Scripts/Combat/Skill/Skill.cs#L459) 的 `CombatConst.TauntBuffId` 直接施加，被 4 个嘲讽技能共用。所以「按配置统计」会漏掉它——判定归属时需人肉排除这一类。

---

## 三、护盾（300001「盾」）明细核对

**结论：15 个技能（Sname 去重）/ 75 行，数量正确**（15 × 5 行 = 75，全部为 Lv1~5）。

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 曹操 | 护驾 | `SkillAidImperialGuard` | 2020006~2020010 | `护,盾`（多段） |
| 程普 | 横扫 | `SkillAidBashShield` | 2020221~2020225 | `盾` |
| 邓艾 | 偷渡阴平 | `SkillLowHpFullHealShield` | 2020726~2020730 | `盾,愈`（多段） |
| 高览 | 御守 | `SkillAidGuardian` | 2020321~2020325 | `盾` |
| 国 | 国家护盾 | `SkillFactionShield` | 2000001~2000005 | `盾`（非武将技能） |
| 护 | 义胆 | `SkillAidBuffLowHp` | 2010061~2010065 | `盾`（兵种技能） |
| 俭 | 温良 | `SkillAidSelfShield` | 2010146~2010150 | `盾`（兵种技能） |
| 蒋琬 | 抚军 | `SkillSoldierShield` | 2020901~2020905 | `盾` |
| 老 | 益壮 | `SkillAttackedShield` | 2010106~2010110 | `盾,愈`（多段，兵种技能） |
| 李严 | 护军 | `SkillAidGuardArmy` | 2020606~2020610 | `盾` |
| 刘禅 | 安乐 | `SkillAidHealShield` | 2021301~2021305 | `盾` |
| 吕蒙 | 埋伏 | `SkillAttackedTeleport` | 2020701~2020705 | `乱,盾`（多段） |
| 严颜 | 老当益壮 | `SkillAttackedShield` | 2020716~2020720 | `盾` |
| 周仓 | 忠勇 | `SkillAidSelfAndLowHp` | 2020706~2020710 | `盾` |
| 周泰 | 不屈 | `SkillAidUnbreakable` | 2021411~2021415 | `盾,甲`（多段） |

**为什么是这个数（易误判点）**

1. **多段 BuffId 也算引用**：曹操 `护,盾`、邓艾/老 `盾,愈`、吕蒙 `乱,盾`、周泰 `盾,甲` 共 5 条技能把「盾」写成多段之一，早期只看「整串等于盾」会漏掉它们，得到偏小的数字。
2. **Sname 不全是武将**：`国`（国家护盾）、`护`（义胆）、`俭`（温良）、`老`（益壮）是羁绊/兵种技能，Sname 是短名而非人名。
3. **同脚本可被多个技能共用**：`SkillAttackedShield` 同时被「老」（兵种）和「严颜」（武将 2020716~）使用，属两个不同技能，各自计数。
4. **计数单位**：本表 15 是「技能（Sname 去重）数」；若按「技能行」计则为 75 行。同 Sname 的 Lv1~5 是同一技能的 5 个等级行，不重复计技能。

---

## 四、全量明细（每个 Buff → 引用它的技能）

单位：`Sname` 去重技能数 / 行数。`Id 段` 为 Lv1~5 的技能 Id 区间。

### 300001 护盾（盾）— 15 技能 / 75 行
见上一节表格。

### 300002 减伤盾（硬）— 9 技能 / 33 行

| Sname | 技能名 | 脚本 | Id 段 | 行数 |
| --- | --- | --- | --- | --- |
| 黄盖 | 苦肉 | `SkillAidSelfSacrifice` | 2020621~2020625 | 5 |
| 乐进 | 御斩 | `SkillAidSlashDefend` | 2020216~2020220 | 5 |
| 司马炎 | 一统 | `SkillAidBuffArea` | 2020021~2020025 | 5 |
| 徐盛 | 疑城 | `SkillAidShieldValue` | 2020711~2020715 | 5 |
| 于禁 | 铁壁 | `SkillAidIronGuard` | 2021516~2021520 | 5 |
| 张梁 | 黄天 | `SkillAidShieldValue` | 2020626~2020630 | 5 |
| 文润 | 文润 | `SkillInitShieldValue` | 2090015 | 1（道具技能） |
| 玄盾 | 玄盾 | `SkillInitShieldValue` | 2090006 | 1（道具技能） |
| 玄武 | 玄武 | `SkillInitShieldValue` | 2090023 | 1（道具技能） |

> 33 = 6×5 + 3×1；3 条道具技能（209xxxx）只有 Lv1 单行，这是「行数 ≠ 技能数×5」的唯一原因。

### 300003 吸血（吸）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 徐晃 | 长驱 | `SkillAidLifestealStrike` | 2020411~2020415 | `重,吸`（多段） |

### 300004 伤害提升（重）— 3 技能 / 15 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 董卓 | 暴虐 | `SkillAidBuffArea` | 2020016~2020020 | `重` |
| 徐晃 | 长驱 | `SkillAidLifestealStrike` | 2020411~2020415 | `重,吸`（多段） |
| 华雄 | 狂暴 | `SkillAidBerserk` | 2020126~2020130 | `重,伤`（多段） |

### 300005 攻速提升（快）— 5 技能 / 25 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 陈泰 | 扬鞭 | `SkillAidHasteStrike` | 2020111~2020115 | `快` |
| 鞠义 | 先锋 | `SkillAidAssault` | 2020426~2020430 | `快` |
| 凌统 | 迅射 | `SkillAidFrenzy` | 2020316~2020320 | `快` |
| 王濬 | 疾击 | `SkillAidFrenzy` | 2020526~2020530 | `快` |
| 小乔 | 鼓舞 | `SkillAidBuffHasteMoveSpeed` | 2021211~2021215 | `快,速`（多段） |

### 300101 据守（守）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 |
| --- | --- | --- | --- |
| 曹仁 | 天人守城 | `SkillAidDefStack` | 2020721~2020725 |

### 300007 急救（愈）— 2 技能 / 10 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 邓艾 | 偷渡阴平 | `SkillLowHpFullHealShield` | 2020726~2020730 | `盾,愈` |
| 老 | 益壮 | `SkillAttackedShield` | 2010106~2010110 | `盾,愈` |

### 300008 攻击提升（攻）— 5 技能 / 25 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 大乔 | 国色 | `SkillAidBuffArea` | 2021221~2021225 | `攻` |
| 马腾 | 腾驾 | `SkillAidAtkDrain` | 2020431~2020435 | `慑,攻` |
| 甄宓 | 洛神 | `SkillAidBuffAtk` | 2021201~2021205 | `攻` |
| 才 | 奇才 | `SkillAidBuffArea` | 2010141~2010145（兵种技能） | `攻,甲,抗`（多段） |
| 袁绍 | 名门 | `SkillAidNobleBless` | 2020026~2020030 | `攻,甲,抗`（多段） |

### 300009 移速提升（速）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 小乔 | 鼓舞 | `SkillAidBuffHasteMoveSpeed` | 2021211~2021215 | `快,速`（多段，第 2 段） |

### 300102 复活（复）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 |
| --- | --- | --- | --- |
| 张昭 | 筑垒 | `SkillSoldierFortify` | 2020911~2020915 |

### 300103 乱阵（威）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 贾诩 | 乱阵 | `SkillSoldierStun` | 2020916~2020920 | `威,乱,惑` |

### 300013 魔抗提升（抗）— 2 技能 / 10 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 才 | 奇才 | `SkillAidBuffArea` | 2010141~2010145（兵种技能） | `攻,甲,抗`（多段，第 3 段） |
| 袁绍 | 名门 | `SkillAidNobleBless` | 2020026~2020030 | `攻,甲,抗`（多段，第 3 段） |

### 300014 生命回复（血）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 刘备 | 仁德 | `SkillAidBenevolence` | 2020001~2020005 | `血,蓝`（多段，第 1 段） |

### 300015 闪避（闪）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 |
| --- | --- | --- | --- |
| 左慈 | 天书 | `SkillAidRangeHeal` | 2021321~2021325 |

### 300105 倍击（倍）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 |
| --- | --- | --- | --- |
| 丁奉 | 奋威 | `SkillAidEmpower` | 2020516~2020520 |

### 300018 法力回复（蓝）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 刘备 | 仁德 | `SkillAidBenevolence` | 2020001~2020005 | `血,蓝`（多段，第 2 段） |

### 301101 嘲讽（嘲）— 0 配置引用 / **4 技能（代码引用）**

| Sname | 技能名 | 脚本 | Id 段 | 备注 |
| --- | --- | --- | --- | --- |
| 李严 | 护军 | `SkillAidGuardArmy` | 2020606~2020610 | 调 `TauntEnemies(owner, lowest.pos, Area)` |
| 庞德 | 抬棺 | `SkillAidTauntSlam` | 2020611~2020615 | 调 `TauntEnemies(Area)` |
| 黄盖 | 苦肉 | `SkillAidSelfSacrifice` | 2020621~2020625 | 调 `TauntEnemies(Area)` |
| 于禁 | 铁壁 | `SkillAidIronGuard` | 2021516~2021520 | 调 `TauntEnemies(Area)` |

> 这 4 个技能的 `SkillConfig.BuffId` 填的是自己的另一条 buff（`盾` / `硬`），嘲讽走代码常量施加，故配置统计为 0。

### 300106 生命链接（链）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 |
| --- | --- | --- | --- |
| 貂蝉 | 离间 | `SkillAidLifeLink` | 2021216~2021220 |

### 300028 护驾（护）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 曹操 | 护驾 | `SkillAidImperialGuard` | 2020006~2020010 | `护,盾` |

### 300107 望族（望）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 |
| --- | --- | --- | --- |
| 族 | 望族 | `SkillInitDecayDef` | 2010131~2010135（兵种技能） |

### 300108 兼资（兼）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 |
| --- | --- | --- | --- |
| 兼 | 将相 | `SkillAttackStackBuffer` | 2010136~2010140（兵种技能） |

### 300032 护甲（甲）— 5 技能 / 25 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 典韦 | 舍身救主 | `SkillAidSacrificeGuard` | 2021606~2021610 | `甲` |
| 李典 | 御阵 | `SkillAidSevenCharge` | 2020601~2020605 | `甲` |
| 周泰 | 不屈 | `SkillAidUnbreakable` | 2021411~2021415 | `盾,甲` |
| 才 | 奇才 | `SkillAidBuffArea` | 2010141~2010145（兵种技能） | `攻,甲,抗`（多段） |
| 袁绍 | 名门 | `SkillAidNobleBless` | 2020026~2020030 | `攻,甲,抗`（多段） |

### 300109 威震（震）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 张辽 | 威震 | `SkillAidVengefulStance` | 2021511~2021515 | `震,乱` |

### 301001 眩晕（乱）— 8 技能 / 40 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 贾诩 | 乱阵 | `SkillSoldierStun` | 2020916~2020920 | `威,乱,惑` |
| 吕布 | 魔神 | `SkillCastingAidDrain` | 2020101~2020105 | `乱` |
| 吕蒙 | 埋伏 | `SkillAttackedTeleport` | 2020701~2020705 | `乱,盾` |
| 马谡 | 守势 | `SkillAidSmashStun` | 2021506~2021510 | `乱` |
| 谋 | 深谋 | `SkillAttackStunDamage` | 2010046~2010050（兵种） | `乱` |
| 枪 | 枪阵 | `SkillHitBuff` | 2000141~2000145（兵种） | `乱` |
| 夏侯惇 | 歼灭 | `SkillAidSmashStun` | 2020211~2020215 | `乱` |
| 张辽 | 威震 | `SkillAidVengefulStance` | 2021511~2021515 | `震,乱` |

### 301102 连锁（锁）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 |
| --- | --- | --- | --- |
| 庞统 | 连锁 | `SkillHitBuffArea` | 2021001~2021005 |

### 301003 增伤（伤）— 6 技能 / 30 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 冲 | 铁骑 | `SkillAttackRunCrossPlus` | 2010006~2010010（兵种技能） | `伤` |
| 奸 | 权奸 | `SkillInitEnemyRandomBuff` | 2010096~2010100（兵种技能） | `伤` |
| 刘晔 | 卸甲 | `SkillAidArmorBreak` | 2021711~2021715 | `伤` |
| 许攸 | 傲言 | `SkillAidArrogantWord` | 2020831~2020835 | `伤` |
| 朱桓 | 标记 | `SkillAidMarkTarget` | 2020116~2020120 | `伤` |
| 华雄 | 狂暴 | `SkillAidBerserk` | 2020126~2020130 | `重,伤`（多段） |

### 301004 减攻（慑）— 4 技能 / 20 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 陈宫 | 绝策 | `SkillAidMarkTarget` | 2021026~2021030 | `慑` |
| 法正 | 明断 | `SkillAidQuietPlot` | 2021006~2021010 | `慑` |
| 马腾 | 腾驾 | `SkillAidAtkDrain` | 2020431~2020435 | `慑,攻` |
| 徐庶 | 困敌 | `SkillAidHexField` | 2020501~2020505 | `慑,疫` |

### 301006 溃败（败）— 4 技能 / 20 行

| Sname | 技能名 | 脚本 | Id 段 |
| --- | --- | --- | --- |
| 程昱 | 断粮 | `SkillAidCutSupply` | 2020816~2020820 |
| 于吉 | 妖咒 | `SkillAidCurseHeal` | 2021311~2021315 |
| 张飞 | 旋风斩 | `SkillAttackSpinAttack` | 2021501~2021505 |
| 张任 | 毒箭 | `SkillAidPoisonArrow` | 2020326~2020330 |

### 301007 减疗（疫）— 5 技能 / 25 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 陈群 | 律令 | `SkillAidSlowStrike` | 2021726~2021730 | `疫` |
| 李儒 | 鸩酒 | `SkillAidPoisonWine` | 2020821~2020825 | `疫` |
| 魏延 | 奇袭 | `SkillAidDiveCleave` | 2021601~2021605 | `疫` |
| 徐庶 | 困敌 | `SkillAidHexField` | 2020501~2020505 | `慑,疫` |
| 钟会 | 蓄谋 | `SkillAidQuietPlot` | 2021021~2021025 | `疫` |

### 301008 减速（缓）— 4 技能 / 20 行

| Sname | 技能名 | 脚本 | Id 段 |
| --- | --- | --- | --- |
| 荀攸 | 奇策 | `SkillAidSweeping` | 2021016~2021020 |
| 张郃 | 震碎 | `SkillAidQuake` | 2020506~2020510 |
| 张松 | 舌辩 | `SkillAidSilverTongue` | 2020806~2020810 |
| 诸葛亮 | 惊雷 | `SkillAidShockWave` | 2021701~2021705 |

### 301009 减防（破）— 9 技能 / 45 行

| Sname | 技能名 | 脚本 | Id 段 |
| --- | --- | --- | --- |
| 曹彰 | 突阵 | `SkillAttackRunCrossPlus` | 2020131~2020135 |
| 甘宁 | 百骑劫营 | `SkillAidRaidCamp` | 2020416~2020420 |
| 马超 | 铁骑冲 | `SkillAidCavalryStrike` | 2020106~2020110 |
| 潘璋 | 偷刃 | `SkillAidStealBlade` | 2021416~2021420 |
| 孙坚 | 破阵 | `SkillAidArmorBreak` | 2021521~2021525 |
| 王双 | 崩山 | `SkillAidArmorShred` | 2021401~2021405 |
| 夏侯渊 | 猎鹰 | `SkillAidArmorShred` | 2020306~2020310 |
| 许褚 | 虎啸 | `SkillAidShockWave` | 2021421~2021425 |
| 张绣 | 枪出如龙 | `SkillAidSweepingAoe` | 2021526~2021530 |

### 301103 叛逃（叛）— 2 技能 / 10 行

| Sname | 技能名 | 脚本 | Id 段 |
| --- | --- | --- | --- |
| 司马懿 | 隐忍 | `SkillAidEnduranceStrike` | 2020811~2020815 |
| 司马昭 | 篡逆 | `SkillAidUsurpPressure` | 2020826~2020830 |

### 301104 混乱（惑）— 1 技能 / 5 行

| Sname | 技能名 | 脚本 | Id 段 | 该行 BuffId |
| --- | --- | --- | --- | --- |
| 贾诩 | 乱阵 | `SkillSoldierStun` | 2020916~2020920 | `威,乱,惑` |

---

## 五、未引用的 Buff（3 条）

| BuffId | 名字 | 短名 | 脚本类 | 说明 |
| --- | --- | --- | --- | --- |
| 300104 | 多重箭 | 箭 | `BuffMultiShot` | 无任何技能引用（`SkillConfig.BuffId` 与代码均无） |
| 300025 | 反伤 | 反 | `BuffReflect` | 同上 |
| 301005 | 陷阵 | 停 | `BuffNoMove` | 同上 |

> 已删：`300009 汲血快攻`、`300034 龙胆`（2026-10-17 清理，两者当时均无引用）；`300019 袭杀`（拆为 `重,吸` 两个通用 Buff）；`300022 狂暴`（拆为 `重,伤` 两个通用 Buff）；`300010 御风疾驰`（拆为 `快,速`）、`300027 仁政`（拆为 `血,蓝`）、`300029 名门`（拆为 `攻,甲,抗`）。`300003 吸血` 由徐晃·长驱的 `重,吸` 恢复引用。
>
> BuffId 号段（2026-10-17 重排）：`300xxx` 正面通用、`3001xx` 正面特化、`301xxx` 负面通用、`3011xx` 负面特化。特化 Buff 已迁入：据守 300101、复活 300102、乱阵 300103、多重箭 300104、倍击 300105、生命链接 300106、望族 300107、兼资 300108、威震 300109；嘲讽 301101、连锁 301102、叛逃 301103、混乱 301104。

---

## 六、多段 BuffId 行清单（判定归属时最易出错的地方）

| 技能 | Sname | 脚本 | 该行 BuffId |
| --- | --- | --- | --- |
| 护驾 | 曹操 | `SkillAidImperialGuard` | `护,盾` |
| 偷渡阴平 | 邓艾 | `SkillLowHpFullHealShield` | `盾,愈` |
| 益壮 | 老 | `SkillAttackedShield` | `盾,愈` |
| 埋伏 | 吕蒙 | `SkillAttackedTeleport` | `乱,盾` |
| 不屈 | 周泰 | `SkillAidUnbreakable` | `盾,甲` |
| 腾驾 | 马腾 | `SkillAidAtkDrain` | `慑,攻` |
| 困敌 | 徐庶 | `SkillAidHexField` | `慑,疫` |
| 威震 | 张辽 | `SkillAidVengefulStance` | `震,乱` |
| 乱阵 | 贾诩 | `SkillSoldierStun` | `威,乱,惑` |
| 长驱 | 徐晃 | `SkillAidLifestealStrike` | `重,吸` |
| 狂暴 | 华雄 | `SkillAidBerserk` | `重,伤` |
| 鼓舞 | 小乔 | `SkillAidBuffHasteMoveSpeed` | `快,速` |
| 仁德 | 刘备 | `SkillAidBenevolence` | `血,蓝` |
| 奇才 | 才 | `SkillAidBuffArea` | `攻,甲,抗` |
| 名门 | 袁绍 | `SkillAidNobleBless` | `攻,甲,抗` |

---

## 七、日常核对步骤

1. 找某个 buff 被谁用了：在 `SkillConfig_s.cs` 搜 `"<短名>"`（注意短名可能出现在多段串里，建议搜 `"<短名>"` 与 `,<短名>` 两种形态）。
2. 找某技能挂了哪些 buff：看该技能行 `BuffId` 字段，按 `,`/`，`/`;`/`；` 拆段，每段取 `BuffConfig.GetConfigByNameS` 得 Id。
3. 别忘代码引用：搜 `CombatConst.` 在 `Skill/`、`Buff/` 下的 `AddBuff` 调用（当前只有 `TauntBuffId` 一处）。
4. buff 派生 buff（如眩晕）：`SkillSoldierStun` / `BuffHitStun` / `BuffVengefulStance` 会在运行时二次施加 buff，短名仍取自同一行 `BuffId` 的其它段。
