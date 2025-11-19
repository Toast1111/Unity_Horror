# Door Interaction Features Guide

## Overview
This guide explains the new door interaction features added to the Unity Horror game, including sliding doors, UI prompts, and NPC door interaction.

## New Features

### 1. Sliding Door Support
Doors now support three different opening mechanisms:
- **Rotating Doors** (default) - Doors that swing open on a hinge
- **Single Sliding Doors** - Single door that slides in one direction
- **Double Sliding Doors** - Two doors that slide apart (like elevator doors)

### 2. UI Interaction Prompts
Players now receive context-sensitive prompts when near interactive objects:
- "Press E to Open Door" - For closed, unlocked doors
- "Press E to Close Door" - For open doors
- "Door is Locked - Need Key" - For locked doors without the correct key
- "Press E to Unlock Door" - For locked doors when holding the correct key
- "Press E to Pick Up Key" - For keys
- "Press E to Hide in Locker" - For lockers
- "Press E to Exit Locker" - When hiding in a locker

### 3. Jimmy Can Open Doors
The enemy AI (Jimmy) can now open closed but unlocked doors when pathing through the level.

## Setup Instructions

### Setting Up the UI System

1. Create a Canvas in your scene:
   - Right-click in Hierarchy → UI → Canvas
   - Set Canvas Scaler to "Scale With Screen Size"

2. Create a Panel for the prompt:
   - Right-click on Canvas → UI → Panel
   - Name it "PromptPanel"
   - Adjust size and position (recommended: bottom center of screen)
   - Adjust transparency/background color as desired

3. Add a Text element:
   - Right-click on PromptPanel → UI → Text
   - Name it "PromptText"
   - Set font size and alignment (recommended: 24pt, centered)
   - Adjust color for visibility

4. Create an InteractionUI GameObject:
   - Right-click in Hierarchy → Create Empty
   - Name it "InteractionUI"
   - Add the InteractionUI.cs component
   - Drag the PromptText and PromptPanel into the component's fields

### Setting Up Rotating Doors (Default)

1. Create a door GameObject
2. Add the Door.cs script
3. Configure the Door component:
   - **Door Type**: Set to "Rotating"
   - **Door Transform**: Assign the door mesh transform
   - **Open Rotation**: Set to (0, 90, 0) or desired rotation
   - **Open Speed**: Adjust animation speed (default: 2)
   - **Required Key ID**: Set to match a key ID (e.g., "Key_1")
   - **Is Locked**: Check if door should start locked
4. Add a Collider component to make the door interactable

### Setting Up Single Sliding Doors

1. Create a door GameObject structure:
   ```
   DoorParent (with Door.cs script)
   └── DoorMesh (the visible door model)
   ```

2. Configure the Door component:
   - **Door Type**: Set to "Sliding Single"
   - **Left Door**: Assign the DoorMesh transform
   - **Slide Direction**: Set to direction door should slide (e.g., Vector3.right for right)
   - **Slide Distance**: Set how far to slide (e.g., 1.5)
   - **Slide Speed**: Adjust animation speed (default: 2)
   - **Required Key ID**: Set to match a key ID
   - **Is Locked**: Check if door should start locked

### Setting Up Double Sliding Doors

1. Create a door GameObject structure:
   ```
   DoorParent (with Door.cs script)
   ├── LeftDoorMesh
   └── RightDoorMesh
   ```

2. Configure the Door component:
   - **Door Type**: Set to "Sliding Double"
   - **Left Door**: Assign the LeftDoorMesh transform
   - **Right Door**: Assign the RightDoorMesh transform
   - **Slide Direction**: Set to direction doors should slide (e.g., Vector3.right)
   - **Slide Distance**: Set how far to slide (e.g., 1.5)
   - **Slide Speed**: Adjust animation speed (default: 2)
   - **Required Key ID**: Set to match a key ID
   - **Is Locked**: Check if door should start locked

### Setting Up Jimmy to Open Doors

1. Ensure your Jimmy AI GameObject has the JimmyAI.cs script
2. Configure the Door Interaction Settings (new in JimmyAI component):
   - **Door Detection Range**: Distance to detect doors (default: 2)
   - **Door Open Check Interval**: How often to check for doors (default: 0.5 seconds)
3. Make sure doors are on a layer that Jimmy can detect

## Usage

### For Players
- Approach any door, locker, or key
- The UI prompt will automatically appear showing what action you can take
- Press E to interact (open/close doors, pick up keys, hide in lockers)
- Doors can be opened and closed repeatedly
- Locked doors require the matching key

### For Level Designers
- Mix and match door types throughout your level
- Use rotating doors for room entrances
- Use sliding doors for sci-fi/modern areas or tight spaces
- Use double sliding doors for dramatic entrances or large openings
- Adjust slide direction and distance to match your door models

## Technical Details

### Door.cs Changes
- Added `DoorType` enum with three options
- Added sliding door properties (leftDoor, rightDoor, slideDirection, slideDistance, slideSpeed)
- Update() now handles all three door types
- Added `Toggle()` method to open/close doors
- Close() now properly animates doors to closed position

### InteractionUI.cs (New)
- Singleton pattern for easy access
- Methods for showing each type of prompt
- Automatic prompt hiding when not near objects

### PlayerController.cs Changes
- Added `UpdateInteractionUI()` method
- Checks nearby objects each frame
- Shows appropriate prompts based on object type and state
- Added `UpdateHidingUI()` for locker prompts

### JimmyAI.cs Changes
- Added `CheckAndOpenDoors()` method
- Checks for doors in path periodically
- Opens unlocked doors automatically
- Only opens doors when actively moving

## Testing Checklist

- [ ] Rotating doors open and close smoothly
- [ ] Single sliding doors slide in the correct direction
- [ ] Double sliding doors slide apart symmetrically
- [ ] UI prompts appear when near objects
- [ ] UI prompts show correct message for each object type
- [ ] UI prompts disappear when moving away
- [ ] Player can unlock doors with correct key
- [ ] Player cannot open locked doors without key
- [ ] Player can toggle doors open and closed
- [ ] Jimmy opens unlocked doors in his path
- [ ] Jimmy does not open locked doors
- [ ] Door sounds play (if configured)
- [ ] Multiple door types work in the same scene

## Performance Notes

- Jimmy checks for doors every 0.5 seconds (configurable)
- UI prompts update every frame but only for nearby objects (within interaction range)
- Door animations use Lerp/Slerp for smooth performance

## Troubleshooting

**UI prompts don't appear:**
- Check that InteractionUI instance exists in scene
- Verify PromptText and PromptPanel are assigned
- Check that objects have colliders

**Doors don't animate:**
- For rotating: Verify doorTransform is assigned
- For sliding: Verify leftDoor/rightDoor transforms are assigned
- Check that the transforms are children of the Door GameObject

**Jimmy doesn't open doors:**
- Verify doors are unlocked
- Check Door Detection Range is large enough
- Ensure Jimmy is actively pathing (has a destination)
- Make sure doors have colliders

**Sliding doors slide in wrong direction:**
- Adjust the slideDirection vector (e.g., Vector3.right, Vector3.left, Vector3.forward)
- Adjust the slideDistance value
- Check that the door mesh pivot point is correct

## Future Enhancements

Possible additions:
- Automatic doors that open when player/Jimmy approaches
- Doors that lock behind the player
- Breakable doors
- Doors that require multiple keys
- Door open/close sounds integration
- Animated door handles
- Security keypads/card readers
