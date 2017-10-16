<Query Kind="Program" />

void Main()
{
	Runner(int.MaxValue, 102, -1, int.MinValue + 1, 2324356);
}

public static void Runner(params int[] nums)
{
	foreach (var num in nums)
		Runner(num);
}

public static void Runner(int num)
{
	Console.WriteLine("{0:n0} -> {1}", num, num.ToWordsViaDigitsRecursive());
	Console.WriteLine("\t{1}", num, num.ToWords());
}

public static class Numerical
{
	public static IEnumerable<int> GetDigits(this int target, int @base = 10)
	{
		if (@base < 2)
			throw new ArgumentException("Bits are the lowest base I can use to represent");
		
		if (target == 0)
		{
			yield return 0;
			yield break;
		}
		
		target = Math.Abs(target);
		var magnitude = (int)Math.Pow(@base, (int)Math.Log(target, @base));
		while (magnitude > 0)
		{
			var nextDigit = target / magnitude;
			yield return nextDigit;
			target -= nextDigit * magnitude;
			magnitude /= @base;
		}
	}
	
	/// Converts a sequence of positive integers into a number based on position in sequence
	/// Throws exception if concatenated sequence is more than int.Max
	public static int ToInt(this IEnumerable<int> target)
	{
		var text = string.Join(string.Empty, target.Select(Math.Abs));
		if (string.IsNullOrEmpty(text)) return 0;
		if (text.Length > 10) throw new ArgumentException("Concatenated number is necessarily more than int.Max");
		return int.Parse(text);
	}

	/// Converts a sequence of positive 64-bit integers into a number based on position in sequence
	/// Throws exception if concatenated sequence is more than long.Max
	public static long ToLong(this IEnumerable<int> target)
	{
		var text = string.Join(string.Empty, target.Select(Math.Abs));
		if (string.IsNullOrEmpty(text)) return 0;
		if (text.Length > 20) throw new ArgumentException("Concatenated number is necessarily more than long.Max");
		return long.Parse(text);
	}
}

public static class NumericWords
{
	public static string ToWords(this int input)
	{
		if (input == 0) return "zero";

		var result = new StringBuilder();
		if (input < 0)
		{
			input = -input;
			result.AppendWithSpace("negative");
		}

		if (NumberNames.TryGetValue(input, out var numberName))
		{
			result.Append(numberName);
			return result.ToString();
		}
		
		if (input < 100)
		{
			int
				tens = input / 10,
				units = input % 10;
			return result
				.Append(NumberNames[tens * 10]).Append("-").Append(NumberNames[units])
				.ToString();
		}
		
		int remaining = input;
		var magnitudeNames = new Dictionary<int, string>
		{
			{1_000_000_000, "billion"},
			{1_000_000, "million"},
			{1_000, "thousand"},
			{1_00, "hundred"},
		};
		
		foreach (var magnitudePair in magnitudeNames)
		{
			var magnitude = magnitudePair.Key;
			var magnitudeName = magnitudePair.Value;
			if (remaining >= magnitude)
			{
				var number = remaining / magnitude;
				remaining -= number * magnitude;
				result.AppendWithSpace(number.ToWords()).AppendWithSpace(magnitudeName);
			}
		}

		if (remaining > 0)
			result.Append(remaining.ToWords());
		
		return result.ToTrimmedString();
	}
	
