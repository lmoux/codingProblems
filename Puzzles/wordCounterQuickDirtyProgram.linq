<Query Kind="Program" />

/*
 * Instructions:
 * 	Write a quick and dirty program (not just standalone function) to print
 *	a count of all different "words" in a text file. Use any definition of word that
 * 	makes logical sense or makes the job easy. The output might look like:
 *		17 	a
 *		14	the
 *		9 	of
 *		9 	in
 *		8	com
 *		7	you
 */

void Main()
{
	Console.WriteLine("This program prints the counts of each different word on a file");
	Console.WriteLine("---------------------------------------------------------------");
	
	while (true)
	{
		Console.WriteLine("To exit program hit Ctrl-C or enter \"EXIT!\" (case insensitive) instead of path");
		Console.WriteLine("Please specify path to file: ");
		
		// for my test: c:\Users\luis\Desktop\testText.txt
		var path = Console.ReadLine();
		if (string.Equals("Exit!", path, StringComparison.OrdinalIgnoreCase))
			break;
		
		var punctationRemover = new Regex("[!\\.,;:]", RegexOptions.Compiled);
		Func<string, string> removePunctationMarks = 
			word => punctationRemover.Replace(word, string.Empty);
		
		var histogram = path
			.ReadWordsFromFilePath(caseInsensitve: false)
			.Select (removePunctationMarks)
			.ToHistogram(useLinq: true);
		
		if (histogram.Count > 0)
			Console.WriteLine("\t{0}\t\t{1}", "Word", "Frequency/Count");
		foreach (var pair in histogram.OrderByDescending(h => h.Value).ThenByDescending (h => h.Key))
		{
			Console.WriteLine("\t{0}\t\t{1}", pair.Key, pair.Value);
		}
	}
}

public static class FileLogic
{
	/// <summary>Retrieve each word in a given file</summary>
	/// <remarks>
	///		- Word is defined here as text that is isolated from other text by space around it
	///		- This implicitly uses UTF-8, per System.IO.File.ReadLines Method (String)'s MSDN documentation
	///		- This is greedily evaluated, so make be careful about file sizes used.
	///		- By default, we normalize words such that "HELLO" and "hello" are mapped to the same word.
	///			However, you can specify the wanted behaviour via the caseInsensitive parameter.
	/// </remarks>
	public static IEnumerable<string> ReadWordsFromFilePath(this string filePath, bool caseInsensitve = true)
	{
		if (filePath == null || !File.Exists(filePath))
			return Enumerable.Empty<string>();
		
		var result = File
			.ReadLines(filePath)
			.SelectMany (line => line.Split(new []{" "}, StringSplitOptions.RemoveEmptyEntries));
			
		if (caseInsensitve)
			return result.Select (r => r.ToUpperInvariant());
		else
			return result;
	}
}

public static class Maths
{
	public static IDictionary<T, int> ToHistogram<T>(this IEnumerable<T> items, bool useLinq = true)
	{
		if (items == null) return new Dictionary<T, int>();
		
		if (useLinq)
			return LinqVersionOfHistogram(items);
		else
			return ManualVersionOfHistogram(items);
	}
	
	private static IDictionary<T, int> LinqVersionOfHistogram<T>(IEnumerable<T> items)
	{
		return items
			.GroupBy(i => i)
			.Select (g => new {Item = g.Key, Frequency = g.Count() })
			.ToDictionary (key => key.Item, value => value.Frequency );
	}
	
	private static IDictionary<T, int> ManualVersionOfHistogram<T>(IEnumerable<T> items)
	{
		var result = new Dictionary<T, int>();
		foreach (var item in items)
		{
			int value = 0;
			if (!result.TryGetValue(item, out value))
				result.Add(item, 1);
			else
				result[item] = value + 1;
		}
		
		return result;
	}
}