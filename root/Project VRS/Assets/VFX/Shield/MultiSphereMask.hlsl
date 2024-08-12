#ifndef CUSTOM_INSTANCING
#define CUSTOM_INSTANCING
uniform sampler1D _IncomingTexture;
uniform int _ArrayLength;

float Remap(float In, float2 InMinMax, float2 OutMinMax)
{
    return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void MultiSphereMask_float(float3 Coords, float3 Offset, float Radius, float Hardness, out float Out)
{
	float value = 0;
	for(int i = 0; i < _ArrayLength; i++)
	{
		float p = 1.0/float(_ArrayLength-1);
        float4 valueAtPosition = tex1Dlod(_IncomingTexture, i * p);
		value += 1 - saturate((distance(Coords, valueAtPosition.xyz) - Remap(valueAtPosition.w, float2(0,1), float2(0, Radius))) / (1 - Hardness));
	}
	Out = value;
}
#endif