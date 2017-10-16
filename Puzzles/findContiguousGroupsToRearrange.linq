<Query Kind="Program" />

/*
	Description:
		There is a collection which is partially sorted and one needs to determine minimum 
		number of modifications to make whole collection sorted. Do it in O(n*log n)

	Discussion:
		Basically do a comparison sort but instead of returning the ordered array, 
		you want me to return the minimum number of changes (to students location) to rearrange
		such that it makes the whole set ordered.
		
		You are additionally constraining me to a worst case of O(n*log n) which precludes these:
		  -  insertion sort with worst case time O(n^2)
		  -  selection sort with worst case time O(n^2) 
		I could use: mergesort, quicksort, heapsort
		But you want me to count continuous groups so it is not exactly the same as sorting...
		
		I could do something with the comparison of sorted array against the unsorted one
		Two arrays would comply with O(n) space complexity
		However, .NET's array sort uses insertion sort in some cases (16 or fewer elements) 
		which means that I can't use it.
*/

void Main()
{
	Tests.All();
}

public static class PartiallySorted
{
	public static int ChangeCounter(IList<int> collection)
	{
		if (collection == null || collection.Count <= 0)
			return 0;
	
		// We want to copies for comparison
		var sorted = collection
			.ToArray()
			.QuickSorted();
		
		int? firstLocation = null;
		var secondLocation = default(int?);
		
		for (int i = 0; i < collection.Count; i++)
		{
			bool isCorrectlySorted = collection[i] == sorted[i];
			if (!isCorrectlySorted)
			{
				if (!firstLocation.HasValue)
					firstLocation = i;
				else
					secondLocation = i;
			}
		}
		
		if (firstLocation.HasValue && secondLocation.HasValue)
			return secondLocation.Value - firstLocation.Value + 1;
		
		if (firstLocation.HasValue)
		{
			var valueOutOfPlace = collection[firstLocation.Value];
			var count = 0;
			for (int i = firstLocation.Value; i < collection.Count; i++)
			{
				if ( collection[i] > valueOutOfPlace) count++;
				else break;
			}
			
			return count;
		}
		
		return 0;
	}
}

public static class Sorting
{
	public static void QuickSort(int [] input)
	{
		QuickSort(input, 0, input.Length-1);
	}
	
	public static int[] QuickSorted(this int [] input)
	{
		QuickSort(input);
		return input;
	}
	
	/// <summary>Quicksort using CLRS' notation</summary>
	/// <param name="A">Array to sort</param>
	/// <param name="p">from where we partition</param>
	/// <param name="r">Is the pivot</param>
	private static int[] QuickSort(int [] A, int p, int r)
	{
		if (p < r)
		{
			var q = Partition(A, p, r);
			QuickSort(A, p, q-1);
			QuickSort(A, q+1, r);
		}
		
		return A;
	}

	private static void Exchange(int [] A, int i, int j)
	{
		var temp = A[i];
		A[i] = A[j];
		A[j] = temp;
	}
	
	private static int Partition(int [] A, int p, int r)
	{
		var x = A[r];
		var i = p - 1;
		for (int j = p; j <= r - 1; j++)
		{
			if (A[j] <= x)
			{
				i = i + 1;
				Exchange(A, i, j);
			}
		}
		
		Exchange(A, i+1, r);
		return i+1;
	} 
}

public static class Tests
{
	public static void All()
	{
		Runner(3, new []{1,2,6,5,5,8,9});
		Runner(4, new []{1,2,6, 5, 5, 5, 8,9});
		Runner(5, new []{1,2,6, 5, 5, 5, 4, 8, 9});
		Runner(2, new []{1,2,6,5,8,9});
	}

	private static void Runner(int expected, int [] input)
	{
		var actualResult = PartiallySorted.ChangeCounter(input);
		if (expected != actualResult)
			throw new Exception(
				string.Format(
					"Expected {0} but got {1} for {2}",
					expected,
					actualResult,
					string.Concat("[", string.Join(",", input), "]")));
		
		Console.WriteLine("Success");
	}
}