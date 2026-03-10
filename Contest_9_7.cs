using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    static void Main()
    {
        int n = int.Parse(Console.ReadLine());
        string[] patterns = new string[n];
        int totalLen = 0;
        for (int i = 0; i < n; i++)
        {
            patterns[i] = Console.ReadLine();
            totalLen += patterns[i].Length;
        }
        string text = Console.ReadLine();

        const int ALPH = 26;
        int maxNodes = totalLen + 1;
        int[] next = new int[maxNodes * ALPH];
        for (int i = 0; i < next.Length; i++) next[i] = -1;

        List<int>[] output = new List<int>[maxNodes];
        int nodeCount = 1;

        for (int i = 0; i < n; i++)
        {
            string s = patterns[i];
            int cur = 0;
            for (int j = 0; j < s.Length; j++)
            {
                int c = s[j] - 'a';
                int idx = cur * ALPH + c;
                if (next[idx] == -1)
                {
                    next[idx] = nodeCount;
                    nodeCount++;
                }
                cur = next[idx];
            }
            if (output[cur] == null) output[cur] = new List<int>();
            output[cur].Add(i);
        }

        int[] fail = new int[nodeCount];
        List<int> order = new List<int>();
        Queue<int> queue = new Queue<int>();

        for (int c = 0; c < ALPH; c++)
        {
            int idx = 0 * ALPH + c;
            if (next[idx] != -1)
            {
                int child = next[idx];
                fail[child] = 0;
                queue.Enqueue(child);
            }
        }

        while (queue.Count > 0)
        {
            int u = queue.Dequeue();
            order.Add(u);
            for (int c = 0; c < ALPH; c++)
            {
                int idx = u * ALPH + c;
                if (next[idx] != -1)
                {
                    int v = next[idx];
                    int f = fail[u];
                    while (f != 0 && next[f * ALPH + c] == -1)
                        f = fail[f];
                    if (next[f * ALPH + c] != -1)
                        fail[v] = next[f * ALPH + c];
                    else
                        fail[v] = 0;
                    queue.Enqueue(v);
                }
            }
        }

        bool[] visited = new bool[nodeCount];
        int curNode = 0;
        for (int i = 0; i < text.Length; i++)
        {
            int c = text[i] - 'a';
            while (curNode != 0 && next[curNode * ALPH + c] == -1)
                curNode = fail[curNode];
            if (next[curNode * ALPH + c] != -1)
                curNode = next[curNode * ALPH + c];
            else
                curNode = 0;
            visited[curNode] = true;
        }

        for (int i = order.Count - 1; i >= 0; i--)
        {
            int u = order[i];
            if (visited[u])
            {
                int f = fail[u];
                visited[f] = true;
            }
        }

        bool[] ans = new bool[n];
        for (int u = 0; u < nodeCount; u++)
        {
            if (output[u] != null && visited[u])
            {
                foreach (int idx in output[u])
                    ans[idx] = true;
            }
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < n; i++)
            sb.AppendLine(ans[i] ? "YES" : "NO");
        Console.Write(sb.ToString());
    }
}
