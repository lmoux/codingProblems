<Query Kind="Program" />

/*
 * Instructions:
 *	Consider a directed graph of small non-negative integers where
 *	each integer is less than 60,000 and each integer is unique.
 *	In this case, a directed graph is a data strucutre where a node
 *	is represented by a unique integer and each node has zero or more 
 *	child node. As above, don't just use an existing graph library.
 *		- Write a function that creates a node in a graph.
 *		- Write a function that inserts a node as a child of another node.
 *		- These functions should not allow cycles to be created. That is, a node
 *			may not directly or indirectly point to itself.
 *		- Write a function to print out a graph.
 *
 *	Here is a simple example graph without cycles:
 *		1 -> 2, 3, 4
 *		2 -> 5
 *		3 -> 6
 *		4 -> 3, 6
 *		5 -> 6
 *		6 -> No children
 */
 
void Main()
{
//	var graph = Testing.MainExample();
//	Testing.SpotCheckTraversals();
	Testing.SpotCheckPrintingOfGraph();
	Testing.SimulateUniquenessViolation();
	Testing.SimulateCycleSelf();
	Testing.SimulateCycleOther();
}

/// <summary>Represents a directed acyclical graph</summary>
public sealed class Graph<T>
{
	public Graph(bool enforceUniquenessOfValues = true)
	{
		this.Nodes = new List<Node>();
		this.AllNodeReferences = new List<Node>();
		this.ShouldEnforceUniquenessOfValues = enforceUniquenessOfValues;
		this.UniqueValues = new HashSet<T>();
	}
	
	public bool ShouldEnforceUniquenessOfValues {get; private set;}
	public List<Node> Nodes {get; private set;}
	public Node Root {get; private set;}
	private List<Node> AllNodeReferences {get; set;}
	private HashSet<T> UniqueValues {get; set;}
	
	public Node CreateNode(T value)
	{
		this.EnforceUniquenessOfValuesNow(value);
		var node = new Node(value, this);

		this.Nodes.Add(node);
		this.AllNodeReferences.Add(node);
		
		if (this.Root == null)
			this.Root = node;

		return node;
	}
	
	private void EnforceUniquenessOfValuesNow(T value)
	{
		if (this.ShouldEnforceUniquenessOfValues)
		{
			if(this.UniqueValues.Contains(value))
				throw new ArgumentException(
					"That value is already represented and you specified we should enforce uniqueness of values");
			else
				this.UniqueValues.Add(value);
		}
	}

	#region Nodes
	public sealed class Node
	{
		public Node(T value, Graph<T> graph = null)
		{
			this.Connections = new List<Node>();
			this.Value = value;
			this.Visited = false;
			this.AssociatedGraph = graph;
			if (graph != null)
			{
				graph.AllNodeReferences.Add(this);
			}
		}
	
		public Graph<T> AssociatedGraph {get; private set;}
		public List<Node> Connections {get; private set;}
		public T Value {get; private set;}
		public bool Visited {get; set;}
		private bool associatedWithGraph {get { return this.AssociatedGraph != null; } }
		public void Insert(Node node)
		{
			if (Object.ReferenceEquals(this, node)) 
				throw new Exception("Inserting nodes into themselves is disallowed because it would form a cycle");
			
			var subGraph = associatedWithGraph ?
				this.AssociatedGraph.DepthFirstTraversalRecursive(node).ToList() :
				new List<Node>();
			this.AssociatedGraph.ResetVisited();
			if (subGraph.Any(x => Object.ReferenceEquals(this, x)))
			{
				throw new Exception("Inserting this node is disallowed because it would form a cycle");
			}
			
			this.Connections.Add(node);
		}
		
		public Node InsertNew(T value)
		{	
			if (this.associatedWithGraph)
				this.AssociatedGraph.EnforceUniquenessOfValuesNow(value);
		
			var node = new Node(value, this.AssociatedGraph);	
			this.Insert(node);
			
			return node;
		}
		
		public override string ToString()
		{
			return string.Format(
				"{0} -> {1}",
				this.Value,
				this.Connections.Any() ?
					string.Join(", ", this.Connections.Select (c => c.Value)):
					"No children");
		}
	}
	#endregion
	
	#region Helpers
	public void ResetVisited()
	{
		for (var i = 0; i < this.AllNodeReferences.Count; i++)
		{
			var node = this.AllNodeReferences[i];
			node.Visited = false;
		}
	}
	
	public IEnumerable<Node> BreathFirstTraversal(Node root = null)
	{
		this.ResetVisited();
		var queue = new Queue<Node>();
		
		if (root == null)
		{
			foreach(var node in this.Nodes)
				queue.Enqueue(node);
		}
		else
		{
			queue.Enqueue(root);
		}
		
		while (queue.Any())
		{
			var current = queue.Dequeue();
	
			if (!current.Visited)
				yield return current;
			
			current.Visited = true;			
			foreach (var node in current.Connections)
			{
				if (!node.Visited)
				{
					queue.Enqueue(node);
				}
			}
		}
	}
	
