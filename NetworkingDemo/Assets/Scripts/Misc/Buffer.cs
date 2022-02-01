using System.Collections;
using System.Collections.Generic;


public class Buffer<T>
{ 

    private T[] bufArray;
    private int m_count = 0;
    private int m_head = 0;
    private int m_tail = 0;

    public T this[int i] => bufArray[i];

    public int Count => GetCount();
    public int Size => GetSize();

    public Buffer(int capacity = 8)
    {
        bufArray = new T[capacity];
    }

    private void Increment(ref int i)
    {
        i = i++ % Size;
    }

    private int GetCount()
    {
        return m_count;
    }

    private int GetSize()
    {
        return bufArray.Length;
    }

    public void Add(T item)
    {
        Increment(ref m_head);
        bufArray[m_tail] = item;
        Increment(ref m_tail);
        if (m_count != Size) m_count++;
    }

    public T Pop()
    {
        T item = bufArray[m_head];
        Increment(ref m_head);
        m_count--;
        return item;
    }

    public void Clear()
    {
        bufArray = new T[bufArray.Length];
        m_count = 0;
        m_tail = 0;
        m_head = 0;
    }

    public bool Contains(T item)
    {
        EqualityComparer<T> comp = EqualityComparer<T>.Default;
        foreach(T t in bufArray)
        {
            if (comp.Equals(item, t)) return true;
        }
        return false;
    }

}
