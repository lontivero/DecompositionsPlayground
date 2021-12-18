namespace DecompositionPlayground
{
	public static class Decomposer
	{
		public static IEnumerable<(long Sum, ulong Decomposition)> Combinations(long target, long tolerance, int k, long[] denoms) =>
			denoms.SelectMany((_, i) => Combinations(i,	target, tolerance, 0ul, 0, k, denoms));

		private static IEnumerable<(long Sum, ulong Decomposition)> Combinations(
			int currentDenominationIdx,
			long target,
			long tolerance,
			ulong accumulator,
			long sum,
			int k,
			long[] denoms)
		{
			accumulator = (accumulator << 8) | ((ulong)currentDenominationIdx & 0xff);
			var currentDenomination = denoms[currentDenominationIdx];
			sum += currentDenomination;
			var remaining = target - currentDenomination;
			if (k == 0)
				return new[] { (sum, accumulator) };

			var startingIndex = Array.BinarySearch(denoms, currentDenominationIdx, denoms.Length - currentDenominationIdx, remaining, ReverseComparer.Default);
			currentDenominationIdx = startingIndex < 0 ? ~startingIndex : startingIndex;

			return Enumerable.Range(0, denoms.Length - currentDenominationIdx)
				.TakeWhile(i => k * denoms[currentDenominationIdx + i] >= remaining - tolerance)
				.SelectMany((_, i) => Combinations(currentDenominationIdx + i, remaining, tolerance, accumulator, sum, k - 1, denoms));
		}
	}
}