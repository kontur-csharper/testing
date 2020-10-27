using System;
using System.Collections;
using System.Linq;
using System.Text;
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
		public void AddWord_Throws_AfterAddingNull()
		{
			Assert.Throws<ArgumentNullException>(() => wordsStatistics.AddWord(null));
		}

		[Test]
		public void AddWord_CutsWord_WordLenBiggerThan10()
		{
			wordsStatistics.AddWord("abcdefghijklmnop");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("abcdefghij", 1));
		}

		[Test]
		public void GetStatistics_ContainsItemSeveralTimes_OneWordSeveralTimes()
		{
			wordsStatistics.AddWord("a");
			wordsStatistics.AddWord("a");
			wordsStatistics.AddWord("a");
			wordsStatistics.AddWord("a");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("a", 4));
		}

		[Test]
		public void GetStatistics_ShouldIgnoreUppercase_WordInUppercase()
		{
			wordsStatistics.AddWord("AAA");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("aaa", 1));
		}

		[Test]
		public void AddWord_ShouldIgnoreEmptyString_EmptyString()
		{
			wordsStatistics.AddWord("");
			wordsStatistics.GetStatistics().Should().HaveCount(0);
		}

		[Test]
		public void AddWord_ShouldIgnoreBlankString_BlankString()
		{
			wordsStatistics.AddWord("                             \t \n \r\v");
			wordsStatistics.GetStatistics().Should().HaveCount(0);
		}

		[Test]
		public void GetStatistic_OrderedByCount_AfterAdditionOfDifferentWordsWithDiffFreq()
		{
			wordsStatistics.AddWord("a");
			wordsStatistics.AddWord("b");
			wordsStatistics.AddWord("b");

			wordsStatistics.GetStatistics().Should().Equal(new [] {new WordCount("b", 2), new WordCount("a", 1)});
		}

		[Test]
		public void AddWord_ShouldIncreaceCountWithUpperCase_SameWordInUpperCaseAndLower()
		{
			wordsStatistics.AddWord("aaa");
			wordsStatistics.AddWord("AAA");
			
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("aaa", 2));
		}

		[Test]
		public void GetStatistics_OrderedByName_DiffWord()
		{
			wordsStatistics.AddWord("bb");
			wordsStatistics.AddWord("aa");
			wordsStatistics.GetStatistics().Should().Equal(new [] {new WordCount("aa", 1), new WordCount("bb", 1)});
		}

		[Test]
		public void AddWord_IncreaseCount_LongWordsWithSameBegin()
		{
			wordsStatistics.AddWord("aaaaaaaaaabbb");
			wordsStatistics.AddWord("aaaaaaaaaaccc");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("aaaaaaaaaa", 2));
		}

		[Test]
		public void AddWord_OrderedByName_DiffWordsManyTimes()
		{
			wordsStatistics.AddWord("bb");
			wordsStatistics.AddWord("bb");
			wordsStatistics.AddWord("aa");
			wordsStatistics.AddWord("aa");
			wordsStatistics.GetStatistics().Should().Equal(new [] {new WordCount("aa", 2), new WordCount("bb", 2)});
		}

		[Test]
		public void AddWord_AddBlankWord_WordWith10SpacesAtBegin()
		{
			wordsStatistics.AddWord("                            s \t \n \r\v");
			wordsStatistics.GetStatistics().Should().HaveCount(1);
		}

		[Test]
		public void AddWord_ShouldntCutWWord_Word10Len()
		{
			wordsStatistics.AddWord("1234567890");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("1234567890", 1));
		}
		
		[Test]
		public void AddWord_ShouldCutWWord_Word11Len()
		{
			wordsStatistics.AddWord("12345678901");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("1234567890", 1));
		}

		/*[Test]
		public void GetStatistics_ShouldntHaveLimitCapacity_3000DiffWords()
		{
			var builder = new StringBuilder();
			for (var i = 0; i < 3000; i++)
			{
				builder.Append(i.ToString());
				wordsStatistics.AddWord(builder.ToString());
			}

			wordsStatistics.GetStatistics().Count().Should().BeGreaterThan(2500);
		}*/

		[Test]
		public void GetStatistics_ShouldntClear_OneWord()
		{
			wordsStatistics.AddWord("1");
			wordsStatistics.GetStatistics().Should().HaveCount(1);
			wordsStatistics.GetStatistics().Should().HaveCount(1);
		}

		[Test]
		public void GetStatistics_ShouldReturnIEnumerable()
		{
			var w = "AA";
			wordsStatistics.AddWord(w);
			w.Should().Be("AA");
		}

		// Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
	} 
}