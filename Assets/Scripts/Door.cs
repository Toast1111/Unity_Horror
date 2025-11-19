using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        Rotating,
        SlidingSingle,
        SlidingDouble
    }
    
    public string requiredKeyID = "Key_1";
    public bool isLocked = true;
    public bool isOpen = false;
    
    [Header("Door Type")]
    public DoorType doorType = DoorType.Rotating;
    
    [Header("Rotating Door Settings")]
    public Transform doorTransform;
    public Vector3 openRotation = new Vector3(0, 90, 0);
    public float openSpeed = 2f;
    
    [Header("Sliding Door Settings")]
    public Transform leftDoor;
    public Transform rightDoor;
    public Vector3 slideDirection = Vector3.right;
    public float slideDistance = 1f;
    public float slideSpeed = 2f;
    
    private Quaternion closedRotation;
    private Quaternion targetOpenRotation;
    private Vector3 leftDoorClosedPosition;
    private Vector3 rightDoorClosedPosition;
    private Vector3 leftDoorOpenPosition;
    private Vector3 rightDoorOpenPosition;
    
    void Start()
    {
        if (doorType == DoorType.Rotating)
        {
            if (doorTransform == null)
            {
                doorTransform = transform;
            }
            closedRotation = doorTransform.localRotation;
            targetOpenRotation = closedRotation * Quaternion.Euler(openRotation);
        }
        else if (doorType == DoorType.SlidingSingle)
        {
            if (leftDoor != null)
            {
                leftDoorClosedPosition = leftDoor.localPosition;
                leftDoorOpenPosition = leftDoorClosedPosition + slideDirection * slideDistance;
            }
        }
        else if (doorType == DoorType.SlidingDouble)
        {
            if (leftDoor != null)
            {
                leftDoorClosedPosition = leftDoor.localPosition;
                leftDoorOpenPosition = leftDoorClosedPosition + slideDirection * slideDistance;
            }
            if (rightDoor != null)
            {
                rightDoorClosedPosition = rightDoor.localPosition;
                rightDoorOpenPosition = rightDoorClosedPosition - slideDirection * slideDistance;
            }
        }
    }
    
    void Update()
    {
        if (doorType == DoorType.Rotating && doorTransform != null)
        {
            Quaternion targetRotation = isOpen ? targetOpenRotation : closedRotation;
            doorTransform.localRotation = Quaternion.Slerp(
                doorTransform.localRotation, 
                targetRotation, 
                Time.deltaTime * openSpeed
            );
        }
        else if (doorType == DoorType.SlidingSingle && leftDoor != null)
        {
            Vector3 targetPosition = isOpen ? leftDoorOpenPosition : leftDoorClosedPosition;
            leftDoor.localPosition = Vector3.Lerp(
                leftDoor.localPosition,
                targetPosition,
                Time.deltaTime * slideSpeed
            );
        }
        else if (doorType == DoorType.SlidingDouble)
        {
            if (leftDoor != null)
            {
                Vector3 leftTarget = isOpen ? leftDoorOpenPosition : leftDoorClosedPosition;
                leftDoor.localPosition = Vector3.Lerp(
                    leftDoor.localPosition,
                    leftTarget,
                    Time.deltaTime * slideSpeed
                );
            }
            if (rightDoor != null)
            {
                Vector3 rightTarget = isOpen ? rightDoorOpenPosition : rightDoorClosedPosition;
                rightDoor.localPosition = Vector3.Lerp(
                    rightDoor.localPosition,
                    rightTarget,
                    Time.deltaTime * slideSpeed
                );
            }
        }
    }
    
    public void Unlock()
    {
        isLocked = false;
        Open();
        Debug.Log($"Door unlocked: {gameObject.name}");
    }
    
    public void Open()
    {
        if (!isLocked)
        {
            isOpen = true;
            Debug.Log($"Door opened: {gameObject.name}");
            
            // Notify all Jimmy AI about door opening
            NotifyDoorOpened();
        }
    }
    
    void NotifyDoorOpened()
    {
        JimmyAI[] enemies = FindObjectsOfType<JimmyAI>();
        foreach (JimmyAI enemy in enemies)
        {
            enemy.NotifyDoorOpened(transform.position);
        }
    }
    
    public void Close()
    {
        isOpen = false;
        Debug.Log($"Door closed: {gameObject.name}");
    }
    
    public bool IsLocked()
    {
        return isLocked;
    }
    
    public bool IsOpen()
    {
        return isOpen;
    }
    
    public void Toggle()
    {
        if (!isLocked)
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }
}