	public IEnumerable<Node> DepthFirstTraversalIterative(Node root = null)
	{
		var wasRootSpecified = root != null;
		var stack = new Stack<Node>();
		
		IList<Node> startingPoint = wasRootSpecified ? 
			root.AsSingleMemberCollection().ToList() : 
			this.Nodes;

		foreach (var raiz in startingPoint)
			stack.Push(raiz);

		while(stack.Any())
		{
			var current = stack.Pop();
			if (!current.Visited)
			{
				yield return current;
				current.Visited = true;
				for (int i = 0; i < current.Connections.Count; i++)
					stack.Push(current.Connections[i]);
			}
		}
	}
	
	public IEnumerable<Node> DepthFirstTraversalRecursive(Node root = null)
	{
		var wasRootSpecified = root != null;
	
		IList<Node> startingPoint = wasRootSpecified ? 
			root.AsSingleMemberCollection().ToList() : 
			this.Nodes;
		
		if (!wasRootSpecified)
			this.ResetVisited();
		
		for (var i = 0; i < startingPoint.Count; i++)
		{
			var raiz = startingPoint[i];
			if (!raiz.Visited)
				yield return raiz;
			
			raiz.Visited = true;
			foreach (var vertex in raiz.Connections)
			{
				if (!vertex.Visited)
					foreach(var v in DepthFirstTraversalRecursive(vertex))
						yield return v;
			}
		}
	}
	
	#endregion
	
	public override string ToString()
	{
		var builder = new StringBuilder();
		foreach (var node in this.BreathFirstTraversal().OrderBy (t => t.Value))
		{
			builder.AppendLine(node.ToString());
		}
		
		return builder.ToString();
	}
}

public static class Extensions
{
	public static IEnumerable<T> AsSingleMemberCollection<T>(this T target)
	{
		// Alternatively we could do the following but I prefer this approach
		///		return new [] { target } 
		yield return target;
	}

	public static IEnumerable<T> NullSafe<T>(this IEnumerable<T> target)
	{
		return target ?? Enumerable.Empty<T>();
	}
	
	public static void Operate<T>(this IEnumerable<T> collection, Action<T> work)
	{
		if (work == null) return;
		foreach (var item in collection.NullSafe() )
		{
			work(item);
		}
	}
	
	public static HashSet<T> ToHashSet<T>(this IEnumerable<T> collection)
	{
		return new HashSet<T>(collection.NullSafe());
	}
}

public static class Testing
{
	public static Graph<int> MainExample()
	{
		var graph = new Graph<int>();
		var one = graph.CreateNode(1);
		var two = one.InsertNew(2);
		var three = one.InsertNew(3);
		var four = one.InsertNew(4);
		var five = two.InsertNew(5);
		
		/* 
		 * This pattern isn't supported right now (for uniqueness enforcement):
		 * 		var six = new Graph<int>.Node(6, graph);
		 * 		three.Insert(six);
		 * Please don't use the constructor directly!
		 */
		
		var six = three.InsertNew(6);
		four.Insert(three);
		four.Insert(six);
		five.Insert(six);
		
		return graph;
	}

	public static void SpotCheckTraversals()
	{
		Console.WriteLine("> SpotCheckTraversals");
		var graph = Testing.MainExample();
		
		Console.WriteLine("-------BreathFirstTraversal");
		graph.ResetVisited();
		graph.BreathFirstTraversal()
			.Operate(x => Console.WriteLine(x.ToString()));
			
		Console.WriteLine("-------BreathFirstTraversal");
		graph.ResetVisited();	
		graph.BreathFirstTraversal()
			.Operate(x => Console.WriteLine(x.ToString()));
	
		Console.WriteLine("-------DepthFirstTraversalIterative");
		graph.ResetVisited();
		graph.DepthFirstTraversalIterative()
			.Operate(x => Console.WriteLine(x.ToString()));
	
		Console.WriteLine("-------DepthFirstTraversalRecursive");
		graph.ResetVisited();
		graph.DepthFirstTraversalRecursive()
			.Operate(x => Console.WriteLine(x.ToString()));
	
	}
	
	public static void SpotCheckPrintingOfGraph()
	{
		Console.WriteLine("> SpotCheckPrintingOfGraph");
		var graph = MainExample();
		Console.WriteLine(graph.ToString());
	}
	
	public static void SimulateUniquenessViolation()
	{
		try
		{
			var graph = MainExample();
			graph.CreateNode(6);
			Console.WriteLine("> SimulateUniquenessViolation: failure as I expected an exception!");
		}
		catch
		{
			Console.WriteLine("> SimulateUniquenessViolation: success");
		}
	}
	
	public static void SimulateCycleSelf()
	{
		try
		{
			var graph = MainExample();
			var seven = graph.CreateNode(7);
			seven.Insert(seven);
			Console.WriteLine("> SimulateCycleSelf: failure as I expected an exception!");
		}
		catch
		{
			Console.WriteLine("> SimulateCycleSelf: success");
		}
	}
	
	public static void SimulateCycleOther()
	{
		try
		{
			var graph = MainExample();
			var seven = graph.CreateNode(7);
			var eight = seven.InsertNew(8);
			eight.Insert(seven);
			Console.WriteLine("> SimulateCycleOther: failure as I expected an exception!");
		}
		catch
		{
			Console.WriteLine("> SimulateCycleOther: success");
		}
	}
}