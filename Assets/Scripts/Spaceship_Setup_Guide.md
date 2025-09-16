# Spaceship System Setup Guide

This guide explains how to set up the spaceship collision system where red and blue spaceships can destroy their respective enemies and earn points.

## System Overview

The spaceship system includes:
- **Red Spaceship**: Collides with red enemies to destroy them and earn points
- **Blue Spaceship**: Collides with blue enemies to destroy them and earn points
- **Automatic Spawning**: Spaceships spawn automatically at intervals
- **Scoring System**: Separate tracking for red and blue spaceship scores
- **Collision Detection**: Only same-color spaceships and enemies can collide

## Required Tags

Create these tags in Unity:
- `RedEnemy` - For red enemies
- `BlueEnemy` - For blue enemies  
- `Spaceship` - For all spaceships

## Step-by-Step Setup

### 1. Create Spaceship Prefabs

#### Red Spaceship Prefab:
1. **Create GameObject**:
   - Right-click in Hierarchy → Create Empty
   - Name it "RedSpaceship"

2. **Add Components**:
   - SpriteRenderer (add red spaceship sprite)
   - Collider2D (set as Trigger)
   - Rigidbody2D
   - SpaceshipController script

3. **Configure SpaceshipController**:
   - Spaceship Type: Red
   - Move Speed: 2
   - Score Value: 50
   - Horizontal Range: 4
   - Stop Duration: 2
   - Move Duration: 3
   - Direction Change Chance: 0.3
   - Stop Duration Variation: 1
   - Move Duration Variation: 1

4. **Set Tag**: "Spaceship"

5. **Create Prefab**: Drag to Project window

#### Blue Spaceship Prefab:
1. **Create GameObject**:
   - Right-click in Hierarchy → Create Empty
   - Name it "BlueSpaceship"

2. **Add Components**:
   - SpriteRenderer (add blue spaceship sprite)
   - Collider2D (set as Trigger)
   - Rigidbody2D
   - SpaceshipController script

3. **Configure SpaceshipController**:
   - Spaceship Type: Blue
   - Move Speed: 2
   - Score Value: 50
   - Horizontal Range: 4
   - Stop Duration: 2
   - Move Duration: 3
   - Direction Change Chance: 0.3
   - Stop Duration Variation: 1
   - Move Duration Variation: 1

4. **Set Tag**: "Spaceship"

5. **Create Prefab**: Drag to Project window

### 2. Create SpaceshipSpawner

1. **Create Empty GameObject**:
   - Right-click in Hierarchy → Create Empty
   - Name it "SpaceshipSpawner"

2. **Add SpaceshipSpawner Script**:
   - Add Component → Scripts → SpaceshipSpawner

3. **Configure Settings**:
   - Red Spaceship Prefab: Drag red spaceship prefab
   - Blue Spaceship Prefab: Drag blue spaceship prefab
   - Spawn Delay: 5 (seconds before first spawn)
   - Spawn Interval: 10 (seconds between spawns)
   - Max Spaceships: 2
   - Auto Spawn: true
   - Spawn On Start: true
   - Auto Respawn: true
   - Respawn Delay: 1 (seconds before respawning)

### 3. Update Enemy Tags

#### Red Enemy Setup:
1. Select your red enemy prefab
2. Change tag from "Enemy" to "RedEnemy"
3. Apply changes to prefab

#### Blue Enemy Setup:
1. Select your blue enemy prefab
2. Change tag from "Enemy" to "BlueEnemy"
3. Apply changes to prefab

### 4. Configure Collision Matrix

1. **Open Physics2D Settings**:
   - Edit → Project Settings → Physics2D

2. **Set Up Collision Matrix**:
   - RedEnemy + Spaceship: ✅ (collision enabled)
   - BlueEnemy + Spaceship: ✅ (collision enabled)
   - RedEnemy + BlueEnemy: ❌ (no collision)
   - Other combinations as needed

### 5. Test the System

