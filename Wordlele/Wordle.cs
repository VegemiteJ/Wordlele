using System.Runtime.CompilerServices;

namespace Wordlele
{
    public class Wordle
    {
        private string Word;
        private IReadOnlySet<string> ValidGuesses;
        private byte[] CharSet = new byte[26];

        private List<byte[]> LastResult = new List<byte[]>();

        public Wordle(string word, IReadOnlyList<string> validGuesses)
        {
            this.Word = word.Trim().ToUpperInvariant();
            foreach (char c in Word)
            {
                CharSet[(int)c - 65]++;
            }
            this.ValidGuesses = new HashSet<string>(validGuesses);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] UnsafeGuess(string guess)
        {
            var result = new byte[5];
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
            LastResult.Add(result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGuess(string guess, out byte[] result)
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
            if (guess.Any(c => !char.IsUpper(c) || (int)c < 65 || (int)c > 90))
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetLastResult()
        {
            if (LastResult.Count == 0)
            {
                throw new InvalidOperationException("No last result");
            }
            return LastResult.Last();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            LastResult.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PlayGame(IWordleSolver solver, bool Unsafe = false)
        {
            string guess = solver.GenerateGuess();
            int guessCnt = 1;
            while (true)
            {
                byte[] result;
                if (Unsafe)
                {
                    result = UnsafeGuess(guess);
                }
                else if (!TryGuess(guess, out result))
                {
                    throw new InvalidOperationException("Not a word");
                }

                // This way is probably slightly faster than All((b) => b == 2))
                if (result[0] == 2
                    && result[1] == 2
                    && result[2] == 2
                    && result[3] == 2
                    && result[4] == 2)
                {
                    break;
                }
                else
                {
                    guess = solver.GenerateGuess(result);
                    guessCnt++;
                }
            }
            return guessCnt;
        }
    }
}
