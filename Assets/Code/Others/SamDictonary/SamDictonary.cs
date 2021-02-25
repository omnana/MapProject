using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 简易Dictonary
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class SamDictonary <TKey, TValue>
{
    public struct SamEntry
    {
        public int hashCode;

        public TKey key;

        public TValue value;

        public int next;
    }

    private int[] buckets;

    private SamEntry[] entries;

    private int count;//元素数量

    private const int MaxSize = 256;

    public object Current => throw new NotImplementedException();

    public TValue this[TKey key]
    {
        get
        {
            int i = FindEntry(key);

            if (i >= 0) return entries[i].value;

            return default;
        }
        set
        {
            Insert(key, value);
        }
    }

    public SamDictonary()
    {
        Initialize(0);
    }

    private void Initialize(int capacity)
    {
        int size = MaxSize;
        buckets = new int[size];
        for (int i = 0; i < buckets.Length; i++) buckets[i] = -1;
        entries = new SamEntry[size];
    }

    public void Add(TKey key, TValue value)
    {
        Insert(key, value);
    }

    public bool Remove(TKey key)
    {
        if (buckets != null)
        {
            int hashCode = HashTool.Bernstein(key.ToString()) & 0x7FFFFFFF;

            int bucket = hashCode % buckets.Length;

            int last = -1;

            for (int i = buckets[bucket]; i >= 0; last = i, i = entries[i].next)
            {
                if (entries[i].hashCode == hashCode && entries[i].key.Equals(key))
                {
                    if(last <= 0)
                    {
                        buckets[bucket] = entries[i].next;
                    }
                    else
                    {
                        entries[last].next = entries[i].next;
                    }

                    entries[i].key = default;

                    entries[i].value = default;

                    entries[i].next = -1;

                    entries[i].hashCode = -1;

                    count--;

                    return true;
                }
            }
        }

        return false;
    }

    public bool ContainsKey(TKey key)
    {
        return FindEntry(key) >= 0;
    }

    private int FindEntry(TKey key)
    {
        if (key == null)
        {
        }

        if (buckets != null)
        {
            int hashCode = HashTool.Bernstein(key.ToString()) & 0x7FFFFFFF;

            //int hashCode = key.GetHashCode() & 0x7FFFFFFF;

            for (int i = buckets[hashCode % buckets.Length]; i >= 0; i = entries[i].next)
            {
                if (entries[i].hashCode == hashCode && entries[i].key.Equals(key)) return i;
            }
        }

        return -1;
    }

    private void Insert(TKey key, TValue value)
    {
        if (buckets == null) Initialize(0);

        int hashCode = HashTool.Bernstein(key.ToString()) & 0x7FFFFFFF;
        //int hashCode = key.GetHashCode() & 0x7FFFFFFF;

        int targetBucket = hashCode % buckets.Length;

        int collisionCount = 0;

        for (int i = buckets[targetBucket]; i >= 0; i = entries[i].next)
        {
            if (entries[i].hashCode == hashCode && entries[i].key.Equals(key))
            {
                entries[i].value = value;

                return;
            }

            collisionCount++;
        }

        var index = count;

        count++;

        entries[index].hashCode = hashCode;

        entries[index].next = buckets[targetBucket];

        entries[index].key = key;

        entries[index].value = value;

        buckets[targetBucket] = index;
    }

    public void Clear()
    {
        if (count > 0)
        {
            for (int i = 0; i < buckets.Length; i++) buckets[i] = -1;

            Array.Clear(entries, 0, count);

            count = 0;
        }
    }

    public bool MoveNext()
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}
