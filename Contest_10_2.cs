using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    static void Main()
    {
        int k = int.Parse(Console.ReadLine());
        for (int test = 0; test < k; test++)
        {
            string[] mn = Console.ReadLine().Split();
            int m = int.Parse(mn[0]);
            int n = int.Parse(mn[1]);

            bool[,] known = new bool[m + 1, n + 1];

            for (int i = 1; i <= m; i++)
            {
                string line = Console.ReadLine();
                string[] parts = line.Split();
                foreach (string p in parts)
                {
                    int x = int.Parse(p);
                    if (x == 0) break;
                    known[i, x] = true;
                }
            }

            List<int>[] g = new List<int>[m + 1];
            for (int i = 1; i <= m; i++)
                g[i] = new List<int>();

            for (int i = 1; i <= m; i++)
                for (int j = 1; j <= n; j++)
                    if (!known[i, j])
                        g[i].Add(j);

            int[] matchR = new int[n + 1];
            bool[] used;

            bool Dfs(int v)
            {
                foreach (int u in g[v])
                {
                    if (used[u]) continue;
                    used[u] = true;
                    if (matchR[u] == 0 || Dfs(matchR[u]))
                    {
                        matchR[u] = v;
                        return true;
                    }
                }
                return false;
            }

            for (int i = 1; i <= m; i++)
            {
                used = new bool[n + 1];
                Dfs(i);
            }

            bool[] leftMatched = new bool[m + 1];
            for (int j = 1; j <= n; j++)
                if (matchR[j] != 0)
                    leftMatched[matchR[j]] = true;

            bool[] visL = new bool[m + 1];
            bool[] visR = new bool[n + 1];

            void DfsCover(int u)
            {
                visL[u] = true;
                foreach (int v in g[u])
                {
                    if (matchR[v] != u && !visR[v])
                    {
                        visR[v] = true;
                        if (matchR[v] != 0)
                            DfsCover(matchR[v]);
                    }
                }
            }

            for (int i = 1; i <= m; i++)
                if (!leftMatched[i])
                    DfsCover(i);

            List<int> boys = new List<int>();
            for (int i = 1; i <= m; i++)
                if (visL[i]) boys.Add(i);

            List<int> girls = new List<int>();
            for (int j = 1; j <= n; j++)
                if (!visR[j]) girls.Add(j);

            Console.WriteLine(boys.Count + girls.Count);
            Console.WriteLine(boys.Count + " " + girls.Count);
            Console.WriteLine(string.Join(" ", boys));
            Console.WriteLine(string.Join(" ", girls));
            if (test < k - 1)
                Console.WriteLine();
        }
    }
}
