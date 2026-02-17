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

public class ArticulationPointsFinder
{
    private static List<int>[] graph;
    private static int[] tin;
    private static int[] fup;
    private static bool[] visited;
    private static bool[] isArticulation;
    private static int timer;
    private static List<int> articulationPoints;

    public static void Main()
    {
        FastScanner fs = new FastScanner(Console.In);
        using (var output = new StreamWriter(Console.OpenStandardOutput()))
        {
            int n = fs.NextInt();
            int m = fs.NextInt();

            graph = new List<int>[n + 1];
            for (int i = 1; i <= n; i++)
            {
                graph[i] = new List<int>();
            }

            for (int i = 0; i < m; i++)
            {
                int u = fs.NextInt();
                int v = fs.NextInt();
                graph[u].Add(v);
                graph[v].Add(u);
            }

            tin = new int[n + 1];
            fup = new int[n + 1];
            visited = new bool[n + 1];
            isArticulation = new bool[n + 1];
            articulationPoints = new List<int>();

            for (int i = 1; i <= n; i++)
            {
                if (!visited[i])
                {
                    Dfs(i, -1);
                }
            }

            for (int i = 1; i <= n; i++)
            {
                if (isArticulation[i])
                {
                    articulationPoints.Add(i);
                }
            }

            output.WriteLine(articulationPoints.Count);
            if (articulationPoints.Count > 0)
            {
                output.WriteLine(string.Join(" ", articulationPoints));
            }
        }
    }

    private static void Dfs(int v, int parent)
    {
        visited[v] = true;
        tin[v] = fup[v] = ++timer;
        int children = 0;

        foreach (int to in graph[v])
        {
            if (to == parent)
                continue;

            if (visited[to])
            {
                fup[v] = Math.Min(fup[v], tin[to]);
            }
            else
            {
                Dfs(to, v);
                fup[v] = Math.Min(fup[v], fup[to]);

                if (fup[to] >= tin[v] && parent != -1)
                {
                    isArticulation[v] = true;
                }
                children++;
            }
        }

        if (parent == -1 && children > 1)
        {
            isArticulation[v] = true;
        }
    }
}
