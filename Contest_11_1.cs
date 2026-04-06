using System;
using System.Collections.Generic;

class Program
{
    class Edge
    {
        public int To;
        public int Rev;
        public long Cap;
        public Edge(int to, int rev, long cap)
        {
            To = to;
            Rev = rev;
            Cap = cap;
        }
    }

    static List<Edge>[] graph;
    static int[] level, iter;

    static Edge AddDirectedEdge(int from, int to, long cap)
    {
        Edge e1 = new Edge(to, graph[to].Count, cap);
        Edge e2 = new Edge(from, graph[from].Count, 0);
        graph[from].Add(e1);
        graph[to].Add(e2);
        return e1;
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
            foreach (Edge e in graph[v])
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
        for (; iter[v] < graph[v].Count; iter[v]++)
        {
            Edge e = graph[v][iter[v]];
            if (e.Cap > 0 && level[v] < level[e.To])
            {
                long d = Dfs(e.To, t, Math.Min(f, e.Cap));
                if (d > 0)
                {
                    e.Cap -= d;
                    graph[e.To][e.Rev].Cap += d;
                    return d;
                }
            }
        }
        return 0;
    }

    static long MaxFlow(int s, int t)
    {
        long flow = 0;
        level = new int[graph.Length];
        iter = new int[graph.Length];
        while (true)
        {
            Bfs(s);
            if (level[t] < 0) break;
            for (int i = 0; i < iter.Length; i++) iter[i] = 0;
            long f;
            while ((f = Dfs(s, t, long.MaxValue)) > 0)
                flow += f;
        }
        return flow;
    }

    static void Main()
    {
        int n = int.Parse(Console.ReadLine());
        int m = int.Parse(Console.ReadLine());
        graph = new List<Edge>[n + 1];
        for (int i = 1; i <= n; i++) graph[i] = new List<Edge>();

        var pipes = new List<(Edge ab, Edge ba, long cap)>();

        for (int i = 0; i < m; i++)
        {
            string[] parts = Console.ReadLine().Split();
            int a = int.Parse(parts[0]);
            int b = int.Parse(parts[1]);
            long c = long.Parse(parts[2]);

            Edge ab = AddDirectedEdge(a, b, c);
            Edge ba = AddDirectedEdge(b, a, c);
            pipes.Add((ab, ba, c));
        }

        long maxFlow = MaxFlow(1, n);
        Console.WriteLine(maxFlow);

        foreach (var p in pipes)
        {
            long flowAB = p.cap - p.ab.Cap;
            long flowBA = p.cap - p.ba.Cap;
            long net = flowAB - flowBA;
            Console.WriteLine($"{net:F3}");
        }
    }
}
