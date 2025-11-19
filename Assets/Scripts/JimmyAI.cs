using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class JimmyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    
    [Header("AI States")]
    public enum AIState
    {
        Patrolling,
        Investigating,
        Chasing,
        Searching,
        Distracted,
        Hiding
    }
    
    public AIState currentState = AIState.Patrolling;
    
    [Header("Detection Settings")]
    public float detectionRange = 20f;
    public float fieldOfView = 90f;
    public LayerMask obstacleMask;
    public float memoryDuration = 5f;
    public LayerMask targetLayers;
    
    [Header("Movement Settings")]
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;
    public float investigateSpeed = 4f;
    public float walkSpeed = 3.5f;
    public float runSpeed = 6f;
    
    [Header("Patrol Settings")]
    public List<Transform> patrolPoints = new List<Transform>();
    public float waypointWaitTime = 2f;
    public float hotZoneRadius = 10f;
    
    [Header("Investigation Settings")]
    public float investigationRadius = 3f;
    public float investigationTime = 5f;
    
    private NavMeshAgent navAgent;
    private int currentPatrolIndex = 0;
    private float waypointWaitTimer = 0f;
    private Vector3 lastKnownPlayerPosition;
    private float playerMemoryTimer = 0f;
    private Vector3 investigationPoint;
    private float investigationTimer = 0f;
    private List<Vector3> hotZones = new List<Vector3>();
    private PlayerController playerController;
    private float boredomTimer = 0f;
    private float lastSeenTimer = 0f;
    private bool goToLastSeen = false;
    private HidingSpot currentHidingSpot = null;
    
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            Debug.LogError("JimmyAI requires a NavMeshAgent component!");
            return;
        }
        
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerController = playerObj.GetComponent<PlayerController>();
        }
        
        // Initialize patrol
        if (patrolPoints.Count > 0)
        {
            GoToNextPatrolPoint();
        }
        
        // Initialize hot zones around patrol points
        InitializeHotZones();
    }
    
    void Update()
    {
        if (navAgent == null || player == null) return;
        
        UpdateState();
        
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
                Distracted();
                break;
            case AIState.Hiding:
                HideAndPeek();
                break;
        }
    }
    
    void InitializeHotZones()
    {
        hotZones.Clear();
        foreach (Transform point in patrolPoints)
        {
            hotZones.Add(point.position);
        }
    }
    
    void UpdateState()
    {
        // Check if player is hiding
        if (playerController != null && playerController.IsHiding())
        {
            // Can't detect hidden player
            if (currentState == AIState.Chasing)
            {
                currentState = AIState.Searching;
                investigationTimer = investigationTime;
            }
            return;
        }
        
        // Check for direct line of sight to player
        bool canSeePlayer = CanSeePlayer();
        
        if (canSeePlayer)
        {
            currentState = AIState.Chasing;
            lastKnownPlayerPosition = player.position;
            playerMemoryTimer = memoryDuration;
            lastSeenTimer = 0f;
            return;
        }
        
        // Enhanced detection: hear nearby movement even without direct sight
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool isPlayerMovingFast = playerController != null && !playerController.IsHiding();
        
        // If player is close and moving, can hear them
        if (isPlayerMovingFast && distanceToPlayer < 5f)
        {
            // Player is very close and not hiding
            if (currentState != AIState.Chasing && currentState != AIState.Distracted)
            {
                currentState = AIState.Investigating;
                investigationPoint = player.position;
                investigationTimer = investigationTime;
            }
        }
        
        // Update memory timer
        if (playerMemoryTimer > 0)
        {
            playerMemoryTimer -= Time.deltaTime;
            
            if (playerMemoryTimer <= 0)
            {
                if (currentState == AIState.Chasing)
                {
                    currentState = AIState.Distracted;
                    goToLastSeen = true;
                }
            }
        }
        
        // If in chase but lost player, transition to distracted
        if (currentState == AIState.Chasing && !canSeePlayer)
        {
            currentState = AIState.Distracted;
            goToLastSeen = true;
        }
    }
    
    bool CanSeePlayer()
    {
        if (player == null) return false;
        
        Vector3 playerHeadPosition = player.position + Vector3.up * 1.5f; // Approximate head height
        Vector3 eyePosition = transform.position + Vector3.up * 1.5f; // Jimmy's eye height
        
        Vector3 directionToPlayer = (playerHeadPosition - eyePosition).normalized;
        float distanceToPlayer = Vector3.Distance(eyePosition, playerHeadPosition);
        
        // Check if player is in range
        if (distanceToPlayer > detectionRange)
            return false;
        
        // Check if player is in field of view
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > fieldOfView / 2f)
            return false;
        
        // Check if there's a clear line of sight using both target layers and obstacle mask
        RaycastHit hit;
        LayerMask combinedMask = targetLayers.value != 0 ? targetLayers : obstacleMask;
        
        if (Physics.Linecast(eyePosition, playerHeadPosition, out hit, combinedMask))
        {
            // Check if the hit object is the player
            if (hit.transform != player && !hit.transform.IsChildOf(player))
                return false;
        }
        
        return true;
    }
    
    void Patrol()
    {
        navAgent.speed = patrolSpeed;
        
        if (patrolPoints.Count == 0) return;
        
        // Increment timers
        lastSeenTimer += Time.deltaTime;
        
        // Boredom timer - periodically check player's location (like YandereScript)
        if (lastSeenTimer > 60f)
        {
            lastKnownPlayerPosition = player.position;
            currentState = AIState.Distracted;
            lastSeenTimer = 0f;
            return;
        }
        
        // Check if reached waypoint
        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            waypointWaitTimer += Time.deltaTime;
            
            if (waypointWaitTimer >= waypointWaitTime)
            {
                GoToNextPatrolPoint();
                waypointWaitTimer = 0f;
            }
        }
    }
    
    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Count == 0) return;
        
        // Choose next patrol point dynamically based on hot zones
        currentPatrolIndex = GetNextDynamicPatrolPoint();
        
        if (currentPatrolIndex < patrolPoints.Count)
        {
            navAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }
    
    int GetNextDynamicPatrolPoint()
    {
        // Favor hot zones (areas with recent activity)
        float highestPriority = -1f;
        int bestIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        
        for (int i = 0; i < patrolPoints.Count; i++)
        {
            if (i == currentPatrolIndex) continue;
            
            float priority = 0f;
            
            // Check if this point is near a hot zone
            foreach (Vector3 hotZone in hotZones)
            {
                float distance = Vector3.Distance(patrolPoints[i].position, hotZone);
                if (distance < hotZoneRadius)
                {
                    priority += 1f / (distance + 1f);
                }
            }
            
            if (priority > highestPriority)
            {
                highestPriority = priority;
                bestIndex = i;
            }
        }
        
        return bestIndex;
    }
    
    void ChasePlayer()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Close range - full speed chase (0-4 units)
        if (distanceToPlayer <= 4f)
        {
            navAgent.speed = runSpeed;
            navAgent.SetDestination(player.position);
            lastKnownPlayerPosition = player.position;
            boredomTimer = 0f;
            
            // Clear hiding spot if too close
            if (currentHidingSpot != null)
            {
                currentHidingSpot = null;
            }
        }
        // Medium range - walk/run based on boredom (4-5 units)
        else if (distanceToPlayer <= 5f)
        {
            navAgent.speed = walkSpeed;
            navAgent.SetDestination(player.position);
            lastKnownPlayerPosition = player.position;
            boredomTimer = 0f;
            
            // Clear hiding spot if too close
            if (currentHidingSpot != null)
            {
                currentHidingSpot = null;
            }
        }
        // Medium-far range - search behavior with hiding spot option (5-20 units)
        else if (distanceToPlayer <= 20f)
        {
            // If we don't have a hiding spot and can see player, try to find one
            if (currentHidingSpot == null && CanSeePlayer())
            {
                HidingSpot closest = HidingSpot.GetClosest(player.position, 5f, true);
                if (closest != null && closest.distanceToJimmy <= 5f)
                {
                    // Found a good hiding spot, transition to Hiding state
                    currentHidingSpot = closest;
                    currentState = AIState.Hiding;
                    return;
                }
            }
            
            // No hiding spot found or not visible, continue normal chase
            navAgent.speed = walkSpeed;
            navAgent.SetDestination(player.position);
            lastKnownPlayerPosition = player.position;
            boredomTimer += Time.deltaTime;
            
            // If bored and player still far, speed up
            if (boredomTimer >= 15f && distanceToPlayer <= 20f)
            {
                navAgent.speed = runSpeed;
            }
        }
        // Far range - run to last known position (20+ units)
        else
        {
            navAgent.speed = runSpeed;
            navAgent.SetDestination(player.position);
            lastKnownPlayerPosition = player.position;
            boredomTimer = 0f;
            
            // Clear hiding spot if too far
            if (currentHidingSpot != null)
            {
                currentHidingSpot = null;
            }
        }
    }
    
    void Investigate()
    {
        navAgent.speed = investigateSpeed;
        
        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            investigationTimer -= Time.deltaTime;
            
            if (investigationTimer <= 0)
            {
                currentState = AIState.Patrolling;
                GoToNextPatrolPoint();
            }
        }
    }
    
    void SearchForPlayer()
    {
        navAgent.speed = investigateSpeed;
        
        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            investigationTimer -= Time.deltaTime;
            
            if (investigationTimer <= 0)
            {
                currentState = AIState.Patrolling;
                GoToNextPatrolPoint();
            }
        }
        else if (investigationTimer > 0)
        {
            // Move to last known position
            navAgent.SetDestination(lastKnownPlayerPosition);
        }
    }
    
    void Distracted()
    {
        navAgent.speed = runSpeed;
        
        // Move to last known player position
        if (Vector3.Distance(transform.position, lastKnownPlayerPosition) > 4f || 
            (goToLastSeen && Vector3.Distance(transform.position, lastKnownPlayerPosition) > 1f))
        {
            if (navAgent.destination != lastKnownPlayerPosition)
            {
                navAgent.SetDestination(lastKnownPlayerPosition);
            }
        }
        else
        {
            // Reached last known position, wait and look around
            investigationTimer += Time.deltaTime;
            
            // Wait at the position for a while before returning to patrol
            if (investigationTimer >= investigationTime * (goToLastSeen ? 3f : 1f))
            {
                currentState = AIState.Patrolling;
                goToLastSeen = false;
                investigationTimer = 0f;
            }
        }
        
        boredomTimer = 0f;
    }
    
    void HideAndPeek()
    {
        if (currentHidingSpot == null || player == null)
        {
            // No hiding spot, return to chase
            currentState = AIState.Chasing;
            return;
        }
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float distanceToSpot = Vector3.Distance(transform.position, currentHidingSpot.transform.position);
        
        // Check if we should abandon the hiding spot
        if (distanceToPlayer < 5f)
        {
            // Player too close, chase them directly
            currentHidingSpot = null;
            currentState = AIState.Chasing;
            return;
        }
        
        if (distanceToPlayer > 25f)
        {
            // Player too far, abandon hiding spot
            currentHidingSpot = null;
            currentState = AIState.Chasing;
            return;
        }
        
        // Check if player is hiding
        if (playerController != null && playerController.IsHiding())
        {
            // Player is hiding, abandon hiding spot and search
            currentHidingSpot = null;
            currentState = AIState.Searching;
            investigationTimer = investigationTime;
            return;
        }
        
        // Move to hiding spot if not there yet
        if (distanceToSpot > 1f)
        {
            navAgent.speed = runSpeed;
            navAgent.SetDestination(currentHidingSpot.transform.position);
            boredomTimer = 0f;
        }
        else
        {
            // At hiding spot, stop and peek
            navAgent.speed = 0f;
            
            // Look towards the player
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0; // Keep rotation on horizontal plane
            if (directionToPlayer.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
            
            // Lerp position to exact hiding spot position for peeking
            transform.position = Vector3.Lerp(transform.position, currentHidingSpot.transform.position, Time.deltaTime * 5f);
            
            // Increment boredom timer while hiding
            boredomTimer += Time.deltaTime;
            
            // After watching for a while, decide what to do
            if (boredomTimer >= 10f)
            {
                // Been here too long, chase player directly
                currentHidingSpot = null;
                currentState = AIState.Chasing;
                boredomTimer = 0f;
            }
            else if (!CanSeePlayer())
            {
                // Lost sight of player, leave hiding spot
                currentHidingSpot = null;
                currentState = AIState.Distracted;
                goToLastSeen = true;
            }
        }
    }
    
    public void HearNoise(Vector3 noisePosition)
    {
        // Add to hot zones for dynamic patrolling
        UpdateHotZone(noisePosition);
        
        // If not chasing, investigate the noise
        if (currentState != AIState.Chasing)
        {
            currentState = AIState.Distracted;
            lastKnownPlayerPosition = noisePosition;
            investigationTimer = 0f;
            navAgent.SetDestination(noisePosition);
            Debug.Log("Jimmy heard a noise!");
        }
    }
    
    public void NotifyDoorOpened(Vector3 doorPosition)
    {
        // Add to hot zones
        UpdateHotZone(doorPosition);
        
        // Investigate door opening
        if (currentState == AIState.Patrolling)
        {
            currentState = AIState.Distracted;
            lastKnownPlayerPosition = doorPosition;
            investigationTimer = 0f;
            navAgent.SetDestination(doorPosition);
            Debug.Log("Jimmy noticed a door opened!");
        }
    }
    
    void UpdateHotZone(Vector3 position)
    {
        // Add new hot zone
        hotZones.Add(position);
        
        // Limit hot zones to prevent memory issues
        if (hotZones.Count > 10)
        {
            hotZones.RemoveAt(0);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if caught player
        if (other.CompareTag("Player") && playerController != null && !playerController.IsHiding())
        {
            GameManager.Instance?.GameOver();
            Debug.Log("Jimmy caught the player!");
        }
    }
    
    // Debug visualization
    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw field of view
        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfView / 2f, Vector3.up) * transform.forward * detectionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfView / 2f, Vector3.up) * transform.forward * detectionRange;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);
        
        // Draw hot zones
        Gizmos.color = Color.cyan;
        foreach (Vector3 hotZone in hotZones)
        {
            Gizmos.DrawWireSphere(hotZone, hotZoneRadius);
        }
    }
}
