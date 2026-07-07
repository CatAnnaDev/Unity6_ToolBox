#ifndef CATANNADEV_EFFECTS_INCLUDED
#define CATANNADEV_EFFECTS_INCLUDED

#include "Common.hlsl"
#include "Hash.hlsl"

float Cat_Dissolve(float noise, float threshold, float edgeWidth, out float edge)
{
    float mask = smoothstep(threshold, threshold + edgeWidth, noise);
    float inner = smoothstep(threshold + edgeWidth, threshold + edgeWidth * 2.0, noise);
    edge = saturate(mask - inner);
    return mask;
}

float Cat_Scanline(float uvY, float count, float time, float speed)
{
    return 0.5 + 0.5 * sin((uvY * count - time * speed) * CAT_TAU);
}

float Cat_Hologram(float2 uv, float time, float scanCount, float scanSpeed)
{
    float scan = 0.5 + 0.5 * sin((uv.y * scanCount - time * scanSpeed) * CAT_TAU);
    float flicker = 0.9 + 0.1 * sin(time * 30.0 + uv.y * 12.0);
    return scan * flicker;
}

void Cat_ChromaticOffset(float2 uv, float2 center, float amount, out float2 uvR, out float2 uvG, out float2 uvB)
{
    float2 dir = uv - center;
    uvR = uv + dir * amount;
    uvG = uv;
    uvB = uv - dir * amount;
}

float Cat_Vignette(float2 uv, float2 center, float radius, float smoothness)
{
    float dist = distance(uv, center);
    return 1.0 - smoothstep(radius, radius + smoothness, dist);
}

float2 Cat_GlitchUV(float2 uv, float time, float amount)
{
    float row = floor(uv.y * 24.0);
    float frame = floor(time * 15.0);
    float n = Cat_Hash12(float2(row, frame));
    float active = step(0.8, n);
    float shift = (Cat_Hash11(n) - 0.5) * amount * active;
    return float2(uv.x + shift, uv.y);
}

float Cat_OutlineFromMask(float mask, float width)
{
    float rising = smoothstep(0.5 - width, 0.5, mask);
    float falling = smoothstep(0.5, 0.5 + width, mask);
    return saturate(rising - falling);
}

#endif
