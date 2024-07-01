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
    /// <summary>
    /// Maps a value from one range to another
    /// </summary>
    /// <param name="value">The value you want to remap</param>
    /// <param name="from1">The lower bound of the starting range</param>
    /// <param name="to1">The upper bound of the starting range</param>
    /// <param name="from2">The lower bound of the ending range</param>
    /// <param name="to2">The upper bound of the ending range</param>
    /// <returns>The value remapped from one range to another as a float</returns>
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

    #region Vector3 Methods
    static Vector3 Abs(this Vector3 inputVector)
    {
        return new Vector3(Mathf.Abs(inputVector.x), Mathf.Abs(inputVector.y), Mathf.Abs(inputVector.z));
    }

    #endregion

    public static void SwingTwist(this Quaternion q, Vector3 twistAxis, out Quaternion swing, out Quaternion twist)
    {
        Vector3 r = new Vector3(q.x, q.y, q.z);

        // singularity: rotation by 180 degree
        if (r.sqrMagnitude < float.Epsilon)
        {
            Vector3 rotatedTwistAxis = q * twistAxis;
            Vector3 swingAxis = Vector3.Cross(twistAxis, rotatedTwistAxis);

            if (swingAxis.sqrMagnitude > float.Epsilon)
            {
                float swingAngle =
                  Vector3.Angle(twistAxis, rotatedTwistAxis);
                swing = Quaternion.AngleAxis(swingAngle, swingAxis);
            }
            else
            {
                // more singularity: 
                // rotation axis parallel to twist axis
                swing = Quaternion.identity; // no swing
            }

            // always twist 180 degree on singularity
            twist = Quaternion.AngleAxis(180.0f, twistAxis);
            return;
        }

        // meat of swing-twist decomposition
        Vector3 p = Vector3.Project(r, twistAxis);
        twist = new Quaternion(p.x, p.y, p.z, q.w);
        twist = Quaternion.Normalize(twist);
        swing = q * Quaternion.Inverse(twist);
    }
}
