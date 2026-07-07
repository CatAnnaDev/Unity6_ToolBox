# CatAnnaDev Shader Library — Reference

A pure-HLSL library of small, reusable shader functions. Every function is prefixed `Cat_`, has no dependencies beyond the library itself, and touches no engine globals, no textures, no samplers, and no pipeline constant buffers. That makes the whole library **pipeline-agnostic**: the exact same code compiles and runs under HDRP, URP, the Built-in pipeline, a raw HLSL compute pass, or a Shader Graph Custom Function node. Functions are deterministic math over their arguments and nothing else.

The source is deliberately comment-free — *this document is the documentation*. If you want to know what a function does, look it up here; if you want to know exactly how it does it, read the (short) function body in the corresponding module file.

Every module is a self-contained `.hlsl` file guarded by an include guard and pulling in whatever sibling modules it needs (`Common.hlsl` underpins everything; `Noise`, `Voronoi`, and `Effects` also pull in `Hash.hlsl`). Including any single module transitively includes its dependencies, so you never have to track include order yourself.

## How to use it

There are three supported ways to pull these functions into a shader.

### 1. Handwritten HLSL shader

Include the umbrella header (or an individual module) and call the functions directly:

```hlsl
#include "Assets/CatAnnaDev/ShaderLibrary/CatShaderLibrary.hlsl"

float4 frag(Varyings i) : SV_Target
{
    float n = Cat_Fbm2(i.uv * 4.0, 5, 2.0, 0.5);
    return float4(n.xxx, 1.0);
}
```

If you only need one area, include just that module (for example `#include "Assets/CatAnnaDev/ShaderLibrary/Noise.hlsl"`); its dependencies come along automatically.

### 2. HDRP / URP Shader Graph

Add a **Custom Function** node, set its mode to **File**, point **Source** at `Assets/CatAnnaDev/ShaderLibrary/Nodes/CatNodes.hlsl`, and set **Name** to the wrapped function you want, for example `Cat_Fbm2`. Shader Graph appends the precision suffix automatically, so the entry point it links against is `Cat_Fbm2_float`. Wire the node's input ports to your graph and read its output port(s). See the [Shader Graph nodes](#shader-graph-nodes) table at the end for the exact port layout of each wrapped node.

### 3. HDRP custom pass (or any C#-driven blit / compute)

From a custom-pass shader, a fullscreen shader, or a compute kernel, include the umbrella header the same way as a handwritten shader:

```hlsl
#include "Assets/CatAnnaDev/ShaderLibrary/CatShaderLibrary.hlsl"
```

Because nothing in the library reads pipeline state, it composes cleanly on top of whatever HDRP/URP includes the pass already brings in — there are no symbol collisions with engine code (everything is `Cat_`-namespaced) and no assumptions about which render pipeline is active.

---

## Module: Common (`Common.hlsl`)

Foundational constants and math helpers used across the whole library. Defines `CAT_PI`, `CAT_TAU`, and `CAT_EPSILON` (`1e-5`).

| Function | Description | Range / notes | Usage |
|---|---|---|---|
| `float Cat_Remap(float v, float2 inMinMax, float2 outMinMax)` | Linearly remap `v` from the input interval to the output interval. | Unclamped. | `Cat_Remap(x, float2(-1,1), float2(0,1))` |
| `float Cat_Remap01(float v, float inMin, float inMax)` | Remap `v` from `[inMin,inMax]` into `[0,1]`, saturated. | Output `[0,1]`. | `Cat_Remap01(d, 0.2, 0.8)` |
| `float2x2 Cat_Rot2(float radians)` | 2D rotation matrix. | — | `mul(Cat_Rot2(a), p)` |
| `float3x3 Cat_Rot3X(float radians)` | 3D rotation matrix about X. | — | `mul(Cat_Rot3X(a), v)` |
| `float3x3 Cat_Rot3Y(float radians)` | 3D rotation matrix about Y. | — | `mul(Cat_Rot3Y(a), v)` |
| `float3x3 Cat_Rot3Z(float radians)` | 3D rotation matrix about Z. | — | `mul(Cat_Rot3Z(a), v)` |
| `float3 Cat_SafeNormalize(float3 v)` | Normalize, returning `(0,0,0)` for near-zero-length input instead of NaN. | Unit length or zero. | `Cat_SafeNormalize(normal)` |
| `float Cat_Pulse(float edge0, float edge1, float x)` | Rectangular pulse: 1 while `edge0 <= x < edge1`, else 0. | `{0,1}`. | `Cat_Pulse(0.3, 0.6, t)` |
| `float2 Cat_AspectUV(float2 uv, float aspect)` | Scale UVs about center `0.5` by `aspect` on X to correct non-square aspect ratios. | — | `Cat_AspectUV(uv, width/height)` |
| `float3 Cat_Saturate3(float3 v)` | Component-wise `saturate` on a float3. | `[0,1]³`. | `Cat_Saturate3(color)` |

## Module: Hash (`Hash.hlsl`)

Fast, texture-free pseudo-random hashes (Dave Hoskins style). All outputs are deterministic per input and land in `[0,1]`. The dimension suffix reads as `HashNM`: `N` input components, `M` output components.

