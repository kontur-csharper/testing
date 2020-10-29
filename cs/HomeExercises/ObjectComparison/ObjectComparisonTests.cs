using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.ObjectComparison
{
	public class ObjectComparisonTests
	{
		private readonly HashSet<string> excludingFieldNames = new HashSet<string> {nameof(Person.Id)};

		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			actualTsar.Should().BeEquivalentTo(expectedTsar, option => option
				.AllowingInfiniteRecursion()
				.Excluding(person => excludingFieldNames.Contains(person.SelectedMemberInfo.Name)));
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

		/*
		 * Первый недостаток такого подхода - расширяемость. Если у класса Person появятся новые поля,
		 * придется прописывать сравнение этих полей вручную в методе AreEqual.
		 *
		 * Второй недостаток - неинформативность. При падении такого теста будет абсолютно не ясно, в каком месте произошла ошибка,
		 * в этом плане подход с использованием FluentAssertions сильно выигрывает, так как дает описание непройденного теста,
		 * в котором укажет, при сравнении каких полей произошла ошибка.
		 *
		 * Третий недостаток - по сравнению с FluentAssertion недостаточная читаемость.
		 */
		
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