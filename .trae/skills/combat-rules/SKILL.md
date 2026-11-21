---
name: combat-rules
description: 《三国卡牌》战斗层开发规范：Skill/Buff/Chess 与各战斗 Manager 的架构、技能与 Buff 实现约定、BuffId 段位、连锁羁绊、伤害/MP 公式、战斗常量归属（CombatConst / 实现类 private const / XxxConst）。当编写或修改 Assets/Resources/Scripts/Combat/ 下代码（新增 SkillXxx、BuffXxx、战斗 Manager、调整伤害或机制数值）时必须加载。配表细节（SkillConfig 43 列、模板/组件）另见 combat-skill-design。
---

# 战斗层开发规范

本 skill 承接原 `project_rules.md` 中战斗相关内容；全局规范（日志、随机数、配置类、文档）仍以 `.trae/rules/project_rules.md` 为准。

## 一、架构与目录

- 战斗层代码：`Assets/Resources/Scripts/Combat/`
  - `Chess.cs`：战斗单位（属性/索敌/移动/攻击/MP/受伤管线）
  - `Skill/`：`Skill` 基类 + `SkillXxx` 派生（一技能一文件）+ `SkillManager`
  - `Buff/`：`Buff` 基类 + `BuffXxx` 派生 + `BuffManager`
  - 各机制 Manager：`FatigueManager`、`FriendLineManager`、`FactionShieldManager`、`EffectManager` 等
- `CombatConst.cs`：只放**跨类共享**的战斗 Id / 类型 / 标签 / 通用公式常量

## 二、技能系统

- 基类 `Skill`（`Combat/Skill/Skill.cs` 构造函数 `(int id, Chess unit)`），派生类放 `Combat/Skill/`
- 命名前缀：`SkillAidXxx` 英雄专属 / `SkillComXxx` 通用模板 / `SkillSoldierXxx` 兵种 / `SkillBondXxx` 羁绊 / `SkillEquipXxx` 道具；`ScriptName`、类名、文件名三者一致
- 等级：同一技能在 SkillConfig 按 `Sname` 展开 Lv1~5，`SetLevel(lv)` 切配置行；连锁（兵种/好友特殊）通过修改 `Level` 生效
- 技能伤害统一公式：`Strength + 关联属性 × SkillDamageAttrRate`（Attr：`ap`=法强 / `might`=无双 / `atk`=武力）；法术伤害再按 `(100+ap)/100` 与目标魔抗结算
- 主动技能在 `CheckAidSkill()` 里用 `CheckBurst(target)` 做 MP/CD/概率闸门；被动钩子用 `Tick` / `BattleBegin` / `OnCastSuccess` / `OnDeath`
- 新增 `.cs` 必须加入 `Assembly-CSharp.csproj` 的 `<Compile Include>`

## 三、Buff 系统

- 基类 `Buff`（`Combat/Buff/Buff.cs`），派生类放 `Combat/Buff/`，`BuffManager` 反射 `ScriptName` 构造
- **BuffId 号段**：`300xxx` 正面通用、`3001xx` 正面特化、`301xxx` 负面通用、`3011xx` 负面特化
  - 通用 = 可复用、语义单一（加攻/加防/加速/护盾/回复/减攻/减速等）；特化 = 专属机制/需独立脚本状态（复活、据守、嘲讽、混乱等）
  - 新增先判正负面与是否特化，再顺延取号；`NameS` 被 SkillConfig.BuffId 引用，改 Id 要同步 `CombatConst` 与注释
- 复杂 Buff 优先拆多个通用 Buff 组合（`SkillConfig.BuffId` 用 `,` 分隔；各段按位置读 `StrengthBuff1`，如 `快,速`、`攻,甲,抗`）
- 数值读取统一 `GetStrBuff(key)` / `GetStr2(key)`（语义 key，不按下标猜槽位）
- 品质/法强加成（仅英雄专属技能 + `BuffAttrDynamic` 键）：
  - `BuffConfig.IsQualityBonus=true` → ×`CombatConst.QualityBuffFactors`
  - `BuffConfig.IsApBonus=true` → ×`(100+施法者ap)/100`
  - 黑名单键：`ap` / `evade` / `critRate`（`生效率/概率`类不参与）
- 叠加模式见 `CombatConst.BuffStack*`：1 共存 / 2 数值融合 / 3 时间延长 / 4 刷新

## 四、连锁/羁绊

- 兵种连锁 `JobLinkManager`（静态类）：上阵同职业 1/2/3/4/5 人触发职业技能 Lv1~5；`LinkSelf`=连接英雄自身加成，`LinkTeam`=全队总量（配置即总量，不乘人数），数值全部走 SkillConfig
- 阵营护盾 `FactionShieldManager`：同阵营人数档位用 `CombatConst.FactionShieldCounts`，技能缩写 `CombatConst.FactionShieldSkillSname`
- 好友连线 `FriendLineManager`：档位数组在 `AIConst.FriendLineCounts`（AI 评分用），线值走 SkillConfig，不硬编码

## 五、战斗常量归属（重点）

**能归属到某个类的，不要往常量类里塞**：

1. 只被单个类/单个机制使用 → 实现类 `private const`
   - `SkillComBuffAura.TickInterval / BuffDuration / DefaultMpRate`
   - `FatigueManager.StartTime / Interval / BaseDamage / DamageGrowth`
   - `EffectManager.AreaEffectBaseRadius / UnitEffectExtraOffsetY`
2. 被多个类共享 → `CombatConst`（BuffId、伤害类型/标签、叠加模式、抗性、移动避障、单位间距、品质成长、技能 MP 修正等）
3. 成套且跨多类共用 → 新建独立静态常量类 `XxxConst`，不要膨胀 `CombatConst`
4. `AIConst` 只放 AI 决策策略；`GameConst` 只放非战斗通用玩法
- 禁止在战斗代码硬编码技能/Buff Id 与机制数值；`CombatConst` 只放数值/Id，不放业务方法

## 六、战斗机制要点

- 伤害管线：`Chess.OnSkillDamaged` → 抗性 `ResistMultiplier` → 护盾吸收 → Buff 钩子（`BeforeCalDamaged`）→ 扣血/回蓝；派生伤害（`LockChain` / `AntiShield`）不得再派生
- 法术暴击：Buff `300020`「法暴」，法术技能伤害按 `magicCritChance` 概率 ×`magicCritRate`，判定在 `Skill.ApplyDamageModifiers`
- 光环 `SkillComBuffAura`：每 1.4s 挂 1.5s Buff；MpCost>0 时需施法开启，开启后每秒扣 `1×SkillConfig.AuraMpRate`（默认 20），期间攻击/受击回蓝照常累计，MP<=0 自动关闭
- 疲劳 `FatigueManager`：超时后按间隔对全体英雄施加递增真实伤害，不致死（最低 1 血）
- 索敌/移动：`Chess.FindTarget`、`TargetSelector`，避障与单位间距常量在 `CombatConst`