| Function | Description | Range / notes | Usage |
|---|---|---|---|
| `float Cat_Hash11(float p)` | 1D → 1D hash. | `[0,1]`. | `Cat_Hash11(seed)` |
| `float Cat_Hash12(float2 p)` | 2D → 1D hash. | `[0,1]`. | `Cat_Hash12(cellId)` |
| `float Cat_Hash13(float3 p3)` | 3D → 1D hash. | `[0,1]`. | `Cat_Hash13(voxel)` |
| `float Cat_Hash21(float2 p)` | 2D → 1D hash (alias of `Cat_Hash12`). | `[0,1]`. | `Cat_Hash21(p)` |
| `float2 Cat_Hash22(float2 p)` | 2D → 2D hash. | `[0,1]²`. | `Cat_Hash22(cellId)` |
| `float3 Cat_Hash33(float3 p3)` | 3D → 3D hash. | `[0,1]³`. | `Cat_Hash33(cell)` |

## Module: Noise (`Noise.hlsl`)

Value, gradient, and simplex noise plus fractal combinators. FBM/ridged/turbulence loops are capped at `CAT_FBM_MAX_OCTAVES` (16) for a bounded, unrollable loop; `octaves` is clamped by that ceiling.

| Function | Description | Range / notes | Usage |
|---|---|---|---|
| `float Cat_ValueNoise2(float2 p)` | 2D value noise, smoothstep-interpolated. | `[0,1]`. | `Cat_ValueNoise2(uv*8)` |
| `float Cat_ValueNoise3(float3 p)` | 3D value noise. | `[0,1]`. | `Cat_ValueNoise3(p)` |
| `float Cat_GradientNoise2(float2 p)` | 2D gradient (Perlin-style) noise. | Approx `[-1,1]`. | `Cat_GradientNoise2(uv*8)` |
| `float Cat_GradientNoise3(float3 p)` | 3D gradient noise. | Approx `[-1,1]`. | `Cat_GradientNoise3(p)` |
| `float Cat_Simplex2(float2 p)` | 2D simplex noise. | Approx `[-1,1]`. | `Cat_Simplex2(uv*6)` |
| `float Cat_Fbm2(float2 p, int octaves, float lacunarity, float gain)` | Fractal Brownian motion over 2D gradient noise. | Approx `[-1,1]` at gain 0.5. | `Cat_Fbm2(p, 5, 2.0, 0.5)` |
| `float Cat_Fbm3(float3 p, int octaves, float lacunarity, float gain)` | FBM over 3D gradient noise. | Approx `[-1,1]`. | `Cat_Fbm3(p, 5, 2.0, 0.5)` |
| `float Cat_Ridged2(float2 p, int octaves)` | Ridged multifractal (fixed lacunarity 2, gain 0.5); sharp ridge lines. | Approx `[0,1]`. | `Cat_Ridged2(p, 5)` |
| `float Cat_Turbulence2(float2 p, int octaves)` | Sum of `abs` gradient noise; billowy/cloudy. | Approx `[0,1]`. | `Cat_Turbulence2(p, 5)` |
| `float2 Cat_Curl2(float2 p)` | Divergence-free curl of the 2D noise field (for flow / advection). | Vector field. | `p += Cat_Curl2(p)*dt` |

## Module: Voronoi (`Voronoi.hlsl`)

Cellular / Worley noise on a 3×3 (and, for edges, 5×5) neighborhood search.

| Function | Description | Range / notes | Usage |
|---|---|---|---|
| `void Cat_Voronoi2(float2 p, out float2 cellId, out float f1, out float f2)` | Full cellular query: nearest cell id, nearest feature distance `f1`, and second-nearest `f2`. | `f1 <= f2`, both `>= 0`. | `Cat_Voronoi2(p, id, f1, f2)` |
| `float Cat_Worley2(float2 p)` | Distance to nearest feature point (`f1` only). | `>= 0`. | `Cat_Worley2(uv*10)` |
| `float Cat_VoronoiEdges2(float2 p)` | Distance to the nearest cell boundary; small near edges, useful for crack/border masks. | `>= 0`. | `smoothstep(0,0.05,Cat_VoronoiEdges2(p))` |

## Module: SDF 2D (`Sdf2D.hlsl`)

Signed distance fields for 2D primitives (negative inside, zero on the boundary, positive outside), boolean/smooth operators, and a fill helper. `p` is the sample position relative to the shape's local origin.

