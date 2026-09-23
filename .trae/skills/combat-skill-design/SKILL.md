---
name: combat-skill-design
description: 编写《三国卡牌》(金铲铲式)战斗技能与数值配置的规范指南。当需要新增技能、调整技能数值或平衡、改技能说明(Descript)、分配技能 Id 段时使用。不要用于纯 Buff、羁绊或物品的配置。
---

# 战斗技能设计规范

指导在本工程中新增或调整一个战斗技能。先通读 SkillConfig 的既有行（如强击/火墙/火矢）作为模板，再按本规范落地。

## 一、技能实体分层

- **配置数据**：`SkillConfig`（`Configs/SkillConfig_s.cs`），一行一条记录，按 `Id` 存入字典。
- **业务逻辑**：`Combat/Skill/SkillXxx.cs`，一技能一文件，继承基类 `Skill`。减法/伤害/状态相关虚方法覆写签名须与基类一致。
- **常量引用**：战斗代码中出现的技能/Buff Id 或机制数值必须提取到 `CombatConst`，禁止硬编码。

## 二、等级机制

- 同一技能按 `Sname` 展开为 5 行（`Lv=1~5`）。`SetLevel(lv)` 会切换到对应配置行；连锁/好友等机制通过改写 `Level` 生效。
- 取配置统一走 `ConfigManager.GetSkillConfig(sname, lv)`（按缩写+等级）。
- 同名技能各等级行其余字段应保持语义一致，仅数值（Strength/CD/耗蓝等）按级递增。
- 新增配置类或字段必须同步 `ConfigManager.Init()` 注册，并核对 `fieldMeta` 与全部数据行的参数位顺序。

## 三、伤害公式

统一公式：`GetSkillDamage() = Strength + 关联属性 × (1 + Strength2)`

- 魔法技能：`Strength × (100 + ap×(1+Strength2)) / 100`（受魔抗减免）；武力技能：`Strength + atk×(1+Strength2)`。属性类型由技能类型决定（技/术对应职业主属性）。
- 倍率槽：`Strength` 为主数值/基础伤害；当 `Strength` 被占用（作基础伤害或其它用途）时，倍率系数放入 `Strength2`。
- 普攻与技能伤害统一经 `SkillManager.DuringCalDamage` 计算，再走受击方 `DuringCalDamaged`。

## 四、数值设计原则（平衡核心）

1. **触发率 Rate**：`Rate = 0` 表示必然发动（无随机判定）。需要概率触发时才填非 0。
2. **单体 vs 群体**：群体技能（多目标/范围弹射）的单体伤害要**明显小于**单体爆伤技能，通过目标数控制总输出上限，避免范围技能碾压单体。
3. **MP 消耗是主要制约与平衡手段**：高耗蓝技能应换来更高的伤害或更强效果；同强度下耗蓝越高、数值越强，反之亦然。不要把单体与群体技能拉到同一伤害水平，靠耗蓝错开。
4. **数值成长对照基准技能**：以单体基准技能（如 `强击`，Strength Lv1=60，描述"对目标造成法强/1的魔法伤害"）为参照，逐等级提升伤害。参考模板档位：
   - 单体法术阵（火墙式）：Strength Lv1~5 = 25/40/60/80/110。
   - 群体/弹射技能：用 `Strength` 作伤害倍率（如 0.4、0.8，经 `damage × Strength` 结算）+ 目标数 `TargetCount` 控制，使单段伤害低于单体基准。
5. 数值全部放进配置行，不在逻辑里写死；调整数值只改 `SkillConfig`，无需改代码。

## 五、技能说明（Descript）规范

- **模板**：仅在 `Lv1` 行的 `Descript` 填模板，数值处用占位符 `/1`、`/2`…（斜杠无需 C# 转义）。
- **参数**：各级行 `DescriptVal` 填参数，多个用 `;` 分隔（如 `"3;40%"`）；数值随触发技能等级，魔法成长填 Strength（如 `"25"`/`"40"`）。
- 其余等级（Lv2~5）的 `Descript` 留空，显示时自动取同 Sname Lv1 模板 + 本级参数拼接。
- 显示统一走 `ConfigManager.GetSkillDescript(cfg, withNext)`：默认仅当前等级；`withNext=true` 每参数位附下一档不同值（淡绿 `SysColor.UI.NextLv`），供职业/好友档位提示。
- 结构无法用模板表达（各等级数字个数不一致）时才保留每级完整 Descript 兜底。

## 六、技能 Id 段分配

- 武将专属技能集中在 `20202xx` 连续段：单体法术向放在火矢(2020215) 之后（如 2020216+），弹射/范围技能紧随其后。已固定占用示例：共杀 2020216~2020220、旋风斩 2020221~2020225、落雷 2020226~2020230。
- 复用其它既定技能时避免改其 Id；新技能才在空段分配，勿与已占用 Id 冲突。
- 全局搜索确认旧 Id 无其它脚本引用后再改段。

## 七、Buff 引用与命名

- `SkillConfig` 的 `BuffId` 为 `string` 类型，存 `BuffConfig` 的 `NameS` 短名（空字符串表示无 buff）。
- 技能/Buff 常量归 `CombatConst`；不变的通用技能 Sname（如强击等）命名尽量见名知意。

## 八、落地后核对清单

- 技能文件已加 `<Compile Include>`（`Assembly-CSharp.csproj`）。
- 全工程 `Grep` 旧字段/旧 Id 确认无残留引用。
- 描述各等级参数与实机数值一一对应，`DescriptVal` 参数顺序匹配模板占位符。