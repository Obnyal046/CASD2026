using System;
using System.Text;
 
class Program
{
    static void Main()
    {
        string s = Console.ReadLine();
        int n = s.Length;
        int[] pi = new int[n];
        for (int i = 1; i < n; i++)
        {
            int j = pi[i - 1];
            while (j > 0 && s[i] != s[j])
                j = pi[j - 1];
            if (s[i] == s[j])
                j++;
            pi[i] = j;
        }
        
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < n; i++)
        {
            sb.Append(pi[i]);
            if (i < n - 1) sb.Append(' ');
        }
        Console.WriteLine(sb.ToString());
    }
}
