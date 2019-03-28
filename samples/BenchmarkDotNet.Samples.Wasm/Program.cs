using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess;
using System;
using System.Linq;

namespace BenchmarkDotNet.Samples.Wasm
{
    public class Program
    {
        static int Main(string[] args)
        {
            var s = BenchmarkRunner.Run(typeof(Program).Assembly, new CoreConfig());
            Console.WriteLine(s.ToString());

            return 0;
        }

    }

    public class CoreConfig : ManualConfig
    {
        public CoreConfig()
        {
            Add(ConsoleLogger.Default);
            Add(Job.InProcess
                .WithLaunchCount(1)
                .WithWarmupCount(1)
                .WithIterationCount(5)
                .With(InProcessToolchain.Synchronous)
                .WithId("InProcess")
            );
        }
    }

    public class SpanTesting
    {
        [Params(10, 20)]
        public int Items { get; set; }

        [Benchmark(Baseline = true)]
        public void EnumerableSum()
        {
            var r = Enumerable.Range(0, Items).ToArray();

            var s = r.Sum();
        }


        [Benchmark()]
        public void SpanSum()
        {
            var r = Enumerable.Range(0, Items).ToArray();

            var s = ((Span<int>)r).Sum();
        }

    }

    public static class Extensions
    {
        public static double Sum(this Span<int> span)
        {
            double result = 0;

            foreach (var value in span)
            {
                result += value;
            }

            return result;
        }
    }

    //public class IntroBaseline
    //{
    //    private readonly Random random = new Random(42);

    //    [Params(10, 20)]
    //    public int BaseTime { get; set; }

    //    [Benchmark(Baseline = true)]
    //    public void Baseline()
    //    {
    //        for (int i = 0; i < BaseTime; i++)
    //        {
    //            i.ToString();
    //        }
    //    }

    //    [Benchmark]
    //    public void Slow()
    //    {
    //        for (int i = 0; i < BaseTime * 10; i++)
    //        {
    //            i.ToString();
    //        }
    //    }

    //    [Benchmark]
    //    public void Fast()
    //    {
    //        for (int i = 0; i < BaseTime / 2; i++)
    //        {
    //            i.ToString();
    //        }
    //    }

    //    [Benchmark]
    //    public void Unstable()
    //    {
    //        var variation = (int)((random.NextDouble() - 0.5) * 2 * BaseTime);

    //        for (int i = 0; i < BaseTime + variation; i++)
    //        {
    //            i.ToString();
    //        }
    //    }
    //}

}
