# API Reference

## Historical Convenience API

```csharp
GamepadVibrationManager.Play(0.5f, 0.8f, 0.2f, fadeOut: true);
GamepadVibrationManager.StopAll();
```

APIs without an explicit target use `Gamepad.current`.

## Settings and Presets

```csharp
GamepadVibrationPresets.HeavyHit.Play();

GamepadVibrationSettings custom = GamepadVibrationPresets.HeavyHit.With(
    duration: 0.35f,
    globalIntensity: 0.8f);
GamepadVibrationHandle handle = custom.Play();
```

## Explicit Targeting

```csharp
profile.Play(gamepad);
profile.Play(playerInput);
GamepadVibrationPresets.Alert.Play(playerInput);
```

Explicit invalid targets do not fall back to another player's gamepad.

## Continuous Playback

```csharp
GamepadVibrationHandle engine =
    GamepadVibrationManager.PlayContinuous(0.45f, 0.15f, gamepad);

engine.Stop();
```

Use the returned handle to stop only the owned request. Avoid `StopAll()` when unrelated effects may be active.

## Generated Profiles

```csharp
GamepadVibrationCustom.SwordAttack.Play();
GamepadVibrationCustom.ShotgunRecoil.Play(playerInput);
GamepadVibrationHandle idle = GamepadVibrationCustom.EngineIdle.Play(gamepad);
idle.Stop();
```

## Time and Mixing

Playback uses unscaled time. Simultaneous requests are mixed per target device. Disabling a component stops only requests owned by that component.
