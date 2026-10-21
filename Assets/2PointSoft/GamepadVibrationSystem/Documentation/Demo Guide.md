# Demo Guide

## Main Feature Demo

`Demo/Scenes/GamepadVibrationDemo.unity`

Shows components, UnityEvents, built-in presets, direct C# calls, triggers, recoil, projectile impact, pulsed effects, and continuous machinery feedback.

## Local Multiplayer Demo

`Demo/Scenes/GamepadVibrationLocalMultiplayerDemo.unity`

Connect up to four gamepads. Each player joins by pressing A/Cross, and the demo stores that exact `Gamepad` reference in the player's slot. Effects target the stored device directly instead of relying on a changing `Gamepad.all` index. Disconnected slots are released so a replacement controller can join. Runtime APIs also support `PlayerInput` targeting for projects that use Unity's player-joining workflow.

## Profile Demo

`Demo/Scenes/GamepadVibrationProfilesDemo.unity`

The Hierarchy and Inspector expose three explicit examples:

1. **Sword Attack** — One Shot.
2. **Shotgun Recoil** — Pulsed.
3. **Engine Idle** — Continuous with separate Start and Stop.

Enter Play Mode with a connected gamepad. Each profile has its own visible card and action. To replace an example, exit Play Mode, select **Custom Vibration Profiles Demo**, and assign another `GamepadVibrationProfile` to the named field.
