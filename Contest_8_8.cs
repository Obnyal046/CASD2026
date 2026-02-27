using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string[] input = Console.ReadLine().Split();
        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);
        int s = int.Parse(input[2]);

        List<int>[] graph = new List<int>[n + 1];
        List<int>[] reverseGraph = new List<int>[n + 1];

        for (int i = 1; i <= n; i++)
        {
            graph[i] = new List<int>();
            reverseGraph[i] = new List<int>();
        }

        for (int i = 0; i < m; i++)
        {
            string[] edge = Console.ReadLine().Split();
            int u = int.Parse(edge[0]);
            int v = int.Parse(edge[1]);
            graph[u].Add(v);
            reverseGraph[v].Add(u);
        }

        int[] degree = new int[n + 1];
        for (int i = 1; i <= n; i++)
            degree[i] = graph[i].Count;

        bool[] win = new bool[n + 1];
        bool[] lose = new bool[n + 1];
        bool[] visited = new bool[n + 1];

        Queue<int> queue = new Queue<int>();

        for (int i = 1; i <= n; i++)
        {
            if (degree[i] == 0)
            {
                lose[i] = true;
                queue.Enqueue(i);
            }
        }

        while (queue.Count > 0)
        {
            int u = queue.Dequeue();
            visited[u] = true;

            foreach (int prev in reverseGraph[u])
            {
                if (visited[prev]) continue;

                if (!win[prev] && lose[u])
                {
                    win[prev] = true;
                    queue.Enqueue(prev);
                }
                else if (!win[prev] && !lose[prev])
                {
                    degree[prev]--;
                    if (degree[prev] == 0)
                    {
                        lose[prev] = true;
                        queue.Enqueue(prev);
                    }
                }
            }
        }

        if (win[s])
            Console.WriteLine("First player wins");
        else
            Console.WriteLine("Second player wins");
    }
}
