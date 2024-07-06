Shader "Unlit/Hologram"
{
    Properties
    {
        //Textures
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha("Alpha Map", 2D) = "white" {}

        //Lines
        _LinesAlpha("Scrolling Alpha Lines", 2D) = "White" {}
        _LineApplicationAmount("Line Application Amount", Range(0.0, 1.0)) = 1
        _LineScrollSpeed("Line Scroll Speed", float) = 1

        //Color
        [MainColor] _TintColor("Tint Color", Color) = (1,1,1,1)
        _AlphaAddition("Alpha Addditional", float) = 0

        //Sway Effects
        _SwayPeriod("Sway Period", float) = 1
        _SwayAmplitude("Sway Amplitude", float) = 1
        _SwayDistance("Sway Distance", float) = 1

        //Glitch Effects
        _GlitchStartDistance("Glitch Start Distance", float) = 0.25
        _GlitchEndDistance("Glitch End Distance", float) = 0.75
        _GlitchOffset("Glitch Offset", float) = 0.25
        _GlitchFrequency("Glitch Frequency", float) = 1
        _GlitchScrollSpeed("Glitch Scroll Speed", float) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 viewPos : TEXCOORD1;
            };

            float ParsePeriod(float incomingPeriod)
            {
                static const float PI = 3.14159265f;
                return incomingPeriod * 2 * PI;
            }

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TintColor;
            sampler2D _Alpha;
            sampler2D _LinesAlpha;
            float _SwayPeriod;
            float _SwayAmplitude;
            float _SwayDistance;
            float _AlphaAddition;
            float _LineScrollSpeed;
            float _LineApplicationAmount;
            float _GlitchStartDistance;
            float _GlitchEndDistance;
            float _GlitchOffset;
            float _GlitchScrollSpeed;
            float _GlitchFrequency;
            v2f vert (appdata v)
            {
                const float e = 2.718281828459045;

                //Give some slight sway to the object
                v.vertex.x += pow(e,(sin(((_Time.y / _SwayPeriod) * _SwayAmplitude)+ v.vertex.y) * _SwayDistance));
                v.vertex.x -= 1;
                v.vertex.y += pow(e,(sin(((_Time.y / _SwayPeriod) * _SwayAmplitude)+ v.vertex.z) * _SwayDistance));
                v.vertex.y -= 1;
                v.vertex.z += pow(e,(sin(((_Time.y / _SwayPeriod) * _SwayAmplitude)+ v.vertex.x) * _SwayDistance));
                v.vertex.z -= 1;

                //Setup info to pass to fragment
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.viewPos = UnityObjectToViewPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //Glitch Effect
                float2 glitchScrolledUVs = i.viewPos.xy + float2(0, _Time.y * _GlitchScrollSpeed);
                float2 uvCoordsToUse = i.uv;
                if(glitchScrolledUVs.y % _GlitchFrequency > _GlitchStartDistance && glitchScrolledUVs.y % _GlitchFrequency < _GlitchEndDistance)
                {
                    uvCoordsToUse += float2( _GlitchOffset, 0);
                }
                fixed4 col = tex2D(_MainTex, uvCoordsToUse) * _TintColor + _AlphaAddition;
                col.a *= (tex2D(_Alpha, uvCoordsToUse));
                //Line Scrolling
                float2 lineScrolledUVs = i.viewPos.xy + float2(0, _Time.y * _LineScrollSpeed);
                //Apply line alpha
                const float white = 1;
                col.a *= lerp(tex2D(_LinesAlpha, lineScrolledUVs), white, 1 - _LineApplicationAmount);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
