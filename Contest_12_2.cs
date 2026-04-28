using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        int n = int.Parse(Console.ReadLine());
        long[,] a = new long[n + 1, n + 1];
        for (int i = 1; i <= n; i++)
        {
            string[] parts = Console.ReadLine().Split();
            for (int j = 1; j <= n; j++)
                a[i, j] = long.Parse(parts[j - 1]);
        }

        long[] u = new long[n + 1];
        long[] v = new long[n + 1];
        int[] p = new int[n + 1];
        int[] way = new int[n + 1];

        for (int i = 1; i <= n; i++)
        {
            p[0] = i;
            int j0 = 0;
            long[] minv = new long[n + 1];
            bool[] used = new bool[n + 1];
            for (int j = 1; j <= n; j++) minv[j] = long.MaxValue;

            do
            {
                used[j0] = true;
                int i0 = p[j0];
                long delta = long.MaxValue;
                int j1 = 0;
                for (int j = 1; j <= n; j++)
                    if (!used[j])
                    {
                        long cur = a[i0, j] - u[i0] - v[j];
                        if (cur < minv[j])
                        {
                            minv[j] = cur;
                            way[j] = j0;
                        }
                        if (minv[j] < delta)
                        {
                            delta = minv[j];
                            j1 = j;
                        }
                    }
                for (int j = 0; j <= n; j++)
                    if (used[j])
                    {
                        u[p[j]] += delta;
                        v[j] -= delta;
                    }
                    else minv[j] -= delta;
                j0 = j1;
            } while (p[j0] != 0);

            do
            {
                int j1 = way[j0];
                p[j0] = p[j1];
                j0 = j1;
            } while (j0 != 0);
        }

        long ans = -v[0];
        Console.WriteLine(ans);

        int[] colForRow = new int[n + 1];
        for (int j = 1; j <= n; j++)
            colForRow[p[j]] = j;

        List<(int row, int col)> pairs = new List<(int, int)>();
        for (int col = 1; col <= n; col++)
            for (int row = 1; row <= n; row++)
                if (colForRow[row] == col)
                {
                    pairs.Add((row, col));
                    break;
                }

        foreach (var pair in pairs)
            Console.WriteLine($"{pair.row} {pair.col}");
    }
}
