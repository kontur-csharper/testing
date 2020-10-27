using System.Linq;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		
		private static EquivalencyAssertionOptions<Person> ExcludingIdAndParent(EquivalencyAssertionOptions<Person> options)
		{
			return options
				.Excluding(person => person.Id)
				.Excluding(person => person.Parent);
		}
		
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Перепишите код на использование Fluent Assertions.
			actualTsar.Name.Should().Be(expectedTsar.Name);
			actualTsar.Age.Should().Be(expectedTsar.Age);
			actualTsar.Height.Should().Be(expectedTsar.Height);
			actualTsar.Weight.Should().Be(expectedTsar.Weight);

			var actualParent = actualTsar.Parent!;
			var expectedParent = expectedTsar.Parent!;

			actualParent.Name.Should().Be(expectedParent.Name);
			actualParent.Age.Should().Be(expectedParent.Age);
			actualParent.Height.Should().Be(expectedParent.Height);
			actualParent.Weight.Should().Be(expectedParent.Weight);
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			actualTsar.Should().BeEquivalentTo(expectedTsar, ExcludingIdAndParent);
			actualTsar.Parent.Should().BeEquivalentTo(expectedTsar.Parent, ExcludingIdAndParent);
			
			/* Почему это решение лучше:
			 * 1) Легко расширяемо - нужно будет только дописывать новые свойства в expectedTsar
			 * 2) Информативно при ошибке - дает информацию какие свойства отличаются
			 * 3) Код стал более читаемым
			 * 4) Тест продолжает корректно работать
			 */
		}
	}

	public class TsarRegistry
	{
		public static Person GetCurrentTsar()
		{
			return new Person(
				"Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
		}
	}

	public class Person
	{
		public static int IdCounter = 0;
		public int Age, Height, Weight;
		public string Name;
		public Person? Parent;
		public int Id;

		public Person(string name, int age, int height, int weight, Person? parent)
		{
			Id = IdCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
		}
	}
}