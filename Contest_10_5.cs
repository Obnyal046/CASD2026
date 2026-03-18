using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        int n = int.Parse(Console.ReadLine());
        var horiz = new List<(int x1, int x2, int y)>();
        var vert = new List<(int x, int y1, int y2)>();

        for (int i = 0; i < n; i++)
        {
            string[] parts = Console.ReadLine().Split();
            int x1 = int.Parse(parts[0]);
            int y1 = int.Parse(parts[1]);
            int x2 = int.Parse(parts[2]);
            int y2 = int.Parse(parts[3]);

            if (x1 == x2)
            {
                int ymin = Math.Min(y1, y2);
                int ymax = Math.Max(y1, y2);
                vert.Add((x1, ymin, ymax));
            }
            else
            {
                int xmin = Math.Min(x1, x2);
                int xmax = Math.Max(x1, x2);
                horiz.Add((xmin, xmax, y1));
            }
        }

        int h = horiz.Count;
        int v = vert.Count;
        List<int>[] graph = new List<int>[h];
        for (int i = 0; i < h; i++) graph[i] = new List<int>();

        for (int i = 0; i < h; i++)
        {
            var (xmin, xmax, y) = horiz[i];
            for (int j = 0; j < v; j++)
            {
                var (x, ymin, ymax) = vert[j];
                if (x >= xmin && x <= xmax && y >= ymin && y <= ymax)
                    graph[i].Add(j);
            }
        }

        int[] matchR = new int[v];
        for (int j = 0; j < v; j++) matchR[j] = -1;

        bool Dfs(int u, bool[] used)
        {
            foreach (int w in graph[u])
            {
                if (used[w]) continue;
                used[w] = true;
                if (matchR[w] == -1 || Dfs(matchR[w], used))
                {
                    matchR[w] = u;
                    return true;
                }
            }
            return false;
        }

        int matching = 0;
        for (int i = 0; i < h; i++)
        {
            bool[] used = new bool[v];
            if (Dfs(i, used)) matching++;
        }

        Console.WriteLine(n - matching);
    }
}