1. **Play the Game**
2. **Wait for spaceships to spawn** (5 seconds)
3. **Wait for enemies to spawn**
4. **Watch for collisions**:
   - Red spaceship should only collide with red enemies
   - Blue spaceship should only collide with blue enemies
   - Both spaceship and enemy should be destroyed on collision
   - Score should increase

## Spaceship Behavior

### Movement Pattern:
- **Horizontal Movement**: Moves left and right within defined range
- **Stop and Go**: Alternates between moving and stopping for configurable durations
- **Random Direction Changes**: Can randomly change direction during movement
- **Screen Exit**: Destroyed when reaching bottom of screen

### Movement Settings Explained:
- **Move Speed**: How fast the spaceship moves horizontally (2 = slow, 5 = fast)
- **Stop Duration**: Base duration for how long the spaceship stops (in seconds)
- **Move Duration**: Base duration for how long the spaceship moves (in seconds)
- **Stop Duration Variation**: Random variation added to stop duration (±variation seconds)
- **Move Duration Variation**: Random variation added to move duration (±variation seconds)
- **Direction Change Chance**: Probability of changing direction (0.0 = never, 1.0 = always)
- **Horizontal Range**: Maximum distance from starting position before reversing

### Random Timing System:
- Each spaceship gets **unique random durations** for movement and stopping
- Durations are recalculated after each stop-move cycle
- **Example**: If Stop Duration = 2 and Stop Duration Variation = 1, actual stops will be 1-3 seconds
- **Example**: If Move Duration = 3 and Move Duration Variation = 1, actual moves will be 2-4 seconds

### Collision Logic:
- **Red Spaceship** + **Red Enemy** = Both destroyed + Score added + Red spaceship respawns after 1 second
- **Blue Spaceship** + **Blue Enemy** = Both destroyed + Score added + Blue spaceship respawns after 1 second
- **Red Spaceship** + **Blue Enemy** = No collision
- **Blue Spaceship** + **Red Enemy** = No collision

### Scoring System:
- **Red Spaceship Score**: Tracked separately
- **Blue Spaceship Score**: Tracked separately
- **Total Spaceship Score**: Combined score
- **Main Score**: Includes spaceship scores

## Customization Options

### SpaceshipController Settings:
- **Move Speed**: How fast spaceships move horizontally
- **Score Value**: Points earned per collision
- **Horizontal Range**: How far spaceships can move left/right
- **Vertical Speed**: How fast spaceships move down

### SpaceshipSpawner Settings:
- **Spawn Delay**: Time before first spaceship spawns
- **Spawn Interval**: Time between spaceship spawns
- **Max Spaceships**: Maximum number of spaceships (usually 2)
- **Auto Spawn**: Whether to spawn automatically
- **Auto Respawn**: Whether to automatically respawn destroyed spaceships
- **Respawn Delay**: Time delay before respawning (default: 1 second)

## Troubleshooting

### Spaceships Not Spawning:
1. Check that prefabs are assigned
2. Verify spawn points are created
3. Check console for error messages
4. Ensure auto spawn is enabled

### No Collisions:
1. Verify enemy tags are correct (RedEnemy, BlueEnemy)
2. Check spaceship tags are set to "Spaceship"
3. Ensure colliders are set as triggers
4. Check Physics2D collision matrix

### Wrong Collision Behavior:
1. Verify spaceship types are set correctly
2. Check collision detection logic
3. Ensure proper tag assignments

### Scoring Issues:
1. Check GameManager references
2. Verify score values are set
3. Check console for score messages

## Advanced Features

### Visual Effects:
- **Explosion Effects**: Add particle systems for collisions
- **Sound Effects**: Add audio clips for collisions
- **Trail Effects**: Add particle trails for spaceships

### Gameplay Balance:
- **Speed Adjustment**: Balance spaceship vs enemy speeds
- **Spawn Timing**: Adjust spawn intervals for difficulty
- **Score Values**: Balance points for different actions
- **Respawn Timing**: Adjust respawn delay for game flow
- **Auto Respawn**: Enable/disable automatic respawning

This spaceship system adds strategic depth by allowing players to use spaceships as a tool to clear specific enemy types and earn bonus points!
