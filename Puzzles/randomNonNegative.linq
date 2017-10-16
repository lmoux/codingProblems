<Query Kind="Program" />

/*
 * Instructions:
 * 	Write a function that returns M random non-negative integers less than some value N.
 *	Each integer must also be unique.
 */

void Main()
{
	var random = new Random();
	for (int testId = 0; testId < 10; testId++)
	{
		var N = random.Next(50, 10000);
		var M = random.Next(1, 40);
		var results = Maths.GetRandomPositives(M, N);
		
		// I could have reused my ToHistogram routine from the other problem 
		// but I have decided to redo it here in the interest of not having you need
		// to link them when compiling, etc. since I'm not giving you a sln file or make file, etc.
		var histogram = results
			.GroupBy (r => r)
			.Select (r => new {r.Key, Times = r.Count()})
			.ToDictionary (r => r.Key, r => r.Times);
		
		if (histogram.Any (h => h.Value > 1) || histogram.Count != M || histogram.Any (h => h.Key >= N))
			throw new Exception("Parameters violated, we need uniqueness, M count and N value restriction!");
	}
}

public static class Maths
{
	/// <param name="m">Total number of random numbers to return</param>
	/// <param name="n">Maximum value allowed (exclusive)</param>
	/// <remarks>
	///		- In the interest of time, I'm not caring about cryptographically secure randomness
	///			In principle, I'll pick a random number generator suitable to the use case (no context here)
	///		- I'm assuming that M isn't going to be large enough that my use of hasing becomes an issue
	///		- I'm taking zero to be positive number
	/// </remarks>
	public static IList<int> GetRandomPositives(int m, int n)
	{
		if (m < 0) throw new ArgumentOutOfRangeException("m");
		if (n < 0) throw new ArgumentOutOfRangeException("n");
		
		// The following precludes cases like asking to get 15 unique positive
		// integers under the value 10... which, of course, is impossible.
		if (m > n)
			throw new Exception("Request can't be fulfilled.");
		
		var hash = new HashSet<int>();
		var random = new Random();
		
		while (hash.Count < m)
		{
			// Remember that here N is exclusive rather than inclusive
			var next = random.Next(0, n);
			if (!hash.Contains(next))
				hash.Add(next);
		}
		
		return hash.ToList();
	}
}