| Function | Description | Range / notes | Usage |
|---|---|---|---|
| `float Cat_SdfCircle(float2 p, float r)` | Circle of radius `r`. | Signed distance. | `Cat_SdfCircle(p, 0.4)` |
| `float Cat_SdfBox(float2 p, float2 b)` | Axis-aligned box with half-extents `b`. | Signed distance. | `Cat_SdfBox(p, float2(0.3,0.2))` |
| `float Cat_SdfRoundedBox(float2 p, float2 b, float r)` | Box with corner radius `r`. | Signed distance. | `Cat_SdfRoundedBox(p, b, 0.05)` |
| `float Cat_SdfSegment(float2 p, float2 a, float2 b)` | Unsigned distance to the segment `a`–`b`. | `>= 0`. | `Cat_SdfSegment(p, a, b)` |
| `float Cat_SdfTriangle(float2 p, float r)` | Equilateral triangle of size `r`. | Signed distance. | `Cat_SdfTriangle(p, 0.4)` |
| `float Cat_SdfHexagon(float2 p, float r)` | Regular hexagon of size `r`. | Signed distance. | `Cat_SdfHexagon(p, 0.4)` |
| `float Cat_SdfStar5(float2 p, float r, float rf)` | Five-point star, outer size `r`, inner factor `rf`. | Signed distance. | `Cat_SdfStar5(p, 0.4, 0.5)` |
| `float Cat_SdfArc(float2 p, float2 apertureSinCos, float radius, float thickness)` | Arc of a given aperture (as `sin,cos`), ring radius, and stroke `thickness`. | Signed distance. | `Cat_SdfArc(p, float2(sin(a),cos(a)), 0.4, 0.02)` |
| `float Cat_SdfPie(float2 p, float2 apertureSinCos, float radius)` | Filled pie/wedge of half-aperture (`sin,cos`) and `radius`. | Signed distance. | `Cat_SdfPie(p, float2(sin(a),cos(a)), 0.4)` |
| `float Cat_OpUnion(float a, float b)` | Boolean union (`min`). | Signed distance. | `Cat_OpUnion(d1, d2)` |
| `float Cat_OpSubtract(float a, float b)` | Subtract `a` from `b`. | Signed distance. | `Cat_OpSubtract(hole, body)` |
| `float Cat_OpIntersect(float a, float b)` | Boolean intersection (`max`). | Signed distance. | `Cat_OpIntersect(d1, d2)` |
| `float Cat_OpSmoothUnion(float a, float b, float k)` | Union with smooth blend of width `k`. | Signed distance. | `Cat_OpSmoothUnion(d1, d2, 0.1)` |
| `float Cat_OpSmoothSubtract(float a, float b, float k)` | Smooth subtraction. | Signed distance. | `Cat_OpSmoothSubtract(hole, body, 0.1)` |
| `float Cat_OpSmoothIntersect(float a, float b, float k)` | Smooth intersection. | Signed distance. | `Cat_OpSmoothIntersect(d1, d2, 0.1)` |
| `float Cat_OpRound(float d, float r)` | Inflate a field by `r` (rounds corners / grows the shape). | Signed distance. | `Cat_OpRound(d, 0.05)` |
| `float Cat_OpAnnular(float d, float r)` | Turn a solid field into a ring/outline of half-width `r`. | Signed distance. | `Cat_OpAnnular(d, 0.02)` |
| `float Cat_SdfFill(float d, float smoothing)` | Convert a signed distance to an antialiased inside mask. | `[0,1]`. | `Cat_SdfFill(d, fwidth(d))` |

## Module: SDF 3D (`Sdf3D.hlsl`)

Signed distance fields for 3D primitives, for raymarching or volumetric masks. Includes `Sdf2D.hlsl`, so the 2D operators (`Cat_OpUnion`, smooth blends, `Cat_OpRound`, `Cat_OpAnnular`, …) apply to 3D fields as well.

| Function | Description | Range / notes | Usage |
|---|---|---|---|
| `float Cat_SdfSphere(float3 p, float r)` | Sphere of radius `r`. | Signed distance. | `Cat_SdfSphere(p, 1.0)` |
| `float Cat_SdfBox3(float3 p, float3 b)` | Axis-aligned box with half-extents `b`. | Signed distance. | `Cat_SdfBox3(p, float3(1,1,1))` |
| `float Cat_SdfRoundBox3(float3 p, float3 b, float r)` | Box with corner radius `r`. | Signed distance. | `Cat_SdfRoundBox3(p, b, 0.1)` |
| `float Cat_SdfTorus(float3 p, float majorRadius, float minorRadius)` | Torus in the XZ plane. | Signed distance. | `Cat_SdfTorus(p, 1.0, 0.3)` |
| `float Cat_SdfCapsule(float3 p, float3 a, float3 b, float r)` | Capsule from `a` to `b` with radius `r`. | Signed distance. | `Cat_SdfCapsule(p, a, b, 0.2)` |
| `float Cat_SdfCylinder(float3 p, float halfHeight, float r)` | Capped cylinder along Y. | Signed distance. | `Cat_SdfCylinder(p, 1.0, 0.5)` |
| `float Cat_SdfPlane(float3 p, float3 n, float h)` | Plane with unit normal `n` and offset `h`. | Signed distance. | `Cat_SdfPlane(p, float3(0,1,0), 0.0)` |
| `float Cat_SdfOctahedron(float3 p, float s)` | Octahedron of size `s`. | Signed distance. | `Cat_SdfOctahedron(p, 1.0)` |

## Module: Color (`Color.hlsl`)

Color-space conversions and per-pixel color grading. All RGB is treated as linear unless a name says otherwise; HSV/HSL channels are all `[0,1]` (hue is normalized, not degrees).

