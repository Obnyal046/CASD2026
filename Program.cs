using System;
using System.Collections;
using System.Collections.Generic;

public class MyTreeMap<K, V> where K : IComparable<K>
{
    private class Node
    {
        public K Key;
        public V Value;
        public Node Left;
        public Node Right;
        public Node Parent;

        public Node(K key, V value, Node parent)
        {
            Key = key;
            Value = value;
            Parent = parent;
        }
    }

    private IComparer<K> comparator;
    private Node root;
    private int size;

    public MyTreeMap()
    {
        comparator = Comparer<K>.Default;
        root = null;
        size = 0;
    }

    public MyTreeMap(IComparer<K> comp)
    {
        comparator = comp ?? Comparer<K>.Default;
        root = null;
        size = 0;
    }

    public void Clear()
    {
        root = null;
        size = 0;
    }

    public bool ContainsKey(object key)
    {
        if (key == null || !(key is K))
            return false;
        return GetNode((K)key) != null;
    }

    public bool ContainsValue(object value)
    {
        return ContainsValue(root, value);
    }

    private bool ContainsValue(Node node, object value)
    {
        if (node == null)
            return false;
        if (Equals(node.Value, value))
            return true;
        return ContainsValue(node.Left, value) || ContainsValue(node.Right, value);
    }

    public Set<KeyValuePair<K, V>> EntrySet()
    {
        Set<KeyValuePair<K, V>> set = new Set<KeyValuePair<K, V>>();
        InOrderEntries(root, set);
        return set;
    }

    private void InOrderEntries(Node node, Set<KeyValuePair<K, V>> set)
    {
        if (node == null)
            return;
        InOrderEntries(node.Left, set);
        set.Add(new KeyValuePair<K, V>(node.Key, node.Value));
        InOrderEntries(node.Right, set);
    }

    public V Get(object key)
    {
        if (key == null || !(key is K))
            return default(V);
        Node node = GetNode((K)key);
        return node == null ? default(V) : node.Value;
    }

    public bool IsEmpty()
    {
        return size == 0;
    }

    public Set<K> KeySet()
    {
        Set<K> set = new Set<K>();
        InOrderKeys(root, set);
        return set;
    }

    private void InOrderKeys(Node node, Set<K> set)
    {
        if (node == null)
            return;
        InOrderKeys(node.Left, set);
        set.Add(node.Key);
        InOrderKeys(node.Right, set);
    }

    public V Put(K key, V value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (root == null)
        {
            root = new Node(key, value, null);
            size++;
            return default(V);
        }

        Node current = root;
        Node parent = null;
        int cmp;

        while (current != null)
        {
            parent = current;
            cmp = comparator.Compare(key, current.Key);

            if (cmp < 0)
                current = current.Left;
            else if (cmp > 0)
                current = current.Right;
            else
            {
                V oldValue = current.Value;
                current.Value = value;
                return oldValue;
            }
        }

        cmp = comparator.Compare(key, parent.Key);
        Node newNode = new Node(key, value, parent);
        if (cmp < 0)
            parent.Left = newNode;
        else
            parent.Right = newNode;

        size++;
        return default(V);
    }

    public V Remove(object key)
    {
        if (key == null || !(key is K))
            return default(V);
        
        Node node = GetNode((K)key);
        if (node == null)
            return default(V);

        V oldValue = node.Value;
        DeleteNode(node);
        return oldValue;
    }

    private void DeleteNode(Node node)
    {
        if (node.Left == null && node.Right == null)
        {
            if (node.Parent == null)
                root = null;
            else if (node.Parent.Left == node)
                node.Parent.Left = null;
            else
                node.Parent.Right = null;
            size--;
        }
        else if (node.Left == null || node.Right == null)
        {
            Node child = node.Left ?? node.Right;
            if (node.Parent == null)
                root = child;
            else if (node.Parent.Left == node)
                node.Parent.Left = child;
            else
                node.Parent.Right = child;
            child.Parent = node.Parent;
            size--;
        }
        else
        {
            Node successor = GetMinimum(node.Right);
            node.Key = successor.Key;
            node.Value = successor.Value;
            DeleteNode(successor);
        }
    }

    public int Size()
    {
        return size;
    }

    public K FirstKey()
    {
        if (root == null)
            throw new InvalidOperationException("TreeMap is empty");
        return GetMinimum(root).Key;
    }

    public K LastKey()
    {
        if (root == null)
            throw new InvalidOperationException("TreeMap is empty");
        return GetMaximum(root).Key;
    }

    public MyTreeMap<K, V> HeadMap(K end)
    {
        MyTreeMap<K, V> result = new MyTreeMap<K, V>(comparator);
        AddHeadMap(root, end, result);
        return result;
    }

    private void AddHeadMap(Node node, K end, MyTreeMap<K, V> map)
    {
        if (node == null)
            return;
        if (comparator.Compare(node.Key, end) < 0)
        {
            AddHeadMap(node.Left, end, map);
            map.Put(node.Key, node.Value);
            AddHeadMap(node.Right, end, map);
        }
        else
        {
            AddHeadMap(node.Left, end, map);
        }
    }

    public MyTreeMap<K, V> SubMap(K start, K end)
    {
        if (comparator.Compare(start, end) > 0)
            throw new ArgumentException("start key cannot be greater than end key");
        
        MyTreeMap<K, V> result = new MyTreeMap<K, V>(comparator);
        AddSubMap(root, start, end, result);
        return result;
    }

