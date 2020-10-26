using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		/// <summary>
		/// Альтернативное решение не даёт информации о том, какие данные были неверными.
		/// Может вызвать переполнение стека вызовов.
		/// При добавлении нового поля в классе Person нужно добавлять проверку в методе AreEqual.
		///
		/// FluentAssertions позволяет избавиться от вложенных вызовов методов.
		/// Например: Assert.True(AreEqual(actualTsar, expectedTsar)).
		/// Should() явно указывает на актуальное и ожидаемое значения.
		/// Сообщение о неверном результате предоставляет полную информацию.
		/// Обнаружение цикличных ссылок.
		/// При добавлении нового поля нет необходимости вносить изменения в тест.
		/// </summary>
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			var idFieldName = nameof(Person.Id);
			actualTsar.Should().BeEquivalentTo(expectedTsar, options => options
				.Excluding(info => info.SelectedMemberInfo.Name == idFieldName));
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