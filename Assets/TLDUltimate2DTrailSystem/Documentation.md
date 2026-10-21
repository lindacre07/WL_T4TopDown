# 🚀 TLD Ultimate 2D Trail System - Quick Setup Guide

## 📋 Table of Contents
- [5-Minute Setup](#5-minute-setup)
- [Common Use Cases](#common-use-cases)
- [Integration Examples](#integration-examples)
- [Troubleshooting](#troubleshooting)

---

## ⚡ 5-Minute Setup

### Step 1: Import the Package
1. Copy `TLDUltimate2DTrailSystem` folder into your `Assets` folder
2. Unity will automatically compile the scripts

### Step 2: Create a Trail Material
1. Right-click in Project → Create → Material
2. Name it "TrailMaterial"
3. Set Shader to: `TLD/Ultimate 2D Trail`
4. Adjust properties:
   - Glow Intensity: 1.5
   - Edge Softness: 0.5
   - Src Blend: SrcAlpha
   - Dst Blend: One (for additive) or OneMinusSrcAlpha (for alpha)

### Step 3: Add to Your GameObject
1. Select your character/projectile GameObject
2. Add Component → TLD → Ultimate 2D Trail System → Trail Renderer
3. Assign your Trail Material to the "Trail Material" slot
4. Click a preset button (e.g., "✨ Sword Slash")
5. Done! Play the game and move your object

---

## 🎮 Common Use Cases

### 1️⃣ Character Dash Trail

**Goal**: Create a sharp, quick trail when the player dashes

```csharp
using TLD.Trail2D;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] private TLDTrailRenderer trailRenderer;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    
    private bool isDashing = false;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            StartDash();
        }
    }
    
    void StartDash()
    {
        isDashing = true;
        
        // Apply dash preset
        trailRenderer.SetPreset(TLDTrailPreset.CreateSlashEffect());
        
        // Perform dash movement (add your dash logic here)
        Vector2 dashDirection = new Vector2(Input.GetAxis("Horizontal"), 0).normalized;
        GetComponent<Rigidbody2D>().velocity = dashDirection * dashSpeed;
        
        Invoke(nameof(EndDash), dashDuration);
    }
    
    void EndDash()
    {
        isDashing = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        
        // Clear trail after dash
        Invoke(nameof(ClearTrail), 0.3f);
    }
    
    void ClearTrail()
    {
        trailRenderer.ClearTrail();
    }
}
```

**Setup**:
- Trail Type: Motion Trail
- Preset: Sword Slash
- Trail Duration: 0.3s
- Speed-Based Stretching: Enabled

---

### 2️⃣ Projectile Trail

**Goal**: Add a glowing trail behind bullets/arrows

```csharp
using TLD.Trail2D;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 5f;
    
    private TLDTrailRenderer trailRenderer;
    
    void Start()
    {
        trailRenderer = GetComponent<TLDTrailRenderer>();
        
        // Apply projectile preset
        trailRenderer.SetPreset(TLDTrailPreset.CreateMagicStreak());
        
        // Set velocity
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
        
        // Auto-destroy
        Destroy(gameObject, lifetime);
    }
    
    void OnDestroy()
    {
        // Return to pool if using pooling
        if (TLDTrailPoolManager.Instance != null)
        {
            TLDTrailPoolManager.Instance.ReturnTrail(gameObject);
        }
    }
}
```

**Setup**:
- Trail Type: Projectile Trail
- Preset: Magic Streak
- Trail Duration: 0.8s
- Glow: Enabled
- Turbulence: Slight (0.05)

---

### 3️⃣ Sword Slash Effect

**Goal**: Create a dramatic slash effect for melee attacks

```csharp
using TLD.Trail2D;
using UnityEngine;

public class SwordSlash : MonoBehaviour
{
    [SerializeField] private TLDTrailEffects trailEffects;
    [SerializeField] private Transform slashStart;
    [SerializeField] private Transform slashEnd;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PerformSlash();
        }
    }
    
    void PerformSlash()
    {
        // Use the built-in slash effect
        trailEffects.SlashEffect(slashStart.position, slashEnd.position, 0.15f);
        
        // Add your attack logic here
        // DealDamage(), PlaySound(), etc.
    }
}
```

**Setup**:
- Attach TLDTrailEffects component
- Trail Type: Motion Trail
- Preset: Sword Slash
- Tapering: Both Ends
- Width Curve: Sharp peak in middle

---

### 4️⃣ Cape/Scarf Trail

**Goal**: Flowing cloth-like trail behind character

```csharp
using TLD.Trail2D;
using UnityEngine;

public class CapeTrail : MonoBehaviour
{
    [SerializeField] private Transform capeAttachPoint;
    private TLDTrailRenderer trailRenderer;
    
    void Start()
    {
        trailRenderer = GetComponent<TLDTrailRenderer>();
        
        // Apply ribbon preset
        trailRenderer.SetPreset(TLDTrailPreset.CreateRibbonFlow());
        
        // Position at cape attach point
        transform.position = capeAttachPoint.position;
    }
    
    void LateUpdate()
    {
        // Follow the attach point
        transform.position = capeAttachPoint.position;
    }
}
```

**Setup**:
- Trail Type: Ribbon Trail
- Preset: Flowing Ribbon
- Apply Gravity: Yes (2.0)
- Apply Wind: Yes (0.5)
- Smoothing: High (6)
- Trail Duration: 0.8s

---

### 5️⃣ Racing Game Ghost Trail

**Goal**: Show previous lap ghost with semi-transparent trail

```csharp
using TLD.Trail2D;
using UnityEngine;
using System.Collections.Generic;

public class GhostRacer : MonoBehaviour
{
    [SerializeField] private TLDTrailRenderer trailRenderer;
    private List<Vector3> recordedPositions = new List<Vector3>();
    private List<float> recordedTimes = new List<float>();
    
    void Start()
    {
        trailRenderer.SetPreset(TLDTrailPreset.CreateGhostReplay());
    }
    
    void Update()
    {
        // Recording mode
        if (isRecording)
        {
            RecordPosition();
        }
        // Playback mode
        else if (isPlaying)
        {
            PlaybackPosition();
        }
    }
    
    void RecordPosition()
    {
        recordedPositions.Add(transform.position);
        recordedTimes.Add(Time.time);
    }
    
    void PlaybackPosition()
    {
        // Play back recorded positions
        // Trail will automatically show the path
    }
}
```

**Setup**:
- Trail Type: Path History Trail
- Preset: Ghost Replay
- Trail Duration: 3.0s (long)
- Opacity: 0.4 (semi-transparent)
- Max Trail Points: 200
- Ghost Replay Mode: Enabled

---

### 6️⃣ Rainbow Power-Up Trail

**Goal**: Colorful rainbow trail during power-up state

```csharp
using TLD.Trail2D;
using UnityEngine;

public class PowerUpState : MonoBehaviour
{
    [SerializeField] private TLDTrailRenderer trailRenderer;
    [SerializeField] private float powerUpDuration = 10f;
    
    private bool isPoweredUp = false;
    
    public void ActivatePowerUp()
    {
        isPoweredUp = true;
        
        // Switch to rainbow trail
        trailRenderer.SetPreset(TLDTrailPreset.CreateRainbowDash());
        
        // Deactivate after duration
        Invoke(nameof(DeactivatePowerUp), powerUpDuration);
    }
    
    void DeactivatePowerUp()
    {
        isPoweredUp = false;
        trailRenderer.ClearTrail();
    }
}
```

**Setup**:
- Trail Type: Motion Trail
- Preset: Rainbow Dash
- Rainbow Mode: Enabled
- Rainbow Speed: 0.5
- Sparkles: Enabled

---

## 🔌 Integration with Existing Systems

### With Character Controller
```csharp
// In your existing CharacterController script:
private TLDTrailRenderer trail;

void Start()
{
    trail = GetComponent<TLDTrailRenderer>();
    trail.SetPreset(TLDTrailPreset.CreateSlashEffect());
}

void OnMove()
{
    // Trail automatically follows
    // Only clear when needed
    if (isStoppedMoving)
    {
        trail.ClearTrail();
    }
}
```

### With Particle Systems
```csharp
// Combine trails with particles for extra impact
public ParticleSystem hitParticles;
public TLDTrailEffects trailEffects;

void OnHit()
{
    trailEffects.FlashColor(Color.red, 0.2f);
    hitParticles.Play();
}
```

### With Animation Events
```csharp
// Call from animation events
public void OnSlashAnimation()
{
    GetComponent<TLDTrailEffects>().SlashEffect(
        slashStart.position, 
        slashEnd.position, 
        0.2f
    );
}
```

---

## 🛠️ Advanced Customization

### Custom Preset Creation
```csharp
public TLDTrailPreset CreateCustomPreset()
{
    var preset = ScriptableObject.CreateInstance<TLDTrailPreset>();
    
    preset.presetName = "My Custom Trail";
    preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;
    
    // Setup gradient
    Gradient gradient = new Gradient();
    gradient.SetKeys(
        new GradientColorKey[] {
            new GradientColorKey(Color.cyan, 0f),
            new GradientColorKey(Color.magenta, 1f)
        },
        new GradientAlphaKey[] {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(0f, 1f)
        }
    );
    preset.colorGradient = gradient;
    
    // Setup curves
    preset.widthCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.3f);
    preset.opacityCurve = AnimationCurve.Linear(0, 1, 1, 0);
    
    // Other settings
    preset.trailDuration = 0.5f;
    preset.smoothingIterations = 3;
    
    return preset;
}
```

### Dynamic Trail Color Based on Speed
```csharp
void Update()
{
    float speed = GetComponent<Rigidbody2D>().velocity.magnitude;
    
    // Change color based on speed
    Gradient gradient = new Gradient();
    
    if (speed < 5f)
    {
        // Slow - Blue
        gradient = CreateGradient(Color.blue, Color.cyan);
    }
    else if (speed < 10f)
    {
        // Medium - Yellow
        gradient = CreateGradient(Color.yellow, Color.orange);
    }
    else
    {
        // Fast - Red
        gradient = CreateGradient(Color.red, Color.white);
    }
    
    trail.SetColorGradient(gradient);
}

Gradient CreateGradient(Color start, Color end)
{
    Gradient g = new Gradient();
    g.SetKeys(
        new GradientColorKey[] {
            new GradientColorKey(start, 0f),
            new GradientColorKey(end, 1f)
        },
        new GradientAlphaKey[] {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(0f, 1f)
        }
    );
    return g;
}
```

---

## ❗ Common Issues & Solutions

### Issue: Trail is invisible
**Solution**: 
- Check Sorting Layer and Order in Layer
- Ensure material has correct shader
- Verify Z-position for 2D camera
- Check trail duration > 0

### Issue: Trail looks blocky
**Solution**:
- Increase Smoothing Iterations (3-6)
- Decrease Min Vertex Distance (0.05-0.1)
- Increase Max Trail Points

### Issue: Performance problems
**Solution**:
- Enable Object Pooling
- Reduce Max Trail Points (50-75)
- Disable expensive effects (turbulence, sprite stamping)
- Lower Smoothing Iterations (1-2)

### Issue: Trail doesn't follow object
**Solution**:
- Trail updates in LateUpdate()
- Ensure component is on the moving GameObject
- Check if trail is accidentally disabled

---

## 🎨 Material Tips

### Glow Effect Material
```
Shader: TLD/Ultimate 2D Trail
Glow Intensity: 2.0
Glow Color: White
Edge Softness: 0.7
Blend: SrcAlpha, One (Additive)
```

### Solid Trail Material
```
Shader: TLD/Ultimate 2D Trail
Glow Intensity: 0
Edge Softness: 0.3
Blend: SrcAlpha, OneMinusSrcAlpha
```

### Sparkle Trail Material
```
Shader: TLD/Ultimate 2D Trail
Use Sparkles: Enabled
Sparkle Size: 0.05
Sparkle Speed: 1.0
Glow Intensity: 1.5
```

---

## 📊 Performance Guidelines

### Mobile (60 FPS Target)
- Max Trail Points: 50
- Smoothing Iterations: 1-2
- Trail Duration: < 0.5s
- Avoid: Turbulence, Sprite Stamping, Physics

### Desktop (144 FPS Target)
- Max Trail Points: 100
- Smoothing Iterations: 3-4
- Trail Duration: < 1.0s
- OK to use: All features moderately

### High-End (No Limit)
- Max Trail Points: 200+
- Smoothing Iterations: 6+
- Trail Duration: Any
- Use: All features freely

---

**Need more help?** Check the main README.md or contact support!

**Happy Trailing!** ✨
