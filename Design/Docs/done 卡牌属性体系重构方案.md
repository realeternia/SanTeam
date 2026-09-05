# 卡牌属性体系重构方案（金铲铲风格）

> 日期：2026-08-25
> 状态：待实施
> 目标：将卡牌属性从「统帅/智力/武力/生命」重构为「攻击/法术强度/无双强度/生命 + 护甲/法术抗性」，并调整升星成本与战斗伤害公式。

---

## 一、新属性体系总览

| 新属性 | 英文标识 | 来源（旧） | 是否随升星成长 | 作用 |
| --- | --- | --- | --- | --- |
| 攻击 | `Atk` | 统帅 `LeadShip` | 成长 | 决定普攻伤害 |
| 法术强度 | `Ap` | 智力 `Inte` | 成长 | 决定法术技能伤害 |
| 生命 | `Hp` | 生命 `Hp` | 成长 | 生命值 |
| 无双强度 | `Might` | 武力 `Str` | 成长 | 部分技能（飞斧/火矢/旋风斩等）伤害 |
| 护甲 | `Armor` | 新增 | **不成长** | 减免普攻/物理伤害 |
| 法术抗性 | `MagicRes` | 新增 | **不成长** | 减免法术技能伤害 |

- 成长属性：攻击、法术强度、生命、无双强度（共 4 个，其中前 3 个为主属性）。
- 固定属性：护甲、法术抗性（一般卡不随升星成长，可由装备/羁绊/技能临时加成）。

### 数值定位（金铲铲量级，按 Price 费用分档）
- 1 星初始数值按 **Price（费用 3~10）** 分档，参考金铲铲「攻击:生命 ≈ 1:10」量级，三攻总和与生命随费用递增（每档梯度明显，同费内按英雄定位保留专精差异）：
  | Price | 三攻总和基准 | 生命基准 | 主属性参考 |
  | --- | --- | --- | --- |
  | 3 | 150 | 450 | 50 |
  | 4 | 165 | 500 | 55 |
  | 5 | 180 | 560 | 60 |
  | 6 | 195 | 620 | 65 |
  | 7 | 210 | 680 | 70 |
  | 8 | 225 | 750 | 75 |
  | 9 | 240 | 830 | 80 |
  | 10 | 255 | 920 | 85 |
- 每个英雄的三攻按原有专精比例缩放至档位总和（保留武将高攻/谋士高法强的定位差异）；生命在档位基准上按原生命高低微调 ±60。
- 升星成长改为**线性成长**：每星按配置的成长百分比字段提升（`AtkP/ApP/MightP`，当前全部为 80=每星+80%，2 星 ≈ 1.8 倍基础，对齐金铲铲 1→2 星强度变化；生命沿用每星 +80%）。`HeroConfig.Total` 字段已删除，三攻总和由 `Atk+Ap+Might` 推导。
- 新增 `AtkSpeed`（攻速）字段：攻击间隔秒数，默认 1.5s（暂所有英雄一致），接入战斗攻击冷却判定。
- 护甲/法术抗性基础值按「品质 + 职业/站位」计算，取值约 5~55，不随星级成长。

---

## 二、升星成本调整

| 星级 | 所需卡牌数（累计） | 当前（旧） |
| --- | --- | --- |
| 1 星 | 1 | 1 |
| 2 星 | 3 | 2 |
| 3 星 | 5 | 4 |
| 4 星 | 7 | 7 |
| 5 星 | 9 | 11 |
| N 星 | `2N - 1` | 递增 |

- 规则：1→2 需要 3 张，2→3 需要 5 张，3→4 需要 7 张，以此类推（奇数递增），提高低星升级成本。
- 实现：`cardHeroExp` 数组由 `[1,2,4,7,11,...]` 改为按 `2N-1` 生成的奇数数组。

---

## 三、战斗公式改动

