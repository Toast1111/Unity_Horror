# Hiding State Implementation

This document describes the new Hiding state and HidingSpot system added to JimmyAI.

## Overview

JimmyAI now has a "Hiding" state similar to YandereScript where Jimmy will find a nearby hiding spot to peek at the player from cover. This creates more dynamic and unpredictable AI behavior, especially at medium distances.

## New Components

### 1. HidingSpot.cs (Assets/Scripts/HidingSpot.cs)

A new script that can be attached to empty GameObjects in the Unity Editor to mark locations where Jimmy can hide and peek at the player.

**Features:**
- Can be placed on any GameObject (typically empty objects at corners, doorways, or behind cover)
- Automatically registers/unregisters itself in a static registry
- Tracks distance to Jimmy for finding closest spots
- Has visual gizmos in the editor for easy placement
- Includes methods for finding closest suitable hiding spots
- Has a `isLeftSide` boolean for indicating peek direction (for future animation support)

**Public Properties:**
- `isLeftSide` (bool) - Whether this is a left or right side hiding spot
- `distanceToJimmy` (float, read-only) - Auto-updated distance to Jimmy

**Static Methods:**
- `GetClosest(Vector3 fromPosition, float maxDistance, bool mustBeInFront)` - Find closest hiding spot
- `GetAllHidingSpots()` - Get all registered hiding spots
- `HasLineOfSightTo(Vector3 targetPosition, LayerMask obstacleMask)` - Check line of sight

**Editor Visualization:**
- Cyan sphere shows hiding spot location
- Cyan arrow shows peek direction (forward)
- Larger visualization when selected

### 2. Enhanced JimmyAI (Assets/Scripts/JimmyAI.cs)

Added new "Hiding" state to the AI state machine.

**New State: AIState.Hiding**

When active, Jimmy:
1. Moves to the hiding spot at run speed
2. Stops at the spot and peeks at the player
3. Rotates to face the player while peeking
4. Continues tracking the player from cover
5. Leaves the hiding spot if:
   - Player gets too close (< 5 units) → Chase
   - Player gets too far (> 25 units) → Chase
   - Player hides → Searching
   - Been hiding too long (10 seconds) → Chase
   - Lost sight of player → Distracted

## Behavior Changes

### Chase State Enhancement

During the Chase state at medium-far range (5-20 units):
- If Jimmy can see the player and there's a hiding spot nearby (within 5 units of Jimmy)
- Jimmy will transition to the Hiding state instead of direct pursuit
- This creates more strategic and unpredictable chase patterns

### State Flow with Hiding

```
Chase (5-20 units, player visible, hiding spot nearby)
    ↓
Hiding (at hiding spot, peeking at player)
    ↓
    ├─ Player too close → Chase
    ├─ Player too far → Chase  
    ├─ Player hides → Searching
    ├─ Lost sight → Distracted
    └─ Bored (10s) → Chase
```

## Setup Instructions

### In Unity Editor:

1. **Create Hiding Spots:**
   - Create empty GameObjects where you want hiding spots
   - Position them at corners, doorways, or behind cover
   - Add the `HidingSpot` component to each GameObject
   - Rotate the GameObject so the forward (blue) arrow points toward where Jimmy should peek
   - Check `isLeftSide` if the hiding spot is on the left side (for future use)

2. **Test Placement:**
   - Select a hiding spot GameObject in the hierarchy
   - You'll see a cyan sphere and arrow in the Scene view
   - The arrow shows the peek direction
   - Place multiple hiding spots throughout your level for best results

3. **Recommended Placement:**
   - Corners where Jimmy can peek around
   - Doorways where Jimmy can watch from
   - Behind pillars or obstacles
   - Near patrol routes
   - Areas with good sightlines to common player paths

## Technical Details

### New Private Variables in JimmyAI:
- `currentHidingSpot` (HidingSpot) - Tracks which hiding spot Jimmy is using

### Modified Methods:
- `ChasePlayer()` - Now checks for hiding spots at medium range
- Added `HideAndPeek()` - New method implementing hiding behavior

### Performance:
- HidingSpot.Update() runs on each hiding spot to track distance
- Uses static registry for efficient lookups
- Minimal overhead added to chase behavior

## Differences from YandereScript

**Simplified Features:**
- No zone system (Classroom/Corridor types) - uses simple distance checks
- No animation-specific logic (spy/peek animations) - focuses on positioning
- No renderer visibility checks - assumes simple 3D environment
- No specific left/right peek animations - just rotation toward player

**Retained Core Behavior:**
- Finding closest hiding spot near player
- Moving to and stopping at hiding spot
- Watching player from cover
- Transitioning based on distance and visibility
- Boredom timer for leaving hiding spot

## Example Usage Scenario

1. Player is spotted by Jimmy
2. Jimmy enters Chase state
3. Player runs away, distance becomes 12 units
4. Jimmy sees a hiding spot 3 units away, near the player's path
5. Jimmy transitions to Hiding state
6. Jimmy runs to the hiding spot and stops
7. Jimmy rotates to face the player and watches
8. Player continues moving, Jimmy tracks rotation
9. After 8 seconds, Jimmy gets bored and returns to Chase
10. Jimmy resumes direct pursuit

## Testing Checklist

- [ ] Create at least 3-5 hiding spots in the level
- [ ] Position them at strategic locations (corners, doorways)
- [ ] Orient them to face open areas where player might be
- [ ] Test chase at medium range (5-20 units)
- [ ] Verify Jimmy moves to hiding spot when player is visible
- [ ] Verify Jimmy peeks/rotates toward player from hiding spot
- [ ] Verify Jimmy leaves hiding spot when player gets close
- [ ] Verify Jimmy leaves hiding spot when player hides
- [ ] Verify boredom timer (10s) causes Jimmy to resume chase
- [ ] Check that hiding spots don't interfere with normal patrol/chase at other ranges

## Compatibility

✅ Compatible with all existing Assets/Scripts components
✅ No breaking changes to existing functionality
✅ Works with PlayerController hiding system
✅ Works with NavMeshAgent pathfinding
✅ Maintains all existing public APIs

## Future Enhancements (Optional)

- Add animation triggers for peek left/right
- Add sound effects for peeking behavior
- Add more sophisticated spot selection based on player direction
- Add cooldown timer to prevent rapid hiding spot switching
- Add zone-based hiding spot filtering
- Add hiding spot preference based on player last known direction
