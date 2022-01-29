using System.Diagnostics;
using System.Runtime.CompilerServices;
using Wordlele;

namespace Wordlele
{
    public class Program
    {
        private static bool EnableLogging = true;
        private static Random Random = new Random(567);

        public static void Main(string[] args)
        {
            //IReadOnlyList<string> wordList = CSVUtils.ReadWordList(@"C:\Users\baxte\source\repos\Wordlele\ShortWordList.csv");
            //IReadOnlyList<string> guessList = CSVUtils.ReadWordList(@"C:\Users\baxte\source\repos\Wordlele\ShortGuessList.csv");

            IReadOnlyList<string> wordList = CSVUtils.ReadWordList(@"C:\Users\baxte\source\repos\Wordlele\WordList.csv");
            IReadOnlyList<string> guessList = CSVUtils.ReadWordList(@"C:\Users\baxte\source\repos\Wordlele\GuessList.csv");

            DebugSolver(new PrunerRandom(wordList, wordList, EnableLogging/*, "KEBAB"*/), wordList, guessList);

            //PlayHuman(wordList, guessList);

            EnableLogging = false;
            //DetermineBestStartingWord(wordList, guessList, @"C:\Users\baxte\source\repos\Wordlele\Results.csv");
            SimulateNGames(1000, wordList, wordList);
        }


        private static void DetermineBestStartingWord(IReadOnlyList<string> wordList, IReadOnlyList<string> guessList, string resultFile)
        {
            List<string> validList = new List<string>(wordList);
            validList.AddRange(guessList);
            // For every first word as a guess
            //      Simulate a game using every word as a final
            //      Track the average number of guesses
            var results = new Dictionary<string, double>(guessList.Count);
            foreach(string guess in guessList)
            {
                Console.WriteLine($"Simming Guess {guess}...");
                int N = wordList.Count;
                Stopwatch timer = Stopwatch.StartNew();
                List<int> guessCnts = new List<int>(N);
                foreach (string word in wordList)
                {
                    Wordle wordle = new Wordle(word, validList);
                    guessCnts.Add(PlaySolver(wordle, new PrunerRandom(wordList, guessList, false, guess)));
                }
                timer.Stop();
                double avgGuesses = guessCnts.Average();
                results.Add(guess, avgGuesses);
                Console.WriteLine($"Played {N} games for avg of {avgGuesses} in {timer.ElapsedMilliseconds}ms");
            }

            CSVUtils.WriteResultsToFile(resultFile, results);
        }

        private static void DebugSolver(IWordleSolver solver,
            IReadOnlyList<string> wordList, IReadOnlyList<string> guessList)
        {
            Log("Enter word or hit enter for random word:");
            string word = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(word))
            {
                word = wordList[Random.Next(wordList.Count)];
                Log($"Debug word is:{word}");
            }
            List<string> validList = new List<string>(wordList);
            validList.AddRange(guessList);
            var wordle = new Wordle(word, validList);
            PlaySolver(wordle, solver);
        }

        private static void SimulateNGames(int N, IReadOnlyList<string> wordList, IReadOnlyList<string> guessList)
        {
            Console.WriteLine("Starting sim...");
            Stopwatch timer = Stopwatch.StartNew();
            List<int> guessCnts = new List<int>(N);
            for (int i = 0; i < N; i++)
            {
                string word = wordList[Random.Next(wordList.Count)];
                Console.Write(word + "|");
                List<string> validList = new List<string>(wordList);
                validList.AddRange(guessList);
                Wordle wordle = new Wordle(word, validList);
                guessCnts.Add(PlaySolver(wordle, new PrunerRandom(wordList, guessList, false)));
            }
            timer.Stop();
            double avgGuesses = guessCnts.Average();
            Console.WriteLine($"Played {N} games for avg of {avgGuesses} in {timer.ElapsedMilliseconds}ms");
        }

        private static int PlaySolver(Wordle wordle, IWordleSolver solver)
        {
            Log("First Guess:");
            string guess = solver.GenerateGuess();
            int guessCnt = 1;
            Log(guess);
            while (true)
            {
                if (wordle.TryGuess(guess?.ToUpperInvariant(), out int[] result))
                {
                    string d = $"{string.Join("", result)}";
                    Log(d);
                    if (d == "22222")
                    {
                        break;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Not a word");
                }
                guess = solver.GenerateGuess(result);
                guessCnt++;
                Log("Guess:");
                Log(guess);
            }
            Log($"Solved puzzle in {guessCnt} guesses!");
            if (EnableLogging) Console.ReadKey();
            return guessCnt;
        }

        private static void PlayHuman(IReadOnlyList<string> wordList, IReadOnlyList<string> guessList)
        {
            string word = "GUMMY";// wordList[Random.Next(wordList.Count)];
            Log($"Debug word is:{word}");
            List<string> validList = new List<string>(wordList);
            validList.AddRange(guessList);
            var wordle = new Wordle(word, validList);
            int guessCnt = 0;
            while (true)
            {
                Console.WriteLine("Guess:");
                string guess = Console.ReadLine();
                if (wordle.TryGuess(guess?.ToUpperInvariant(), out int[] result))
                {
                    guessCnt++;
                    string d = $"{string.Join("", result)}";
                    Console.WriteLine(d);
                    if (d == "22222")
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Not a word");
                }
            }
            Log($"Solved puzzle in {guessCnt} guesses!");
            Console.ReadKey();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Log(string msg)
        {
            if (!EnableLogging) return;
            Console.WriteLine(msg);
        }
    }
}