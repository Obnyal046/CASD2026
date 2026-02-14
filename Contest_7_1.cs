using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        var input = Console.ReadLine()?.Split();
        if (input == null) return;

        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);

        List<int>[] graph = new List<int>[n + 1];
        for (int i = 1; i <= n; i++)
            graph[i] = new List<int>();

        int[] inDegree = new int[n + 1];

        for (int i = 0; i < m; i++)
        {
            var edge = Console.ReadLine()?.Split();
            if (edge == null) continue;

            int from = int.Parse(edge[0]);
            int to = int.Parse(edge[1]);

            graph[from].Add(to);
            inDegree[to]++;
        }

        var result = TopologicalSort(graph, inDegree, n);

        if (result == null)
            Console.WriteLine(-1);
        else
            Console.WriteLine(string.Join(" ", result));
    }

    static List<int> TopologicalSort(List<int>[] graph, int[] inDegree, int n)
    {
        Queue<int> queue = new Queue<int>();
        List<int> result = new List<int>();

        for (int i = 1; i <= n; i++)
        {
            if (inDegree[i] == 0)
                queue.Enqueue(i);
        }

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();
            result.Add(current);

            foreach (int neighbor in graph[current])
            {
                inDegree[neighbor]--;
                if (inDegree[neighbor] == 0)
                    queue.Enqueue(neighbor);
            }
        }

        return result.Count == n ? result : null;
    }
}
