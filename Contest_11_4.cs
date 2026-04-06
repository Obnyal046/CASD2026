using System;
using System.Collections.Generic;
 
class Program
{
    class Edge
    {
        public int To, Rev;
        public int Cap;
        public Edge(int to, int rev, int cap)
        {
            To = to;
            Rev = rev;
            Cap = cap;
        }
    }
 
    static List<Edge>[] g;
    static int[] level, iter;
 
    static void AddEdge(int from, int to, int cap)
    {
        g[from].Add(new Edge(to, g[to].Count, cap));
        g[to].Add(new Edge(from, g[from].Count - 1, 0));
    }
 
    static void Bfs(int s)
    {
        for (int i = 0; i < level.Length; i++) level[i] = -1;
        Queue<int> q = new Queue<int>();
        level[s] = 0;
        q.Enqueue(s);
        while (q.Count > 0)
        {
            int v = q.Dequeue();
            foreach (Edge e in g[v])
            {
                if (e.Cap > 0 && level[e.To] < 0)
                {
                    level[e.To] = level[v] + 1;
                    q.Enqueue(e.To);
                }
            }
        }
    }
 
    static int Dfs(int v, int t, int f)
    {
        if (v == t) return f;
        for (; iter[v] < g[v].Count; iter[v]++)
        {
            Edge e = g[v][iter[v]];
            if (e.Cap > 0 && level[v] < level[e.To])
            {
                int d = Dfs(e.To, t, Math.Min(f, e.Cap));
                if (d > 0)
                {
                    e.Cap -= d;
                    g[e.To][e.Rev].Cap += d;
                    return d;
                }
            }
        }
        return 0;
    }
 
    static int MaxFlow(int s, int t)
    {
        int flow = 0;
        level = new int[g.Length];
        iter = new int[g.Length];
        while (true)
        {
            Bfs(s);
            if (level[t] < 0) break;
            for (int i = 0; i < iter.Length; i++) iter[i] = 0;
            int f;
            while ((f = Dfs(s, t, int.MaxValue)) > 0)
                flow += f;
        }
        return flow;
    }
 
    static void Main()
    {
        int n = int.Parse(Console.ReadLine());
        char[,] table = new char[n + 1, n + 1];
        int[] cur = new int[n + 1];
 
        for (int i = 1; i <= n; i++)
        {
            string line = Console.ReadLine();
            for (int j = 1; j <= n; j++)
            {
                char c = line[j - 1];
                table[i, j] = c;
                if (i == j) continue;
                if (c == 'W') cur[i] += 3;
                else if (c == 'w') cur[i] += 2;
                else if (c == 'l') cur[i] += 1;
                else if (c == 'L') cur[i] += 0;
            }
        }
 
        int[] need = new int[n + 1];
        string[] ps = Console.ReadLine().Split();
        for (int i = 1; i <= n; i++)
            need[i] = int.Parse(ps[i - 1]) - cur[i];
 
        List<(int i, int j, int node)> games = new List<(int, int, int)>();
        int S = 0;
        int T = n + 1;
        int nextNode = n + 2;
        int maxGames = n * (n - 1) / 2;
        g = new List<Edge>[n + 2 + maxGames];
        for (int i = 0; i < g.Length; i++) g[i] = new List<Edge>();
 
        for (int i = 1; i <= n; i++)
            AddEdge(i, T, need[i]);
 
        for (int i = 1; i <= n; i++)
        {
            for (int j = i + 1; j <= n; j++)
            {
                if (table[i, j] == '.')
                {
                    int gameNode = nextNode++;
                    games.Add((i, j, gameNode));
                    AddEdge(S, gameNode, 3);
                    AddEdge(gameNode, i, 3);
                    AddEdge(gameNode, j, 3);
                }
            }
        }
 
        MaxFlow(S, T);
 
        foreach (var (i, j, node) in games)
        {
            int flowToI = 0, flowToJ = 0;
            foreach (Edge e in g[node])
            {
                if (e.To == i) flowToI = 3 - e.Cap;
                if (e.To == j) flowToJ = 3 - e.Cap;
            }
            if (flowToI == 3 && flowToJ == 0)
            {
                table[i, j] = 'W';
                table[j, i] = 'L';
            }
            else if (flowToI == 0 && flowToJ == 3)
            {
                table[i, j] = 'L';
                table[j, i] = 'W';
            }
            else if (flowToI == 2 && flowToJ == 1)
            {
                table[i, j] = 'w';
                table[j, i] = 'l';
            }
            else if (flowToI == 1 && flowToJ == 2)
            {
                table[i, j] = 'l';
                table[j, i] = 'w';
            }
        }
 
        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= n; j++)
                Console.Write(table[i, j]);
            Console.WriteLine();
        }
    }
}
