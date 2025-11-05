# IMG420-Assignment-5

A Godot 4 C# project demonstrating custom shaders, rigid body physics, and raycasting.

## Features

This project implements three advanced Godot features:

1. **Custom Particle Shader** - Animated particles with wave distortion and color gradients
2. **Physics Chain** - Connected rigid bodies using pin joints
3. **Laser Detection** - Raycasting system that detects player movement

---

## Setup & Controls

### Requirements
- Godot 4.x with .NET support
- .NET SDK 9.0 or higher

### Installation
1. Open project in Godot 4.x (with .NET)
2. Click **Build** → **Build Project**
3. Press **F5** to run

### Controls
- **WASD** or **Arrow Keys** - Move player
- Walk through the green laser to trigger alarm
- Walk into the chain to make it swing

---

## Part 1: Custom Shader Explanation

### How the Shader Works

The custom shader (`custom_particle.gdshader`) creates animated visual effects using three techniques:

#### 1. Wave Distortion
```glsl
uv.x += sin(uv.y * wave_frequency + TIME * 2.0) * wave_intensity;
uv.y += cos(uv.x * wave_frequency * 0.5 + TIME * 1.5) * wave_intensity * 0.5;
```

**How it works:**
- Uses `sin()` and `cos()` functions to offset the UV coordinates
- `TIME` variable creates continuous animation
- `wave_frequency` (10.0) controls how many waves appear
- `wave_intensity` (0.1) controls how strong the distortion is
- Result: Flowing, wavy motion in the particles

#### 2. Color Gradient
```glsl
float gradient = uv.y;
vec4 color = mix(color_start, color_end, gradient);
```

**How it works:**
- Smoothly blends between two colors based on UV position
- `color_start`: Orange (1.0, 0.5, 0.0)
- `color_end`: Pink (1.0, 0.0, 0.5)
- Result: Vertical color gradient across each particle

#### 3. Pulsing Effect
```glsl
float pulse = sin(TIME * 3.0) * 0.2 + 0.8;
color.rgb *= pulse;
```

**How it works:**
- Brightness oscillates between 60% and 100%
- Creates a "breathing" animation
- Adds visual depth to the particles

The `ParticleController.cs` script also updates shader parameters in real-time to create additional animation effects.

---

## Part 2: Physics Properties Explanation

### Why These Physics Values Were Chosen

#### Mass = 1.0
```csharp
rigidBody.Mass = 1.0f;
```
**Why:** 
- Creates uniform, predictable behavior across all segments
- Each segment responds proportionally to forces
- Value of 1.0 provides good balance - not too heavy, not too light

#### GravityScale = 1.0
```csharp
rigidBody.GravityScale = 1.0f;
```
**Why:** 
- Standard gravity creates natural hanging motion
- Chain behaves like a real-world chain would
- Segments swing realistically when disturbed

#### Softness = 0.1
```csharp
joint.Softness = 0.1f;
```
**Why:** 
- Low softness creates stiff connections between segments
- Prevents excessive stretching or elastic bouncing
- Maintains chain integrity during motion
- 0.0 would be completely rigid (unrealistic), higher values would be too elastic

#### Bias = 0.3
```csharp
joint.Bias = 0.3f;
```
**Why:** 
- Controls how fast the joint corrects errors
- 0.3 balances stability and responsiveness
- Lower values cause drift, higher values cause jittering
- Prevents segments from separating while maintaining smooth motion

#### Collision Layers
```csharp
rigidBody.CollisionLayer = 2;  // Chain is on layer 2
rigidBody.CollisionMask = 3;   // Detect player (1) and other segments (2)
```
**Why:**
- Allows chain to detect and interact with the player
- Enables collisions between chain segments
- Without this, chain would pass through everything

#### Dampening
```csharp
rigidBody.LinearDamp = 0.5f;   // Air resistance
rigidBody.AngularDamp = 0.5f;  // Rotation dampening
```
**Why:**
- Simulates air resistance
- Prevents endless swinging
- Creates realistic motion that gradually comes to rest

