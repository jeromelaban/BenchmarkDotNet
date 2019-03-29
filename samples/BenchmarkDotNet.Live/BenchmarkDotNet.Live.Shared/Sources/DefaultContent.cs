//
// Welcome to the BenchmarkDotNet Live for WebAssembly !
//
// You can enter some code in this window, then press Run Benchmark above.
//
// The code will be compiled using Roslyn running entirely in your browser, then
// use BenchmarkDotNet to measure the performance of each method marked with the
// [Benchmark] attribute.
//
// Note that WebAssembly currently does not support threading (support is coming), which
// means that the page will freeze during the run of the benchmarks.
//
// See https://benchmarkdotnet.org/articles/samples/IntroBasic.html for more samples !

using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace MyBenchmarks
{
    public class Md5VsSha256
    {
        private const int N = 1000;
        private readonly byte[] data;

        private readonly SHA256 sha256 = SHA256.Create();
        private readonly MD5 md5 = MD5.Create();

        public Md5VsSha256()
        {
            data = new byte[N];
            new Random(42).NextBytes(data);
        }

        [Benchmark]
        public byte[] Sha256() => sha256.ComputeHash(data);

        [Benchmark]
        public byte[] Md5() => md5.ComputeHash(data);
    }
}