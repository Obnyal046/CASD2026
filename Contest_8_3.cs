using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        int n = int.Parse(Console.ReadLine());
        int[,] graph = new int[n, n];

        for (int i = 0; i < n; i++)
        {
            string[] line = Console.ReadLine().Split();
            for (int j = 0; j < n; j++)
            {
                graph[i, j] = int.Parse(line[j]);
            }
        }

        int[] dist = new int[n];
        int[] parent = new int[n];
        int start = -1;

        for (int i = 0; i < n; i++)
        {
            dist[i] = 0;
            parent[i] = -1;
        }

        for (int iter = 0; iter < n; iter++)
        {
            start = -1;
            for (int u = 0; u < n; u++)
            {
                for (int v = 0; v < n; v++)
                {
                    if (graph[u, v] < 100000 && dist[u] + graph[u, v] < dist[v])
                    {
                        dist[v] = dist[u] + graph[u, v];
                        parent[v] = u;
                        if (iter == n - 1)
                        {
                            start = v;
                        }
                    }
                }
            }
        }

        if (start == -1)
        {
            Console.WriteLine("NO");
            return;
        }

        for (int i = 0; i < n; i++)
        {
            start = parent[start];
        }

        List<int> cycle = new List<int>();
        int current = start;
        do
        {
            cycle.Add(current + 1);
            current = parent[current];
        } while (current != start);

        cycle.Reverse();

        Console.WriteLine("YES");
        Console.WriteLine(cycle.Count);
        Console.WriteLine(string.Join(" ", cycle));
    }
}
