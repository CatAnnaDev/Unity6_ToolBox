#ifndef CATANNADEV_NOISE_INCLUDED
#define CATANNADEV_NOISE_INCLUDED

#include "Common.hlsl"
#include "Hash.hlsl"

#define CAT_FBM_MAX_OCTAVES 16

float Cat_ValueNoise2(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);
    float2 u = f * f * (3.0 - 2.0 * f);
    float a = Cat_Hash12(i + float2(0.0, 0.0));
    float b = Cat_Hash12(i + float2(1.0, 0.0));
    float c = Cat_Hash12(i + float2(0.0, 1.0));
    float d = Cat_Hash12(i + float2(1.0, 1.0));
    return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
}

float Cat_ValueNoise3(float3 p)
{
    float3 i = floor(p);
    float3 f = frac(p);
    float3 u = f * f * (3.0 - 2.0 * f);
    float n000 = Cat_Hash13(i + float3(0.0, 0.0, 0.0));
    float n100 = Cat_Hash13(i + float3(1.0, 0.0, 0.0));
    float n010 = Cat_Hash13(i + float3(0.0, 1.0, 0.0));
    float n110 = Cat_Hash13(i + float3(1.0, 1.0, 0.0));
    float n001 = Cat_Hash13(i + float3(0.0, 0.0, 1.0));
    float n101 = Cat_Hash13(i + float3(1.0, 0.0, 1.0));
    float n011 = Cat_Hash13(i + float3(0.0, 1.0, 1.0));
    float n111 = Cat_Hash13(i + float3(1.0, 1.0, 1.0));
    float x00 = lerp(n000, n100, u.x);
    float x10 = lerp(n010, n110, u.x);
    float x01 = lerp(n001, n101, u.x);
    float x11 = lerp(n011, n111, u.x);
    float y0 = lerp(x00, x10, u.y);
    float y1 = lerp(x01, x11, u.y);
    return lerp(y0, y1, u.z);
}

float Cat_GradientNoise2(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);
    float2 u = f * f * (3.0 - 2.0 * f);
    float2 ga = Cat_Hash22(i + float2(0.0, 0.0)) * 2.0 - 1.0;
    float2 gb = Cat_Hash22(i + float2(1.0, 0.0)) * 2.0 - 1.0;
    float2 gc = Cat_Hash22(i + float2(0.0, 1.0)) * 2.0 - 1.0;
    float2 gd = Cat_Hash22(i + float2(1.0, 1.0)) * 2.0 - 1.0;
    float va = dot(ga, f - float2(0.0, 0.0));
    float vb = dot(gb, f - float2(1.0, 0.0));
    float vc = dot(gc, f - float2(0.0, 1.0));
    float vd = dot(gd, f - float2(1.0, 1.0));
    return lerp(lerp(va, vb, u.x), lerp(vc, vd, u.x), u.y);
}

float Cat_GradientNoise3(float3 p)
{
    float3 i = floor(p);
    float3 f = frac(p);
    float3 u = f * f * (3.0 - 2.0 * f);
    float3 g000 = Cat_Hash33(i + float3(0.0, 0.0, 0.0)) * 2.0 - 1.0;
    float3 g100 = Cat_Hash33(i + float3(1.0, 0.0, 0.0)) * 2.0 - 1.0;
    float3 g010 = Cat_Hash33(i + float3(0.0, 1.0, 0.0)) * 2.0 - 1.0;
    float3 g110 = Cat_Hash33(i + float3(1.0, 1.0, 0.0)) * 2.0 - 1.0;
    float3 g001 = Cat_Hash33(i + float3(0.0, 0.0, 1.0)) * 2.0 - 1.0;
    float3 g101 = Cat_Hash33(i + float3(1.0, 0.0, 1.0)) * 2.0 - 1.0;
    float3 g011 = Cat_Hash33(i + float3(0.0, 1.0, 1.0)) * 2.0 - 1.0;
    float3 g111 = Cat_Hash33(i + float3(1.0, 1.0, 1.0)) * 2.0 - 1.0;
    float v000 = dot(g000, f - float3(0.0, 0.0, 0.0));
    float v100 = dot(g100, f - float3(1.0, 0.0, 0.0));
    float v010 = dot(g010, f - float3(0.0, 1.0, 0.0));
    float v110 = dot(g110, f - float3(1.0, 1.0, 0.0));
    float v001 = dot(g001, f - float3(0.0, 0.0, 1.0));
    float v101 = dot(g101, f - float3(1.0, 0.0, 1.0));
    float v011 = dot(g011, f - float3(0.0, 1.0, 1.0));
    float v111 = dot(g111, f - float3(1.0, 1.0, 1.0));
    float x00 = lerp(v000, v100, u.x);
    float x10 = lerp(v010, v110, u.x);
    float x01 = lerp(v001, v101, u.x);
    float x11 = lerp(v011, v111, u.x);
    float y0 = lerp(x00, x10, u.y);
    float y1 = lerp(x01, x11, u.y);
    return lerp(y0, y1, u.z);
}

