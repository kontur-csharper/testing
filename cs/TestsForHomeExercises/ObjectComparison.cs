using FluentAssertions;
using HomeExercises;
using NUnit.Framework;

namespace TestsForHomeExercises
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
			actualTsar.Should().BeEquivalentTo(expectedTsar, option
				=> option.Excluding(memberInfo => memberInfo.SelectedMemberInfo.Name.Equals("Id")
				                                  && memberInfo.SelectedMemberInfo.DeclaringType.Equals(typeof(Person)))
					.AllowingInfiniteRecursion());
		}

		//В решении, представленном в методе  CheckCurrentTsar_WithCustomEquality()
		// используется дополнительный метод AreEqual(Person actual, Person expected). 
		// В случае добавления новых полей, участвующих в сравнении, в класс Person
		// метод  AreEqual будет необходимо переписать. Решение с использованием FluentAssertation 
		// в таком случае продолжит работать без каких-либо изменений в коде.
		// Кроме того, в тесте из альтернативного решения не очевидно, какие поля класса Person 
		// игнорируются при сравнении. 
		// При падении теста CheckCurrentTsar_WithCustomEquality() не понятно, какие поля сравниваемых экземпляров
		// не совпадают, выводится только сообщение "Expected: True. But was: False"

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
}