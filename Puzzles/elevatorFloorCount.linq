<Query Kind="Program" />

/*
	Description:
		Provide a way to count how many stops an elevator must take before
		carrying everyone waiting at the lobby.
*/

void Main()
{
	var floorCount = int.Parse(Util.ReadLine("How many floors does the building has?"));
	var elevator = Interaction.AskForElevator();
	var people = Interaction.AskForPeople(floorCount);
	var stops = CalculateStops(elevator, people);
	Console.WriteLine("The elevator will need to make {0} stops (ending at lobby) to carry everone.", stops);
}

public static int CalculateStops(Elevator elevator, Queue<Person> people)
{
	int totalStops = 0, currentWeight = 0, currentPeople = 0;
	var targetFloors = new HashSet<int>();
	while (people.Count > 0)
	{
		var nextPerson = people.Peek();
		bool
			canElevatorTakeNewWeight = currentWeight + nextPerson.Weight <= elevator.WeightLimit,
			canElevatorTakeNewPeople = currentPeople + 1 <= elevator.PeopleLimit;

		if (canElevatorTakeNewWeight && canElevatorTakeNewPeople)
		{
			currentWeight += nextPerson.Weight;
			currentPeople++;
			targetFloors.Add(nextPerson.TargetFloor);
			people.Dequeue(); //To actually remove it
		}
		else
		{
			totalStops += targetFloors.Count + 1; //+1 to return to lobby
			targetFloors.Clear();
			currentPeople = 0; currentWeight = 0;
		}
	}
	
	if (targetFloors.Count > 0)
		totalStops += targetFloors.Count + 1;
	
	return totalStops;
}

public sealed class Elevator
{
	public Elevator(int peopleLimit, int weightLimit)
	{
		Guards.IsPositive(peopleLimit, nameof(peopleLimit));
		Guards.IsPositive(weightLimit, nameof(weightLimit));

		this.PeopleLimit = peopleLimit;
		this.WeightLimit = weightLimit;
	}

	/// <summary>Maximum number of people the elevator can carry at once</summary>
	public int PeopleLimit { get; private set; }

	/// <summary>Maximum weight the elevator can carry</summary>
	public int WeightLimit { get; private set; }
}

public sealed class Person
{
	public int Weight { get; set; }
	public int TargetFloor { get; set;}
}

public static class Guards
{
	public static void IsPositive(int number, string name)
	{
		if (number > 0) return;
		
		var message = string.Format("Expected {0} to be positive (>0).");
		throw new ArgumentException(message);
	}
}

public static class Interaction
{
	public static Elevator AskForElevator()
	{
		var peopleLimit = int.Parse(Util.ReadLine("How many people can we fit?"));
		var weightLimit = int.Parse(Util.ReadLine("How many pounds can we carry?"));
		return new Elevator(peopleLimit, weightLimit);
	}
	
	public static Queue<Person> AskForPeople(int maxFloor)
	{
		var result =  new Queue<Person>();
		var peopleCount = int.Parse(Util.ReadLine("How many people should we queue?"));
		for (int i = 0; i < peopleCount; i++)
		{
			Person person = null;
			var isKosher = false;
			do
			{
				person = AskForPerson(maxFloor, i+1);
				isKosher = person != null;
			}
			while(!isKosher);
			
			result.Enqueue(person);
		}
		
		return result;
	}

	public static Person AskForPerson(int maxFloor, int personId)
	{
		int weight, targetFloor;
		do
		{
			var weightText = Util.ReadLine(string.Format("What is person's #{0}'s weight?", personId));
			if (!int.TryParse(weightText, out weight))
			{
				Console.Error.WriteLine("Expect a positive integer");
				continue;
			}
			
			break;
		}
		while (true);

		do
		{
			var floorText = Util.ReadLine(string.Format("What is person's #{0}'s target floor?", personId));
			if (!int.TryParse(floorText, out targetFloor))
			{
				Console.Error.WriteLine("Expect a positive integer less than or equal to {0}", maxFloor);
				continue;
			}
			
			break;
		}
		while (true);

		return new Person { TargetFloor = targetFloor, Weight = weight };
	}
}