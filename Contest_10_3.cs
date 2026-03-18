using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        string[] first = Console.ReadLine().Split();
        int n = int.Parse(first[0]);
        int v = int.Parse(first[1]);

        var obs = new List<(double time, int x, int y)>();
        for (int i = 0; i < n; i++)
        {
            string line = Console.ReadLine();
            string[] parts = line.Split();
            string[] hm = parts[0].Split(':');
            int h = int.Parse(hm[0]);
            int m = int.Parse(hm[1]);
            double time = h + m / 60.0;
            int x = int.Parse(parts[1]);
            int y = int.Parse(parts[2]);
            obs.Add((time, x, y));
        }

        var sorted = obs.OrderBy(o => o.time).ToList();

        var graph = new List<int>[n];
        for (int i = 0; i < n; i++) graph[i] = new List<int>();

        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                double dt = sorted[j].time - sorted[i].time;
                double dx = sorted[j].x - sorted[i].x;
                double dy = sorted[j].y - sorted[i].y;
                double dist = Math.Sqrt(dx * dx + dy * dy);
                if (dist <= v * dt + 1e-9)
                {
                    graph[i].Add(j);
                }
            }
        }

        int[] matchR = new int[n];
        for (int i = 0; i < n; i++) matchR[i] = -1;

        bool[] used;

        bool Dfs(int u)
        {
            foreach (int v in graph[u])
            {
                if (used[v]) continue;
                used[v] = true;
                if (matchR[v] == -1 || Dfs(matchR[v]))
                {
                    matchR[v] = u;
                    return true;
                }
            }
            return false;
        }

        int matching = 0;
        for (int i = 0; i < n; i++)
        {
            used = new bool[n];
            if (Dfs(i)) matching++;
        }

        int result = n - matching;
        Console.WriteLine(result);
    }
}
