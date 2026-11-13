using System.ComponentModel.DataAnnotations.Schema;
using System.Formats.Asn1;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace SpellChecker;

public class Program
{
    public static char[,] qwertyKeyboard;
    public static List<string> MostCommonEnglishWords;
    public static HashSet<(char, char)> substitutionCosts;

    public static void Main(string[] args)
    {
        qwertyKeyboard = BuildQwertyKeyboard();
        substitutionCosts = BuildSubstitutionCosts();
        MostCommonEnglishWords = new List<string>();
        string MostCommonEnglishWordsAddress = Path.Combine(Directory.GetCurrentDirectory(),"1000-most-common-words.txt");
        foreach(string line in File.ReadLines(MostCommonEnglishWordsAddress)) 
        {
                if (!string.IsNullOrWhiteSpace(line))
                    MostCommonEnglishWords.Add(line.Trim().ToLower());
        }
        Dictionary<string, int> EnglishWords = new Dictionary<string, int>();
        string EnglishWordsAddress = Path.Combine(Directory.GetCurrentDirectory(),"words_alpha.txt");
        foreach(string line in File.ReadLines(EnglishWordsAddress)) 
        {
            if (line != "") EnglishWords.Add(line.Trim().ToLower(), 0);
        }
        Console.WriteLine("Uploaded English Words to dictionary");
        Console.Write("What word would you like to test?\n>> ");
        string checking = Console.ReadLine().Trim().ToLower();
        if (checking == "") Console.WriteLine("Do not enter empty space.");
        else if (EnglishWords.ContainsKey(checking)) Console.WriteLine("'" + checking + "' is already spelled correctly!");
        else
        {
            foreach (string alt in EnglishWords.Keys)
            {
                if (alt.Length <= checking.Length + 2 && alt.Length >= checking.Length - 2)
                {
                    EnglishWords[alt] = Distance(checking, alt);
                }
                else EnglishWords[alt] = 100;
            }
            var EnglishWordsAnswer = EnglishWords.MinBy(pair => pair.Value);
            int minVal = EnglishWordsAnswer.Value;
            if (minVal > 4) Console.WriteLine("No suggestions found.");
            else
            {
                List<string> closeWords = new List<string>();
                bool closeFreqWord = false;
                foreach (var value in EnglishWords.Where(pair => pair.Value == minVal))
                    if (MostCommonEnglishWords.Contains(value.Key)) closeFreqWord = true;
                
                foreach (var value in EnglishWords.Where(pair => pair.Value == minVal))
                {
                    if (!closeFreqWord || (closeFreqWord && MostCommonEnglishWords.Contains(value.Key)))
                    {
                        closeWords.Add(value.Key);
                        Console.WriteLine(value.Key);   
                    }
                }
            }  
        }
    }
    static int Distance(string a, string b)
    {
        int[,] table = new int[a.Length + 1, b.Length + 1];

        for (int i = 0; i <= a.Length; i++)
            table[i, 0] = i * 2;

        for (int j = 0; j <= b.Length; j++)
            table[0, j] = j * 2;

        int prevRowMin = 0;

        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                int substitutionCost = 0;
                if (a[i - 1] != b[j - 1]) 
                {
                    if (substitutionCosts.Contains((a[i-1], b[j-1]))) substitutionCost = 1;
                    else substitutionCost = 2;
                }

                int deletion = table[i - 1, j] + 2;
                int insertion = table[i, j - 1] + 2;
                int substitution = table[i - 1, j - 1] + substitutionCost;
                
                int best = Math.Min(deletion, Math.Min(insertion, substitution));

                if (i > 1 && j > 1 && a[i - 1] == b[j - 2] && a[i - 2] == b[j - 1])
                {
                    int transposition = table[i - 2, j - 2] + 1;
                    best = Math.Min(best, transposition);
                }
                table[i, j] = best;
            }
            int rowMin = int.MaxValue; // checks if 2 rows are both above the maximum, if they are then it returns 100 and stops building the grid
            for (int j = 0; j <= b.Length; j++)
                rowMin = Math.Min(rowMin, table[i, j]);
            
            if (rowMin > 4 && prevRowMin > 4) return 100;
            prevRowMin = rowMin;
        }
        return table[a.Length, b.Length];
    } 
    static char[,] BuildQwertyKeyboard()
    {
        char[,] keyboard = new char[11, 4];
        string[] rows = {"1234567890-", "qwertyuiop[", "asdfghjkl;'", @"zxcvbnm,./" };

        for (int row = 0; row < 4; row++)
        {
            for (int column = 0; column < rows[row].Length; column++)
            {
                keyboard[column, row] = rows[row][column];
            }
        }

        return keyboard;
    }
    static HashSet<(char,char)> BuildSubstitutionCosts()
    {
        HashSet<(char, char)> substitutionCosts = new HashSet<(char, char)>();
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 11; c++)
            {
                if (qwertyKeyboard[c,r] == '\0') continue;

                if (c > 0) substitutionCosts.Add((qwertyKeyboard[c,r],qwertyKeyboard[c-1,r]));
                if (c > 0 && r < 3) substitutionCosts.Add((qwertyKeyboard[c,r],qwertyKeyboard[c-1,r+1]));
                if (c < 10) substitutionCosts.Add((qwertyKeyboard[c,r],qwertyKeyboard[c+1,r]));
                if (c < 10 && r > 0) substitutionCosts.Add((qwertyKeyboard[c,r],qwertyKeyboard[c+1,r-1]));
                if (r > 0) substitutionCosts.Add((qwertyKeyboard[c,r],qwertyKeyboard[c,r-1]));
                if (r < 3) substitutionCosts.Add((qwertyKeyboard[c,r],qwertyKeyboard[c,r+1]));
            }
        }
        return substitutionCosts;
    }
}