| Function | Description | Range / notes | Usage |
|---|---|---|---|
| `float3 Cat_RgbToHsv(float3 rgb)` | RGB → HSV. | HSV in `[0,1]³`. | `Cat_RgbToHsv(c)` |
| `float3 Cat_HsvToRgb(float3 hsv)` | HSV → RGB. | RGB `[0,1]³`. | `Cat_HsvToRgb(float3(h,1,1))` |
| `float3 Cat_RgbToHsl(float3 rgb)` | RGB → HSL. | HSL in `[0,1]³`. | `Cat_RgbToHsl(c)` |
| `float Cat_HueToChannel(float p, float q, float t)` | HSL-to-RGB channel helper (used by `Cat_HslToRgb`). | `[0,1]`. | internal helper |
| `float3 Cat_HslToRgb(float3 hsl)` | HSL → RGB. | RGB `[0,1]³`. | `Cat_HslToRgb(float3(h,0.5,0.5))` |
| `float Cat_Luminance(float3 rgb)` | Rec.709 luminance. | `[0,1]` for LDR input. | `Cat_Luminance(c)` |
| `float3 Cat_Grayscale(float3 rgb)` | Desaturate to luminance gray. | `[0,1]³`. | `Cat_Grayscale(c)` |
| `float3 Cat_Saturation(float3 rgb, float amount)` | Scale saturation (`0`=gray, `1`=original, `>1`=boosted). | — | `Cat_Saturation(c, 1.4)` |
| `float3 Cat_Contrast(float3 rgb, float amount)` | Contrast about mid-gray `0.5`. | — | `Cat_Contrast(c, 1.2)` |
| `float3 Cat_Brightness(float3 rgb, float amount)` | Multiply brightness. | — | `Cat_Brightness(c, 1.1)` |
| `float3 Cat_HueShift(float3 rgb, float degrees)` | Rotate hue by `degrees`. | — | `Cat_HueShift(c, 90.0)` |
| `float3 Cat_Posterize(float3 rgb, float steps)` | Quantize each channel to `steps` levels. | `[0,1]³`. | `Cat_Posterize(c, 5)` |
| `float3 Cat_ColorTemperature(float kelvin)` | Approximate blackbody color for a Kelvin temperature (clamped 1000–40000). | RGB `[0,1]³`. | `Cat_ColorTemperature(6500.0)` |
| `float3 Cat_CosinePalette(float t, float3 a, float3 b, float3 c, float3 d)` | Iñigo Quílez cosine gradient palette. | RGB. | `Cat_CosinePalette(t, a, b, c, d)` |
| `float3 Cat_LinearToGamma(float3 rgb)` | Linear → gamma (`pow 1/2.2`). | — | `Cat_LinearToGamma(c)` |
| `float3 Cat_GammaToLinear(float3 rgb)` | Gamma → linear (`pow 2.2`). | — | `Cat_GammaToLinear(c)` |

## Module: Blend (`Blend.hlsl`)

Photoshop-style layer blend modes. Every function takes `base`, `blend`, and an `opacity` that lerps between the untouched base and the fully blended result. Inputs are expected in `[0,1]`.

| Function | Description | Range / notes | Usage |
|---|---|---|---|
| `float3 Cat_BlendMultiply(float3 base, float3 blend, float opacity)` | Multiply (darkens). | `[0,1]³`. | `Cat_BlendMultiply(a, b, 1.0)` |
| `float3 Cat_BlendScreen(float3 base, float3 blend, float opacity)` | Screen (lightens). | `[0,1]³`. | `Cat_BlendScreen(a, b, 1.0)` |
| `float3 Cat_BlendOverlay(float3 base, float3 blend, float opacity)` | Overlay (contrast, base-driven). | `[0,1]³`. | `Cat_BlendOverlay(a, b, 1.0)` |
| `float3 Cat_BlendSoftLight(float3 base, float3 blend, float opacity)` | Soft light. | `[0,1]³`. | `Cat_BlendSoftLight(a, b, 1.0)` |
| `float3 Cat_BlendHardLight(float3 base, float3 blend, float opacity)` | Hard light (blend-driven overlay). | `[0,1]³`. | `Cat_BlendHardLight(a, b, 1.0)` |
| `float3 Cat_BlendColorDodge(float3 base, float3 blend, float opacity)` | Color dodge. | `[0,1]³`. | `Cat_BlendColorDodge(a, b, 1.0)` |
| `float3 Cat_BlendColorBurn(float3 base, float3 blend, float opacity)` | Color burn. | `[0,1]³`. | `Cat_BlendColorBurn(a, b, 1.0)` |
| `float3 Cat_BlendLinearDodge(float3 base, float3 blend, float opacity)` | Linear dodge / add. | `[0,1]³`. | `Cat_BlendLinearDodge(a, b, 1.0)` |
| `float3 Cat_BlendLinearBurn(float3 base, float3 blend, float opacity)` | Linear burn. | `[0,1]³`. | `Cat_BlendLinearBurn(a, b, 1.0)` |
| `float3 Cat_BlendDifference(float3 base, float3 blend, float opacity)` | Absolute difference. | `[0,1]³`. | `Cat_BlendDifference(a, b, 1.0)` |
| `float3 Cat_BlendExclusion(float3 base, float3 blend, float opacity)` | Exclusion (lower-contrast difference). | `[0,1]³`. | `Cat_BlendExclusion(a, b, 1.0)` |
| `float3 Cat_BlendLighten(float3 base, float3 blend, float opacity)` | Keep the lighter channel (`max`). | `[0,1]³`. | `Cat_BlendLighten(a, b, 1.0)` |
| `float3 Cat_BlendDarken(float3 base, float3 blend, float opacity)` | Keep the darker channel (`min`). | `[0,1]³`. | `Cat_BlendDarken(a, b, 1.0)` |
| `float3 Cat_BlendVividLight(float3 base, float3 blend, float opacity)` | Vivid light (dodge/burn by blend). | `[0,1]³`. | `Cat_BlendVividLight(a, b, 1.0)` |
| `float3 Cat_BlendPinLight(float3 base, float3 blend, float opacity)` | Pin light. | `[0,1]³`. | `Cat_BlendPinLight(a, b, 1.0)` |

