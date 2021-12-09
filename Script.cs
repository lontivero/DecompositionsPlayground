using System;
using System.Numerics;

public class Program2
{
	private static void Main()
	{
		var items = new long[] { 0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20}.Reverse().ToArray();

		var target = 123_450_000L; // 1.2345
		var tolerance = 10_000L;
		var dust = 2048;
		var k = 8;

		var compositions = new Compositions(5, items);
		foreach (var i in Enumerable.Range(0, (int)compositions.Length-1))
		{
			var composition = compositions[i];
			Console.WriteLine($"{{ {string.Join(", ", composition)} }}");
		}
	}
}

public class Compositions
{
	public Compositions(int r, long[] items)
	{
		_items = items;
		_r = r;
		Length = nCr(items.Length + r - 1, r);
	}

	private readonly ReadOnlyMemory<long> _items;
	private readonly int _r;
	public BigInteger Length { get; }

	public List<long> this[int k] => this[new BigInteger(k)];
	public List<long> this[BigInteger k] => Composition(k % Length, _r, _items.Span);

	private List<long> Composition(BigInteger k, int r, ReadOnlySpan<long> items)
	{
		static void Composition(BigInteger k, int r, ReadOnlySpan<long> items, List<long> list)
		{
			while(r >= 0)
			{
				var n = items.Length;
				var position = 0;
				BigInteger d = nCr(n + r - 2, r - 1);
				while (k >= d)
				{
					k -= d;
					position++;
					d = nCr(n + r - position - 2, r - 1);
				}
				var value = items[position];
				list.Add(value);
				items = items[position..n];
				r--;
			}
		}
		var list = new List<long>();
		Composition(k, r, items, list);
		return list;
	}


	private static readonly Dictionary<int, BigInteger> factorialCache = new ();

	public static BigInteger Factorial(int n) =>
		factorialCache.ContainsKey(n)
		? factorialCache[n]
		: (n < 2 ? BigInteger.One : factorialCache[n] = new BigInteger(n) * Factorial(n - 1));

	public static BigInteger nPr(int n, int r) => Factorial(n) / Factorial(n - r); // Permutations
	public static BigInteger nCr(int n, int r) => nPr(n, r) / Factorial(r); // Combinations
}