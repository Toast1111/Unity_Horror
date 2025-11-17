# Unity_Horror

A first-person horror game built in Unity where the player must collect keys while avoiding "Jimmy", an intelligent AI enemy.

## Game Overview

### Objective
Find and collect keys scattered around the map to unlock doors and escape. Each key unlocks a specific door, and you must collect all keys to win.

### Gameplay Mechanics

#### Player Controls
- **WASD** - Move
- **Mouse** - Look around
- **Left Shift** - Sprint (generates more noise)
- **Left Control** - Crouch (move quietly)
- **E** - Interact (open doors, pick up keys, hide in lockers)

#### The Enemy: Jimmy
Jimmy is an advanced AI enemy that hunts the player using:
- **Noise Detection** - Hears player movement (sprinting is louder)
- **Visual Detection** - Sees player within field of view
- **Door Tracking** - Investigates opened doors
- **Dynamic Patrol** - Uses hot zones to patrol areas with recent activity
- **Smart States** - Patrols, investigates, chases, and searches

#### Hiding System
- Find lockers throughout the map to hide from Jimmy
- While hiding, Jimmy cannot detect you
- Press E to enter and exit lockers

## Unity Setup

### Requirements
- Unity 2020.3 or later
- NavMesh Components

### Scene Setup

1. **Player Setup**
   - Create a GameObject with tag "Player"
   - Add CharacterController component
   - Add PlayerController script
   - Create a child Camera with CameraController script

2. **Jimmy Setup**
   - Create a GameObject
   - Add NavMeshAgent component
   - Add JimmyAI script
   - Add Collider with "Is Trigger" enabled
   - Set patrol points in the inspector

3. **Environment**
   - Create NavMesh for the level
   - Add Door GameObjects with Door script
   - Add Key GameObjects with Key script (set matching keyIDs)
   - Add Locker GameObjects with Locker script
   - Add GameManager GameObject with GameManager and NoiseManager scripts

### Key-Door Matching
Set the `keyID` on Key scripts to match the `requiredKeyID` on Door scripts:
- Key_1 opens Door with requiredKeyID = "Key_1"
- Key_2 opens Door with requiredKeyID = "Key_2"
- etc.

## Scripts Overview

- **PlayerController.cs** - Handles player movement, interaction, inventory, and hiding
- **CameraController.cs** - First-person camera control
- **JimmyAI.cs** - Advanced enemy AI with multiple states and detection systems
- **Key.cs** - Key pickup functionality
- **Door.cs** - Door locking/unlocking and opening mechanics
- **Locker.cs** - Hiding spot for player
- **GameManager.cs** - Game state management, win/lose conditions
- **NoiseManager.cs** - Singleton for propagating noise events to enemies

## Game States

### Patrol
Jimmy patrols between waypoints, favoring hot zones (areas with recent activity).

### Investigation
Jimmy investigates noises and opened doors.

### Chase
Jimmy actively pursues the player when spotted.

### Search
Jimmy searches the last known player position after losing sight.

## Tips for Level Design

1. Place keys behind locked doors to create a progression system
2. Add multiple lockers for hiding opportunities
3. Create patrol points that cover key areas
4. Balance open spaces (dangerous) with hiding spots (safe)
5. Use doors strategically - opening them alerts Jimmy