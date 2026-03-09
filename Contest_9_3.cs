using System;
using System.Text;

class Program
{
    static void Main()
    {
        string s = Console.ReadLine();
        int n = s.Length;
        int[] z = new int[n];
        
        int l = 0, r = 0;
        for (int i = 1; i < n; i++)
        {
            if (i <= r)
                z[i] = Math.Min(r - i + 1, z[i - l]);
            while (i + z[i] < n && s[z[i]] == s[i + z[i]])
                z[i]++;
            if (i + z[i] - 1 > r)
            {
                l = i;
                r = i + z[i] - 1;
            }
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 1; i < n; i++)
        {
            sb.Append(z[i]);
            if (i < n - 1) sb.Append(' ');
        }
        Console.WriteLine(sb.ToString());
    }
}
