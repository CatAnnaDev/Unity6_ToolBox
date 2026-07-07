# CatAnnaDev

A drop-in toolkit of developer quality-of-life systems for Unity. Pure C#, zero external dependencies, portable.

- Unity 6.2 (6000.5) / C# 9 / netstandard2.1
- Root namespace: `CatAnnaDev`
- 191 C# scripts across 13 runtime systems, complete editor tooling, 13 runnable demos, plus a 160-function HLSL shader library
- New Input System only (no legacy `UnityEngine.Input`), so nothing forces a project config change
- Verified: runtime, editor and samples assemblies compile clean against the Unity 6.2 managed assemblies; every `.hlsl` compiles clean under glslang

## Layout

```
Assets/CatAnnaDev/
  Runtime/        CatAnnaDev           core systems (portable, no external deps)
  Editor/         CatAnnaDev.Editor    inspectors, windows, property drawers
  Samples/        CatAnnaDev.Samples   runnable demos (references Unity.InputSystem)
  ShaderLibrary/  (HLSL, no assembly)  reusable pure-HLSL functions + reference + examples
  Resources/      (created on demand)  CatAnnaDevSettings asset lives here
```

Three assembly definitions keep editor and sample code out of your runtime builds; the core is pure C# with zero external dependencies. `ShaderLibrary/` is plain `.hlsl` (no assembly, no C#) that any shader can `#include`. Delete the `Samples/` folder any time; nothing else depends on it.

## Quick start

1. `Tools > CatAnnaDev > Create Settings Asset` (optional; the pack works with sane defaults if you skip it).
2. `Tools > CatAnnaDev > Samples > *` to drop any demo into the current scene, then press Play.
3. Start calling the API. Every subsystem is usable from a fresh scene with no setup.

```csharp
using CatAnnaDev.Pooling;
using CatAnnaDev.Events;
using CatAnnaDev.Tweening;

Pool.Prewarm(bulletPrefab, 32);
GameObject b = Pool.Spawn(bulletPrefab, muzzle.position, muzzle.rotation);
Pool.Despawn(b, 2.5f);

EventBus.Raise(new EnemyKilled { Points = 100 });

transform.TweenPosition(target, 0.4f).SetEase(Ease.OutBack);
```

## Systems

### Pooling (`CatAnnaDev.Pooling`)
Allocation-free object pooling. Static `Pool` facade over a `PoolManager`, generic `ObjectPool<T>` / `LinkedPool<T>` for plain C# objects, `GameObjectPool` for prefabs, `PoolConfig` / `PoolRegistry` ScriptableObjects, `IPoolable` spawn/despawn hooks, `AutoDespawn` (lifetime / particle-stop / audio-stop), and live per-pool stats.

```csharp
using CatAnnaDev.Pooling;

Pool.Prewarm(bulletPrefab, 32);
GameObject go = Pool.Spawn(bulletPrefab, muzzle.position, muzzle.rotation);
go.GetComponent<Rigidbody>().linearVelocity = muzzle.forward * 20f;
Pool.Despawn(go, 2.5f);

PoolManager.Instance.TryGetStats(bulletPrefab, out PoolStats s);
```
Demo: PoolingDemo. Editor: Pool Diagnostics window (`Tools > CatAnnaDev > Pool Diagnostics`), PoolConfig inspector.

### Events (`CatAnnaDev.Events`)
Strongly-typed, allocation-free static `EventBus`, plus ScriptableObject `GameEvent` / `GameEvent<T>` channels with `GameEventListener` components.

```csharp
using CatAnnaDev.Events;

public struct DamageEvent : IEvent { public int Amount; }

void OnEnable()  => EventBus.Register<DamageEvent>(OnDamage);
void OnDisable() => EventBus.Unregister<DamageEvent>(OnDamage);
void OnDamage(DamageEvent e) => Debug.Log(e.Amount);

EventBus.Raise(new DamageEvent { Amount = 10 });
```
Demo: EventsDemo. Editor: GameEvent inspector with a play-mode Raise button.

### Services (`CatAnnaDev.Services`)
`ServiceLocator` (register/resolve/try-get), `MonoSingleton<T>`, `PersistentSingleton<T>`, `SingletonScriptableObject<T>`, and a `Bootstrapper` for ordered init.

```csharp
using CatAnnaDev.Services;

ServiceLocator.Register<IScoreService>(new ScoreService());
if (ServiceLocator.TryGet<IScoreService>(out var score)) score.Add(10);
GameClock clock = GameClock.Instance;
```
Demo: ServicesDemo. Editor: Services window (`Tools > CatAnnaDev > Services`).

### State Machine (`CatAnnaDev.StateMachine`)
Engine-agnostic FSM with a fluent builder, predicate transitions, any-transitions, a blackboard, and a hierarchical variant. Optional `StateMachineRunner` MonoBehaviour.

