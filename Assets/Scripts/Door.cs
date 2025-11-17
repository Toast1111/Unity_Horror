using UnityEngine;

public class Door : MonoBehaviour
{
    public string requiredKeyID = "Key_1";
    public bool isLocked = true;
    public bool isOpen = false;
    
    [Header("Door Animation")]
    public Transform doorTransform;
    public Vector3 openRotation = new Vector3(0, 90, 0);
    public float openSpeed = 2f;
    
    private Quaternion closedRotation;
    private Quaternion targetOpenRotation;
    
    void Start()
    {
        if (doorTransform == null)
        {
            doorTransform = transform;
        }
        closedRotation = doorTransform.localRotation;
        targetOpenRotation = closedRotation * Quaternion.Euler(openRotation);
    }
    
    void Update()
    {
        if (isOpen && doorTransform != null)
        {
            doorTransform.localRotation = Quaternion.Slerp(
                doorTransform.localRotation, 
                targetOpenRotation, 
                Time.deltaTime * openSpeed
            );
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
        if (doorTransform != null)
        {
            doorTransform.localRotation = closedRotation;
        }
    }
    
    public bool IsLocked()
    {
        return isLocked;
    }
    
    public bool IsOpen()
    {
        return isOpen;
    }
}