## Module: UV (`Uv.hlsl`)

UV-space transforms and warps. Unless noted, functions operate on standard `[0,1]` UVs and `center` defaults conceptually to `float2(0.5, 0.5)`.

| Function | Description | Range / notes | Usage |
|---|---|---|---|
| `float2 Cat_RotateUV(float2 uv, float2 center, float radians)` | Rotate UVs about `center`. | — | `Cat_RotateUV(uv, 0.5, t)` |
| `float2 Cat_ScaleUV(float2 uv, float2 center, float2 scale)` | Scale UVs about `center`. | — | `Cat_ScaleUV(uv, 0.5, 2.0)` |
| `float2 Cat_TileUV(float2 uv, float2 tiling, float2 offset)` | `uv * tiling + offset`. | — | `Cat_TileUV(uv, 4, 0)` |
| `float2 Cat_PolarUV(float2 uv, float2 center)` | Cartesian → polar as `(radius, angle)`. | angle in `[-π,π]`. | `Cat_PolarUV(uv, 0.5)` |
| `float2 Cat_Twirl(float2 uv, float2 center, float strength, float radius)` | Swirl UVs, falling off to zero at `radius`. | — | `Cat_Twirl(uv, 0.5, 3.0, 0.5)` |
| `float2 Cat_Panner(float2 uv, float2 speed, float time)` | Scroll UVs by `speed * time`. | — | `Cat_Panner(uv, float2(0.1,0), t)` |
| `float2 Cat_Flipbook(float2 uv, float cols, float rows, float frame)` | Map UVs into one cell of a `cols×rows` sprite sheet (top-left origin). | — | `Cat_Flipbook(uv, 4, 4, f)` |
| `float2 Cat_RadialShear(float2 uv, float2 center, float2 strength)` | Distance-weighted tangential shear (ripple/lens feel). | — | `Cat_RadialShear(uv, 0.5, 0.5)` |
| `float2 Cat_MirrorUV(float2 uv)` | Mirror-repeat wrap into `[0,1]`. | `[0,1]²`. | `tex.Sample(s, Cat_MirrorUV(uv))` |
| `float2 Cat_SphereWarp(float2 uv)` | Fisheye/sphere warp of a centered UV quad. | — | `Cat_SphereWarp(uv)` |
| `float2 Cat_ParallaxOffset(float2 uv, float2 viewDirTS, float height)` | Offset UVs along tangent-space view dir for simple parallax. | — | `Cat_ParallaxOffset(uv, vTS, 0.04)` |
| `float3 Cat_TriplanarWeights(float3 normal, float sharpness)` | Normalized triplanar blend weights from a world normal. | Sums to 1. | `Cat_TriplanarWeights(n, 4.0)` |

## Module: Shapes (`Shapes.hlsl`)

Procedural pattern masks and gradients in UV space. Mask outputs are `[0,1]`.

| Function | Description | Range / notes | Usage |
|---|---|---|---|
| `float Cat_Grid(float2 uv, float2 cells, float lineWidth)` | Antialiased grid lines. | `[0,1]` (1 on lines). | `Cat_Grid(uv, 10, 0.02)` |
| `float Cat_Checker(float2 uv, float2 cells)` | Checkerboard. | `{0,1}`. | `Cat_Checker(uv, 8)` |
| `float Cat_Stripes(float2 uv, float count, float duty)` | Vertical stripes with duty cycle. | `{0,1}`. | `Cat_Stripes(uv, 10, 0.5)` |
| `float Cat_Dots(float2 uv, float2 cells, float radius)` | Grid of antialiased dots. | `[0,1]`. | `Cat_Dots(uv, 10, 0.3)` |
| `void Cat_HexGrid(float2 uv, float scale, out float2 cellId, out float edgeDist)` | Hex tiling: per-cell id and distance to cell edge. | `edgeDist` larger toward center. | `Cat_HexGrid(uv, 8, id, e)` |
| `float Cat_Brick(float2 uv, float2 cells, float offsetPerRow, float mortar)` | Offset brick pattern with mortar gaps. | `[0,1]` (0 in mortar). | `Cat_Brick(uv, float2(6,12), 0.5, 0.05)` |
| `float Cat_RadialGradient(float2 uv, float2 center, float radius)` | Radial ramp `0` at center → `1` at `radius`. | `[0,1]`. | `Cat_RadialGradient(uv, 0.5, 0.5)` |
| `float Cat_LinearGradient(float2 uv, float2 startPoint, float2 endPoint)` | Projected linear ramp between two points. | `[0,1]`. | `Cat_LinearGradient(uv, 0, float2(1,0))` |

