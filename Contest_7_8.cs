using System;
using System.Collections.Generic;
using System.IO;

public class FastScanner
{
    private readonly TextReader reader;
    private char[] buffer;
    private int bufferSize, bufferIndex;

    public FastScanner(TextReader reader)
    {
        this.reader = reader;
        buffer = new char[1 << 12];
        bufferSize = 0;
        bufferIndex = 0;
    }

    private void ReadBuffer()
    {
        bufferSize = reader.Read(buffer, 0, buffer.Length);
        bufferIndex = 0;
        if (bufferSize == 0)
            buffer[0] = '\0';
    }

    private char ReadChar()
    {
        if (bufferIndex >= bufferSize)
            ReadBuffer();
        return buffer[bufferIndex++];
    }

    public int NextInt()
    {
        char c;
        while ((c = ReadChar()) <= ' ') ;

        bool neg = false;
        if (c == '-')
        {
            neg = true;
            c = ReadChar();
        }

        int res = 0;
        do
        {
            res = res * 10 + (c - '0');
            c = ReadChar();
        } while (c >= '0' && c <= '9');

        return neg ? -res : res;
    }
}

public class Program
{
    private static int n;
    private static int[][] cost;

    private static bool IsStronglyConnected(int X)
    {
        bool[] visited = new bool[n];
        DFS(0, X, visited, false);
        for (int i = 0; i < n; i++)
            if (!visited[i]) return false;

        visited = new bool[n];
        DFS(0, X, visited, true);
        for (int i = 0; i < n; i++)
            if (!visited[i]) return false;

        return true;
    }

    private static void DFS(int v, int X, bool[] visited, bool reverse)
    {
        visited[v] = true;
        for (int to = 0; to < n; to++)
        {
            if (visited[to]) continue;
            if (!reverse)
            {
                if (cost[v][to] <= X)
                    DFS(to, X, visited, false);
            }
            else
            {
                if (cost[to][v] <= X)
                    DFS(to, X, visited, true);
            }
        }
    }

    public static void Main()
    {
        FastScanner fs = new FastScanner(Console.In);
        using (var output = new StreamWriter(Console.OpenStandardOutput()))
        {
            n = fs.NextInt();
            cost = new int[n][];
            for (int i = 0; i < n; i++)
            {
                cost[i] = new int[n];
                for (int j = 0; j < n; j++)
                {
                    cost[i][j] = fs.NextInt();
                }
            }

            HashSet<int> weights = new HashSet<int>();
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (i != j)
                        weights.Add(cost[i][j]);
            weights.Add(0);
            List<int> sorted = new List<int>(weights);
            sorted.Sort();

            int left = 0, right = sorted.Count - 1;
            int ans = sorted[right];
            while (left <= right)
            {
                int mid = (left + right) / 2;
                int X = sorted[mid];
                if (IsStronglyConnected(X))
                {
                    ans = X;
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }
            output.WriteLine(ans);
        }
    }
}
