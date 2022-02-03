using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Wordlele.Tests
{
    [TestClass()]
    public class SolverTests
    {
        IReadOnlyList<string> GuessList = new List<string>() { "JELLO", "TRIAG", "CIGAR", "FRAGE", "LEGIT", "HELLO" };

        [TestMethod()]
        public void SolversCanReset()
        {
            void VerifyInner(IWordleSolver solver)
            {
                string first = solver.GenerateGuess();
                first.Should().BeUpperCased();
                // Re-running first guess should solvereserve the first
                solver.GenerateGuess().Should().Be(first);
                solver.GetPossibleGuessCount().Should().Be(GuessList.Count);

                // Resetting should give different result to the first
                int tries = 100;
                while (tries-- > 0)
                {
                    solver.Reset();
                    string next = solver.GenerateGuess();
                    next.Should().BeUpperCased();
                    solver.GetPossibleGuessCount().Should().Be(GuessList.Count);
                    if (!next.Equals(first))
                    {
                        break;
                    }
                }
                // We expected reset to give different guesses
                tries.Should().BePositive();

                // Run a game guess - there should be less possible options
                Wordle w = new Wordle(GuessList[0], GuessList);
                w.TryGuess(solver.GenerateGuess(), out var result).Should().BeTrue();
                _ = solver.GenerateGuess(result);
                solver.GetPossibleGuessCount().Should().BeLessThan(GuessList.Count);
            }

            IWordleSolver pr = new PrunerRandom(GuessList, GuessList, false);
            VerifyInner(pr);
        }

        [TestMethod()]
        public void SolversCanSolveSafely()
        {
            void VerifyInner(IWordleSolver solver)
            {
                Wordle w = new Wordle(GuessList[0], GuessList);
                _ = w.PlayGame(solver, Unsafe: false);
                w.Reset();
                solver.Reset();
                _ = w.PlayGame(solver, Unsafe: true);
            }

            IWordleSolver pr = new PrunerRandom(GuessList, GuessList, false);
            VerifyInner(pr);
        }
    }
}