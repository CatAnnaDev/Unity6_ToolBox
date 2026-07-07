#ifndef CATANNADEV_DITHER_INCLUDED
#define CATANNADEV_DITHER_INCLUDED

#include "Common.hlsl"

float Cat_Bayer4x4(float2 pixelPos)
{
    const float pattern[16] =
    {
         0.0,  8.0,  2.0, 10.0,
        12.0,  4.0, 14.0,  6.0,
         3.0, 11.0,  1.0,  9.0,
        15.0,  7.0, 13.0,  5.0
    };
    int x = ((int)floor(pixelPos.x)) & 3;
    int y = ((int)floor(pixelPos.y)) & 3;
    return pattern[y * 4 + x] / 16.0;
}

float Cat_Bayer8x8(float2 pixelPos)
{
    const float pattern[64] =
    {
         0.0, 32.0,  8.0, 40.0,  2.0, 34.0, 10.0, 42.0,
        48.0, 16.0, 56.0, 24.0, 50.0, 18.0, 58.0, 26.0,
        12.0, 44.0,  4.0, 36.0, 14.0, 46.0,  6.0, 38.0,
        60.0, 28.0, 52.0, 20.0, 62.0, 30.0, 54.0, 22.0,
         3.0, 35.0, 11.0, 43.0,  1.0, 33.0,  9.0, 41.0,
        51.0, 19.0, 59.0, 27.0, 49.0, 17.0, 57.0, 25.0,
        15.0, 47.0,  7.0, 39.0, 13.0, 45.0,  5.0, 37.0,
        63.0, 31.0, 55.0, 23.0, 61.0, 29.0, 53.0, 21.0
    };
    int x = ((int)floor(pixelPos.x)) & 7;
    int y = ((int)floor(pixelPos.y)) & 7;
    return pattern[y * 8 + x] / 64.0;
}

float Cat_OrderedDitherMask(float2 pixelPos)
{
    return Cat_Bayer4x4(pixelPos);
}

float Cat_Dither(float value, float2 pixelPos, float levels)
{
    float steps = max(levels - 1.0, 1.0);
    float threshold = Cat_Bayer4x4(pixelPos);
    return floor(saturate(value) * steps + threshold) / steps;
}

float Cat_Halftone(float2 uv, float value, float scale)
{
    float2 cell = frac(uv * scale) - 0.5;
    float dist = length(cell);
    float radius = sqrt(saturate(value)) * 0.7071068;
    return 1.0 - smoothstep(radius - 0.03, radius + 0.03, dist);
}

#endif
