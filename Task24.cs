using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


using Task18;   // MyTreeMap
using Task22;   // MyHashMap

namespace Task24
{
    class Program
    {
        static void Main()
        {
            int[] sizes = { 100_000, 1_000_000, 10_000_000 };
            int repeats = 20;

            Console.WriteLine("Размер   Операция   HashMap (мс)   TreeMap (мс)");
            Console.WriteLine("-----------------------------------------------");

            foreach (int n in sizes)
            {
                int[] keys = Enumerable.Range(0, n).ToArray();
                Random rng = new Random();
                for (int i = keys.Length - 1; i > 0; i--)
                {
                    int j = rng.Next(i + 1);
                    int tmp = keys[i]; keys[i] = keys[j]; keys[j] = tmp;
                }

                double hashPut = Measure(repeats, () =>
                {
                    var map = new MyHashMap<int, int>(n);
                    foreach (int k in keys) map.Put(k, k);
                });
                double treePut = Measure(repeats, () =>
                {
                    var map = new MyTreeMap<int, int>();
                    foreach (int k in keys) map.Put(k, k);
                });
                Console.WriteLine($"{n,7}   Put      {hashPut,12:F2}   {treePut,12:F2}");

                var hashFull = new MyHashMap<int, int>(n);
                var treeFull = new MyTreeMap<int, int>();
                foreach (int k in keys)
                {
                    hashFull.Put(k, k);
                    treeFull.Put(k, k);
                }

                double hashGet = Measure(repeats, () =>
                {
                    foreach (int k in keys) hashFull.Get(k);
                });
                double treeGet = Measure(repeats, () =>
                {
                    foreach (int k in keys) treeFull.Get(k);
                });
                Console.WriteLine($"{n,7}   Get      {hashGet,12:F2}   {treeGet,12:F2}");

                double hashRem = Measure(repeats, () =>
                {
                    var map = new MyHashMap<int, int>(n);
                    foreach (int k in keys) map.Put(k, k);
                    foreach (int k in keys) map.Remove(k);
                });
                double treeRem = Measure(repeats, () =>
                {
                    var map = new MyTreeMap<int, int>();
                    foreach (int k in keys) map.Put(k, k);
                    foreach (int k in keys) map.Remove(k);
                });
                Console.WriteLine($"{n,7}   Remove   {hashRem,12:F2}   {treeRem,12:F2}");
            }
        }

        static double Measure(int repeatCount, Action action)
        {
            long totalMs = 0;
            for (int i = 0; i < repeatCount; i++)
            {
                var sw = Stopwatch.StartNew();
                action();
                sw.Stop();
                totalMs += sw.ElapsedMilliseconds;
            }
            return (double)totalMs / repeatCount;
        }
    }
}
