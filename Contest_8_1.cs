using System;

class Program
{
    static void Main()
    {
        int n = int.Parse(Console.ReadLine());

        int[,] dist = new int[n, n];
        for (int i = 0; i < n; i++)
        {
            string[] line = Console.ReadLine().Split();
            for (int j = 0; j < n; j++)
            {
                dist[i, j] = int.Parse(line[j]);
            }
        }

        for (int k = 0; k < n; k++)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (dist[i, k] + dist[k, j] < dist[i, j])
                    {
                        dist[i, j] = dist[i, k] + dist[k, j];
                    }
                }
            }
        }

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Console.Write(dist[i, j] + (j == n - 1 ? "" : " "));
            }
            Console.WriteLine();
        }
    }
}
