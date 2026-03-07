using System;
using System.Text;

class Program
{
    static void Main()
    {
        string s = Console.ReadLine();
        int n = s.Length;
        int m = int.Parse(Console.ReadLine());

        const long baseVal = 257;
        const long mod1 = 1000000007;
        const long mod2 = 1000000009;

        long[] pow1 = new long[n + 1];
        long[] pow2 = new long[n + 1];
        pow1[0] = pow2[0] = 1;
        for (int i = 1; i <= n; i++)
        {
            pow1[i] = (pow1[i - 1] * baseVal) % mod1;
            pow2[i] = (pow2[i - 1] * baseVal) % mod2;
        }

        long[] h1 = new long[n + 1];
        long[] h2 = new long[n + 1];
        for (int i = 1; i <= n; i++)
        {
            int ch = s[i - 1]; // используем код символа
            h1[i] = (h1[i - 1] * baseVal + ch) % mod1;
            h2[i] = (h2[i - 1] * baseVal + ch) % mod2;
        }

        StringBuilder sb = new StringBuilder();
        for (int q = 0; q < m; q++)
        {
            string[] parts = Console.ReadLine().Split();
            int a = int.Parse(parts[0]);
            int b = int.Parse(parts[1]);
            int c = int.Parse(parts[2]);
            int d = int.Parse(parts[3]);

            int len1 = b - a + 1;
            int len2 = d - c + 1;
            if (len1 != len2)
            {
                sb.AppendLine("No");
                continue;
            }

            int len = len1;

            long hashA1 = (h1[b] - (h1[a - 1] * pow1[len]) % mod1 + mod1) % mod1;
            long hashA2 = (h2[b] - (h2[a - 1] * pow2[len]) % mod2 + mod2) % mod2;
            long hashB1 = (h1[d] - (h1[c - 1] * pow1[len]) % mod1 + mod1) % mod1;
            long hashB2 = (h2[d] - (h2[c - 1] * pow2[len]) % mod2 + mod2) % mod2;

            if (hashA1 == hashB1 && hashA2 == hashB2)
                sb.AppendLine("Yes");
            else
                sb.AppendLine("No");
        }

        Console.Write(sb.ToString());
    }
}
