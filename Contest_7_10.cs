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

public class Edge : IComparable<Edge>
{
    public int From { get; }
    public int To { get; }
    public int Weight { get; }

    public Edge(int from, int to, int weight)
    {
        From = from;
        To = to;
        Weight = weight;
    }

    public int CompareTo(Edge other)
    {
        return Weight.CompareTo(other.Weight);
    }
}

public class DSU
{
    private int[] parent;
    private int[] rank;

    public DSU(int n)
    {
        parent = new int[n + 1];
        rank = new int[n + 1];
        for (int i = 1; i <= n; i++)
        {
            parent[i] = i;
            rank[i] = 0;
        }
    }

    public int Find(int x)
    {
        if (parent[x] != x)
            parent[x] = Find(parent[x]);
        return parent[x];
    }

    public bool Union(int x, int y)
    {
        x = Find(x);
        y = Find(y);
        
        if (x == y)
            return false;
        
        if (rank[x] < rank[y])
        {
            parent[x] = y;
        }
        else if (rank[x] > rank[y])
        {
            parent[y] = x;
        }
        else
        {
            parent[y] = x;
            rank[x]++;
        }
        
        return true;
    }
}

public class MinimalSpanningTree
{
    public static void Main()
    {
        FastScanner fs = new FastScanner(Console.In);
        using (var output = new StreamWriter(Console.OpenStandardOutput()))
        {
            int n = fs.NextInt();
            int m = fs.NextInt();

            List<Edge> edges = new List<Edge>();

            for (int i = 0; i < m; i++)
            {
                int u = fs.NextInt();
                int v = fs.NextInt();
                int w = fs.NextInt();
                edges.Add(new Edge(u, v, w));
            }

            edges.Sort();

            DSU dsu = new DSU(n);
            long totalWeight = 0;
            int edgesUsed = 0;

            foreach (Edge edge in edges)
            {
                if (dsu.Union(edge.From, edge.To))
                {
                    totalWeight += edge.Weight;
                    edgesUsed++;
                    
                    if (edgesUsed == n - 1)
                        break;
                }
            }

            output.WriteLine(totalWeight);
        }
    }
}