	public static string ToWordsViaDigitsRecursive(this int input)
	{
		var result = new StringBuilder();
		if (input == 0) return "zero";
		if (input < 0)
		{
			result.Append("negative ");
			input = -input;
		}

		var digits = input.GetDigits().ToQueue();
		var remaining = input;
		if (digits.Count == 10)
		{
			int magnitude = 1_000_000;
			var num = digits.Dequeue();
			result.AppendWithSpace(NumberNames[num]).AppendWithSpace("billion");
			remaining -= num * magnitude;
		}

		if (digits.Count >= 7)
		{
			int magnitude, digitsToTake;
			if (digits.Count == 9)
			{
				digitsToTake = 3;
				magnitude = 100_000_000;
			}
			else if (digits.Count == 8)
			{
				digitsToTake = 2;
				magnitude = 10_000_000;
			}
			else
			{
				digitsToTake = 1;
				magnitude = 1_000_000;
			}

			var num = digits.ManyDequeue(digitsToTake).ToInt();
			remaining -= num * magnitude;
			result.AppendWithSpace(num.ToWordsViaDigitsRecursive()).AppendWithSpace("million");
		}

		if (digits.Count > 4)
		{
			int magnitude, digitsToTake;
			if (digits.Count == 6)
			{
				digitsToTake = 3;
				magnitude = 100_000;
			}
			else if (digits.Count == 5)
			{
				digitsToTake = 2;
				magnitude = 10_000;
			}
			else
			{
				digitsToTake = 1;
				magnitude = 1_000;
			}

			var num = digits.ManyDequeue(digitsToTake).ToInt();
			remaining -= num * magnitude;
			result.AppendWithSpace(num.ToWordsViaDigitsRecursive()).AppendWithSpace("thousand");
		}

		if (digits.Count == 3)
		{
			var num = digits.Dequeue();
			var magnitude = 100;
			result.AppendWithSpace(NumberNames[num]).AppendWithSpace("hundred");
			remaining -= num * magnitude;
		}

		if (digits.Count <= 2)
		{
			var num = digits.DequeueAll().ToInt();
			if (NumberNames.TryGetValue(num, out var name))
			{
				result.Append(name);
			}
			else // What remains due to initial setup is xy where x > 1 and y != 0;
			{
				var tens = num / 10;
				var units = num % 10;
				
				result.Append(NumberNames[tens * 10]).Append("-").Append(NumberNames[units]);
			}
		}
		
		return result.ToTrimmedString();
	}

	#region
	readonly static Dictionary<int, string> NumberNames = new Dictionary<int, string>
		{
			{00, string.Empty},
			{1, "one" }, {2, "two"}, {3, "three"}, {4, "four"},
			{5, "five"}, {6, "six"}, {7, "seven"}, {8, "eight"},
			{9, "nine"}, {10, "ten"}, {11, "eleven"}, {12, "twelve"},
			{13, "thirteen"}, {14, "fourteen"}, {15, "fifteen"}, {16, "sixteen"},
			{17, "seventeen"}, {18, "eighteen"}, {19, "nineteen"}, {20, "twenty"},
			{30, "thirty"}, {40, "fourty"}, {50, "fifty"}, {60, "sixty"},
			{70, "seventy"}, {80, "eighty"}, {90, "ninety"},
		};
	#endregion
}

public static class CollectionExtensions
{
	public static Queue<T> ToQueue<T>(this IEnumerable<T> target)
	{
		return new Queue<T>(target);
	}

	public static IEnumerable<T> ManyDequeue<T>(this Queue<T> target, int count)
	{
		if (target == null || target.Count <= 0)
			yield break;
		for (int i = 0; i < count && target.Count > 0; i++)
		{
			yield return target.Dequeue();
		}
	}
	
	public static IEnumerable<T> DequeueAll<T>(this Queue<T> target)
	{
		while (target.Count > 0)
			yield return target.Dequeue();
	}
}

public static class StringExtensions
{
	public static StringBuilder AppendIfTrue(this StringBuilder target, bool condition, string text)
	{
		if (condition)
			target?.Append(text);
		return target;
	}
	
	public static StringBuilder AppendSpace(this StringBuilder target)
	{
		return target?.Append(" ");
	}
	
	public static StringBuilder AppendWithSpace(this StringBuilder target, string text)
	{
		return target?.Append(text)?.AppendSpace();
	}

	public static string ToTrimmedString(this StringBuilder target)
	{
		return target?.ToString()?.Trim() ?? string.Empty;
	}
}