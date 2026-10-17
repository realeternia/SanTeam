# 合并 DescriptVal → Descript（彻底删除 DescriptVal 列）

## Context（为什么改）

`SkillConfig` 的技能说明由两列组成：`Descript`（描述模板，含 `/1 /2` 占位符）与 `DescriptVal`（占位符替换值；Lv1 填 `/字段名` 动态引用，如 `/strength`、`/linkself-atk`），其余等级留空由引擎按本级字段自动推导。占位符 + 数值模板分离对维护者不友好，用户希望**删掉 DescriptVal，直接把字段引用内联进 Descript**。

合并后：Lv1 的 `Descript` 直接写成 `…自身攻击/linkself-atk…`（字段引用内联，无 `/1 /2` 占位符），Lv2-5 仍留空自动回退 Lv1 模板；引擎只需对 Descript 整体做 `SubstituteFieldRefs`。选择**彻底删除**：移除字段、FieldMeta、构造函数第 5 个参数，并同步改写 411 行 `new SkillConfig(...)` 实参。

## 目标数据形态（示例）

改前 Lv1：
```csharp
new SkillConfig(2000001, "国家护盾", "国", "同阵营英雄获得最大生命/1的护盾", "+/strength", "职业", 1, ...)
```
改后 Lv1（合并，去掉第5参数）：
```csharp
new SkillConfig(2000001, "国家护盾", "国", "同阵营英雄获得最大生命+/strength的护盾", "职业", 1, ...)
```
Lv2-5 原 `Descript=""`、`DescriptVal=""` → 变为只有 `Descript=""`（去掉一个 `, ""`），引擎自动回退 Lv1 模板。

## 实施步骤

### 1) 配置类 [SkillConfig_s.cs](file:///d:/Codes/SanTeam2/Assets/Resources/Scripts/Configs/SkillConfig_s.cs)
- 删 FieldMeta 条目：`{"DescriptVal", …}`（第 47 行）。
- 删字段声明与注释：`public string DescriptVal;` 及其 `///<summary>`（第 103-106 行）。
- 构造函数去掉第 5 参数 `string DescriptVal`，并删 `this.DescriptVal = DescriptVal;`（第 237、243 行）。
- 新间隔符顺序变为：`(Id, Name, Sname, Descript, Type, Lv, …)`。

### 2) 机械改写 411 行数据（脚本化）
用 code_mode / 一段脚本处理文件中每一条单行 `config[N] = new SkillConfig(...)`：
- 解析出内层实参列表（按引号与括号边界切分顶层逗号，尊重字符串字面量）。
- 第 5 个实参 = 旧 `DescriptVal`（索引 4）。若 `Descript`（索引 3）非空且含 `/1 /2…` 占位，则将其按顺序替换为 `DescriptVal` 以 `;` 分隔后的各段 → 得到内联了字段引用的新 `Descript`。
- 重组：去掉第 5 实参，写回行。Lv2-5（`Descript=""`,`DescriptVal=""`）等价于去掉一个 `, ""`。

### 3) 引擎 [ConfigManager.cs](file:///d:/Codes/SanTeam2/Assets/Resources/Scripts/Configs/ConfigManager.cs)（L329-384）
- `GetSkillDescript`: 去掉 `curVal`/`GetLevelDescriptVal` 逻辑，直接 `return SubstituteFieldRefs(templateCfg.Descript, cfg, nextCfg)`；`nextCfg` 由 `withNext && cfg.Lv+1` 存在时取得。
- 删除私有方法 `GetLevelDescriptVal`。
- `SubstituteFieldRefs(input, cfg, SkillConfig nextCfg = null)`：现有 `/字段名` 解析不变；新增——当 `nextCfg != null` 且某字段的 `next` 取值与 `cur` 不同时，在 cur 后追加 `SysColor.ColorText("("+next+")", SysColor.UI.NextLv)`。
- `GetFieldRefValue`/`GetAttrFieldValue`、`withNext` 数值比较逻辑均复用，不改。
- 同步更新 L329-332 的注释说明（去掉 DescriptVal 相关描述）。

### 4) 引用处清理 & 文档
- [JobLinkManager.cs](file:///d:/Codes/SanTeam2/Assets/Resources/Scripts/Combat/JobLinkManager.cs) L203-204 注释引用 DescriptVal，改措辞（仅注释）。
- 更新 [combat-skill-design/SKILL.md](file:///d:/Codes/SanTeam2/.trae/skills/combat-skill-design/SKILL.md) 中 DescriptVal 相关约定，改为"字段引用直接内联进 Descript"，防止后续新技能配置回归旧格式。

## 兼容性与注意点
- **withNext**（职业/好友连线档位提示）：旧逻辑按 Lv 的 `;` 段字符串比较；新逻辑在字段级对 `next != cur` 追加括号，语义一致。仅有一处极细微显示差异：字段引用前若带字面 `+` 号（如国家护盾 `+/strength`），旧括号为 `+0.30`、新为 `0.30`；此类场景（`Dumb`，非 withNext 消费方）实际不会显示括号，无影响。
- 无占位符、无字段引用的整型文案技能（结构兜底，各等级直接填完整说明）不参与合并，仅去掉空 `DescriptVal` 实参，运行行为不变。

## 验证
1. **编译**：用 VS MSBuild `C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe` 编译 `Tools/战斗模拟器/BattleSimulator/BattleSimulator.csproj`（经 csproj `<Compile Include … Link>` 桥接 `Configs/*`），EXIT=0。
2. **数据改写复核**：grep `SkillConfig_s.cs` 确认无残留 `"DescriptVal"`/`DescriptVal`，且 411 行 `new SkillConfig(` 实参均少一个。
3. **运行时**：`BattleSimulator.exe --headless` 实跑一场，确认配置加载 + 技能说明生成无回归。
4. **withNext 抽样**：阅读 TooltipHero/TooltipFriend 路径（职业/连线技能），确认 `/linkself-atk`、`/strength%` 等字段引用按本级值 + 下一档括号正确展开。