```csharp
using CatAnnaDev.StateMachine;

var builder = new StateMachineBuilder();
builder.State("Idle").OnTick(TickIdle).TransitionTo("Chase").When(() => SeesPlayer());
builder.State("Chase").OnTick(TickChase).TransitionTo("Idle").When(() => !SeesPlayer());
builder.Initial("Idle");
var machine = builder.Build();
machine.OnStateChanged += (from, to) => Debug.Log(from + " -> " + to);
void Update() => machine.Tick();
```
Demo: StateMachineDemo. Editor: live current-state display on StateMachineRunner.

### Timers (`CatAnnaDev.Timers`)
MonoBehaviour-free timers driven by a single central updater: `CountdownTimer`, `StopwatchTimer`, `FrequencyTimer`, scaled/unscaled time, no per-tick allocation.

```csharp
using CatAnnaDev.Timers;

var t = new CountdownTimer(3f);
t.OnComplete += () => Debug.Log("boom");
t.Start();
```
Demo: TimersDemo.

### Tweening (`CatAnnaDev.Tweening`)
Compact, chainable tweening with the full easing set, loop modes, and fluent extensions on Transform / Material / CanvasGroup, plus a generic `Tweener.To`.

```csharp
using CatAnnaDev.Tweening;

transform.TweenPosition(target, 1.25f).SetEase(Ease.OutBack).SetLoops(-1, LoopType.PingPong);
material.TweenColor(Color.red, 1f);
Tweener.To(0f, 1f, 1f, v => slider.value = v);
```
Demo: TweeningDemo.

### Scheduling (`CatAnnaDev.Scheduling`)
`FrameScheduler` (NextFrame / After / Every with cancelable handles) and a `MainThreadDispatcher` to marshal work back from background threads.

```csharp
using CatAnnaDev.Scheduling;

FrameScheduler.After(1.5f, () => Debug.Log("later"));
ScheduledHandle beat = FrameScheduler.Every(0.5f, Tick);
beat.Cancel();
MainThreadDispatcher.Enqueue(() => Debug.Log("main thread"));
```
Demo: SchedulingDemo.

### Saving (`CatAnnaDev.Saving`)
Slot-based save system with pluggable serializers (JSON / binary) and encryptors (XOR / AES / none), async IO marshaled to the main thread, atomic writes (temp-then-move), and `SaveableEntity` auto-registration.

```csharp
using CatAnnaDev.Saving;

SaveSystem.Save("slot0");
SaveSystem.Load("slot0");
if (SaveSystem.HasSlot("slot0")) SaveSystem.Delete("slot0");
```
Demo: SavingDemo. Editor: Save System window (`Tools > CatAnnaDev > Save System`).

### Scriptable Architecture (`CatAnnaDev.ScriptableArchitecture`)
ScriptableObject variables (Float / Int / Bool / String / Vector2 / Vector3 / Color / GameObject), constant-or-variable `*Reference` structs, and `RuntimeSet` collections with membership components.

```csharp
using CatAnnaDev.ScriptableArchitecture;

var health = ScriptableObject.CreateInstance<FloatVariable>();
health.OnValueChanged += v => Debug.Log(v);
health.Add(-10f);
FloatReference damage = new FloatReference(health);
```
Demo: ScriptableArchitectureDemo. Editor: live runtime-value inspector with reset-to-initial.

### Audio (`CatAnnaDev.Audio`)
`AudioManager` with pooled voices (built on the Pool facade), `SoundData` assets, voice limiting, cooldowns, ducking, and music crossfade. Works with only a `SoundData` asset; emitters are created for you.

```csharp
using CatAnnaDev.Audio;

AudioManager.Instance.PlaySound2D(sfx);
AudioManager.Instance.PlaySoundAtPosition(sfx, transform.position);
AudioManager.Instance.CrossfadeMusic(track, 2f);
AudioManager.Instance.DuckMusic(0.7f, 1f);
```
Demo: AudioDemo. Editor: SoundData inspector with a Preview button.

### VFX (`CatAnnaDev.VFX`)
`VFXManager` plays pooled particle effects that auto-return when their non-looping system finishes (`PooledParticle`). `VFXData` assets, prewarming, follow-target.

```csharp
using CatAnnaDev.VFX;

VFXManager.Instance.Prewarm(explosionPrefab, 8);
VFXManager.Instance.Play(explosionData, hit.point, hit.normal);
```
Demo: VFXDemo. Editor: VFXData inspector with prefab validation.

### Utils (`CatAnnaDev.Utils`)
The big grab-bag (45 scripts): extension methods (Transform / GameObject / Vector / Color / Collection / String / LayerMask / ...), math and random helpers, `WeightedList`, `CircularBuffer`, `PriorityQueue`, `SerializableDictionary`, `SerializableGuid`, `Optional<T>`, `MinMaxRange`, `WaitCache`, `CoroutineRunner`, `Cooldown`, `SceneLoader`, `FPSCounter`, collection pools (`ListPool` / `DictionaryPool` / `HashSetPool` / `StringBuilderPool`), and a full set of inspector attributes.

