using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// JimmyAI - Advanced enemy AI for Unity Horror game
/// Compatible with Unity 2022.3+
/// Integrates with: PlayerController, NoiseManager, GameManager, Door, HidingSpot, Locker
/// </summary>
public class JimmyAI : MonoBehaviour
{
    public enum AIState
    {
        Patrolling,
        Investigating,
        Chasing,
        Searching,
        Distracted
    }

    [Header("AI State")]
    [SerializeField] private AIState currentState = AIState.Patrolling;

    [Header("Movement Settings")]
    [SerializeField] private float patrolSpeed = 3.5f;
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float runSpeed = 6f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private float eyeHeight = 1.5f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask targetLayers;
    
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waypointWaitTime = 2f;
    [SerializeField] private float patrolBoredomTime = 60f;

    [Header("Investigation Settings")]
    [SerializeField] private float searchTime = 5f;
    [SerializeField] private float investigationRadius = 10f;

    [Header("Chase Settings")]
    [SerializeField] private float closeRangeDistance = 4f;
    [SerializeField] private float mediumRangeDistance = 20f;
    [SerializeField] private float boredomTimeThreshold = 15f;
    
    [Header("Hiding Spot Settings")]
    [SerializeField] private float hidingSpotCheckDistance = 5f;
    
    [Header("Door Interaction Settings")]
    [SerializeField] private float doorDetectionRange = 2f;
    [SerializeField] private float doorOpenCheckInterval = 0.5f;

    // Components
    private NavMeshAgent navAgent;
    private Transform player;
    private PlayerController playerController;

    // State tracking
    private int currentPatrolIndex = 0;
    private Vector3 lastKnownPlayerPosition;
    private float stateTimer = 0f;
    private float lastSeenTimer = 0f;
    private float boredomTimer = 0f;
    private bool goToLastSeen = false;
    private HidingSpot currentHidingSpot = null;
    private float doorCheckTimer = 0f;

