using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    const long INF = (long)1e18;

    class Edge
    {
        public int to, rev, cap, cost, id;
        public Edge(int to, int rev, int cap, int cost, int id)
        {
            this.to = to;
            this.rev = rev;
            this.cap = cap;
            this.cost = cost;
            this.id = id;
        }
    }

    static List<Edge>[] graph;
    static int n, m, k;
    static int s, t;

    static void AddEdge(int from, int to, int cap, int cost, int id)
    {
        graph[from].Add(new Edge(to, graph[to].Count, cap, cost, id));
        graph[to].Add(new Edge(from, graph[from].Count - 1, 0, -cost, -1));
    }

    static void Main()
    {
        string[] first = Console.ReadLine().Split();
        n = int.Parse(first[0]);
        m = int.Parse(first[1]);
        k = int.Parse(first[2]);
        s = 0;
        t = n - 1;

        graph = new List<Edge>[n];
        for (int i = 0; i < n; i++) graph[i] = new List<Edge>();

        for (int i = 1; i <= m; i++)
        {
            string[] road = Console.ReadLine().Split();
            int u = int.Parse(road[0]) - 1;
            int v = int.Parse(road[1]) - 1;
            int w = int.Parse(road[2]);
            AddEdge(u, v, 1, w, i);
            AddEdge(v, u, 1, w, i);
        }

        long[] potential = new long[n];
        long[] dist = new long[n];
        int[] prevv = new int[n];
        int[] preve = new int[n];

        long totalCost = 0;
        int flow = 0;

        while (flow < k)
        {
            for (int i = 0; i < n; i++) dist[i] = INF;
            dist[s] = 0;
            var set = new SortedSet<(long d, int v)>();
            set.Add((0, s));
            while (set.Count > 0)
            {
                var cur = set.Min;
                set.Remove(cur);
                int v = cur.v;
                if (cur.d != dist[v]) continue;
                for (int i = 0; i < graph[v].Count; i++)
                {
                    Edge e = graph[v][i];
                    if (e.cap > 0)
                    {
                        long nd = dist[v] + e.cost + potential[v] - potential[e.to];
                        if (nd < dist[e.to])
                        {
                            dist[e.to] = nd;
                            prevv[e.to] = v;
                            preve[e.to] = i;
                            set.Add((nd, e.to));
                        }
                    }
                }
            }
            if (dist[t] == INF) break;

            for (int i = 0; i < n; i++)
                if (dist[i] < INF) potential[i] += dist[i];

            int addf = 1;
            for (int v = t; v != s; v = prevv[v])
            {
                Edge e = graph[prevv[v]][preve[v]];
                addf = Math.Min(addf, e.cap);
            }
            for (int v = t; v != s; v = prevv[v])
            {
                Edge e = graph[prevv[v]][preve[v]];
                e.cap -= addf;
                graph[e.to][e.rev].cap += addf;
                totalCost += (long)addf * e.cost;
            }
            flow += addf;
        }

        if (flow < k)
        {
            Console.WriteLine(-1);
            return;
        }

        List<(int to, int id)>[] used = new List<(int, int)>[n];
        for (int i = 0; i < n; i++) used[i] = new List<(int, int)>();
        for (int u = 0; u < n; u++)
        {
            foreach (Edge e in graph[u])
            {
                if (e.id != -1 && e.cap == 0)
                {
                    used[u].Add((e.to, e.id));
                }
            }
        }

        List<List<int>> paths = new List<List<int>>();
        for (int i = 0; i < k; i++)
        {
            List<int> path = new List<int>();
            int cur = s;
            while (cur != t)
            {
                var (next, id) = used[cur][0];
                used[cur].RemoveAt(0);
                path.Add(id);
                cur = next;
            }
            paths.Add(path);
        }

        double average = (double)totalCost / k;
        Console.WriteLine(average.ToString("F5"));
        for (int i = 0; i < k; i++)
        {
            Console.Write(paths[i].Count);
            foreach (int id in paths[i]) Console.Write(" " + id);
            Console.WriteLine();
        }
    }
}
