using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A hiding spot that JimmyAI can use to peek at the player from cover.
/// Attach this component to empty GameObjects positioned at corners, doorways, or behind obstacles.
/// </summary>
public class HidingSpot : MonoBehaviour
{
    [Header("Hiding Spot Type")]
    [Tooltip("Is this hiding spot on the left or right side? Used for peek direction.")]
    public bool isLeftSide = false;
    
    [Header("Animation Settings")]
    [Tooltip("Animation clip to play when Jimmy uses this hiding spot (optional). Leave empty if no specific animation needed.")]
    public AnimationClip peekAnimation;
    
    [Tooltip("Animation name/trigger to play when Jimmy uses this hiding spot (optional). Used with Animator component.")]
    public string animationName = "";
    
    [Header("Runtime Info (Read Only)")]
    [Tooltip("Distance from this spot to Jimmy AI - updated automatically")]
    public float distanceToJimmy;
    
    // Static registry of all hiding spots in the scene
    private static List<HidingSpot> allHidingSpots = new List<HidingSpot>();
    
    // Reference to Jimmy for distance calculation
    private static JimmyAI jimmyInstance;
    
    void OnEnable()
    {
        // Register this hiding spot
        if (!allHidingSpots.Contains(this))
        {
            allHidingSpots.Add(this);
        }
    }
    
    void OnDisable()
    {
        // Unregister this hiding spot
        allHidingSpots.Remove(this);
    }
    
    void Update()
    {
        // Update distance to Jimmy if he exists
        if (jimmyInstance == null)
        {
            // Try to find Jimmy in the scene
            jimmyInstance = FindObjectOfType<JimmyAI>();
        }
        
        if (jimmyInstance != null)
        {
            distanceToJimmy = Vector3.Distance(transform.position, jimmyInstance.transform.position);
        }
    }
    
    /// <summary>
    /// Find the closest hiding spot to a target position that meets the criteria.
    /// </summary>
    /// <param name="fromPosition">Position to search from (usually player position)</param>
    /// <param name="maxDistance">Maximum distance to consider</param>
    /// <param name="mustBeInFront">If true, only return spots in front of the player (player not behind spot)</param>
    /// <returns>Closest hiding spot, or null if none found</returns>
    public static HidingSpot GetClosest(Vector3 fromPosition, float maxDistance = 5f, bool mustBeInFront = true)
    {
        HidingSpot closest = null;
        float closestDistance = float.MaxValue;
        
        foreach (HidingSpot spot in allHidingSpots)
        {
            if (spot == null) continue;
            
            float distance = spot.distanceToJimmy;
            
            // Check if within max distance
            if (distance > maxDistance)
                continue;
            
            // Check if player is behind this spot (we want spots between player and Jimmy)
            if (mustBeInFront)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    Vector3 spotToPlayer = playerObj.transform.position - spot.transform.position;
                    Vector3 spotForward = spot.transform.forward;
                    
                    // If player is behind the spot, skip it
                    if (Vector3.Dot(spotForward, spotToPlayer) < 0)
                        continue;
                }
            }
            
            // Track closest spot
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = spot;
            }
        }
        
        return closest;
    }
    
    /// <summary>
    /// Get all registered hiding spots in the scene.
    /// </summary>
    public static List<HidingSpot> GetAllHidingSpots()
    {
        return new List<HidingSpot>(allHidingSpots);
    }
    
    /// <summary>
    /// Check if this hiding spot has a clear line of sight to a target position.
    /// </summary>
    public bool HasLineOfSightTo(Vector3 targetPosition, LayerMask obstacleMask)
    {
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1.5f, direction.normalized, out hit, distance, obstacleMask))
        {
            // Something is blocking the view
            return false;
        }
        
        return true;
    }
    
    // Visualize hiding spot in editor
    void OnDrawGizmos()
    {
        // Draw a small sphere to show the hiding spot location
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f); // Cyan
        Gizmos.DrawSphere(transform.position + Vector3.up * 0.5f, 0.3f);
        
        // Draw the forward direction (peek direction)
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, 
                       transform.position + Vector3.up * 0.5f + transform.forward * 1.5f);
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw a larger visualization when selected
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, 1f);
        
        // Show peek direction more clearly
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, 
                       transform.position + Vector3.up * 0.5f + transform.forward * 2f);
        Gizmos.DrawSphere(transform.position + Vector3.up * 0.5f + transform.forward * 2f, 0.2f);
    }
}