#### Static Anchor
```csharp
if (i == 0)
{
    segment = new StaticBody2D();  // First segment doesn't move
}
```
**Why:**
- Anchors the chain to a fixed point
- Prevents entire chain from falling
- Creates realistic hanging behavior like a chain attached to a ceiling

---

## Part 3: Raycast Detection Explanation

### How the Raycast System Works

#### Setup
```csharp
_rayCast = new RayCast2D();
_rayCast.Enabled = true;
_rayCast.TargetPosition = new Vector2(LaserLength, 0);  // 500 pixels horizontal
_rayCast.CollisionMask = 1;  // Detect objects on layer 1 (player)
```

**How it works:**
- Creates an invisible ray starting from the LaserSystem position
- Ray extends 500 pixels horizontally to the right
- Only detects objects on collision layer 1 (the player)

#### Continuous Detection
```csharp
public override void _PhysicsProcess(double delta)
{
    _rayCast.ForceRaycastUpdate();  // Update every frame
    bool isColliding = _rayCast.IsColliding();
}
```

**How it works:**
- Runs in `_PhysicsProcess` for consistent, reliable detection
- `ForceRaycastUpdate()` ensures immediate detection (no lag)
- Checks every physics frame if something is blocking the ray

#### Player Detection
```csharp
var collider = _rayCast.GetCollider();
if (collider is Node node && IsPlayerOrChild(node))
{
    TriggerAlarm();
}
```

**How it works:**
- Gets the object the ray hit
- Checks if it's the player or a child of the player
- Uses `IsPlayerOrChild()` method to walk up the node tree
- This catches collisions with player's CollisionShape2D or other child nodes

#### Visual Feedback
```csharp
private void UpdateLaserBeam(Vector2 endPoint)
{
    _laserBeam.SetPointPosition(0, Vector2.Zero);
    _laserBeam.SetPointPosition(1, endPoint);
}
```

**How it works:**
- Line2D draws the visible laser beam
- Shows full 500px length when nothing is blocking
- Stops at collision point when hitting something
- Updates every frame for smooth visualization

#### Alarm System
```csharp
private void TriggerAlarm()
{
    _laserBeam.DefaultColor = LaserColorAlert;  // Turn red
    _alarmTimer.Start();
    GD.Print("ALARM! Player detected!");
}
```

**How it works:**
- Changes laser color from green to red
- Starts timer for pulsing alarm effect
- Adds flashing visual overlay
- Prints console message
- Automatically resets when player leaves the beam

---

## Project Structure

```
project/
├── scripts/
│   ├── ParticleController.cs      # Particle system with shader
│   ├── PhysicsChain.cs            # Chain physics with joints
│   ├── LaserDetector.cs           # Raycast detection system
│   ├── Player.cs                  # Player movement
│   └── custom_particle.gdshader   # Custom shader file
├── main.tscn                      # Main scene
└── project.godot                  # Project configuration
```

---

## Testing the Features

Run the game (F5) and you should see:

1. **Particles (upper-left, position 200, 100):**
   - Colored particles falling with wave effects
   - Orange to pink color gradient
   - Pulsing animation

2. **Chain (upper-middle, position 400, 50):**
   - 5 hanging segments with alternating colors
   - Swings when player walks into it
   - Gradually comes to rest

3. **Laser (left side, position 100, 300):**
   - Green horizontal line
   - Turns red when player crosses
   - Console prints "ALARM! Player detected!"
   - Returns to green when player leaves

---

## Common Issues

**Particles not visible:**
- Check that `custom_particle.gdshader` exists in project root
- Ensure "Emitting" is checked ON in Inspector

**Chain doesn't swing:**
- Verify you're using the latest PhysicsChain.cs with collision layers
- Player must be on collision layer 1

**Laser doesn't detect:**
- Build the project first
- Set Player Path to `../Player` in LaserSystem Inspector

---

## Technologies Used

- Godot Engine 4.x
- C# / .NET 9.0
- GLSL (shader language)
- Godot Physics Engine
