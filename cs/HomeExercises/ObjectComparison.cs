﻿using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{

    [TestFixture]
	public class ObjectComparison
	{

		private Person actualTsar;

		[SetUp]
		public void SetUp()
		{
			actualTsar = TsarRegistry.GetCurrentTsar();
        }

		[TearDown]
		public void TearDown() { }


        [Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			//добавлен билдер для класса
			//поэтому изменения в конструкторе в классе Person не сломют тесты.
			//Изменения нужно будет внести только в билдере. 
			var parent = TestPersonBuilder.APerson()
				.WithName("Vasili III of Russia")
				.WithAge(28)
				.WithHeight(170)
				.WithWeight(60).Build();

            var expectedTsar = TestPersonBuilder.APerson()
	            .WithName("Ivan IV The Terrible")
	            .WithAge(54)
	            .WithHeight(170)
                .WithWeight(70)
	            .WithParent(parent)
	            .Build();

			//тест переписан на Fluent Assertions
            actualTsar.Should().BeEquivalentTo(expectedTsar, options => options
				.Excluding(s => s.SelectedMemberInfo.DeclaringType == typeof(Person) &&
				                s.SelectedMemberInfo.Name == "Id"));
		}

		[Test]
		[NUnit.Framework.Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			Assert.True(AreEqual(actualTsar, expectedTsar));

            /*
			 Недостатки метода выше:
			 1. В случае, если класс Person будет расширяться, то в метод AreEqual нужно будет дописывать много сравнений. 
			 2. В случае, если поля класса Person будут не простыми типами (строки, числа), а сложными ссылочными типами с большим набором собственных полей, 
			 то придется позаботится и об их корректном сравнении. 
			 3. Если тест не прошел, то не будет понятно из-за чего. FluentAssertions дает, человекопонятное объяснение, что пошло не так. 
			 Например, Expected username to be "jonas", but "dennis" differs near 'd' (index 0)
			 4. У кастомного AreEqual нарушена принятая сигнатура метода. Cначала должен идти expected, затем actual. 
			 При чтении таких тестов могут возникнуть сложности - где-то первым actual, где-то expected. 
			 При использовании Fluent Assertions таких проблем не возникает. 
			 5. С использованием билдера при изменении сигнатуры конструктора класса Person тесты не сломаются, сломается только билдер,
			 который просто дополнить. Исправлять большое количество тестов - задача намного труднее. 
			 5. Функция сравнения лежит прямо в тестовом классе с модифкатором private, что делает ее переиспользование невозможным. 
			 Это может привести к дублированию кода (если вдруг эта функция понадобится где-нибудь в коде или в смежных тестах). 
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


	public class TestPersonBuilder
	{
		public const string DEFAULT_NAME = "John Smith";
		public const int DEFAULT_AGE = 42;
		public const int DEFAULT_HEIGHT = 175;
		public const int DEFAULT_WEIGHT = 175;

		private string name = DEFAULT_NAME;
		private int age = DEFAULT_AGE;
		private int height = DEFAULT_HEIGHT;
		private int weight = DEFAULT_WEIGHT;
		private Person parent;

		private TestPersonBuilder()
		{

		}

		public static TestPersonBuilder APerson()
		{
			return new TestPersonBuilder();
		}

		public TestPersonBuilder WithName(string name)
		{
			this.name = name;
			return this;
		}
		
		public TestPersonBuilder WithAge(int age)
		{
			this.age = age;
			return this;
		}
		public TestPersonBuilder WithHeight(int height)
		{
			this.height = height;
			return this;
		}

		public TestPersonBuilder WithWeight(int weight)
		{
			this.weight = weight;
			return this;
		}

		public TestPersonBuilder WithParent(Person parent)
		{
			this.parent = parent;
			return this;
		}

        public Person Build() => new Person(name, age, height, weight, parent);

    }
}