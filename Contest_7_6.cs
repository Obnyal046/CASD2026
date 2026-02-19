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

        int res = 0;
        do
        {
            res = res * 10 + (c - '0');
            c = ReadChar();
        } while (c >= '0' && c <= '9');

        return res;
    }
}

public class CondensationEdges
{
    private static List<int>[] graph;
    private static List<int>[] reverseGraph;
    private static bool[] visited;
    private static Stack<int> order;
    private static int[] component;
    private static int componentCount;

    public static void Main()
    {
        FastScanner fs = new FastScanner(Console.In);
        using (var output = new StreamWriter(Console.OpenStandardOutput()))
        {
            int n = fs.NextInt();
            int m = fs.NextInt();

            graph = new List<int>[n + 1];
            reverseGraph = new List<int>[n + 1];

            for (int i = 1; i <= n; i++)
            {
                graph[i] = new List<int>();
                reverseGraph[i] = new List<int>();
            }

            for (int i = 0; i < m; i++)
            {
                int u = fs.NextInt();
                int v = fs.NextInt();
                graph[u].Add(v);
                reverseGraph[v].Add(u);
            }

            visited = new bool[n + 1];
            order = new Stack<int>();

            for (int i = 1; i <= n; i++)
            {
                if (!visited[i])
                {
                    Dfs1(i);
                }
            }

            component = new int[n + 1];
            componentCount = 0;

            while (order.Count > 0)
            {
                int v = order.Pop();
                if (component[v] == 0)
                {
                    componentCount++;
                    Dfs2(v, componentCount);
                }
            }

            bool[,] edgeExists = new bool[componentCount + 1, componentCount + 1];

            for (int u = 1; u <= n; u++)
            {
                int compU = component[u];
                foreach (int v in graph[u])
                {
                    int compV = component[v];
                    if (compU != compV && !edgeExists[compU, compV])
                    {
                        edgeExists[compU, compV] = true;
                    }
                }
            }

            int edgesCount = 0;
            for (int i = 1; i <= componentCount; i++)
            {
                for (int j = 1; j <= componentCount; j++)
                {
                    if (edgeExists[i, j])
                        edgesCount++;
                }
            }

            output.WriteLine(edgesCount);
        }
    }

    private static void Dfs1(int v)
    {
        visited[v] = true;
        foreach (int to in graph[v])
        {
            if (!visited[to])
            {
                Dfs1(to);
            }
        }
        order.Push(v);
    }

    private static void Dfs2(int v, int compId)
    {
        component[v] = compId;
        foreach (int to in reverseGraph[v])
        {
            if (component[to] == 0)
            {
                Dfs2(to, compId);
            }
        }
    }
}
