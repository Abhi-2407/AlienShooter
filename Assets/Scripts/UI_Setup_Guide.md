# UI Setup Guide for Button System

This guide explains how to set up the red and blue buttons for controlling enemies in the Alien Shooter game.

## Button System Overview

The button system allows players to:
- Click the **Red Button** to stop all red enemies from moving horizontally and make them come down
- Click the **Blue Button** to stop all blue enemies from moving horizontally and make them come down
- Spawn new enemies of the clicked type
- Buttons have cooldown periods to prevent spam clicking

## Step-by-Step Setup

### 1. Create Canvas and UI Elements

1. **Create Canvas**:
   - Right-click in Hierarchy → UI → Canvas
   - Name it "GameCanvas"

2. **Create Red Button**:
   - Right-click on Canvas → UI → Button
   - Name it "RedButton"
   - Position it on the left side of the screen
   - Set Text to "RED"
   - Change button color to red

3. **Create Blue Button**:
   - Right-click on Canvas → UI → Button
   - Name it "BlueButton"
   - Position it on the right side of the screen
   - Set Text to "BLUE"
   - Change button color to blue

### 2. Create ButtonManager GameObject

1. **Create Empty GameObject**:
   - Right-click in Hierarchy → Create Empty
   - Name it "ButtonManager"

2. **Add ButtonManager Script**:
   - Select ButtonManager GameObject
   - Add Component → Scripts → ButtonManager

3. **Configure ButtonManager**:
   - Drag RedButton from Hierarchy to "Red Button" field
   - Drag BlueButton from Hierarchy to "Blue Button" field
   - Drag EnemySpawner GameObject to "Enemy Spawner" field
   - Adjust "Button Cooldown" (default: 2 seconds)
   - Set "Active Button Color" (default: green)
   - Set "Inactive Button Color" (default: white)

### 3. Button Positioning

**Recommended Layout**:
```
[RED BUTTON]                    [BLUE BUTTON]
     ↓                              ↓
[Player Area]                 [Player Area]
     ↓                              ↓
[Enemy Spawn Area]           [Enemy Spawn Area]
```

**Position Settings**:
- **Red Button**: Anchor to bottom-left, position (50, 50)
- **Blue Button**: Anchor to bottom-right, position (-50, 50)

### 4. Button Styling (Optional)

**Red Button Styling**:
- Background Color: Red (255, 0, 0, 255)
- Text Color: White
- Font Size: 24
- Button Size: 120x60

**Blue Button Styling**:
- Background Color: Blue (0, 0, 255, 255)
- Text Color: White
- Font Size: 24
- Button Size: 120x60

### 5. Testing the System

1. **Play the Game**
2. **Wait for enemies to spawn**
3. **Click Red Button**:
   - All red enemies should stop moving horizontally
   - Red enemies should start moving straight down
   - A new red enemy should spawn
   - Button should show cooldown (grayed out)
4. **Click Blue Button**:
   - All blue enemies should stop moving horizontally
   - Blue enemies should start moving straight down
   - A new blue enemy should spawn
   - Button should show cooldown (grayed out)

## Button Behavior Details

### Red Button
- **Function**: Stops all red enemies and spawns new red enemy
- **Cooldown**: 2 seconds (configurable)
- **Visual Feedback**: Changes color during cooldown
- **Requirements**: At least one red enemy must exist to be clickable

### Blue Button
- **Function**: Stops all blue enemies and spawns new blue enemy
- **Cooldown**: 2 seconds (configurable)
- **Visual Feedback**: Changes color during cooldown
- **Requirements**: At least one blue enemy must exist to be clickable

## Troubleshooting

### Buttons Not Working
1. Check that ButtonManager script is attached
2. Verify button references are assigned
3. Ensure EnemySpawner reference is set
4. Check console for error messages

### Enemies Not Stopping
1. Verify enemy prefabs have RedEnemy or BlueEnemy scripts
2. Check that enemies are tagged as "Enemy"
3. Ensure Rigidbody2D is attached to enemies

### New Enemies Not Spawning
1. Check that enemy prefabs are assigned in EnemySpawner
2. Verify spawn points are created
3. Check console for spawn messages

### UI Not Visible
1. Ensure Canvas is set to Screen Space - Overlay
2. Check Canvas Scaler settings
3. Verify UI elements are children of Canvas

## Customization Options

### ButtonManager Settings
- **Button Cooldown**: Time between button clicks (default: 2s)
- **Active Button Color**: Color when button is clickable
- **Inactive Button Color**: Color during cooldown

### Enemy Behavior
- **Stop Speed**: How fast enemies move down when stopped
- **Spawn Position**: Where new enemies appear
- **Cooldown Logic**: When buttons become available

## Advanced Features

### Button States
- **Enabled**: Button is clickable and enemies exist
- **Disabled**: No enemies of that type exist
- **Cooldown**: Button was recently clicked

### Visual Feedback
- Color changes indicate button state
- Console messages show button clicks
- Enemy movement changes are immediate

This button system adds strategic depth to the game by allowing players to control enemy behavior and manage the battlefield!