## Module: Easing (`Easing.hlsl`)

Standard Robert Penner easing curves plus shaping helpers and periodic waves. Easing functions take a normalized `t` in `[0,1]` and return a shaped `[0,1]` (Back/Elastic overshoot slightly outside that range by design).

| Function | Description | Usage |
|---|---|---|
| `float Cat_EaseInQuad(float t)` / `Cat_EaseOutQuad` / `Cat_EaseInOutQuad` | Quadratic ease. | `Cat_EaseOutQuad(t)` |
| `float Cat_EaseInCubic(float t)` / `Cat_EaseOutCubic` / `Cat_EaseInOutCubic` | Cubic ease. | `Cat_EaseInOutCubic(t)` |
| `float Cat_EaseInQuart(float t)` / `Cat_EaseOutQuart` / `Cat_EaseInOutQuart` | Quartic ease. | `Cat_EaseOutQuart(t)` |
| `float Cat_EaseInQuint(float t)` / `Cat_EaseOutQuint` / `Cat_EaseInOutQuint` | Quintic ease. | `Cat_EaseInQuint(t)` |
| `float Cat_EaseInSine(float t)` / `Cat_EaseOutSine` / `Cat_EaseInOutSine` | Sinusoidal ease. | `Cat_EaseInOutSine(t)` |
| `float Cat_EaseInExpo(float t)` / `Cat_EaseOutExpo` / `Cat_EaseInOutExpo` | Exponential ease. | `Cat_EaseOutExpo(t)` |
| `float Cat_EaseInCirc(float t)` / `Cat_EaseOutCirc` / `Cat_EaseInOutCirc` | Circular ease. | `Cat_EaseInOutCirc(t)` |
| `float Cat_EaseInBack(float t)` / `Cat_EaseOutBack` / `Cat_EaseInOutBack` | Back ease (overshoots). | `Cat_EaseOutBack(t)` |
| `float Cat_EaseInElastic(float t)` / `Cat_EaseOutElastic` / `Cat_EaseInOutElastic` | Elastic ease (oscillates). | `Cat_EaseOutElastic(t)` |
| `float Cat_EaseInBounce(float t)` / `Cat_EaseOutBounce` / `Cat_EaseInOutBounce` | Bounce ease. | `Cat_EaseOutBounce(t)` |
| `float Cat_Gain(float x, float k)` | Symmetric contrast about `0.5`, controlled by `k`. Output `[0,1]`. | `Cat_Gain(x, 2.0)` |
| `float Cat_Bias(float x, float b)` | Push values toward 0 or 1 (`b` in `[0,1]`). Output `[0,1]`. | `Cat_Bias(x, 0.3)` |
| `float Cat_Smootherstep(float a, float b, float x)` | Ken Perlin's C2 smootherstep. Output `[0,1]`. | `Cat_Smootherstep(0, 1, x)` |
| `float Cat_TriangleWave(float x)` | Triangle wave, period 1. Output `[0,1]`. | `Cat_TriangleWave(t)` |
| `float Cat_SawWave(float x)` | Sawtooth (`frac`), period 1. Output `[0,1)`. | `Cat_SawWave(t)` |
| `float Cat_SquareWave(float x, float duty)` | Square wave with duty cycle, period 1. Output `{0,1}`. | `Cat_SquareWave(t, 0.5)` |

## Module: Dither (`Dither.hlsl`)

Ordered dithering and halftone from screen-pixel coordinates. Pass an integer-ish pixel position (e.g. `SV_Position.xy` or `uv * resolution`).

| Function | Description | Range / notes | Usage |
|---|---|---|---|
| `float Cat_Bayer4x4(float2 pixelPos)` | 4×4 ordered-dither threshold. | `[0,1)`. | `Cat_Bayer4x4(pos)` |
| `float Cat_Bayer8x8(float2 pixelPos)` | 8×8 ordered-dither threshold. | `[0,1)`. | `Cat_Bayer8x8(pos)` |
| `float Cat_OrderedDitherMask(float2 pixelPos)` | Default ordered mask (4×4). | `[0,1)`. | `Cat_OrderedDitherMask(pos)` |
| `float Cat_Dither(float value, float2 pixelPos, float levels)` | Quantize `value` to `levels` steps with Bayer dithering. | `[0,1]`. | `Cat_Dither(v, pos, 4)` |
| `float Cat_Halftone(float2 uv, float value, float scale)` | Halftone dot pattern whose dot size tracks `value`. | `[0,1]`. | `Cat_Halftone(uv, v, 40)` |

## Module: Lighting (`Lighting.hlsl`)

Lightweight, pipeline-independent shading terms. Vectors are safe-normalized internally, so raw interpolated normals/dirs are fine.

