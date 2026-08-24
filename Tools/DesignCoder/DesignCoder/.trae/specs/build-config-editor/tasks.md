# Tasks

- [x] Task 1: 创建CsConfigParser解析器类
  - [x] SubTask 1.1: 实现字段定义解析 - 从C#源码中提取public字段的名称、类型和XML注释
  - [x] SubTask 1.2: 实现数据行解析 - 从Load()方法中提取每条`config[id] = new XxxConfig(...)`的参数值，支持int/string/float/bool/string[]/int[]类型
  - [x] SubTask 1.3: 实现C#源码生成 - 将编辑后的字段和数据行重新生成为C#源码，保持原有代码结构

- [x] Task 2: 实现Form1主界面逻辑
  - [x] SubTask 2.1: 实现启动时扫描配置目录，将`*_s.cs`文件名填充到左侧ListView
  - [x] SubTask 2.2: 实现ListView点击事件，解析选中文件并在DataGridView中展示数据
  - [x] SubTask 2.3: 实现DataGridView列头设置（字段名+注释Tooltip）
  - [x] SubTask 2.4: 实现DataGridView数据绑定（DataTable方式）

- [x] Task 3: 实现编辑与保存功能
  - [x] SubTask 3.1: 实现DataGridView单元格编辑后更新内部数据
  - [x] SubTask 3.2: 实现保存按钮，将修改后的数据通过CsConfigParser写回C#源文件
  - [x] SubTask 3.3: 实现新增行和删除行功能

- [x] Task 4: 实现批量选中与填充功能
  - [x] SubTask 4.1: 确保DataGridView的MultiSelect属性为true，支持鼠标框选、Ctrl+点击、Shift+点击
  - [x] SubTask 4.2: 添加"批量填充"按钮/菜单项，弹出输入框，将值填充到所有选中单元格
  - [x] SubTask 4.3: 实现批量填充逻辑：遍历SelectedCells，根据字段类型验证并设置值

- [x] Task 5: 实现单元格颜色标注功能
  - [x] SubTask 5.1: 添加"设置前景色"和"设置背景色"按钮/菜单项，使用ColorDialog选择颜色
  - [x] SubTask 5.2: 实现对选中单元格批量设置前景色和背景色
  - [x] SubTask 5.3: 添加"清除颜色"功能，恢复默认前景色和背景色
  - [x] SubTask 5.4: 实现颜色信息持久化 - 在配置目录下生成与cs文件同名的.color文件保存颜色映射，加载时恢复

- [x] Task 6: 完善UI交互
  - [x] SubTask 6.1: 更新Form1标题为"ConfigCoder"
  - [x] SubTask 6.2: 添加工具栏：保存、刷新、新增行、删除行、批量填充、设置前景色、设置背景色、清除颜色
  - [x] SubTask 6.3: 添加右键菜单：批量填充、设置前景色、设置背景色、清除颜色

# Task Dependencies
- Task 2 depends on Task 1
- Task 3 depends on Task 1 and Task 2
- Task 4 depends on Task 2
- Task 5 depends on Task 2
- Task 6 depends on Task 2, Task 4, Task 5
