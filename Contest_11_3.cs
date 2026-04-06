using System;
using System.Collections.Generic;

class Program
{
    class Edge
    {
        public int To, Rev;
        public long Cap;
        public Edge(int to, int rev, long cap) { To = to; Rev = rev; Cap = cap; }
    }

    static List<Edge>[] graph;
    static List<(int from, int to, Edge edge)> originalEdges = new List<(int, int, Edge)>();

    static void AddEdge(int from, int to, long cap)
    {
        Edge e1 = new Edge(to, graph[to].Count, cap);
        Edge e2 = new Edge(from, graph[from].Count, 0);
        graph[from].Add(e1);
        graph[to].Add(e2);
        originalEdges.Add((from, to, e1));
    }

    static bool Bfs(int s, int t, out (int from, int idx)[] parent)
    {
        parent = new (int, int)[graph.Length];
        for (int i = 0; i < parent.Length; i++) parent[i] = (-1, -1);
        Queue<int> q = new Queue<int>();
        q.Enqueue(s);
        parent[s] = (s, -1);
        while (q.Count > 0)
        {
            int v = q.Dequeue();
            for (int i = 0; i < graph[v].Count; i++)
            {
                Edge e = graph[v][i];
                if (e.Cap > 0 && parent[e.To].from == -1)
                {
                    parent[e.To] = (v, i);
                    if (e.To == t) return true;
                    q.Enqueue(e.To);
                }
            }
        }
        return false;
    }

    static void Main()
    {
        string[] nmst = Console.ReadLine().Split();
        int n = int.Parse(nmst[0]);
        int m = int.Parse(nmst[1]);
        int s = int.Parse(nmst[2]);
        int t = int.Parse(nmst[3]);

        graph = new List<Edge>[n + 1];
        for (int i = 1; i <= n; i++) graph[i] = new List<Edge>();

        for (int i = 0; i < m; i++)
        {
            string[] xy = Console.ReadLine().Split();
            int x = int.Parse(xy[0]);
            int y = int.Parse(xy[1]);
            AddEdge(x, y, 1);
        }

        int flow = 0;
        while (flow < 2)
        {
            if (!Bfs(s, t, out var parent)) break;
            int cur = t;
            while (cur != s)
            {
                var (prev, idx) = parent[cur];
                Edge e = graph[prev][idx];
                e.Cap -= 1;
                graph[e.To][e.Rev].Cap += 1;
                cur = prev;
            }
            flow++;
        }

        if (flow < 2)
        {
            Console.WriteLine("NO");
            return;
        }

        List<int>[] used = new List<int>[n + 1];
        for (int i = 1; i <= n; i++) used[i] = new List<int>();
        foreach (var (from, to, edge) in originalEdges)
            if (edge.Cap == 0)
                used[from].Add(to);

        List<int> FindPath()
        {
            int[] prev = new int[n + 1];
            for (int i = 1; i <= n; i++) prev[i] = -1;
            Stack<int> stack = new Stack<int>();
            stack.Push(s);
            prev[s] = s;
            while (stack.Count > 0)
            {
                int v = stack.Pop();
                if (v == t) break;
                foreach (int to in used[v])
                {
                    if (prev[to] == -1)
                    {
                        prev[to] = v;
                        stack.Push(to);
                    }
                }
            }
            if (prev[t] == -1) return null;
            List<int> path = new List<int>();
            int cur = t;
            while (cur != s)
            {
                path.Add(cur);
                cur = prev[cur];
            }
            path.Add(s);
            path.Reverse();
            for (int i = 0; i < path.Count - 1; i++)
                used[path[i]].Remove(path[i + 1]);
            return path;
        }

        List<int> path1 = FindPath();
        List<int> path2 = FindPath();
        if (path1 == null || path2 == null)
        {
            Console.WriteLine("NO");
            return;
        }

        Console.WriteLine("YES");
        Console.WriteLine(string.Join(" ", path1));
        Console.WriteLine(string.Join(" ", path2));
    }
}
