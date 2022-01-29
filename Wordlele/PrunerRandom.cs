using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Wordlele
{
    internal class PrunerRandom : IWordleSolver
    {
        private IReadOnlyList<string> Guessable;

        private int LastGuess;
        private List<int> ValidIndexes;

        private bool EnableLogging = false;
        private static Random Random = new Random(789);

        internal PrunerRandom(IReadOnlyList<string> wordList, IReadOnlyList<string> guessList, bool enableLogging, string forcedFirstWord = null)
        {
            var tmp = wordList.Union(guessList).Distinct().ToList();
            Guessable = tmp;
            this.EnableLogging = enableLogging;
            Reset();
            if (forcedFirstWord != null)
            {
                LastGuess = tmp.FindIndex((word) => word == forcedFirstWord);
                if (LastGuess < 0)
                {
                    throw new ArgumentException($"Forced first guess isn't in list. Guess: {forcedFirstWord}");
                }
            }
        }

        public string GenerateGuess()
        {
            return Guessable[ValidIndexes[LastGuess]];
        }

        public string GenerateGuess(int[] lastState)
        {
            // Remove previous guess from consideration
            string prevGuess = Guessable[ValidIndexes[LastGuess]];
            ValidIndexes.RemoveAt(LastGuess);

            // Prune all the words containing a letter not in the final word
            PruneAllNotContainingMissingLetters(prevGuess, lastState);

            // Prune all the words that don't contain a letter in the correct spot in the final word
            PruneAllNotContainingGuaranteedSpots(prevGuess, lastState);

            // Prune all the words that don't contain a letter guaranteed in the final word
            PruneAllNotContainingGuaranteedLetters(prevGuess, lastState);

            LastGuess = Random.Next(ValidIndexes.Count);
            Log($"  Remaining possibilities: {ValidIndexes.Count}");
            return Guessable[ValidIndexes[LastGuess]];
        }

        public string GenerateGuess(List<int[]> lastStates, List<string> previouesGuesses)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            ValidIndexes = Enumerable.Range(0, Guessable.Count).ToList();
            LastGuess = Random.Next(ValidIndexes.Count);
        }

        private void PruneAllNotContainingMissingLetters(string prevGuess, int[] lastState)
        {
            sbyte[] CharSet = new sbyte[26];
            // Count the number of each letter
            // Any character appearing at least once will have value > 0 or >-10 < 0
            for (int i = 0; i < lastState.Length; i++)
            {
                var idx = prevGuess[i] - 65;
                if (lastState[i] > 0)
                {
                    CharSet[idx] = 1;
                }
                else
                {
                    CharSet[idx] = CharSet[idx] > 0 ? CharSet[idx] : (sbyte)-1;
                }
            }
            List<char> elims = new List<char>();
            for(int i = 0;i < CharSet.Length;i++)
            {
                sbyte ch = CharSet[i];
                if (ch == -1)
                {
                    elims.Add((char)(i + 65));
                }
            }

            for (int idx = 0; idx < ValidIndexes.Count; idx++)
            {
                string potential = Guessable[ValidIndexes[idx]];
                Log("\t[0]Current word:" + potential);
                foreach (char c in elims)
                {
                    if (potential.Contains(c))
                    {
                        ValidIndexes.RemoveAt(idx);
                        idx--;
                        Log("\t\t[0]Removed");
                        break;
                    }
                }
            }
        }

        private void PruneAllNotContainingGuaranteedLetters(string prevGuess, int[] lastState)
        {
            sbyte[] CharSet = new sbyte[26];
            // Count the number of each letter
            for (int i = 0; i < lastState.Length; i++)
            {
                var idx = prevGuess[i] - 65;
                if (lastState[i] > 0)
                {
                    CharSet[idx]++;
                }
            }
            // Set of known letters
            List<char> mustHaves = new List<char>();
            for (int i = 0; i < CharSet.Length; i++)
            {
                sbyte ch = CharSet[i];
                if (ch > 0)
                {
                    mustHaves.Add((char)(i + 65));
                }
            }

            // For every possible word, count number of each letter
            // and ensure it's at least equal or higher to count in last guess
            sbyte[] currentSet = new sbyte[26];
            for (int idx = 0; idx < ValidIndexes.Count; idx++)
            {
                string potential = Guessable[ValidIndexes[idx]];
                Log("\t[2]Current word:" + potential);
                for(int i = 0; i<potential.Length; i++)
                {
                    currentSet[potential[i] - 65]++;
                }
                foreach (char c in mustHaves)
                {
                    if (currentSet[(int)c - 65] < CharSet[(int)c - 65])
                    {
                        ValidIndexes.RemoveAt(idx);
                        idx--;
                        Log("\t\t[2]Removed");
                        break;
                    }
                }
                Array.Fill<sbyte>(currentSet, 0);
            }
        }

        private void PruneAllNotContainingGuaranteedSpots(string prevGuess, int[] lastState)
        {
            for (int idx = 0; idx < ValidIndexes.Count; idx++)
            {
                string potentialWord = Guessable[ValidIndexes[idx]];
                Log("\t[1]Current word:" + potentialWord);
                for (int i = 0; i < lastState.Length; i++)
                {
                    if (lastState[i] == 2 && potentialWord[i] != prevGuess[i])
                    {
                        ValidIndexes.RemoveAt(idx);
                        idx--;
                        Log("\t\t[1]Removed");
                        break;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Log(string msg)
        {
            if (!EnableLogging) return;
            Console.WriteLine(msg);
        }
    }
}
