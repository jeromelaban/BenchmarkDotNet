#if __WASM__
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host;

namespace BenchmarkDotNet.Live
{
    public class SampleRunner
    {
        public static async Task<Assembly> Build(string code)
        {
            await Task.Yield();
            var st = SyntaxFactory.ParseCompilationUnit(code);

            Compilation compilation = CSharpLanguage.Instance
              .CreateLibraryCompilation(assemblyName: "InMemoryAssembly", enableOptimisations: false)
              .AddSyntaxTrees(new[] { st.SyntaxTree });

            Console.WriteLine($"Got compilation");
            await Task.Yield();

            Console.WriteLine($"Emitting assembly...");
            var stream = new MemoryStream();
            var emitResult = compilation.Emit(stream);

            await Task.Yield();

            if (emitResult.Success)
            {
                Console.WriteLine($"Got binary assembly: {emitResult.Success}");

                return Assembly.Load(stream.ToArray());
            }
            else
            {
                Console.WriteLine($"Failed to emit assembly:");

                foreach (var diagnostic in emitResult.Diagnostics)
                {
                    Console.WriteLine(diagnostic);
                }

                throw new InvalidOperationException($"Failed");
            }
        }

        public class CSharpLanguage : ILanguageService
        {
            private readonly IEnumerable<MetadataReference> _references;

            public static CSharpLanguage Instance { get; } = new CSharpLanguage();

            private CSharpLanguage()
            {
                var sdkFiles = this.GetType().Assembly.GetManifestResourceNames().Where(f => f.Contains("mono_sdk"));

                _references = sdkFiles
                    .Select(f =>
                    {
                        using (var s = this.GetType().Assembly.GetManifestResourceStream(f))
                        {
                            return MetadataReference.CreateFromStream(s);
                        }
                    })
                    .ToArray();
            }

            public Compilation CreateLibraryCompilation(string assemblyName, bool enableOptimisations)
            {
                var options = new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    allowUnsafe: true)
                    // Disabling concurrent builds allows for the emit to finish.
                    .WithConcurrentBuild(false)
                    ;

                return CSharpCompilation.Create(assemblyName, options: options, references: _references);
            }
        }
    }
}
#endif