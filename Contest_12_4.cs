using System;
using System.Collections.Generic;

class Program
{
    const long INF = (long)1e15;

    class Edge
    {
        public int To, Rev;
        public long Cap, Cost;
        public Edge(int to, int rev, long cap, long cost)
        {
            To = to; Rev = rev; Cap = cap; Cost = cost;
        }
    }

    static List<Edge>[] graph;
    static long[] dist;
    static int[] parentNode, parentEdge;
    static bool[] inQueue;

    static void AddEdge(int from, int to, long cap, long cost)
    {
        graph[from].Add(new Edge(to, graph[to].Count, cap, cost));
        graph[to].Add(new Edge(from, graph[from].Count - 1, 0, -cost));
    }

    static void Main()
    {
        string[] nm = Console.ReadLine().Split();
        int n = int.Parse(nm[0]);
        int m = int.Parse(nm[1]);

        long[] a = new long[n + 1];
        string[] aVals = Console.ReadLine().Split();
        for (int i = 1; i <= n; i++) a[i] = long.Parse(aVals[i - 1]);

        long[,] d = new long[n + 1, n + 1];
        for (int i = 1; i <= n; i++)
            for (int j = 1; j <= n; j++)
                d[i, j] = (i == j) ? 0 : INF;

        for (int i = 0; i < m; i++)
        {
            string[] road = Console.ReadLine().Split();
            int u = int.Parse(road[0]);
            int v = int.Parse(road[1]);
            long c = long.Parse(road[2]);
            if (c < d[u, v]) d[u, v] = c;
        }

        for (int k = 1; k <= n; k++)
            for (int i = 1; i <= n; i++)
                for (int j = 1; j <= n; j++)
                    if (d[i, k] + d[k, j] < d[i, j])
                        d[i, j] = d[i, k] + d[k, j];

        int S = 0, T = 2 * n + 1;
        graph = new List<Edge>[T + 1];
        for (int i = 0; i <= T; i++) graph[i] = new List<Edge>();

        for (int i = 1; i <= n; i++)
        {
            AddEdge(S, i, 1, 0);
            AddEdge(i + n, T, 1, 0);
            AddEdge(i, i + n, 1, a[i]);

            for (int j = 1; j <= n; j++)
                if (i != j && d[i, j] < INF)
                    AddEdge(i, j + n, 1, d[i, j]);
        }

        long minCost = 0;
        int flow = 0;
        dist = new long[T + 1];
        parentNode = new int[T + 1];
        parentEdge = new int[T + 1];
        inQueue = new bool[T + 1];

        while (flow < n)
        {
            for (int i = 0; i <= T; i++) dist[i] = INF;
            dist[S] = 0;
            Queue<int> q = new Queue<int>();
            q.Enqueue(S);
            inQueue[S] = true;

            while (q.Count > 0)
            {
                int v = q.Dequeue();
                inQueue[v] = false;
                for (int i = 0; i < graph[v].Count; i++)
                {
                    Edge e = graph[v][i];
                    if (e.Cap > 0 && dist[e.To] > dist[v] + e.Cost)
                    {
                        dist[e.To] = dist[v] + e.Cost;
                        parentNode[e.To] = v;
                        parentEdge[e.To] = i;
                        if (!inQueue[e.To])
                        {
                            q.Enqueue(e.To);
                            inQueue[e.To] = true;
                        }
                    }
                }
            }

            if (dist[T] == INF) break;

            for (int v = T; v != S; v = parentNode[v])
            {
                int u = parentNode[v];
                int idx = parentEdge[v];
                Edge e = graph[u][idx];
                e.Cap -= 1;
                graph[e.To][e.Rev].Cap += 1;
                minCost += e.Cost;
            }
            flow++;
        }

        Console.WriteLine(minCost);
    }
}
