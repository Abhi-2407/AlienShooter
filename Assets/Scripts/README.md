# Alien Shooter Game Scripts

This folder contains all the C# scripts needed to create a 2D space shooter game similar to classic arcade games.

## Scripts Overview

### Core Game Scripts

1. **PlayerController.cs** - Controls player movement, shooting, and health
2. **EnemyController.cs** - Base class for enemy AI, movement, and shooting
3. **RedEnemy.cs** - Red enemy type with aggressive behavior and horizontal movement
4. **BlueEnemy.cs** - Blue enemy type with defensive behavior and horizontal movement
5. **Bullet.cs** - Manages projectile behavior and collision detection
6. **GameManager.cs** - Main game state manager, scoring, and wave progression
7. **EnemySpawner.cs** - Spawns red and blue enemies on both sides
8. **UIManager.cs** - Manages all UI elements and game menus
9. **ButtonManager.cs** - Controls red and blue buttons for enemy control
10. **SpaceshipController.cs** - Controls red and blue spaceships for enemy collision
11. **SpaceshipSpawner.cs** - Manages spaceship spawning and lifecycle

### Utility Scripts

12. **Boundary.cs** - Handles screen boundaries and cleanup
13. **PowerUp.cs** - Manages collectible power-ups
14. **AudioManager.cs** - Handles all audio and music

## Setup Instructions

### 1. Create GameObjects

#### Player Setup
- Create an empty GameObject named "Player"
- Add the `PlayerController` script
- Add a SpriteRenderer for the player sprite
- Add a Collider2D (CircleCollider2D recommended)
- Add a Rigidbody2D
- Tag the GameObject as "Player"
- Create a child GameObject for the fire point and assign it to the `firePoint` field

#### Enemy Setup
- Create **Red Enemy** prefab with:
  - SpriteRenderer (red colored sprite)
  - Collider2D
  - Rigidbody2D
  - `RedEnemy` script
  - Tag as "Enemy"
- Create **Blue Enemy** prefab with:
  - SpriteRenderer (blue colored sprite)
  - Collider2D
  - Rigidbody2D
  - `BlueEnemy` script
  - Tag as "Enemy"

#### Bullet Setup
- Create bullet prefabs with:
  - SpriteRenderer
  - Collider2D (set as Trigger)
  - Rigidbody2D
  - `Bullet` script
  - Tag as "Bullet"

### 2. UI Setup

#### Canvas Setup
- Create a Canvas with UI elements:
  - Score text (TextMeshProUGUI)
  - Lives text (TextMeshProUGUI)
  - Wave text (TextMeshProUGUI)
  - Time text (TextMeshProUGUI)
  - Health bar (Slider)
  - Game Over panel
  - Pause panel
  - Wave Complete panel

#### UIManager Setup
- Create an empty GameObject named "UIManager"
- Add the `UIManager` script
- Assign all UI references in the inspector

### 3. Game Manager Setup

- Create an empty GameObject named "GameManager"
- Add the `GameManager` script
- Assign UI Manager and Enemy Spawner references

### 4. Enemy Spawner Setup

- Create an empty GameObject named "EnemySpawner"
- Add the `EnemySpawner` script
- Assign the Red Enemy prefab to `redEnemyPrefab`
- Assign the Blue Enemy prefab to `blueEnemyPrefab`
- The script will automatically create 4 spawn points (left red, left blue, right red, right blue)

### 5. Boundaries Setup

- Create invisible boundary objects around the screen edges
- Add the `Boundary` script to each
- Set appropriate boundary types (top, bottom, left, right)

### 6. Audio Setup

- Create an empty GameObject named "AudioManager"
- Add the `AudioManager` script
- Assign audio clips for music and sound effects

### 7. Button System Setup

- Create a Canvas with UI elements
- Add two Button GameObjects:
  - **Red Button**: For controlling red enemies
  - **Blue Button**: For controlling blue enemies
- Create an empty GameObject named "ButtonManager"
- Add the `ButtonManager` script
- Assign the red and blue buttons to the script
- Assign the EnemySpawner reference
- Customize button colors and cooldown settings

### 8. Spaceship System Setup

- Create **Red Spaceship** prefab with:
  - SpriteRenderer (red spaceship sprite)
  - Collider2D (set as Trigger)
  - Rigidbody2D
  - `SpaceshipController` script
  - Tag as "Spaceship"
- Create **Blue Spaceship** prefab with:
  - SpriteRenderer (blue spaceship sprite)
  - Collider2D (set as Trigger)
  - Rigidbody2D
  - `SpaceshipController` script
  - Tag as "Spaceship"
- Create empty GameObject named "SpaceshipSpawner"
- Add the `SpaceshipSpawner` script
- Assign spaceship prefabs and configure spawn settings
- Update enemy tags: Red enemies → "RedEnemy", Blue enemies → "BlueEnemy"

## Input System

The game uses Unity's Input System. Make sure to:
1. Install the Input System package
2. Create Input Actions asset
3. Set up WASD/Arrow keys for movement
4. Set up Space/Left Mouse for shooting
5. Set up Escape for pause

## Tags Required

Create these tags in Unity:
- Player
- Enemy
- RedEnemy
- BlueEnemy
- Bullet
- Spaceship
- Boundary

## Layers Recommended

Create these layers:
- Player
- Enemy
- Bullet
- UI

## Physics Settings

- Set up Physics2D collision matrix
- Configure collision detection between appropriate layers
- Set up triggers for bullets and power-ups

## Features Included

- Player movement with screen boundaries
- Shooting system with fire rate control
- **Two enemy types**: Red (aggressive) and Blue (defensive)
- **Horizontal enemy movement** within defined ranges
- **Side-by-side spawning**: 1 red + 1 blue on each side
- **Button Control System**: Click buttons to stop enemy horizontal movement and spawn new enemies
- **Spaceship Collision System**: Red and blue spaceships collide with matching enemies for bonus points
- **Color-coded Collision**: Only same-color spaceships and enemies can collide
- **Automatic Respawn**: Destroyed spaceships automatically respawn after 1-second delay
- Wave-based enemy spawning
- Scoring system with high score persistence
- **Spaceship Scoring**: Separate tracking for red and blue spaceship scores
- Health system with visual feedback
- Power-up system
- Audio management
- Pause functionality
- Game over and restart
- UI management

## Customization

All scripts include public fields that can be adjusted in the Unity Inspector:
- Movement speeds
- Fire rates
- Health values
- Spawn rates
- Difficulty scaling
- UI references
- Audio clips

## Notes

- The game is designed for 2D space shooter gameplay
- All scripts are modular and can be easily extended
- The enemy spawner automatically creates spawn points
- The UI manager handles all UI updates automatically
- Audio manager provides centralized audio control
- Game manager uses singleton pattern for easy access

## Troubleshooting

1. Make sure all required components are attached
2. Check that all tags are properly assigned
3. Verify that colliders are set as triggers where needed
4. Ensure UI references are properly assigned
5. Check that audio clips are assigned to the AudioManager
