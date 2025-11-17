# Quick Start Guide

Get your Unity Horror Game running in 15 minutes!

## Prerequisites
- Unity 2020.3 or later installed
- Basic familiarity with Unity Editor

## Fast Setup (Minimal Scene)

### Step 1: Create Unity Project (2 minutes)
1. Open Unity Hub
2. New Project → 3D Template
3. Name it "HorrorGame"
4. Copy all files from this repository into the project folder

### Step 2: Create Basic Scene (5 minutes)

#### Create Ground
1. GameObject → 3D Object → Plane
2. Scale: (5, 1, 5)
3. Name: "Ground"
4. In Navigation window: Mark as "Navigation Static"

#### Create Player
1. GameObject → Create Empty → Name: "Player"
2. Tag: "Player"
3. Add Component → Character Controller
4. Add Component → Player Controller (script)
5. Position: (0, 1, 0)

#### Add Camera to Player
1. Right-click Player → Camera
2. Position: (0, 0.6, 0)
3. Add Component → Camera Controller (script)
4. Drag Player into "Player Body" field

#### Create Jimmy
1. GameObject → 3D Object → Capsule → Name: "Jimmy"
2. Position: (10, 1, 0)
3. Material: Make it red/dark (optional)
4. Add Component → Nav Mesh Agent
5. Add Component → Capsule Collider
   - Check "Is Trigger"
6. Add Component → Jimmy AI (script)
7. Drag Player into "Player" field

#### Create Patrol Points
1. GameObject → Create Empty → Name: "PatrolPoint1"
2. Position: (5, 0, 5)
3. Duplicate for PatrolPoint2 at (10, 0, 10)
4. Duplicate for PatrolPoint3 at (15, 0, 5)
5. In Jimmy AI, set Patrol Points size to 3
6. Drag all 3 patrol points into array

#### Create Key and Door
1. GameObject → 3D Object → Cube → Name: "Key_1"
   - Scale: (0.2, 0.2, 0.2)
   - Position: (8, 0.5, 8)
   - Add Component → Key (script)
   - Set Key ID: "Key_1"

2. GameObject → 3D Object → Cube → Name: "Door_1"
   - Scale: (2, 3, 0.2)
   - Position: (15, 1.5, 0)
   - Add Component → Door (script)
   - Set Required Key ID: "Key_1"
   - Check "Is Locked"

#### Create Locker
1. GameObject → 3D Object → Cube → Name: "Locker_1"
   - Scale: (1, 2, 1)
   - Position: (3, 1, 3)
   - Add Component → Locker (script)

#### Create Game Manager
1. GameObject → Create Empty → Name: "GameManager"
2. Add Component → Game Manager (script)
   - Total Keys Required: 1
3. Add Component → Noise Manager (script)

### Step 3: Bake Navigation (2 minutes)
1. Window → AI → Navigation
2. Select Ground plane
3. Check "Navigation Static" in Inspector
4. Navigation window → Bake tab → Click "Bake"

### Step 4: Test Play (1 minute)
1. Click Play button
2. Use WASD to move
3. Mouse to look around
4. Walk to key and press E
5. Walk to door and press E (should unlock)
6. Try to avoid Jimmy!

## Controls Reminder
- **WASD** - Move
- **Shift** - Sprint (louder)
- **Ctrl** - Crouch (quieter)
- **E** - Interact
- **Mouse** - Look

## Quick Troubleshooting

### Jimmy doesn't move
- Check Navigation is baked
- Check patrol points are set
- Check NavMeshAgent is present

### Can't pick up key
- Check you're close enough (default 2 units)
- Press E when near the key
- Check key has Key script

### Door won't open
- Check key ID matches door's required key ID
- Make sure you picked up the key first
- Check door has Door script

### Player falls through ground
- Add a collider to the ground plane
- Check CharacterController is present on player

## What's Happening?

