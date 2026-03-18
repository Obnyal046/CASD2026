using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string[] nmab = Console.ReadLine().Split();
        int n = int.Parse(nmab[0]);
        int m = int.Parse(nmab[1]);
        int a = int.Parse(nmab[2]);
        int b = int.Parse(nmab[3]);

        bool[,] free = new bool[n, m];
        int total = 0;
        for (int i = 0; i < n; i++)
        {
            string line = Console.ReadLine();
            for (int j = 0; j < m; j++)
            {
                free[i, j] = line[j] == '*';
                if (free[i, j]) total++;
            }
        }

        if (a >= 2 * b)
        {
            Console.WriteLine((long)total * b);
            return;
        }

        List<int>[] graph = new List<int>[n * m];
        int[] leftId = new int[n * m];

        int leftCount = 0;
        int rightCount = 0;
        int[,] leftIndex = new int[n, m];
        int[,] rightIndex = new int[n, m];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if (!free[i, j]) continue;
                if ((i + j) % 2 == 0)
                {
                    leftIndex[i, j] = leftCount++;
                }
                else
                {
                    rightIndex[i, j] = rightCount++;
                }
            }
        }

        for (int i = 0; i < leftCount; i++) graph[i] = new List<int>();

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if (!free[i, j] || (i + j) % 2 != 0) continue;
                int u = leftIndex[i, j];
                for (int d = 0; d < 4; d++)
                {
                    int ni = i + dx[d];
                    int nj = j + dy[d];
                    if (ni >= 0 && ni < n && nj >= 0 && nj < m && free[ni, nj])
                    {
                        int v = rightIndex[ni, nj];
                        graph[u].Add(v);
                    }
                }
            }
        }

        int[] matchR = new int[rightCount];
        for (int i = 0; i < rightCount; i++) matchR[i] = -1;
        bool[] used;

        bool Dfs(int u)
        {
            foreach (int v in graph[u])
            {
                if (used[v]) continue;
                used[v] = true;
                if (matchR[v] == -1 || Dfs(matchR[v]))
                {
                    matchR[v] = u;
                    return true;
                }
            }
            return false;
        }

        int matching = 0;
        for (int u = 0; u < leftCount; u++)
        {
            used = new bool[rightCount];
            if (Dfs(u)) matching++;
        }

        long result = (long)total * b + (long)(a - 2 * b) * matching;
        Console.WriteLine(result);
    }
}