```csharp
using CatAnnaDev.Utils;

float x = MathUtils.Remap(mouseX, 0f, Screen.width, -5f, 5f);
loot.Shuffle();
string pick = loot.RandomElement();
using (ListPool<int>.Get(out var scratch)) { scratch.Add(1); }
CoroutineRunner.RunDelayed(1f, () => Debug.Log("done"));
```

Inspector attributes (drawers included): `[ReadOnly]`, `[MinMaxSlider(min,max)]`, `[ShowIf]`, `[HideIf]`, `[EnableIf]`, `[Required]`, `[TagSelector]`, `[LayerSelector]`, `[SceneSelector]`, `[InfoBox]`, `[Title]`, `[Expandable]`, `[ProgressBar]`, `[Button]`.

```csharp
using CatAnnaDev.Utils;

[Title("Stats")]
[MinMaxSlider(0f, 100f)] public Vector2 damageRange;
[ShowIf("usesShield")] public float shield;
public bool usesShield;
[Button] private void FullHeal() { }
```

Demos: UtilsDemo and AttributesShowcase (select it in the Inspector to see every drawer).

## HLSL shader library (`Assets/CatAnnaDev/ShaderLibrary`)

160 reusable HLSL functions for writing shaders. Every function is pure math - no textures, no samplers, no engine globals, no Unity macros - so the exact same code compiles under HDRP, URP, Built-in, a compute pass, or a Shader Graph node. All functions are prefixed `Cat_`. The `.hlsl` files carry no comments; `ShaderLibrary/REFERENCE.md` is the full function reference.

| Module | What's inside |
|---|---|
| `Common`, `Hash`, `Easing` | remap, rotations, deterministic hashes, full easing set, wave shapers |
| `Noise`, `Voronoi` | value / gradient / simplex noise, fbm, ridged, turbulence, curl, voronoi F1/F2/edges |
| `Sdf2D`, `Sdf3D` | signed-distance primitives (circle, box, hexagon, star, torus, capsule...) + smooth ops (union, subtract, round, annular) |
| `Color`, `Blend` | RGB/HSV/HSL, hue shift, posterize, cosine palette, color temperature + 15 Photoshop blend modes |
| `Uv`, `Shapes` | rotate / polar / twirl / panner / flipbook / triplanar weights + grid / checker / dots / hex grid / brick |
| `Dither`, `Lighting`, `Effects` | bayer / halftone, fresnel / rim / matcap / toon / fake SSS, dissolve / hologram / scanline / vignette / glitch |

```hlsl
#include "Assets/CatAnnaDev/ShaderLibrary/CatShaderLibrary.hlsl"

float n = Cat_Fbm2(uv * 4.0, 5, 2.0, 0.5);
float mask = Cat_SdfFill(Cat_SdfHexagon(p, 0.4), 0.01);
float3 col = Cat_HueShift(baseColor, 40.0);
```

Three ways to use it:
1. **Handwritten shader / HDRP custom pass** - `#include` the umbrella `CatShaderLibrary.hlsl` (or a single module) and call the functions.
2. **Shader Graph (HDRP/URP)** - add a **Custom Function** node, File mode, Source `ShaderLibrary/Nodes/CatNodes.hlsl`, Name e.g. `Cat_Fbm2` (Shader Graph appends `_float`), then wire the inputs.
3. **Examples** - `ShaderLibrary/Examples/` has three Built-in-RP unlit shaders (noise, dissolve, hologram) that `#include` the library.

Verified: all 16 `.hlsl` files compile clean under the glslang HLSL compiler. The three example `.shader` files are ShaderLab (not glslang-checkable) - confirm them by eye in the editor.

## Running the demos

Fastest path: `Tools > CatAnnaDev > Samples > <name>` creates a GameObject with that demo and frames it. Or add any `*Demo` component to an empty GameObject yourself. Then press Play. Each demo builds its own camera, lights and objects from code and shows an on-screen panel with the controls and live status. Run one demo at a time (each creates its own camera).

## Editor tooling

- `Tools > CatAnnaDev` menu: settings, about, and the diagnostic windows below.
- Windows: Pool Diagnostics, Services, Save System.
- Custom inspectors: PoolConfig, GameEvent, ScriptableArchitecture variables, SoundData, VFXData, StateMachineRunner.
- Project Settings > CatAnnaDev panel for the pack settings.
- Property drawers for every inspector attribute above.

## Notes

- `[Button]` on a method renders in the Inspector only when the scripting define `CATANNADEV_GLOBAL_INSPECTOR` is set (Project Settings > Player > Scripting Define Symbols). It is off by default so it never hijacks other custom inspectors. All other attribute drawers work automatically.
- Pooling and the manager singletons are main-thread only. `ObjectPool<T>` / `LinkedPool<T>` are not internally synchronized; use `MainThreadDispatcher` to hop threads.
- The pack logs through `CatLog`, filtered by `CatAnnaDevSettings` (log level, in-build logging, color).
- Everything is pure C# with no external package dependencies. If you ever hit a measured hot path that needs native speed, add a Burst job or an FFI module at that spot only.
