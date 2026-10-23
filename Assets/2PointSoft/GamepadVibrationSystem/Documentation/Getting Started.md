# Getting Started

## Installation

Import the `Assets/2PointSoft/GamepadVibrationSystem` folder and ensure the Unity Input System package is installed. Demo scenes also require Unity UGUI.

## Component Workflow

1. Add **Gamepad Vibration** from `Add Component > 2 Point Soft > Gamepad Vibration`.
2. Choose direct settings, a built-in preset, or a reusable vibration profile.
3. Connect `Play()` and `Stop()` to UnityEvents or call them from C#.
4. For local multiplayer, choose `PlayerInput` targeting and assign the player's `PlayerInput` component.

## Profile Workflow

Open `Tools > 2 Point Soft > Vibration Profile Manager`.

- **From Scratch** starts with neutral values.
- **Copy Built In** copies a protected default.
- **Copy Existing** copies a profile asset selected with Unity's object picker.

Configure all settings in the full right panel, create the asset, then use **Save Profile** for edits. **Reset Unsaved Changes** restores the last saved state. Preview controls work in Play Mode only.

## Generated API

The generated API is optional. Profile assets and components work without it. Use **Generate API** or **Update API** in the Profile Manager to create:

```text
Assets/GamepadVibrationGenerated/GamepadVibrationCustom.cs
```

Example:

```csharp
using TwoPointSoft.GamepadVibration.Generated;

GamepadVibrationCustom.SwordAttack.Play();
```
