#ifndef CATANNADEV_COLOR_INCLUDED
#define CATANNADEV_COLOR_INCLUDED

#include "Common.hlsl"

float3 Cat_RgbToHsv(float3 rgb)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(rgb.bg, K.wz), float4(rgb.gb, K.xy), step(rgb.b, rgb.g));
    float4 q = lerp(float4(p.xyw, rgb.r), float4(rgb.r, p.yzx), step(p.x, rgb.r));
    float d = q.x - min(q.w, q.y);
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + CAT_EPSILON)), d / (q.x + CAT_EPSILON), q.x);
}

float3 Cat_HsvToRgb(float3 hsv)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(hsv.xxx + K.xyz) * 6.0 - K.www);
    return hsv.z * lerp(K.xxx, saturate(p - K.xxx), hsv.y);
}

float3 Cat_RgbToHsl(float3 rgb)
{
    float maxc = max(rgb.r, max(rgb.g, rgb.b));
    float minc = min(rgb.r, min(rgb.g, rgb.b));
    float lightness = (maxc + minc) * 0.5;
    float hue = 0.0;
    float saturation = 0.0;
    float delta = maxc - minc;
    if (delta > CAT_EPSILON)
    {
        saturation = lightness > 0.5 ? delta / (2.0 - maxc - minc) : delta / (maxc + minc);
        if (maxc == rgb.r)
            hue = (rgb.g - rgb.b) / delta + (rgb.g < rgb.b ? 6.0 : 0.0);
        else if (maxc == rgb.g)
            hue = (rgb.b - rgb.r) / delta + 2.0;
        else
            hue = (rgb.r - rgb.g) / delta + 4.0;
        hue /= 6.0;
    }
    return float3(hue, saturation, lightness);
}

float Cat_HueToChannel(float p, float q, float t)
{
    t = frac(t);
    if (t < 1.0 / 6.0) return p + (q - p) * 6.0 * t;
    if (t < 1.0 / 2.0) return q;
    if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6.0;
    return p;
}

float3 Cat_HslToRgb(float3 hsl)
{
    float hue = hsl.x;
    float saturation = hsl.y;
    float lightness = hsl.z;
    if (saturation <= CAT_EPSILON)
        return float3(lightness, lightness, lightness);
    float q = lightness < 0.5 ? lightness * (1.0 + saturation) : lightness + saturation - lightness * saturation;
    float p = 2.0 * lightness - q;
    return float3(
        Cat_HueToChannel(p, q, hue + 1.0 / 3.0),
        Cat_HueToChannel(p, q, hue),
        Cat_HueToChannel(p, q, hue - 1.0 / 3.0));
}

float Cat_Luminance(float3 rgb)
{
    return dot(rgb, float3(0.2126, 0.7152, 0.0722));
}

float3 Cat_Grayscale(float3 rgb)
{
    float value = Cat_Luminance(rgb);
    return float3(value, value, value);
}

float3 Cat_Saturation(float3 rgb, float amount)
{
    float luminance = Cat_Luminance(rgb);
    return lerp(float3(luminance, luminance, luminance), rgb, amount);
}

float3 Cat_Contrast(float3 rgb, float amount)
{
    return (rgb - 0.5) * amount + 0.5;
}

float3 Cat_Brightness(float3 rgb, float amount)
{
    return rgb * amount;
}

float3 Cat_HueShift(float3 rgb, float degrees)
{
    float3 hsv = Cat_RgbToHsv(rgb);
    hsv.x = frac(hsv.x + degrees / 360.0);
    return Cat_HsvToRgb(hsv);
}

float3 Cat_Posterize(float3 rgb, float steps)
{
    steps = max(steps, 1.0);
    return floor(rgb * steps) / steps;
}

float3 Cat_ColorTemperature(float kelvin)
{
    float t = clamp(kelvin, 1000.0, 40000.0) / 100.0;
    float r;
    float g;
    float b;
    if (t <= 66.0)
    {
        r = 1.0;
        g = saturate(0.39008157876901960784 * log(t) - 0.63184144378862745098);
    }
    else
    {
        r = saturate(1.29293618606274509804 * pow(t - 60.0, -0.1332047592));
        g = saturate(1.12989086089529411765 * pow(t - 60.0, -0.0755148492));
    }
    if (t >= 66.0)
        b = 1.0;
    else if (t <= 19.0)
        b = 0.0;
    else
        b = saturate(0.54320678911019607843 * log(t - 10.0) - 1.19625408914491176470);
    return float3(r, g, b);
}

float3 Cat_CosinePalette(float t, float3 a, float3 b, float3 c, float3 d)
{
    return a + b * cos(CAT_TAU * (c * t + d));
}

float3 Cat_LinearToGamma(float3 rgb)
{
    return pow(max(rgb, 0.0), 1.0 / 2.2);
}

float3 Cat_GammaToLinear(float3 rgb)
{
    return pow(max(rgb, 0.0), 2.2);
}

#endif
