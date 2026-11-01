<div align="center"><img src="https://github.com/WPFDevelopersOrg/ResourcesCache/raw/main/resources/Logo.png"/></div>

<div align="center">

[![nuget-version](https://img.shields.io/nuget/v/WPFDevelopers.Avalonia.svg?color=red)](https://www.nuget.org/packages/WPFDevelopers.Avalonia/) [![Github stars](https://img.shields.io/github/stars/WPFDevelopersOrg/WPFDevelopers.Avalonia)](https://github.com/WPFDevelopersOrg/WPFDevelopers.Avalonia/stargazers) ![Downloads](https://img.shields.io/nuget/dt/WPFDevelopers.Avalonia?color=%23409EF)

</div>

[English](README.md) | [简体中文](README-zh.md)

## Welcome to WPFDevelopers.Avalonia

基于 Avalonia 12.1.2 的 UI 组件库，提供丰富的控件主题和实用控件。控件库和示例程序均支持 .NET 8、.NET 9 和 .NET 10；使用方应用需采用兼容的 Avalonia 12 版本。构建多目标项目需安装 .NET 10 SDK 或更新版本。

### WPF 版本

也在使用 WPF？欢迎体验 **WPFDevelopers**，为 Windows 桌面应用提供丰富的控件与实用示例。Demo 首页也提供独立的 WPF 版本入口。

[GitHub](https://github.com/WPFDevelopersOrg/WPFDevelopers) · [AtomGit](https://atomgit.com/WPFDevelopersOrg/WPFDevelopers) · [Gitee](https://gitee.com/WPFDevelopersOrg/WPFDevelopers)

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

### Carousel（走马灯）

支持直接嵌套 XAML 内容，或使用 `ItemsSource`、`DisplayMemberPath` 绑定图片；使用 `AutoPlay` 和 `AutoPlayInterval` 控制自动播放，使用 `ItemClick` 或 `ItemClickCommand` 响应点击。

```xml
<wd:Carousel Width="400"
             Height="200"
             AutoPlay="True"
             AutoPlayInterval="0:0:3"
             ShowArrows="True"
             ShowDots="True"
             ItemClick="OnCarouselItemClick">
    <Border Background="#722ed1">
        <TextBlock Text="第一页"
                   FontSize="24"
                   Foreground="White"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center" />
    </Border>
    <Border Background="#1890ff">
        <TextBlock Text="第二页"
                   FontSize="24"
                   Foreground="White"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center" />
    </Border>
</wd:Carousel>
```

绑定图片数据时，可通过 `DisplayMemberPath` 指定图片路径属性；`SelectedIndex` 和 `SelectedItem` 支持双向绑定，代码中也可调用 `GoToNext()` / `GoToPrevious()` 切换页面。

```xml
<wd:Carousel Width="400"
             Height="200"
             ItemsSource="{Binding ImageItems}"
             DisplayMemberPath="URL"
             AutoPlay="True"
             AutoPlayInterval="0:0:3" />
```

`ItemClick` 事件触发时可从控件的 `SelectedItem` 读取当前项；也可绑定 `ItemClickCommand`，命令参数为被点击的数据项。

```csharp
private void OnCarouselItemClick(object? sender, RoutedEventArgs e)
{
    if (sender is Carousel carousel)
        HandleItemClick(carousel.SelectedItem);
}
```

自动播放和下一页箭头从最后一页继续向左滑动时，第一页从右侧进入，动画结束后无感复位，不会倒着穿过中间页面或直接跳回。滑动期间忽略重复的上一页/下一页操作，避免重叠动画。

### Window（自定义窗口）

`wd:Window` 在 Windows 上使用 WD 标题栏和窗口按钮；其他平台回退为 Avalonia 原生 `Window`。`TitleBarMode="High"` 使用较高标题栏，`TitleBarLayout` 可在标题与窗口按钮之间放置工具栏内容。

```xml
<wd:Window xmlns="https://github.com/avaloniaui"
           xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
           xmlns:wd="https://github.com/WPFDevelopersOrg/WPFDevelopers.Avalonia"
           x:Class="YourApp.MainWindow"
           Title="我的应用"
           Width="900"
           Height="600"
           TitleBarMode="High">
    <wd:Window.TitleBarLayout>
        <StackPanel Orientation="Horizontal" Spacing="8">
            <Button Content="保存" />
            <Button Content="设置" />
        </StackPanel>
    </wd:Window.TitleBarLayout>
    <Grid>
        <TextBlock Text="窗口内容"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center" />
    </Grid>
</wd:Window>
```

`TitleBarMode="Normal"` 使用普通标题栏；设为 `None` 可隐藏标准标题栏并改用 `TitleBar` 提供自定义标题栏内容。`TitleHeight` 和 `TitleBackground` 分别设置标题栏高度与背景色。

```xml
<wd:Window Title="自定义标题栏"
           Width="800"
           Height="500"
           TitleBarMode="None">
    <wd:Window.TitleBar>
        <Grid ColumnDefinitions="*,Auto" Height="40">
            <TextBlock Margin="12,0"
                       VerticalAlignment="Center"
                       Text="自定义标题" />
            <Button Grid.Column="1" Content="帮助" Margin="4" />
        </Grid>
    </wd:Window.TitleBar>
    <Grid>
        <TextBlock Text="窗口内容"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center" />
    </Grid>
</wd:Window>
```

### DataGrid（数据表格）

鼠标移入列头可调整宽度的边缘区域时，显示 5px 宽的主题色提示，移出后隐藏。提示使用 `WD.PrimaryBrush`，跟随明暗主题和主题色切换，不拦截鼠标事件，保留原有列宽调整、排序和列重排行为；不可调整宽度的边缘不显示提示。

### ScrollBar（滚动条）

滚动条沿用 WD 的细条交互：默认滑块厚度为 2px，鼠标进入滚动条区域时在 0.1 秒内展开到 10px，拖动期间保持展开，离开后收回。横向和竖向均保留固定的 12px 感应区域，展开不会挤动内容。

展开及拖动时仅滑块使用 50% 不透明度，使叠加区域下方的内容仍然可见；默认细条保持完全不透明，确保可辨识性。不改变内容透明度或滚动条的鼠标感应区域。

横竖滑块统一使用专用的 `WD.ScrollBarThumbBrush`，悬停和拖动时使用 `WD.ScrollBarThumbPointerOverBrush`，不再复用文字颜色。亮色主题默认浅灰 `#C0C4CC`，暗色主题默认 `#606266`，交互状态均为 `#909399`，颜色随明暗主题切换。可覆盖 `WD.ScrollBarSize`、`WD.ScrollBarThumbSize` 和 `WD.ScrollBarThumbExpandedSize` 调整感应区域及滑块厚度。

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

### SwitchButton（双内容切换按钮）

继承 `ToggleButton`，沿用 WPF 版的 `UncheckedContent` / `CheckedContent` API。未选中时显示 `UncheckedContent`，选中时显示 `CheckedContent`，支持文本、`Image`、SVG 控件和组合布局；SVG 使用应用自己的渲染控件，控件库不依赖 SVG 包。两种内容共用继承的 `ContentTemplate`，不会改写 `Content` 或它的绑定。

```xml
<wd:SwitchButton UncheckedContent="未开启"
                 CheckedContent="已开启"
                 IsChecked="{Binding IsEnabled, Mode=TwoWay}" />

<wd:SwitchButton AutomationProperties.Name="收藏">
    <wd:SwitchButton.UncheckedContent>
        <wd:PathIconEx Kind="Star" Width="20" Height="20" />
    </wd:SwitchButton.UncheckedContent>
    <wd:SwitchButton.CheckedContent>
        <wd:PathIconEx Kind="Star" Width="20" Height="20"
                       Foreground="{DynamicResource WD.WarningBrush}" />
    </wd:SwitchButton.CheckedContent>
</wd:SwitchButton>
```

默认外观参考 WPF：无常驻底色，悬停或按下时显示背景反馈，支持键盘焦点、空格键、命令及禁用状态。可通过 `WD.SwitchButton` 主题扩展样式。通常保持 `IsThreeState="False"`；若启用三态，`IsChecked="{x:Null}"` 与未选中一样显示 `UncheckedContent`。纯图标按钮应设置 `AutomationProperties.Name`。

这与 `wd-switch` 滑块样式不同：`SwitchButton` 用于切换两套内容。Demo 的“自定义 → SwitchButton”包含文本双向绑定、图标、SVG、图片和明暗主题示例；主题逻辑由应用处理，页面会同步其他入口及系统引起的实际主题变化。多种主题色或“跟随系统”模式仍应使用多选项选择器。

### GestureUnlock（手势解锁）

`Password` 是按经过节点的顺序拼接而成的索引字符串（节点编号为 `0`–`8`）。完成后可通过 `Completed` 事件或 `CompletedCommand` 校验，并设置 `State` 显示成功或错误状态。

```xml
<wd:GestureUnlock Width="300"
                  Height="300"
                  Completed="GestureUnlock_Completed" />
```

```csharp
private void GestureUnlock_Completed(object? sender, RoutedEventArgs e)
{
    if (sender is GestureUnlock unlock)
        unlock.State = unlock.Password == "0426" ? ControlState.Success : ControlState.Error;
}
```

### OtpBox（验证码输入）

示例程序的“自定义”菜单提供 `OtpBox` 页面，包含四位验证码校验（正确值为 `1234`）、六位自定义尺寸输入和清空操作。
支持数字输入、自动跳转、方向键导航、退格和 `Ctrl+V` 粘贴。`Value` 默认双向绑定，所有格子填满后才包含完整验证码，未填满时为空字符串。
完成输入后可通过 `Completed` 事件或 `CompletedCommand` 校验；设置 `State` 为 `OtpBoxState.Success` 或 `OtpBoxState.Error` 可显示反馈，1.5 秒后状态复位，错误时还会清空输入。

```xml
<wd:OtpBox Length="6"
           Value="{Binding VerificationCode, Mode=TwoWay}"
           CompletedCommand="{Binding VerifyCommand}"
           BoxWidth="36"
           BoxHeight="44"
           Spacing="6" />
```

### Timeline（时间线）

支持纵向和横向时间线，`Mode` 可选 `Start`（内容在右侧/下方）、`Alternate`（交错）和 `End`（内容在左侧/上方）。`Reverse` 只反转显示顺序，不修改源集合；`ItemSpacing`、`LineBrush` 和 `LineThickness` 控制间距与连接线。

```xml
<wd:Timeline Mode="Alternate" ItemSpacing="20">
    <wd:TimelineItem Label="09:00" Content="创建服务" />
    <wd:TimelineItem Label="10:00" Content="测试完成"
                     DotBrush="{DynamicResource WD.SuccessBrush}" />
    <wd:TimelineItem Label="11:00" Content="部署中" IsLoading="True" />
</wd:Timeline>
```

设置 `Orientation="Horizontal"` 切换横向布局。节点支持 `Label`/`LabelTemplate`、`Dot`/`DotTemplate` 和 `ContentTemplate`，`Position="Start"` 或 `"End"` 可覆盖单个节点的位置。`IsLoading` 显示旋转节点，并将相邻连接线显示为虚线。

使用 `ItemsSource` 和 `ItemTemplate` 展示数据集合时，通过 `Timeline.Styles` 中的 `Style Selector="wd|TimelineItem"` 为生成的节点绑定 `Label`、`DotBrush` 和 `IsLoading` 等属性。示例程序的“自定义 → Timeline”页面提供基础、横向和动态增删节点演示。

### ColorPicker（颜色选择器）

`DisplayMode="Inline"`（默认）显示内嵌调色板；`Popup` 显示点击后展开调色板的色块触发器。`SelectedColor` 默认支持双向绑定，调色板可切换 RGB、HSL 和 HEX 输入格式。

```xml
<wd:ColorPicker SelectedColor="{Binding SelectedColor, Mode=TwoWay}" />
<wd:ColorPicker DisplayMode="Popup"
                ShowColorText="True"
                SelectedColor="{Binding SelectedColor, Mode=TwoWay}" />
<wd:ColorPickerButton ShowColorText="True"
                      SelectedColor="{Binding SelectedColor, Mode=TwoWay}" />
```

`ShowColorText` 控制 Popup 色块是否显示 HEX 值，`SwatchSize` 设置色块大小，`ColorType` 设置调色板初始格式。`ColorPickerButton` 是兼容入口，新代码建议使用 `ColorPicker DisplayMode="Popup"`。

弹层默认与触发器左边缘对齐，避免连续选色时 HEX 文本宽度变化导致弹层左右移动；靠近屏幕边缘时由 Avalonia 自动调整位置。

### SplitButton（分割按钮）

主按钮触发 `Click`，下拉项由 `ItemsSource` 提供；选择下拉项后更新按钮内容并触发 `SelectionChanged`。

```xml
<wd:SplitButton Content="Save"
                ItemsSource="{Binding MenuItems}"
                Theme="{StaticResource WD.SplitButtonPrimary}"
                Click="OnMainClick"
                SelectionChanged="OnSelectionChanged" />
```

`SelectionChanged` 事件参数包含 `OldValue` 和 `NewValue`。主题可选 `WD.SplitButtonPrimary`、`WD.SplitButtonSuccess`、`WD.SplitButtonWarning`、`WD.SplitButtonDanger` 及对应的 `SuccessPrimary`、`WarningPrimary`、`DangerPrimary` 变体。

### MultiSelectComboBox（多选下拉框）

支持搜索、多选、全选，以及以分隔文本或标签显示已选项。通过 `DisplayMemberPath` 指定数据项用于显示的属性。

```xml
<wd:MultiSelectComboBox Width="320"
                        ItemsSource="{Binding TeamMembers}"
                        SelectedItemsExt="{Binding SelectedMembers}"
                        DisplayMemberPath="Name"
                        PlaceholderText="选择成员"
                        SearchWatermark="搜索成员"
                        SelectionMode="Multiple"
                        SelectAllContent="全选"
                        Delimiter=", " />
```

将 `SelectedItemsExt` 绑定到 ViewModel 的 `IList` 可同步已选项；`ShowType="Tag"` 可用标签显示已选项，`IsSearch="False"` 可关闭搜索。示例程序的“自定义 → MultiSelectComboBox”页面还展示了自定义 `ItemTemplate`。

### Segmented（分段选择器）

单选分段控件，支持水平或垂直排列，并可通过 `SelectedIndex`、`SelectedItem` 等 ListBox 属性获取选择状态。

```xml
<wd:Segmented SelectedIndex="0">
    <wd:SegmentedItem Content="自定义" />
    <wd:SegmentedItem Content="基础控件" />
</wd:Segmented>
```

`Orientation="Vertical"` 切换为纵向排列，`AnimationDuration` 控制选中指示器的动画时长。

### Pagination（分页）

分页控件支持页码导航、总数提示、每页条数切换、快速跳转和简洁模式。`Current` 与 `PageSize` 支持双向绑定，用户操作后通过 `PageChanged` 获取新页码和每页条数。

```xml
<wd:Pagination Current="1" PageSize="10" Total="85"
               ShowTotal="True"
               ShowSizeChanger="True"
               ShowQuickJumper="True"
               PageChanged="OnPageChanged" />

<wd:Pagination Current="1" Total="85" Simple="True" />
```

`PageChanged` 事件参数 `PaginationChangedEventArgs` 提供更新后的 `Current` 和 `PageSize`，可据此加载对应数据页。`PageSizeOptions` 可自定义每页条数选项，`Disabled="True"` 可禁用分页操作。

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

<!-- GitHub README / Shields 风格的双段 Badge -->
<wd:Tag Classes="badge" Label="version" Content="12.1.2" Background="#E05D44" />
<wd:Tag Classes="badge" Label="stars" Content="1,234" Background="#DFB317" />
<wd:Tag Classes="badge" Label="downloads" Content="12,345" Background="#007EC6" />
```

`Classes="badge"` 使用左侧名称、右侧内容的紧凑双段布局，不显示关闭按钮。它仍以 `Tag` 控件承载内容；`Label`、`LabelBackground`、`LabelForeground` 控制左段，`Content`、`Background`、`Foreground` 控制右段，支持绑定、内容模板、字号、内边距和圆角。这里的 README Badge 与 `wd:Badge.BadgeText` 等附加属性提供的控件角标用途不同；普通 Tag 的外观和关闭行为不变。

上面数值仅为样式示例；首页版本来自当前控件库程序集（保留预发布版本号），Star 来自 GitHub API，下载量来自 NuGet 搜索 API 的 `totalDownloads`（全部版本累计值），不会用示例数据替代失败的请求。

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

`wd:Loading.IsShow` 会在控件上显示加载遮罩。若只需要独立的旋转指示器，可使用 `DefaultLoading`，并通过 `IsActive` 控制动画：

```xml
<wd:DefaultLoading IsActive="True"
                   StrokeBrush="{DynamicResource WD.PrimaryBrush}"
                   ArcThickness="2" />
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

### Step（步骤条）

```xml
<wd:Step StepIndex="1">
    <wd:StepItem Content="Account" />
    <wd:StepItem Content="Profile" />
    <wd:StepItem Content="Confirmation" />
</wd:Step>
```

`StepIndex` 从 0 开始，前面的步骤显示为已完成；设置 `Orientation="Vertical"` 可切换为纵向布局。

### Drawer（抽屉）

```xml
<wd:Drawer Position="Left" IsOpen="{Binding IsDrawerOpen}" HeaderWidth="280">
    <wd:Drawer.Header>
        <StackPanel Width="280" Spacing="8">
            <TextBlock FontSize="18" Text="Drawer" />
            <TextBlock Text="Click the shaded area to close." />
        </StackPanel>
    </wd:Drawer.Header>
    <TextBlock Text="Drawer content" />
</wd:Drawer>
```

`Position` 支持 `Left`、`Top`、`Right` 和 `Bottom`；点击遮罩区域可关闭抽屉。`HeaderWidth` 用于限制左右抽屉的宽度，未设置时最大宽度默认为控件宽度的三分之二。
抽屉面板及阴影裁剪在边框内部，收起距离包含阴影范围，关闭后不会在边缘残留色条。
首次布局直接呈现 `IsOpen` 对应的状态，不播放开合动画；后续切换 `IsOpen` 时播放动画。

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

### GridSplitter（网格分隔器）

在 Grid 中为分隔器所在行或列预留 `Auto` 空间；拖动分隔器可调整相邻行列的大小。`ResizeDirection` 应与分隔器所在列或行对应，`ResizeBehavior` 可指定调整哪两侧的定义，`ShowsPreview` 可启用拖动预览。

```xml
<StackPanel Spacing="16">
    <!-- 竖向分隔：调整左右列 -->
    <Grid Height="180" ColumnDefinitions="*,Auto,*">
        <Border Grid.Column="0" MinWidth="60" Background="{DynamicResource WD.PrimaryMouseOverBrush}">
            <TextBlock Text="左侧" HorizontalAlignment="Center" VerticalAlignment="Center" />
        </Border>
        <GridSplitter Grid.Column="1"
                      Width="5"
                      Margin="2,0"
                      ResizeBehavior="PreviousAndNext"
                      ResizeDirection="Columns"
                      ShowsPreview="True" />
        <Border Grid.Column="2" MinWidth="60" Background="{DynamicResource WD.PrimaryMouseOverBrush}">
            <TextBlock Text="右侧" HorizontalAlignment="Center" VerticalAlignment="Center" />
        </Border>
    </Grid>

    <!-- 横向分隔：调整上下行 -->
    <Grid Height="240" RowDefinitions="*,Auto,*">
        <Border Grid.Row="0" MinHeight="40" Background="{DynamicResource WD.PrimaryMouseOverBrush}">
            <TextBlock Text="上方" HorizontalAlignment="Center" VerticalAlignment="Center" />
        </Border>
        <GridSplitter Grid.Row="1"
                      Height="5"
                      Margin="0,2"
                      ResizeBehavior="PreviousAndNext"
                      ResizeDirection="Rows"
                      ShowsPreview="False" />
        <Border Grid.Row="2" MinHeight="40" Background="{DynamicResource WD.PrimaryMouseOverBrush}">
            <TextBlock Text="下方" HorizontalAlignment="Center" VerticalAlignment="Center" />
        </Border>
    </Grid>
</StackPanel>
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
