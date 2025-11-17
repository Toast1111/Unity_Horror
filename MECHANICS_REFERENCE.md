# Game Mechanics Quick Reference

## Core Systems Implemented

### 1. Player System
**File:** `PlayerController.cs` (209 lines)

**Features:**
- Movement (walk, sprint, crouch)
- Key inventory management
- Door unlocking logic
- Locker hiding system
- Noise generation based on movement

**Controls:**
- WASD: Move
- Left Shift: Sprint (louder noise)
- Left Control: Crouch (quieter noise)
- E: Interact
- Mouse: Look around

### 2. Jimmy AI System
**File:** `JimmyAI.cs` (364 lines)

**AI States:**
1. **Patrolling** - Moves between waypoints using dynamic hot zones
2. **Investigating** - Checks out noises and opened doors
3. **Chasing** - Actively pursues visible player
4. **Searching** - Searches last known player position

**Detection Systems:**
- **Visual Detection:** Field of view, range, line of sight checks
- **Noise Detection:** Hears player movement within radius
- **Door Detection:** Alerted when doors are opened
- **Hot Zones:** Dynamically adjusts patrol based on activity

**Key Parameters:**
- Detection Range: 15 units
- Field of View: 120 degrees
- Patrol Speed: 3 units/sec
- Chase Speed: 6 units/sec

### 3. Key-Door System
**Files:** `Key.cs` (20 lines), `Door.cs` (85 lines)

**How it Works:**
- Each key has a unique `keyID`
- Each door has a `requiredKeyID`
- Keys must match door IDs to unlock
- Doors notify Jimmy when opened

**Example Configuration:**
```
Key_1 → Door_1
Key_2 → Door_2
Key_3 → Door_3
```

### 4. Hiding System
**File:** `Locker.cs` (29 lines)

**Mechanics:**
- Player can hide in lockers
- While hiding, player is invisible to Jimmy
- Can only exit locker manually
- Multiple lockers can exist in level

### 5. Noise System
**File:** `NoiseManager.cs` (33 lines)

**Noise Sources:**
- Walking: 5 unit radius
- Sprinting: 15 unit radius
- Crouching: 2 unit radius
- Opening doors: 10 unit radius

### 6. Game Management
**File:** `GameManager.cs` (103 lines)

**Functions:**
- Tracks collected keys
- Win condition (collect all keys)
- Lose condition (caught by Jimmy)
- Game restart functionality
- Singleton pattern implementation

### 7. Camera System
**File:** `CameraController.cs` (33 lines)

**Features:**
- First-person view
- Mouse look with sensitivity control
- Rotation clamping (prevents over-rotation)
- Locked cursor during gameplay

## Technical Details

### Required Unity Components

**Player:**
- CharacterController
- PlayerController script
- Camera (child) with CameraController script

**Jimmy:**
- NavMeshAgent
- CapsuleCollider (trigger)
- JimmyAI script

**Doors:**
- Collider
- Door script

**Keys:**
- Collider
- Key script

**Lockers:**
- Collider
- Locker script

**GameManager:**
- GameManager script
- NoiseManager script

### Unity Tags Required
- "Player" - for player GameObject

### NavMesh Requirements
- Baked NavMesh for level geometry
- Agent radius: 0.5
- Agent height: 2
- Max slope: 45 degrees

## Gameplay Flow

1. **Game Start**
   - Player spawns at start position
   - Jimmy begins patrolling waypoints
   - Keys and doors are positioned in level

2. **Exploration Phase**
   - Player explores to find keys
   - Must avoid Jimmy's patrol routes
   - Can hide in lockers when detected

3. **Collection Phase**
   - Collect keys to unlock doors
   - Each door requires specific key
   - Opening doors alerts Jimmy

4. **Escape Phase**
   - Collect all required keys
   - Navigate to exit
   - Avoid Jimmy to win

5. **Game End**
   - **Win:** Collect all keys
   - **Lose:** Caught by Jimmy

## AI Behavior Patterns

### Patrol Behavior
- Follows waypoint path
- Prioritizes hot zones (recent activity)
- Waits briefly at each waypoint
- Always alert for player

### Investigation Behavior
- Moves to noise/door location
- Searches area for set duration
- Returns to patrol if nothing found
- Increases hot zone priority

### Chase Behavior
- Maximum speed pursuit
- Direct line to player
- Updates path continuously
- Maintains player memory

### Search Behavior
- Goes to last known position
- Searches surrounding area
- Limited search duration
- Returns to patrol if unsuccessful

## Balance Considerations

### Player Advantages
- Can hide in lockers
- Crouching reduces noise
- Can see before being seen
- Multiple escape routes

### Jimmy Advantages
- Faster chase speed
- Wide detection range
- Multiple detection methods
- Persistent hunting
- Dynamic patrol adaptation

### Risk vs Reward
- Sprinting is faster but louder
- Opening doors grants access but alerts enemy
- Hiding is safe but stationary
- Collecting keys requires exploration

## Performance Notes

- Total code: 966 lines across 9 scripts
- Efficient singleton patterns for managers
- NavMesh for optimized pathfinding
- Dynamic hot zones limited to 10 max
- Minimal memory allocation during gameplay

## Extensibility

The code is designed to be easily extended:
- Add new AI states by extending enum
- Add new detection types in UpdateState()
- Add new interactable types in HandleInteraction()
- Add multiple Jimmy instances (fully supported)
- Create different key-door combinations
- Adjust all parameters via Inspector

## Testing Checklist

Core Functionality:
- [ ] Player movement works
- [ ] Camera rotates properly
- [ ] Keys can be collected
- [ ] Doors unlock with correct keys
- [ ] Doors open after unlocking
- [ ] Lockers hide player successfully
- [ ] Jimmy patrols waypoints
- [ ] Jimmy hears noise
- [ ] Jimmy investigates noise
- [ ] Jimmy chases visible player
- [ ] Jimmy loses player in locker
- [ ] Door opening alerts Jimmy
- [ ] Game ends when caught
- [ ] Win condition triggers correctly

Advanced Testing:
- [ ] Hot zones update properly
- [ ] Multiple lockers work
- [ ] Key-door IDs match correctly
- [ ] NavMesh pathfinding works
- [ ] Line of sight detection accurate
- [ ] Field of view calculation correct
- [ ] Memory system functions
- [ ] State transitions smooth
