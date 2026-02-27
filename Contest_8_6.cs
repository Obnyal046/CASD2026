using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string[] input = Console.ReadLine().Split();
        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);

        List<(int to, long weight)>[] graph = new List<(int, long)>[n + 1];
        for (int i = 1; i <= n; i++)
            graph[i] = new List<(int, long)>();

        for (int i = 0; i < m; i++)
        {
            string[] edge = Console.ReadLine().Split();
            int u = int.Parse(edge[0]);
            int v = int.Parse(edge[1]);
            long w = long.Parse(edge[2]);
            graph[u].Add((v, w));
            graph[v].Add((u, w));
        }

        string[] targets = Console.ReadLine().Split();
        int a = int.Parse(targets[0]);
        int b = int.Parse(targets[1]);
        int c = int.Parse(targets[2]);

        long[] distA = Dijkstra(n, graph, a);
        long[] distB = Dijkstra(n, graph, b);
        long[] distC = Dijkstra(n, graph, c);

        long ans = long.MaxValue;

        if (distA[b] != long.MaxValue && distB[c] != long.MaxValue)
            ans = Math.Min(ans, distA[b] + distB[c]);

        if (distA[c] != long.MaxValue && distC[b] != long.MaxValue)
            ans = Math.Min(ans, distA[c] + distC[b]);

        if (distB[a] != long.MaxValue && distA[c] != long.MaxValue)
            ans = Math.Min(ans, distB[a] + distA[c]);

        if (distB[c] != long.MaxValue && distC[a] != long.MaxValue)
            ans = Math.Min(ans, distB[c] + distC[a]);

        if (distC[a] != long.MaxValue && distA[b] != long.MaxValue)
            ans = Math.Min(ans, distC[a] + distA[b]);

        if (distC[b] != long.MaxValue && distB[a] != long.MaxValue)
            ans = Math.Min(ans, distC[b] + distB[a]);

        Console.WriteLine(ans == long.MaxValue ? -1 : ans);
    }

    static long[] Dijkstra(int n, List<(int to, long weight)>[] graph, int start)
    {
        long[] dist = new long[n + 1];
        for (int i = 1; i <= n; i++)
            dist[i] = long.MaxValue;

        dist[start] = 0;
        var pq = new SortedSet<(long dist, int vertex)>();
        pq.Add((0, start));

        while (pq.Count > 0)
        {
            var current = pq.Min;
            pq.Remove(current);

            int u = current.vertex;
            long d = current.dist;

            if (d > dist[u]) continue;

            foreach (var edge in graph[u])
            {
                int v = edge.to;
                long w = edge.weight;

                if (dist[u] + w < dist[v])
                {
                    pq.Remove((dist[v], v));
                    dist[v] = dist[u] + w;
                    pq.Add((dist[v], v));
                }
            }
        }

        return dist;
    }
}
