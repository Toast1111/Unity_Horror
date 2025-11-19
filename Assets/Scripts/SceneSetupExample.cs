using UnityEngine;

/// <summary>
/// Example scene setup script for testing purposes.
/// This demonstrates how to programmatically set up the game objects.
/// In a real Unity project, you would set these up in the Scene Editor.
/// </summary>
public class SceneSetupExample : MonoBehaviour
{
    [Header("Example Setup - For Reference Only")]
    [Tooltip("This script is for documentation purposes. Set up your scene in the Unity Editor instead.")]
    public bool enableExampleSetup = false;
    
    void Start()
    {
        if (!enableExampleSetup)
        {
            Debug.Log("SceneSetupExample is disabled. Enable it to see programmatic setup example.");
            return;
        }
        
        // This is just an example - in practice, create objects in Unity Editor
        Debug.Log("=== Scene Setup Example ===");
        Debug.Log("1. Create Player with CharacterController and PlayerController");
        Debug.Log("2. Add Camera with CameraController as child of Player");
        Debug.Log("3. Create Jimmy with NavMeshAgent and JimmyAI");
        Debug.Log("4. Create Doors with Door script and matching key IDs");
        Debug.Log("   - Set Door Type to Rotating, SlidingSingle, or SlidingDouble");
        Debug.Log("   - For rotating: Assign doorTransform");
        Debug.Log("   - For sliding: Assign leftDoor and/or rightDoor transforms");
        Debug.Log("5. Create Keys with Key script and matching key IDs");
        Debug.Log("6. Create Lockers with Locker script");
        Debug.Log("7. Create GameManager with GameManager and NoiseManager scripts");
        Debug.Log("8. Create InteractionUI with InteractionUI script");
        Debug.Log("   - Create Canvas with Panel and Text for UI prompts");
        Debug.Log("   - Assign Panel and Text to InteractionUI component");
        Debug.Log("9. Bake NavMesh for level");
        Debug.Log("See SETUP_GUIDE.md and DOOR_INTERACTION_GUIDE.md for detailed instructions");
    }
    
    // Example key-door configuration
    public struct KeyDoorPair
    {
        public string keyID;
        public Vector3 keyPosition;
        public Vector3 doorPosition;
        public Door.DoorType doorType;
        
        public KeyDoorPair(string id, Vector3 keyPos, Vector3 doorPos, Door.DoorType type = Door.DoorType.Rotating)
        {
            keyID = id;
            keyPosition = keyPos;
            doorPosition = doorPos;
            doorType = type;
        }
    }
    
    // Example patrol configuration
    public struct PatrolConfiguration
    {
        public Vector3[] patrolPoints;
        public float patrolSpeed;
        public float chaseSpeed;
        public float detectionRange;
        
        public PatrolConfiguration(Vector3[] points, float patrol, float chase, float detection)
        {
            patrolPoints = points;
            patrolSpeed = patrol;
            chaseSpeed = chase;
            detectionRange = detection;
        }
    }
    
    // Example level configuration with different door types
    public static KeyDoorPair[] GetExampleKeyDoorPairs()
    {
        return new KeyDoorPair[]
        {
            new KeyDoorPair("Key_1", new Vector3(5, 0, 5), new Vector3(10, 0, 0), Door.DoorType.Rotating),
            new KeyDoorPair("Key_2", new Vector3(15, 0, 5), new Vector3(20, 0, 0), Door.DoorType.SlidingSingle),
            new KeyDoorPair("Key_3", new Vector3(25, 0, 5), new Vector3(30, 0, 0), Door.DoorType.SlidingDouble)
        };
    }
    
    public static PatrolConfiguration GetExamplePatrolConfig()
    {
        Vector3[] points = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(10, 0, 10),
            new Vector3(20, 0, 0),
            new Vector3(10, 0, -10)
        };
        
        return new PatrolConfiguration(points, 3f, 6f, 15f);
    }
}
