using System;
using System.Collections;
using System.Collections.Generic;
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

		public void AddWordsToWordsStatistics(params string[] words)
		{
			foreach (var word in words)
			{
				wordsStatistics.AddWord(word);
			}
		}

		public string[] GetWordsFromStatistics()
		{
			return wordsStatistics
				.GetStatistics()
				.Select(wordCount => wordCount.Word)
				.ToArray();
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
		public void GetStatistics_OrderByWordCountDescending()
		{
			var wordsToAdd = new[] {"bb", "aa", "bb", "bb"};
			
			AddWordsToWordsStatistics(wordsToAdd);
			var words = GetWordsFromStatistics();
			
			Assert.That(words, Is.EqualTo(new[] {"bb", "aa"}));
		}

		[Test]
		public void GetStatistics_SeveralCall_GiveSameResult()
		{
			wordsStatistics.AddWord("ab");
			var first = wordsStatistics.GetStatistics();
			var second = wordsStatistics.GetStatistics();
			
			Assert.That(first, Is.EqualTo(second));
		}

		[Test]
		public void GetStatistics_IfCountEqual_OrderByLexicographic()
		{
			var wordsToAdd = new[] {"bb", "aa", "bb", "aa"};

			AddWordsToWordsStatistics(wordsToAdd);
			var words = GetWordsFromStatistics();
			
			Assert.That(words, Is.EqualTo(new[] {"aa", "bb"}));
		}

		[Test]
		public void AddWord_IfGotNull_ThrowArgumentNullException()
		{
			Assert.That(
				() => { wordsStatistics.AddWord(null); },
				Throws.ArgumentNullException);
		}

		[TestCase("", TestName = "Got empty string")]
		[TestCase(" ", TestName = "Got WhiteSpace strings")]
		public void AddWord_IfGotEmptyOrWhiteSpaceString_DoesntAddIt(string wordToAdd)
		{
			wordsStatistics.AddWord(wordToAdd);
			var words = GetWordsFromStatistics();
			
			Assert.That(words, Is.Empty);
		}

		[Test]
		public void AddWord_IfWordLengthGreaterThan10_AddOnlyFirst10Chars()
		{
			wordsStatistics.AddWord("abcdefghijk");
			var words = GetWordsFromStatistics();

			Assert.That(words, Is.EqualTo(new[] { "abcdefghij" }));
		}

		[Test]
		public void AddWord_IfWordLengthLessOrEqual10_AddAllWord()
		{
			wordsStatistics.AddWord("abc");
			var words = GetWordsFromStatistics();

			Assert.That(words, Is.EqualTo(new[] {"abc"}));
		}
		
		[Test]
		public void AddWord_IgnoreCaseOfWord()
		{
			var wordsToAdd = new[] {"abc", "ABC"};
			
			AddWordsToWordsStatistics(wordsToAdd);
			var words = GetWordsFromStatistics();
			
			Assert.That(words, Is.EqualTo(new[] {"abc"}));
		}
		
		[Test]
		public void AddWord_CorrectCount()
		{
			var wordsToAdd = new[] {"ab", "ab"};
			
			AddWordsToWordsStatistics(wordsToAdd);
			var wordCount = wordsStatistics.GetStatistics().First();
			
			Assert.That(wordCount, Is.EqualTo(new WordCount("ab", 2)));
		}
		// Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
	}
}