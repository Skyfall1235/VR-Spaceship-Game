using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilter
{
    NoiseSettings settings;
    Noise noise = new Noise();

    public NoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evalutate(Vector3 point)
    {
        float noiseValue = 0f;
        float freq = settings.baseRoughness;
        float amplitude = 1;
        for (int i = 0; i < settings.numberOfLayers; i++)
        {
            float v = noise.Evaluate(point * freq + settings.center);
            noiseValue += (v + 1) * 0.5f * amplitude;
            freq *= settings.baseRoughness;
            amplitude *= settings.persistance;
        }

        noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
        return noiseValue * settings.strength;
    }
}
