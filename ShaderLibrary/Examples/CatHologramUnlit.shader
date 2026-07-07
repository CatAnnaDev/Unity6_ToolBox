Shader "CatAnnaDev/Examples/HologramUnlit"
{
    Properties
    {
        _Color ("Hologram Color", Color) = (0.3, 0.8, 1.0, 1.0)
        _FresnelPower ("Fresnel Power", Float) = 2.5
        _ScanCount ("Hologram Scan Count", Float) = 30.0
        _ScanSpeed ("Hologram Scan Speed", Float) = 1.0
        _LineCount ("Scanline Count", Float) = 80.0
        _LineSpeed ("Scanline Speed", Float) = 4.0
        _Intensity ("Intensity", Float) = 1.5
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100

        Pass
        {
            Blend One One
            ZWrite Off
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "../CatShaderLibrary.hlsl"

            float4 _Color;
            float _FresnelPower;
            float _ScanCount;
            float _ScanSpeed;
            float _LineCount;
            float _LineSpeed;
            float _Intensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldViewDir : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldViewDir = _WorldSpaceCameraPos.xyz - worldPos;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float time = _Time.y;
                float fresnel = Cat_Fresnel(i.worldNormal, i.worldViewDir, _FresnelPower);
                float holo = Cat_Hologram(i.uv, time, _ScanCount, _ScanSpeed);
                float scan = Cat_Scanline(i.uv.y, _LineCount, time, _LineSpeed);
                float glow = (fresnel + holo * 0.5 + scan * 0.25) * _Intensity;
                float3 color = _Color.rgb * glow;
                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
}
