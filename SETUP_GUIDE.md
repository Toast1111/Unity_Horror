# Unity Horror Game - Detailed Setup Guide

## Step-by-Step Scene Setup

### 1. Create New Unity Scene
1. Open Unity
2. Create a new 3D scene
3. Save it in `Assets/Scenes/` folder

### 2. Setup Player

#### Create Player GameObject:
```
Hierarchy:
└── Player (Tag: "Player")
    ├── Main Camera
    └── Body (optional visual representation)
```

#### Player Configuration:
1. Select Player GameObject
2. Add Component → Character Controller
   - Height: 2
   - Radius: 0.5
   - Center: (0, 1, 0)
3. Add Component → Player Controller (script)
   - Set interaction range, speeds as desired
4. Position: Start position in your level

#### Camera Configuration:
1. Select Main Camera (child of Player)
2. Add Component → Camera Controller (script)
   - Mouse Sensitivity: 100
   - Player Body: Drag Player GameObject here
3. Position: (0, 0.6, 0) relative to player
4. Rotation: (0, 0, 0)

### 3. Setup Jimmy (Enemy)

#### Create Jimmy GameObject:
```
Hierarchy:
└── Jimmy
    └── Model (optional visual representation)
```

#### Jimmy Configuration:
1. Select Jimmy GameObject
2. Add Component → Nav Mesh Agent
   - Speed: 3.5
   - Angular Speed: 120
   - Acceleration: 8
   - Stopping Distance: 2
3. Add Component → Capsule Collider
   - Is Trigger: ✓ (checked)
   - Radius: 0.5
   - Height: 2
4. Add Component → Jimmy AI (script)
   - Player: Drag Player GameObject here
   - Detection Range: 15
   - Field Of View: 120
   - Patrol Speed: 3
   - Chase Speed: 6

#### Create Patrol Points:
1. Create Empty GameObjects named "PatrolPoint1", "PatrolPoint2", etc.
2. Position them around your level
3. In Jimmy AI script, set Patrol Points array size
4. Drag patrol point GameObjects into the array

### 4. Create Doors

#### For each door:
1. Create GameObject named "Door_1", "Door_2", etc.
2. Add a visual representation (cube or 3D model)
3. Add Component → Box Collider
4. Add Component → Door (script)
   - Required Key ID: "Key_1" (match with corresponding key)
   - Is Locked: ✓ (checked)
   - Door Transform: Drag the visual door object
   - Open Rotation: (0, 90, 0)
5. Position door in doorway

### 5. Create Keys

#### For each key:
1. Create GameObject named "Key_1", "Key_2", etc.
2. Add visual representation (small cube, cylinder, or 3D model)
3. Add Component → Box Collider or Sphere Collider
4. Add Component → Key (script)
   - Key ID: "Key_1" (match with door's required key ID)
   - Key Name: "Blue Key" (or descriptive name)
5. Position key in level (possibly behind locked doors)

### 6. Create Lockers

#### For each locker:
1. Create GameObject named "Locker_1", "Locker_2", etc.
2. Add visual representation (tall box or 3D model)
3. Add Component → Box Collider
4. Add Component → Locker (script)
5. Position lockers around the level

### 7. Setup Level Navigation

#### Bake NavMesh:
1. Window → AI → Navigation
2. Select all floor/walkable surfaces
3. In Navigation window, mark as "Navigation Static"
4. Click "Bake" tab
5. Adjust settings:
   - Agent Radius: 0.5
   - Agent Height: 2
   - Max Slope: 45
6. Click "Bake" button

### 8. Setup Game Manager

#### Create Manager GameObject:
1. Create Empty GameObject named "GameManager"
2. Add Component → Game Manager (script)
   - Total Keys Required: 3 (or number of keys in level)
3. Add Component → Noise Manager (script)

### 9. Configure Layers (Optional but Recommended)

1. Edit → Project Settings → Tags and Layers
2. Add layers:
   - Player
   - Enemy
   - Interactable
3. Assign layers to respective GameObjects
4. In Jimmy AI, set Obstacle Mask to exclude "Enemy" layer

### 10. Lighting (Optional)

For horror atmosphere:
1. Window → Rendering → Lighting
2. Reduce ambient light intensity
3. Add Point Lights or Spotlights sparingly
4. Use dark colors for atmosphere

## Testing Your Game

### Test Checklist:
- [ ] Player can move with WASD
- [ ] Camera rotates with mouse
- [ ] Player can sprint with Shift
- [ ] Player can crouch with Ctrl
- [ ] Player can pick up keys
- [ ] Keys unlock correct doors
- [ ] Doors open after unlocking
- [ ] Player can hide in lockers
- [ ] Jimmy patrols between waypoints
- [ ] Jimmy hears player movement
- [ ] Jimmy investigates noise
- [ ] Jimmy chases player when seen
- [ ] Jimmy can catch player (game over)
- [ ] Jimmy cannot detect player in locker
- [ ] Opening doors alerts Jimmy

## Example Level Layout

```
[Start Area]
    ├── Player Spawn
    ├── Locker_1
    └── Door_1 (locked, needs Key_1)
        │
        └─→ [Room 1]
            ├── Key_2
            ├── Locker_2
            └── Door_2 (locked, needs Key_2)
                │
                └─→ [Room 2]
                    ├── Key_3
                    └── Door_3 (locked, needs Key_3)
                        │
                        └─→ [Exit Room]
                            └── Key_1 (creates backtracking)

[Patrol Route]
PatrolPoint1 → PatrolPoint2 → PatrolPoint3 → PatrolPoint4 → PatrolPoint1
```

## Common Issues and Solutions

### Jimmy doesn't move:
- Check NavMesh is baked
- Ensure Jimmy has NavMeshAgent component
- Verify patrol points are set

### Player can't pick up keys:
- Check interaction range in PlayerController
- Ensure keys have colliders
- Verify player is close enough

### Doors don't open:
- Check key ID matches door's required key ID
- Ensure door has Door script
- Verify door isn't locked without the key

### Jimmy can't see player:
- Check detection range and field of view
- Ensure no obstacles blocking view
- Verify layer masks are correct

## Performance Tips

1. Use occlusion culling for larger levels
2. Limit number of active lights
3. Use LOD (Level of Detail) for 3D models
4. Bake lighting when possible
5. Keep patrol point count reasonable (5-10 points)

## Next Steps

After basic setup:
1. Add sound effects (footsteps, door creaks, ambient)
2. Improve visuals (models, textures, post-processing)
3. Add UI (key counter, health, stamina)
4. Implement multiple difficulty levels
5. Add save/load system
6. Create multiple levels
