using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using WPFDevelopers.Avalonia.Controls;

namespace WPFDevelopers.Demo.DemoPages
{
    public partial class HomeDemo : UserControl, IDisposable
    {
        private static readonly HttpClient Client = CreateClient();
        private readonly CancellationTokenSource _lifetime = new();
        private bool _started;
        private bool _disposed;

        public HomeDemo()
        {
            InitializeComponent();
            VersionTag.Content = MainWindow.LibraryVersion;
        }

        protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            if (_started || _disposed)
                return;

            _started = true;
            await RefreshStatisticsAsync();
        }

        private static HttpClient CreateClient()
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("WPFDevelopers.Avalonia-Demo/1.0");
            return client;
        }

        private async void RefreshButton_Click(object? sender, RoutedEventArgs e)
        {
            await RefreshStatisticsAsync();
        }

        private async Task RefreshStatisticsAsync()
        {
            if (_disposed || !RefreshButton.IsEnabled)
                return;

            RefreshButton.IsEnabled = false;
            StatisticsStatus.Text = "正在读取 GitHub Star 和 NuGet 累计下载量…";
            try
            {
                var results = await Task.WhenAll(
                    LoadMetricAsync(StarsTag, "GitHub", LoadStarsAsync),
                    LoadMetricAsync(DownloadsTag, "NuGet", LoadDownloadsAsync));
                if (_disposed)
                    return;

                var errors = results.Where(error => error != null).ToArray();
                StatisticsStatus.Text = errors.Length == 0
                    ? $"更新于 {DateTime.Now:HH:mm:ss} · 下载量为 NuGet 所有版本累计下载量"
                    : string.Join(Environment.NewLine, errors);
            }
            finally
            {
                if (!_disposed)
                    RefreshButton.IsEnabled = true;
            }
        }

        private async Task<string?> LoadMetricAsync(WPFDevelopers.Avalonia.Controls.Tag tag,
            string source, Func<CancellationToken, Task<long>> load)
        {
            tag.Content = "加载中";
            try
            {
                var count = await load(_lifetime.Token);
                if (!_disposed)
                    tag.Content = count.ToString("N0", CultureInfo.InvariantCulture);
                return null;
            }
            catch (OperationCanceledException) when (_lifetime.IsCancellationRequested)
            {
                return null;
            }
            catch (Exception ex) when (ex is HttpRequestException or JsonException or
                                       InvalidDataException or OperationCanceledException)
            {
                if (!_disposed)
                    tag.Content = "加载失败";
                return $"{source} 统计加载失败：{ex.Message}。请点击“刷新统计”重试。";
            }
        }

        private static async Task<long> LoadStarsAsync(CancellationToken cancellationToken)
        {
            using var response = await Client.GetAsync(
                "https://api.github.com/repos/WPFDevelopersOrg/WPFDevelopers.Avalonia", cancellationToken);
            response.EnsureSuccessStatusCode();
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
            return ReadCount(json.RootElement, "stargazers_count");
        }

        private static async Task<long> LoadDownloadsAsync(CancellationToken cancellationToken)
        {
            using var response = await Client.GetAsync(
                "https://azuresearch-usnc.nuget.org/query?q=packageid:WPFDevelopers.Avalonia&prerelease=true",
                cancellationToken);
            response.EnsureSuccessStatusCode();
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
            if (json.RootElement.ValueKind == JsonValueKind.Object &&
                json.RootElement.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
            {
                foreach (var package in data.EnumerateArray())
                {
                    if (package.ValueKind == JsonValueKind.Object &&
                        package.TryGetProperty("id", out var id) && id.ValueKind == JsonValueKind.String &&
                        string.Equals(id.GetString(), "WPFDevelopers.Avalonia", StringComparison.OrdinalIgnoreCase))
                        return ReadCount(package, "totalDownloads");
                }
            }

            throw new InvalidDataException("NuGet 未返回 WPFDevelopers.Avalonia 包统计");
        }

        private static long ReadCount(JsonElement element, string property)
        {
            if (element.ValueKind == JsonValueKind.Object &&
                element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.Number &&
                value.TryGetInt64(out var count) && count >= 0)
                return count;

            throw new InvalidDataException($"统计响应缺少有效的 {property}");
        }

        private async void RepositoryButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is not Button { Tag: string address })
                throw new InvalidOperationException("Repository button has no address.");

            LinkError.IsVisible = false;
            var launcher = TopLevel.GetTopLevel(this)?.Launcher;
            if (launcher == null || !await launcher.LaunchUriAsync(new Uri(address)))
            {
                LinkError.Text = $"无法打开浏览器，请手动访问：{address}";
                LinkError.IsVisible = true;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _lifetime.Cancel();
            _lifetime.Dispose();
        }
    }
}
