using System;

class Program
{
    static void Main()
    {
        string s = Console.ReadLine();
        int n = s.Length;
        int[] sa = BuildSuffixArray(s);
        int[] lcp = BuildLCP(s, sa);
        long total = (long)n * (n + 1) / 2;
        long sumLcp = 0;
        for (int i = 0; i < n - 1; i++)
            sumLcp += lcp[i];
        long result = total - sumLcp;
        Console.WriteLine(result);
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
        for(int i=0; i < n; i++)
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