| Function | Description | Range / notes | Usage |
|---|---|---|---|
| `float Cat_Fresnel(float3 normal, float3 viewDir, float power)` | Fresnel/edge term `(1 - N·V)^power`. | `[0,1]`. | `Cat_Fresnel(n, v, 5.0)` |
| `float Cat_Rim(float3 normal, float3 viewDir, float power, float threshold)` | Thresholded rim-light mask. | `[0,1]`. | `Cat_Rim(n, v, 4.0, 0.5)` |
| `float2 Cat_MatcapUV(float3 normalVS)` | View-space normal → matcap lookup UV. | `[0,1]²`. | `tex.Sample(s, Cat_MatcapUV(nVS))` |
| `float Cat_HalfLambert(float3 normal, float3 lightDir)` | Squared half-Lambert wrap diffuse. | `[0,1]`. | `Cat_HalfLambert(n, l)` |
| `float Cat_Toon(float ndotl, float steps)` | Quantize a diffuse term into `steps` bands. | `[0,1]`. | `Cat_Toon(saturate(dot(n,l)), 3)` |
| `float Cat_FakeSSS(float3 normal, float3 lightDir, float3 viewDir, float distortion, float power)` | Cheap back-scatter / translucency term. | `[0,1]`. | `Cat_FakeSSS(n, l, v, 0.3, 4.0)` |

## Module: Effects (`Effects.hlsl`)

Higher-level screen and material effects. Includes `Hash.hlsl` for the glitch jitter.

| Function | Description | Range / notes | Usage |
|---|---|---|---|
| `float Cat_Dissolve(float noise, float threshold, float edgeWidth, out float edge)` | Dissolve mask from a noise field; `edge` is the glowing burn ring. | mask/edge `[0,1]`. | `float a = Cat_Dissolve(n, t, 0.05, edge);` |
| `float Cat_Scanline(float uvY, float count, float time, float speed)` | Animated horizontal scanline intensity. | `[0,1]`. | `Cat_Scanline(uv.y, 200, t, 1.0)` |
| `float Cat_Hologram(float2 uv, float time, float scanCount, float scanSpeed)` | Scanline plus flicker hologram mask. | `[0,1]`. | `Cat_Hologram(uv, t, 100, 1.0)` |
| `void Cat_ChromaticOffset(float2 uv, float2 center, float amount, out float2 uvR, out float2 uvG, out float2 uvB)` | Per-channel UVs for chromatic aberration sampling. | — | `Cat_ChromaticOffset(uv, 0.5, 0.01, r, g, b)` |
| `float Cat_Vignette(float2 uv, float2 center, float radius, float smoothness)` | Vignette mask, 1 at center → 0 at edges. | `[0,1]`. | `col *= Cat_Vignette(uv, 0.5, 0.4, 0.3)` |
| `float2 Cat_GlitchUV(float2 uv, float time, float amount)` | Random per-row horizontal displacement for digital glitch. | — | `Cat_GlitchUV(uv, t, 0.1)` |
| `float Cat_OutlineFromMask(float mask, float width)` | Extract a thin outline band from a `[0,1]` mask around its `0.5` edge. | `[0,1]`. | `Cat_OutlineFromMask(m, 0.05)` |

---

## Shader Graph nodes

Wrapped entry points exposed through `Nodes/CatNodes.hlsl` for use with a **Custom Function** node (File mode). Set the node **Name** to the value in the first column; Shader Graph links against the `_float` variant automatically. Ports below mirror the HLSL signatures — scalars become `Float` ports, `float2`/`float3` become `Vector 2`/`Vector 3`, and `out` parameters become output ports.

