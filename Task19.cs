using System;
using System.Collections;
using System.Collections.Generic;

namespace Task19
{
    public class RbNode<K, V>
    {
        public K Key;
        public V Value;
        public RbNode<K, V> Left;
        public RbNode<K, V> Right;
        public RbNode<K, V> Parent;
        public bool IsRed;

        public RbNode(K key, V value)
        {
            Key = key;
            Value = value;
            IsRed = true;
        }
    }

    public class MyTreeMap<K, V> where K : IComparable<K>
    {
        private IComparer<K> comparator;
        private RbNode<K, V> root;
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

        public IComparer<K> Comparator => comparator;

        public int Size() => size;
        public bool IsEmpty() => size == 0;
        public void Clear() { root = null; size = 0; }

        public bool ContainsKey(K key) => FindNode(key) != null;

        private RbNode<K, V> FindNode(K key)
        {
            var cur = root;
            while (cur != null)
            {
                int cmp = comparator.Compare(key, cur.Key);
                if (cmp == 0) return cur;
                cur = cmp < 0 ? cur.Left : cur.Right;
            }
            return null;
        }

        public void Put(K key, V value)
        {
            if (root == null)
            {
                root = new RbNode<K, V>(key, value);
                root.IsRed = false;
                size = 1;
                return;
            }

            RbNode<K, V> cur = root;
            RbNode<K, V> parent = null;
            int cmp = 0;
            while (cur != null)
            {
                parent = cur;
                cmp = comparator.Compare(key, cur.Key);
                if (cmp < 0) cur = cur.Left;
                else if (cmp > 0) cur = cur.Right;
                else
                {
                    cur.Value = value;
                    return;
                }
            }

            var newNode = new RbNode<K, V>(key, value) { Parent = parent };
            if (cmp < 0) parent.Left = newNode;
            else parent.Right = newNode;
            size++;
            FixAfterInsert(newNode);
        }

        private void RotateLeft(RbNode<K, V> p)
        {
            if (p == null) return;
            var r = p.Right;
            p.Right = r.Left;
            if (r.Left != null) r.Left.Parent = p;
            r.Parent = p.Parent;
            if (p.Parent == null) root = r;
            else if (p.Parent.Left == p) p.Parent.Left = r;
            else p.Parent.Right = r;
            r.Left = p;
            p.Parent = r;
        }

        private void RotateRight(RbNode<K, V> p)
        {
            if (p == null) return;
            var l = p.Left;
            p.Left = l.Right;
            if (l.Right != null) l.Right.Parent = p;
            l.Parent = p.Parent;
            if (p.Parent == null) root = l;
            else if (p.Parent.Right == p) p.Parent.Right = l;
            else p.Parent.Left = l;
            l.Right = p;
            p.Parent = l;
        }

        private void FixAfterInsert(RbNode<K, V> x)
        {
            x.IsRed = true;
            while (x != null && x != root && x.Parent != null && x.Parent.IsRed)
            {
                if (x.Parent == x.Parent.Parent?.Left)
                {
                    var y = x.Parent.Parent.Right;
                    if (y != null && y.IsRed)
                    {
                        x.Parent.IsRed = false;
                        y.IsRed = false;
                        x.Parent.Parent.IsRed = true;
                        x = x.Parent.Parent;
                    }
                    else
                    {
                        if (x == x.Parent.Right)
                        {
                            x = x.Parent;
                            RotateLeft(x);
                        }
                        x.Parent.IsRed = false;
                        x.Parent.Parent.IsRed = true;
                        RotateRight(x.Parent.Parent);
                    }
                }
                else
                {
                    var y = x.Parent.Parent?.Left;
                    if (y != null && y.IsRed)
                    {
                        x.Parent.IsRed = false;
                        y.IsRed = false;
                        x.Parent.Parent.IsRed = true;
                        x = x.Parent.Parent;
                    }
                    else
                    {
                        if (x == x.Parent.Left)
                        {
                            x = x.Parent;
                            RotateRight(x);
                        }
                        x.Parent.IsRed = false;
                        x.Parent.Parent.IsRed = true;
                        RotateLeft(x.Parent.Parent);
                    }
                }
            }
            root.IsRed = false;
        }

        public bool Remove(K key)
        {
            var node = FindNode(key);
            if (node == null) return false;
            DeleteNode(node);
            size--;
            return true;
        }

