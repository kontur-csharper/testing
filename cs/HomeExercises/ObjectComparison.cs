using System;
using FluentAssertions;
using FluentAssertions.Equivalency;
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
			
			actualTsar.Should().BeEquivalentTo(expectedTsar, options =>
				options.Excluding(o => ExcludeField(o, typeof(Person), "Id")));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			///В первую очередь нет смысла придумывать то, что уже реализовано(строить велосипед)
			/// можно было сделать через FluentAssertions
			/// предположим, что готового решения нет
			/// тогда лучшим вариантом было переопределить метод Equals(+GetHashCode)
			/// вместо написания своего метода
			/// также сама реализация плохо поддерживает расширяемость
			/// для каждого нового поля придется переписывать код
			/// лучшим решения я бы предложил использовать рекурсию
			/// и исключить в ручную поля которые не нужно сравнивать
			/// а так решение похоже на including в FluentAssertions
			/// кроме расширяемости такой подход плохо читаем
			/// т.е. другие пользователи кода будут тратить больше своего времени
			/// и также можно допустить больше ошибок
			Assert.True(AreEqual(actualTsar, expectedTsar));
		}

		private bool AreEqual(Person actual, Person expected)
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
		
		private bool ExcludeField(IMemberInfo memberInfo, Type typeToExclude, string propertyName)
		{
			return memberInfo.SelectedMemberInfo.DeclaringType == typeToExclude && 
			       memberInfo.SelectedMemberInfo.Name == propertyName;
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
		public Person Parent;
		public int Id;

		public Person(string name, int age, int height, int weight, Person parent)
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