# Fish Catching System Setup Guide

This guide explains how to set up the fish catching system where fish get caught by light colliders and are pulled towards spaceships.

## System Overview

The fish catching system includes:
- **Light Detection**: Fish detect when they enter `BlueLight` or `RedLight` colliders
- **Catching Behavior**: Fish stop swimming and are pulled towards the spaceship
- **Scoring System**: Points are awarded when fish are caught
- **Visual Feedback**: Fish rotate towards the spaceship while being caught

## Required Setup

### 1. Fish Prefab Setup

Ensure your fish prefab has:
- **Collider2D** (set as Trigger) - for detecting light colliders
- **FishController** script - includes the catching system
- **SpriteRenderer** - for visual representation

### 2. Spaceship Light Colliders

Each spaceship needs a light collider child object:

#### For Red Spaceship:
1. **Create Child GameObject**:
   - Right-click on RedSpaceship → Create Empty
   - Name it "RedLight"

2. **Add Components**:
   - Collider2D (set as Trigger)
   - Set size to desired light range (e.g., 3x3 units)

3. **Set Tag**: "RedLight"

#### For Blue Spaceship:
1. **Create Child GameObject**:
   - Right-click on BlueSpaceship → Create Empty
   - Name it "BlueLight"

2. **Add Components**:
   - Collider2D (set as Trigger)
   - Set size to desired light range (e.g., 3x3 units)

3. **Set Tag**: "BlueLight"

### 3. Fish Controller Settings

Configure the `FishController` script on your fish prefab:

#### Light Catching System Settings:
- **Catch Speed**: 3 (how fast fish moves towards spaceship)
- **Catch Distance**: 0.5 (how close fish needs to be to be "caught")
- **Catch Score Value**: 5 (points awarded for catching fish)

#### Collision Detection:
- Ensure fish collider is set as **Trigger**
- Fish will automatically detect `BlueLight` and `RedLight` tags

### 4. Fish Spawner Auto-Respawn Settings

Configure the `FishSpawner` script for automatic fish replacement:

#### Auto Respawn Settings:
- **Auto Respawn On Destroy**: true (automatically spawn new fish when any are destroyed)
- **Respawn Delay On Destroy**: 2 (seconds to wait before respawning)
- **Target Fish Count**: 5 (desired number of fish to maintain)
- **Max Fish Count**: 10 (maximum fish allowed at once)

## How It Works

### 1. Detection Phase:
- When fish enters a light collider (`BlueLight` or `RedLight`), `OnTriggerEnter2D` is called
- Fish stops normal swimming behavior
- Fish starts being pulled towards the spaceship

### 2. Catching Phase:
- Fish moves towards the spaceship at `catchSpeed`
- Fish rotates to face the spaceship
- Normal swimming behavior is disabled

### 3. Completion Phase:
- When fish gets within `catchDistance` of spaceship, it's considered "caught"
- Points are awarded based on `catchScoreValue`
- Fish is destroyed
- Score is added to both general score and spaceship-specific score

### 4. Release Phase:
- If fish exits the light collider before being caught, it resumes normal swimming
- This allows fish to escape if spaceship moves away

### 5. Auto-Respawn Phase:
- When any fish is destroyed (caught or otherwise), the system detects the count change
- After a configurable delay, new fish are automatically spawned to maintain the target count
- Fish are respawned with a small delay between each for visual effect
- The system ensures the fish population stays at the desired level

## Testing the System

### 1. Basic Test:
1. **Spawn fish** using FishSpawner
2. **Spawn spaceships** using SpaceshipSpawner
3. **Move spaceships** near fish
4. **Watch fish behavior**:
   - Fish should stop swimming when entering light
   - Fish should move towards spaceship
   - Fish should be destroyed when close enough
   - Score should increase

### 2. Advanced Test:
1. **Test with both spaceship types** (red and blue)
2. **Test fish escaping** by moving spaceship away
3. **Test multiple fish** being caught simultaneously
4. **Verify scoring** works correctly

## Troubleshooting

### Fish Not Being Caught:
- Check that fish has Collider2D set as Trigger
- Verify light colliders have correct tags (`BlueLight` or `RedLight`)
- Ensure light colliders are child objects of spaceships
- Check that fish prefab has FishController script

### Fish Not Moving Towards Spaceship:
- Verify `catchSpeed` is greater than 0
- Check that spaceship has a Transform component
- Ensure fish is actually entering the trigger (check debug logs)

### No Score Being Added:
- Verify GameManager exists in scene
- Check that `catchScoreValue` is greater than 0
- Ensure spaceship has SpaceshipController script for spaceship-specific scoring

## Customization Options

### Adjusting Catch Behavior:
- **Catch Speed**: Higher values make fish move faster towards spaceship
- **Catch Distance**: Smaller values require fish to get closer before being caught
- **Catch Score Value**: Points awarded for each fish caught

### Visual Effects:
- Add particle effects when fish is caught
- Add sound effects for catching and releasing
- Modify fish appearance when being caught (e.g., change color, add glow)

### Gameplay Balance:
- Adjust light collider sizes for different difficulty levels
- Modify catch speed based on fish type or spaceship type
- Add cooldown periods between catches

## Auto-Respawn System

### New Public Methods:
- `SetAutoRespawnOnDestroy(bool)`: Enable/disable auto-respawn
- `SetRespawnDelayOnDestroy(float)`: Set delay before respawning
- `SetTargetFishCount(int)`: Set desired number of fish to maintain
- `ForceRespawnToTarget()`: Immediately respawn to target count
- `RespawnFishImmediately(int)`: Spawn specific number of fish

### Auto-Respawn Behavior:
- **Detection**: System monitors fish count changes every frame
- **Delay**: Configurable delay before respawning (prevents spam)
- **Target Maintenance**: Keeps fish count at target level
- **Visual Effect**: Small delay between individual fish respawns
- **Debug Logging**: Shows when fish are destroyed and respawned

## Integration with Existing Systems

The fish catching system integrates with:
- **GameManager**: For scoring and game state
- **SpaceshipController**: For spaceship-specific scoring
- **FishSpawner**: For spawning fish with catching capabilities and auto-respawn
- **UI System**: For displaying updated scores

## Performance Considerations

- Fish catching uses simple distance calculations
- Trigger detection is efficient for small numbers of fish
- Consider object pooling for frequent fish spawning/catching
- Debug logs can be disabled in production builds