        private void DeleteNode(RbNode<K, V> p)
        {
            if (p.Left != null && p.Right != null)
            {
                var s = GetMin(p.Right);
                p.Key = s.Key;
                p.Value = s.Value;
                p = s;
            }

            var child = p.Left ?? p.Right;
            if (child != null)
            {
                child.Parent = p.Parent;
                if (p.Parent == null) root = child;
                else if (p == p.Parent.Left) p.Parent.Left = child;
                else p.Parent.Right = child;

                p.Left = p.Right = p.Parent = null;
                if (!p.IsRed) FixAfterDelete(child);
            }
            else if (p.Parent == null)
            {
                root = null;
            }
            else
            {
                if (!p.IsRed) FixAfterDelete(p);
                if (p.Parent != null)
                {
                    if (p == p.Parent.Left) p.Parent.Left = null;
                    else p.Parent.Right = null;
                    p.Parent = null;
                }
            }
        }

        private void FixAfterDelete(RbNode<K, V> x)
        {
            while (x != root && (x == null || !x.IsRed))
            {
                if (x == (x?.Parent?.Left))
                {
                    var sib = x.Parent.Right;
                    if (sib != null && sib.IsRed)
                    {
                        sib.IsRed = false;
                        x.Parent.IsRed = true;
                        RotateLeft(x.Parent);
                        sib = x.Parent.Right;
                    }
                    if ((sib.Left == null || !sib.Left.IsRed) && (sib.Right == null || !sib.Right.IsRed))
                    {
                        sib.IsRed = true;
                        x = x.Parent;
                    }
                    else
                    {
                        if (sib.Right == null || !sib.Right.IsRed)
                        {
                            if (sib.Left != null) sib.Left.IsRed = false;
                            sib.IsRed = true;
                            RotateRight(sib);
                            sib = x.Parent.Right;
                        }
                        sib.IsRed = x.Parent.IsRed;
                        x.Parent.IsRed = false;
                        if (sib.Right != null) sib.Right.IsRed = false;
                        RotateLeft(x.Parent);
                        x = root;
                    }
                }
                else
                {
                    var sib = x.Parent.Left;
                    if (sib != null && sib.IsRed)
                    {
                        sib.IsRed = false;
                        x.Parent.IsRed = true;
                        RotateRight(x.Parent);
                        sib = x.Parent.Left;
                    }
                    if ((sib.Right == null || !sib.Right.IsRed) && (sib.Left == null || !sib.Left.IsRed))
                    {
                        sib.IsRed = true;
                        x = x.Parent;
                    }
                    else
                    {
                        if (sib.Left == null || !sib.Left.IsRed)
                        {
                            if (sib.Right != null) sib.Right.IsRed = false;
                            sib.IsRed = true;
                            RotateLeft(sib);
                            sib = x.Parent.Left;
                        }
                        sib.IsRed = x.Parent.IsRed;
                        x.Parent.IsRed = false;
                        if (sib.Left != null) sib.Left.IsRed = false;
                        RotateRight(x.Parent);
                        x = root;
                    }
                }
            }
            if (x != null) x.IsRed = false;
        }

        private RbNode<K, V> GetMin(RbNode<K, V> node)
        {
            while (node?.Left != null) node = node.Left;
            return node;
        }

        private RbNode<K, V> GetMax(RbNode<K, V> node)
        {
            while (node?.Right != null) node = node.Right;
            return node;
        }

        public K FirstKey()
        {
            if (root == null) throw new InvalidOperationException("Map is empty");
            return GetMin(root).Key;
        }

        public K LastKey()
        {
            if (root == null) throw new InvalidOperationException("Map is empty");
            return GetMax(root).Key;
        }

        public K LowerKey(K key)
        {
            var e = LowerEntry(key);
            return e == null ? default : e.Key;
        }

        public RbNode<K, V> LowerEntry(K key)
        {
            RbNode<K, V> cur = root, cand = null;
            while (cur != null)
            {
                if (comparator.Compare(cur.Key, key) < 0) { cand = cur; cur = cur.Right; }
                else cur = cur.Left;
            }
            return cand;
        }

        public K FloorKey(K key)
        {
            var e = FloorEntry(key);
            return e == null ? default : e.Key;
        }

        public RbNode<K, V> FloorEntry(K key)
        {
            RbNode<K, V> cur = root, cand = null;
            while (cur != null)
            {
                int cmp = comparator.Compare(cur.Key, key);
                if (cmp == 0) return cur;
                if (cmp < 0) { cand = cur; cur = cur.Right; }
                else cur = cur.Left;
            }
            return cand;
        }

        public K HigherKey(K key)
        {
            var e = HigherEntry(key);
            return e == null ? default : e.Key;
        }

