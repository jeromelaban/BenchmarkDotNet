using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Characteristics;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.Results;

namespace BenchmarkDotNet.Toolchains.ProjectJson
{
	public class XamarinBuilder : IBuilder
	{
		public BuildResult Build(GenerateResult generateResult, ILogger logger, Benchmark benchmark, IResolver resolver)
		{
			return BuildResult.Success(
				new GenerateResult(
					new ArtifactsPaths(
						artifactCleanup: p => { }
						, rootArtifactsFolderPath: "in-process"
						, appConfigPath: "in-process"
						, binariesDirectoryPath: "in-process"
						, buildArtifactsDirectoryPath: "in-process"
						, buildScriptFilePath: "in-process"
						, executablePath: "in-process"
						, programCodePath: "in-process"
						, programName: "in-process"
						, projectFilePath: "in-process"
					)
					, isGenerateSuccess: true
					, generateException: null
				)
			);
		}
	}
}
