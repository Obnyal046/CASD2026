using System;
using System.Linq;

class Program
{
    static void Main()
    {
        var input = Console.ReadLine().Split().Select(long.Parse).ToArray();
        long r1 = input[0], s1 = input[1], p1 = input[2];
        input = Console.ReadLine().Split().Select(long.Parse).ToArray();
        long r2 = input[0], s2 = input[1], p2 = input[2];

        long loss1 = r1 - r2 - p2;
        long loss2 = s1 - s2 - r2;
        long loss3 = p1 - p2 - s2;
        long result = Math.Max(0, Math.Max(loss1, Math.Max(loss2, loss3)));
        Console.WriteLine(result);
    }
}
