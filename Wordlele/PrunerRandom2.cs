using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Wordlele
{
    internal class PrunerRandom2 : IWordleSolver
    {
        private IReadOnlyList<string> Guessable;

        private int LastGuess;
        private List<int> ValidIndexes;

        private bool EnableLogging = false;

        internal PrunerRandom2(IReadOnlyList<string> wordList, IReadOnlyList<string> guessList, bool enableLogging)
        {
            List<string> union = new List<string>(wordList);
            union.AddRange(guessList);
            Guessable = union;
            this.EnableLogging = enableLogging;
            Reset();
        }

        public string GenerateGuess()
        {
            return Guessable[ValidIndexes[LastGuess]];
        }

        public string GenerateGuess(int[] lastState)
        {
            // Remove previous guess from consideration
            string prevGuess = Guessable[ValidIndexes[LastGuess]];
            List<int> newGuesses = new List<int>();

            // Prune all the words containing a letter not in the final word
            PruneAllNotContainingMissingLetters(prevGuess, lastState, newGuesses);

            // Prune all the words that don't contain a letter guaranteed in the final word
            PruneAllNotContainingGuaranteedLetters(prevGuess, lastState, newGuesses);

            // Prune all the words that don't contain a letter in the correct spot in the final word
            PruneAllNotContainingGuaranteedSpots(prevGuess, lastState, newGuesses);

            LastGuess = Random.Shared.Next(ValidIndexes.Count);
            Log($"  Remaining possibilities: {ValidIndexes.Count}");
            return Guessable[ValidIndexes[LastGuess]];
        }

        public string GenerateGuess(List<int[]> lastStates, List<string> previouesGuesses)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            ValidIndexes = new List<int>(Guessable.Count);
            LastGuess = Random.Shared.Next(Guessable.Count);
        }

        private void PruneAllNotContainingMissingLetters(string prevGuess, int[] lastState, List<int> newGuesses)
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

            for (int i = 0; i < ValidIndexes.Count; i++)
            {
                //Console.WriteLine("\tCurrent word:" + Guessable[ValidIndexes[i]]);
                foreach (char c in elims)
                {
                    string potential = Guessable[ValidIndexes[i]];
                    if (potential.Contains(c))
                    {
                        ValidIndexes.RemoveAt(i);
                        i--;
                        //Console.WriteLine("\t\tRemoved");
                        break;
                    }
                }
            }
        }

        private void PruneAllNotContainingGuaranteedSpots(string prevGuess, int[] lastState, List<int> newGuesses)
        {
            return;
        }

        private void PruneAllNotContainingGuaranteedLetters(string prevGuess, int[] lastState, List<int> newGuesses)
        {
            return;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Log(string msg)
        {
            if (!EnableLogging) return;
            Console.WriteLine(msg);
        }
    }
}
