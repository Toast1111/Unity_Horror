# Quick Start Testing Guide

This guide helps you quickly test the new door interaction features in Unity Editor.

## Prerequisites
- Unity 2020.3 or later
- This project opened in Unity Editor
- Basic Unity scene with NavMesh

## Quick Setup (5 minutes)

### 1. Create UI System
1. Right-click in Hierarchy → UI → Canvas
2. Right-click on Canvas → UI → Panel (name it "PromptPanel")
3. Right-click on PromptPanel → UI → Text (name it "PromptText")
4. Create Empty GameObject named "InteractionUI"
5. Add InteractionUI.cs component to it
6. Drag PromptPanel and PromptText into the component fields

### 2. Test Rotating Door (Existing Feature)
1. Create a Cube (this will be your door)
2. Add Door.cs component
3. Set "Door Type" to "Rotating"
4. Set "Is Locked" to false (for testing)
5. Add a Box Collider if not present
6. Test: Run the game, walk near door, press E

### 3. Test Single Sliding Door
1. Create Empty GameObject named "SlidingDoorParent"
2. Create Child Cube named "DoorPanel"
3. Add Door.cs to SlidingDoorParent
4. Set "Door Type" to "Sliding Single"
5. Drag DoorPanel to "Left Door" field
6. Set "Slide Direction" to (1, 0, 0) [right]
7. Set "Slide Distance" to 2
8. Set "Is Locked" to false
9. Test: Run the game, walk near door, press E

### 4. Test Double Sliding Door
1. Create Empty GameObject named "DoubleSlidingDoorParent"
2. Create Child Cube named "LeftPanel" (position at -0.5, 0, 0)
3. Create Child Cube named "RightPanel" (position at 0.5, 0, 0)
4. Add Door.cs to DoubleSlidingDoorParent
5. Set "Door Type" to "Sliding Double"
6. Drag LeftPanel to "Left Door" field
7. Drag RightPanel to "Right Door" field
8. Set "Slide Direction" to (1, 0, 0)
9. Set "Slide Distance" to 1.5
10. Set "Is Locked" to false
11. Test: Run the game, walk near door, press E

### 5. Test Locked Door with Key
1. Create a door (any type)
2. Set "Is Locked" to true
3. Set "Required Key ID" to "TestKey"
4. Create a Sphere (this will be your key)
5. Add Key.cs component
6. Set "Key ID" to "TestKey"
7. Add a Sphere Collider if not present
8. Test: Run game, pick up key, unlock door

### 6. Test Jimmy Opening Doors
1. Ensure you have a Jimmy GameObject with JimmyAI.cs
2. Create an unlocked door between two patrol points
3. Run the game and watch Jimmy patrol
4. Observe: Jimmy should open the door when passing through

## Expected Behaviors

### UI Prompts Should Show:
- "Press E to Open Door" when near closed, unlocked door
- "Press E to Close Door" when near open door
- "Door is Locked - Need Key" when near locked door without key
- "Press E to Unlock Door" when near locked door with correct key
- "Press E to Pick Up Key" when near key
- "Press E to Hide in Locker" when near locker

### Doors Should:
- Rotating: Swing open smoothly when opened
- Single Sliding: Slide to one side when opened
- Double Sliding: Both panels slide apart when opened
- All types: Animate back to closed position when closed

### Jimmy Should:
- Open unlocked doors in his path automatically
- NOT open locked doors
- Continue patrolling after opening doors

## Common Issues

**Prompts don't appear:**
- Check InteractionUI GameObject exists and has component
- Verify PromptPanel and PromptText are assigned
- Check Player has correct tag "Player"

**Doors don't move:**
- Rotating: Check doorTransform is assigned
- Sliding: Check leftDoor/rightDoor transforms are assigned
- All: Check transforms are children of Door GameObject

**Jimmy doesn't open doors:**
- Verify door "Is Locked" is false
- Check "Door Detection Range" in JimmyAI (try increasing to 3)
- Ensure door is on Jimmy's path between patrol points

## Performance Test

Run the game with:
- 10+ doors of mixed types
- 2+ Jimmy AI instances
- Monitor frame rate

Should maintain 60+ FPS on modern hardware.

## What to Report

If you find issues, please report:
1. Unity version
2. Door type being tested
3. Steps to reproduce
4. Expected vs actual behavior
5. Console errors (if any)

## Next Steps

After testing:
1. Adjust door speeds, distances, and other parameters
2. Add door sounds (AudioSource component)
3. Create more complex door configurations
4. Design levels with strategic door placement
5. Balance locked vs unlocked doors for gameplay

## Tips

- Use unlocked doors in main pathways
- Use locked doors to gate progression
- Mix door types for visual variety
- Place keys strategically to guide exploration
- Test with multiple players in multiplayer (if applicable)

## Success Criteria

✅ All three door types work correctly
✅ UI prompts appear with correct messages
✅ Player can open, close, and unlock doors
✅ Jimmy opens unlocked doors automatically
✅ No console errors
✅ Smooth performance
✅ Backward compatible with existing scenes

Happy testing!
