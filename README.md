<div align="center"><img src="https://github.com/WPFDevelopersOrg/ResourcesCache/raw/main/resources/Logo.png"/></div>

<div align="center">

[![nuget-version](https://img.shields.io/nuget/v/WPFDevelopers.Avalonia.svg?color=red)](https://www.nuget.org/packages/WPFDevelopers.Avalonia/) [![Github stars](https://img.shields.io/github/stars/WPFDevelopersOrg/WPFDevelopers.Avalonia)](https://github.com/WPFDevelopersOrg/WPFDevelopers.Avalonia/stargazers) ![Downloads](https://img.shields.io/nuget/dt/WPFDevelopers.Avalonia?color=%23409EF)

</div>


## Welcome to WPFDevelopers.Avalonia

基于 Avalonia 11.3 的 UI 组件库，提供丰富的控件主题和实用控件。

---

## 快速开始

### 1. 安装 NuGet 包

```bash
dotnet add package WPFDevelopers.Avalonia
```

### 2. 引入资源

在 `App.axaml` 中引入 WD 命名空间并加载资源：

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:wd="https://github.com/WPFDevelopersOrg/WPFDevelopers.Avalonia"
             x:Class="YourApp.App">
    <Application.Resources>
        <wd:Resources Theme="Default"/>
    </Application.Resources>
</Application>
```

> `Theme` 支持：`Light`（亮色）、`Dark`（暗色）、`Default`（跟随系统）

---

## 控件使用

### Buttons（按钮）

```xml
<WrapPanel>
    <!-- 主要按钮 -->
    <Button Content="Primary"  />
    <!-- 圆形按钮 -->
    <Button Content="Primary" wd:ElementHelper.IsRound="True" Theme="{StaticResource wd-primary}" />
    <!-- 默认描边按钮 -->
    <Button Content="Default" Theme="{StaticResource wd-default}" />
    <!-- 普通无框按钮 -->
    <Button Content="Normal" Theme="{StaticResource wd-normal}" />
    <!-- 成功 / 失败 / 警告（描边） -->
    <Button Content="Success" Theme="{StaticResource wd-success}" />
    <Button Content="Warning" Theme="{StaticResource wd-warning}" />
    <Button Content="Danger" Theme="{StaticResource wd-danger}" />
    <!-- 成功 / 失败 / 警告（实心） -->
    <Button Content="Filled Success" Theme="{StaticResource wd-success-filled}" />
    <Button Content="Filled Warning" Theme="{StaticResource wd-warning-filled}" />
    <Button Content="Filled Danger" Theme="{StaticResource wd-danger-filled}" />
</WrapPanel>
```

### Icons（图标）

```xml
<!-- PathIconEx 内置图标 -->
<wd:PathIconEx Kind="Home" Width="24" Height="24" Foreground="{DynamicResource WD.PrimaryBrush}" />
<wd:PathIconEx Kind="CheckMark" Width="24" Height="24" Foreground="{DynamicResource WD.SuccessBrush}" />
<wd:PathIconEx Kind="Warning" Width="24" Height="24" Foreground="{DynamicResource WD.WarningBrush}" />
<wd:PathIconEx Kind="Error" Width="24" Height="24" Foreground="{DynamicResource WD.DangerBrush}" />
<wd:PathIconEx Kind="Star" Width="24" Height="24" />
<wd:PathIconEx Kind="Add" Width="24" Height="24" />
<wd:PathIconEx Kind="Time" Width="24" Height="24" />
<wd:PathIconEx Kind="Copy" Width="24" Height="24" />
<wd:PathIconEx Kind="Filter" Width="24" Height="24" />
<!-- 使用内置几何数据 -->
<PathIcon Data="{StaticResource WD.CheckMarkGeometry}" Foreground="{DynamicResource WD.SuccessBrush}" Width="24" Height="24" />
```

### Inputs（输入控件）

```xml
<!-- TextBox -->
<TextBox Watermark="Enter text here..." />
<TextBox Watermark="Clear text" Classes="IsClear" />
<TextBox Watermark="Enter password here..." PasswordChar="●" RevealPassword="False" />

<!-- CheckBox -->
<CheckBox Content="CheckBox (checked)" IsChecked="True" />
<CheckBox Content="CheckBox (unchecked)" />
<CheckBox Content="CheckBox (indeterminate)" IsThreeState="True" IsChecked="{x:Null}" />

<!-- RadioButton -->
<RadioButton Content="Option A" GroupName="demo" IsChecked="True" />
<RadioButton Content="Option B" GroupName="demo" />

<!-- ToggleButton -->
<ToggleButton Content="Toggle Off" />
<ToggleButton Content="Toggle On" IsChecked="True" />
<!-- Switch 样式 -->
<ToggleButton Theme="{StaticResource wd-switch}" IsChecked="True" />
<ToggleButton Theme="{StaticResource wd-switch}" />
```

### GroupBox（分组框）

```xml
<wd:GroupBox Header="Group Title" Width="500">
    <TextBlock Text="This is the content of the GroupBox." Margin="4" />
</wd:GroupBox>
<!-- 支持任意 Header 内容 -->
<wd:GroupBox Width="500">
    <wd:GroupBox.Header>
        <StackPanel Orientation="Horizontal" Spacing="8">
            <wd:PathIconEx Kind="Home" Width="16" Height="16" />
            <TextBlock Text="Header with Icon" />
        </StackPanel>
    </wd:GroupBox.Header>
    <TextBlock Text="Content here" Margin="4" />
</wd:GroupBox>
```

### Tag（标签）

```xml
<wd:Tag Content="Default Tag" />
<wd:Tag Content="Non-closable" IsClose="False" />
```

### Mask（遮罩）

```xml
<Label Content="Label Mask" Background="{StaticResource WD.DangerBrush}"
       Width="100" Height="100"
       wd:Mask.IsShow="True"/>
```

### Loading（加载）

```xml
<Label Content="Label Loading" Background="{StaticResource WD.PrimaryBrush}"
       Width="100" Height="100"
       wd:Loading.IsShow="True"/>
```

### Badge（徽标）

```xml
<!-- 文字徽标 -->
<Button wd:Badge.BadgeText="5" Content="Messages" Theme="{StaticResource wd-default}" />
<Button wd:Badge.BadgeText="99+" Content="Notifications" Theme="{StaticResource wd-default}" />
<!-- 圆点徽标 -->
<Button wd:Badge.IsDot="True" Content="Dot Badge" Theme="{StaticResource wd-default}" />
```

### Progress & Slider（进度条 & 滑块）

```xml
<!-- 水平进度条 -->
<ProgressBar Value="60" Width="400" ShowProgressText="True"/>
<ProgressBar IsIndeterminate="True" Value="20" Width="400" />
<!-- 垂直进度条 -->
<ProgressBar Value="20" Height="300" Orientation="Vertical" ShowProgressText="True"/>
<!-- 滑块 -->
<Slider Width="400" Minimum="0" Maximum="100" Value="40" />
<Slider Width="400" Orientation="Vertical" Minimum="0" Maximum="100" Value="40" />
```

### Expander（展开面板）

```xml
<!-- 向下展开 -->
<Expander Header="Expander">
    <Rectangle Width="400" Height="300" Fill="{DynamicResource WD.DangerBrush}" />
</Expander>
<!-- 向上展开（默认展开） -->
<Expander ExpandDirection="Up" FlowDirection="RightToLeft" Header="Expander1" IsExpanded="True">
    <Rectangle Width="400" Height="300" Fill="{DynamicResource WD.LightBrush}" />
</Expander>
<!-- 向右展开 -->
<Expander ExpandDirection="Right" Header="Expander">
    <Rectangle Width="400" Height="300" Fill="{DynamicResource WD.SuccessMouseOverBrush}" />
</Expander>
<!-- 向左展开（默认展开） -->
<Expander ExpandDirection="Left" IsExpanded="True" Header="Expander">
    <Rectangle Width="400" Height="300" Fill="{DynamicResource WD.PrimaryBrush}" />
</Expander>
```

### ComboBox（下拉列表）

```xml
<ComboBox Width="200" SelectedIndex="0">
    <ComboBoxItem Content="Option 1" />
    <ComboBoxItem Content="Option 2" />
    <ComboBoxItem Content="Option 3" />
</ComboBox>
```

### DataGrid（数据表格）

```xml
<DataGrid Name="DemoDataGrid" Width="500" Height="200" x:DataType="local:DemoItem">
    <DataGrid.Columns>
        <DataGridTextColumn Header="Name" Binding="{Binding Name}" Width="150" />
        <DataGridTextColumn Header="Age" Binding="{Binding Age}" Width="80" />
        <DataGridCheckBoxColumn Header="Active" Binding="{Binding IsActive}" Width="80" />
        <DataGridTextColumn Header="Role" Binding="{Binding Role}" Width="*" />
    </DataGrid.Columns>
</DataGrid>
```

### Date & Time（日期时间）

```xml
<!-- 日期控件 -->
<DatePicker Width="200" />
<Calendar />
<CalendarDatePicker Width="200"/>
<!-- 时间选择器 -->
<TimePicker Width="200" />
<TimePicker Width="200" UseSeconds="True"/>
<TimePicker Width="200" ClockIdentifier="12HourClock"/>
```

### ListBox（列表）

```xml
<ListBox Width="200" Height="150" SelectionMode="Single">
    <ListBoxItem Content="Item One" />
    <ListBoxItem Content="Item Two" IsSelected="True" />
    <ListBoxItem Content="Item Three" />
</ListBox>
```

### Menu（菜单）

```xml
<Menu>
    <MenuItem Header="File">
        <MenuItem Header="New" />
        <MenuItem Header="Open" />
        <Separator />
        <MenuItem Header="Exit" />
    </MenuItem>
    <MenuItem Header="Edit">
        <MenuItem Header="Cut" />
        <MenuItem Header="Copy" />
    </MenuItem>
</Menu>
```

### TabControl（选项卡）

```xml
<TabControl Width="400" Height="150">
    <TabItem Header="General">
        <TextBlock Text="General settings content" Margin="8" />
    </TabItem>
    <TabItem Header="Advanced">
        <TextBlock Text="Advanced settings content" Margin="8" />
    </TabItem>
</TabControl>
```

### TreeView（树形视图）

```xml
<TreeView Width="250" Height="200">
    <TreeViewItem Header="Root">
        <TreeViewItem Header="Child 1">
            <TreeViewItem Header="Grandchild 1.1" />
            <TreeViewItem Header="Grandchild 1.2" />
        </TreeViewItem>
        <TreeViewItem Header="Child 2" />
        <TreeViewItem Header="Child 3" IsExpanded="True">
            <TreeViewItem Header="Grandchild 3.1" />
        </TreeViewItem>
    </TreeViewItem>
</TreeView>
```

### RepeatButton（长按按钮）

```xml
<RepeatButton Content="-" Width="40" Height="40" />
<RepeatButton Width="40" Height="40">
    <wd:PathIconEx Kind="Add" />
</RepeatButton>
```

### Label & Separator（标签 & 分隔符）

```xml
<Label Content="Primary Label" Foreground="{DynamicResource WD.PrimaryBrush}" />
<Label Content="Regular label" />
<!-- 超链接标签 -->
<Label Content="HyperlinkButton">
    <Label.ContentTemplate>
        <DataTemplate>
            <HyperlinkButton Content="{Binding}" NavigateUri="https://github.com/WPFDevelopersOrg/WPFDevelopers.Avalonia" />
        </DataTemplate>
    </Label.ContentTemplate>
</Label>
<Separator Margin="0,12" />
```

### NumericUpDown（数字输入）

```xml
<NumericUpDown Width="200" Value="42" Minimum="0" Maximum="1000" />
```

### AutoCompleteBox（自动补全）

```xml
<AutoCompleteBox Width="200" FilterMode="Contains" Watermark="Search..." />
```

### SplitView（分栏视图）

```xml
<SplitView IsPaneOpen="True" OpenPaneLength="200"
           Width="400" Height="250"
           DisplayMode="CompactOverlay">
    <SplitView.Pane>
        <StackPanel Background="{DynamicResource WD.BaseMoveBrush}">
            <TextBlock Text="Pane" FontWeight="SemiBold" Margin="8,0" />
            <Button Content="Home" Theme="{StaticResource wd-normal}" Margin="0,2" />
            <Button Content="Settings" Theme="{StaticResource wd-normal}" Margin="0,2" />
        </StackPanel>
    </SplitView.Pane>
    <Border Background="{DynamicResource WD.LightBrush}" Padding="16">
        <TextBlock Text="Main Content" VerticalAlignment="Center" HorizontalAlignment="Center" />
    </Border>
</SplitView>
```

### ContextMenu（右键菜单）

```xml
<Button Content="Right-click me for ContextMenu" Theme="{StaticResource wd-default}">
    <Button.ContextMenu>
        <ContextMenu>
            <MenuItem Header="Copy" />
            <MenuItem Header="Paste" />
            <MenuItem Header="Cut" />
            <Separator />
            <MenuItem Header="Select All" />
        </ContextMenu>
    </Button.ContextMenu>
</Button>
```

### Toast（消息通知）

```xml
<Button Content="Info" Theme="{StaticResource wd-primary}" Name="ToastInfoBtn" />
<Button Content="Success" Theme="{StaticResource wd-success}" Name="ToastSuccessBtn" />
<Button Content="Warning" Theme="{StaticResource wd-warning}" Name="ToastWarningBtn" />
<Button Content="Error" Theme="{StaticResource wd-danger}" Name="ToastErrorBtn" />
<Button Content="Clear" Theme="{StaticResource wd-normal}" Name="ToastClearBtn" />
```

### MessageBox（消息弹窗）

```xml
<Button Content="Info" Theme="{StaticResource wd-primary}" Name="MsgBoxInfoBtn" />
<Button Content="Warning" Theme="{StaticResource wd-warning}" Name="MsgBoxWarningBtn" />
<Button Content="Error" Theme="{StaticResource wd-danger}" Name="MsgBoxErrorBtn" />
<Button Content="Question" Theme="{StaticResource wd-success}" Name="MsgBoxQuestionBtn" />
<Button Content="YesNoCancel" Theme="{StaticResource wd-default}" Name="MsgBoxYesNoCancelBtn" />
```

### TrayIcon（托盘图标）

```csharp
// 在 App.OnFrameworkInitializationCompleted 或 Window 构造函数中初始化
if (Application.Current is { } app)
{
    var trayIcons = TrayIcon.GetIcons(app);
    if (trayIcons == null || trayIcons.Count == 0)
    {
        var tray = new TrayIcon
        {
            Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://YourApp/Assets/App.ico"))),
            Menu = new NativeMenu
            {
                new NativeMenuItem("Show"),
                new NativeMenuItemSeparator(),
                new NativeMenuItem("Exit")
            }
        };
        TrayIcon.SetIcons(app, [tray]);
    }
}

// 菜单项点击事件（在 Window 代码中绑定）
private void TrayShow_Click(object? sender, EventArgs e)
{
    Show();
    WindowState = WindowState.Normal;
    Activate();
}

private void TrayExit_Click(object? sender, EventArgs e)
{
    Environment.Exit(0);
}
```

> 注意：Avalonia 的 `TrayIcon` 不支持 XAML 属性方式设置，需通过 `TrayIcon.SetIcons()` 代码初始化。`Tip` 属性在当前版本不可用。

---

## 自定义主题色

### 方式一：代码动态修改

```csharp
ThemeManager.Instance.PrimaryColor = Color.Parse("#6366F1");
```

### 方式二：覆盖资源字典

```xml
<wd:Resources Theme="Light"/>
<ResourceDictionary>
    <Color x:Key="WD.PrimaryColor">#6366F1</Color>
</ResourceDictionary>
```

---

## 常用资源变量

| Key | 类型 | 说明 |
|-----|------|------|
| `WD.PrimaryBrush` | `Brush` | 主色画刷 |
| `WD.BackgroundBrush` | `Brush` | 背景画刷 |
| `WD.PrimaryTextBrush` | `Brush` | 主文字画刷 |
| `WD.RegularTextBrush` | `Brush` | 常规文字画刷 |
| `WD.SuccessBrush` | `Brush` | 成功色 |
| `WD.WarningBrush` | `Brush` | 警告色 |
| `WD.DangerBrush` | `Brush` | 危险色 |
| `WD.CornerRadius` | `CornerRadius` | 默认圆角 |
| `WD.ScrollBarSize` | `double` | 滚动条尺寸 |
| `WD.Padding` | `Thickness` | 默认内边距 |
