using System.Diagnostics;

namespace DecompositionPlayground
{
	public static class Decomposer
	{
		public static IEnumerable<(long Sum, int Count, ulong Decomposition)> Combinations(long target, long tolerance, int maxLength, long[] denoms)
		{
			IEnumerable<(long Sum, int Count, ulong Decomposition)> Combinations(
				int currentDenominationIdx,
				ulong accumulator,
				long sum,
				int k)
			{
				accumulator = (accumulator << 8) | ((ulong)currentDenominationIdx & 0xff);
				var currentDenomination = denoms[currentDenominationIdx];
				sum += currentDenomination;
				var remaining = target - sum;
				if (k == 0 || remaining < tolerance)
					return new[] { (sum, maxLength - k + 1, accumulator) };

				currentDenominationIdx = Search(remaining, denoms, currentDenominationIdx);

				return Enumerable.Range(0, denoms.Length - currentDenominationIdx)
					.TakeWhile(i => k * denoms[currentDenominationIdx + i] >= remaining - tolerance)
					.SelectMany((_, i) =>
						Combinations(currentDenominationIdx + i, accumulator, sum, k - 1)
						.TakeUntil(x => x.Sum == target));
			}

			return denoms.SelectMany((_, i) => Combinations(i, 0ul, 0, maxLength));
		}

		private static int Search(long value, long[] denoms, int offset)
		{
			var startingIndex = Array.BinarySearch(denoms, offset, denoms.Length - offset, value, ReverseComparer.Default);
			return startingIndex < 0 ? ~startingIndex : startingIndex;
		}
	}
}