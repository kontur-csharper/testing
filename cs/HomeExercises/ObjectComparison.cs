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

			actualTsar.Should().BeEquivalentTo(expectedTsar, options =>
				options.Excluding(info =>
					info.SelectedMemberInfo.DeclaringType == typeof(Person) &&
					info.SelectedMemberInfo.Name == "Id"));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			/* 1) Этот вариант менее расширяем чем решение с использованием
			 *    Fluent Assertions (после добавления новых свойств в класс Person нам необходимо
			 * 	  влезать в метод AreEqual и добавлять туда новые сравнения).
			 *    Также у нас добавляется дополнительное ограничение на сравниваемые в AreEqual свойcтва/поля
			 *    класса Person: если мы хотим сравнивать эти поля по значению, то их тип должен переопределять
			 *    должным образом метод Equal или оператор сравнения. В то время как BeEquivalentTo сравнивает
			 *    графы объектов (object graph).
			 * 
			 * 2) Это труднее читать: мы используем метод Assert.True, в качестве параметра
			 *    которого у нас результат работы функции AreEqual (имя которой совпадает с
			 *    Assert.AreEqual), и нам ещё нужно залезать в код этого метода,
			 *    чтобы понять что там происходит, сверху добавляется рекурсивность метода и
			 *    значит нам ещё нужно искать где там у него условие выхода из рекурсии.
			 *    Из-за рекурсивности ещё вынужденно поставлен nullable-reference тип в качестве параметра,
			 *    так как выход из рекурсии осуществляется когда передается свойтсво Parent равное null.
			 *    Этот код выглядит излишне сложным.
			 *
			 *  Решение с использование FluentAssertions имеет лучшую читаемость и расширяемость.
			 *  При добавлении новых свойств/полей в класс Person потенциально код теста не нужно менять,
			 *  так как делаться это будет только если новые свойства/поля не должны сравниваться.
			 *  Но у решения с использованием IMemberInfo есть недостаток - возможны проблемы с тестом после
			 *  рефакторинга. Если мы переименуем исключаемое свойство, то нам нужно лезть в тест и вручную
			 *  изменять имя в делегате, передаваемому Excluding в качестве параметра.
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