using System.Collections;
using System.Collections.Generic;


public class RingBuffer<T>
{ 

    private T[] bufArray;
    private int m_count = 0;
    private int m_head = 0;
    private int m_tail = 0;

    public T this[int i] => bufArray[i];

    public int Count => m_count;
    public int Size => bufArray.Length;

    public RingBuffer(int capacity = 8) => bufArray = new T[capacity];
    

    private void Increment(ref int i) => i = i++ % Size;



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