### When you move:
- Player generates noise based on speed
- Jimmy hears the noise if close enough
- Jimmy investigates the location

### When you pick up key:
- Key is added to inventory
- Key object is hidden
- You can now unlock matching door

### When you open door:
- Door swings open
- Noise is generated
- Jimmy is alerted to the door position

### When Jimmy sees you:
- Changes to chase mode
- Runs faster toward you
- Game over if he catches you

### When you hide in locker:
- Player becomes invisible
- Jimmy can't detect you
- Safe until you exit

## Expand Your Game

### Add More Keys and Doors
1. Duplicate existing key
2. Change Key ID to "Key_2"
3. Duplicate door
4. Change Required Key ID to "Key_2"
5. Update Game Manager: Total Keys Required = 2

### Make the Level Bigger
1. Add more planes or terrain
2. Add walls (cubes scaled long and thin)
3. Mark all as "Navigation Static"
4. Re-bake navigation
5. Add more patrol points

### Add More Lockers
1. Duplicate existing locker
2. Position around the map
3. That's it! They work automatically

### Make Jimmy Smarter
In Jimmy AI Inspector:
- Increase Detection Range (see further)
- Decrease Field of View (harder to hide)
- Increase Chase Speed (harder to escape)
- Add more patrol points (covers more area)

### Make it Scarier
- Reduce lighting (darker)
- Add fog (Window → Rendering → Lighting)
- Add sound effects (footsteps, ambient)
- Add scary 3D models for Jimmy
- Add textures to environment

## Next Steps

Once you have the basic game working:
1. Read SETUP_GUIDE.md for detailed scene setup
2. Read MECHANICS_REFERENCE.md to understand all systems
3. Read ARCHITECTURE.md to understand the code structure
4. Design your own level layout
5. Add your own art and sound
6. Test with friends!

## Performance Tips
- Keep patrol points under 10
- Don't make the level too big initially
- Use occlusion culling for large levels
- Bake lighting instead of real-time

## Common Enhancements

### Add UI
- Create Canvas → Text for key counter
- Update GameManager to display text
- Show "Press E to interact" hints

### Add Health System
- Player can take a hit before game over
- Jimmy does damage instead of instant kill
- Add health bar UI

### Add Stamina
- Sprinting consumes stamina
- Must rest to recover
- Adds tension to chases

### Add Collectibles
- Notes that tell a story
- Optional items for achievements
- Extra keys for bonus doors

### Add Multiple Enemies
- Duplicate Jimmy
- Each patrols different area
- Coordinate or independent behavior

### Add Difficulty Levels
- Easy: Slower Jimmy, more lockers
- Normal: Current settings
- Hard: Faster Jimmy, fewer hiding spots

## File Structure
```
Unity_Horror/
├── Assets/
│   ├── Scenes/
│   │   └── MainScene.unity
│   ├── Scripts/
│   │   ├── PlayerController.cs
│   │   ├── CameraController.cs
│   │   ├── JimmyAI.cs
│   │   ├── Key.cs
│   │   ├── Door.cs
│   │   ├── Locker.cs
│   │   ├── GameManager.cs
│   │   └── NoiseManager.cs
│   └── Prefabs/ (optional)
├── README.md
├── SETUP_GUIDE.md
├── MECHANICS_REFERENCE.md
└── ARCHITECTURE.md
```

## Success!

If you can:
- Move around with WASD
- Pick up a key
- Unlock a door
- Hide in a locker
- See Jimmy patrol

**Congratulations!** Your horror game is working! 🎮👻

Now make it your own by:
- Designing interesting levels
- Adding your own art style
- Creating puzzle elements
- Writing a story
- Adding more scares!

## Need Help?

Check the detailed guides:
- **SETUP_GUIDE.md** - Comprehensive Unity setup
- **MECHANICS_REFERENCE.md** - All game systems explained
- **ARCHITECTURE.md** - How the code works
- **README.md** - Project overview

Happy game developing! 🎃
