# Asteroids — Rovio Take-Home Assignment
**Rajkumar Manikindi**

---

## How to Run

1. Open the project in **Unity 6000.3.11f1 LTS**
2. Open **LoadingScene** (not GameScene directly)
3. Press Play — the loading screen will pre-warm all pools and transition automatically
4. Controls: **W / Up Arrow** — thrust, **A/D / Left/Right** — rotate, **Space** — shoot

> Starting from GameScene directly will work but skips the Addressables pre-warm step. Always start from LoadingScene for the intended experience.

---

## What I Built

All 7 modules are complete:

| Module | Feature |
|--------|---------|
| 1 | Ship movement — thrust, rotation, inertia via Rigidbody2D |
| 2 | Screen wrapping — ship and asteroids wrap all four edges |
| 3 | Shooting — bullets with fire rate cooldown, object pooling |
| 4 | Asteroid spawning — from screen edges, random direction |
| 5 | Collision detection — bullet destroys asteroid, score updates |
| 6 | Asteroid splitting — Large → 2 Medium → 2 Small, wave progression |
| 7 | Player health — 3 lives, respawn with invincibility flash, Game Over UI, restart |

---

## Project Structure

```
Assets/
  _Project/
    Scripts/          ← MonoBehaviours + Unity-dependent concrete classes
    InputActions/     ← New Input System action asset + generated C#

Packages/
  com.rajkumar.asteroids.core/
    Runtime/          ← Pure C# interfaces and logic, no MonoBehaviour dependency
    Tests/            ← EditMode unit tests + mocks
```

The core package compiles and tests independently of Unity's scene system. MonoBehaviours in Assets act as a thin layer that connects Unity's engine to the core logic — they don't own any game rules themselves.

---

## Architecture

### Loading Scene → Game Scene

All Addressable prefabs are loaded and pre-instantiated in a **LoadingScene** before the game starts. The Preloader MonoBehaviour creates pool parent GameObjects, marks them DontDestroyOnLoad, and registers itself in a lightweight **ServiceLocator** before the GameScene loads.

When GameScene starts, VContainer resolves the Preloader from the ServiceLocator — no `FindObjectOfType`, no magic strings — and PoolManager collects the pre-existing children into its queues. GameBootstrapper starts the game with everything already ready.

This means there are zero async loading calls or Instantiate calls during gameplay. No frame spikes.

### Dependency Injection — VContainer

I used VContainer as the DI framework since the brief mentioned the team uses one. Every system is registered against an interface — `IScoreSystem`, `IHealthSystem`, `IGameManager`, `IAsteroidManager` — so nothing depends on a concrete class.

The one pattern worth explaining: `AsteroidSpawner` and `AsteroidManager` are created manually in `GameBootstrapper` rather than registered in VContainer. This is intentional. They depend on runtime object pools which are populated from Addressables — their construction can't happen during VContainer's synchronous registration phase. GameBootstrapper is the composition root, and creating a small number of objects there is its job.

### Object Pooling

All bullets and asteroids are pre-instantiated in the loading scene and never created or destroyed during gameplay. `ObjectPool<T>` is a generic queue-based pool with three constructors: one for prefab-based instantiation, one for collecting pre-instantiated children from a parent transform, and one for unit tests that takes an `IEnumerable<T>` directly.

Asteroids return to their correct typed pool via a return callback set on `AsteroidSpawner`. `DeactivateSilently()` is called during pool initialisation to avoid triggering the `OnDestroyed` event on objects that haven't been used yet.

### Design Patterns

**Observer** — collision events, game state changes, wave progression. `GameManager` fires `OnGameRestart` and `OnStateChanged`; systems like `AsteroidManager` and `SpaceGun` subscribe and react independently. No system tells another what to do directly.

**Factory** — `PlayerFactory` handles player instantiation and VContainer injection. Nothing else creates players.

**Facade** — `PlayerComponents` implements `IPlayer` and exposes `ShipMovement`, `ISpaceGun`, and `RespawnSystem` as a single interface. GameBootstrapper works with `IPlayer` rather than three separate GetComponent calls.

