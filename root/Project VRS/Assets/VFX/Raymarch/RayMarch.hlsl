void raymarch_float( float3 rayOrigin, float3 rayDirection, float numSteps, float stepSize,
                     float densityScale, UnityTexture3D volumeTex, UnitySamplerState volumeSampler,
                     float3 offset, float numLightSteps, float lightStepSize, float3 lightDir,
                     float lightAbsorb, float darknessThreshold, float transmittance, out float3 result )
{
	float density = 0;
	float transmission = 0;
	float lightAccumulation = 0;
	float finalLight = 0;

    
	for(int i =0; i< numSteps; i++){
		rayOrigin += (rayDirection*stepSize);

		//The blue dot position
		float3 samplePos = rayOrigin+offset;
		float sampledDensity = SAMPLE_TEXTURE3D(volumeTex, volumeSampler, samplePos).r;
		density += sampledDensity*densityScale;

		//light loop
		float3 lightRayOrigin = samplePos;
		
		for(int j = 0; j < numLightSteps; j++){
			//The red dot position
			lightRayOrigin += -lightDir*lightStepSize;
			float lightDensity = SAMPLE_TEXTURE3D(volumeTex, volumeSampler, lightRayOrigin).r;
			//The accumulated density from samplePos to the light - the higher this value the less light reaches samplePos
			lightAccumulation += lightDensity;
		}

		//The amount of light received along the ray from param rayOrigin in the direction rayDirection
        float lightTransmission = exp(-lightAccumulation);
		//shadow tends to the darkness threshold as lightAccumulation rises
		float shadow = darknessThreshold + lightTransmission * (1.0 -darknessThreshold);
		//The final light value is accumulated based on the current density, transmittance value and the calculated shadow value 
		finalLight += density*transmittance*shadow;
		//Initially a param its value is updated at each step by lightAbsorb, this sets the light lost by scattering
		transmittance *= exp(-density*lightAbsorb);
					
	}

    transmission = exp(-density);

	result = float3(finalLight, transmission, transmittance);
}

void RaymarchColor_float( float3 rayOrigin, float3 rayDirection, float numSteps, float stepSize,
                     float densityScale, UnityTexture3D volumeTex, UnityTexture3D volumeColor, UnityTexture3D volumeShadows, UnitySamplerState volumeSampler,
                     float3 offset, float numLightSteps, float lightStepSize, float3 lightDir,
                     float lightAbsorb, float darknessThreshold, float transmittance,
					 out float3 result, out float4 colorOut, out float4 shadowOut)
{
	float density = 0;
	float transmission = 0;
	float lightAccumulation = 0;
	float finalLight = 0;
	float4 currentColor = float4(0,0,0,0);
	float4 finalColor = float4(0,0,0,0);
    float4 currentShadow = float4(0,0,0,0);
	float4 finalShadow = float4(0,0,0,0);
	for(int i =0; i< numSteps; i++){
		rayOrigin += (rayDirection*stepSize);

		//The blue dot position
		float3 samplePos = rayOrigin+offset;
		float sampledDensity = SAMPLE_TEXTURE3D(volumeTex, volumeSampler, samplePos).r;
		float4 sampledColor = SAMPLE_TEXTURE3D(volumeColor, volumeSampler, samplePos);
		float4 sampledShadow = SAMPLE_TEXTURE3D(volumeShadows, volumeSampler, samplePos);
		density += sampledDensity*densityScale;
		currentColor += sampledColor / numSteps;
		currentShadow += sampledShadow / numSteps;
		//light loop
		float3 lightRayOrigin = samplePos;
		
		for(int j = 0; j < numLightSteps; j++){
			//The red dot position
			lightRayOrigin += -lightDir*lightStepSize;
			float lightDensity = SAMPLE_TEXTURE3D(volumeTex, volumeSampler, lightRayOrigin).r;
			//The accumulated density from samplePos to the light - the higher this value the less light reaches samplePos
			lightAccumulation += lightDensity;
		}
		//The amount of light received along the ray from param rayOrigin in the direction rayDirection
        float lightTransmission = exp(-lightAccumulation);
		//shadow tends to the darkness threshold as lightAccumulation rises
		float shadow = darknessThreshold + lightTransmission * (1.0 -darknessThreshold);
		//The final light value is accumulated based on the current density, transmittance value and the calculated shadow value 
		finalLight += density*transmittance*shadow;
		finalColor += (currentColor/numSteps);
		finalColor += (currentShadow/numSteps);
		//Initially a param its value is updated at each step by lightAbsorb, this sets the light lost by scattering
		transmittance *= exp(-density*lightAbsorb);
	}

    transmission = exp(-density);

	result = float3(finalLight, transmission, transmittance);
	colorOut = finalColor;
	shadowOut = finalShadow;
}