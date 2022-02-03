using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Wordlele;

public class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<WordleBench>();
        //BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
    }
}

[MemoryDiagnoser]
public class WordleBench
{
    List<string> Valids;
    int GuessWordIdx;
    int WordIdx;

    // Not working yet
    List<IWordleSolver> Solvers = new List<IWordleSolver>();
    List<Wordle> Wordles = new List<Wordle>();

    // Not working yet
    [GlobalSetup]
    public void Setup()
    {
        // Setup game
        var rand = new Random(12345678);
        Valids = new List<string>(WordSets.GuessList);
        Valids.AddRange(WordSets.WordList);
        WordIdx = rand.Next(WordSets.WordList.Length);
        GuessWordIdx = rand.Next(Valids.Count);

        // For all the different solvers, populate and request the first guess
        Solvers.Add(new PrunerRandom(WordSets.WordList, WordSets.GuessList, false, forcedFirstWord: WordSets.GuessList[GuessWordIdx], randOverride: new Random(123)));
        //Solvers.Add(new PrunerRandom2(WordSets.WordList, WordSets.GuessList, false, forcedFirstWord: valids[guessWordIdx], randOverride: new Random(123)));

        // Create all the wordles so the benchmarks don't step on each other
        Wordles.Add(new Wordle(Valids[WordIdx], Valids));
    }

    [IterationSetup]
    public void SetupBenches()
    {
        for (int i = 0; i < Solvers.Count; i++)
        {
            Solvers[i].Reset();
            Wordles[i].Reset();
        }
    }

    [Benchmark]
    public int BenchmarkPrunerRandomSingleGuess()
    {
        // Initial guess which is always the forced word - Should be a consistent time for every bench
        // ------------------------
        Wordles[0].TryGuess(Solvers[0].GenerateGuess(), out var firstResult);
        // ------------------------
        // Under Test:
        return Wordles[0].UnsafeGuess(Solvers[0].GenerateGuess(firstResult)).Length;
    }

    [Benchmark]
    public int BenchmarkPrunerRandomGame()
    {
        // Under Test:
        return Wordles[0].PlayGame(Solvers[0], Unsafe: true);
    }
}