**Service Locator** — used only for the Preloader, which must cross the scene boundary before VContainer builds. This is the only place I used it — deliberately kept narrow.

**Object Pool** — generic, typed, used for bullets and all three asteroid sizes.

### Input

Using Unity's **New Input System** with a generated C# class from an Input Action asset. `KeyboardInputProvider` implements `IInputProvider` and fires C# events. `InputManager` in the core package mediates between the provider and consumers — ship movement, space gun — so neither knows about Unity's input API directly.

### Screen Boundaries

`GameBoardBoundary` uses `ViewportToWorldPoint` rather than `ScreenToWorldPoint` with `Screen.width/height`. This avoids the pixel offset issue that appears on certain resolutions and display scales.

---

## SOLID in Practice

A few specific examples rather than just claiming it:

**Single Responsibility** — `AsteroidManager` owns wave progression and clearing. `GameManager` owns game state. They communicate via the `OnGameRestart` event so neither knows the other's internals.

**Open/Closed** — `BoundaryBase` is an abstract class. `BoundaryHandler` (wraps) and `BoundaryDestroyer` (destroys bullets) extend it without modifying it. Adding a third boundary behaviour is one new class.

**Dependency Inversion** — `ShipMovement` depends on `IInputProvider` and `IBoundaryHandler`. It has no idea whether input comes from a keyboard, gamepad, or a test mock. Same for every other consumer.

**Interface Segregation** — `IBulletBoundaryHandler` is a marker interface that extends `IBoundaryHandler`. This lets VContainer register `BoundaryHandler` and `BoundaryDestroyer` as separate types without a naming conflict, and avoids forcing one class to handle both concerns.

---

## Unit Tests

53 tests, all passing, all EditMode — no scene required.

To run: **Window → General → Test Runner → EditMode → Run All**

```
AsteroidManagerTests        8 tests  — wave progression, ClearAll, restart
HealthSystemTests           9 tests  — lives, events, reset, guard conditions
SpawnPositionProviderTests  7 tests  — bounds, edge spawning, null guard, mock verification
InputManagerTests           6 tests  — thrust/rotate state, unsubscription
BoundaryHandlerTests        8 tests  — wrapping all four edges, Z preservation
ScoreSystemTests            5 tests  — accumulation, reset, events
ObjectPoolTests             5 tests  — Get, Return, empty pool, count
+ Mocks                     5 tests  — MockProvider, MockSpawner behaviour
```

Mocks are injected through interfaces — no MonoBehaviour, no scene, no camera. `ObjectPool<T>` has a dedicated test constructor that accepts `IEnumerable<T>` so pool tests run in pure C# without touching Unity's instantiation system.

---

## Addressables

All five prefabs (Player, Bullet, LargeAsteroid, MediumAsteroid, SmallAsteroid) are marked Addressable and loaded via string keys defined in a static `AddressableKeys` class. The loading scene loads them all asynchronously, then pre-instantiates the pool objects synchronously using `WaitForCompletion()` — safe here because the assets are already in memory from the async load.

In GameScene, `WaitForCompletion()` is used only in `PlayerFactory` for the same reason: the player prefab was already cached in the loading scene.

---

## What I'd Do With More Time

**Sound** — there's no audio. I'd add a simple `IAudioService` abstraction with pooled audio sources, wired through VContainer.

**Controller and touch support** — `IInputProvider` makes this straightforward. A new concrete implementation is all that's needed, no game logic changes.

**Screenshake** — would add a `CameraShaker` that subscribes to `OnAsteroidDestroyed`. One subscriber, no coupling to anything.

**More test coverage** — `ObjectPool` growing behaviour (when prefab is available), `BoundaryDestroyer`, and integration-style tests for the full asteroid split chain.

**Visual polish** — thruster particle effect, asteroid rotation variation, bullet trail. Kept the visuals minimal to focus on architecture.

---

## Known Limitations

- Starting from GameScene directly bypasses Addressables pre-warm. The ServiceLocator will throw a clear exception explaining what happened.
- No audio.
- Asteroids spawn at a fixed offset of 1 unit outside the camera boundary. On very wide aspect ratios this offset could be visible — a proper implementation would calculate the offset based on asteroid size.
