using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main()
    {
        using (var reader = new StreamReader(Console.OpenStandardInput()))
        using (var writer = new StreamWriter(Console.OpenStandardOutput()))
        {
            int n = int.Parse(reader.ReadLine());
            var patterns = new List<string>(n);
            for (int i = 0; i < n; i++)
                patterns.Add(reader.ReadLine());
            string t = reader.ReadLine();

            int[] sa = BuildSuffixArray(t);
            int m = t.Length;

            int[] log = new int[m + 1];
            for (int i = 2; i <= m; i++)
                log[i] = log[i / 2] + 1;
            int logn = log[m];

            int[][] stMin = new int[logn + 1][];
            stMin[0] = sa;
            for (int k = 1; k <= logn; k++)
            {
                int len = m - (1 << k) + 1;
                stMin[k] = new int[len];
                int step = 1 << (k - 1);
                for (int i = 0; i < len; i++)
                    stMin[k][i] = Math.Min(stMin[k - 1][i], stMin[k - 1][i + step]);
            }
            int[][] stMax = new int[logn + 1][];
            stMax[0] = sa;
            for (int k = 1; k <= logn; k++)
            {
                int len = m - (1 << k) + 1;
                stMax[k] = new int[len];
                int step = 1 << (k - 1);
                for (int i = 0; i < len; i++)
                    stMax[k][i] = Math.Max(stMax[k - 1][i], stMax[k - 1][i + step]);
            }
            int QueryMin(int l, int r)
            {
                int k = log[r - l + 1];
                return Math.Min(stMin[k][l], stMin[k][r - (1 << k) + 1]);
            }
            int QueryMax(int l, int r)
            {
                int k = log[r - l + 1];
                return Math.Max(stMax[k][l], stMax[k][r - (1 << k) + 1]);
            }

            int ComparePattern(string s, int pos, string pat)
            {
                return string.Compare(s, pos, pat, 0, pat.Length, StringComparison.Ordinal);
            }

            int CompareUpper(string s, int pos, string pat)
            {
                int cmp = string.Compare(s, pos, pat, 0, pat.Length, StringComparison.Ordinal);
                if (cmp != 0) return cmp;
                if (pos + pat.Length < s.Length)
                {
                    char next = s[pos + pat.Length];
                    return next.CompareTo('{'); // '{' > 'z'
                }
                else
                {
                    return -1;
                }
            }


            int LowerBound(int[] array, Func<int, int> compare)
            {
                int lo = 0, hi = array.Length;
                while (lo < hi)
                {
                    int mid = (lo + hi) / 2;
                    int pos = array[mid];
                    if (compare(pos) >= 0)
                        hi = mid;
                    else
                        lo = mid + 1;
                }
                return lo;
            }

            foreach (string pat in patterns)
            {
                int L = LowerBound(sa, pos => ComparePattern(t, pos, pat));
                int R = LowerBound(sa, pos => CompareUpper(t, pos, pat));
                if (L < R)
                {
                    int leftmost = QueryMin(L, R - 1);
                    int rightmost = QueryMax(L, R - 1);
                    writer.WriteLine(leftmost + " " + rightmost);
                }
                else
                {
                    writer.WriteLine("-1 -1");
                }
            }
        }
    }

    static int[] BuildSuffixArray(string s)
    {
        int n = s.Length;
        int[] sa = new int[n];
        int[] rank = new int[n];
        int[] tmp = new int[n];
        for (int i = 0; i < n; i++)
        {
            sa[i] = i;
            rank[i] = s[i];
        }
        for (int k = 1; k < n; k <<= 1)
        {
            int[] second = new int[n];
            for (int i = 0; i < n; i++)
            {
                int pos = sa[i] + k;
                second[i] = pos < n ? rank[pos] + 1 : 0;
            }
            int maxSecond = 0;
            for (int i = 0; i < n; i++)
                if (second[i] > maxSecond) maxSecond = second[i];
            int[] cnt = new int[maxSecond + 2];
            for (int i = 0; i < n; i++)
                cnt[second[i]]++;
            for (int i = 1; i < cnt.Length; i++)
                cnt[i] += cnt[i - 1];
            int[] newSa = new int[n];
            for (int i = n - 1; i >= 0; i--)
            {
                int idx = --cnt[second[i]];
                newSa[idx] = sa[i];
            }

            int[] first = new int[n];
            for (int i = 0; i < n; i++)
                first[i] = rank[newSa[i]] + 1;
            int maxFirst = 0;
            for (int i = 0; i < n; i++)
                if (first[i] > maxFirst) maxFirst = first[i];
            cnt = new int[maxFirst + 2];
            for (int i = 0; i < n; i++)
                cnt[first[i]]++;
            for (int i = 1; i < cnt.Length; i++)
                cnt[i] += cnt[i - 1];
            int[] newSa2 = new int[n];
            for (int i = n - 1; i >= 0; i--)
            {
                int idx = --cnt[first[i]];
                newSa2[idx] = newSa[i];
            }
            sa = newSa2;

            tmp[sa[0]] = 0;
            for (int i = 1; i < n; i++)
            {
                int cur1 = rank[sa[i]];
                int cur2 = (sa[i] + k < n) ? rank[sa[i] + k] : -1;
                int prev1 = rank[sa[i - 1]];
                int prev2 = (sa[i - 1] + k < n) ? rank[sa[i - 1] + k] : -1;
                if (cur1 == prev1 && cur2 == prev2)
                    tmp[sa[i]] = tmp[sa[i - 1]];
                else
                    tmp[sa[i]] = tmp[sa[i - 1]] + 1;
            }
            Array.Copy(tmp, rank, n);
            if (rank[sa[n - 1]] == n - 1) break;
        }
        return sa;
    }

    static int[] BuildLCP(string s, int[] sa)
    {
        int n = s.Length;
        int[] rank = new int[n];
        for (int i = 0; i < n; i++)
            rank[sa[i]] = i;
        int[] lcp = new int[n - 1];
        int h = 0;
        for (int i = 0; i < n; i++)
        {
            if (rank[i] == n - 1)
            {
                h = 0;
                continue;
            }
            int j = sa[rank[i] + 1];
            while (i + h < n && j + h < n && s[i + h] == s[j + h])
                h++;
            lcp[rank[i]] = h;
            if (h > 0) h--;
        }
        return lcp;
    }
}
