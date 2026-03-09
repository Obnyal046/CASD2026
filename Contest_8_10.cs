using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    static void Main()
    {
        string[] input = Console.ReadLine().Split();
        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);

        List<int>[] graph = new List<int>[n + 1];
        int[] indeg = new int[n + 1];

        for (int i = 1; i <= n; i++)
            graph[i] = new List<int>();

        for (int i = 0; i < m; i++)
        {
            input = Console.ReadLine().Split();
            int x = int.Parse(input[0]);
            int y = int.Parse(input[1]);
            graph[x].Add(y);
            indeg[y]++;
        }

        Queue<int> queue = new Queue<int>();
        for (int i = 1; i <= n; i++)
            if (indeg[i] == 0)
                queue.Enqueue(i);

        List<int> topOrder = new List<int>();
        while (queue.Count > 0)
        {
            int u = queue.Dequeue();
            topOrder.Add(u);
            foreach (int v in graph[u])
            {
                indeg[v]--;
                if (indeg[v] == 0)
                    queue.Enqueue(v);
            }
        }

        int[] grundy = new int[n + 1];

        for (int i = topOrder.Count - 1; i >= 0; i--)
        {
            int u = topOrder[i];
            HashSet<int> values = new HashSet<int>();
            foreach (int v in graph[u])
                values.Add(grundy[v]);

            int mex = 0;
            while (values.Contains(mex))
                mex++;
            grundy[u] = mex;
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 1; i <= n; i++)
            sb.AppendLine(grundy[i].ToString());

        Console.Write(sb.ToString());
    }
}
