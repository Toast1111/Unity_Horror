using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    
    [Header("Noise Generation")]
    public float walkNoiseRadius = 5f;
    public float sprintNoiseRadius = 15f;
    public float crouchNoiseRadius = 2f;
    
    [Header("Interaction")]
    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    
    private CharacterController characterController;
    private List<Key> inventory = new List<Key>();
    private bool isHiding = false;
    private Locker currentLocker = null;
    private bool isCrouching = false;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("PlayerController requires a CharacterController component!");
        }
    }
    
    void Update()
    {
        if (isHiding)
        {
            HandleHidingInput();
            return;
        }
        
        HandleMovement();
        HandleInteraction();
        HandleCrouch();
    }
    
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        
        bool isSprinting = Input.GetKey(sprintKey) && !isCrouching;
        float currentSpeed = isSprinting ? sprintSpeed : (isCrouching ? crouchSpeed : moveSpeed);
        
        if (characterController != null)
        {
            characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
            
            // Apply gravity
            if (!characterController.isGrounded)
            {
                characterController.Move(Vector3.down * 9.81f * Time.deltaTime);
            }
        }
        
        // Generate noise if moving
        if (moveDirection.magnitude > 0.1f)
        {
            float noiseRadius = isSprinting ? sprintNoiseRadius : (isCrouching ? crouchNoiseRadius : walkNoiseRadius);
            NoiseManager.Instance?.GenerateNoise(transform.position, noiseRadius);
        }
    }
    
    void HandleCrouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = !isCrouching;
        }
    }
    
    void HandleInteraction()
    {
        if (Input.GetKeyDown(interactKey))
        {
            // Check for interactable objects
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
            
            foreach (Collider hitCollider in hitColliders)
            {
                // Try to interact with door
                Door door = hitCollider.GetComponent<Door>();
                if (door != null)
                {
                    TryOpenDoor(door);
                    continue;
                }
                
                // Try to pick up key
                Key key = hitCollider.GetComponent<Key>();
                if (key != null)
                {
                    PickUpKey(key);
                    continue;
                }
                
                // Try to hide in locker
                Locker locker = hitCollider.GetComponent<Locker>();
                if (locker != null && !locker.IsOccupied())
                {
                    EnterLocker(locker);
                    continue;
                }
            }
        }
    }
    
    void HandleHidingInput()
    {
        if (Input.GetKeyDown(interactKey))
        {
            ExitLocker();
        }
    }
    
    void TryOpenDoor(Door door)
    {
        if (door.IsLocked())
        {
            // Check if player has the correct key
            Key requiredKey = FindKeyForDoor(door);
            if (requiredKey != null)
            {
                door.Unlock();
                inventory.Remove(requiredKey);
                Debug.Log($"Door unlocked with {requiredKey.keyID}");
            }
            else
            {
                Debug.Log("You need a key to unlock this door!");
            }
        }
        else
        {
            door.Open();
            NoiseManager.Instance?.GenerateNoise(door.transform.position, 10f);
        }
    }
    
    Key FindKeyForDoor(Door door)
    {
        foreach (Key key in inventory)
        {
            if (key.keyID == door.requiredKeyID)
            {
                return key;
            }
        }
        return null;
    }
    
    void PickUpKey(Key key)
    {
        inventory.Add(key);
        key.PickUp();
        Debug.Log($"Picked up key: {key.keyID}");
    }
    
    void EnterLocker(Locker locker)
    {
        isHiding = true;
        currentLocker = locker;
        locker.Hide(this);
        gameObject.SetActive(false); // Hide player
        Debug.Log("Hiding in locker");
    }
    
    void ExitLocker()
    {
        if (currentLocker != null)
        {
            isHiding = false;
            currentLocker.Exit();
            currentLocker = null;
            gameObject.SetActive(true); // Show player
            Debug.Log("Exited locker");
        }
    }
    
    public bool IsHiding()
    {
        return isHiding;
    }
    
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    
    public int GetKeyCount()
    {
        return inventory.Count;
    }
}
