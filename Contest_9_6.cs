using System;
using System.Collections.Generic;

class Program
{
    const long MOD1 = 1000000007;
    const long MOD2 = 1000000009;
    const long BASE = 91138233;

    static void Main()
    {
        int k = int.Parse(Console.ReadLine());
        string[] strings = new string[k];
        int minLen = int.MaxValue;
        for (int i = 0; i < k; i++)
        {
            strings[i] = Console.ReadLine();
            if (strings[i].Length < minLen)
                minLen = strings[i].Length;
        }

        if (k == 0 || minLen == 0)
        {
            Console.WriteLine();
            return;
        }

        long[] pow1 = new long[minLen + 1];
        long[] pow2 = new long[minLen + 1];
        pow1[0] = pow2[0] = 1;
        for (int i = 1; i <= minLen; i++)
        {
            pow1[i] = (pow1[i - 1] * BASE) % MOD1;
            pow2[i] = (pow2[i - 1] * BASE) % MOD2;
        }

        long[][][] pref = new long[k][][];
        for (int i = 0; i < k; i++)
        {
            string s = strings[i];
            int len = s.Length;
            pref[i] = new long[2][];
            pref[i][0] = new long[len + 1];
            pref[i][1] = new long[len + 1];
            for (int j = 0; j < len; j++)
            {
                int ch = s[j];
                pref[i][0][j + 1] = (pref[i][0][j] * BASE + ch) % MOD1;
                pref[i][1][j + 1] = (pref[i][1][j] * BASE + ch) % MOD2;
            }
        }

        Func<int, int, int, Tuple<long, long>> getHash = (idx, l, r) =>
        {
            int len = r - l + 1;
            long h1 = (pref[idx][0][r + 1] - pref[idx][0][l] * pow1[len]) % MOD1;
            if (h1 < 0) h1 += MOD1;
            long h2 = (pref[idx][1][r + 1] - pref[idx][1][l] * pow2[len]) % MOD2;
            if (h2 < 0) h2 += MOD2;
            return Tuple.Create(h1, h2);
        };

        int left = 0, right = minLen;
        string answer = "";

        while (left < right)
        {
            int mid = (left + right + 1) / 2;

            var current = new HashSet<Tuple<long, long>>();
            var firstPos = new Dictionary<Tuple<long, long>, int>();
            string first = strings[0];
            for (int i = 0; i + mid <= first.Length; i++)
            {
                var h = getHash(0, i, i + mid - 1);
                current.Add(h);
                if (!firstPos.ContainsKey(h))
                    firstPos[h] = i;
            }

            bool found = true;
            for (int j = 1; j < k; j++)
            {
                var next = new HashSet<Tuple<long, long>>();
                string cur = strings[j];
                for (int i = 0; i + mid <= cur.Length; i++)
                {
                    var h = getHash(j, i, i + mid - 1);
                    next.Add(h);
                }
                current.IntersectWith(next);
                if (current.Count == 0)
                {
                    found = false;
                    break;
                }
            }

            if (found)
            {
                foreach (var h in current)
                {
                    answer = first.Substring(firstPos[h], mid);
                    break;
                }
                left = mid;
            }
            else
            {
                right = mid - 1;
            }
        }

        Console.WriteLine(answer);
    }
}
