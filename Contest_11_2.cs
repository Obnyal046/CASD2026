using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    class Edge
    {
        public int To, Rev;
        public long Cap;
        public Edge(int to, int rev, long cap)
        {
            To = to; Rev = rev; Cap = cap;
        }
    }

    static List<Edge>[] g;
    static int[] level, iter;

    static void AddEdge(int from, int to, long cap)
    {
        g[from].Add(new Edge(to, g[to].Count, cap));
        g[to].Add(new Edge(from, g[from].Count - 1, 0));
    }

    static void Bfs(int s)
    {
        Array.Fill(level, -1);
        var q = new Queue<int>();
        level[s] = 0;
        q.Enqueue(s);
        while (q.Count > 0)
        {
            int v = q.Dequeue();
            foreach (var e in g[v])
                if (e.Cap > 0 && level[e.To] < 0)
                {
                    level[e.To] = level[v] + 1;
                    q.Enqueue(e.To);
                }
        }
    }

    static long Dfs(int v, int t, long f)
    {
        if (v == t) return f;
        for (; iter[v] < g[v].Count; iter[v]++)
        {
            var e = g[v][iter[v]];
            if (e.Cap > 0 && level[v] < level[e.To])
            {
                long d = Dfs(e.To, t, Math.Min(f, e.Cap));
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

    static long MaxFlow(int s, int t)
    {
        long flow = 0;
        level = new int[g.Length];
        iter = new int[g.Length];
        while (true)
        {
            Bfs(s);
            if (level[t] < 0) break;
            Array.Fill(iter, 0);
            long f;
            while ((f = Dfs(s, t, long.MaxValue)) > 0)
                flow += f;
        }
        return flow;
    }

    static void Main()
    {
        string[] nm = Console.ReadLine().Split();
        int n = int.Parse(nm[0]);
        int m = int.Parse(nm[1]);

        var edges = new List<(int a, int b, long cap)>();
        g = new List<Edge>[n + 1];
        for (int i = 1; i <= n; i++) g[i] = new List<Edge>();

        for (int i = 0; i < m; i++)
        {
            string[] parts = Console.ReadLine().Split();
            int a = int.Parse(parts[0]);
            int b = int.Parse(parts[1]);
            long cap = long.Parse(parts[2]);
            edges.Add((a, b, cap));
            AddEdge(a, b, cap);
            AddEdge(b, a, cap);
        }

        MaxFlow(1, n);

        bool[] reachable = new bool[n + 1];
        var queue = new Queue<int>();
        queue.Enqueue(1);
        reachable[1] = true;
        while (queue.Count > 0)
        {
            int v = queue.Dequeue();
            foreach (var e in g[v])
                if (e.Cap > 0 && !reachable[e.To])
                {
                    reachable[e.To] = true;
                    queue.Enqueue(e.To);
                }
        }

        var cutEdges = new List<int>();
        long totalCap = 0;
        for (int i = 0; i < m; i++)
        {
            var (a, b, cap) = edges[i];
            if (reachable[a] != reachable[b])
            {
                cutEdges.Add(i + 1);
                totalCap += cap;
            }
        }

        Console.WriteLine($"{cutEdges.Count} {totalCap}");
        if (cutEdges.Count > 0)
            Console.WriteLine(string.Join(" ", cutEdges));
        else
            Console.WriteLine();
    }
}