| Node name | Inputs | Outputs |
|---|---|---|
| `Cat_Remap` | `v` Float, `inMinMax` Vector 2, `outMinMax` Vector 2 | `Out` Float |
| `Cat_Remap01` | `v` Float, `inMin` Float, `inMax` Float | `Out` Float |
| `Cat_Pulse` | `edge0` Float, `edge1` Float, `x` Float | `Out` Float |
| `Cat_AspectUV` | `uv` Vector 2, `aspect` Float | `Out` Vector 2 |
| `Cat_Hash11` | `p` Float | `Out` Float |
| `Cat_Hash12` | `p` Vector 2 | `Out` Float |
| `Cat_Hash13` | `p` Vector 3 | `Out` Float |
| `Cat_Hash22` | `p` Vector 2 | `Out` Vector 2 |
| `Cat_Hash33` | `p` Vector 3 | `Out` Vector 3 |
| `Cat_ValueNoise2` | `p` Vector 2 | `Out` Float |
| `Cat_ValueNoise3` | `p` Vector 3 | `Out` Float |
| `Cat_GradientNoise2` | `p` Vector 2 | `Out` Float |
| `Cat_GradientNoise3` | `p` Vector 3 | `Out` Float |
| `Cat_Simplex2` | `p` Vector 2 | `Out` Float |
| `Cat_Fbm2` | `p` Vector 2, `octaves` Float, `lacunarity` Float, `gain` Float | `Out` Float |
| `Cat_Fbm3` | `p` Vector 3, `octaves` Float, `lacunarity` Float, `gain` Float | `Out` Float |
| `Cat_Ridged2` | `p` Vector 2, `octaves` Float | `Out` Float |
| `Cat_Turbulence2` | `p` Vector 2, `octaves` Float | `Out` Float |
| `Cat_Curl2` | `p` Vector 2 | `Out` Vector 2 |
| `Cat_Voronoi2` | `p` Vector 2 | `CellId` Vector 2, `F1` Float, `F2` Float |
| `Cat_Worley2` | `p` Vector 2 | `Out` Float |
| `Cat_VoronoiEdges2` | `p` Vector 2 | `Out` Float |
| `Cat_SdfCircle` | `p` Vector 2, `r` Float | `Out` Float |
| `Cat_SdfBox` | `p` Vector 2, `b` Vector 2 | `Out` Float |
| `Cat_SdfRoundedBox` | `p` Vector 2, `b` Vector 2, `r` Float | `Out` Float |
| `Cat_SdfHexagon` | `p` Vector 2, `r` Float | `Out` Float |
| `Cat_SdfStar5` | `p` Vector 2, `r` Float, `rf` Float | `Out` Float |
| `Cat_SdfFill` | `d` Float, `smoothing` Float | `Out` Float |
| `Cat_OpSmoothUnion` | `a` Float, `b` Float, `k` Float | `Out` Float |
| `Cat_SdfSphere` | `p` Vector 3, `r` Float | `Out` Float |
| `Cat_SdfBox3` | `p` Vector 3, `b` Vector 3 | `Out` Float |
| `Cat_SdfTorus` | `p` Vector 3, `majorRadius` Float, `minorRadius` Float | `Out` Float |
| `Cat_RgbToHsv` | `rgb` Vector 3 | `Out` Vector 3 |
| `Cat_HsvToRgb` | `hsv` Vector 3 | `Out` Vector 3 |
| `Cat_HueShift` | `rgb` Vector 3, `degrees` Float | `Out` Vector 3 |
| `Cat_Posterize` | `rgb` Vector 3, `steps` Float | `Out` Vector 3 |
| `Cat_ColorTemperature` | `kelvin` Float | `Out` Vector 3 |
| `Cat_CosinePalette` | `t` Float, `a` Vector 3, `b` Vector 3, `c` Vector 3, `d` Vector 3 | `Out` Vector 3 |
| `Cat_BlendOverlay` | `base` Vector 3, `blend` Vector 3, `opacity` Float | `Out` Vector 3 |
| `Cat_BlendSoftLight` | `base` Vector 3, `blend` Vector 3, `opacity` Float | `Out` Vector 3 |
| `Cat_RotateUV` | `uv` Vector 2, `center` Vector 2, `radians` Float | `Out` Vector 2 |
| `Cat_Twirl` | `uv` Vector 2, `center` Vector 2, `strength` Float, `radius` Float | `Out` Vector 2 |
| `Cat_Panner` | `uv` Vector 2, `speed` Vector 2, `time` Float | `Out` Vector 2 |
| `Cat_Flipbook` | `uv` Vector 2, `cols` Float, `rows` Float, `frame` Float | `Out` Vector 2 |
| `Cat_PolarUV` | `uv` Vector 2, `center` Vector 2 | `Out` Vector 2 |
| `Cat_TriplanarWeights` | `normal` Vector 3, `sharpness` Float | `Out` Vector 3 |
| `Cat_Grid` | `uv` Vector 2, `cells` Vector 2, `lineWidth` Float | `Out` Float |
| `Cat_Checker` | `uv` Vector 2, `cells` Vector 2 | `Out` Float |
| `Cat_Dots` | `uv` Vector 2, `cells` Vector 2, `radius` Float | `Out` Float |
| `Cat_HexGrid` | `uv` Vector 2, `scale` Float | `CellId` Vector 2, `EdgeDist` Float |
| `Cat_Brick` | `uv` Vector 2, `cells` Vector 2, `offsetPerRow` Float, `mortar` Float | `Out` Float |
| `Cat_RadialGradient` | `uv` Vector 2, `center` Vector 2, `radius` Float | `Out` Float |
| `Cat_Dither` | `value` Float, `pixelPos` Vector 2, `levels` Float | `Out` Float |
| `Cat_Halftone` | `uv` Vector 2, `value` Float, `scale` Float | `Out` Float |
| `Cat_Fresnel` | `normal` Vector 3, `viewDir` Vector 3, `power` Float | `Out` Float |
| `Cat_Rim` | `normal` Vector 3, `viewDir` Vector 3, `power` Float, `threshold` Float | `Out` Float |
| `Cat_MatcapUV` | `normalVS` Vector 3 | `Out` Vector 2 |
| `Cat_HalfLambert` | `normal` Vector 3, `lightDir` Vector 3 | `Out` Float |
| `Cat_Toon` | `ndotl` Float, `steps` Float | `Out` Float |
| `Cat_FakeSSS` | `normal` Vector 3, `lightDir` Vector 3, `viewDir` Vector 3, `distortion` Float, `power` Float | `Out` Float |
| `Cat_Dissolve` | `noise` Float, `threshold` Float, `edgeWidth` Float | `Mask` Float, `Edge` Float |
| `Cat_Hologram` | `uv` Vector 2, `time` Float, `scanCount` Float, `scanSpeed` Float | `Out` Float |
| `Cat_ChromaticOffset` | `uv` Vector 2, `center` Vector 2, `amount` Float | `UvR` Vector 2, `UvG` Vector 2, `UvB` Vector 2 |
| `Cat_Vignette` | `uv` Vector 2, `center` Vector 2, `radius` Float, `smoothness` Float | `Out` Float |
| `Cat_GlitchUV` | `uv` Vector 2, `time` Float, `amount` Float | `Out` Vector 2 |
