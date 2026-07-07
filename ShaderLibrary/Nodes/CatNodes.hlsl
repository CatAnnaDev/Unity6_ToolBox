#ifndef CATANNADEV_NODES_INCLUDED
#define CATANNADEV_NODES_INCLUDED

#include "../CatShaderLibrary.hlsl"

void Cat_ValueNoise2_float(float2 UV, out float Out)
{
    Out = Cat_ValueNoise2(UV);
}

void Cat_ValueNoise3_float(float3 Position, out float Out)
{
    Out = Cat_ValueNoise3(Position);
}

void Cat_PerlinNoise2_float(float2 UV, out float Out)
{
    Out = Cat_GradientNoise2(UV) * 0.5 + 0.5;
}

void Cat_PerlinNoise3_float(float3 Position, out float Out)
{
    Out = Cat_GradientNoise3(Position) * 0.5 + 0.5;
}

void Cat_Simplex2_float(float2 UV, out float Out)
{
    Out = Cat_Simplex2(UV) * 0.5 + 0.5;
}

void Cat_Fbm2_float(float2 UV, float Octaves, float Lacunarity, float Gain, out float Out)
{
    Out = Cat_Fbm2(UV, (int)Octaves, Lacunarity, Gain);
}

void Cat_Fbm3_float(float3 Position, float Octaves, float Lacunarity, float Gain, out float Out)
{
    Out = Cat_Fbm3(Position, (int)Octaves, Lacunarity, Gain);
}

void Cat_Ridged2_float(float2 UV, float Octaves, out float Out)
{
    Out = Cat_Ridged2(UV, (int)Octaves);
}

void Cat_Turbulence2_float(float2 UV, float Octaves, out float Out)
{
    Out = Cat_Turbulence2(UV, (int)Octaves);
}

void Cat_Curl2_float(float2 UV, out float2 Out)
{
    Out = Cat_Curl2(UV);
}

void Cat_Voronoi2_float(float2 UV, out float2 CellId, out float F1, out float F2)
{
    Cat_Voronoi2(UV, CellId, F1, F2);
}

void Cat_Worley2_float(float2 UV, out float Out)
{
    Out = Cat_Worley2(UV);
}

void Cat_VoronoiEdges2_float(float2 UV, out float Out)
{
    Out = Cat_VoronoiEdges2(UV);
}

void Cat_SdfCircle_float(float2 UV, float Radius, out float Out)
{
    Out = Cat_SdfCircle(UV, Radius);
}

void Cat_SdfBox_float(float2 UV, float2 HalfSize, out float Out)
{
    Out = Cat_SdfBox(UV, HalfSize);
}

void Cat_SdfRoundedBox_float(float2 UV, float2 HalfSize, float Radius, out float Out)
{
    Out = Cat_SdfRoundedBox(UV, HalfSize, Radius);
}

void Cat_SdfHexagon_float(float2 UV, float Radius, out float Out)
{
    Out = Cat_SdfHexagon(UV, Radius);
}

void Cat_SdfTriangle_float(float2 UV, float Radius, out float Out)
{
    Out = Cat_SdfTriangle(UV, Radius);
}

void Cat_SdfStar5_float(float2 UV, float Radius, float Inset, out float Out)
{
    Out = Cat_SdfStar5(UV, Radius, Inset);
}

void Cat_SdfFill_float(float Distance, float Smoothing, out float Out)
{
    Out = Cat_SdfFill(Distance, Smoothing);
}

void Cat_SmoothUnion_float(float A, float B, float Smoothness, out float Out)
{
    Out = Cat_OpSmoothUnion(A, B, Smoothness);
}

void Cat_SmoothSubtract_float(float A, float B, float Smoothness, out float Out)
{
    Out = Cat_OpSmoothSubtract(A, B, Smoothness);
}

void Cat_SmoothIntersect_float(float A, float B, float Smoothness, out float Out)
{
    Out = Cat_OpSmoothIntersect(A, B, Smoothness);
}

void Cat_SdfSphere_float(float3 Position, float Radius, out float Out)
{
    Out = Cat_SdfSphere(Position, Radius);
}

void Cat_SdfTorus_float(float3 Position, float MajorRadius, float MinorRadius, out float Out)
{
    Out = Cat_SdfTorus(Position, MajorRadius, MinorRadius);
}

