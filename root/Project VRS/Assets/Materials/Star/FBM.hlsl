float hash (float2 n)
{
    return frac(sin(dot(n, float2(12.9898,78.233))) * 43758.5453123 );
}

float noise(float2 p)
{
    float2 i = floor(p);
    float2 u = smoothstep(0.0, 1.0, frac(p));
    float a = hash(i + float2(0,0));
    float b = hash(i + float2(1,0));
    float c = hash(i + float2(0,1));
    float d = hash(i + float2(1,1));
    float r = lerp(lerp(a, b, u.x),lerp(c, d, u.x), u.y);
    return r * r;
}

void FBM_float(float2 p, int octaves, float lacunarity, float gain, out float result)
{
    float value = 0.0;
    float amplitude = 0.5;
    float e = 3.0;
    for (int i = 0; i < octaves; ++ i)
    {
        value += amplitude * noise(p); 
        p = p * e; 
        amplitude *= lacunarity; 
        e *= gain;
    }
    result = value;
}