    // Hot zones for dynamic patrol
    private List<Vector3> hotZones = new List<Vector3>();
    private const int MAX_HOT_ZONES = 10;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            Debug.LogError($"JimmyAI on {gameObject.name} requires a NavMeshAgent component!");
        }
    }

    private void Start()
    {
        // Find player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerController = playerObject.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogError("JimmyAI: Player not found! Make sure player has 'Player' tag.");
        }

        // Initialize patrol
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            navAgent.speed = patrolSpeed;
            if (navAgent.isOnNavMesh)
            {
                navAgent.SetDestination(patrolPoints[0].position);
            }
        }
        else
        {
            Debug.LogWarning($"JimmyAI on {gameObject.name}: No patrol points assigned!");
        }
    }

    private void Update()
    {
        if (player == null || navAgent == null || !navAgent.isOnNavMesh) return;

        UpdateState();
        ExecuteState();
        CheckAndOpenDoors();
    }

    private void UpdateState()
    {
        // Don't detect hidden players
        if (playerController != null && playerController.IsHiding())
        {
            if (currentState == AIState.Chasing)
            {
                currentState = AIState.Searching;
                stateTimer = 0f;
            }
            return;
        }

        // Check if player is visible
        bool canSeePlayer = CanSeePlayer();

        // Check proximity for sound detection
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool isPlayerMovingFast = IsPlayerMovingFast();

        // State transition logic
        switch (currentState)
        {
            case AIState.Patrolling:
                lastSeenTimer += Time.deltaTime;
                
                // Periodic player check (boredom timer)
                if (lastSeenTimer > patrolBoredomTime)
                {
                    lastKnownPlayerPosition = player.position;
                    AddHotZone(player.position);
                    currentState = AIState.Distracted;
                    goToLastSeen = false;
                    lastSeenTimer = 0f;
                }
                // Detect player
                else if (canSeePlayer || (isPlayerMovingFast && distanceToPlayer < 5f))
                {
                    OnPlayerDetected();
                }
                break;

            case AIState.Investigating:
                if (canSeePlayer || (isPlayerMovingFast && distanceToPlayer < 5f))
                {
                    OnPlayerDetected();
                }
                break;

            case AIState.Chasing:
                if (canSeePlayer)
                {
                    lastKnownPlayerPosition = player.position;
                    boredomTimer = 0f;
                }
                else
                {
                    currentState = AIState.Distracted;
                    goToLastSeen = true;
                    stateTimer = 0f;
                }
                break;

            case AIState.Distracted:
                if (canSeePlayer)
                {
                    OnPlayerDetected();
                }
                break;

            case AIState.Searching:
                if (canSeePlayer)
                {
                    OnPlayerDetected();
                }
                break;
        }
    }

    private void ExecuteState()
    {
        switch (currentState)
        {
            case AIState.Patrolling:
                Patrol();
                break;
            case AIState.Investigating:
                Investigate();
                break;
            case AIState.Chasing:
                ChasePlayer();
                break;
            case AIState.Searching:
                SearchForPlayer();
                break;
            case AIState.Distracted:
                HandleDistraction();
                break;
        }
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        navAgent.speed = patrolSpeed;

        // Check if we've reached the current patrol point
        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            stateTimer += Time.deltaTime;

            if (stateTimer >= waypointWaitTime)
            {
                stateTimer = 0f;
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                
                // Prefer hot zones if available
                Vector3 targetPoint = GetNextPatrolPoint();
                navAgent.SetDestination(targetPoint);
            }
        }
    }

    private Vector3 GetNextPatrolPoint()
    {
        // 50% chance to visit a hot zone if available
        if (hotZones.Count > 0 && Random.value > 0.5f)
        {
            Vector3 hotZone = hotZones[Random.Range(0, hotZones.Count)];
            return hotZone;
        }

        return patrolPoints[currentPatrolIndex].position;
    }

    private void Investigate()
    {
        navAgent.speed = runSpeed;

        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            stateTimer += Time.deltaTime;

            if (stateTimer >= searchTime)
            {
                stateTimer = 0f;
                currentState = AIState.Patrolling;
            }
        }
    }

    private void ChasePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Distance-based chase behavior
        if (distance <= closeRangeDistance)
        {
            // Close range: Run at full speed
            navAgent.speed = runSpeed;
            navAgent.SetDestination(player.position);
        }
        else if (distance <= mediumRangeDistance)
        {
            // Medium range: Variable behavior
            if (boredomTimer >= boredomTimeThreshold)
            {
                navAgent.speed = runSpeed;
            }
            else
            {
                navAgent.speed = walkSpeed;
                boredomTimer += Time.deltaTime;
            }

            // Try to use hiding spot for tactical advantage
            TryUseHidingSpot();

            navAgent.SetDestination(player.position);
        }
        else
        {
            // Far range: Run to catch up
            navAgent.speed = runSpeed;
            navAgent.SetDestination(player.position);
            boredomTimer = 0f;
        }

        // Check if we caught the player
        if (distance < 1f)
        {
            CatchPlayer();
        }
    }

    private void SearchForPlayer()
    {
        navAgent.speed = walkSpeed;

        stateTimer += Time.deltaTime;

        if (stateTimer >= searchTime)
        {
            stateTimer = 0f;
            currentState = AIState.Patrolling;
        }
    }

    private void HandleDistraction()
    {
        navAgent.speed = runSpeed;

        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            stateTimer += Time.deltaTime;

            // Wait longer if we just lost sight of player
            float waitTime = goToLastSeen ? searchTime * 3f : searchTime;

            if (stateTimer >= waitTime)
            {
                stateTimer = 0f;
                goToLastSeen = false;
                currentState = AIState.Patrolling;
            }
        }
        else
        {
            navAgent.SetDestination(lastKnownPlayerPosition);
        }
    }

    private void TryUseHidingSpot()
    {
        if (currentHidingSpot == null)
        {
            currentHidingSpot = HidingSpot.GetClosest(player.position, hidingSpotCheckDistance, true);
        }

        if (currentHidingSpot != null)
        {
            float distanceToSpot = Vector3.Distance(transform.position, currentHidingSpot.transform.position);
            
            if (distanceToSpot < 1f)
            {
                // Reached hiding spot - peek at player
                transform.position = Vector3.Lerp(transform.position, currentHidingSpot.transform.position, Time.deltaTime * 5f);
                transform.rotation = Quaternion.Lerp(transform.rotation, currentHidingSpot.transform.rotation, Time.deltaTime * 5f);
                
                boredomTimer += Time.deltaTime;
                
                // Give up hiding spot if bored or player moved far
                if (boredomTimer >= boredomTimeThreshold || Vector3.Distance(player.position, currentHidingSpot.transform.position) > mediumRangeDistance)
                {
                    currentHidingSpot = null;
                    boredomTimer = 0f;
                }
            }
            else
            {
                // Moving to hiding spot
                navAgent.SetDestination(currentHidingSpot.transform.position);
            }
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 eyePosition = transform.position + Vector3.up * eyeHeight;
        Vector3 playerHeadPosition = player.position + Vector3.up * eyeHeight;
        Vector3 directionToPlayer = playerHeadPosition - eyePosition;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Check distance
        if (distanceToPlayer > detectionRange) return false;

        // Check field of view
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > fieldOfView / 2f) return false;

        // Check line of sight with combined mask
        LayerMask combinedMask = obstacleMask | targetLayers;
        if (Physics.Linecast(eyePosition, playerHeadPosition, out RaycastHit hit, combinedMask))
        {
            return hit.collider.CompareTag("Player");
        }

        return true;
    }

    private bool IsPlayerMovingFast()
    {
        if (playerController == null) return false;
        
        // Check if player is sprinting and not hiding
        return !playerController.IsHiding() && player.GetComponent<CharacterController>() != null &&
               player.GetComponent<CharacterController>().velocity.magnitude > 3f;
    }

    private void OnPlayerDetected()
    {
        currentState = AIState.Chasing;
        lastKnownPlayerPosition = player.position;
        AddHotZone(player.position);
        lastSeenTimer = 0f;
        boredomTimer = 0f;
        currentHidingSpot = null;
    }

    private void CatchPlayer()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogWarning("JimmyAI: GameManager not found! Cannot trigger game over.");
        }
    }
    
    /// <summary>
    /// Check for doors in front of Jimmy and open them if they're closed but unlocked
    /// </summary>
    private void CheckAndOpenDoors()
    {
        doorCheckTimer += Time.deltaTime;
        
        // Only check periodically to save performance
        if (doorCheckTimer < doorOpenCheckInterval)
        {
            return;
        }
        
        doorCheckTimer = 0f;
        
        // Check if there's a door blocking Jimmy's path
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, doorDetectionRange);
        
        foreach (Collider col in nearbyColliders)
        {
            Door door = col.GetComponent<Door>();
            
            if (door != null && !door.IsLocked() && !door.IsOpen())
            {
                // Check if door is in front of Jimmy (within forward direction)
                Vector3 directionToDoor = door.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, directionToDoor);
                
                // If door is roughly in front of Jimmy (within 90 degrees)
                if (angle < 90f)
                {
                    // Check if Jimmy is moving towards the door
                    if (navAgent.hasPath && navAgent.remainingDistance > doorDetectionRange)
                    {
                        door.Open();
                        Debug.Log($"Jimmy opened door: {door.gameObject.name}");
                    }
                }
            }
        }
    }

    private void AddHotZone(Vector3 position)
    {
        // Add to front of list
        hotZones.Insert(0, position);

        // Limit list size
        if (hotZones.Count > MAX_HOT_ZONES)
        {
            hotZones.RemoveAt(hotZones.Count - 1);
        }
    }

    /// <summary>
    /// Called by NoiseManager when player makes noise
    /// </summary>
    public void HearNoise(Vector3 noisePosition)
    {
        if (currentState == AIState.Chasing) return; // Already chasing, ignore noise

        lastKnownPlayerPosition = noisePosition;
        AddHotZone(noisePosition);

        if (currentState != AIState.Distracted)
        {
            currentState = AIState.Investigating;
            stateTimer = 0f;
            navAgent.SetDestination(noisePosition);
        }
    }

    /// <summary>
    /// Called by Door when opened
    /// </summary>
    public void NotifyDoorOpened(Vector3 doorPosition)
    {
        if (currentState == AIState.Chasing) return; // Already chasing, ignore door

        lastKnownPlayerPosition = doorPosition;
        AddHotZone(doorPosition);

        currentState = AIState.Investigating;
        stateTimer = 0f;
        navAgent.SetDestination(doorPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if player is hiding
            if (playerController != null && playerController.IsHiding()) return;

            CatchPlayer();
        }
    }

    // Public getters for debugging
    public AIState GetCurrentState() => currentState;
    public Vector3 GetLastKnownPlayerPosition() => lastKnownPlayerPosition;
    public int GetHotZoneCount() => hotZones.Count;

    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position + Vector3.up * eyeHeight, detectionRange);

        // Draw field of view
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward * detectionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2f, 0) * transform.forward * detectionRange;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(eyePos, eyePos + leftBoundary);
        Gizmos.DrawLine(eyePos, eyePos + rightBoundary);

        // Draw hot zones
        Gizmos.color = Color.red;
        foreach (Vector3 hotZone in hotZones)
        {
            Gizmos.DrawWireSphere(hotZone, 1f);
        }

        // Draw patrol points
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawWireSphere(patrolPoints[i].position, 0.5f);
                    
                    // Draw line to next patrol point
                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    }
                    else if (i == patrolPoints.Length - 1 && patrolPoints[0] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
                    }
                }
            }
        }
    }
}
