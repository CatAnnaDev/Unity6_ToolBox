#ifndef CATANNADEV_COMMON_INCLUDED
#define CATANNADEV_COMMON_INCLUDED

#define CAT_PI   3.14159265358979323846
#define CAT_TAU  6.28318530717958647692
#define CAT_EPSILON 1e-5

float Cat_Remap(float v, float2 inMinMax, float2 outMinMax)
{
    float t = (v - inMinMax.x) / (inMinMax.y - inMinMax.x);
    return outMinMax.x + t * (outMinMax.y - outMinMax.x);
}

float Cat_Remap01(float v, float inMin, float inMax)
{
    return saturate((v - inMin) / (inMax - inMin));
}

float2x2 Cat_Rot2(float radians)
{
    float s = sin(radians);
    float c = cos(radians);
    return float2x2(c, -s, s, c);
}

float3x3 Cat_Rot3X(float radians)
{
    float s = sin(radians);
    float c = cos(radians);
    return float3x3(
        1.0, 0.0, 0.0,
        0.0, c,  -s,
        0.0, s,   c);
}

float3x3 Cat_Rot3Y(float radians)
{
    float s = sin(radians);
    float c = cos(radians);
    return float3x3(
        c,   0.0, s,
        0.0, 1.0, 0.0,
       -s,   0.0, c);
}

float3x3 Cat_Rot3Z(float radians)
{
    float s = sin(radians);
    float c = cos(radians);
    return float3x3(
        c,  -s,   0.0,
        s,   c,   0.0,
        0.0, 0.0, 1.0);
}

float3 Cat_SafeNormalize(float3 v)
{
    float lenSq = dot(v, v);
    return lenSq > CAT_EPSILON ? v * rsqrt(lenSq) : float3(0.0, 0.0, 0.0);
}

float Cat_Pulse(float edge0, float edge1, float x)
{
    return step(edge0, x) - step(edge1, x);
}

float2 Cat_AspectUV(float2 uv, float aspect)
{
    float2 centered = uv - 0.5;
    centered.x *= aspect;
    return centered + 0.5;
}

float3 Cat_Saturate3(float3 v)
{
    return saturate(v);
}

#endif