void Cat_HueShift_float(float3 Color, float Degrees, out float3 Out)
{
    Out = Cat_HueShift(Color, Degrees);
}

void Cat_Posterize_float(float3 Color, float Steps, out float3 Out)
{
    Out = Cat_Posterize(Color, Steps);
}

void Cat_Saturation_float(float3 Color, float Amount, out float3 Out)
{
    Out = Cat_Saturation(Color, Amount);
}

void Cat_Contrast_float(float3 Color, float Amount, out float3 Out)
{
    Out = Cat_Contrast(Color, Amount);
}

void Cat_CosinePalette_float(float T, float3 Offset, float3 Amplitude, float3 Frequency, float3 Phase, out float3 Out)
{
    Out = Cat_CosinePalette(T, Offset, Amplitude, Frequency, Phase);
}

void Cat_RgbToHsv_float(float3 Color, out float3 Out)
{
    Out = Cat_RgbToHsv(Color);
}

void Cat_HsvToRgb_float(float3 Hsv, out float3 Out)
{
    Out = Cat_HsvToRgb(Hsv);
}

void Cat_ColorTemperature_float(float Kelvin, out float3 Out)
{
    Out = Cat_ColorTemperature(Kelvin);
}

void Cat_RotateUV_float(float2 UV, float2 Center, float Rotation, out float2 Out)
{
    Out = Cat_RotateUV(UV, Center, Rotation);
}

void Cat_PolarUV_float(float2 UV, float2 Center, out float2 Out)
{
    Out = Cat_PolarUV(UV, Center);
}

void Cat_Twirl_float(float2 UV, float2 Center, float Strength, float Radius, out float2 Out)
{
    Out = Cat_Twirl(UV, Center, Strength, Radius);
}

void Cat_Flipbook_float(float2 UV, float Columns, float Rows, float Frame, out float2 Out)
{
    Out = Cat_Flipbook(UV, Columns, Rows, Frame);
}

void Cat_Panner_float(float2 UV, float2 Speed, float Time, out float2 Out)
{
    Out = Cat_Panner(UV, Speed, Time);
}

void Cat_Checker_float(float2 UV, float2 Cells, out float Out)
{
    Out = Cat_Checker(UV, Cells);
}

void Cat_Grid_float(float2 UV, float2 Cells, float LineWidth, out float Out)
{
    Out = Cat_Grid(UV, Cells, LineWidth);
}

void Cat_HexGrid_float(float2 UV, float Scale, out float2 CellId, out float EdgeDistance)
{
    Cat_HexGrid(UV, Scale, CellId, EdgeDistance);
}

void Cat_Fresnel_float(float3 Normal, float3 ViewDir, float Power, out float Out)
{
    Out = Cat_Fresnel(Normal, ViewDir, Power);
}

void Cat_Rim_float(float3 Normal, float3 ViewDir, float Power, float Threshold, out float Out)
{
    Out = Cat_Rim(Normal, ViewDir, Power, Threshold);
}

void Cat_Dissolve_float(float Noise, float Threshold, float EdgeWidth, out float Mask, out float Edge)
{
    Mask = Cat_Dissolve(Noise, Threshold, EdgeWidth, Edge);
}

void Cat_Hologram_float(float2 UV, float Time, float ScanCount, float ScanSpeed, out float Out)
{
    Out = Cat_Hologram(UV, Time, ScanCount, ScanSpeed);
}

void Cat_Vignette_float(float2 UV, float2 Center, float Radius, float Smoothness, out float Out)
{
    Out = Cat_Vignette(UV, Center, Radius, Smoothness);
}

void Cat_Dither_float(float Value, float2 PixelPos, float Levels, out float Out)
{
    Out = Cat_Dither(Value, PixelPos, Levels);
}

void Cat_BlendOverlay_float(float3 Base, float3 Blend, float Opacity, out float3 Out)
{
    Out = Cat_BlendOverlay(Base, Blend, Opacity);
}

void Cat_BlendScreen_float(float3 Base, float3 Blend, float Opacity, out float3 Out)
{
    Out = Cat_BlendScreen(Base, Blend, Opacity);
}

void Cat_BlendSoftLight_float(float3 Base, float3 Blend, float Opacity, out float3 Out)
{
    Out = Cat_BlendSoftLight(Base, Blend, Opacity);
}

#endif
