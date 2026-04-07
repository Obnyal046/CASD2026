using System;
using System.Collections.Generic;
using System.Linq;

namespace Task20
{
    public class Graph
    {
        public int VerticesCount { get; }
        public List<int>[] AdjList { get; }
        public bool IsDirected { get; }

        public Graph(int vertices, bool directed = false)
        {
            VerticesCount = vertices;
            IsDirected = directed;
            AdjList = new List<int>[vertices];
            for (int i = 0; i < vertices; i++)
                AdjList[i] = new List<int>();
        }

        public void AddEdge(int from, int to)
        {
            AdjList[from].Add(to);
            if (!IsDirected)
                AdjList[to].Add(from);
        }

    }

    public static class TransitiveClosureDfs
    {
        public static bool[,] Compute(Graph graph)
        {
            int n = graph.VerticesCount;
            bool[,] reach = new bool[n, n];
            bool[] visited = new bool[n];
            for (int u = 0; u < n; u++)
            {
                Array.Clear(visited, 0, visited.Length);
                Dfs(graph, u, u, visited, reach);
            }
            return reach;
        }

        private static void Dfs(Graph graph, int start, int current, bool[] visited, bool[,] reach)
        {
            visited[current] = true;
            reach[start, current] = true;
            foreach (int neighbor in graph.AdjList[current])
            {
                if (!visited[neighbor])
                    Dfs(graph, start, neighbor, visited, reach);
            }
        }
    }

    public static class EdmondsKarp
    {
        public static int MaxFlow(Graph graph, int source, int sink, int[,] capacity = null)
        {
            int n = graph.VerticesCount;
            if (capacity == null)
            {
                capacity = new int[n, n];
                for (int u = 0; u < n; u++)
                    foreach (int v in graph.AdjList[u])
                        capacity[u, v] = 1;
            }

            int[,] flow = new int[n, n];
            int maxFlow = 0;

            while (true)
            {
                int[] parent = new int[n];
                for (int i = 0; i < n; i++) parent[i] = -1;
                parent[source] = source;
                Queue<int> queue = new Queue<int>();
                queue.Enqueue(source);
                bool pathFound = false;

                while (queue.Count > 0 && !pathFound)
                {
                    int u = queue.Dequeue();
                    foreach (int v in graph.AdjList[u])
                    {
                        if (parent[v] == -1 && capacity[u, v] - flow[u, v] > 0)
                        {
                            parent[v] = u;
                            if (v == sink)
                            {
                                pathFound = true;
                                break;
                            }
                            queue.Enqueue(v);
                        }
                    }
                }

                if (!pathFound) break;

                int augment = int.MaxValue;
                for (int v = sink; v != source; v = parent[v])
                {
                    int u = parent[v];
                    int residual = capacity[u, v] - flow[u, v];
                    if (residual < augment) augment = residual;
                }

                for (int v = sink; v != source; v = parent[v])
                {
                    int u = parent[v];
                    flow[u, v] += augment;
                    flow[v, u] -= augment;
                }
                maxFlow += augment;
            }
            return maxFlow;
        }
    }

    public static class ArticulationPointsFinder
    {
        public static List<int> Find(Graph graph)
        {
            int n = graph.VerticesCount;
            bool[] visited = new bool[n];
            int[] discoveryTime = new int[n];
            int[] low = new int[n];
            int[] parent = new int[n];
            bool[] articulation = new bool[n];
            int time = 0;

            for (int i = 0; i < n; i++) parent[i] = -1;

            for (int i = 0; i < n; i++)
                if (!visited[i])
                    Dfs(i, visited, discoveryTime, low, parent, articulation, ref time, graph);

            List<int> result = new List<int>();
            for (int i = 0; i < n; i++)
                if (articulation[i]) result.Add(i);
            return result;
        }

        private static void Dfs(int u, bool[] visited, int[] disc, int[] low, int[] parent, bool[] articulation, ref int time, Graph graph)
        {
            visited[u] = true;
            disc[u] = low[u] = ++time;
            int childrenCount = 0;

            foreach (int v in graph.AdjList[u])
            {
                if (!visited[v])
                {
                    childrenCount++;
                    parent[v] = u;
                    Dfs(v, visited, disc, low, parent, articulation, ref time, graph);
                    low[u] = Math.Min(low[u], low[v]);

                    if (parent[u] == -1 && childrenCount > 1)
                        articulation[u] = true;

                    if (parent[u] != -1 && low[v] >= disc[u])
                        articulation[u] = true;
                }
                else if (v != parent[u])
                {
                    low[u] = Math.Min(low[u], disc[v]);
                }
            }
        }
    }
}
