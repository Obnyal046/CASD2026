using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        string[] input = Console.ReadLine().Split();
        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);
        int k = int.Parse(input[2]);
        int s = int.Parse(input[3]) - 1;

        List<(int to, int weight)>[] graph = new List<(int, int)>[n];
        for (int i = 0; i < n; i++)
            graph[i] = new List<(int, int)>();

        for (int i = 0; i < m; i++)
        {
            string[] edge = Console.ReadLine().Split();
            int a = int.Parse(edge[0]) - 1;
            int b = int.Parse(edge[1]) - 1;
            int w = int.Parse(edge[2]);
            graph[a].Add((b, w));
        }

        long[] dist = new long[n];
        long[] newDist = new long[n];

        for (int i = 0; i < n; i++)
        {
            dist[i] = i == s ? 0 : long.MaxValue;
            newDist[i] = long.MaxValue;
        }

        for (int step = 0; step < k; step++)
        {
            for (int i = 0; i < n; i++)
                newDist[i] = long.MaxValue;

            for (int v = 0; v < n; v++)
            {
                if (dist[v] == long.MaxValue) continue;

                foreach (var edge in graph[v])
                {
                    int to = edge.to;
                    long newWeight = dist[v] + edge.weight;
                    if (newWeight < newDist[to])
                        newDist[to] = newWeight;
                }
            }

            long[] temp = dist;
            dist = newDist;
            newDist = temp;
        }

        for (int i = 0; i < n; i++)
        {
            if (dist[i] == long.MaxValue)
                Console.WriteLine(-1);
            else
                Console.WriteLine(dist[i]);
        }
    }
}
