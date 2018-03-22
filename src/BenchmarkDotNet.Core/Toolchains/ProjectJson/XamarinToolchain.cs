using BenchmarkDotNet.Characteristics;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Extensions;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.DotNetCli;
using JetBrains.Annotations;

namespace BenchmarkDotNet.Toolchains.ProjectJson
{
    [PublicAPI]
    public class XamarinToolchain : Toolchain
    {
        [PublicAPI] public static readonly IToolchain Instance = new XamarinToolchain();

        // In case somebody calls ClassicToolchain from .NET Core process 
        // we will build the project as 4.6 because it's the most safe way to do it:
        // * everybody that uses .NET Core must have VS 2015 installed and 4.6 is part of the installation
        // * from 4.6 you can target < 4.6
        private const string TargetFrameworkMoniker = "Xamarin";

        [PublicAPI]
        public XamarinToolchain() : base(
            "Xamarin",
            new InProcess.InProcessGenerator(),
            new InProcess.InProcessBuilder(), 
            new InProcess.InProcessExecutor(System.TimeSpan.FromSeconds(60), InProcess.BenchmarkActionCodegen.DelegateCombine, true)
		)
        {
        }

        public override bool IsSupported(Benchmark benchmark, ILogger logger, IResolver resolver)
        {
            return true;
        }

    }
}