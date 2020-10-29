using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Challenge
{
	[TestFixture]
	public class WordsStatistics_Tests
	{
		public virtual IWordsStatistics CreateStatistics()
		{
			// меняется на разные реализации при запуске exe
			return new WordsStatistics();
		}

		private IWordsStatistics wordsStatistics;

		[SetUp]
		public void SetUp()
		{
			wordsStatistics = CreateStatistics();
		}

		[Test]
		public void GetStatistics_IsEmpty_AfterCreation()
		{
			wordsStatistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void GetStatistics_HandlesManyWords()
		{
			for (var i = char.MinValue; i < char.MinValue + 100; i++)
			{
				for (var j = char.MinValue; j < char.MinValue + 100; j++)
					wordsStatistics.AddWord(i.ToString() + j);
			}

			wordsStatistics.GetStatistics().First().Word.Should().Be("aa");
		}

		[Test]
		public void GetStatistics_ContainsItem_AfterAddition()
		{
			wordsStatistics.AddWord("abc");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("abc", 1));
		}

		[Test]
		public void GetStatistics_ContainsManyItems_AfterAdditionOfDifferentWords()
		{
			wordsStatistics.AddWord("abc");
			wordsStatistics.AddWord("def");
			wordsStatistics.GetStatistics().Should().HaveCount(2);
		}

		[Test]
		public void GetStatistics_ChangesAfterAddedWord()
		{
			wordsStatistics.AddWord("abc");
			var a = wordsStatistics.GetStatistics();
			wordsStatistics.GetStatistics().Should().HaveCount(1);
			wordsStatistics.AddWord("def");
			var b = wordsStatistics.GetStatistics();
			wordsStatistics.GetStatistics().Should().HaveCount(2);
		}

		[Test]
		public void GetStatistics_OrdersWordsWithEqualCountByAplabet()
		{
			wordsStatistics.AddWord("abc");
			wordsStatistics.AddWord("abc");
			wordsStatistics.AddWord("def");
			wordsStatistics.AddWord("def");
			wordsStatistics.GetStatistics().First().Word.Should().Be("abc");
		}

		[Test]
		public void GetStatistics_OrderByCountIsMain()
		{
			wordsStatistics.AddWord("abc");
			wordsStatistics.AddWord("abc");
			wordsStatistics.AddWord("def");
			wordsStatistics.AddWord("def");
			wordsStatistics.AddWord("def");
			wordsStatistics.GetStatistics().First().Word.Should().Be("def");
		}

		[Test]
		public void GetStatistics_HandlesCutWordsCorrect()
		{
			wordsStatistics.AddWord("abcdefghijklmnopqrstu");
			wordsStatistics.AddWord("abcdefghij");
			wordsStatistics.GetStatistics().First().Count.Should().Be(2);
		}

		[Test]
		public void AddWord_ThrowsArumentNullExceptionWhenWordIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => wordsStatistics.AddWord(null));
		}

		[Test]
		public void AddWord_DoesntAddEmptyLine()
		{
			wordsStatistics.AddWord("");
			wordsStatistics.GetStatistics().Count().Should().Be(0);
		}

		[Test]
		public void AddWord_DoesntAddSpace()
		{
			wordsStatistics.AddWord("   ");
			wordsStatistics.GetStatistics().Count().Should().Be(0);
		}

		[Test]
		public void AddWord_SavesSpacesFromCutWord()
		{
			wordsStatistics.AddWord("          abc");
			wordsStatistics.GetStatistics().Count().Should().Be(1);
			wordsStatistics.GetStatistics().First().Word.Should().Be("          ");
		}

		[Test]
		public void AddWord_MakesWordsLower()
		{
			wordsStatistics.AddWord("AAABBBCC123");
			wordsStatistics.AddWord("aaaBBbCC123");
			wordsStatistics.GetStatistics().First().Word.Should().Be("aaabbbcc12");
			wordsStatistics.GetStatistics().First().Count.Should().Be(2);
		}

		[Test]
		public void AddWord_HandlesWithLanguagesCorrectly()
		{
			wordsStatistics.AddWord("abc");
			wordsStatistics.AddWord("абв");
			wordsStatistics.GetStatistics().First().Word.Should().Be("abc");
		}
		// Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
	}
}