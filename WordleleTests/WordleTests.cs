using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wordlele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Wordlele.Tests
{
    [TestClass()]
    public class WordleTests
    {
        IReadOnlyList<string> guessList = new List<string>() { "JELLO", "TRIAG", "CIGAR", "FRAGE", "LEGIT", "HELLO" };

        [TestMethod()]
        public void WordleSafeGuess()
        {    
            string word = "HELLO";
            Wordle wordle = new Wordle(word, guessList);

            wordle.TryGuess(null, out _).Should().BeFalse();
            wordle.TryGuess("HELLLO", out _).Should().BeFalse();
            wordle.TryGuess("HELO", out _).Should().BeFalse();
            wordle.TryGuess("HELL0", out _).Should().BeFalse();
            wordle.TryGuess("DOGGY", out _).Should().BeFalse();
            wordle.TryGuess("LEGIT", out _).Should().BeTrue();
            wordle.TryGuess("HELLO", out _).Should().BeTrue();
        }

        [TestMethod()]
        public void WordleUnSafeGuess()
        {
            string word = "HELLO";
            Wordle wordle = new Wordle(word, guessList);

            wordle.UnsafeGuess("LEGIT").ToList().Should().BeEquivalentTo(new List<sbyte>() { 1,2,0,0,0 });
            wordle.UnsafeGuess("JELLO").ToList().Should().BeEquivalentTo(new List<sbyte>() { 0,2,2,2,2 });
            wordle.UnsafeGuess("HELLO").ToList().Should().BeEquivalentTo(new List<sbyte>() { 2,2,2,2,2 });
            wordle.UnsafeGuess("HELLL").ToList().Should().BeEquivalentTo(new List<sbyte>() { 2,2,2,2,0 });
            wordle.UnsafeGuess("ELLLH").ToList().Should().BeEquivalentTo(new List<sbyte>() { 1,0,2,2,1 });
        }

        [TestMethod]
        public void WordleCanReset()
        {
            string word = "HELLO";
            Wordle wordle = new Wordle(word, guessList);

            // Run the option
            wordle.UnsafeGuess("HELLL").ToList().Should().BeEquivalentTo(new List<sbyte>() { 2, 2, 2, 2, 0 });
            wordle.GetLastResult().Should().BeEquivalentTo(new List<sbyte>() { 2, 2, 2, 2, 0 });

            // Reset and we expect empty LastResult
            wordle.Reset();
            Action act = () => wordle.GetLastResult();
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void CanSetupBenchmark()
        {
            // Setup game
            var rand = new Random(12345678);
            var valids = new List<string>(WordSets.GuessList);
            valids.AddRange(WordSets.WordList);
            int wordIdx = rand.Next(WordSets.WordList.Length);
            int guessWordIdx = rand.Next(valids.Count);

            var solvers = new List<IWordleSolver>();
            var wordles = new List<Wordle>();

            // For all the different solvers, populate and request the first guess
            solvers.Add(new PrunerRandom(WordSets.WordList, WordSets.GuessList, false, forcedFirstWord: WordSets.GuessList[guessWordIdx], randOverride: new Random(123)));
            //Solvers[1] = new PrunerRandom2(WordSets.WordList, WordSets.GuessList, false, forcedFirstWord: valids[guessWordIdx], randOverride: new Random(123));

            // Create all the wordles so the benchmarks don't step on each other
            wordles.Add(new Wordle(valids[wordIdx], valids));

            // Setup first guess and word states
            for (int i = 0; i < solvers.Count; i++)
            {
                wordles[i].TryGuess(solvers[i].GenerateGuess(), out _);
            }

            // Generate first guesses
            for (int i = 0; i < solvers.Count; i++)
            {
                solvers[i].GenerateGuess(wordles[i].GetLastResult()).Length.Should().BeGreaterThan(0);
            }
        }
    }
}