        public RbNode<K, V> HigherEntry(K key)
        {
            RbNode<K, V> cur = root, cand = null;
            while (cur != null)
            {
                if (comparator.Compare(cur.Key, key) > 0) { cand = cur; cur = cur.Left; }
                else cur = cur.Right;
            }
            return cand;
        }

        public K CeilingKey(K key)
        {
            var e = CeilingEntry(key);
            return e == null ? default : e.Key;
        }

        public RbNode<K, V> CeilingEntry(K key)
        {
            RbNode<K, V> cur = root, cand = null;
            while (cur != null)
            {
                int cmp = comparator.Compare(cur.Key, key);
                if (cmp == 0) return cur;
                if (cmp > 0) { cand = cur; cur = cur.Left; }
                else cur = cur.Right;
            }
            return cand;
        }

        public RbNode<K, V> PollFirstEntry()
        {
            if (root == null) return null;
            var min = GetMin(root);
            Remove(min.Key);
            return min;
        }

        public RbNode<K, V> PollLastEntry()
        {
            if (root == null) return null;
            var max = GetMax(root);
            Remove(max.Key);
            return max;
        }

        public List<K> KeySet()
        {
            var list = new List<K>();
            Inorder(root, list);
            return list;
        }

        private void Inorder(RbNode<K, V> node, List<K> list)
        {
            if (node == null) return;
            Inorder(node.Left, list);
            list.Add(node.Key);
            Inorder(node.Right, list);
        }

        public MyTreeMap<K, V> HeadMap(K end)
        {
            var result = new MyTreeMap<K, V>(comparator);
            AddHead(root, end, result);
            return result;
        }

        private void AddHead(RbNode<K, V> node, K end, MyTreeMap<K, V> map)
        {
            if (node == null) return;
            AddHead(node.Left, end, map);
            if (comparator.Compare(node.Key, end) < 0)
            {
                map.Put(node.Key, node.Value);
                AddHead(node.Right, end, map);
            }
        }

        public MyTreeMap<K, V> SubMap(K start, K end)
        {
            var result = new MyTreeMap<K, V>(comparator);
            AddSub(root, start, end, result);
            return result;
        }

        private void AddSub(RbNode<K, V> node, K start, K end, MyTreeMap<K, V> map)
        {
            if (node == null) return;
            if (comparator.Compare(node.Key, start) >= 0)
                AddSub(node.Left, start, end, map);
            if (comparator.Compare(node.Key, start) >= 0 && comparator.Compare(node.Key, end) < 0)
                map.Put(node.Key, node.Value);
            if (comparator.Compare(node.Key, end) < 0)
                AddSub(node.Right, start, end, map);
        }

        public MyTreeMap<K, V> TailMap(K start)
        {
            var result = new MyTreeMap<K, V>(comparator);
            AddTail(root, start, result);
            return result;
        }

        private void AddTail(RbNode<K, V> node, K start, MyTreeMap<K, V> map)
        {
            if (node == null) return;
            AddTail(node.Left, start, map);
            if (comparator.Compare(node.Key, start) >= 0)
            {
                map.Put(node.Key, node.Value);
                AddTail(node.Right, start, map);
            }
            else
            {
                AddTail(node.Right, start, map);
            }
        }
    }

    public class MyTreeSet<E> : IEnumerable<E> where E : IComparable<E>
    {
        private readonly MyTreeMap<E, object> map;
        private readonly IComparer<E> comparator;
        private static readonly object dummy = new object();

        public MyTreeSet() : this(Comparer<E>.Default) { }

        public MyTreeSet(IComparer<E> comp)
        {
            comparator = comp ?? Comparer<E>.Default;
            map = new MyTreeMap<E, object>(comparator);
        }

        public MyTreeSet(MyTreeMap<E, object> m)
        {
            map = m ?? throw new ArgumentNullException(nameof(m));
            comparator = map.Comparator;
        }

        public MyTreeSet(E[] a) : this()
        {
            if (a != null) AddAll(a);
        }

        public MyTreeSet(SortedSet<E> s) : this()
        {
            if (s != null)
                foreach (var e in s) Add(e);
        }

        public void Add(E e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            map.Put(e, dummy);
        }

        public void AddAll(E[] a)
        {
            if (a == null) return;
            foreach (var e in a) if (e != null) Add(e);
        }

        public void Clear() => map.Clear();

        public bool Contains(object o)
        {
            if (o is E e) return map.ContainsKey(e);
            return false;
        }

        public bool ContainsAll(E[] a)
        {
            if (a == null) return true;
            foreach (var e in a) if (!Contains(e)) return false;
            return true;
        }