    private void AddSubMap(Node node, K start, K end, MyTreeMap<K, V> map)
    {
        if (node == null)
            return;
        
        if (comparator.Compare(node.Key, start) >= 0 && comparator.Compare(node.Key, end) < 0)
        {
            AddSubMap(node.Left, start, end, map);
            map.Put(node.Key, node.Value);
            AddSubMap(node.Right, start, end, map);
        }
        else if (comparator.Compare(node.Key, start) < 0)
        {
            AddSubMap(node.Right, start, end, map);
        }
        else
        {
            AddSubMap(node.Left, start, end, map);
        }
    }

    public MyTreeMap<K, V> TailMap(K start)
    {
        MyTreeMap<K, V> result = new MyTreeMap<K, V>(comparator);
        AddTailMap(root, start, result);
        return result;
    }

    private void AddTailMap(Node node, K start, MyTreeMap<K, V> map)
    {
        if (node == null)
            return;
        if (comparator.Compare(node.Key, start) >= 0)
        {
            AddTailMap(node.Left, start, map);
            map.Put(node.Key, node.Value);
            AddTailMap(node.Right, start, map);
        }
        else
        {
            AddTailMap(node.Right, start, map);
        }
    }

    public KeyValuePair<K, V>? LowerEntry(K key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        Node node = GetLowerNode(root, key);
        return node == null ? (KeyValuePair<K, V>?)null : new KeyValuePair<K, V>(node.Key, node.Value);
    }

    private Node GetLowerNode(Node node, K key)
    {
        Node result = null;
        while (node != null)
        {
            if (comparator.Compare(node.Key, key) < 0)
            {
                result = node;
                node = node.Right;
            }
            else
            {
                node = node.Left;
            }
        }
        return result;
    }

    public KeyValuePair<K, V>? FloorEntry(K key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        Node node = GetFloorNode(root, key);
        return node == null ? (KeyValuePair<K, V>?)null : new KeyValuePair<K, V>(node.Key, node.Value);
    }

    private Node GetFloorNode(Node node, K key)
    {
        Node result = null;
        while (node != null)
        {
            int cmp = comparator.Compare(node.Key, key);
            if (cmp <= 0)
            {
                result = node;
                node = node.Right;
            }
            else
            {
                node = node.Left;
            }
        }
        return result;
    }

    public KeyValuePair<K, V>? HigherEntry(K key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        Node node = GetHigherNode(root, key);
        return node == null ? (KeyValuePair<K, V>?)null : new KeyValuePair<K, V>(node.Key, node.Value);
    }

    private Node GetHigherNode(Node node, K key)
    {
        Node result = null;
        while (node != null)
        {
            if (comparator.Compare(node.Key, key) > 0)
            {
                result = node;
                node = node.Left;
            }
            else
            {
                node = node.Right;
            }
        }
        return result;
    }

    public KeyValuePair<K, V>? CeilingEntry(K key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        Node node = GetCeilingNode(root, key);
        return node == null ? (KeyValuePair<K, V>?)null : new KeyValuePair<K, V>(node.Key, node.Value);
    }

    private Node GetCeilingNode(Node node, K key)
    {
        Node result = null;
        while (node != null)
        {
            int cmp = comparator.Compare(node.Key, key);
            if (cmp >= 0)
            {
                result = node;
                node = node.Left;
            }
            else
            {
                node = node.Right;
            }
        }
        return result;
    }

    public K LowerKey(K key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        Node node = GetLowerNode(root, key);
        return node == null ? default(K) : node.Key;
    }

    public K FloorKey(K key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        Node node = GetFloorNode(root, key);
        return node == null ? default(K) : node.Key;
    }

    public K HigherKey(K key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        Node node = GetHigherNode(root, key);
        return node == null ? default(K) : node.Key;
    }

    public K CeilingKey(K key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        Node node = GetCeilingNode(root, key);
        return node == null ? default(K) : node.Key;
    }

    public KeyValuePair<K, V>? PollFirstEntry()
    {
        if (root == null)
            return null;
        Node first = GetMinimum(root);
        KeyValuePair<K, V> result = new KeyValuePair<K, V>(first.Key, first.Value);
        DeleteNode(first);
        return result;
    }

    public KeyValuePair<K, V>? PollLastEntry()
    {
        if (root == null)
            return null;
        Node last = GetMaximum(root);
        KeyValuePair<K, V> result = new KeyValuePair<K, V>(last.Key, last.Value);
        DeleteNode(last);
        return result;
    }

    public KeyValuePair<K, V>? FirstEntry()
    {
        if (root == null)
            return null;
        Node first = GetMinimum(root);
        return new KeyValuePair<K, V>(first.Key, first.Value);
    }

    public KeyValuePair<K, V>? LastEntry()
    {
        if (root == null)
            return null;
        Node last = GetMaximum(root);
        return new KeyValuePair<K, V>(last.Key, last.Value);
    }

    private Node GetNode(K key)
    {
        Node current = root;
        while (current != null)
        {
            int cmp = comparator.Compare(key, current.Key);
            if (cmp < 0)
                current = current.Left;
            else if (cmp > 0)
                current = current.Right;
            else
                return current;
        }
        return null;
    }

    private Node GetMinimum(Node node)
    {
        while (node.Left != null)
            node = node.Left;
        return node;
    }

    private Node GetMaximum(Node node)
    {
        while (node.Right != null)
            node = node.Right;
        return node;
    }
}

public class Set<T> : IEnumerable<T>
{
    private List<T> items = new List<T>();

    public void Add(T item)
    {
        if (!items.Contains(item))
            items.Add(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
