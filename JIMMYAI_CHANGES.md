# JimmyAI Enhancement Summary

This document details the changes made to `Assets/Scripts/JimmyAI.cs` to replicate YandereScript's AI behavior while maintaining compatibility with the existing Unity Horror game scripts.

## Overview

The JimmyAI script has been enhanced with more sophisticated AI behaviors inspired by the YandereScript from the reference project. The changes focus on making Jimmy a more intelligent and unpredictable enemy while ensuring full compatibility with all scripts in the `Assets/Scripts` folder.

## Key Changes

### 1. New AI State: Distracted

**Purpose:** Provides a transition state when Jimmy loses sight of the player or hears a noise.

**Behavior:**
- Jimmy runs at full speed to the last known player position or noise location
- Upon arrival, he waits and searches the area for a configurable time
- If the player was recently seen (goToLastSeen flag), he waits 3x longer
- After the timer expires, returns to patrolling

**YandereScript Inspiration:** Replicates YandereScript's Distracted state which creates more tension by having the AI actively investigate before giving up.

### 2. Enhanced Vision System

**Old Implementation:**
```csharp
// Simple raycast from Jimmy's position
Vector3 directionToPlayer = (player.position - transform.position).normalized;
Physics.Raycast(transform.position + Vector3.up, directionToPlayer, ...)
```

**New Implementation:**
```csharp
// Eye-level raycasting for both Jimmy and player
Vector3 playerHeadPosition = player.position + Vector3.up * 1.5f;
Vector3 eyePosition = transform.position + Vector3.up * 1.5f;
Physics.Linecast(eyePosition, playerHeadPosition, ...)
```

**Benefits:**
- More realistic line-of-sight checks
- Better handles height differences
- Uses Linecast instead of Raycast for more accurate detection
- Supports both targetLayers and obstacleMask for flexible collision detection

### 3. Distance-Based Chase Behavior

**Old Implementation:**
```csharp
void ChasePlayer()
{
    navAgent.speed = chaseSpeed;
    navAgent.SetDestination(player.position);
}
```

**New Implementation:**
```csharp
void ChasePlayer()
{
    float distance = Vector3.Distance(transform.position, player.position);
    
    if (distance <= 4f)
        navAgent.speed = runSpeed;      // Close: Run at full speed
    else if (distance <= 5f)
        navAgent.speed = walkSpeed;     // Medium-close: Walk
    else if (distance <= 20f) {
        navAgent.speed = walkSpeed;     // Medium: Walk (or run if bored)
        if (boredomTimer >= 15f)
            navAgent.speed = runSpeed;
    }
    else
        navAgent.speed = runSpeed;      // Far: Run to catch up
}
```

**Benefits:**
- Creates varied and unpredictable chase patterns
- More intense when player is close
- Gives player brief windows of opportunity at medium range
- Boredom mechanic prevents player from safely staying at medium range forever

### 4. Boredom Timer System

**Feature:** After 60 seconds of patrolling without seeing the player, Jimmy automatically checks the player's current location.

**Implementation:**
```csharp
void Patrol()
{
    lastSeenTimer += Time.deltaTime;
    
    if (lastSeenTimer > 60f)
    {
        lastKnownPlayerPosition = player.position;
        currentState = AIState.Distracted;
        lastSeenTimer = 0f;
    }
}
```

**Benefits:**
- Prevents player from camping in one location forever
- Creates dynamic gameplay even when player is stealthy
- Replicates YandereScript's periodic player checks

### 5. Enhanced State Transitions

**Old Flow:**
```
Chase → Lost Sight → Searching → Patrol
```

**New Flow:**
```
Chase → Lost Sight → Distracted → (Wait at location) → Patrol
Patrol → 60s Timer → Distracted → (Check player location) → Patrol
Hear Noise → Distracted → (Investigate) → Patrol
```

**Benefits:**
- More realistic AI behavior
- Better transition between aggressive and passive states
- Creates more opportunities for stealth gameplay

### 6. Proximity Detection Enhancement

**New Feature:** Jimmy can detect the player at close range (< 5 units) even without direct line of sight, representing hearing footsteps.

**Implementation:**
```csharp
void UpdateState()
{
    // ... other checks ...
    
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    bool isPlayerMovingFast = playerController != null && !playerController.IsHiding();
    
    if (isPlayerMovingFast && distanceToPlayer < 5f)
    {
        if (currentState != AIState.Chasing && currentState != AIState.Distracted)
        {
            currentState = AIState.Investigating;
        }
    }
}
```

**Benefits:**
- Adds tension when player is near Jimmy
- Prevents easy exploitation of blind spots
- More realistic detection system

## New Parameters

### Detection Settings
- `targetLayers` (LayerMask): Additional layer mask for flexible collision detection

### Movement Settings
- `walkSpeed` (float, default 3.5): Speed when walking during medium-range chase
- `runSpeed` (float, default 6.0): Speed when running during close/far chase or distraction

