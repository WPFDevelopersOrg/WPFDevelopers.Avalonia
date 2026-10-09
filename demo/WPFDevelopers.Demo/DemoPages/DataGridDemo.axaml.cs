using System.Collections.Generic;
using Avalonia.Controls;

namespace WPFDevelopers.Demo.DemoPages
{
    public partial class DataGridDemo : UserControl
    {
        public DataGridDemo()
        {
            InitializeComponent();
            var grid = this.FindControl<DataGrid>("DemoDataGrid");
            if (grid != null)
            {
                grid.ItemsSource = new List<DemoItem>
                {
                    new() { Name = "Alice", Age = 28, IsActive = true, Role = "Admin" },
                    new() { Name = "Bob", Age = 34, IsActive = true, Role = "Editor" },
                    new() { Name = "Charlie", Age = 22, IsActive = false, Role = "Viewer" },
                    new() { Name = "Diana", Age = 41, IsActive = true, Role = "Admin" },
                    new() { Name = "Eve", Age = 30, IsActive = false, Role = "Editor" }
                };
            }
        }
    }
}
