using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string[] input = Console.ReadLine().Split();
        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);

        List<(int to, int weight)>[] graph = new List<(int, int)>[n + 1];
        for (int i = 1; i <= n; i++)
            graph[i] = new List<(int, int)>();

        for (int i = 0; i < m; i++)
        {
            string[] edge = Console.ReadLine().Split();
            int u = int.Parse(edge[0]);
            int v = int.Parse(edge[1]);
            int w = int.Parse(edge[2]);
            
            graph[u].Add((v, w));
            graph[v].Add((u, w));
        }

        long[] dist = new long[n + 1];
        for (int i = 1; i <= n; i++)
            dist[i] = long.MaxValue;
        
        dist[1] = 0;
        
        var pq = new SortedSet<(long dist, int vertex)>();
        pq.Add((0, 1));

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
                int w = edge.weight;
                
                if (dist[u] + w < dist[v])
                {
                    pq.Remove((dist[v], v));
                    dist[v] = dist[u] + w;
                    pq.Add((dist[v], v));
                }
            }
        }

        for (int i = 1; i <= n; i++)
        {
            Console.Write(dist[i] + (i == n ? "" : " "));
        }
    }
}
