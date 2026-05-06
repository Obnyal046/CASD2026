using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Task23
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
            if (initialCapacity < 0) throw new ArgumentOutOfRangeException("initialCapacity");
            if (loadFactor <= 0 || float.IsNaN(loadFactor)) throw new ArgumentOutOfRangeException("loadFactor");

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

        public bool ContainsValue(object value)
        {
            for (int i = 0; i < table.Length; i++)
            {
                var entry = table[i];
                while (entry != null)
                {
                    if (Equals(entry.Value, value)) return true;
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
                var entry = table[i];
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
            var entry = GetEntry(key);
            return entry != null ? entry.Value : default(V);
        }

        public bool IsEmpty() => size == 0;

        public System.Collections.Generic.List<K> KeySet()
        {
            var result = new System.Collections.Generic.List<K>();
            for (int i = 0; i < table.Length; i++)
            {
                var entry = table[i];
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

            var current = table[index];
            while (current != null)
            {
                if (Equals(current.Key, key))
                {
                    current.Value = value;
                    return;
                }
                current = current.Next;
            }

            var newEntry = new Entry<K, V>(key, value);
            newEntry.Next = table[index];
            table[index] = newEntry;
            size++;

            if (size > threshold) Resize();
        }

        public bool Remove(object key)
        {
            if (key == null) return false;

            int hash = GetHash(key);
            int index = hash & (table.Length - 1);

            Entry<K, V> prev = null;
            var current = table[index];
            while (current != null)
            {
                if (Equals(current.Key, key))
                {
                    if (prev == null) table[index] = current.Next;
                    else prev.Next = current.Next;
                    size--;
                    return true;
                }
                prev = current;
                current = current.Next;
            }
            return false;
        }

        public int Size() => size;

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
            int index = GetHash(key) & (table.Length - 1);
            var entry = table[index];
            while (entry != null)
            {
                if (Equals(entry.Key, key)) return entry;
                entry = entry.Next;
            }
            return null;
        }

        private void Resize()
        {
            int newCapacity = table.Length * 2;
            var oldTable = table;
            table = new Entry<K, V>[newCapacity];
            threshold = (int)(newCapacity * loadFactor);
            size = 0;

            for (int i = 0; i < oldTable.Length; i++)
            {
                var entry = oldTable[i];
                while (entry != null)
                {
                    var next = entry.Next;
                    int newIndex = GetHash(entry.Key) & (newCapacity - 1);
                    entry.Next = table[newIndex];
                    table[newIndex] = entry;
                    size++;
                    entry = next;
                }
            }
        }
    }

    public enum DataType
    {
        Int,
        Float,
        Double,
        Unknown
    }

    class Program
    {
        static void Main()
        {
            MyHashMap<string, (DataType type, string value)> variables = new MyHashMap<string, (DataType, string)>();

            string inputFile = "input.txt";
            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Файл input.txt не найден.");
                return;
            }

            Regex definitionRegex = new Regex(
                @"(?<type>int|float|double)\s+(?<name>[a-zA-Z_][a-zA-Z0-9_]*)\s*=\s*(?<value>\d+)\s*;",
                RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace
            );

            string fileContent = File.ReadAllText(inputFile);

            MatchCollection matches = definitionRegex.Matches(fileContent);
            if (matches.Count == 0)
            {
                Console.WriteLine("Не найдено ни одного корректного определения.");
            }

            System.Collections.Generic.List<string> duplicateWarnings = new System.Collections.Generic.List<string>();

            foreach (Match m in matches)
            {
                string typeStr = m.Groups["type"].Value.ToLower();
                string varName = m.Groups["name"].Value;
                string valueStr = m.Groups["value"].Value;

                DataType type;
                switch (typeStr)
                {
                    case "int": type = DataType.Int; break;
                    case "float": type = DataType.Float; break;
                    case "double": type = DataType.Double; break;
                    default: type = DataType.Unknown; break;
                }

                if (type == DataType.Unknown)
                {
                    Console.WriteLine($"Некорректный тип '{typeStr}' для переменной '{varName}'. Определение пропущено.");
                    continue;
                }

                if (variables.ContainsKey(varName))
                {
                    duplicateWarnings.Add($"Переопределение переменной '{varName}' (первое объявление оставлено).");
                    continue;
                }

                variables.Put(varName, (type, valueStr));
            }

            string outputFile = "output.txt";
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                foreach (var entry in variables.EntrySet())
                {
                    string typeName = "";
                    switch (entry.Value.type)
                    {
                        case DataType.Int: typeName = "int"; break;
                        case DataType.Float: typeName = "float"; break;
                        case DataType.Double: typeName = "double"; break;
                    }
                    writer.WriteLine($"{typeName} => {entry.Key}({entry.Value.value})");
                }
            }

            if (duplicateWarnings.Count > 0)
            {
                Console.WriteLine("\nОбнаружены переопределения:");
                foreach (var w in duplicateWarnings)
                    Console.WriteLine(w);
            }
            Console.WriteLine($"\nОбработка завершена. Результат сохранён в {outputFile}");
        }
    }
}
