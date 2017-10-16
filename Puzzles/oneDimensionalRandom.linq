<Query Kind="Program" />

/*
 * Instructions:
 * 	Given a one dimensional array of data write a funciton that returns M random elements
 * 	of that array. Each element must also be from a different position in the array.
 */
 
void Main()
{
	var population = Enumerable.Range(1, 1000).ToArray();
	var sampleSize = 10;
	var sample = population.PickRandomSample(sampleSize);
	
	foreach(var datum in sample)
		Console.WriteLine(datum);
}

public static class CollectionExtensions
{
	/// <param name="population">One dimensional array (or array-like) data collection</param>
	/// <param name="m">Sample size</param>
	public static IEnumerable<T> PickRandomSample<T>(this IList<T> population, int m)
	{
		if (population == null) return Enumerable.Empty<T>();
		if (m > population.Count)
			throw new ArgumentOutOfRangeException(
				"m",
				"You are asking for more than the population and didn't describe the sample size as a maximum");
	
		// note, I could reuse the code from the other question (#3). But in the
		// interest of not having to make you compile a full sln or makefile, etc linking the two,
		//	I'm going to sort of redo it here. Just know that I regularly reuse code as per best practices
		var hash = new HashSet<int>();
		var random = new Random();
		var populationSize = population.Count;
		while (hash.Count < m)
		{
			var nextIndex = random.Next(0, populationSize);
			if (!hash.Contains(nextIndex))
				hash.Add(nextIndex);
		}
		
		var result = new List<T>(m);
		foreach(var index in hash)
			result.Add(population[index]);
		
		return result;
	}
}