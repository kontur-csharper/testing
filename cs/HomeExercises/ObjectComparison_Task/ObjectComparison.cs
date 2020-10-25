using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.ObjectComparison_Task
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
			actualTsar.Should().BeEquivalentTo(expectedTsar,
				options => options.AllowingInfiniteRecursion().Excluding(memberInfo =>
					memberInfo.SelectedMemberInfo.DeclaringType == typeof(Person) &&
					memberInfo.SelectedMemberInfo.Name == nameof(Person.Id)));
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
			// При падении данного теста будет выведено сообщение: Expected: True, But was: False
			// Это является крайне неинформативным, будет трудно понять, где произошла ошибка
			// Если программист добавит новые поля в Person, то в тест придется вносить целый ряд изменений
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
}