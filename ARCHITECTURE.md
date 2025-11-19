# System Architecture

## Component Interaction Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                         GAME MANAGER                             │
│  (Singleton - Manages game state, win/lose conditions)          │
└────────────────┬────────────────────────────────────────────────┘
                 │
                 │ Checks win condition
                 │
    ┌────────────┴──────────────┐
    │                           │
    ▼                           ▼
┌──────────────┐          ┌──────────────┐
│   PLAYER     │◄────────►│   JIMMY AI   │
│              │  Detects │              │
│ - Movement   │          │ - Patrol     │
│ - Interact   │          │ - Chase      │
│ - Inventory  │          │ - Search     │
│ - Hide       │          │ - Investigate│
└──┬─────┬─────┘          └──────┬───────┘
   │     │                       │
   │     │                       │ Listens for
   │     │                       │
   │     │   ┌───────────────────┴────────────────┐
   │     │   │         NOISE MANAGER               │
   │     │   │  (Singleton - Propagates noise)    │
   │     │   └───────────────────┬────────────────┘
   │     │                       │
   │     │                       │ Generates noise
   │     │                       │
   │     └───────────────────────┘
   │
   │ Interacts with
   │
   ├─────────► KEYS
   │           - Pick up
   │           - Add to inventory
   │
   ├─────────► DOORS
   │           - Unlock with key
   │           - Open (creates noise)
   │           - Notify Jimmy
   │
   └─────────► LOCKERS
               - Hide inside
               - Become undetectable


┌─────────────────────────────────────────────────────────────────┐
│                    CAMERA CONTROLLER                             │
│           (Child of Player - First Person View)                 │
└─────────────────────────────────────────────────────────────────┘
```

## Data Flow

### Player Movement → Noise Generation
```
Player Input (WASD + Shift/Ctrl)
    ↓
PlayerController.HandleMovement()
    ↓
Calculate movement speed (walk/sprint/crouch)
    ↓
NoiseManager.GenerateNoise(position, radius)
    ↓
JimmyAI.HearNoise(position)
```

### Key Collection → Door Unlocking
```
Player presses E near Key
    ↓
PlayerController.HandleInteraction()
    ↓
PlayerController.PickUpKey()
    ↓
Key added to inventory
    ↓
Player presses E near locked Door
    ↓
PlayerController.TryOpenDoor()
    ↓
Check inventory for matching key
    ↓
Door.Unlock() → Door.Open()
    ↓
Door.NotifyDoorOpened()
    ↓
JimmyAI.NotifyDoorOpened(position)
```

### Player Detection → Chase
```
JimmyAI.Update() called every frame
    ↓
JimmyAI.UpdateState()
    ↓
Check if player is hiding → If yes, skip detection
    ↓
JimmyAI.CanSeePlayer()
    ├─ Check distance
    ├─ Check field of view
    └─ Check line of sight
    ↓
If detected: Change state to Chasing
    ↓
JimmyAI.ChasePlayer()
    ↓
NavMeshAgent.SetDestination(player.position)
```

### Hiding in Locker
```
Player presses E near Locker
    ↓
PlayerController.HandleInteraction()
    ↓
Check if locker is occupied
    ↓
PlayerController.EnterLocker()
    ↓
Set isHiding = true
    ↓
Locker.Hide(player)
    ↓
Player GameObject.SetActive(false)
    ↓
Jimmy cannot detect player
    ↓
Player presses E to exit
    ↓
PlayerController.ExitLocker()
    ↓
Player GameObject.SetActive(true)
```

## State Machine: Jimmy AI

```
                 ┌─────────────┐
                 │   START     │
                 └──────┬──────┘
                        │
                        ▼
               ┌────────────────┐
          ┌────┤   PATROLLING   ├────┐
          │    └────────────────┘    │
          │            │              │
  Hears   │            │              │ 60s timer
  noise   │            │ Sees player  │ expires
  or door │            ▼              │
          │    ┌────────────────┐    │
          └───►│   CHASING      │    │
               └────────┬───────┘    │
                        │             │
          Lost sight    │             │
          of player     │             │
                        ▼             │
               ┌────────────────┐    │
         ┌────►│  DISTRACTED    ├────┘
         │     └────────┬───────┘
         │              │
  Door   │              │ Timer expires
  opened │              │ at location
         │              ▼
         │     ┌────────────────┐
         └─────┤ INVESTIGATING  │
               └────────────────┘
                        │
          Timeout       │
                        │
                        └─────► Back to PATROLLING
                        
               ┌────────────────┐
               │   SEARCHING    │◄─── Player hides
               └────────────────┘     during chase
                        │
                        │ Timer expires
                        ▼
                  Back to PATROLLING