### 1. 普攻（普通攻击）
- 旧：`damage = max(智力差, 统帅差, 武力差) × 2`，伤害类型取自差值最大的属性。
- 新：`damage = 攻击方攻击(Atk)`，伤害类型固定为 `atk`（物理）。
- 减免：`最终伤害 = damage × 100 / (100 + 防守方护甲)`，护甲不成长。

### 2. 技能伤害
- 伤害技能改为关联**法术强度(Ap)** 或 **无双强度(Might)**：
  - 原 `Attr="inte"` 的技能（火计/落雷/火墙/惊雷/鬼谋等）→ 改为 `"ap"`，吃法术强度。
  - 原 `Attr="str"` 的技能（飞斧/火矢/旋风斩/斩/魔神/威震/击破等）→ 改为 `"might"`，吃无双强度。
  - 原 `Attr="leadShip"` 的技能（炮车/戟阵/箭雨/连击等普攻联动类）→ 改为 `"atk"`，随普攻物理伤害。
- 减免：**法术强度(Ap) 类技能**伤害受目标法术抗性减免：`伤害 × 100 / (100 + 目标法抗)`。
- 无双强度(Might) 类技能视为无视抗性的强力伤害，不做额外减免。
- 普攻联动类技能的基础伤害已在普攻阶段受护甲减免，不再二次减免。

### 3. 伤害类型字符串（damType）映射
| 旧 | 新 |
| --- | --- |
| `inte` | `ap` |
| `leadShip` | `atk` |
| `str` | `might` |

### 4. 英雄普攻相关数值
- `attackDamage`（普攻基准 / 对士兵最低伤害下限）= 英雄攻击 `Atk`（旧为 `统帅/3`）。
- 攻击/法术/无双/生命在战斗中的初始化、装备加成、羁绊（友人）加成、属性技能（识破/鼓舞等）全部迁移到新字段。

---

## 四、配置表（Excel → 生成）改动

> 配置文件由 `Design/config/*.xlsx` 经 `Excel2dll.exe` 生成到 `Design/out/Csharp_s/`，再由 `run.bat` 复制到 `Assets/Resources/Scripts/Configs/`。

### 1. `HeroConfig.xlsx`
- 列名语义重定义：`统帅`→`攻击`、`智力`→`法术强度`、`武力`→`无双强度`（若生成器支持列名映射则改列名，否则保留列名、仅重定义字段含义，见下方兼容策略）。
- 新增列：`护甲(Armor)`、`法术抗性(MagicRes)`。当前实现：不依赖 Excel 列，由脚本按「品质 + 职业 + 站位」预计算 103 个英雄的 Armor/MagicRes 值，作为普通配置字段直接写入 `HeroConfig_s.cs` 每行数据（见下方兼容策略）。

### 2. `SkillConfig.xlsx`
- `属性(Attr)` 列取值：`inte`→`ap`、`leadShip`→`atk`、`str`→`might`。
- `克制属性(CheckAttrs)` 列取值同步替换（如 `str，leadShip`→`might，atk`、`inte`→`ap`）。

### 3. `ItemConfig.xlsx`
- `属性1/属性2(Attr1/Attr2)` 取值：`str`→`might`、`inte`→`ap`、`lead`→`atk`、`shield`→`hp`。
- 道具描述文案中的「武力/智力/统帅」同步更新为「无双强度/法术强度/攻击」。

### 4. `JobConfig.xlsx`
- 无结构改动（仅用于护甲/法抗计算的职业判断）。

### 兼容策略（若无法直接改 Excel 生成）
- 字段名（`LeadShip/Inte/Str`）为生成器内置映射，若无法改 Excel 列名，则**保留 C# 字段名不变**，仅在文档与代码注释中重定义其语义为「攻击/法术强度/无双强度」，所有业务代码按新语义读取。
- 护甲/法术抗性：不新增 Excel 列，由脚本按「品质 + 职业 + 站位」公式预计算各英雄 Armor/MagicRes 值，追加为 `HeroConfig` 的普通配置字段并逐行填入，不使用计算属性。

