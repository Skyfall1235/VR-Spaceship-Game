using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings shapeSettings;
    NoiseFilter filter;

    public ShapeGenerator(ShapeSettings shapeSettings)
    {
        this.shapeSettings = shapeSettings;
        //filter = new NoiseFilter(shapeSettings.noiseSettings);
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnSphere)
    {
        float elevation = filter.Evalutate(pointOnSphere);
        return pointOnSphere * shapeSettings.PlanetRadius * (1 + elevation);
    }
}
