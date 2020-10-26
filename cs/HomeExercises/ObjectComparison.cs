using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Перепишите код на использование Fluent Assertions.
			Assert.AreEqual(actualTsar.Name, expectedTsar.Name);
			Assert.AreEqual(actualTsar.Age, expectedTsar.Age);
			Assert.AreEqual(actualTsar.Height, expectedTsar.Height);
			Assert.AreEqual(actualTsar.Weight, expectedTsar.Weight);

			Assert.AreEqual(expectedTsar.Parent!.Name, actualTsar.Parent!.Name);
			Assert.AreEqual(expectedTsar.Parent.Age, actualTsar.Parent.Age);
			Assert.AreEqual(expectedTsar.Parent.Height, actualTsar.Parent.Height);
			Assert.AreEqual(expectedTsar.Parent.Parent, actualTsar.Parent.Parent);
		}

		[Test]
		[Description("Отрефакторенный тест CheckCurrentTsar")]
		public void GetCurrentTsar_CheckCurrentTsar_Correct()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			actualTsar.Should().BeEquivalentTo(expectedTsar, options => options.
				Excluding(x => x.SelectedMemberInfo.Name.Equals("Id") && x.SelectedMemberInfo.DeclaringType == typeof(Person)));
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

	/*
	1. В моём тесте больше информации, где именно, с какими данными произошла ошибка.
		Вывод при ошибке:
		• CheckCurrentTsar_WithCustomEquality()
			Expected: True
			But was:  False
		• GetCurrentTsar_CheckCurrentTsar_Correct()
			Expected member Parent.Height to be 170, but found 171.
	2. При добавлении полей придётся лезть в  альтернативный тест. Если тестов, в которых создается экземпляр класса Person 100, 
	то не завидую. В моём же тесте всё это возлагается на FluentAssertions.
	3. В противовес можно сказать, что если мы добавили поле, которое не нужно сравнивать(как, к примеру, Id), то в моём тесте придётся 
	вручную указывать это. Но такие уникальные поля скорее всего будут появляться максимально редко.
	4. Хочу отметить, что метод AreEqual при такой реализации придётся запихивать и в другие тесты, где есть сравнение двух экземпляров 
	класса Person. Лучше переопределить Equals + GetHashCode у класса Person.
	5. Читеаемость моего теста лучше.
	6. В альтернативном решении может возникнуть бесконечная рекурсия, если, к примеру, персона N будет являться своим же отцом.
	Будет StackOverflowException.
	*/

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