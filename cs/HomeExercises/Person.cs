namespace HomeExercises
{
	public class Person
	{
		private static int idCounter;
		public readonly int Age;
		public readonly int Height;
		public readonly string Name;
		public readonly int Weight;
		public int Id;
		public readonly Person? Parent;

		public Person(string name, int age, int height, int weight, Person? parent)
		{
			Id = idCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
		}
	}
}