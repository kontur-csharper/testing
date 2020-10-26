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
			actualTsar.Should().BeEquivalentTo(expectedTsar, options => 
			options.Excluding(p => p.SelectedMemberInfo.DeclaringType == typeof(Person) && 
								   p.SelectedMemberInfo.Name == nameof(Person.Id)));
			// Почему лучше?
			// Первое - граммотное сообщение об ошибке
			// Второе - если появится новое поле, то код никак не изменится))
			// если же появилось поле типа Person (например второй родитель), то сравние пойдет рекурсивно по всем полям.
			// кроме того поле Id не проверяется только у объектов типа Person
			// если в Person будет инкапсулированно поле другого типа с Id, то Id будет сравниваться
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			// Если проверка не прошла, то выведет сообщение вида "ожидалась истина, а была ложь"
			// Что затрудняет понимание теста и сравнения объектов
			// Кроме того метод нерасширяем для Person - т.е. нельзя добавить новые поля. 
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