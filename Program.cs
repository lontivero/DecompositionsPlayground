using System;
using System.Collections;
using System.Numerics;
using System.Security.Cryptography;

namespace X
{
	public class Program
	{
		public static long[] StdDenoms { get; } = new long[] { 1, 2, 3, 4, 5, 6, 8, 9, 10, 16, 18, 20, 27, 32, 50, 54, 64, 81, 100, 128, 162, 200, 243, 256, 486, 500, 512, 729, 1000, 1024, 1458, 2000, 2048, 2187, 4096, 4374, 5000, 6561, 8192, 10000, 13122, 16384, 19683, 20000, 32768, 39366, 50000, 59049, 65536, 100000, 118098, 131072, 177147, 200000, 262144, 354294, 500000, 524288, 531441, 1000000, 1048576, 1062882, 1594323, 2000000, 2097152, 3188646, 4194304, 4782969, 5000000, 8388608, 9565938, 10000000, 14348907, 16777216, 20000000, 28697814, 33554432, 43046721, 50000000, 67108864, 86093442, 100000000, 129140163, 134217728, 200000000, 258280326, 268435456, 387420489, 500000000, 536870912, 774840978, 1000000000, 1073741824, 1162261467, 2000000000, 2147483648, 2324522934, 3486784401, 4294967296, 5000000000, 6973568802, 8589934592, 10000000000, 10460353203, 17179869184, 20000000000, 20920706406, 31381059609, 34359738368, 50000000000, 62762119218, 68719476736, 94143178827, 100000000000, 137438953472, 188286357654, 200000000000, 274877906944, 282429536481, 500000000000, 549755813888, 564859072962, 847288609443, 1000000000000, 1099511627776, 1694577218886, 2000000000000, 2199023255552, 2541865828329 };

		public static long Dust = 2048 - 1;
		public static int MaxRecursionDepth = 6;

		public static void Main()
		{
			var stdDenomsInAcceptableRange = StdDenoms.Where(x => x > Dust).ToArray().AsSpan();

			var decompositionsCache = new SortedList<long, IDecomposition>();
			FillDecompositionsCache(decompositionsCache, stdDenomsInAcceptableRange);

			var target = (long)(1.2345 * 100_000_000);
			stdDenomsInAcceptableRange = StdDenoms.Where(x => x > Dust && x <= target).ToArray().AsSpan();
			var decompositionTree = Decompose(target, stdDenomsInAcceptableRange, decompositionsCache, MaxRecursionDepth);

			var candidates = ExtractCandidates((Factor)decompositionTree, new SecureRandom()).ToArray();
		}

		private static IEnumerable<List<long>> ExtractCandidates(Factor decompositionTree, IRandomGenerator rndGen)
		{
			var candidate = new List<long>();
			var path = rndGen.Next(3);
			if (path == 0)
			{
				candidate.Add(decompositionTree.Value);
			}
			else
			{
				foreach(var alternative in decompositionTree.Alternatives)
				{
					if (path == 1)
					{
						if (alternative is Addition addition)
						{
							candidate.Add(addition.Left.Value);
//							var right = ExtractCandidates(alternative, rndGen);
						}

					}
				}
			}
			yield return candidate;
		}

		private static void FillDecompositionsCache(SortedList<long, IDecomposition> cache, Span<long> stdDenomsInAcceptableRange)
		{
			cache.Add(stdDenomsInAcceptableRange[0], new Factor(stdDenomsInAcceptableRange[0]));
			for (var i = 1; i < stdDenomsInAcceptableRange.Length; i++)
			{
				var currentDenomination = stdDenomsInAcceptableRange[i];
				var decomposition = Decompose(currentDenomination, stdDenomsInAcceptableRange, cache, MaxRecursionDepth);
				cache.Add(currentDenomination, decomposition);
			}
		}

		public static IDecomposition Decompose(
			long target,
			Span<long> values,
			SortedList<long, IDecomposition> cache,
			int maxDepth)
		{
			if (cache.TryGetValue(target, out var cachedDecomposition))
			{
				return cachedDecomposition;
			}

			var decompositions = new List<IDecomposition>();

			for (var i = 0; i < values.Length; i++)
			{
				var value = values[i];
				if (value >= target)
				{
					break;
				}

				if (cache.TryGetValue(value, out var decomposition))
				{
					var rest = target - value;
					if (rest > Dust)
					{
						if (cache.TryGetValue(rest, out var restDecomposition))
						{
							decompositions.Add(new Addition(decomposition, restDecomposition));
						}
						else
						{
							if (maxDepth - 1 <= 0)
							{
								continue;
							}
							var staringPoint = Math.Abs(values.BinarySearch(rest / 2) + 1);
							var newValues = values.Slice(staringPoint);
							restDecomposition = Decompose(rest, newValues, cache, maxDepth - 1);
							decompositions.Add(new Addition(decomposition, restDecomposition));
						}
					}
					else
					{
						// decompositions.Add(new Addition(decomposition, new Factor(rest)));
					}
				}
			}
			return new Factor(target, decompositions);
		}
	}

	public interface IDecomposition
	{
		long Value { get; }
	}

	public class Factor : IDecomposition
	{
		public Factor(long value, List<IDecomposition>? alternatives = null)
		{
			Value = value;
			Alternatives = alternatives ?? new List<IDecomposition>();
		}

		public long Value { get; }
		public List<IDecomposition> Alternatives { get; }
	}

	public class Addition : IDecomposition
	{
		public Addition(IDecomposition left, IDecomposition right)
		{
			Value = left.Value + right.Value;
			Left = left;
			Right = right;
		}
		public long Value { get; }
		public IDecomposition Left { get; }
		public IDecomposition Right { get; }
	}

	public interface IRandomGenerator
	{
		int Next(int maxValue);
	}

	public class SecureRandom : IRandomGenerator
	{
		public SecureRandom()
		{
			Random = RandomNumberGenerator.Create();
		}

		private RandomNumberGenerator Random { get; }

		public int Next(int maxValue)
		{
			var bytes = new byte[4];
			Random.GetBytes(bytes);
			var value = BitConverter.ToInt32(bytes, 0);
			return value % maxValue;
		}
	}
}