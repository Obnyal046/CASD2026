using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        int t = int.Parse(Console.ReadLine());

        for (int test = 0; test < t; test++)
        {
            if (test > 0) Console.WriteLine();

            string[] input = Console.ReadLine().Split();
            int n = int.Parse(input[0]);
            int m = int.Parse(input[1]);

            List<int>[] graph = new List<int>[n + 1];
            List<int>[] reverseGraph = new List<int>[n + 1];

            for (int i = 1; i <= n; i++)
            {
                graph[i] = new List<int>();
                reverseGraph[i] = new List<int>();
            }

            int[] outDegree = new int[n + 1];

            for (int i = 0; i < m; i++)
            {
                string[] edge = Console.ReadLine().Split();
                int u = int.Parse(edge[0]);
                int v = int.Parse(edge[1]);
                graph[u].Add(v);
                reverseGraph[v].Add(u);
                outDegree[u]++;
            }

            int[] state = new int[n + 1];
            int[] degree = new int[n + 1];
            Queue<int> queue = new Queue<int>();

            for (int i = 1; i <= n; i++)
            {
                degree[i] = outDegree[i];
                if (degree[i] == 0)
                {
                    state[i] = 2;
                    queue.Enqueue(i);
                }
            }

            while (queue.Count > 0)
            {
                int u = queue.Dequeue();

                foreach (int prev in reverseGraph[u])
                {
                    if (state[prev] != 0) continue;

                    if (state[u] == 2)
                    {
                        state[prev] = 1;
                        queue.Enqueue(prev);
                    }
                    else if (state[u] == 1)
                    {
                        degree[prev]--;
                        if (degree[prev] == 0)
                        {
                            state[prev] = 2;
                            queue.Enqueue(prev);
                        }
                    }
                }
            }

            for (int i = 1; i <= n; i++)
            {
                if (state[i] == 0)
                    Console.WriteLine("DRAW");
                else if (state[i] == 1)
                    Console.WriteLine("FIRST");
                else
                    Console.WriteLine("SECOND");
            }
        }
    }
}
