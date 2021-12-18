namespace DecompositionPlayground
{
	public class FailureRateAnalyzer
	{
		private static readonly Random Random = new ();

		private static readonly long[] StdDenoms = new long[] {
			1, 2, 3, 4, 5, 6, 8, 9, 10, 16, 18, 20, 27, 32, 50, 54, 64, 81, 100, 128, 162, 200,
			243, 256, 486, 500, 512, 729, 1000, 1024, 1458, 2000, 2048, 2187, 4096, 4374, 5000,
			6561, 8192, 10000, 13122, 16384, 19683, 20000, 32768, 39366, 50000, 59049, 65536,
			100000, 118098, 131072, 177147, 200000, 262144, 354294, 500000, 524288, 531441,
			1000000, 1048576, 1062882, 1594323, 2000000, 2097152, 3188646, 4194304, 4782969,
			5000000, 8388608, 9565938, 10000000, 14348907, 16777216, 20000000, 28697814,
			33554432, 43046721, 50000000, 67108864, 86093442, 100000000, 129140163, 134217728,
			200000000, 258280326, 268435456, 387420489, 500000000, 536870912, 774840978,
			1000000000, 1073741824, 1162261467, 2000000000, 2147483648, 2324522934, 3486784401,
			4294967296, 5000000000, 6973568802, 8589934592, 10000000000, 10460353203, 17179869184,
			20000000000, 20920706406, 31381059609, 34359738368, 50000000000, 62762119218,
			68719476736, 94143178827, 100000000000, 137438953472, 188286357654, 200000000000,
			274877906944, 282429536481, 500000000000, 549755813888, 564859072962, 847288609443,
			1000000000000, 1099511627776, 1694577218886, 2000000000000, 2199023255552, 2541865828329
		};

		public static void Analyze()
		{
			var iterations = 100_000;
			var failures = 0;
			var startTime = DateTime.UtcNow;
			for(var i = 0; i < iterations; i ++)
			{
				var dust = 500; // Random.NextInt64(1_000);
				var k = 8; //Random.Next(7, 8);
				var tolerance = 100; //Random.NextInt64(dust / 20);
				var target = Random.NextInt64(dust, 43 * 100_000_000L);
				var denoms = StdDenoms.SkipWhile(x => x < dust).TakeWhile(x => x <= target).Reverse().ToArray();

				var results = Decomposer.Combinations(target, tolerance, k - 1, denoms).Take(50).ToList();
				if ( results.Count < 50)
				{
					failures++;
					Console.WriteLine($"target = {target}  k = {k}  tolerance = {tolerance}  dust = {dust} results = {results.Count}");
					// PrintAll(results, denoms);
				}
			}
			var totalTime = DateTime.UtcNow - startTime;
			var failureRate = (double)failures / iterations;
			Console.WriteLine($"Failure rate = {failureRate}");
			Console.WriteLine($"Success rate = {1 - failureRate}");
			Console.WriteLine($"Finished after {totalTime.TotalMilliseconds}ms that is average time {totalTime.TotalMilliseconds / iterations}ms.");
		}

		private static void PrintAll(IEnumerable<(long Sum, ulong Decomposition)> decompositions, long[] denoms)
		{
			foreach (var composition in decompositions)
			{
				Print(composition, denoms);
			}
		}

		private static void Print((long Sum, ulong Decomposition) composition, long[] denoms)
		{
			var indexes = BitConverter.GetBytes(composition.Decomposition);
			var remaining = composition.Sum;
			var i = 0;
			var list = new List<long>();
			do
			{
				var val = denoms[indexes[i++]];
				list.Insert(0, val);
				remaining -= val;
			} while (remaining > 0);
			Console.WriteLine("Sum: {0} -> [{1}]", composition.Sum, string.Join(", ", list));
		}
	}
}

