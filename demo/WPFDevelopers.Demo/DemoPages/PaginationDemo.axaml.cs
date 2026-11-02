using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using WPFDevelopers.Avalonia.Controls;

namespace WPFDevelopers.Demo.DemoPages;

public partial class PaginationDemo : UserControl
{
    private readonly List<PaginationDemoItem> _items = Enumerable.Range(1, 85)
        .Select(index => new PaginationDemoItem(
            index,
            $"User {index:000}",
            Departments[(index - 1) % Departments.Length],
            Roles[(index - 1) % Roles.Length]))
        .ToList();

    private static readonly string[] Departments = ["Design", "Engineering", "Marketing", "Support"];
    private static readonly string[] Roles = ["Admin", "Editor", "Reviewer", "Viewer"];

    public PaginationDemo()
    {
        InitializeComponent();
        UpdatePage(1, 10);
    }

    private void OnPageChanged(object? sender, PaginationChangedEventArgs e)
    {
        SelectionInfo.Text = $"Current page: {e.Current}, page size: {e.PageSize}";
        if (sender == PagedPagination)
            UpdatePage(e.Current, e.PageSize);
    }

    private void UpdatePage(int current, int pageSize)
    {
        PagedDataGrid.ItemsSource = _items
            .Skip((current - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
}

public sealed record PaginationDemoItem(int Index, string Name, string Department, string Role);
