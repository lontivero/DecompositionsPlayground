using System;
using System.Numerics;

public class Program2
{
	private void Main()
	{
		var items = new long[] { 0, 1, 2, 3, 4, 5, 6, 8, 9, 10}.Reverse().ToArray();

		var target = 123_450_000L; // 1.2345
		var tolerance = 10_000L;
		var dust = 2048;
		var k = 8;

		var compositions = new Compositions<long>(5, items.ToHashSet());
		foreach (var i in Enumerable.Range(0, (int)compositions.Length-1))
		{
			var composition = compositions[i];
			if (composition.Sum() == 7)
			{
				Console.WriteLine($"{{ {string.Join(", ", composition)} }}");
			}
		}
	}
}

public class Compositions<T>
{
	public Compositions(int r, HashSet<T> items)
	{
		_items = items.ToList();
		_r = r;
		Length = Helpers._nCr(items.Count + r - 1, r);
	}

	private readonly List<T> _items;
	private readonly int _r;
	public BigInteger Length { get; }

	public List<T> this[int k] => this[new BigInteger(k)];
	public List<T> this[BigInteger k] => Composition(k % Length, _r, _items);

	private List<T> Composition(BigInteger k, int r, List<T> items)
	{
		int n = items.Count, position = 0;
		BigInteger d = Helpers._nCr(n + r - position - 2, r - 1);
		while (k >= d)
		{
			k -= d;
			position++;
			d = Helpers._nCr(n + r - position - 2, r - 1);
		}
		if (r == 0)
		{
			return new List<T>();
		}
		var tail = items.GetRange(position, items.Count - position);
		var ret = new List<T>(new[]{ items[position] });
		ret.AddRange(Composition(k, r - 1, tail));
		return ret;
	}
}

public static class Helpers
{
	private static readonly Dictionary<int, BigInteger> factorialCache = new ();

	public static BigInteger Factorial(int n) =>
		factorialCache.ContainsKey(n)
		? factorialCache[n]
		: (n < 2 ? BigInteger.One : factorialCache[n] = new BigInteger(n) * Factorial(n - 1));

	public static BigInteger _nPr(int n, int r) => Factorial(n) / Factorial(n - r);
	public static BigInteger _nCr(int n, int r) => _nPr(n, r) / Factorial(r);
}