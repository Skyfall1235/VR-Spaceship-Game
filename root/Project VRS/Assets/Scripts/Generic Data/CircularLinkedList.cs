using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularLinkedList<T>
{
    private Node<T> head = null;
    private Node<T> tail = null;
    public int Count { get; private set; }
    public void Add(T item)
    {

    }
    public bool Contains(T Item)
    {
        return false;
    }
    public void Remove(T Item) 
    {

    }

}
public class Node<T> 
{
    public T value;
    public Node<T> _next;
    public Node<T> _prev;
    public Node(Node<T> next, Node<T> prev)
    {
        _next = next;
        _prev = prev;
    }
}
