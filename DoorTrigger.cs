using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
	public Door myDoor;

	public BoxCollider MyCollider;

	public void Start()
	{
		myDoor = base.transform.parent.GetComponent<Door>();
		MyCollider = GetComponent<BoxCollider>();
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 9 && (bool)myDoor)
		{
			if (!myDoor.Swinging)
			{
				myDoor.Interact(isOpen: true);
			}
			else if (MyCollider.bounds.Contains(YandereScript.instance.Pathfinding.destination))
			{
				myDoor.Interact(isOpen: true);
			}
		}
		if (other.gameObject.layer == 8 && (bool)myDoor && myDoor.Swinging)
		{
			PlayerController.instance.CurrentLocker = myDoor;
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 9 && (bool)myDoor)
		{
			myDoor.Interact(isOpen: false);
		}
		if (other.gameObject.layer == 8 && (bool)myDoor && myDoor.Swinging && PlayerController.instance.CurrentLocker == myDoor)
		{
			PlayerController.instance.CurrentLocker = null;
		}
	}
}
