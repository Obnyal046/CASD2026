using System;
using System.Collections.Generic;
 
class Program
{
    const int INF = 1000000;
 
    class Edge
    {
        public int To, Rev;
        public int Cap;
        public Edge(int to, int rev, int cap) { To = to; Rev = rev; Cap = cap; }
    }
 
    static List<Edge>[] graph;
    static int[] level, iter;
 
    static void AddEdge(int from, int to, int cap)
    {
        graph[from].Add(new Edge(to, graph[to].Count, cap));
        graph[to].Add(new Edge(from, graph[from].Count - 1, 0));
    }
 
    static void Bfs(int s)
    {
        for (int i = 0; i < level.Length; i++) level[i] = -1;
        Queue<int> q = new Queue<int>();
        level[s] = 0;
        q.Enqueue(s);
        while (q.Count > 0)
        {
            int v = q.Dequeue();
            foreach (Edge e in graph[v])
                if (e.Cap > 0 && level[e.To] < 0)
                {
                    level[e.To] = level[v] + 1;
                    q.Enqueue(e.To);
                }
        }
    }
 
    static int Dfs(int v, int t, int f)
    {
        if (v == t) return f;
        for (; iter[v] < graph[v].Count; iter[v]++)
        {
            Edge e = graph[v][iter[v]];
            if (e.Cap > 0 && level[v] < level[e.To])
            {
                int d = Dfs(e.To, t, Math.Min(f, e.Cap));
                if (d > 0)
                {
                    e.Cap -= d;
                    graph[e.To][e.Rev].Cap += d;
                    return d;
                }
            }
        }
        return 0;
    }
 
    static int MaxFlow(int s, int t)
    {
        int flow = 0;
        level = new int[graph.Length];
        iter = new int[graph.Length];
        while (true)
        {
            Bfs(s);
            if (level[t] < 0) break;
            for (int i = 0; i < iter.Length; i++) iter[i] = 0;
            int f;
            while ((f = Dfs(s, t, INF)) > 0)
                flow += f;
        }
        return flow;
    }
 
    static void Main()
    {
        string[] mn = Console.ReadLine().Split();
        int m = int.Parse(mn[0]);
        int n = int.Parse(mn[1]);
        char[,] map = new char[m, n];
        int ax = -1, ay = -1, bx = -1, by = -1;
        for (int i = 0; i < m; i++)
        {
            string line = Console.ReadLine();
            for (int j = 0; j < n; j++)
            {
                map[i, j] = line[j];
                if (map[i, j] == 'A') { ax = i; ay = j; }
                if (map[i, j] == 'B') { bx = i; by = j; }
            }
        }
 
        bool[,] can = new bool[m, n];
        for (int i = 0; i < m; i++)
            for (int j = 0; j < n; j++)
                can[i, j] = (map[i, j] != '#');
        bool pathExists = BfsPath(ax, ay, bx, by, can, m, n);
        if (!pathExists)
        {
            Console.WriteLine(0);
            PrintMap(map, m, n);
            return;
        }
 
        int totalCells = m * n;
        int nodes = totalCells * 2 + 2;
        graph = new List<Edge>[nodes];
        for (int i = 0; i < nodes; i++) graph[i] = new List<Edge>();
 
        int S = (ax * n + ay) * 2;
        int T = (bx * n + by) * 2 + 1;
 
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (map[i, j] == '#') continue;
                int inNode = (i * n + j) * 2;
                int outNode = inNode + 1;
                if (map[i, j] == '.')
                    AddEdge(inNode, outNode, 1);
                else
                    AddEdge(inNode, outNode, INF);
            }
        }
 
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (map[i, j] == '#') continue;
                int outNode = (i * n + j) * 2 + 1;
                for (int d = 0; d < 4; d++)
                {
                    int ni = i + dx[d];
                    int nj = j + dy[d];
                    if (ni >= 0 && ni < m && nj >= 0 && nj < n && map[ni, nj] != '#')
                    {
                        int inNeighbor = (ni * n + nj) * 2;
                        AddEdge(outNode, inNeighbor, INF);
                    }
                }
            }
        }
 
        int flow = MaxFlow(S, T);
        if (flow >= INF)
        {
            Console.WriteLine(-1);
            return;
        }
 
        bool[] reachable = new bool[nodes];
        Queue<int> q = new Queue<int>();
        q.Enqueue(S);
        reachable[S] = true;
        while (q.Count > 0)
        {
            int v = q.Dequeue();
            foreach (Edge e in graph[v])
                if (e.Cap > 0 && !reachable[e.To])
                {
                    reachable[e.To] = true;
                    q.Enqueue(e.To);
                }
        }
 
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (map[i, j] == '.')
                {
                    int inNode = (i * n + j) * 2;
                    int outNode = inNode + 1;
                    if (reachable[inNode] && !reachable[outNode])
                        map[i, j] = '+';
                }
            }
        }
 
        int wallCount = 0;
        for (int i = 0; i < m; i++)
            for (int j = 0; j < n; j++)
                if (map[i, j] == '+')
                    wallCount++;
 
        Console.WriteLine(wallCount);
        PrintMap(map, m, n);
    }
 
    static bool BfsPath(int sx, int sy, int tx, int ty, bool[,] can, int m, int n)
    {
        bool[,] visited = new bool[m, n];
        Queue<(int, int)> q = new Queue<(int, int)>();
        q.Enqueue((sx, sy));
        visited[sx, sy] = true;
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };
        while (q.Count > 0)
        {
            var (x, y) = q.Dequeue();
            if (x == tx && y == ty) return true;
            for (int d = 0; d < 4; d++)
            {
                int nx = x + dx[d];
                int ny = y + dy[d];
                if (nx >= 0 && nx < m && ny >= 0 && ny < n && can[nx, ny] && !visited[nx, ny])
                {
                    visited[nx, ny] = true;
                    q.Enqueue((nx, ny));
                }
            }
        }
        return false;
    }
 
    static void PrintMap(char[,] map, int m, int n)
    {
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
                Console.Write(map[i, j]);
            Console.WriteLine();
        }
    }
}
