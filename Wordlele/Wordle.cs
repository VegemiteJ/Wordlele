#define SAFETY
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordlele
{
    internal class Wordle
    {
        private string Word;
        private IReadOnlySet<string> ValidGuesses;

        private byte[] CharSet = new byte[26];

        internal Wordle(string word, IReadOnlyList<string> validGuesses)
        {
            this.Word = word.Trim().ToUpperInvariant();
            foreach (char c in Word)
            {
                CharSet[(int)c - 65]++;
            }
            this.ValidGuesses = new HashSet<string>(validGuesses);
        }

        public int[] UnsafeGuess(string guess)
        {
            var result = new int[5];
            byte[] guessCharSet = new byte[26];
            // First fill in all the spots where it is exact
            for (int i = 0; i < guess.Length; i++)
            {
                char c = guess[i];
                // If there is a letter at exactly this spot
                if (c == Word[i])
                {
                    result[i] = 2;
                    guessCharSet[(int)c - 65]++;
                }
            }
            for (int i = 0; i < guess.Length; i++)
            {
                char c = guess[i];
                if (c == Word[i])
                {
                    continue;
                }
                // Else if word contains this letter,
                // and output does not already reflect this for each occurrence
                // Then set to 1
                else if (CharSet[(int)c - 65] > guessCharSet[(int)c - 65]++)
                {
                    result[i] = 1;
                }
                else
                {
                    result[i] = 0;
                }
            }
            return result;
        }

        public bool TryGuess(string guess, out int[] result)
        {
            result = null;
            if (guess == null)
            {
                return false;   
            }
            if (guess.Length != this.Word.Length)
            {
                return false;
            }
            if (guess.Any(c => !Char.IsUpper(c) || (int)c < 65 || (int)c > 90))
            {
                return false;
            }
            if(!ValidGuesses.Contains(guess))
            {
                return false;
            }

            result = UnsafeGuess(guess);
            return true;
        }
    }
}
