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
   git clone [your-repo-url]
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
