using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Task22
{
    public class Entry<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
        public Entry<K, V> Next { get; set; }

        public Entry(K key, V value)
        {
            Key = key;
            Value = value;
            Next = null;
        }
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
                throw new ArgumentOutOfRangeException("initialCapacity", "Начальная ёмкость не может быть отрицательной.");
            if (loadFactor <= 0 || float.IsNaN(loadFactor))
                throw new ArgumentOutOfRangeException("loadFactor", "Коэффициент загрузки должен быть положительным числом.");

            int capacity = 1;
            while (capacity < initialCapacity)
                capacity <<= 1;

            this.loadFactor = loadFactor;
            this.threshold = (int)(capacity * loadFactor);
            this.table = new Entry<K, V>[capacity];
            this.size = 0;
        }

        public void Clear()
        {
            for (int i = 0; i < table.Length; i++)
                table[i] = null;
            size = 0;
        }

        public bool ContainsKey(object key)
        {
            return GetEntry(key) != null;
        }

        public bool ContainsValue(object value)
        {
            for (int i = 0; i < table.Length; i++)
            {
                Entry<K, V> entry = table[i];
                while (entry != null)
                {
                    if (Equals(entry.Value, value))
                        return true;
                    entry = entry.Next;
                }
            }
            return false;
        }

        public System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<K, V>> EntrySet()
        {
            var result = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<K, V>>();
            for (int i = 0; i < table.Length; i++)
            {
                Entry<K, V> entry = table[i];
                while (entry != null)
                {
                    result.Add(new System.Collections.Generic.KeyValuePair<K, V>(entry.Key, entry.Value));
                    entry = entry.Next;
                }
            }
            return result;
        }

        public V Get(object key)
        {
            Entry<K, V> entry = GetEntry(key);
            return entry != null ? entry.Value : default(V);
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public System.Collections.Generic.List<K> KeySet()
        {
            var result = new System.Collections.Generic.List<K>();
            for (int i = 0; i < table.Length; i++)
            {
                Entry<K, V> entry = table[i];
                while (entry != null)
                {
                    result.Add(entry.Key);
                    entry = entry.Next;
                }
            }
            return result;
        }

        public void Put(K key, V value)
        {
            if (key == null) throw new ArgumentNullException("key");

            int hash = GetHash(key);
            int index = hash & (table.Length - 1);

            Entry<K, V> current = table[index];
            while (current != null)
            {
                if (Equals(current.Key, key))
                {
                    current.Value = value;
                    return;
                }
                current = current.Next;
            }

            Entry<K, V> newEntry = new Entry<K, V>(key, value);
            newEntry.Next = table[index];
            table[index] = newEntry;
            size++;

            if (size > threshold)
                Resize();
        }

        public bool Remove(object key)
        {
            if (key == null) return false;

            int hash = GetHash(key);
            int index = hash & (table.Length - 1);

            Entry<K, V> prev = null;
            Entry<K, V> current = table[index];
            while (current != null)
            {
                if (Equals(current.Key, key))
                {
                    if (prev == null)
                        table[index] = current.Next;
                    else
                        prev.Next = current.Next;
                    size--;
                    return true;
                }
                prev = current;
                current = current.Next;
            }
            return false;
        }

        public int Size()
        {
            return size;
        }

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
            int hash = GetHash(key);
            int index = hash & (table.Length - 1);
            Entry<K, V> entry = table[index];
            while (entry != null)
            {
                if (Equals(entry.Key, key))
                    return entry;
                entry = entry.Next;
            }
            return null;
        }

        private void Resize()
        {
            int newCapacity = table.Length * 2;
            Entry<K, V>[] oldTable = table;
            table = new Entry<K, V>[newCapacity];
            threshold = (int)(newCapacity * loadFactor);
            size = 0;

            for (int i = 0; i < oldTable.Length; i++)
            {
                Entry<K, V> entry = oldTable[i];
                while (entry != null)
                {
                    Entry<K, V> next = entry.Next;
                    int newIndex = GetHash(entry.Key) & (newCapacity - 1);
                    entry.Next = table[newIndex];
                    table[newIndex] = entry;
                    size++;
                    entry = next;
                }
            }
        }
    }

    class Program
    {
        static void Main()
        {
            MyHashMap<string, int> tagCount = new MyHashMap<string, int>();

            string filePath = "input.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл input.txt не найден.");
                return;
            }

            Regex tagRegex = new Regex(@"<\/?[A-Za-z][A-Za-z0-9]*>");

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    MatchCollection matches = tagRegex.Matches(line);
                    foreach (Match match in matches)
                    {
                        string tag = match.Value;
                        string tagName = tag.Trim('<', '>').TrimStart('/').ToLower();

                        if (tagCount.ContainsKey(tagName))
                        {
                            int current = tagCount.Get(tagName);
                            tagCount.Put(tagName, current + 1);
                        }
                        else
                        {
                            tagCount.Put(tagName, 1);
                        }
                    }
                }
            }

            Console.WriteLine("Статистика тегов:");
            foreach (var entry in tagCount.EntrySet())
            {
                Console.WriteLine($"{entry.Key}: {entry.Value}");
            }
        }
    }
}
