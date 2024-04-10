using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
public static class ExtensionMethods
{
    public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
    {
        return current.Next ?? current.List.First;
    }
    public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> current)
    {
        return current.Previous ?? current.List.Last;
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    #region float3 methods for utilization in Unity Jobs
    public static float3 Vector3ToFloat3(this Vector3 from)
    {
        return new float3(from.x, from.y, from.z);
    }
    public static Vector3 Float3ToVector3(this float3 from)
    {
        return new Vector3(from.x, from.y, from.z);
    }
    public static float DotProduct(float3 a, float3 b)
    {
        float product = a.x * b.x + a.y * b.y + a.z * b.z;
        return product;
    }
    public static float GetFloat3Magnitude(this float3 floatVal)
    {
        return (float)Mathf.Sqrt(floatVal.x * floatVal.x + floatVal.y * floatVal.y + floatVal.z * floatVal.z);
    }
    public static float3 NormalizeVector(this float3 vector)
    {
        float magnitude = GetFloat3Magnitude(vector);
        if (magnitude > 0)
        {
            return new float3(vector.x / magnitude, vector.y / magnitude, vector.z / magnitude);
        }
        else
        {
            return float3.zero;
        }
    }
    #endregion
    public static bool HasComponent<T>(this GameObject gameObject)
    {
        return gameObject.GetComponent<T>() != null;
    }
}
