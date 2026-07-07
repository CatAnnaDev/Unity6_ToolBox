#ifndef CATANNADEV_UV_INCLUDED
#define CATANNADEV_UV_INCLUDED

#include "Common.hlsl"

float2 Cat_RotateUV(float2 uv, float2 center, float radians)
{
    float2x2 rotation = Cat_Rot2(radians);
    return mul(rotation, uv - center) + center;
}

float2 Cat_ScaleUV(float2 uv, float2 center, float2 scale)
{
    return (uv - center) * scale + center;
}

float2 Cat_TileUV(float2 uv, float2 tiling, float2 offset)
{
    return uv * tiling + offset;
}

float2 Cat_PolarUV(float2 uv, float2 center)
{
    float2 delta = uv - center;
    float radius = length(delta);
    float angle = atan2(delta.y, delta.x);
    return float2(radius, angle);
}

float2 Cat_Twirl(float2 uv, float2 center, float strength, float radius)
{
    float2 delta = uv - center;
    float dist = length(delta);
    float falloff = saturate(1.0 - dist / max(radius, CAT_EPSILON));
    float2x2 rotation = Cat_Rot2(strength * falloff);
    return mul(rotation, delta) + center;
}

float2 Cat_Panner(float2 uv, float2 speed, float time)
{
    return uv + speed * time;
}

float2 Cat_Flipbook(float2 uv, float cols, float rows, float frame)
{
    float2 tileSize = float2(1.0, 1.0) / float2(cols, rows);
    float tileIndex = floor(frame);
    float rowIndex = floor(tileIndex / cols);
    float colIndex = tileIndex - cols * rowIndex;
    float2 tileOrigin = float2(colIndex, rows - 1.0 - rowIndex) * tileSize;
    return uv * tileSize + tileOrigin;
}

float2 Cat_RadialShear(float2 uv, float2 center, float2 strength)
{
    float2 delta = uv - center;
    float distSquared = dot(delta, delta);
    float2 shear = distSquared * strength;
    return uv + float2(delta.y, -delta.x) * shear;
}

float2 Cat_MirrorUV(float2 uv)
{
    float2 wrapped = fmod(abs(uv), 2.0);
    return 1.0 - abs(1.0 - wrapped);
}

float2 Cat_SphereWarp(float2 uv)
{
    float2 centered = uv * 2.0 - 1.0;
    float radiusSquared = saturate(dot(centered, centered));
    float depth = sqrt(1.0 - radiusSquared);
    float2 warped = centered / (depth + 1.0);
    return warped + 0.5;
}

float2 Cat_ParallaxOffset(float2 uv, float2 viewDirTS, float height)
{
    return uv + viewDirTS * height;
}

float3 Cat_TriplanarWeights(float3 normal, float sharpness)
{
    float3 weights = pow(abs(normal), sharpness);
    float total = weights.x + weights.y + weights.z + CAT_EPSILON;
    return weights / total;
}

#endif
