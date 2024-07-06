using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public float strength = 1;
    public float roughness = 2;
    public Vector3 center;
    public int numberOfLayers = 1;
    public float persistance = 0.5f;
    public float baseRoughness = 1;
    public float minValue;
}
