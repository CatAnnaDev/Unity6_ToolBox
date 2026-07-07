#ifndef CATANNADEV_SDF2D_INCLUDED
#define CATANNADEV_SDF2D_INCLUDED

#include "Common.hlsl"

float Cat_SdfCircle(float2 p, float r)
{
    return length(p) - r;
}

float Cat_SdfBox(float2 p, float2 b)
{
    float2 d = abs(p) - b;
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}

float Cat_SdfRoundedBox(float2 p, float2 b, float r)
{
    float2 d = abs(p) - b + r;
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0) - r;
}

float Cat_SdfSegment(float2 p, float2 a, float2 b)
{
    float2 pa = p - a;
    float2 ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

float Cat_SdfTriangle(float2 p, float r)
{
    const float k = sqrt(3.0);
    p.x = abs(p.x) - r;
    p.y = p.y + r / k;
    if (p.x + k * p.y > 0.0)
    {
        p = float2(p.x - k * p.y, -k * p.x - p.y) / 2.0;
    }
    p.x -= clamp(p.x, -2.0 * r, 0.0);
    return -length(p) * sign(p.y);
}

float Cat_SdfHexagon(float2 p, float r)
{
    const float3 k = float3(-0.866025404, 0.5, 0.577350269);
    p = abs(p);
    p -= 2.0 * min(dot(k.xy, p), 0.0) * k.xy;
    p -= float2(clamp(p.x, -k.z * r, k.z * r), r);
    return length(p) * sign(p.y);
}

float Cat_SdfStar5(float2 p, float r, float rf)
{
    const float2 k1 = float2(0.809016994375, -0.587785252292);
    const float2 k2 = float2(-k1.x, k1.y);
    p.x = abs(p.x);
    p -= 2.0 * max(dot(k1, p), 0.0) * k1;
    p -= 2.0 * max(dot(k2, p), 0.0) * k2;
    p.x = abs(p.x);
    p.y -= r;
    float2 ba = rf * float2(-k1.y, k1.x) - float2(0.0, 1.0);
    float h = clamp(dot(p, ba) / dot(ba, ba), 0.0, r);
    return length(p - ba * h) * sign(p.y * ba.x - p.x * ba.y);
}

float Cat_SdfArc(float2 p, float2 apertureSinCos, float radius, float thickness)
{
    p.x = abs(p.x);
    float onArc = (apertureSinCos.y * p.x > apertureSinCos.x * p.y)
        ? length(p - apertureSinCos * radius)
        : abs(length(p) - radius);
    return onArc - thickness;
}

float Cat_SdfPie(float2 p, float2 apertureSinCos, float radius)
{
    p.x = abs(p.x);
    float l = length(p) - radius;
    float m = length(p - apertureSinCos * clamp(dot(p, apertureSinCos), 0.0, radius));
    return max(l, m * sign(apertureSinCos.y * p.x - apertureSinCos.x * p.y));
}

float Cat_OpUnion(float a, float b)
{
    return min(a, b);
}

float Cat_OpSubtract(float a, float b)
{
    return max(-a, b);
}

float Cat_OpIntersect(float a, float b)
{
    return max(a, b);
}

float Cat_OpSmoothUnion(float a, float b, float k)
{
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return lerp(b, a, h) - k * h * (1.0 - h);
}

float Cat_OpSmoothSubtract(float a, float b, float k)
{
    float h = clamp(0.5 - 0.5 * (b + a) / k, 0.0, 1.0);
    return lerp(b, -a, h) + k * h * (1.0 - h);
}

float Cat_OpSmoothIntersect(float a, float b, float k)
{
    float h = clamp(0.5 - 0.5 * (b - a) / k, 0.0, 1.0);
    return lerp(b, a, h) + k * h * (1.0 - h);
}

float Cat_OpRound(float d, float r)
{
    return d - r;
}

float Cat_OpAnnular(float d, float r)
{
    return abs(d) - r;
}

float Cat_SdfFill(float d, float smoothing)
{
    return smoothstep(smoothing, -smoothing, d);
}

#endif
