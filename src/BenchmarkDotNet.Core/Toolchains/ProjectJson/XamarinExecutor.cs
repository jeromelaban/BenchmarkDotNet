using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Toolchains.Parameters;
using BenchmarkDotNet.Toolchains.Results;

namespace BenchmarkDotNet.Toolchains.ProjectJson
{
	public class XamarinExecutor : IExecutor
	{
		public ExecuteResult Execute(ExecuteParameters executeParameters)
		{

			return new ExecuteResult(true, 0, new List<string>(), new List<string>());
		}
	}
}
