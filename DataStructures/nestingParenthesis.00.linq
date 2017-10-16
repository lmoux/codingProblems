<Query Kind="Program" />

/*
Parenthesis nesting problem with an extra twist

Braces that should be validated: ()[]{}<>
Return true when: 
	a) all braces are balanced (number of opening braces equals to the number of closing braces) AND
	b) all braces are in correct order (if there is a closing brace then the previous opening brace should be of the same kind)

Correct: 
	(abc)asdjh[({})]kjdfd<>
	(a{}b<cdf>)
	(){}<>[]

Incorrect:
	({)}khdfkhdf
	({)}khdf(}
	((({}))
	)(
	[)]
	]
*/

void Main(string input)
{
	// Left here to keep original problem spec.
	ValidateBraces(input, "()[]{}<>");
	
	// For testing 
	Runner("(abc)asdjh[({})]kjdfd<>", true);
	Runner("(a{}b<cdf>)", true);
	Runner("(){}<>[]", true);
	
	Runner("({)}khdfkhdf", false);
	Runner("({)}khdf(}", false);
	Runner("((({}))", false);
	Runner(")(", false);
	Runner("[)]", false);
	Runner("]", false);
}

static bool ValidateBraces(string input, string braces)
{
	var parser = new Validator(braces);
	return parser.AreParenthesisNestedCorrectly(input);
}

public class Validator
{
	public Validator(string braces)
	{
		PopulateParenClosures(braces);
		ParenthesisFilter = new HashSet<char>(ParenthesisClosures.Keys.Union(ParenthesisClosures.Values));
	}
	
	readonly Dictionary<char, char> ParenthesisClosures = new Dictionary<char, char>();
	readonly HashSet<char> ParenthesisFilter;

	public bool AreParenthesisNestedCorrectly(string target)
	{
		if (string.IsNullOrEmpty(target))
			return false;

		var justParenthesis = new List<char>(target.Length);
		foreach (var nextCharater in target)
		{
			if (ParenthesisFilter.Contains(nextCharater))
				justParenthesis.Add(nextCharater);
		}

		return AreParenthesisNestedCorrectly(justParenthesis);
	}
	public bool AreParenthesisNestedCorrectly(ICollection<char> target)
	{
		if (target == null || target.Count <= 0 || target.Count % 2 != 0)
			return false;

		var stack = new Stack<char>();
		foreach (var next in target)
		{
			if (ParenthesisClosures.ContainsKey(next))
			{
				stack.Push(next);
				continue;
			}

			if (stack.Count <= 0)
				return false;
			var topOfStack = stack.Pop();
			if (next != ParenthesisClosures[topOfStack])
				return false;
		}

		return true;
	}

	void PopulateParenClosures(string braces)
	{
		if (string.IsNullOrWhiteSpace(braces) || braces.Length % 2 != 0)
			throw new ArgumentException("Expect a string sequence like: ()[]{}<>");

		foreach (var pair in braces.ToCharArray().ToPairs())
		{
			this.ParenthesisClosures.Add(pair.Item1, pair.Item2);
		}
	}
}

public static class CollectionExtensions
{
	public static IEnumerable<Tuple<T, T>> ToPairs<T>(this IEnumerable<T> target)
	{
		foreach (var chunk in ConsecutiveChunks(target, 2))
		{
			yield return Tuple.Create(chunk[0], chunk[1]);
		}
	}

	public static IEnumerable<T[]> ConsecutiveChunks<T>(this IEnumerable<T> target, int size)
	{
		if (target == null) yield break;
		var queue = new Queue<T>(size);

		foreach (var item in target)
		{
			if (queue.Count >= size)
			{
				yield return queue.ToArray();
				queue.Clear();
			}
			
			queue.Enqueue(item);
		}
	}
}

public static void Runner(string target, bool expected)
{
	var result = ValidateBraces(target, "()[]{}<>");
	if (expected != result)
		Console.WriteLine("\"{0}\" -> ERROR: got {1}, expected {2}", target, result, expected);
	else
		Console.WriteLine("\"{0}\" -> validated well", target, result, expected);
}