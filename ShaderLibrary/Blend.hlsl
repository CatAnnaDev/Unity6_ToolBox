#ifndef CATANNADEV_BLEND_INCLUDED
#define CATANNADEV_BLEND_INCLUDED

#include "Common.hlsl"

float3 Cat_BlendMultiply(float3 base, float3 blend, float opacity)
{
    float3 result = base * blend;
    return lerp(base, result, opacity);
}

float3 Cat_BlendScreen(float3 base, float3 blend, float opacity)
{
    float3 result = 1.0 - (1.0 - base) * (1.0 - blend);
    return lerp(base, result, opacity);
}

float3 Cat_BlendOverlay(float3 base, float3 blend, float opacity)
{
    float3 result = lerp(2.0 * base * blend, 1.0 - 2.0 * (1.0 - base) * (1.0 - blend), step(0.5, base));
    return lerp(base, result, opacity);
}

float3 Cat_BlendSoftLight(float3 base, float3 blend, float opacity)
{
    float3 curved = lerp(((16.0 * base - 12.0) * base + 4.0) * base, sqrt(base), step(0.25, base));
    float3 result = lerp(base - (1.0 - 2.0 * blend) * base * (1.0 - base),
                         base + (2.0 * blend - 1.0) * (curved - base),
                         step(0.5, blend));
    return lerp(base, result, opacity);
}

float3 Cat_BlendHardLight(float3 base, float3 blend, float opacity)
{
    float3 result = lerp(2.0 * base * blend, 1.0 - 2.0 * (1.0 - base) * (1.0 - blend), step(0.5, blend));
    return lerp(base, result, opacity);
}

float3 Cat_BlendColorDodge(float3 base, float3 blend, float opacity)
{
    float3 result = saturate(base / max(1.0 - blend, CAT_EPSILON));
    return lerp(base, result, opacity);
}

float3 Cat_BlendColorBurn(float3 base, float3 blend, float opacity)
{
    float3 result = 1.0 - saturate((1.0 - base) / max(blend, CAT_EPSILON));
    return lerp(base, result, opacity);
}

float3 Cat_BlendLinearDodge(float3 base, float3 blend, float opacity)
{
    float3 result = saturate(base + blend);
    return lerp(base, result, opacity);
}

float3 Cat_BlendLinearBurn(float3 base, float3 blend, float opacity)
{
    float3 result = saturate(base + blend - 1.0);
    return lerp(base, result, opacity);
}

float3 Cat_BlendDifference(float3 base, float3 blend, float opacity)
{
    float3 result = abs(base - blend);
    return lerp(base, result, opacity);
}

float3 Cat_BlendExclusion(float3 base, float3 blend, float opacity)
{
    float3 result = base + blend - 2.0 * base * blend;
    return lerp(base, result, opacity);
}

float3 Cat_BlendLighten(float3 base, float3 blend, float opacity)
{
    float3 result = max(base, blend);
    return lerp(base, result, opacity);
}

float3 Cat_BlendDarken(float3 base, float3 blend, float opacity)
{
    float3 result = min(base, blend);
    return lerp(base, result, opacity);
}

float3 Cat_BlendVividLight(float3 base, float3 blend, float opacity)
{
    float3 burn = 1.0 - (1.0 - base) / max(2.0 * blend, CAT_EPSILON);
    float3 dodge = base / max(2.0 * (1.0 - blend), CAT_EPSILON);
    float3 result = saturate(lerp(burn, dodge, step(0.5, blend)));
    return lerp(base, result, opacity);
}

float3 Cat_BlendPinLight(float3 base, float3 blend, float opacity)
{
    float3 result = lerp(min(base, 2.0 * blend), max(base, 2.0 * blend - 1.0), step(0.5, blend));
    return lerp(base, result, opacity);
}

#endif
