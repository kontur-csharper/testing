using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		/*
		 * Недостаток метода CheckCurrentTsar_WithCustomEquality() такой:
		 * При добавлении новых полей, обязательно нужно прописывать новые проверки на равенство полей
		 * В моем методе проверка по всем полям происходит автоматически
		 * При довавление нового поля, которое не нужно учитывать, нужно всего лишь прописать дополнительный Excluding()
		 * К тому же при падении теста в CheckCurrentTsar_WithCustomEquality() 
		 * Нужно будет искать ошибку в трассировке стека
		*/
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			actualTsar.Should().BeEquivalentTo(expectedTsar,
				options => options.AllowingInfiniteRecursion()
					.Excluding(x => x.SelectedMemberInfo.DeclaringType == typeof(Person) 
					                && x.SelectedMemberInfo.Name == nameof(Person.Id)));
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
}