using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages UI prompts for player interactions
/// Shows context-sensitive prompts for doors, lockers, and keys
/// </summary>
public class InteractionUI : MonoBehaviour
{
    public static InteractionUI Instance { get; private set; }
    
    [Header("UI Elements")]
    public Text promptText;
    public GameObject promptPanel;
    
    [Header("Prompt Messages")]
    public string openDoorPrompt = "Press E to Open Door";
    public string closeDoorPrompt = "Press E to Close Door";
    public string lockedDoorPrompt = "Door is Locked - Need Key";
    public string unlockDoorPrompt = "Press E to Unlock Door";
    public string pickupKeyPrompt = "Press E to Pick Up Key";
    public string enterLockerPrompt = "Press E to Hide in Locker";
    public string exitLockerPrompt = "Press E to Exit Locker";
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        HidePrompt();
    }
    
    /// <summary>
    /// Show a custom prompt message
    /// </summary>
    public void ShowPrompt(string message)
    {
        if (promptText != null)
        {
            promptText.text = message;
        }
        
        if (promptPanel != null)
        {
            promptPanel.SetActive(true);
        }
    }
    
    /// <summary>
    /// Hide the prompt
    /// </summary>
    public void HidePrompt()
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Show prompt for an open door
    /// </summary>
    public void ShowOpenDoorPrompt()
    {
        ShowPrompt(openDoorPrompt);
    }
    
    /// <summary>
    /// Show prompt for a closed door
    /// </summary>
    public void ShowCloseDoorPrompt()
    {
        ShowPrompt(closeDoorPrompt);
    }
    
    /// <summary>
    /// Show prompt for a locked door
    /// </summary>
    public void ShowLockedDoorPrompt()
    {
        ShowPrompt(lockedDoorPrompt);
    }
    
    /// <summary>
    /// Show prompt for unlocking a door with a key
    /// </summary>
    public void ShowUnlockDoorPrompt()
    {
        ShowPrompt(unlockDoorPrompt);
    }
    
    /// <summary>
    /// Show prompt for picking up a key
    /// </summary>
    public void ShowPickupKeyPrompt()
    {
        ShowPrompt(pickupKeyPrompt);
    }
    
    /// <summary>
    /// Show prompt for entering a locker
    /// </summary>
    public void ShowEnterLockerPrompt()
    {
        ShowPrompt(enterLockerPrompt);
    }
    
    /// <summary>
    /// Show prompt for exiting a locker
    /// </summary>
    public void ShowExitLockerPrompt()
    {
        ShowPrompt(exitLockerPrompt);
    }
}
