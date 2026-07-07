Shader "CatAnnaDev/Examples/NoiseUnlit"
{
    Properties
    {
        _Scale ("Scale", Float) = 4.0
        _Speed ("Speed", Float) = 0.25
        _Octaves ("Octaves", Range(1, 12)) = 5
        _Lacunarity ("Lacunarity", Float) = 2.0
        _Gain ("Gain", Float) = 0.5
        _PaletteA ("Palette Bias", Color) = (0.5, 0.5, 0.5, 1.0)
        _PaletteB ("Palette Amplitude", Color) = (0.5, 0.5, 0.5, 1.0)
        _PaletteC ("Palette Frequency", Color) = (1.0, 1.0, 1.0, 1.0)
        _PaletteD ("Palette Phase", Color) = (0.0, 0.33, 0.67, 1.0)
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

            float _Scale;
            float _Speed;
            float _Octaves;
            float _Lacunarity;
            float _Gain;
            float4 _PaletteA;
            float4 _PaletteB;
            float4 _PaletteC;
            float4 _PaletteD;

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
                float time = _Time.y * _Speed;
                float2 p = i.uv * _Scale + float2(time, time * 0.5);
                float n = Cat_Fbm2(p, (int)_Octaves, _Lacunarity, _Gain);
                float t = n * 0.5 + 0.5;
                float3 color = Cat_CosinePalette(t, _PaletteA.rgb, _PaletteB.rgb, _PaletteC.rgb, _PaletteD.rgb);
                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
}
