using UnityEngine;

public class Key : MonoBehaviour
{
    public string keyID = "Key_1";
    public string keyName = "Generic Key";
    
    private bool isPickedUp = false;
    
    public void PickUp()
    {
        isPickedUp = true;
        gameObject.SetActive(false);
    }
    
    public bool IsPickedUp()
    {
        return isPickedUp;
    }
}
