using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BenchmarkDotNet.Live.Shared
{
    public sealed partial class BenchmarkControl : ContentControl
    {
        private const string BenchmarksBaseNamespace = "SamplesApp.Benchmarks.Suite";
        private TextBlockLogger _logger;

        public BenchmarkControl()
        {
            this.InitializeComponent();

            _logger = new TextBlockLogger(runLogs);

            var name = GetType().Assembly.GetManifestResourceNames().FirstOrDefault(m => m.EndsWith("DefaultContent.cs", StringComparison.OrdinalIgnoreCase));

            if(name != null)
            {
                using (var s = new StreamReader(GetType().Assembly.GetManifestResourceStream(name)))
                {
                    sources.Text = s.ReadToEnd();
                }
            }
        }

        private void OnRunTests(object sender, object args)
        {
            _ = Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () => await Run()
            );
        }

        private async Task Run()
        {
            runButton.IsEnabled = false;

            try
            {

                await SetStatus($"Building source");
                var config = new CoreConfig(_logger);

                var assembly = await SampleRunner.Build(sources.Text);

                await SetStatus($"Running benchmark (The UI will freeze during the test)");
                var b = BenchmarkRunner.Run(assembly, config);

                await SetStatus($"Done.");
            }
            catch (Exception e)
            {
                await SetStatus($"Failed {e?.Message}");
                _logger.WriteLine(LogKind.Error, e?.ToString());
            }
            finally
            {
                runButton.IsEnabled = true;
            }
        }

        private async Task SetStatus(string status)
        {
            runStatus.Text = status;
            await Task.Yield();
        }

        private IEnumerable<Type> EnumerateBenchmarks(IConfig config)
            => from type in GetType().GetTypeInfo().Assembly.GetTypes()
               where !type.IsGenericType
               where type.Namespace?.StartsWith(BenchmarksBaseNamespace) ?? false
               where BenchmarkConverter.TypeToBenchmarks(type, config).BenchmarksCases.Length != 0
               select type;

        public class CoreConfig : ManualConfig
        {
            public CoreConfig(ILogger logger)
            {
                Add(logger);
                Add(AsciiDocExporter.Default);

                Add(RankColumn.Arabic);
                Add(TargetMethodColumn.Method);
                Add(StatisticColumn.AllStatistics);

                Add(Job.InProcess
                    .WithLaunchCount(1)
                    .WithWarmupCount(1)
                    .WithIterationCount(5)
                    .With(InProcessToolchain.Synchronous)
                    .WithId("InProcess")
                );
            }
        }

        private class TextBlockLogger : ILogger
        {
            private static Dictionary<LogKind, SolidColorBrush> ColorfulScheme { get; } =
               new Dictionary<LogKind, SolidColorBrush>
               {
                    { LogKind.Default, new SolidColorBrush(Colors.Gray) },
                    { LogKind.Help, new SolidColorBrush(Colors.DarkGreen) },
                    { LogKind.Header, new SolidColorBrush(Colors.Magenta) },
                    { LogKind.Result, new SolidColorBrush(Colors.DarkCyan) },
                    { LogKind.Statistic, new SolidColorBrush(Colors.Cyan) },
                    { LogKind.Info, new SolidColorBrush(Colors.DarkOrange) },
                    { LogKind.Error, new SolidColorBrush(Colors.Red) },
                    { LogKind.Hint, new SolidColorBrush(Colors.DarkCyan) }
               };

            private readonly TextBlock _target;

            public TextBlockLogger(TextBlock target)
                => _target = target;

            public void Flush() { }

            public void Write(LogKind logKind, string text)
            {
                _target.Inlines.Add(new Run { Text = text, Foreground = GetLogKindColor(logKind) });
            }

            public static Brush GetLogKindColor(LogKind logKind)
            {
                if (!ColorfulScheme.TryGetValue(logKind, out var brush))
                {
                    brush = ColorfulScheme[LogKind.Default];
                }

                return brush;
            }

            public void WriteLine() => _target.Inlines.Add(new LineBreak());

            public void WriteLine(LogKind logKind, string text)
            {
                Write(logKind, text);
                WriteLine();
            }
        }
    }
}
