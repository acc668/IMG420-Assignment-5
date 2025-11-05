# IMG420-Assignment-5

A Godot 4 C# project demonstrating custom shaders, physics simulations, and raycasting systems.

## Project Overview

This project implements three core advanced features in Godot Engine:
1. **Custom Particle Shader** - GPU particles with animated wave distortion and color gradients
2. **Physics Chain System** - Rigid body segments connected with pin joints
3. **Laser Detection System** - Raycasting with visual feedback and collision detection

## Features

### Part 1: Custom Canvas Item Shader with Particles
- GPU-accelerated particle system with 100 particles
- Custom GLSL shader applying wave distortion effects
- Dynamic color gradient transitioning from orange to pink
- Time-based animation with pulsing intensity
- Shader parameters exposed for runtime modification

### Part 2: Physics Chain with Joints
- 5 interconnected rigid body segments
- Static anchor point (first segment)
- Pin joints connecting all segments
- Realistic physics simulation with gravity
- Alternating visual colors for segment identification
- Force application system for interactive testing

### Part 3: Raycasting Laser Detection
- Continuous horizontal raycast at 500 pixel length
- Line2D visualization of laser beam
- Real-time player detection
- Visual alarm system (color change: green → red)
- Console logging for detection events
- Automatic alarm reset when player leaves beam

## Setup Instructions

### Prerequisites
- **Godot 4.x** with .NET support
- **.NET SDK 9.0** or higher
- C# development environment

### Installation

1. **Clone or download this repository**
   ```
   git clone (https://github.com/acc668/IMG420-Assignment-5)
   cd [project-folder]
   ```

2. **Open in Godot**
   - Launch Godot 4.x (with .NET support)
   - Click "Import"
   - Navigate to project folder
   - Select `project.godot`
   - Click "Import & Edit"

3. **Build the project**
   - Click **Build** → **Build Project** (top-right)
   - Wait for "Build succeeded" message
   - All C# scripts will be compiled

4. **Run the project**
   - Press **F5** or click the Play button
   - Use **WASD** or arrow keys to move the player

## Controls

- **W / ↑** - Move Up
- **S / ↓** - Move Down
- **A / ←** - Move Left
- **D / →** - Move Right
- **F5** - Run project
- **F6** - Run current scene

## Technical Implementation

### Part 1: How the Shader Works

The custom particle shader (`custom_particle.gdshader`) creates dynamic visual effects through several techniques:

#### Wave Distortion
```glsl
uv.x += sin(uv.y * wave_frequency + TIME * 2.0) * wave_intensity;
uv.y += cos(uv.x * wave_frequency * 0.5 + TIME * 1.5) * wave_intensity * 0.5;
```
- Uses sine and cosine functions to offset UV coordinates
- `TIME` variable provides continuous animation
- `wave_frequency` (10.0) controls the number of waves
- `wave_intensity` (0.1) controls distortion strength
- Creates flowing, organic motion in the particles

#### Color Gradient
```glsl
float gradient = uv.y;
vec4 color = mix(color_start, color_end, gradient);
```
- Interpolates between two colors based on vertical UV position
- `color_start`: Orange (RGB: 1.0, 0.5, 0.0)
- `color_end`: Pink (RGB: 1.0, 0.0, 0.5)
- Creates smooth color transition across each particle

#### Pulsing Animation
```glsl
float pulse = sin(TIME * 3.0) * 0.2 + 0.8;
color.rgb *= pulse;
```
- Brightness oscillates between 60% and 100%
- Creates a "breathing" effect
- Adds visual interest and depth

#### Runtime Animation
The `ParticleController.cs` dynamically updates shader parameters:
```csharp
float waveIntensity = 0.1f + Mathf.Sin(_time * 2f) * 0.05f;
_shaderMaterial.SetShaderParameter("wave_intensity", waveIntensity);
```
- Wave intensity varies between 0.05 and 0.15
- Color shift creates animated color transitions
- All parameters controllable in real-time

### Part 2: Physics Properties Explained

The physics chain demonstrates realistic rope/chain simulation through carefully chosen properties:

#### Segment Configuration
```csharp
[Export] public int ChainSegments = 5;
[Export] public float SegmentDistance = 30f;
[Export] public float SegmentWidth = 20f;
[Export] public float SegmentHeight = 40f;
```

**Why these values:**
- **5 segments**: Enough to show physics without being computationally expensive
- **30px spacing**: Creates visible gaps between segments
- **20x40 size**: Rectangle shape mimics chain links

#### Mass and Gravity
```csharp
rigidBody.Mass = 1.0f;
rigidBody.GravityScale = 1.0f;
```

**Why Mass = 1.0:**
- Uniform mass creates predictable, consistent behavior
- Each segment responds proportionally to forces
- Lighter segments would float, heavier would be sluggish
- 1.0 provides good balance

**Why GravityScale = 1.0:**
- Standard gravity creates natural hanging motion
- Matches realistic expectations for chain behavior
- Segments swing and oscillate naturally when disturbed

#### Joint Properties
```csharp
joint.Softness = 0.1f;
joint.Bias = 0.3f;
```

