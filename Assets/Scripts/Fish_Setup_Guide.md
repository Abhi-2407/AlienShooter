# Fish System Setup Guide

This guide explains how to set up the fish spawning and swimming system in your Unity project.

## Components Created

### 1. FishController.cs
- Controls individual fish behavior and movement
- Handles swimming patterns within defined boundaries
- Supports both random direction movement and target-based movement
- Automatically keeps fish within swimming area bounds

### 2. FishSpawner.cs
- Manages spawning of multiple fish within a defined area
- Supports continuous spawning and respawning
- Configures fish behavior settings
- Provides visual debugging with Gizmos

## Setup Instructions

### Step 1: Create Fish Prefabs
1. Create a GameObject for your fish
2. Add a SpriteRenderer component
3. Assign a fish sprite/texture
4. Add a Collider2D (optional, for collision detection)
5. Add the FishController script
6. Save as a prefab in your Assets folder

### Step 2: Set Up Fish Spawner
1. Create an empty GameObject in your scene
2. Name it "FishSpawner"
3. Add the FishSpawner script
4. Assign your fish prefabs to the "Fish Prefabs" array
5. Configure the spawn and swim areas

### Step 3: Configure Settings

#### Fish Spawner Settings:
- **Fish Prefabs**: Array of fish prefabs to spawn
- **Fish Count**: Number of fish to spawn initially
- **Spawn Area**: Area where fish will be spawned
- **Swim Area**: Area where fish can swim around
- **Use Camera Bounds**: Automatically use camera view bounds

#### Fish Controller Settings (per fish):
- **Swim Speed**: How fast the fish moves
- **Rotation Speed**: How fast the fish rotates
- **Change Direction Interval**: How often fish changes direction
- **Swimming Area**: Boundaries for fish movement

## Features

### Fish Behavior:
- **Random Movement**: Fish swim in random directions
- **Target-Based Movement**: Fish move towards random targets
- **Boundary Respect**: Fish stay within defined swimming area
- **Speed Variation**: Fish can have different swimming speeds
- **Direction Changes**: Fish periodically change direction

### Spawner Features:
- **Multiple Fish Types**: Support for different fish prefabs
- **Continuous Spawning**: Option to continuously spawn new fish
- **Area Control**: Define both spawn and swim areas
- **Visual Debugging**: Gizmos show spawn and swim areas
- **Runtime Control**: Methods to control spawning at runtime

## Usage Examples

### Basic Setup:
```csharp
// Get reference to fish spawner
FishSpawner spawner = FindObjectOfType<FishSpawner>();

// Spawn 5 fish
spawner.SetFishCount(5);
spawner.StartSpawning();

// Enable continuous spawning
spawner.EnableContinuousSpawning(true);
```

### Custom Areas:
```csharp
// Set custom spawn area
spawner.SetSpawnArea(new Vector2(0, 0), new Vector2(10, 6));

// Set custom swim area
spawner.SetSwimArea(new Vector2(0, 0), new Vector2(8, 4));
```

### Individual Fish Control:
```csharp
// Get fish controller
FishController fish = fishObject.GetComponent<FishController>();

// Set custom swim speed
fish.SetSwimSpeed(2.5f);

// Set custom swim area
fish.SetSwimArea(new Vector2(0, 0), new Vector2(6, 4));
```

## Tips

1. **Performance**: Limit the number of fish for better performance
2. **Variety**: Use different fish prefabs for visual variety
3. **Areas**: Make sure swim area is smaller than spawn area
4. **Camera Bounds**: Use camera bounds for automatic area setup
5. **Debugging**: Use the Gizmos to visualize spawn and swim areas

## Troubleshooting

- **Fish not spawning**: Check if fish prefabs are assigned
- **Fish not moving**: Ensure FishController is attached to fish prefabs
- **Fish leaving area**: Check swim area boundaries are set correctly
- **Performance issues**: Reduce fish count or increase spawn delays



