using FluentAssertions;
using HomeExercises;
using NUnit.Framework;
using HomeExercises;

namespace HomeExercisesTests
{
	public class ObjectComparison
	{
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			var actualTsar = TsarRegistry.GetCurrentTsar();
			
			actualTsar.Should().BeEquivalentTo(expectedTsar, options => options
				.Excluding(su => su.SelectedMemberInfo.Name == "Id")
				.AllowingInfiniteRecursion() // Fluent Assertions will recurse up to 10 levels deep by default
			);
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			/*
			 * 1) Сохранить правильность теста при изменении Person возможно с правками AreEqual.
			 * 2) Читабельность снижается ввиду анализа различных условных и логических операторов.
			 * 3) В случае падения теста понять какие поля не совпали - невозможно.
			 */
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
}