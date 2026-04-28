using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    class Edge
    {
        public int To, Rev;
        public long Cap;
        public long Cost;
        public Edge(int to, int rev, long cap, long cost)
        {
            To = to; Rev = rev; Cap = cap; Cost = cost;
        }
    }

    static List<Edge>[] graph;
    static long[] potential, dist;
    static int[] prevv, preve;

    static void AddEdge(int from, int to, long cap, long cost)
    {
        graph[from].Add(new Edge(to, graph[to].Count, cap, cost));
        graph[to].Add(new Edge(from, graph[from].Count - 1, 0, -cost));
    }

    static long MinCostMaxFlow(int s, int t, int n)
    {
        long flow = 0;
        long cost = 0;
        potential = new long[n];
        dist = new long[n];
        prevv = new int[n];
        preve = new int[n];

        while (true)
        {
            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            dist[s] = 0;
            var pq = new SortedSet<(long d, int v)>();
            pq.Add((0, s));

            while (pq.Count > 0)
            {
                var cur = pq.Min;
                pq.Remove(cur);
                int v = cur.v;
                if (cur.d != dist[v]) continue;
                for (int i = 0; i < graph[v].Count; i++)
                {
                    Edge e = graph[v][i];
                    if (e.Cap > 0)
                    {
                        long nd = dist[v] + e.Cost + potential[v] - potential[e.To];
                        if (dist[e.To] > nd)
                        {
                            dist[e.To] = nd;
                            prevv[e.To] = v;
                            preve[e.To] = i;
                            pq.Add((nd, e.To));
                        }
                    }
                }
            }

            if (dist[t] == long.MaxValue) break;

            for (int i = 0; i < n; i++)
                if (dist[i] < long.MaxValue)
                    potential[i] += dist[i];

            long addf = long.MaxValue;
            for (int v = t; v != s; v = prevv[v])
            {
                Edge e = graph[prevv[v]][preve[v]];
                addf = Math.Min(addf, e.Cap);
            }

            for (int v = t; v != s; v = prevv[v])
            {
                Edge e = graph[prevv[v]][preve[v]];
                e.Cap -= addf;
                graph[v][e.Rev].Cap += addf;
                cost += addf * e.Cost;
            }
            flow += addf;
        }
        return cost;
    }

    static void Main()
    {
        string[] nm = Console.ReadLine().Split();
        int n = int.Parse(nm[0]);
        int m = int.Parse(nm[1]);
        graph = new List<Edge>[n];
        for (int i = 0; i < n; i++) graph[i] = new List<Edge>();

        for (int i = 0; i < m; i++)
        {
            string[] parts = Console.ReadLine().Split();
            int u = int.Parse(parts[0]) - 1;
            int v = int.Parse(parts[1]) - 1;
            long cap = long.Parse(parts[2]);
            long cost = long.Parse(parts[3]);
            AddEdge(u, v, cap, cost);
        }

        long result = MinCostMaxFlow(0, n - 1, n);
        Console.WriteLine(result);
    }
}
