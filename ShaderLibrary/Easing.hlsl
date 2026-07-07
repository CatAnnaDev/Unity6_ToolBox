#ifndef CATANNADEV_EASING_INCLUDED
#define CATANNADEV_EASING_INCLUDED

#include "Common.hlsl"

float Cat_EaseInQuad(float t)
{
    return t * t;
}

float Cat_EaseOutQuad(float t)
{
    return t * (2.0 - t);
}

float Cat_EaseInOutQuad(float t)
{
    return t < 0.5 ? 2.0 * t * t : 1.0 - pow(-2.0 * t + 2.0, 2.0) * 0.5;
}

float Cat_EaseInCubic(float t)
{
    return t * t * t;
}

float Cat_EaseOutCubic(float t)
{
    float f = 1.0 - t;
    return 1.0 - f * f * f;
}

float Cat_EaseInOutCubic(float t)
{
    return t < 0.5 ? 4.0 * t * t * t : 1.0 - pow(-2.0 * t + 2.0, 3.0) * 0.5;
}

float Cat_EaseInQuart(float t)
{
    return t * t * t * t;
}

float Cat_EaseOutQuart(float t)
{
    float f = 1.0 - t;
    return 1.0 - f * f * f * f;
}

float Cat_EaseInOutQuart(float t)
{
    return t < 0.5 ? 8.0 * t * t * t * t : 1.0 - pow(-2.0 * t + 2.0, 4.0) * 0.5;
}

float Cat_EaseInQuint(float t)
{
    return t * t * t * t * t;
}

float Cat_EaseOutQuint(float t)
{
    float f = 1.0 - t;
    return 1.0 - f * f * f * f * f;
}

float Cat_EaseInOutQuint(float t)
{
    return t < 0.5 ? 16.0 * t * t * t * t * t : 1.0 - pow(-2.0 * t + 2.0, 5.0) * 0.5;
}

float Cat_EaseInSine(float t)
{
    return 1.0 - cos(t * CAT_PI * 0.5);
}

float Cat_EaseOutSine(float t)
{
    return sin(t * CAT_PI * 0.5);
}

float Cat_EaseInOutSine(float t)
{
    return -0.5 * (cos(CAT_PI * t) - 1.0);
}

float Cat_EaseInExpo(float t)
{
    return t <= 0.0 ? 0.0 : pow(2.0, 10.0 * t - 10.0);
}

float Cat_EaseOutExpo(float t)
{
    return t >= 1.0 ? 1.0 : 1.0 - pow(2.0, -10.0 * t);
}

float Cat_EaseInOutExpo(float t)
{
    if (t <= 0.0) return 0.0;
    if (t >= 1.0) return 1.0;
    return t < 0.5
        ? pow(2.0, 20.0 * t - 10.0) * 0.5
        : (2.0 - pow(2.0, -20.0 * t + 10.0)) * 0.5;
}

float Cat_EaseInCirc(float t)
{
    return 1.0 - sqrt(1.0 - t * t);
}

float Cat_EaseOutCirc(float t)
{
    float f = t - 1.0;
    return sqrt(1.0 - f * f);
}

float Cat_EaseInOutCirc(float t)
{
    return t < 0.5
        ? (1.0 - sqrt(1.0 - pow(2.0 * t, 2.0))) * 0.5
        : (sqrt(1.0 - pow(-2.0 * t + 2.0, 2.0)) + 1.0) * 0.5;
}

float Cat_EaseInBack(float t)
{
    const float c1 = 1.70158;
    const float c3 = c1 + 1.0;
    return c3 * t * t * t - c1 * t * t;
}

float Cat_EaseOutBack(float t)
{
    const float c1 = 1.70158;
    const float c3 = c1 + 1.0;
    float f = t - 1.0;
    return 1.0 + c3 * f * f * f + c1 * f * f;
}

float Cat_EaseInOutBack(float t)
{
    const float c1 = 1.70158;
    const float c2 = c1 * 1.525;
    return t < 0.5
        ? (pow(2.0 * t, 2.0) * ((c2 + 1.0) * 2.0 * t - c2)) * 0.5
        : (pow(2.0 * t - 2.0, 2.0) * ((c2 + 1.0) * (t * 2.0 - 2.0) + c2) + 2.0) * 0.5;
}

float Cat_EaseInElastic(float t)
{
    if (t <= 0.0) return 0.0;
    if (t >= 1.0) return 1.0;
    const float c4 = CAT_TAU / 3.0;
    return -pow(2.0, 10.0 * t - 10.0) * sin((t * 10.0 - 10.75) * c4);
}

float Cat_EaseOutElastic(float t)
{
    if (t <= 0.0) return 0.0;
    if (t >= 1.0) return 1.0;
    const float c4 = CAT_TAU / 3.0;
    return pow(2.0, -10.0 * t) * sin((t * 10.0 - 0.75) * c4) + 1.0;
}

float Cat_EaseInOutElastic(float t)
{
    if (t <= 0.0) return 0.0;
    if (t >= 1.0) return 1.0;
    const float c5 = CAT_TAU / 4.5;
    return t < 0.5
        ? -(pow(2.0, 20.0 * t - 10.0) * sin((20.0 * t - 11.125) * c5)) * 0.5
        : (pow(2.0, -20.0 * t + 10.0) * sin((20.0 * t - 11.125) * c5)) * 0.5 + 1.0;
}

float Cat_EaseOutBounce(float t)
{
    const float n1 = 7.5625;
    const float d1 = 2.75;
    if (t < 1.0 / d1)
    {
        return n1 * t * t;
    }
    else if (t < 2.0 / d1)
    {
        t -= 1.5 / d1;
        return n1 * t * t + 0.75;
    }
    else if (t < 2.5 / d1)
    {
        t -= 2.25 / d1;
        return n1 * t * t + 0.9375;
    }
    else
    {
        t -= 2.625 / d1;
        return n1 * t * t + 0.984375;
    }
}

float Cat_EaseInBounce(float t)
{
    return 1.0 - Cat_EaseOutBounce(1.0 - t);
}

float Cat_EaseInOutBounce(float t)
{
    return t < 0.5
        ? (1.0 - Cat_EaseOutBounce(1.0 - 2.0 * t)) * 0.5
        : (1.0 + Cat_EaseOutBounce(2.0 * t - 1.0)) * 0.5;
}

float Cat_Gain(float x, float k)
{
    float a = 0.5 * pow(2.0 * (x < 0.5 ? x : 1.0 - x), k);
    return x < 0.5 ? a : 1.0 - a;
}

float Cat_Bias(float x, float b)
{
    return pow(x, log(b) / log(0.5));
}

float Cat_Smootherstep(float a, float b, float x)
{
    float t = saturate((x - a) / (b - a));
    return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
}

float Cat_TriangleWave(float x)
{
    return abs(frac(x) * 2.0 - 1.0);
}

float Cat_SawWave(float x)
{
    return frac(x);
}

float Cat_SquareWave(float x, float duty)
{
    return frac(x) < duty ? 1.0 : 0.0;
}

#endif