float Cat_Simplex2(float2 p)
{
    const float K1 = 0.366025403784438647;
    const float K2 = 0.211324865405187118;
    float2 i = floor(p + (p.x + p.y) * K1);
    float2 a = p - i + (i.x + i.y) * K2;
    float m = step(a.y, a.x);
    float2 o = float2(m, 1.0 - m);
    float2 b = a - o + K2;
    float2 c = a - 1.0 + 2.0 * K2;
    float3 h = max(0.5 - float3(dot(a, a), dot(b, b), dot(c, c)), 0.0);
    float3 g = float3(
        dot(a, Cat_Hash22(i + float2(0.0, 0.0)) * 2.0 - 1.0),
        dot(b, Cat_Hash22(i + o) * 2.0 - 1.0),
        dot(c, Cat_Hash22(i + float2(1.0, 1.0)) * 2.0 - 1.0));
    float3 n = h * h * h * h * g;
    return dot(n, float3(70.0, 70.0, 70.0));
}

float Cat_Fbm2(float2 p, int octaves, float lacunarity, float gain)
{
    float sum = 0.0;
    float amplitude = 0.5;
    float frequency = 1.0;
    for (int index = 0; index < CAT_FBM_MAX_OCTAVES; index++)
    {
        if (index >= octaves)
        {
            break;
        }
        sum += amplitude * Cat_GradientNoise2(p * frequency);
        frequency *= lacunarity;
        amplitude *= gain;
    }
    return sum;
}

float Cat_Fbm3(float3 p, int octaves, float lacunarity, float gain)
{
    float sum = 0.0;
    float amplitude = 0.5;
    float frequency = 1.0;
    for (int index = 0; index < CAT_FBM_MAX_OCTAVES; index++)
    {
        if (index >= octaves)
        {
            break;
        }
        sum += amplitude * Cat_GradientNoise3(p * frequency);
        frequency *= lacunarity;
        amplitude *= gain;
    }
    return sum;
}

float Cat_Ridged2(float2 p, int octaves)
{
    float sum = 0.0;
    float amplitude = 0.5;
    float frequency = 1.0;
    for (int index = 0; index < CAT_FBM_MAX_OCTAVES; index++)
    {
        if (index >= octaves)
        {
            break;
        }
        float n = Cat_GradientNoise2(p * frequency);
        n = 1.0 - abs(n);
        n = n * n;
        sum += amplitude * n;
        frequency *= 2.0;
        amplitude *= 0.5;
    }
    return sum;
}

float Cat_Turbulence2(float2 p, int octaves)
{
    float sum = 0.0;
    float amplitude = 0.5;
    float frequency = 1.0;
    for (int index = 0; index < CAT_FBM_MAX_OCTAVES; index++)
    {
        if (index >= octaves)
        {
            break;
        }
        sum += amplitude * abs(Cat_GradientNoise2(p * frequency));
        frequency *= 2.0;
        amplitude *= 0.5;
    }
    return sum;
}

float2 Cat_Curl2(float2 p)
{
    float epsilon = 0.001;
    float2 dx = float2(epsilon, 0.0);
    float2 dy = float2(0.0, epsilon);
    float px1 = Cat_GradientNoise2(p + dx);
    float px0 = Cat_GradientNoise2(p - dx);
    float py1 = Cat_GradientNoise2(p + dy);
    float py0 = Cat_GradientNoise2(p - dy);
    float gradientX = (px1 - px0) / (2.0 * epsilon);
    float gradientY = (py1 - py0) / (2.0 * epsilon);
    return float2(gradientY, -gradientX);
}

#endif
