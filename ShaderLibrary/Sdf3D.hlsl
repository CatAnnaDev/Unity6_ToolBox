#ifndef CATANNADEV_SDF3D_INCLUDED
#define CATANNADEV_SDF3D_INCLUDED

#include "Sdf2D.hlsl"

float Cat_SdfSphere(float3 p, float r)
{
    return length(p) - r;
}

float Cat_SdfBox3(float3 p, float3 b)
{
    float3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

float Cat_SdfRoundBox3(float3 p, float3 b, float r)
{
    float3 q = abs(p) - b + r;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0) - r;
}

float Cat_SdfTorus(float3 p, float majorRadius, float minorRadius)
{
    float2 q = float2(length(p.xz) - majorRadius, p.y);
    return length(q) - minorRadius;
}

float Cat_SdfCapsule(float3 p, float3 a, float3 b, float r)
{
    float3 pa = p - a;
    float3 ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}

float Cat_SdfCylinder(float3 p, float halfHeight, float r)
{
    float2 d = abs(float2(length(p.xz), p.y)) - float2(r, halfHeight);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}

float Cat_SdfPlane(float3 p, float3 n, float h)
{
    return dot(p, n) + h;
}

float Cat_SdfOctahedron(float3 p, float s)
{
    p = abs(p);
    float m = p.x + p.y + p.z - s;
    float3 q;
    if (3.0 * p.x < m)
    {
        q = p.xyz;
    }
    else if (3.0 * p.y < m)
    {
        q = p.yzx;
    }
    else if (3.0 * p.z < m)
    {
        q = p.zxy;
    }
    else
    {
        return m * 0.57735027;
    }
    float k = clamp(0.5 * (q.z - q.y + s), 0.0, s);
    return length(float3(q.x, q.y - s + k, q.z - k));
}

#endif
