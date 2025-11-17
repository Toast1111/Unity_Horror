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
        Searching
    }
    
    public AIState currentState = AIState.Patrolling;
    
    [Header("Detection Settings")]
    public float detectionRange = 15f;
    public float fieldOfView = 120f;
    public LayerMask obstacleMask;
    public float memoryDuration = 5f;
    
    [Header("Movement Settings")]
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;
    public float investigateSpeed = 4f;
    
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
        if (CanSeePlayer())
        {
            currentState = AIState.Chasing;
            lastKnownPlayerPosition = player.position;
            playerMemoryTimer = memoryDuration;
            return;
        }
        
        // Update memory timer
        if (playerMemoryTimer > 0)
        {
            playerMemoryTimer -= Time.deltaTime;
            
            if (playerMemoryTimer <= 0)
            {
                if (currentState == AIState.Chasing)
                {
                    currentState = AIState.Searching;
                    investigationTimer = investigationTime;
                }
            }
        }
    }
    
    bool CanSeePlayer()
    {
        if (player == null) return false;
        
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Check if player is in range
        if (distanceToPlayer > detectionRange)
            return false;
        
        // Check if player is in field of view
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > fieldOfView / 2f)
            return false;
        
        // Check if there's a clear line of sight
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, distanceToPlayer, obstacleMask))
        {
            if (hit.transform != player)
                return false;
        }
        
        return true;
    }
    
    void Patrol()
    {
        navAgent.speed = patrolSpeed;
        
        if (patrolPoints.Count == 0) return;
        
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
        navAgent.speed = chaseSpeed;
        
        if (player != null)
        {
            navAgent.SetDestination(player.position);
            lastKnownPlayerPosition = player.position;
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
    
    public void HearNoise(Vector3 noisePosition)
    {
        // Add to hot zones for dynamic patrolling
        UpdateHotZone(noisePosition);
        
        // If not chasing, investigate the noise
        if (currentState != AIState.Chasing)
        {
            currentState = AIState.Investigating;
            investigationPoint = noisePosition;
            investigationTimer = investigationTime;
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
            currentState = AIState.Investigating;
            investigationPoint = doorPosition;
            investigationTimer = investigationTime;
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
