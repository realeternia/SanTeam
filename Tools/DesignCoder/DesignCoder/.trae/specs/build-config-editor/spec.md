# ConfigCoder - C#配置表Excel编辑器 Spec

## Why
项目中有大量以C#代码形式硬编码的配置表（如HeroConfig、ArmsConfig等），数据以`config[id] = new XxxConfig(...)`形式写在Load()方法中。直接编辑代码容易出错且不直观，需要一个类似Excel的可视化编辑器来浏览和修改这些配置数据。

## What Changes
- 实现CS配置文件解析器，从C#源码中提取字段定义（名称+注释）和数据行（Load方法中的构造函数调用）
- 实现启动时自动扫描配置目录，将`*_s.cs`文件列表填充到左侧ListView
- 实现点击左侧列表项后，右侧DataGridView以Excel表格形式展示该配置表的所有字段和数据
- 实现DataGridView编辑后，将修改写回C#源文件（保持原有代码结构）
- 实现批量选中多个单元格并批量填充数据
- 实现单元格前景色和背景色的修改（支持单个和批量）

## Impact
- Affected code: Form1.cs, Form1.Designer.cs, 新增CsConfigParser.cs
- Affected configs: D:\U3dPrj\SanKingdom\Assets\Resources\Scripts\Configs\*_s.cs

## ADDED Requirements

### Requirement: 配置文件扫描与列表展示
系统SHALL在启动时扫描`D:\U3dPrj\SanKingdom\Assets\Resources\Scripts\Configs`目录下所有`*_s.cs`文件，将文件名（去掉`_s.cs`后缀）显示在左侧ListView中。

#### Scenario: 启动时加载文件列表
- **WHEN** 应用启动
- **THEN** 左侧ListView显示所有`*_s.cs`文件的名称列表

### Requirement: C#配置文件解析
系统SHALL提供CsConfigParser类，能够从C#配置文件源码中解析出：
1. 字段定义：字段名、字段类型、XML注释描述
2. 数据行：Load()方法中每条`config[id] = new XxxConfig(...)`的参数值

#### Scenario: 解析字段定义
- **WHEN** 解析一个配置文件
- **THEN** 提取所有public字段的名称、类型和XML注释

#### Scenario: 解析数据行
- **WHEN** 解析一个配置文件
- **THEN** 提取Load()方法中所有构造函数调用的参数值，每行一条记录

### Requirement: Excel式表格展示
系统SHALL在用户点击左侧列表项后，在右侧DataGridView中以表格形式展示配置数据：
- 列头为字段名（带注释提示）
- 每行对应一条配置记录
- 支持基本类型：int, string, float, bool
- 数组类型（string[], int[]）以逗号分隔的字符串形式展示

#### Scenario: 选择配置表
- **WHEN** 用户点击左侧列表中的某个配置文件名
- **THEN** 右侧DataGridView显示该配置表的所有字段作为列，所有数据行作为行

#### Scenario: 数组字段展示
- **WHEN** 字段类型为string[]或int[]
- **THEN** 在单元格中以逗号分隔的字符串形式展示，如"忠义,仁德"

### Requirement: 数据编辑与保存
系统SHALL支持在DataGridView中直接编辑单元格值，编辑完成后可将修改写回C#源文件，保持原有代码结构（using、namespace、类定义、非Load方法等不变）。

#### Scenario: 编辑并保存
- **WHEN** 用户修改DataGridView中的单元格并触发保存
- **THEN** 修改后的值被写回对应的C#源文件，Load()方法中的构造函数参数更新，其余代码不变

### Requirement: 新增与删除配置行
系统SHALL支持在DataGridView中新增和删除配置行（即新增/删除config字典条目）。

#### Scenario: 新增配置行
- **WHEN** 用户在DataGridView中添加新行
- **THEN** 新行数据在保存时写入C#源文件的Load()方法

#### Scenario: 删除配置行
- **WHEN** 用户删除DataGridView中的某行
- **THEN** 该行数据在保存时从C#源文件的Load()方法中移除

### Requirement: 批量选中与填充数据
系统SHALL支持在DataGridView中批量选中多个单元格（支持鼠标框选、Ctrl+点击、Shift+点击），并可将一个值批量填充到所有选中的单元格中。

#### Scenario: 批量填充数据
- **WHEN** 用户选中多个单元格后，在填充输入框中输入值并确认
- **THEN** 所有选中的单元格值被替换为输入的值

#### Scenario: 多区域选中
- **WHEN** 用户按住Ctrl键点击多个单元格
- **THEN** 所有点击的单元格被选中，支持后续批量操作

### Requirement: 单元格颜色标注
系统SHALL支持修改DataGridView单元格的前景色（文字颜色）和背景色，支持单个和批量操作。颜色信息随数据一起保存，重新加载时恢复。

#### Scenario: 修改单元格颜色
- **WHEN** 用户选中一个或多个单元格后，通过颜色选择器设置前景色或背景色
- **THEN** 所选单元格的前景色/背景色立即更新

#### Scenario: 颜色持久化
- **WHEN** 用户保存文件后重新打开
- **THEN** 之前设置的颜色标注被正确恢复

#### Scenario: 清除颜色
- **WHEN** 用户对已标注颜色的单元格选择"清除颜色"
- **THEN** 单元格恢复默认前景色和背景色
