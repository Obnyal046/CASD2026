using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        string[] mkn = Console.ReadLine().Split();
        int m = int.Parse(mkn[0]);
        int k = int.Parse(mkn[1]);
        int n = int.Parse(mkn[2]);

        int t = int.Parse(Console.ReadLine());
        bool[,] forbidden = new bool[m + 1, k + 1];
        for (int i = 0; i < t; i++)
        {
            string[] pair = Console.ReadLine().Split();
            int g = int.Parse(pair[0]);
            int y = int.Parse(pair[1]) - m;
            forbidden[g, y] = true;
        }

        int q = int.Parse(Console.ReadLine());
        bool[] greenReq = new bool[m + 1];
        bool[] yellowReq = new bool[k + 1];
        string[] reqs = Console.ReadLine().Split();
        for (int i = 0; i < q; i++)
        {
            int id = int.Parse(reqs[i]);
            if (id <= m)
                greenReq[id] = true;
            else
                yellowReq[id - m] = true;
        }

        int greenReqCount = greenReq.Skip(1).Count(x => x);
        int yellowReqCount = yellowReq.Skip(1).Count(x => x);
        if (greenReqCount > n || yellowReqCount > n)
        {
            Console.WriteLine("NO");
            return;
        }

        int totalVertices = 2 + m + k;
        int S = 0, T = 1;
        int Lstart = 2, Rstart = 2 + m;

        var edges = new List<EdgeInfo>();

        for (int i = 1; i <= m; i++)
        {
            int low = greenReq[i] ? 1 : 0;
            edges.Add(new EdgeInfo(S, Lstart + i - 1, low, 1));
        }

        for (int j = 1; j <= k; j++)
        {
            int low = yellowReq[j] ? 1 : 0;
            edges.Add(new EdgeInfo(Rstart + j - 1, T, low, 1));
        }

        for (int i = 1; i <= m; i++)
        {
            for (int j = k; j >= 1; j--)
            {
                if (!forbidden[i, j])
                {
                    edges.Add(new EdgeInfo(Lstart + i - 1, Rstart + j - 1, 0, 1));
                }
            }
        }

        edges.Add(new EdgeInfo(T, S, n, n));

        int N = totalVertices + 2;
        int SS = totalVertices;
        int TT = totalVertices + 1;
        var flowGraph = new FlowGraph(N);
        long[] balance = new long[N];

        foreach (var e in edges)
        {
            if (e.low > 0)
            {
                balance[e.from] -= e.low;
                balance[e.to] += e.low;
            }
            flowGraph.AddEdge(e.from, e.to, e.high - e.low);
        }

        long sumPos = 0;
        for (int i = 0; i < N; i++)
        {
            if (balance[i] > 0)
            {
                flowGraph.AddEdge(SS, i, balance[i]);
                sumPos += balance[i];
            }
            else if (balance[i] < 0)
            {
                flowGraph.AddEdge(i, TT, -balance[i]);
            }
        }

        long flow = flowGraph.MaxFlow(SS, TT);
        if (flow != sumPos)
        {
            Console.WriteLine("NO");
            return;
        }

        var resultPairs = new List<(int, int)>();
        for (int u = Lstart; u < Lstart + m; u++)
        {
            foreach (var edge in flowGraph.Adj[u])
            {
                int v = edge.To;
                if (v >= Rstart && v < Rstart + k && edge.Capacity == 0)
                {
                    int green = u - Lstart + 1;
                    int yellow = v - Rstart + 1;
                    resultPairs.Add((green, yellow));
                }
            }
        }

        if (resultPairs.Count != n)
        {
            Console.WriteLine("NO");
            return;
        }

        Console.WriteLine("YES");
        foreach (var p in resultPairs)
        {
            Console.WriteLine($"{p.Item1} {p.Item2 + m}");
        }
    }
}

class EdgeInfo
{
    public int from, to;
    public long low, high;
    public EdgeInfo(int f, int t, long l, long h) { from = f; to = t; low = l; high = h; }
}

class FlowGraph
{
    public List<Edge>[] Adj;
    public int N;

    public FlowGraph(int n)
    {
        N = n;
        Adj = new List<Edge>[n];
        for (int i = 0; i < n; i++) Adj[i] = new List<Edge>();
    }

    public void AddEdge(int from, int to, long cap)
    {
        Edge e1 = new Edge(to, cap, Adj[to].Count);
        Edge e2 = new Edge(from, 0, Adj[from].Count);
        Adj[from].Add(e1);
        Adj[to].Add(e2);
    }

    public long MaxFlow(int s, int t)
    {
        long flow = 0;
        int[] level = new int[N];
        int[] ptr = new int[N];
        while (true)
        {
            Bfs(s, t, level);
            if (level[t] == -1) break;
            Array.Fill(ptr, 0);
            long pushed;
            while ((pushed = Dfs(s, t, long.MaxValue, level, ptr)) > 0)
            {
                flow += pushed;
            }
        }
        return flow;
    }

    private void Bfs(int s, int t, int[] level)
    {
        Array.Fill(level, -1);
        Queue<int> q = new Queue<int>();
        level[s] = 0;
        q.Enqueue(s);
        while (q.Count > 0)
        {
            int v = q.Dequeue();
            foreach (var e in Adj[v])
            {
                if (e.Capacity > 0 && level[e.To] == -1)
                {
                    level[e.To] = level[v] + 1;
                    q.Enqueue(e.To);
                }
            }
        }
    }

    private long Dfs(int v, int t, long f, int[] level, int[] ptr)
    {
        if (v == t) return f;
        for (; ptr[v] < Adj[v].Count; ptr[v]++)
        {
            Edge e = Adj[v][ptr[v]];
            if (e.Capacity > 0 && level[e.To] == level[v] + 1)
            {
                long pushed = Dfs(e.To, t, Math.Min(f, e.Capacity), level, ptr);
                if (pushed > 0)
                {
                    e.Capacity -= pushed;
                    Adj[e.To][e.Rev].Capacity += pushed;
                    return pushed;
                }
            }
        }
        return 0;
    }
}

class Edge
{
    public int To;
    public long Capacity;
    public int Rev;
    public Edge(int to, long cap, int rev) { To = to; Capacity = cap; Rev = rev; }
}
