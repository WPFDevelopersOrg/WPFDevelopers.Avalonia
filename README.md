<div align="center"><img src="https://github.com/WPFDevelopersOrg/ResourcesCache/raw/main/resources/Logo.png"/></div>

<div align="center">

[![nuget-version](https://img.shields.io/nuget/v/WPFDevelopers.Avalonia.svg?color=red)](https://www.nuget.org/packages/WPFDevelopers.Avalonia/) [![Github stars](https://img.shields.io/github/stars/WPFDevelopersOrg/WPFDevelopers.Avalonia)](https://github.com/WPFDevelopersOrg/WPFDevelopers.Avalonia/stargazers) ![Downloads](https://img.shields.io/nuget/dt/WPFDevelopers.Avalonia?color=%23409EF)

</div>

[English](README.md) | [Chinese](README-zh.md)

## Welcome to WPFDevelopers.Avalonia

An Avalonia 12.1.2 UI component library with a rich collection of control themes and practical controls. Both the library and the demo app target .NET 8, .NET 9, and .NET 10. Applications using the library must use a compatible Avalonia 12 version. Building the multi-targeted projects requires the .NET 10 SDK or later.

### Build and Run the Demo

Build all three frameworks from the repository root:

```powershell
dotnet build .\demo\WPFDevelopers.Demo\WPFDevelopers.Demo.csproj -c Release
```

The demo executables are generated in `Output\net8.0`, `Output\net9.0`, and `Output\net10.0`. All three share the same source code and reference the matching library target. Install the corresponding .NET runtime and specify `--framework` when running:

```powershell
dotnet run --project .\demo\WPFDevelopers.Demo\WPFDevelopers.Demo.csproj --framework net8.0 -c Release
dotnet run --project .\demo\WPFDevelopers.Demo\WPFDevelopers.Demo.csproj --framework net9.0 -c Release
dotnet run --project .\demo\WPFDevelopers.Demo\WPFDevelopers.Demo.csproj --framework net10.0 -c Release
```

The demo title bar displays `WPFDevelopers Demo · v<version>`. It shares the home page's version source: the loaded control library's assembly informational version, preserving prerelease labels and omitting build metadata after `+`.

### WPF Version

Using WPF as well? Check out **WPFDevelopers**, which provides a rich collection of controls and practical examples for Windows desktop apps. The demo home page also includes a link to the WPF version.

[GitHub](https://github.com/WPFDevelopersOrg/WPFDevelopers) · [AtomGit](https://atomgit.com/WPFDevelopersOrg/WPFDevelopers) · [Gitee](https://gitee.com/WPFDevelopersOrg/WPFDevelopers)

---

## Getting Started

### 1. Install the NuGet Package

```bash
dotnet add package WPFDevelopers.Avalonia
```

### 2. Add the Resources

Add the WD namespace and load the resources in `App.axaml`:

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:wd="https://github.com/WPFDevelopersOrg/WPFDevelopers.Avalonia"
             x:Class="YourApp.App">
    <Application.Resources>
        <wd:Resources Theme="Default"/>
    </Application.Resources>
</Application>
```

> `Theme` supports `Light`, `Dark`, and `Default` (follows the system theme).

---

## Controls

### Carousel

Carousel supports inline XAML content or image data bound through `ItemsSource` and `DisplayMemberPath`. Use `AutoPlay` and `AutoPlayInterval` to configure automatic playback, and handle clicks with `ItemClick` or `ItemClickCommand`.

```xml
<wd:Carousel Width="400"
             Height="200"
             AutoPlay="True"
             AutoPlayInterval="0:0:3"
             ShowArrows="True"
             ShowDots="True"
             ItemClick="OnCarouselItemClick">
    <Border Background="#722ed1">
        <TextBlock Text="Slide 1"
                   FontSize="24"
                   Foreground="White"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center" />
    </Border>
    <Border Background="#1890ff">
        <TextBlock Text="Slide 2"
                   FontSize="24"
                   Foreground="White"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center" />
    </Border>
</wd:Carousel>
```

When binding image data, use `DisplayMemberPath` to specify the image path property. `SelectedIndex` and `SelectedItem` support two-way binding. You can also call `GoToNext()` or `GoToPrevious()` in code to change slides.

```xml
<wd:Carousel Width="400"
             Height="200"
             ItemsSource="{Binding ImageItems}"
             DisplayMemberPath="URL"
             AutoPlay="True"
             AutoPlayInterval="0:0:3" />
```

When `ItemClick` fires, read the current item from the control's `SelectedItem`. Alternatively, bind `ItemClickCommand`; its parameter is the clicked data item.

```csharp
private void OnCarouselItemClick(object? sender, RoutedEventArgs e)
{
    if (sender is Carousel carousel)
        HandleItemClick(carousel.SelectedItem);
}
```

When autoplay or the next arrow advances from the last slide, the first slide enters from the right and resets seamlessly when the animation finishes. It does not move backward through the intervening slides or jump abruptly. Repeated previous/next actions are ignored during an animation to prevent overlapping transitions.

### Window

On Windows, `wd:Window` uses the WD title bar and window buttons; on other platforms it falls back to Avalonia's native `Window`. `TitleBarMode="High"` uses a taller title bar, and `TitleBarLayout` places toolbar content between the title and window buttons.

```xml
<wd:Window xmlns="https://github.com/avaloniaui"
           xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
           xmlns:wd="https://github.com/WPFDevelopersOrg/WPFDevelopers.Avalonia"
           x:Class="YourApp.MainWindow"
           Title="My App"
           Width="900"
           Height="600"
           TitleBarMode="High">
    <wd:Window.TitleBarLayout>
        <StackPanel Orientation="Horizontal" Spacing="8">
            <Button Content="Save" />
            <Button Content="Settings" />
        </StackPanel>
    </wd:Window.TitleBarLayout>
    <Grid>
        <TextBlock Text="Window content"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center" />
    </Grid>
</wd:Window>
```

`TitleBarMode="Normal"` uses the standard title bar. Set it to `None` to hide the standard title bar and provide custom content through `TitleBar`. `TitleHeight` and `TitleBackground` set the title bar's height and background, respectively.

```xml
<wd:Window Title="Custom title bar"
           Width="800"
           Height="500"
           TitleBarMode="None">
    <wd:Window.TitleBar>
        <Grid ColumnDefinitions="*,Auto" Height="40">
            <TextBlock Margin="12,0"
                       VerticalAlignment="Center"
                       Text="Custom title" />
            <Button Grid.Column="1" Content="Help" Margin="4" />
        </Grid>
    </wd:Window.TitleBar>
    <Grid>
        <TextBlock Text="Window content"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center" />
    </Grid>
</wd:Window>
```

### DataGrid

When the pointer enters a resizable edge of a column header, a 5 px theme-colored indicator appears and disappears when the pointer leaves. It uses `WD.PrimaryBrush`, follows theme and accent-color changes, and does not intercept pointer events or affect column resizing, sorting, or reordering. No indicator is shown on edges that cannot be resized.

### ScrollBar

The WD scrollbar uses a slim-thumb interaction: the thumb is 2 px thick by default, expands to 10 px within 0.1 seconds when the pointer enters the scrollbar area, stays expanded while dragging, and retracts when the pointer leaves. Both horizontal and vertical scrollbars retain a fixed 12 px hit area, and expansion does not shift content.

Only the thumb becomes 50% opaque while expanded or being dragged, keeping content visible beneath the overlay. The default slim thumb remains fully opaque for visibility. Content opacity and the scrollbar hit area are unchanged.

Both orientations use the dedicated `WD.ScrollBarThumbBrush`; hover and drag use `WD.ScrollBarThumbPointerOverBrush` instead of reusing text colors. The default thumb color is light gray (`#C0C4CC`) in light themes and `#606266` in dark themes; the interaction color is `#909399` in both. Colors follow theme changes. Override `WD.ScrollBarSize`, `WD.ScrollBarThumbSize`, and `WD.ScrollBarThumbExpandedSize` to adjust the hit area and thumb thickness.

### Buttons

```xml
<WrapPanel>
    <!-- Primary button -->
    <Button Content="Primary"  />
    <!-- Round button -->
    <Button Content="Primary" wd:ElementHelper.IsRound="True" Theme="{StaticResource wd-primary}" />
    <!-- Default outlined button -->
    <Button Content="Default" Theme="{StaticResource wd-default}" />
    <!-- Borderless button -->
    <Button Content="Normal" Theme="{StaticResource wd-normal}" />
    <!-- Success / warning / danger (outlined) -->
    <Button Content="Success" Theme="{StaticResource wd-success}" />
    <Button Content="Warning" Theme="{StaticResource wd-warning}" />
    <Button Content="Danger" Theme="{StaticResource wd-danger}" />
    <!-- Success / warning / danger (filled) -->
    <Button Content="Filled Success" Theme="{StaticResource wd-success-filled}" />
    <Button Content="Filled Warning" Theme="{StaticResource wd-warning-filled}" />
    <Button Content="Filled Danger" Theme="{StaticResource wd-danger-filled}" />
</WrapPanel>
```

### Icons

```xml
<!-- Built-in PathIconEx icons -->
<wd:PathIconEx Kind="Home" Width="24" Height="24" Foreground="{DynamicResource WD.PrimaryBrush}" />
<wd:PathIconEx Kind="CheckMark" Width="24" Height="24" Foreground="{DynamicResource WD.SuccessBrush}" />
<wd:PathIconEx Kind="Warning" Width="24" Height="24" Foreground="{DynamicResource WD.WarningBrush}" />
<wd:PathIconEx Kind="Error" Width="24" Height="24" Foreground="{DynamicResource WD.DangerBrush}" />
<wd:PathIconEx Kind="Star" Width="24" Height="24" />
<wd:PathIconEx Kind="Add" Width="24" Height="24" />
<wd:PathIconEx Kind="Time" Width="24" Height="24" />
<wd:PathIconEx Kind="Copy" Width="24" Height="24" />
<wd:PathIconEx Kind="Filter" Width="24" Height="24" />
<!-- Use built-in geometry resources -->
<PathIcon Data="{StaticResource WD.CheckMarkGeometry}" Foreground="{DynamicResource WD.SuccessBrush}" Width="24" Height="24" />
```

### Inputs

```xml
<!-- TextBox -->
<TextBox PlaceholderText="Enter text here..." />
<TextBox PlaceholderText="Clear text" Classes="IsClear" />
<TextBox PlaceholderText="Enter password here..." PasswordChar="●" RevealPassword="False" />

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
<!-- Switch style -->
<ToggleButton Theme="{StaticResource wd-switch}" IsChecked="True" />
<ToggleButton Theme="{StaticResource wd-switch}" />
```

### SwitchButton (Two-State Content Toggle)

`SwitchButton` inherits from `ToggleButton` and uses the same `UncheckedContent` / `CheckedContent` API as the WPF version. It displays `UncheckedContent` when unchecked and `CheckedContent` when checked. Both properties support text, `Image`, SVG controls, and composite layouts. SVG rendering is handled by the application, so the control library does not depend on an SVG package. Both states share the inherited `ContentTemplate`; the control does not overwrite `Content` or its binding.

```xml
<wd:SwitchButton UncheckedContent="Off"
                 CheckedContent="On"
                 IsChecked="{Binding IsEnabled, Mode=TwoWay}" />

<wd:SwitchButton AutomationProperties.Name="Favorite">
    <wd:SwitchButton.UncheckedContent>
        <wd:PathIconEx Kind="Star" Width="20" Height="20" />
    </wd:SwitchButton.UncheckedContent>
    <wd:SwitchButton.CheckedContent>
        <wd:PathIconEx Kind="Star" Width="20" Height="20"
                       Foreground="{DynamicResource WD.WarningBrush}" />
    </wd:SwitchButton.CheckedContent>
</wd:SwitchButton>
```

The default appearance follows WPF: no persistent background, with visual feedback on hover and press. Keyboard focus, the Space key, commands, and the disabled state are supported. Extend its styling with the `WD.SwitchButton` theme. Keep `IsThreeState="False"` in most cases; when three-state mode is enabled, `IsChecked="{x:Null}"` displays `UncheckedContent`, just like the unchecked state. Set `AutomationProperties.Name` on icon-only buttons.

Unlike the `wd-switch` toggle-switch style, `SwitchButton` toggles between two content views. The demo's Custom > SwitchButton page shows two-way text binding, icons, SVG, images, and light/dark themes. Theme changes are handled by the application, and the page stays in sync with other entry points and system-initiated changes. Use a multi-option selector for multiple accent colors or a "follow system" option.

### GestureUnlock (Pattern Unlock)

`Password` is a string of node indices (`0`–`8`) in the order they were selected. Validate it through the `Completed` event or `CompletedCommand`, then set `State` to display success or error feedback.

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

### OtpBox (One-Time Password Input)

The demo's Custom menu includes an `OtpBox` page with four-digit verification (the expected value is `1234`), a six-digit input with custom sizing, and a clear action.
The control supports numeric input, automatic focus advance, arrow-key navigation, Backspace, and `Ctrl+V` paste. `Value` is two-way by default and contains the complete code only when every box is filled; otherwise, it is an empty string.
Validate completed input with the `Completed` event or `CompletedCommand`. Set `State` to `OtpBoxState.Success` or `OtpBoxState.Error` to show feedback. The state resets after 1.5 seconds; an error also clears the input.

```xml
<wd:OtpBox Length="6"
           Value="{Binding VerificationCode, Mode=TwoWay}"
           CompletedCommand="{Binding VerifyCommand}"
           BoxWidth="36"
           BoxHeight="44"
           Spacing="6" />
```

### Timeline

Supports vertical and horizontal timelines. `Mode` can be `Start` (content on the right/bottom), `Alternate`, or `End` (content on the left/top). `Reverse` reverses the displayed order without changing the source collection. `ItemSpacing`, `LineBrush`, and `LineThickness` configure spacing and connector lines.

```xml
<wd:Timeline Mode="Alternate" ItemSpacing="20">
    <wd:TimelineItem Label="09:00" Content="Create service" />
    <wd:TimelineItem Label="10:00" Content="Testing complete"
                     DotBrush="{DynamicResource WD.SuccessBrush}" />
    <wd:TimelineItem Label="11:00" Content="Deploying" IsLoading="True" />
</wd:Timeline>
```

Set `Orientation="Horizontal"` for a horizontal layout. Items support `Label` / `LabelTemplate`, `Dot` / `DotTemplate`, and `ContentTemplate`. Set an item's `Position` to `"Start"` or `"End"` to override its position. `IsLoading` displays a rotating dot and renders adjacent connector lines as dashed.

To display a data collection with `ItemsSource` and `ItemTemplate`, use a `Style Selector="wd|TimelineItem"` in `Timeline.Styles` to bind properties such as `Label`, `DotBrush`, and `IsLoading` on generated items. The demo's Custom > Timeline page shows basic and horizontal layouts, plus dynamically adding and removing items.

### ColorPicker

`DisplayMode="Inline"` (the default) displays the color palette inline. `Popup` displays a swatch that opens the palette when clicked. `SelectedColor` is two-way by default, and the palette supports RGB, HSL, and HEX input formats.

```xml
<wd:ColorPicker SelectedColor="{Binding SelectedColor, Mode=TwoWay}" />
<wd:ColorPicker DisplayMode="Popup"
                ShowColorText="True"
                SelectedColor="{Binding SelectedColor, Mode=TwoWay}" />
<wd:ColorPickerButton ShowColorText="True"
                      SelectedColor="{Binding SelectedColor, Mode=TwoWay}" />
```

`ShowColorText` controls whether the popup swatch displays its HEX value. `SwatchSize` sets the swatch size, and `ColorType` sets the palette's initial format. `ColorPickerButton` remains available for compatibility; new code should use `ColorPicker DisplayMode="Popup"`.

The popup is aligned with the trigger's left edge by default, preventing it from shifting as the HEX text width changes during color selection. Avalonia adjusts its position near screen edges.

### SplitButton

The main button raises `Click`, and the dropdown items come from `ItemsSource`. Selecting a dropdown item updates the button content and raises `SelectionChanged`.

```xml
<wd:SplitButton Content="Save"
                ItemsSource="{Binding MenuItems}"
                Theme="{StaticResource WD.SplitButtonPrimary}"
                Click="OnMainClick"
                SelectionChanged="OnSelectionChanged" />
```

The `SelectionChanged` event args provide `OldValue` and `NewValue`. Available themes include `WD.SplitButtonPrimary`, `WD.SplitButtonSuccess`, `WD.SplitButtonWarning`, `WD.SplitButtonDanger`, and the corresponding `SuccessPrimary`, `WarningPrimary`, and `DangerPrimary` variants.

### MultiSelectComboBox

Supports search, multiple selection, select-all, and displaying selected items as delimited text or tags. Use `DisplayMemberPath` to specify which property to display for each item.

```xml
<wd:MultiSelectComboBox Width="320"
                        ItemsSource="{Binding TeamMembers}"
                        SelectedItemsExt="{Binding SelectedMembers}"
                        DisplayMemberPath="Name"
                        PlaceholderText="Select team members"
                        SearchWatermark="Search members"
                        SelectionMode="Multiple"
                        SelectAllContent="Select All"
                        Delimiter=", " />
```

Bind `SelectedItemsExt` to an `IList` in your view model to synchronize the selected items. Set `ShowType="Tag"` to display selections as tags, or `IsSearch="False"` to disable search. The demo's Custom > MultiSelectComboBox page also shows a custom `ItemTemplate`.

### Segmented

A single-selection segmented control that can be arranged horizontally or vertically. Read its selection using inherited ListBox properties such as `SelectedIndex` and `SelectedItem`.

```xml
<wd:Segmented SelectedIndex="0">
    <wd:SegmentedItem Content="Custom" />
    <wd:SegmentedItem Content="Basic controls" />
</wd:Segmented>
```

Set `Orientation="Vertical"` for a vertical layout. `AnimationDuration` controls the selection indicator's animation duration.

### Pagination

The pagination control supports page navigation, a total-count display, page-size selection, quick jump, and simple mode. `Current` and `PageSize` support two-way binding. Use `PageChanged` to get the current page and page size after user interaction.

```xml
<wd:Pagination Current="1" PageSize="10" Total="85"
               ShowTotal="True"
               ShowSizeChanger="True"
               ShowQuickJumper="True"
               PageChanged="OnPageChanged" />

<wd:Pagination Current="1" Total="85" Simple="True" />
```

The `PageChanged` event args, `PaginationChangedEventArgs`, provide the updated `Current` and `PageSize`, which you can use to load the corresponding data page. Set `PageSizeOptions` to customize the available page sizes, or `Disabled="True"` to disable pagination.

### GroupBox

```xml
<wd:GroupBox Header="Group Title" Width="500">
    <TextBlock Text="This is the content of the GroupBox." Margin="4" />
</wd:GroupBox>
<!-- The header can contain arbitrary content -->
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

### Tag

```xml
<wd:Tag Content="Default Tag" />
<wd:Tag Content="Non-closable" IsClose="False" />

<!-- Two-part badge in GitHub README / Shields style -->
<wd:Tag Classes="badge" Label="version" Content="12.1.2" Background="#E05D44" />
<wd:Tag Classes="badge" Label="stars" Content="1,234" Background="#DFB317" />
<wd:Tag Classes="badge" Label="downloads" Content="12,345" Background="#007EC6" />
```

`Classes="badge"` uses a compact two-part layout with a label on the left and content on the right, without a close button. It is still a `Tag` control: `Label`, `LabelBackground`, and `LabelForeground` configure the left side, while `Content`, `Background`, and `Foreground` configure the right. Bindings, content templates, font size, padding, and corner radius are supported. This README badge differs from control-attached badges provided by properties such as `wd:Badge.BadgeText`; the appearance and close behavior of regular tags are unchanged.

The values above are for styling examples only. The home page gets the version from the current control-library assembly (preserving prerelease version numbers), stars from the GitHub API, and total downloads across all versions from the NuGet search API's `totalDownloads`. Failed requests are never replaced with sample data.

### Mask

```xml
<Label Content="Label Mask" Background="{StaticResource WD.DangerBrush}"
       Width="100" Height="100"
       wd:Mask.IsShow="True"/>
```

### Loading

```xml
<Label Content="Label Loading" Background="{StaticResource WD.PrimaryBrush}"
       Width="100" Height="100"
       wd:Loading.IsShow="True"/>
```

`wd:Loading.IsShow` displays a loading overlay on a control. To show a standalone spinner instead, use `DefaultLoading` and control its animation with `IsActive`:

```xml
<wd:DefaultLoading IsActive="True"
                   StrokeBrush="{DynamicResource WD.PrimaryBrush}"
                   ArcThickness="2" />
```

### Badge

```xml
<!-- Text badge -->
<Button wd:Badge.BadgeText="5" Content="Messages" Theme="{StaticResource wd-default}" />
<Button wd:Badge.BadgeText="99+" Content="Notifications" Theme="{StaticResource wd-default}" />
<!-- Dot badge -->
<Button wd:Badge.IsDot="True" Content="Dot Badge" Theme="{StaticResource wd-default}" />
```

### Progress & Slider

```xml
<!-- Horizontal progress bar -->
<ProgressBar Value="60" Width="400" ShowProgressText="True"/>
<ProgressBar IsIndeterminate="True" Value="20" Width="400" />
<!-- Vertical progress bar -->
<ProgressBar Value="20" Height="300" Orientation="Vertical" ShowProgressText="True"/>
<!-- Slider -->
<Slider Width="400" Minimum="0" Maximum="100" Value="40" />
<Slider Width="400" Orientation="Vertical" Minimum="0" Maximum="100" Value="40" />
```

### Step

```xml
<wd:Step StepIndex="1">
    <wd:StepItem Content="Account" />
    <wd:StepItem Content="Profile" />
    <wd:StepItem Content="Confirmation" />
</wd:Step>
```

`StepIndex` is zero-based; preceding steps are shown as completed. Set `Orientation="Vertical"` for a vertical layout.

### Drawer

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

`Position` supports `Left`, `Top`, `Right`, and `Bottom`. Click the overlay to close the drawer. `HeaderWidth` limits the width of left and right drawers; if unset, the maximum width defaults to two-thirds of the control's width.
The drawer panel and shadow are clipped inside the border. The closed offset accounts for the shadow so no colored strip remains at the edge.
On initial layout, the drawer immediately reflects `IsOpen` without an animation. Animations play when `IsOpen` changes afterward.

### Expander

```xml
<!-- Expand down -->
<Expander Header="Expander">
    <Rectangle Width="400" Height="300" Fill="{DynamicResource WD.DangerBrush}" />
</Expander>
<!-- Expand up (expanded by default) -->
<Expander ExpandDirection="Up" FlowDirection="RightToLeft" Header="Expander1" IsExpanded="True">
    <Rectangle Width="400" Height="300" Fill="{DynamicResource WD.LightBrush}" />
</Expander>
<!-- Expand right -->
<Expander ExpandDirection="Right" Header="Expander">
    <Rectangle Width="400" Height="300" Fill="{DynamicResource WD.SuccessMouseOverBrush}" />
</Expander>
<!-- Expand left (expanded by default) -->
<Expander ExpandDirection="Left" IsExpanded="True" Header="Expander">
    <Rectangle Width="400" Height="300" Fill="{DynamicResource WD.PrimaryBrush}" />
</Expander>
```

### ComboBox

```xml
<ComboBox Width="200" SelectedIndex="0">
    <ComboBoxItem Content="Option 1" />
    <ComboBoxItem Content="Option 2" />
    <ComboBoxItem Content="Option 3" />
</ComboBox>
```

### DataGrid

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

### Date & Time

```xml
<!-- Date controls -->
<DatePicker Width="200" />
<Calendar />
<CalendarDatePicker Width="200"/>
<!-- Time pickers -->
<TimePicker Width="200" />
<TimePicker Width="200" UseSeconds="True"/>
<TimePicker Width="200" ClockIdentifier="12HourClock"/>
```

### ListBox

```xml
<ListBox Width="200" Height="150" SelectionMode="Single">
    <ListBoxItem Content="Item One" />
    <ListBoxItem Content="Item Two" IsSelected="True" />
    <ListBoxItem Content="Item Three" />
</ListBox>
```

### Menu

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

### TabControl

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

### TreeView

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

### RepeatButton

```xml
<RepeatButton Content="-" Width="40" Height="40" />
<RepeatButton Width="40" Height="40">
    <wd:PathIconEx Kind="Add" />
</RepeatButton>
```

### Label & Separator

```xml
<Label Content="Primary Label" Foreground="{DynamicResource WD.PrimaryBrush}" />
<Label Content="Regular label" />
<!-- Hyperlink label -->
<Label Content="HyperlinkButton">
    <Label.ContentTemplate>
        <DataTemplate>
            <HyperlinkButton Content="{Binding}" NavigateUri="https://github.com/WPFDevelopersOrg/WPFDevelopers.Avalonia" />
        </DataTemplate>
    </Label.ContentTemplate>
</Label>
<Separator Margin="0,12" />
```

### NumericUpDown

```xml
<NumericUpDown Width="200" Value="42" Minimum="0" Maximum="1000" />
```

### AutoCompleteBox

```xml
<AutoCompleteBox Width="200" FilterMode="Contains" PlaceholderText="Search..." />
```

### SplitView

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

### GridSplitter

Reserve an `Auto`-sized row or column for the splitter in the Grid. Drag it to resize adjacent rows or columns. Set `ResizeDirection` to match the splitter's row or column, use `ResizeBehavior` to choose which definitions to resize, and enable `ShowsPreview` to show a preview while dragging.

```xml
<StackPanel Spacing="16">
    <!-- Vertical splitter: resize the left and right columns -->
    <Grid Height="180" ColumnDefinitions="*,Auto,*">
        <Border Grid.Column="0" MinWidth="60" Background="{DynamicResource WD.PrimaryMouseOverBrush}">
            <TextBlock Text="Left pane" HorizontalAlignment="Center" VerticalAlignment="Center" />
        </Border>
        <GridSplitter Grid.Column="1"
                      Width="5"
                      Margin="2,0"
                      ResizeBehavior="PreviousAndNext"
                      ResizeDirection="Columns"
                      ShowsPreview="True" />
        <Border Grid.Column="2" MinWidth="60" Background="{DynamicResource WD.PrimaryMouseOverBrush}">
            <TextBlock Text="Right pane" HorizontalAlignment="Center" VerticalAlignment="Center" />
        </Border>
    </Grid>

    <!-- Horizontal splitter: resize the top and bottom rows -->
    <Grid Height="240" RowDefinitions="*,Auto,*">
        <Border Grid.Row="0" MinHeight="40" Background="{DynamicResource WD.PrimaryMouseOverBrush}">
            <TextBlock Text="Top pane" HorizontalAlignment="Center" VerticalAlignment="Center" />
        </Border>
        <GridSplitter Grid.Row="1"
                      Height="5"
                      Margin="0,2"
                      ResizeBehavior="PreviousAndNext"
                      ResizeDirection="Rows"
                      ShowsPreview="False" />
        <Border Grid.Row="2" MinHeight="40" Background="{DynamicResource WD.PrimaryMouseOverBrush}">
            <TextBlock Text="Bottom pane" HorizontalAlignment="Center" VerticalAlignment="Center" />
        </Border>
    </Grid>
</StackPanel>
```

### ContextMenu

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

### Toast

```xml
<Button Content="Info" Theme="{StaticResource wd-primary}" Name="ToastInfoBtn" />
<Button Content="Success" Theme="{StaticResource wd-success}" Name="ToastSuccessBtn" />
<Button Content="Warning" Theme="{StaticResource wd-warning}" Name="ToastWarningBtn" />
<Button Content="Error" Theme="{StaticResource wd-danger}" Name="ToastErrorBtn" />
<Button Content="Clear" Theme="{StaticResource wd-normal}" Name="ToastClearBtn" />
```

### MessageBox

```xml
<Button Content="Info" Theme="{StaticResource wd-primary}" Name="MsgBoxInfoBtn" />
<Button Content="Warning" Theme="{StaticResource wd-warning}" Name="MsgBoxWarningBtn" />
<Button Content="Error" Theme="{StaticResource wd-danger}" Name="MsgBoxErrorBtn" />
<Button Content="Question" Theme="{StaticResource wd-success}" Name="MsgBoxQuestionBtn" />
<Button Content="YesNoCancel" Theme="{StaticResource wd-default}" Name="MsgBoxYesNoCancelBtn" />
```

### TrayIcon

```csharp
// Initialize in App.OnFrameworkInitializationCompleted or a Window constructor
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

// Bind menu item click handlers in the Window code
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

> Note: Avalonia's `TrayIcon` cannot be configured using XAML properties. Initialize it in code with `TrayIcon.SetIcons()`. The `Tip` property is unavailable in the current version.

---

## Customizing the Accent Color

### Option 1: Change It in Code

```csharp
ThemeManager.Instance.PrimaryColor = Color.Parse("#6366F1");
```

### Option 2: Override the Resource Dictionary

```xml
<wd:Resources Theme="Light"/>
<ResourceDictionary>
    <Color x:Key="WD.PrimaryColor">#6366F1</Color>
</ResourceDictionary>
```

---

## Common Resource Keys

| Key | Type | Description |
|-----|------|------|
| `WD.PrimaryBrush` | `Brush` | Accent brush |
| `WD.BackgroundBrush` | `Brush` | Background brush |
| `WD.PrimaryTextBrush` | `Brush` | Primary text brush |
| `WD.RegularTextBrush` | `Brush` | Regular text brush |
| `WD.SuccessBrush` | `Brush` | Success color |
| `WD.WarningBrush` | `Brush` | Warning color |
| `WD.DangerBrush` | `Brush` | Danger color |
| `WD.CornerRadius` | `CornerRadius` | Default corner radius |
| `WD.ScrollBarSize` | `double` | Scrollbar size |
| `WD.Padding` | `Thickness` | Default padding |
