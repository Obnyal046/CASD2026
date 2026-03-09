using System;

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
        int period = n - pi[n - 1];
        if (n % period == 0)
            Console.WriteLine(period);
        else
            Console.WriteLine(n);
    }
}
