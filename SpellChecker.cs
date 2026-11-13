using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace SpellChecker;

public class Program
{
    public static void Main(string[] args)
    {
        Dictionary<string, int> EnglishWords = new Dictionary<string, int>();
        string EnglishWordsAddress = Path.Combine(Directory.GetCurrentDirectory(),"words_alpha.txt");
        foreach(string line in File.ReadLines(EnglishWordsAddress)) 
        {
            EnglishWords.Add(line, 0);
        }
        Console.WriteLine("Uploaded English Words to dictionairy");
        string checking = "hella";
        if (EnglishWords.ContainsKey(checking)) Console.WriteLine("'" + checking + "' is already spelled correctly!");
        else
        {
            foreach (string alt in EnglishWords.Keys)
            {
                if (alt.Length < checking.Length + 2 || alt.Length > checking.Length - 2)
                {
                    EnglishWords[alt] = Distance(checking, alt);
                }
                else EnglishWords[alt] = 100;
            }
            var EnglishWordsAnswer = EnglishWords.MinBy(pair => pair.Value);
            Console.WriteLine(EnglishWordsAnswer);    
        }
        
    }
    static int Distance(string a, string b)
    {
        int[,] table = new int[a.Length + 1, b.Length + 1];

        for (int i = 0; i <= a.Length; i++)
            table[i, 0] = i;

        for (int j = 0; j <= b.Length; j++)
            table[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                int substitutionCost = (a[i - 1] == b[j - 1]) ? 0 : 1;

                int deletion     = table[i - 1, j] + 1;
                int insertion    = table[i, j - 1] + 1;
                int substitution = table[i - 1, j - 1] + substitutionCost;
                
                table[i, j] = Math.Min(deletion, Math.Min(insertion, substitution));
            }
            bool tooHigh = true;
            for (int j = 1; j <= b.Length; j++)
                if (table[i, j] <= 2) tooHigh = false;
        }
        return table[a.Length, b.Length];
    } 
}
