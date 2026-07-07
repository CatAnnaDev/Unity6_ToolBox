#ifndef CATANNADEV_LIGHTING_INCLUDED
#define CATANNADEV_LIGHTING_INCLUDED

#include "Common.hlsl"

float Cat_Fresnel(float3 normal, float3 viewDir, float power)
{
    float ndotv = saturate(dot(Cat_SafeNormalize(normal), Cat_SafeNormalize(viewDir)));
    return pow(1.0 - ndotv, power);
}

float Cat_Rim(float3 normal, float3 viewDir, float power, float threshold)
{
    float ndotv = saturate(dot(Cat_SafeNormalize(normal), Cat_SafeNormalize(viewDir)));
    float rim = pow(1.0 - ndotv, power);
    return smoothstep(threshold, 1.0, rim);
}

float2 Cat_MatcapUV(float3 normalVS)
{
    float3 n = Cat_SafeNormalize(normalVS);
    return n.xy * 0.5 + 0.5;
}

float Cat_HalfLambert(float3 normal, float3 lightDir)
{
    float ndotl = dot(Cat_SafeNormalize(normal), Cat_SafeNormalize(lightDir));
    float wrapped = ndotl * 0.5 + 0.5;
    return wrapped * wrapped;
}

float Cat_Toon(float ndotl, float steps)
{
    float s = max(steps, 1.0);
    return floor(saturate(ndotl) * s) / s;
}

float Cat_FakeSSS(float3 normal, float3 lightDir, float3 viewDir, float distortion, float power)
{
    float3 n = Cat_SafeNormalize(normal);
    float3 l = Cat_SafeNormalize(lightDir);
    float3 v = Cat_SafeNormalize(viewDir);
    float3 transmit = Cat_SafeNormalize(l + n * distortion);
    return pow(saturate(dot(v, -transmit)), power);
}

#endif
