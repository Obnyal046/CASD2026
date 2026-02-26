using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string[] input = Console.ReadLine().Split();
        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);
        int s = int.Parse(input[2]) - 1;

        List<(int to, long weight)>[] graph = new List<(int, long)>[n];
        for (int i = 0; i < n; i++)
            graph[i] = new List<(int, long)>();

        for (int i = 0; i < m; i++)
        {
            string[] edge = Console.ReadLine().Split();
            int a = int.Parse(edge[0]) - 1;
            int b = int.Parse(edge[1]) - 1;
            long w = long.Parse(edge[2]);
            graph[a].Add((b, w));
        }

        long[] dist = new long[n];
        bool[] negative = new bool[n];

        for (int i = 0; i < n; i++)
            dist[i] = long.MaxValue;

        dist[s] = 0;

        for (int iter = 0; iter < n - 1; iter++)
        {
            bool updated = false;
            for (int u = 0; u < n; u++)
            {
                if (dist[u] == long.MaxValue) continue;
                foreach (var edge in graph[u])
                {
                    int v = edge.to;
                    long w = edge.weight;
                    if (dist[u] + w < dist[v])
                    {
                        dist[v] = dist[u] + w;
                        updated = true;
                    }
                }
            }
            if (!updated) break;
        }

        Queue<int> queue = new Queue<int>();
        for (int u = 0; u < n; u++)
        {
            if (dist[u] == long.MaxValue) continue;
            foreach (var edge in graph[u])
            {
                int v = edge.to;
                long w = edge.weight;
                if (dist[u] + w < dist[v] && !negative[v])
                {
                    negative[v] = true;
                    queue.Enqueue(v);
                }
            }
        }

        while (queue.Count > 0)
        {
            int u = queue.Dequeue();
            foreach (var edge in graph[u])
            {
                int v = edge.to;
                if (!negative[v])
                {
                    negative[v] = true;
                    queue.Enqueue(v);
                }
            }
        }

        for (int i = 0; i < n; i++)
        {
            if (dist[i] == long.MaxValue)
                Console.WriteLine("*");
            else if (negative[i])
                Console.WriteLine("-");
            else
                Console.WriteLine(dist[i]);
        }
    }
}
