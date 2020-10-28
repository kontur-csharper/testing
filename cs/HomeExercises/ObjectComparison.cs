using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		/* Такое решение лучше предложенной альтернативы так-как:
		 1. Тест покроет все поля, в том числе те, которые могут быть добавлены в будущем. Следовательно при добавлении полей не придётся редактировать тест
		 2. Результаты теста выводятся в понятном формате
		 3. При ошибке в проверке предка можно сразу узнать глубину, на которой она произошла
		 4. Тест избегает StackOverflowException при проверке предков*/
		[Test]
		[Description("Проверка текущего царя (максимальная проверяемая глубина предков - 10)")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			actualTsar.Should().BeEquivalentTo(expectedTsar, opts =>
				opts.Excluding(x => x.SelectedMemberInfo.Name == nameof(actualTsar.Id))); //Исключаем id из проверки из-за особенностей его присвоения
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			// 1. Если тест упадёт, то придётся его дебажить, чтобы понять на каком конкретно этапе он упал.
			/* 2. Метод сравнения находится вне объекта, в случае изменения
			      (предположим что тесты и класс находятся в разных файлах) тест перестанет быть достоверным */
			// 3. Рекурсия в методе сравнения. Может возникнуть StackOverflowException
			// 4. Плохая читаемость результатов теста
			// 5. При добавлении полей придётся редактировать метод сравнения
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