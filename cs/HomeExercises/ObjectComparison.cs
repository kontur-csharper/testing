using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		private static readonly string[] excludedMembers = {nameof(Person.Id)};

		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			actualTsar.Should().BeEquivalentTo(expectedTsar, options => options
				.IncludingNestedObjects()
				.Excluding(mi => excludedMembers.Contains(mi.SelectedMemberInfo.Name)));
			//Селектор свойства не применяется к NestedObjects, поэтому проверяем по имени

			/*
			 * В отличие от решения ниже, при внесении изменений в класс Person изменения потребуются только в случае
			 * добавления новых членов класса, которые нужно будет обрабатывать как-то нестандартно (что маловероятно)
			 * Плюсом, в случае падения теста будет нормальный отчет, что именно пошло не так,
			 * а код ниже просто напишет "expected true, but was false", и придется искать причину через дебаггер,
			 * либо надо всё подряд логгировать в консоль,
			 * либо городить Assert.AreEqual(...) на каждый член класса, который нужно проверить
			 */
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			Assert.True(AreEqual(actualTsar, expectedTsar));
		}

		private bool AreEqual(Person? actual, Person? expected)
		{
			if (actual == expected) return true;
			if (actual == null || expected == null) return false;
			return
				actual.Name == expected.Name
				&& actual.Age == expected.Age
				&& actual.Height == expected.Height
				&& actual.Weight == expected.Weight
				&& AreEqual(actual.Parent, expected.Parent);
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