using System;
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
			expectedTsar.Should().BeEquivalentTo(actualTsar, options => options
				.Excluding(
					o => o.SelectedMemberInfo.DeclaringType == typeof(Person)
					     && o.SelectedMemberInfo.Name == nameof(Person.Id)));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		/*
		 * Как минимум - не используется fluentAssertion
		 * 
		 * Далее - учитывая, что тест должен проходить даже при несовпадающих id,
		 * автору данного теста пришлось городить собственный метод, проверяющий равенство объектов.
		 * Во первых, это просто дольше и сложнее писать писать:
		 * приходится вручную определять, какие именно поля должны быть равны
		 * и при каких дополнительных условиях (вроде если оба объекта null) они будут/не будут равны.
		 * Во вторых, при добавлении новых полей в person придётся дополнять тест.
		 * Да и просто - определять, какие поля ДОЛЖНЫ быть равны,
		 * когда нам нужно лишь НЕ УЧИТЫВАТЬ какое-то поле - излишне. Не учитывать поле нам нужно чаще.
		 *
		 * Далее - Assert.True() просто выдаст, что в строчке с ней должен быть true, а не false,
		 * в то время, как FluentAssertion подробно распишет,
		 * какое именно поле отличается, как именно отличается,
		 * какие правила для сравнения использовались и.т.п
		 */
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