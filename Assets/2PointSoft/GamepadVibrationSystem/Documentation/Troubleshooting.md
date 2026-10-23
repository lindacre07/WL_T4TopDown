# Troubleshooting

## No vibration

- Confirm the Input System package is installed and active.
- Connect a supported gamepad and press a button so it becomes `Gamepad.current`.
- Confirm the device supports standard rumble through Unity Input System.
- Enter Play Mode before using Profile Manager preview controls.

## Wrong local player vibrates

Use `Play(playerInput)` or the component's `PlayerInput` target mode. Do not map players with `Gamepad.all[playerIndex]`.

## Continuous vibration does not stop

Store and stop its handle, or call `Stop()` on the component that owns it:

```csharp
GamepadVibrationHandle handle = profile.Play(playerInput);
handle.Stop();
```

## Generated member is missing

Open the Profile Manager. If the API button is orange or red, click **Update API** or **Generate API** and wait for Unity to compile.

## DualSense notes

Standard rumble was validated by USB on Windows. Bluetooth behavior depends on Unity Input System and platform support. Adaptive triggers and advanced DualSense haptics are outside this package's scope.
