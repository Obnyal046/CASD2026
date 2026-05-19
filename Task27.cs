using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Task27
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
            if (initialCapacity < 0)
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Ёмкость не может быть отрицательной.");
            if (loadFactor <= 0 || float.IsNaN(loadFactor))
                throw new ArgumentOutOfRangeException(nameof(loadFactor), "Коэффициент загрузки должен быть положительным.");
            int capacity = 1;
            while (capacity < initialCapacity) capacity <<= 1;
            this.loadFactor = loadFactor;
            this.threshold = (int)(capacity * loadFactor);
            this.table = new Entry<K, V>[capacity];
            this.size = 0;
        }

        public void Clear()
        {
            for (int i = 0; i < table.Length; i++) table[i] = null;
            size = 0;
        }
        public bool ContainsKey(object key) => GetEntry(key) != null;
        private Entry<K, V> GetEntry(object key)
        {
            if (key == null) return null;
            int index = GetHash(key) & (table.Length - 1);
            var e = table[index];
            while (e != null)
            {
                if (Equals(e.Key, key)) return e;
                e = e.Next;
            }
            return null;
        }
        private int GetHash(object key)
        {
            int h = key.GetHashCode();
            h ^= (h >> 20) ^ (h >> 12);
            h = h ^ (h >> 7) ^ (h >> 4);
            return h;
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
        public List<KeyValuePair<K, V>> EntrySet()
        {
            var result = new List<KeyValuePair<K, V>>();
            for (int i = 0; i < table.Length; i++)
            {
                var e = table[i];
                while (e != null)
                {
                    result.Add(new KeyValuePair<K, V>(e.Key, e.Value));
                    e = e.Next;
                }
            }
            return result;
        }
        public List<K> KeySet()
        {
            var result = new List<K>();
            for (int i = 0; i < table.Length; i++)
            {
                var e = table[i];
                while (e != null)
                {
                    result.Add(e.Key);
                    e = e.Next;
                }
            }
            return result;
        }
        private void Resize()
        {
            int newCapacity = table.Length * 2;
            var old = table;
            table = new Entry<K, V>[newCapacity];
            threshold = (int)(newCapacity * loadFactor);
            size = 0;
            for (int i = 0; i < old.Length; i++)
            {
                var e = old[i];
                while (e != null)
                {
                    var next = e.Next;
                    int idx = GetHash(e.Key) & (newCapacity - 1);
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
            if (a != null) AddAll(a);
        }

        public void Add(E e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e), "Элемент не может быть null.");
            map.Put(e, dummy);
        }
        public void AddAll(E[] a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            foreach (var e in a) Add(e);
        }
        public void Clear() => map.Clear();
        public bool Contains(object o)
        {
            if (o == null) return false;
            if (o is E e) return map.ContainsKey(e);
            return false;
        }
        public bool ContainsAll(E[] a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            foreach (var e in a) if (!Contains(e)) return false;
            return true;
        }
        public bool IsEmpty() => map.IsEmpty();
        public bool Remove(object o)
        {
            if (o == null) return false;
            if (o is E e) return map.Remove(e);
            return false;
        }
        public void RemoveAll(E[] a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            foreach (var e in a) Remove(e);
        }
        public void RetainAll(E[] a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            var keep = new HashSet<E>(a);
            var toRemove = new List<E>();
            foreach (var key in map.KeySet())
                if (!keep.Contains(key)) toRemove.Add(key);
            foreach (var key in toRemove) map.Remove(key);
        }
        public int Size() => map.Size();
        public object[] ToArray()
        {
            var keys = map.KeySet();
            var arr = new object[keys.Count];
            for (int i = 0; i < keys.Count; i++) arr[i] = keys[i];
            return arr;
        }
        public E[] ToArray(E[] a)
        {
            var keys = map.KeySet();
            if (a == null || a.Length < keys.Count) a = new E[keys.Count];
            for (int i = 0; i < keys.Count; i++) a[i] = keys[i];
            if (a.Length > keys.Count) a[keys.Count] = default(E);
            return a;
        }
        public List<E> ToList() => map.KeySet();
    }

    class Task27Program
    {
        static void Main()
        {
            string filePath = "input.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл input.txt не найден.");
                return;
            }

            MyHashSet<string> uniqueWords = new MyHashSet<string>();
            Regex wordRegex = new Regex(@"[A-Za-z]+");

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    MatchCollection matches = wordRegex.Matches(line);
                    foreach (Match m in matches)
                    {
                        string word = m.Value.ToLower();
                        uniqueWords.Add(word);
                    }
                }
            }

            List<string> words = uniqueWords.ToList();
            words.Sort();

            Console.WriteLine("Уникальные слова (регистронезависимо, по алфавиту):");
            foreach (string w in words) Console.WriteLine(w);
        }
    }
}
