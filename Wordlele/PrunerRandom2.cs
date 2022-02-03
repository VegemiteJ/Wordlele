using System.Runtime.CompilerServices;

namespace Wordlele
{
    public class PrunerRandom2 : IWordleSolver
    {
        private IReadOnlyList<string> Guessable;

        private int ForcedGuess = -1;
        private int LastGuess;

        private bool EnableLogging = false;
        private Random Random = new Random(789);

        public PrunerRandom2(IReadOnlyList<string> wordList, IReadOnlyList<string> guessList, bool enableLogging, string forcedFirstWord = null, Random randOverride = null)
        {
            var tmp = wordList.Union(guessList).Distinct().ToList();
            Guessable = tmp;
            this.EnableLogging = enableLogging;
            this.Random = randOverride ?? Random;
            Reset();
            if (forcedFirstWord != null)
            {
                ForcedGuess = tmp.FindIndex((word) => word == forcedFirstWord);
                if (ForcedGuess < 0)
                {
                    throw new ArgumentException($"Forced first guess isn't in list. Guess: {forcedFirstWord}");
                }
                LastGuess = ForcedGuess;
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public int GetPossibleGuessCount()
        {
            throw new NotImplementedException();
        }

        public string GenerateGuess()
        {
            throw new NotImplementedException();
        }

        public string GenerateGuess(byte[] lastState)
        {
            throw new NotImplementedException();
        }

        public string GenerateGuess(List<byte[]> lastStates, List<string> previouesGuesses)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Log(Func<string> msgGen)
        {
            if (!EnableLogging) return;
            Console.WriteLine(msgGen());
        }
    }
}