        public bool IsEmpty() => map.IsEmpty();

        public bool Remove(object o)
        {
            if (o is E e) return map.Remove(e);
            return false;
        }

        public void RemoveAll(E[] a)
        {
            if (a == null) return;
            foreach (var e in a) Remove(e);
        }

        public void RetainAll(E[] a)
        {
            if (a == null) { Clear(); return; }
            var keep = new HashSet<E>(a);
            var toRemove = new List<E>();
            foreach (var e in map.KeySet())
                if (!keep.Contains(e)) toRemove.Add(e);
            foreach (var e in toRemove) map.Remove(e);
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
            if (a == null) a = new E[keys.Count];
            else if (a.Length < keys.Count) a = new E[keys.Count];
            for (int i = 0; i < keys.Count; i++) a[i] = keys[i];
            if (a.Length > keys.Count) a[keys.Count] = default;
            return a;
        }

        public E First() => map.FirstKey();
        public E Last() => map.LastKey();

        public MyTreeSet<E> SubSet(E fromElement, E toElement) =>
            new MyTreeSet<E>(map.SubMap(fromElement, toElement));

        public MyTreeSet<E> HeadSet(E toElement) =>
            new MyTreeSet<E>(map.HeadMap(toElement));

        public MyTreeSet<E> TailSet(E fromElement) =>
            new MyTreeSet<E>(map.TailMap(fromElement));

        public E Ceiling(E obj)
        {
            var entry = map.CeilingEntry(obj);
            if (entry == null) return default;
            return entry.Key;
        }

        public E Floor(E obj)
        {
            var entry = map.FloorEntry(obj);
            if (entry == null) return default;
            return entry.Key;
        }

        public E Higher(E obj)
        {
            var entry = map.HigherEntry(obj);
            if (entry == null) return default;
            return entry.Key;
        }

        public E Lower(E obj)
        {
            var entry = map.LowerEntry(obj);
            if (entry == null) return default;
            return entry.Key;
        }

        public MyTreeSet<E> HeadSet(E upperBound, bool inclusive)
        {
            if (upperBound == null) throw new ArgumentNullException(nameof(upperBound));
            if (inclusive)
            {
                var result = new MyTreeSet<E>(comparator);
                foreach (var e in map.KeySet())
                    if (comparator.Compare(e, upperBound) <= 0) result.Add(e);
                return result;
            }
            else return HeadSet(upperBound);
        }

        public MyTreeSet<E> SubSet(E lowerBound, bool lowIncl, E upperBound, bool highIncl)
        {
            if (lowerBound == null) throw new ArgumentNullException(nameof(lowerBound));
            if (upperBound == null) throw new ArgumentNullException(nameof(upperBound));
            var result = new MyTreeSet<E>(comparator);
            foreach (var e in map.KeySet())
            {
                int cmpLow = comparator.Compare(e, lowerBound);
                int cmpHigh = comparator.Compare(e, upperBound);
                bool lowOk = lowIncl ? cmpLow >= 0 : cmpLow > 0;
                bool highOk = highIncl ? cmpHigh <= 0 : cmpHigh < 0;
                if (lowOk && highOk) result.Add(e);
            }
            return result;
        }

        public MyTreeSet<E> TailSet(E fromElement, bool inclusive)
        {
            if (fromElement == null) throw new ArgumentNullException(nameof(fromElement));
            if (inclusive)
            {
                var result = new MyTreeSet<E>(comparator);
                foreach (var e in map.KeySet())
                    if (comparator.Compare(e, fromElement) >= 0) result.Add(e);
                return result;
            }
            else return TailSet(fromElement);
        }

        public E PollFirst()
        {
            var entry = map.PollFirstEntry();
            if (entry == null) return default;
            return entry.Key;
        }

        public E PollLast()
        {
            var entry = map.PollLastEntry();
            if (entry == null) return default;
            return entry.Key;
        }

        public IEnumerator<E> DescendingIterator()
        {
            var list = map.KeySet();
            for (int i = list.Count - 1; i >= 0; i--)
                yield return list[i];
        }

        public MyTreeSet<E> DescendingSet()
        {
            var revComp = Comparer<E>.Create((x, y) => comparator.Compare(y, x));
            var revSet = new MyTreeSet<E>(revComp);
            foreach (var e in map.KeySet()) revSet.Add(e);
            return revSet;
        }

        public IEnumerator<E> GetEnumerator()
        {
            foreach (var e in map.KeySet()) yield return e;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
