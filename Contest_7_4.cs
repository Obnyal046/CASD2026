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

public class EdgeBiconnectedComponents
{
    private class Edge
    {
        public int To { get; }
        public int Id { get; }

        public Edge(int to, int id)
        {
            To = to;
            Id = id;
        }
    }

    private static List<Edge>[] graph;
    private static bool[] isBridge;
    private static int[] tin;
    private static int[] fup;
    private static bool[] visited;
    private static int[] componentId;
    private static int timer;
    private static int componentCounter;

    public static void Main()
    {
        FastScanner fs = new FastScanner(Console.In);
        using (var output = new StreamWriter(Console.OpenStandardOutput()))
        {
            int n = fs.NextInt();
            int m = fs.NextInt();

            graph = new List<Edge>[n + 1];
            for (int i = 1; i <= n; i++)
            {
                graph[i] = new List<Edge>();
            }

            for (int i = 1; i <= m; i++)
            {
                int u = fs.NextInt();
                int v = fs.NextInt();
                graph[u].Add(new Edge(v, i));
                graph[v].Add(new Edge(u, i));
            }

            tin = new int[n + 1];
            fup = new int[n + 1];
            visited = new bool[n + 1];
            isBridge = new bool[m + 1];
            timer = 0;

            for (int i = 1; i <= n; i++)
            {
                if (!visited[i])
                {
                    FindBridges(i, -1, -1);
                }
            }

            componentId = new int[n + 1];
            componentCounter = 0;
            Array.Fill(visited, false);

            for (int i = 1; i <= n; i++)
            {
                if (!visited[i])
                {
                    componentCounter++;
                    DfsComponents(i, componentCounter);
                }
            }

            output.WriteLine(componentCounter);
            for (int i = 1; i <= n; i++)
            {
                output.Write(componentId[i]);
                if (i < n)
                    output.Write(" ");
            }
            output.WriteLine();
        }
    }

    private static void FindBridges(int v, int parent, int incomingEdgeId)
    {
        visited[v] = true;
        tin[v] = fup[v] = ++timer;

        foreach (var edge in graph[v])
        {
            int to = edge.To;
            int id = edge.Id;

            if (id == incomingEdgeId)
                continue;

            if (visited[to])
            {
                fup[v] = Math.Min(fup[v], tin[to]);
            }
            else
            {
                FindBridges(to, v, id);
                fup[v] = Math.Min(fup[v], fup[to]);

                if (fup[to] > tin[v])
                {
                    isBridge[id] = true;
                }
            }
        }
    }

    private static void DfsComponents(int v, int compId)
    {
        visited[v] = true;
        componentId[v] = compId;

        foreach (var edge in graph[v])
        {
            int to = edge.To;
            int id = edge.Id;

            if (!isBridge[id] && !visited[to])
            {
                DfsComponents(to, compId);
            }
        }
    }
}
