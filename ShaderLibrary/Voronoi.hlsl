#ifndef CATANNADEV_VORONOI_INCLUDED
#define CATANNADEV_VORONOI_INCLUDED

#include "Common.hlsl"
#include "Hash.hlsl"

void Cat_Voronoi2(float2 p, out float2 cellId, out float f1, out float f2)
{
    float2 baseCell = floor(p);
    float2 localPos = frac(p);
    f1 = 1e9;
    f2 = 1e9;
    cellId = baseCell;
    for (int y = -1; y <= 1; y++)
    {
        for (int x = -1; x <= 1; x++)
        {
            float2 neighbor = float2(x, y);
            float2 featurePoint = Cat_Hash22(baseCell + neighbor);
            float2 toFeature = neighbor + featurePoint - localPos;
            float distanceSquared = dot(toFeature, toFeature);
            if (distanceSquared < f1)
            {
                f2 = f1;
                f1 = distanceSquared;
                cellId = baseCell + neighbor;
            }
            else if (distanceSquared < f2)
            {
                f2 = distanceSquared;
            }
        }
    }
    f1 = sqrt(f1);
    f2 = sqrt(f2);
}

float Cat_Worley2(float2 p)
{
    float2 cellId;
    float f1;
    float f2;
    Cat_Voronoi2(p, cellId, f1, f2);
    return f1;
}

float Cat_VoronoiEdges2(float2 p)
{
    float2 baseCell = floor(p);
    float2 localPos = frac(p);
    float2 nearestOffset = float2(0.0, 0.0);
    float minDistanceSquared = 1e9;
    for (int y = -1; y <= 1; y++)
    {
        for (int x = -1; x <= 1; x++)
        {
            float2 neighbor = float2(x, y);
            float2 featurePoint = Cat_Hash22(baseCell + neighbor);
            float2 toFeature = neighbor + featurePoint - localPos;
            float distanceSquared = dot(toFeature, toFeature);
            if (distanceSquared < minDistanceSquared)
            {
                minDistanceSquared = distanceSquared;
                nearestOffset = toFeature;
            }
        }
    }
    float edgeDistance = 1e9;
    for (int ey = -2; ey <= 2; ey++)
    {
        for (int ex = -2; ex <= 2; ex++)
        {
            float2 neighbor = float2(ex, ey);
            float2 featurePoint = Cat_Hash22(baseCell + neighbor);
            float2 toFeature = neighbor + featurePoint - localPos;
            float2 difference = toFeature - nearestOffset;
            if (dot(difference, difference) > CAT_EPSILON)
            {
                float2 midpoint = 0.5 * (nearestOffset + toFeature);
                edgeDistance = min(edgeDistance, dot(midpoint, normalize(difference)));
            }
        }
    }
    return edgeDistance;
}

#endif