```

## Class Hierarchy

```
MonoBehaviour
│
├── PlayerController
│   ├── Movement system
│   ├── Interaction system
│   ├── Inventory system
│   └── Hiding system
│
├── CameraController
│   └── Mouse look system
│
├── JimmyAI
│   ├── NavMeshAgent (Unity component)
│   ├── Detection systems
│   ├── State machine
│   ├── Patrol system
│   └── Hot zone system
│
├── Key
│   └── Pickup logic
│
├── Door
│   ├── Lock/unlock logic
│   ├── Opening animation
│   └── Notification system
│
├── Locker
│   └── Hide/unhide logic
│
├── GameManager (Singleton)
│   ├── Win/lose conditions
│   └── Game state management
│
├── NoiseManager (Singleton)
│   └── Noise propagation
│
└── SceneSetupExample
    └── Reference configurations
```

## Dependencies

### PlayerController depends on:
- UnityEngine.CharacterController
- NoiseManager (via singleton)
- Key, Door, Locker (for interaction)

### JimmyAI depends on:
- UnityEngine.AI.NavMeshAgent
- PlayerController (reference)
- GameManager (for game over)

### Door depends on:
- JimmyAI (for notifications)

### GameManager depends on:
- PlayerController (for key count)
- UnityEngine.SceneManagement

### NoiseManager depends on:
- JimmyAI (to notify)

## Singleton Pattern

Two singleton managers are used:

```
NoiseManager.Instance
    ↓
Handles all noise generation
    ↓
Propagates to all JimmyAI instances

GameManager.Instance
    ↓
Manages game state
    ↓
Checks win/lose conditions
```

## Unity Component Requirements

### Player GameObject
```
Player (Tag: "Player")
├── CharacterController
├── PlayerController (script)
└── Camera (child)
    └── CameraController (script)
```

### Jimmy GameObject
```
Jimmy
├── NavMeshAgent
├── CapsuleCollider (IsTrigger: true)
└── JimmyAI (script)
```

### Door GameObject
```
Door
├── Collider
├── Door (script)
└── Door Visual (child, optional)
```

### Key GameObject
```
Key
├── Collider
└── Key (script)
```

### Locker GameObject
```
Locker
├── Collider
└── Locker (script)
```

### Manager GameObjects
```
GameManager
├── GameManager (script)
└── NoiseManager (script)
```

## Event Flow Example: Complete Playthrough

```
1. Game Start
   ├─ Player spawns
   ├─ Jimmy begins patrol
   └─ GameManager initializes

2. Player moves (sprinting)
   ├─ CharacterController moves player
   ├─ NoiseManager.GenerateNoise(15 radius)
   └─ Jimmy hears noise → INVESTIGATING state

3. Jimmy investigates noise location
   ├─ NavMeshAgent pathfinds to location
   ├─ Adds location to hot zones
   └─ After investigation → PATROLLING state

4. Player finds Key_1
   ├─ Presses E near key
   ├─ Key added to inventory
   └─ Key GameObject deactivated

5. Player opens Door_1
   ├─ Presses E near door
   ├─ Checks inventory for Key_1
   ├─ Door unlocks and opens
   ├─ NoiseManager.GenerateNoise(10 radius)
   └─ Jimmy notified → INVESTIGATING state

6. Player collects all keys (3/3)
   ├─ GameManager.CheckWinCondition()
   └─ GameManager.WinGame() → Game ends

Alternative ending:

6. Jimmy sees player
   ├─ CanSeePlayer() returns true
   ├─ State changes to CHASING
   └─ Pursues player at chase speed

7. Player hides in locker
   ├─ Presses E near locker
   ├─ Player.SetActive(false)
   └─ Jimmy cannot detect → SEARCHING state

8. Player exits locker at wrong time
   ├─ Jimmy still nearby
   ├─ OnTriggerEnter detects player
   └─ GameManager.GameOver() → Game ends
```

## Performance Characteristics

### Update Frequency
- **Every Frame:**
  - PlayerController.Update()
  - JimmyAI.Update()
  - CameraController.Update()
  - Door.Update() (if opening)

### Singleton Calls
- **NoiseManager:** Called on player movement
- **GameManager:** Called on key collection

### Physics Queries
- **OverlapSphere:** Player interaction checks (every E press)
- **Raycast:** Jimmy line of sight (every frame)
- **NavMesh:** Jimmy pathfinding (on destination change)

### Memory Management
- Hot zones limited to 10 entries (prevents unbounded growth)
- Key inventory uses List<Key> (grows with collected keys)
- Singleton pattern ensures single manager instances

## Extensibility Points

### Adding New Interactables
1. Create new class inheriting MonoBehaviour
2. Add to PlayerController.HandleInteraction()
3. Implement interaction logic

### Adding New AI States
1. Add to AIState enum in JimmyAI
2. Add case in Update() switch statement
3. Implement state behavior method

### Adding New Detection Types
1. Add detection logic to UpdateState()
2. Call appropriate state change
3. Update hot zones if needed

### Multiple Enemies
- Already supported
- NoiseManager notifies all JimmyAI instances
- Each maintains independent state
- Each has own NavMeshAgent
