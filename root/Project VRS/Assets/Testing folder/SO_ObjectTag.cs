using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SO_ObjectTag : ScriptableObject
{
    [SerializeField]
    private List<string> m_objectTag = new List<string>();

    public List<string> objectTag
    {  get { return m_objectTag; } }

    public static bool CompareTags(string tag1, string tag2)
    {
        // Exact match (case-sensitive)
        return tag1 == tag2;
    }
}

public interface IObjectTagImplementor
{ 
    public List<string> Tag { get; }
    public SO_ObjectTag tagData { get; }
}

