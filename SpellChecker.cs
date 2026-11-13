namespace SpellChecker;

public class Program
{
    public static void Main(string[] args)
    {
        Dictionary<string, int> EnglishWords = new Dictionary<string, int>();
        string EnglishWordsAddress = "C:/Users/joshg/C Sharp/words_alpha.txt";
        foreach(string line in File.ReadLines(EnglishWordsAddress)) 
        {
            EnglishWords.Add(line, 0);
        }
        Console.WriteLine("Uploaded English Words to dictionairy");
        string checking = "hello";
        foreach (string alt in EnglishWords.Keys)
        {
            if (alt.Length == checking.Length)
            {
                EnglishWords.Conta
            }
        }
    }
}
