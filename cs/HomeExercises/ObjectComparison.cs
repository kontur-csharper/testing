﻿using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		[Test]
		[Description("Проверка текущего царя")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			actualTsar.ShouldBeEquivalentTo(
				expectedTsar,
				options => options
					.AllowingInfiniteRecursion()
					.IncludingFields()
					.Excluding(o => o.SelectedMemberPath.EndsWith(nameof(Person.Id)))
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
			Assert.True(AreEqual(actualTsar, expectedTsar));
			/*
			Недостатки:
			1)Из теста непонятно, как именно сравниваются объекты
			2)Сравнение выносится из теста в другую часть кода (ненужное разделение)
			3)"Написание велосипеда" - сравнение можно произвести без написания собственного метода
			(который, к слову, может содержать ошибки), а с использованием библиотеки.
			4)Если тест не пройдет, не будет ясно, по какой причине это произошло - придётся
			проходить метод .AreEqual() дебаггером.
			5)При добавлении новых полей класса придётся параллельно вносить новые сравнения
			в метод теста.
			6)Плохая читаемость - приходится проходить глазами длинную логическую цепочку условий
			*/
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