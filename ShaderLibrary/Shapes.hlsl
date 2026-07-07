#ifndef CATANNADEV_SHAPES_INCLUDED
#define CATANNADEV_SHAPES_INCLUDED

#include "Common.hlsl"

float Cat_Grid(float2 uv, float2 cells, float lineWidth)
{
    float2 cellUv = frac(uv * cells);
    float2 edgeDist = min(cellUv, 1.0 - cellUv);
    float nearest = min(edgeDist.x, edgeDist.y);
    return 1.0 - smoothstep(0.0, lineWidth, nearest);
}

float Cat_Checker(float2 uv, float2 cells)
{
    float2 cellId = floor(uv * cells);
    return fmod(cellId.x + cellId.y, 2.0);
}

float Cat_Stripes(float2 uv, float count, float duty)
{
    float phase = frac(uv.x * count);
    return step(phase, duty);
}

float Cat_Dots(float2 uv, float2 cells, float radius)
{
    float2 local = frac(uv * cells) - 0.5;
    float dist = length(local);
    return 1.0 - smoothstep(radius - 0.02, radius + 0.02, dist);
}

void Cat_HexGrid(float2 uv, float scale, out float2 cellId, out float edgeDist)
{
    float2 p = uv * scale;
    float2 hexRatio = float2(1.0, 1.7320508);
    float4 baseCells = floor(float4(p, p - float2(0.5, 1.0)) / hexRatio.xyxy) + 0.5;
    float4 local = float4(p - baseCells.xy * hexRatio, p - (baseCells.zw + 0.5) * hexRatio);
    bool firstCloser = dot(local.xy, local.xy) < dot(local.zw, local.zw);
    float2 offset = firstCloser ? local.xy : local.zw;
    cellId = firstCloser ? baseCells.xy : baseCells.zw + 0.5;
    float2 absOffset = abs(offset);
    float hexDist = max(dot(absOffset, normalize(hexRatio)), absOffset.x);
    edgeDist = 0.5 - hexDist;
}

float Cat_Brick(float2 uv, float2 cells, float offsetPerRow, float mortar)
{
    float2 p = uv * cells;
    float row = floor(p.y);
    p.x += row * offsetPerRow;
    float2 cellUv = frac(p);
    float2 edgeDist = min(cellUv, 1.0 - cellUv);
    float nearest = min(edgeDist.x, edgeDist.y);
    return smoothstep(0.0, mortar, nearest);
}

float Cat_RadialGradient(float2 uv, float2 center, float radius)
{
    return saturate(length(uv - center) / max(radius, CAT_EPSILON));
}

float Cat_LinearGradient(float2 uv, float2 startPoint, float2 endPoint)
{
    float2 axis = endPoint - startPoint;
    float projection = dot(uv - startPoint, axis) / (dot(axis, axis) + CAT_EPSILON);
    return saturate(projection);
}

#endif