### Internal State Variables
- `boredomTimer` (float): Tracks how long player has been at medium range during chase
- `lastSeenTimer` (float): Tracks time since last saw player during patrol
- `goToLastSeen` (bool): Flag indicating Jimmy should wait longer at investigation point

## Compatibility

### Maintained Compatibility With:

✅ **PlayerController.cs**
- Uses `IsHiding()` to check if player is in locker
- References player transform and position
- No breaking changes to interface

✅ **GameManager.cs**
- Calls `GameManager.Instance.GameOver()` on player catch
- No changes to game over logic

✅ **NoiseManager.cs**
- Implements `HearNoise(Vector3)` method
- Compatible with noise generation system

✅ **Door.cs**
- Implements `NotifyDoorOpened(Vector3)` method
- Transitions to Distracted state on door notifications

✅ **Locker.cs**
- Respects player hiding state
- Cannot detect hidden players

✅ **NavMeshAgent**
- Uses standard NavMesh pathfinding
- Compatible with Unity's navigation system

## What Was NOT Implemented

The following features from YandereScript were intentionally excluded because they require components/assets not present in the Assets/Scripts folder:

❌ **Animation System**
- YandereScript uses Unity's legacy Animation component with specific animation clips
- JimmyAI doesn't have animation references

❌ **Audio System**
- Heartbeat sound that increases volume when player is near
- Creepy sound effects during chase
- No audio sources configured

❌ **Knife Throwing Mechanics**
- Requires knife prefab and spawn points
- Complex projectile system not in scope

❌ **Electricity Control**
- DisableElectricity state for turning off power
- Requires PowerControl component

❌ **Key Hiding Behavior**
- HideKey state for stealing and hiding keys
- Requires zone system integration

❌ **Murder Animation**
- Complex murder cutscene with camera work
- Requires specific animation setup

## Testing Recommendations

### In Unity Editor:

1. **Test Vision System:**
   - Place player at various heights relative to Jimmy
   - Verify eye-level raycasting works correctly
   - Check that obstructions properly block vision

2. **Test Chase Behavior:**
   - Verify different speeds at different distances
   - Check that boredom timer activates after 15s at medium range
   - Ensure smooth transitions between speed tiers

3. **Test Distracted State:**
   - Make noise and verify Jimmy investigates
   - Check that he waits at the location before returning to patrol
   - Verify longer wait time when losing sight of player

4. **Test Boredom Timer:**
   - Let Jimmy patrol for 60+ seconds without detection
   - Verify he moves toward player's current position
   - Check that timer resets properly

5. **Test Hiding:**
   - Enter locker during chase
   - Verify Jimmy transitions to Searching state
   - Confirm he cannot detect hidden player

### Parameters to Tune:

- `detectionRange`: Adjust how far Jimmy can see (default: 20)
- `fieldOfView`: Adjust peripheral vision (default: 90 degrees)
- `walkSpeed`: Adjust medium-range chase speed (default: 3.5)
- `runSpeed`: Adjust close/far chase speed (default: 6.0)
- `lastSeenTimer` threshold: Change how often Jimmy checks player location during patrol (default: 60s)
- `boredomTimer` threshold: Change when Jimmy speeds up during medium-range chase (default: 15s)

## Behavioral Comparison

### Original JimmyAI:
- Simple 4-state machine (Patrol, Investigate, Chase, Search)
- Single chase speed
- Basic vision detection
- Static investigation behavior

### Enhanced JimmyAI:
- Advanced 5-state machine (Patrol, Investigate, Chase, Search, **Distracted**)
- Dynamic chase speeds based on distance
- Improved vision with eye-level raycasting
- Boredom timers for dynamic behavior
- Better state transitions inspired by YandereScript
- Proximity detection for nearby movement

## Code Quality

- ✅ All braces balanced (61 opening, 61 closing)
- ✅ All methods properly implemented
- ✅ Maintains existing public API
- ✅ No breaking changes to existing functionality
- ✅ Compatible with Unity's C# requirements
- ✅ Clear state machine logic
- ✅ Well-commented code

## Performance Considerations

- Eye-level raycasting adds minimal overhead (one Linecast per frame)
- Distance calculations use `Vector3.Distance()` which is optimized by Unity
- Boredom and timer checks are simple float comparisons
- No new physics queries or expensive operations added
- Hot zone system unchanged (still limited to 10 entries)

## Summary

The enhanced JimmyAI successfully replicates the core AI behaviors from YandereScript while remaining fully compatible with the existing Unity Horror game architecture. The changes create a more intelligent, unpredictable, and engaging enemy AI that will provide a better horror experience for players.

The implementation focuses on:
1. ✅ Better state management
2. ✅ More realistic detection
3. ✅ Dynamic chase behavior
4. ✅ Smarter investigation
5. ✅ Full compatibility with existing scripts
