using System;
using System.Collections.Generic;

class Program
{
    static List<int>[] g;
    static int[] matchB;
    static bool[] used;

    static bool Dfs(int v)
    {
        foreach (int u in g[v])
        {
            if (used[u]) continue;
            used[u] = true;
            if (matchB[u] == 0 || Dfs(matchB[u]))
            {
                matchB[u] = v;
                return true;
            }
        }
        return false;
    }

    static void Main()
    {
        string[] nm = Console.ReadLine().Split();
        int n = int.Parse(nm[0]);
        int m = int.Parse(nm[1]);

        g = new List<int>[n + 1];
        for (int i = 1; i <= n; i++)
        {
            string[] parts = Console.ReadLine().Split();
            List<int> list = new List<int>();
            foreach (string p in parts)
            {
                int x = int.Parse(p);
                if (x == 0) break;
                list.Add(x);
            }
            g[i] = list;
        }

        matchB = new int[m + 1];
        for (int v = 1; v <= n; v++)
        {
            used = new bool[m + 1];
            Dfs(v);
        }

        List<(int, int)> result = new List<(int, int)>();
        for (int u = 1; u <= m; u++)
        {
            if (matchB[u] != 0)
                result.Add((matchB[u], u));
        }

        Console.WriteLine(result.Count);
        foreach (var p in result)
            Console.WriteLine($"{p.Item1} {p.Item2}");
    }
}
