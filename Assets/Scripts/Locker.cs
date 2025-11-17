using UnityEngine;

public class Locker : MonoBehaviour
{
    private bool isOccupied = false;
    private PlayerController hiddenPlayer = null;
    
    public void Hide(PlayerController player)
    {
        isOccupied = true;
        hiddenPlayer = player;
    }
    
    public void Exit()
    {
        isOccupied = false;
        hiddenPlayer = null;
    }
    
    public bool IsOccupied()
    {
        return isOccupied;
    }
    
    public bool HasPlayer()
    {
        return hiddenPlayer != null;
    }
}
