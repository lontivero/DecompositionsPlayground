using System.Collections.Immutable;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Running;

namespace DecompositionPlayground
{
	public class Program
	{
		public static void Main(string[] args)
		{
			if (args.Contains("bench"))
			{
				RunBenchmark(args);
			}
			else if(args.Contains("analyze"))
			{
				FailureRateAnalyzer.Analyze();
			}
		}

		private static void RunBenchmark(string[] args)
		{
			var switcher = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly);
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				switcher.Run(args, new AllowNonOptimized());
			}
			else
			{
				switcher.Run(args);
			}
		}
	}
}