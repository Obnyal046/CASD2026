using System;
using System.Text;

class Program
{
    static void Main()
    {
        string p = Console.ReadLine();
        string t = Console.ReadLine();

        int n = p.Length;
        int m = t.Length;

        int[] pi = new int[n];
        for (int i = 1; i < n; i++)
        {
            int j = pi[i - 1];
            while (j > 0 && p[i] != p[j])
                j = pi[j - 1];
            if (p[i] == p[j])
                j++;
            pi[i] = j;
        }
        StringBuilder positions = new StringBuilder();
        int count = 0;
        int cur = 0;

        for (int i = 0; i < m; i++)
        {
            while (cur > 0 && (cur >= n || p[cur] != t[i]))
                cur = pi[cur - 1];
            if (cur < n && p[cur] == t[i])
                cur++;
            if (cur == n)
            {
                count++;
                if (positions.Length > 0)
                    positions.Append(' ');
                positions.Append(i - n + 2);
                cur = pi[cur - 1];
            }
        }

        Console.WriteLine(count);
        Console.WriteLine(positions.ToString());
    }
}
