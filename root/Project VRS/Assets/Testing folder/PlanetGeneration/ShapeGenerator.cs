using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings shapeSettings;
    NoiseFilter[] filters;

    public ShapeGenerator(ShapeSettings shapeSettings)
    {
        this.shapeSettings = shapeSettings;
        filters = new NoiseFilter[shapeSettings.noiseLayers.Length];
        for (int i = 0; i < shapeSettings.noiseLayers.Length; i++)
        {
            filters[i] = new NoiseFilter(shapeSettings.noiseLayers[i].settings);
        }
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnSphere)
    {
        float firstLayerValue = 0f;
        float elevation = 0;

        if(filters.Length > 0)
        {
            firstLayerValue = filters[0].Evalutate(pointOnSphere);
            if (shapeSettings.noiseLayers[0].enabled)
            {
                elevation = firstLayerValue;
            }
        }
        for (int i = 0;i < filters.Length; i++)
        {
            if (shapeSettings.noiseLayers[i].enabled)
            {
                float mask = (shapeSettings.noiseLayers[i].UseFirstLayerAsMask) ? firstLayerValue : 1;
                elevation += filters[i].Evalutate(pointOnSphere) * mask;
            }
            
        }
        return pointOnSphere * shapeSettings.PlanetRadius * (1 + elevation);
    }
}
