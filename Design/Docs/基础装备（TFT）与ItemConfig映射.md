# 基础装备（TFT 组件） → 仓库 ItemConfig 映射

说明：本文件根据仓库中 Assets/Resources/Scripts/Configs/ItemConfig_s.cs（ItemConfig）条目，与金铲铲（TFT）常见一级基础装备（组件）进行语义映射。优先按 Effect/Attr1/Attr2 字段进行直接对应；无法合理对应或仓库中无明显等价项的组件已留空并在备注中说明。

- 数据来源：Assets/Resources/Scripts/Configs/ItemConfig_s.cs（commit: 89cd43d15960d8a011705c8c08aaaa1259bfa96c）
- 说明：保留 ItemConfig 原始字段数值（未做换算）。如需将数值换算到 TFT 标准，可提出换算规则后我可批量转换。

---

## 映射表（优先直接语义对应）

| TFT 基础组件（中文） | 建议映射 ItemConfig (Id / 名称) | ItemConfig 属性 (Effect / Attr1:Val, Attr2:Val) | 说明 / 备注 |
|---|---:|---|---|
| 大剑 (B.F. Sword) — +攻击 | 400007 / 孙子兵法 | Effect="attr" ， Attr1="atk" : 15 | 孙子兵法给 atk +15，语义上最接近“大剑”攻击加成 |
| 反曲弓 (Recurve Bow) — +攻速 | （无明确映射） | — | ItemConfig 中未找到带 attackRate 或 明确攻速属性的基础条目，故留空 |
| 大棒 (Needlessly Large Rod) — +法强 (AP) | 400012 / 道德经 | Effect="attr" ， Attr1="ap" : 15 | 道德经给 ap +15，可视为法强类组件 |
| 泪滴 (Tear of the Goddess) — +回蓝/法力/技能资源 | （无明确映射） | — | ItemConfig 中没有明显的 MP/法力属性或“回蓝”字段的基础组件，留空 |
| 锁子甲 (Chain Vest) — +护甲 | 409003 / 虎王重甲 | Effect="pattr" ， Attr1="shp" : 40 | 虎王重甲字段 pattr/shp=40，语义接近护甲/耐久。但该条目 id 409003 看似较高阶，不是最基础组件，策划可决定是否接受映射或新增基础护甲组件。 |
| 魔抗斗篷 (Negatron Cloak) — +魔抗 | （无明确映射） | — | ItemConfig 中未发现 magicRes 或类似字段的基础条目，留空 |
| 巨人腰带 (Giant's Belt) — +生命 | 400013 / 赤兔马 （或 400014 / 的卢马 / 400015 / 大宛宝马） | Effect="attr" ， Attr1="hp" : 75 / 50 / 30 | 赤兔马 hp+75、的卢马 hp+50、大宛宝马 hp+30。推荐使用赤兔马作为生命加成组件映射，或按需求选择不同量级组件。 |
| 手套 (Sparring Gloves) — +暴击/闪避/触发 | （无明确映射） | — | ItemConfig 中未见以 critRate 或 dodgeRate 为 Attr 的基础条目，留空 |
| 铲子 (Spatula) — 特殊（改变职业/羁绊） | （无明确映射） | — | 铲子为特殊道具（修改职业/羁绊），ItemConfig 未见对应字段或条目，留空 |
| 其他常见基础组件示例（供参考） | 400001 / 关王刀；400002 / 方天画戟；400003 / 丈八蛇矛；400004 / 檀木弓；400005 / 大斧；400006 / 三丈枪 | 多为 Effect="attr"，Attr1="might" 或 "atk"（数值见配置） | 这些条目多数为物理向（might/atk），可用于设计更多物理组件。需明确 might 与 atk 的语义差别后再决定映射到哪些 TFT 组件。 |

---

## ItemConfig 中可参考的具体条目（摘录）

以下为 ItemConfig_s.cs 中已定义、与组件映射相关或可能可复用的条目（便于策划快速核对）：

- 400001 关王刀 — Effect="attr", Attr1="might", Attr1Val=10
- 400002 方天画戟 — Effect="attr", Attr1="might", Attr1Val=15
- 400003 丈八蛇矛 — Effect="attr", Attr1="might", Attr1Val=11
- 400004 檀木弓 — Effect="attr", Attr1="might", Attr1Val=6
- 400005 大斧 — Effect="attr", Attr1="might", Attr1Val=6
- 400006 三丈枪 — Effect="attr", Attr1="might", Attr1Val=6
- 400007 孙子兵法 — Effect="attr", Attr1="atk", Attr1Val=15
- 400008 墨子 — Effect="attr", Attr1="atk", Attr1Val=6
- 400009 六韬 — Effect="attr", Attr1="atk", Attr1Val=10
- 400010 诗经 — Effect="attr", Attr1="ap", Attr1Val=6
- 400011 易经 — Effect="attr", Attr1="ap", Attr1Val=10
- 400012 道德经 — Effect="attr", Attr1="ap", Attr1Val=15
- 400013 赤兔马 — Effect="attr", Attr1="hp", Attr1Val=75
- 400014 的卢马 — Effect="attr", Attr1="hp", Attr1Val=50
- 400015 大宛宝马 — Effect="attr", Attr1="hp", Attr1Val=30
- 401010 豆腐 — Effect="tpattr", Attr1="might", Attr1Val=5（食物/临时道具）
- 401011 沙拉 — Effect="tpattr", Attr1="ap", Attr1Val=5
- 401012 烤鸭 — Effect="tpattr", Attr1="atk", Attr1Val=5
- 409001 火尖枪 — Effect="attr", Attr1="might", Attr1Val=15（高级/特殊）
- 409002 聚宝盆 — Effect="pattr", Attr1="roundgold", Attr1Val=5（被动收益类）
- 409003 虎王重甲 — Effect="pattr", Attr1="shp", Attr1Val=40
- 409005 酒 — Effect="attr", Attr1="might", Attr1Val=10 ; Attr2="ap", Attr2Val=6

> 注：以上为文件中可直接读取到的条目示例，完整表请以 ItemConfig_s.cs 为准。

---

## 结论与建议

1. 已将能直接语义对应的基础组件映射在表中（如：大剑 → 孙子兵法；大棒 → 道德经；生命 → 赤兔马）。
2. 对于攻速（反曲弓）、泪滴（回蓝）、手套（暴击）、铲子（特殊羁绊）、斗篷（魔抗）等在 ItemConfig 中未找到一对一基础条目的项，已在表中留空，建议由策划决定：
   - 新增对应基础组件条目到 ItemConfig（推荐：为攻速、回蓝、暴击/闪避、铲子类特殊道具、魔抗分别补一条）；或
   - 接受现有条目作为近似映射（例如将檀木弓视作反曲弓的近似），并在合成/合成结果逻辑中说明差异。
3. 若需要，我可以：
   - 将此文档提交到仓库（已准备好执行）；
   - 或根据你给出的“优先级/默认映射规则”自动填充剩余空项并生成最终映射表；
   - 或生成一份“合成表”示例（哪些两个基础组件合成哪个成品），便于策划直接落表。

---

如果你确认，我将这个 md 文件提交至：Design/Docs/基础装备（TFT）与ItemConfig映射.md（现在准备��交）。如需改名或调整内容请告诉我。