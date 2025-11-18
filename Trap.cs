using UnityEngine;

public class Trap : MonoBehaviour
{
	public Zone MyZone;

	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			PlayerController.instance.Trip();
			if (YandereScript.instance.CurrentState != YandereScript.State.Chase)
			{
				PlayerController.instance.LastRunPosition = YandereScript.instance.PowerControlSpot.position;
				YandereScript.instance.CurrentState = YandereScript.State.Distracted;
			}
			MyZone.MyTrap = null;
			TrapManager.instance.TrappedZones.Remove(MyZone);
			TrapManager.instance.SpawnRandomTrap();
			Object.Destroy(base.gameObject);
		}
	}
}