**Why Softness = 0.1:**
- Low softness creates stiff connections
- Prevents excessive stretching or wobbling
- Maintains chain integrity during motion
- 0.0 would be completely rigid (unrealistic)
- Higher values would make it too elastic

**Why Bias = 0.3:**
- Controls constraint solving speed
- 0.3 balances stability and responsiveness
- Lower values = slower correction (more drift)
- Higher values = faster correction (can be jittery)
- This value prevents segments from separating while maintaining smooth motion

#### Static Anchor
```csharp
if (i == 0)
{
    segment = new StaticBody2D();  // First segment is static
}
```

**Why use StaticBody2D:**
- Anchors the chain to a fixed point
- Prevents the entire chain from falling
- Simulates attachment to ceiling/wall
- Creates realistic hanging behavior

#### Visual Design
```csharp
colorRect.Color = i % 2 == 0 ? new Color(0.2f, 0.6f, 0.8f) : new Color(0.8f, 0.4f, 0.2f);
```
- Alternating colors (blue/orange) make individual segments distinguishable
- Helps visualize the physics behavior and segment connections

### Part 3: Raycast Detection System

The laser detection system demonstrates raycasting for collision detection and player tracking:

#### Raycast Configuration
```csharp
_rayCast = new RayCast2D();
_rayCast.Enabled = true;
_rayCast.TargetPosition = new Vector2(LaserLength, 0);  // 500 pixels horizontal
_rayCast.CollisionMask = 1;  // Detect objects on layer 1
```

**How it works:**
- Creates a ray starting from LaserSystem position
- Extends 500 pixels to the right (horizontal)
- Checks for collisions every physics frame
- Only detects objects on collision layer 1 (the player)

#### Continuous Detection
```csharp
public override void _PhysicsProcess(double delta)
{
    _rayCast.ForceRaycastUpdate();  // Update ray each frame
    bool isColliding = _rayCast.IsColliding();
```

**Why _PhysicsProcess:**
- Physics calculations happen at fixed intervals (typically 60 FPS)
- More reliable for collision detection than _Process
- Ensures consistent detection regardless of frame rate

**Why ForceRaycastUpdate:**
- Forces immediate recalculation of raycast
- Without this, detection would lag by one frame
- Critical for real-time response

#### Player Detection Logic
```csharp
var collider = _rayCast.GetCollider();
if (collider is Node node && IsPlayerOrChild(node))
{
    if (!_isAlarmActive)
    {
        TriggerAlarm();
    }
}
```

**IsPlayerOrChild method:**
```csharp
private bool IsPlayerOrChild(Node node)
{
    Node current = node;
    while (current != null)
    {
        if (current == _player)
            return true;
        current = current.GetParent();
    }
    return false;
}
```

**Why this approach:**
- Handles collisions with player's child nodes (CollisionShape2D, ColorRect)
- Walks up the node tree to find the player
- More robust than simple type checking
- Prevents false negatives when hitting child collision shapes

#### Visual Feedback
```csharp
private void UpdateLaserBeam(Vector2 endPoint)
{
    _laserBeam.SetPointPosition(0, Vector2.Zero);
    _laserBeam.SetPointPosition(1, endPoint);
}
```

**Dynamic beam visualization:**
- Line2D shows the exact ray path
- Extends to collision point if hitting something
- Shows full length (500px) when clear
- Updates every frame for smooth visualization

#### Alarm System
```csharp
private void TriggerAlarm()
{
    _isAlarmActive = true;
    _laserBeam.DefaultColor = LaserColorAlert;  // Red
    _alarmTimer.Start();
    GD.Print("ALARM! Player detected!");
}
```

**Multi-modal feedback:**
- **Visual**: Laser color changes (green → red)
- **Visual 2**: Flashing ColorRect overlay
- **Console**: Alarm messages logged
- **Temporal**: Timer-based pulsing for continuous alarm

**Automatic reset:**
```csharp
if (_isAlarmActive)
{
    ResetAlarm();  // When player leaves
}
```
- Alarm resets when player exits beam
- Returns to normal (green) state
- Prevents stuck alarm states

## Project Structure

```
project/
├── scripts/
│   ├── ParticleController.cs      # Particle system with shader
│   ├── PhysicsChain.cs            # Chain physics implementation
│   ├── LaserDetector.cs           # Raycast laser system
│   └── Player.cs                  # Player movement controller
├── custom_particle.gdshader       # Custom particle shader
├── main.tscn                      # Main scene
└── project.godot                  # Project configuration
```

## Scene Hierarchy

```
Main (Node2D)
├── ParticleSystem (GPUParticles2D)  [Position: 200, 100]
│   └── Script: ParticleController.cs
├── PhysicsChain (Node2D)            [Position: 400, 50]
│   └── Script: PhysicsChain.cs
├── LaserSystem (Node2D)             [Position: 100, 300]
│   └── Script: LaserDetector.cs
└── Player (CharacterBody2D)         [Position: 300, 300]
    ├── CollisionShape2D
    ├── ColorRect (Visual)
    └── Script: Player.cs
```
