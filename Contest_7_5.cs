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

public class VertexBiconnectedComponents
{
    private class Edge
    {
        public int From { get; }
        public int To { get; }
        public int Id { get; }

        public Edge(int from, int to, int id)
        {
            From = from;
            To = to;
            Id = id;
        }
    }

    private static List<Edge>[] graph;
    private static int[] tin;
    private static int[] fup;
    private static bool[] visited;
    private static int[] componentId;
    private static int timer;
    private static int componentCounter;
    private static Stack<Edge> stack;

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
                Edge edge = new Edge(u, v, i);
                graph[u].Add(edge);
                graph[v].Add(edge);
            }

            tin = new int[n + 1];
            fup = new int[n + 1];
            visited = new bool[n + 1];
            componentId = new int[m + 1];
            stack = new Stack<Edge>();
            timer = 0;
            componentCounter = 0;

            for (int i = 1; i <= n; i++)
            {
                if (!visited[i])
                {
                    Dfs(i, -1, -1);
                }
            }

            if (stack.Count > 0)
            {
                componentCounter++;
                while (stack.Count > 0)
                {
                    Edge e = stack.Pop();
                    componentId[e.Id] = componentCounter;
                }
            }

            output.WriteLine(componentCounter);
            for (int i = 1; i <= m; i++)
            {
                output.Write(componentId[i]);
                if (i < m)
                    output.Write(" ");
            }
            output.WriteLine();
        }
    }

    private static void Dfs(int v, int parent, int parentEdgeId)
    {
        visited[v] = true;
        tin[v] = fup[v] = ++timer;

        foreach (var edge in graph[v])
        {
            int to = (edge.From == v) ? edge.To : edge.From;

            if (to == parent && edge.Id == parentEdgeId)
                continue;

            if (!visited[to])
            {
                stack.Push(edge);
                Dfs(to, v, edge.Id);
                fup[v] = Math.Min(fup[v], fup[to]);

                if (fup[to] >= tin[v])
                {
                    componentCounter++;
                    while (true)
                    {
                        Edge e = stack.Pop();
                        componentId[e.Id] = componentCounter;
                        if (e == edge)
                            break;
                    }
                }
            }
            else if (tin[to] < tin[v])
            {
                fup[v] = Math.Min(fup[v], tin[to]);
                stack.Push(edge);
            }
        }
    }
}
