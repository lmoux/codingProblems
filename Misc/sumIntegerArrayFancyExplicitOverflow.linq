<Query Kind="Program" />

void Main()
{
	Runner(55, false, Enumerable.Range(1, 10).ToArray());
	Runner(20, false, int.MaxValue, int.MaxValue, 22);
	Runner(0, true, int.MaxValue, int.MaxValue, 22);
	Runner(5, true, int.MaxValue, int.MinValue, 1, 2, 3);

}

public void Runner<T>(T sum, bool throwIfOverflow, params int[] numbers)
where T : struct, IConvertible, IComparable, IComparable<T>
{
	var numberText = string.Join(", ", numbers);
	if (throwIfOverflow)
	{
		try
		{
			var num = Solutions.Sum<T>(throwIfOverflow, numbers);
			Console.WriteLine("Sum({0}) == {1:n0} ? {2}", numberText, sum, num);
		}
		catch (OverflowException)
		{
			Console.WriteLine("Sum({0}) == {1:n0} ? {2}...", numberText, sum, "threw overflow");
		}
	}
	else
	{
		var num = Solutions.Sum<T>(throwIfOverflow, numbers);
		Console.WriteLine("Sum({0}) == {1:n0} ? {2}", numberText, sum, num);
	}
}

public static class Solutions
{
	public static T Sum<T>(bool throwIfOverflow, IEnumerable<int> numbers)
		where T: struct, IConvertible, IComparable, IComparable<T>
	{
		numbers = numbers ?? Enumerable.Empty<int>();
		var policy = NumericalPolicies.All as IAdd<T>;
		
		if (policy == null)
			throw new NotImplementedException("Currently only implement returning int and long");
		
		var zero = policy.Convert(0);
		Func<T, int, T> accumulator;

		if (throwIfOverflow)
		{
			accumulator = (accum, next) =>
			{
				var convertedNext = policy.Convert(next);
				return policy.AddThrowingIfOverflow(accum, convertedNext);
			};
		}
		else
		{
			accumulator = (accum, next) =>
			{
				var convertedNext = policy.Convert(next);
				return policy.AddAllowingOverflow(accum, convertedNext);
			};
		}

		return numbers.Aggregate(zero, accumulator);
	}
}

public interface IAdd<T> where T : struct, IConvertible, IComparable, IComparable<T>
{
	T Convert(int a);
	T AddAllowingOverflow(T a, T b);
	T AddThrowingIfOverflow(T a, T b);
}

public sealed class NumericalPolicies : IAdd<int>, IAdd<long>, IAdd<uint>
{
	public int Convert(int a) => a;
	public int AddAllowingOverflow(int a, int b)
	{
		unchecked
		{
			return a + b;
		}
	}
	public int AddThrowingIfOverflow(int a, int b)
	{
		checked
		{
			return a + b;
		}
	}

	long IAdd<long>.Convert(int a) => System.Convert.ToInt64(a);
	long IAdd<long>.AddAllowingOverflow(long a, long b)
	{
		unchecked
		{
			return a + b;
		}
	}
	long IAdd<long>.AddThrowingIfOverflow(long a, long b)
	{
		checked
		{
			return a + b;
		}
	}

	uint IAdd<uint>.Convert(int a) => System.Convert.ToUInt32(a);
	uint IAdd<uint>.AddAllowingOverflow(uint a, uint b)
	{
		unchecked
		{
			return a + b;
		}
	}
	uint IAdd<uint>.AddThrowingIfOverflow(uint a, uint b)
	{
		checked
		{
			return a + b;
		}
	}

	public static readonly NumericalPolicies All = new NumericalPolicies();
}