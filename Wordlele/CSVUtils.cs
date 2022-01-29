namespace Wordlele
{
    internal class CSVUtils
    {
        internal static List<string> ReadWordList(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                List<string> wordList = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                    {
                        continue;
                    }
                    wordList.AddRange(line.Split(',').Select(x => x.Trim().ToUpperInvariant()));
                }
                return wordList;
            }
        }

        internal static void WriteResultsToFile(string fileName, IDictionary<string, double> results)
        {
            // Write in descending order of values
            var toWrite = results.Select(tp => tp).OrderBy((tp) => tp.Value);
            using (var writer = new StreamWriter(fileName))
            {
                foreach (var line in toWrite)
                {
                    writer.WriteLine($"{line.Key},{line.Value}");
                }
            }
        }
    }
}
