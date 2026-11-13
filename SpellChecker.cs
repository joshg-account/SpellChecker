using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace SpellChecker;

public class Program
{
    public static char[,] qwertyKeyboard;

    public static void Main(string[] args)
    {
        qwertyKeyboard = BuildQwertyKeyboard();
        Dictionary<string, int> EnglishWords = new Dictionary<string, int>();
        string EnglishWordsAddress = Path.Combine(Directory.GetCurrentDirectory(),"words_alpha.txt");
        foreach(string line in File.ReadLines(EnglishWordsAddress)) 
        {
            EnglishWords.Add(line, 0);
        }
        Console.WriteLine("Uploaded English Words to dictionairy");
        string checking = "hellp".ToLower();
        if (EnglishWords.ContainsKey(checking)) Console.WriteLine("'" + checking + "' is already spelled correctly!");
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
            List<string> closeWords = new List<string>();
            foreach (var value in EnglishWords.OrderBy(pair => pair.Value))
            {
                if (value.Value != minVal)
                {
                    break;
                }
                else
                {
                    closeWords.Add(value.Key);
                }
            }
            foreach(string word in closeWords)
            {
                Console.WriteLine(word);    
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
                    (int col, int row)? keyPos = null;
                    for (int row = 0; row < 4; row++)
                    {
                        for (int column = 0; column < 11; column++)
                        { 
                            if (qwertyKeyboard[column, row] == a[i - 1]) keyPos = (column, row);
                        }
                    }
                    if (keyPos != null)
                    {
                        int c = keyPos.Value.col;
                            int r = keyPos.Value.row;
                            bool neighbour = 
                            (c > 0 && qwertyKeyboard[c-1, r] == b[j - 1]) || 
                            (c > 0 && r < 3 && qwertyKeyboard[c-1, r + 1] == b[j - 1]) || 
                            (c < 10 && qwertyKeyboard[c+1, r] == b[j - 1]) ||
                            (c < 10 && r > 0 && qwertyKeyboard[c+1, r - 1] == b[j - 1]) || 
                            (r > 0 && qwertyKeyboard[c, r-1] == b[j - 1]) || 
                            (r < 3 && qwertyKeyboard[c, r+1] == b[j - 1]);

                            substitutionCost = neighbour ? 1 : 2;
                    }
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
            int rowMin = int.MaxValue;
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
        string[] rows = {"1234567890-", "qwertyuiop[", "asdfghjkl;'", @"zxcvbnm,.//" };

        for (int row = 0; row < 4; row++)
        {
            for (int column = 0; column < 11; column++)
            {
                keyboard[column, row] = rows[row][column];
            }
        }

        return keyboard;
    }
}
