# Changelog

## 1.0.0

- Standalone Unity Input System runtime with no Game Creator dependency.
- One Shot, Pulsed, and real Continuous playback.
- OneShot profiles and generated profiles always play exactly once, regardless of a retained Pulses value; Pulsed settings remain unchanged.
- Per-device mixing, handles, ownership, selective stop, and disconnect cleanup.
- Idle-device release after the final request, preventing this system from overwriting other rumble providers with zero every frame.
- Time-based HID stop confirmation retries zero motor output briefly before releasing ownership, preventing intermittent stuck rumble on DualSense USB when an output IOCTL is still in flight.
- Multiplayer demo slot recovery after controller disconnection and demo-owned selective cleanup.
- Render-pipeline-neutral demo shader included inside the package; no URP shader or material dependency.
- Unscaled-time playback and historical manager APIs.
- Fast Enter Play Mode safety through an explicit `SubsystemRegistration` reset of mutable manager static state.
- Protected built-in presets and reusable custom profile assets.
- Gamepad and `PlayerInput` local multiplayer targeting.
- Central Profile Manager with searchable master-detail library, full-pane creation, explicit Save/Reset, Play Mode preview, and API status.
- Optional typed generated C# profile API.
- Main, local multiplayer, and explicit profile demos.