---

## 五、代码改动清单

| 文件 | 改动内容 |
| --- | --- |
| `Scripts/PO/AttrInfo.cs` | 字段改为 `Atk/Ap/Might/Hp/Armor/MagicRes`，更新 `Total/AddAttr` |
| `Scripts/Configs/HeroConfig_s.cs` | 字段语义重定义（或改名），新增护甲/法抗普通配置字段（含 meta、构造器参数与 103 行数据） |
| `Scripts/Configs/SkillConfig_s.cs` | `Attr/CheckAttrs` 取值替换 |
| `Scripts/Configs/ItemConfig_s.cs` | `Attr1/Attr2` 取值替换，描述文案更新 |
| `Scripts/Configs/JobConfig_s.cs` | 无改动 |
| `Scripts/Combat/Chess.cs` | 字段改名 `ap/might/atk/armor/magicRes`；普攻公式；护甲/法抗减免；`GetAttr/AddAttr/UpdateAttr/OnFriendDie` |
| `Scripts/Combat/Skill.cs` | 随字符串映射自动适配 |
| `Scripts/Combat/SkillManager.cs` | 随映射适配，`DuringAttack` 传入新 damType |
| `Scripts/Combat/SkillHelpAidHeal.cs` | `owner.inte` → `owner.ap` |
| `Scripts/Combat/BuffTimeDamage.cs` 等 | 通过 `GetAttr` 取属性，随映射适配 |
| `Scripts/HeroSelectionTool.cs` | `GetCardAttr` 新成长公式；护甲/法抗不成长；星级阈值 `2N-1`；道具属性映射；`GetAttrIcon` 图标映射 |
| `Scripts/PlayerInfo.cs` | `AutoCheckItem` 属性数组与字符串更新 |
| `Scripts/GameManager.cs` | 羁绊配对属性判断（`Inte`→`Ap`、`Str`→`Might`） |
| `Scripts/PlayerAI.cs` | 属性读写改名 |
| `Scripts/CardViewControl.cs` | 展示值改为新属性（保留字段名以兼容预制体绑定） |
| `Scripts/HeroInfo.cs` | 属性展示与最高属性图标逻辑 |
| `Scripts/ItemHeroDetail.cs` | 详情展示新属性 + 护甲/法抗 |
| `Scripts/ItemDetail.cs` | 道具图标加载走 `GetAttrIcon` 映射 |
| `Scripts/Tooltip.cs` | 技能属性标签 `[武]/[统]/[智]` → `[无双]/[攻]/[法]` |
| `Scripts/BagControl.cs` | 技能颜色判断字符串更新 |
| `Scripts/RankCellInfo.cs` / `RankPanelManager.cs` | 属性读写改名（保留序列化 UI 字段名） |
| `Scripts/PickPanelControl.cs` 等 | 随读取路径适配 |

### 关键原则
- **UI 脚本中已序列化的字段名（绑定预制体）不改名**，只改赋值来源，避免丢失预制体绑定；预制体上的文字标签（如「统帅」）需由策划手动改文案。
- 非序列化的内部字段可自由改名。

---

## 六、存档兼容
- `PlayerInfo` 存档中的 `attrAddons`（AttrInfo）字段名变化，旧存档不兼容，属可接受（开发阶段）。

---

## 七、验证点
1. 卡片商店/背包展示的攻击、法术强度、无双强度、生命数值正确。
2. 升星：3 张到 2 星、5 张到 3 星；2 星属性 ≈ 1.8 倍 1 星。
3. 普攻伤害随攻击变化、受护甲减免；法术技能受法抗减免。
4. 火计/落雷等吃法术强度；飞斧/火矢等吃无双强度；炮车/箭雨等吃攻击。
5. 护甲/法抗不随星级变化。
