Shader "CatAnnaDev/Examples/DissolveUnlit"
{
    Properties
    {
        _Color ("Base Color", Color) = (0.2, 0.6, 1.0, 1.0)
        _EdgeColor ("Edge Glow Color", Color) = (1.0, 0.5, 0.1, 1.0)
        _Scale ("Noise Scale", Float) = 6.0
        _Threshold ("Dissolve Threshold", Range(0.0, 1.0)) = 0.5
        _EdgeWidth ("Edge Width", Range(0.001, 0.5)) = 0.08
        _EdgeIntensity ("Edge Intensity", Float) = 3.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "../CatShaderLibrary.hlsl"

            float4 _Color;
            float4 _EdgeColor;
            float _Scale;
            float _Threshold;
            float _EdgeWidth;
            float _EdgeIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float noise = Cat_ValueNoise2(i.uv * _Scale);
                float edge;
                float mask = Cat_Dissolve(noise, _Threshold, _EdgeWidth, edge);
                clip(mask - 0.5);
                float3 color = lerp(_Color.rgb, _EdgeColor.rgb * _EdgeIntensity, edge);
                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
}
