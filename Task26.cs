using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Task26
{
    public class Entry<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
        public Entry<K, V> Next { get; set; }
        public Entry(K key, V value) { Key = key; Value = value; Next = null; }
    }

    public class MyHashMap<K, V>
    {
        private Entry<K, V>[] table;
        private int size;
        private float loadFactor;
        private int threshold;
        private const int DEFAULT_CAPACITY = 16;
        private const float DEFAULT_LOAD_FACTOR = 0.75f;

        public MyHashMap() : this(DEFAULT_CAPACITY, DEFAULT_LOAD_FACTOR) { }
        public MyHashMap(int initialCapacity) : this(initialCapacity, DEFAULT_LOAD_FACTOR) { }
        public MyHashMap(int initialCapacity, float loadFactor)
        {
            if (initialCapacity < 0) throw new ArgumentOutOfRangeException(nameof(initialCapacity));
            if (loadFactor <= 0 || float.IsNaN(loadFactor)) throw new ArgumentOutOfRangeException(nameof(loadFactor));
            int cap = 1;
            while (cap < initialCapacity) cap <<= 1;
            this.loadFactor = loadFactor;
            this.threshold = (int)(cap * loadFactor);
            this.table = new Entry<K, V>[cap];
            this.size = 0;
        }
        public void Clear()
        {
            for (int i = 0; i < table.Length; i++) table[i] = null;
            size = 0;
        }
        public bool ContainsKey(object key) => GetEntry(key) != null;
        private int GetHash(object key)
        {
            int h = key.GetHashCode();
            h ^= (h >> 20) ^ (h >> 12);
            h = h ^ (h >> 7) ^ (h >> 4);
            return h;
        }
        private Entry<K, V> GetEntry(object key)
        {
            if (key == null) return null;
            int idx = GetHash(key) & (table.Length - 1);
            var e = table[idx];
            while (e != null)
            {
                if (Equals(e.Key, key)) return e;
                e = e.Next;
            }
            return null;
        }
        public void Put(K key, V value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            int idx = GetHash(key) & (table.Length - 1);
            var cur = table[idx];
            while (cur != null)
            {
                if (Equals(cur.Key, key))
                {
                    cur.Value = value;
                    return;
                }
                cur = cur.Next;
            }
            var newNode = new Entry<K, V>(key, value) { Next = table[idx] };
            table[idx] = newNode;
            size++;
            if (size > threshold) Resize();
        }
        public V Get(object key)
        {
            var e = GetEntry(key);
            return e != null ? e.Value : default(V);
        }
        public bool Remove(object key)
        {
            if (key == null) return false;
            int idx = GetHash(key) & (table.Length - 1);
            Entry<K, V> prev = null, cur = table[idx];
            while (cur != null)
            {
                if (Equals(cur.Key, key))
                {
                    if (prev == null) table[idx] = cur.Next;
                    else prev.Next = cur.Next;
                    size--;
                    return true;
                }
                prev = cur;
                cur = cur.Next;
            }
            return false;
        }
        public int Size() => size;
        public bool IsEmpty() => size == 0;
        public List<K> KeySet()
        {
            var list = new List<K>();
            for (int i = 0; i < table.Length; i++)
            {
                var e = table[i];
                while (e != null)
                {
                    list.Add(e.Key);
                    e = e.Next;
                }
            }
            return list;
        }
        private void Resize()
        {
            int newCap = table.Length * 2;
            var old = table;
            table = new Entry<K, V>[newCap];
            threshold = (int)(newCap * loadFactor);
            size = 0;
            for (int i = 0; i < old.Length; i++)
            {
                var e = old[i];
                while (e != null)
                {
                    var next = e.Next;
                    int idx = GetHash(e.Key) & (newCap - 1);
                    e.Next = table[idx];
                    table[idx] = e;
                    size++;
                    e = next;
                }
            }
        }
    }

    public class MyHashSet<E>
    {
        private MyHashMap<E, object> map;
        private static readonly object dummy = new object();
        public MyHashSet() : this(16, 0.75f) { }
        public MyHashSet(int initialCapacity) : this(initialCapacity, 0.75f) { }
        public MyHashSet(int initialCapacity, float loadFactor)
        {
            map = new MyHashMap<E, object>(initialCapacity, loadFactor);
        }
        public MyHashSet(E[] a) : this()
        {
            if (a != null) foreach (var e in a) Add(e);
        }
        public void Add(E e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            map.Put(e, dummy);
        }
        public void Clear() => map.Clear();
        public bool Contains(object o)
        {
            if (o is E e) return map.ContainsKey(e);
            return false;
        }
        public bool IsEmpty() => map.IsEmpty();
        public bool Remove(object o)
        {
            if (o is E e) return map.Remove(e);
            return false;
        }
        public int Size() => map.Size();
        public List<E> ToList() => map.KeySet();
    }

    class CustomString : IComparable<CustomString>
    {
        private string original;
        private List<int> sortedLengths;
        public CustomString(string str, List<int> lens)
        {
            original = str;
            sortedLengths = lens;
        }
        public int CompareTo(CustomString other)
        {
            if (other == null) return 1;
            int minLen = Math.Min(sortedLengths.Count, other.sortedLengths.Count);
            for (int i = 0; i < minLen; i++)
            {
                int cmp = sortedLengths[i].CompareTo(other.sortedLengths[i]);
                if (cmp != 0) return cmp;
            }
            return sortedLengths.Count.CompareTo(other.sortedLengths.Count);
        }
        public override bool Equals(object obj)
        {
            return CompareTo(obj as CustomString) == 0;
        }
        public override int GetHashCode()
        {
            int hash = 17;
            foreach (int len in sortedLengths)
                hash = hash * 31 + len;
            return hash;
        }
        public override string ToString() => original;
    }

    class Program
    {
        static void Main()
        {
            string filePath = "input.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл input.txt не найден.");
                return;
            }
            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length == 0)
            {
                Console.WriteLine("Файл пуст.");
                return;
            }

            var set = new MyHashSet<CustomString>();

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                string[] words = trimmed.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                List<int> lengths = new List<int>();
                foreach (string w in words) lengths.Add(w.Length);
                lengths.Sort();
                set.Add(new CustomString(line, lengths));
            }

            Console.WriteLine("Уникальные строки (по правилу сравнения):");
            foreach (var item in set.ToList())
            {
                Console.WriteLine(item);
            }
        }
    }
}
