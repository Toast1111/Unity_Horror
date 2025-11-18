using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrapManager : MonoBehaviour
{
	public static TrapManager instance;

	public List<Zone> SortedSpottedAreas = new List<Zone>();

	public List<Zone> TrappedZones = new List<Zone>();

	public List<Door> PotentialDoors = new List<Door>();

	public List<Door> JumpscareDoors = new List<Door>();

	[Space(20f)]
	public GameObject TrapPrefab;

	public Transform TrapParent;

	[Space(20f)]
	public GameObject BodyPrefab;

	public Transform BodyParent;

	private void OnEnable()
	{
		instance = this;
	}

	private void OnDisable()
	{
		instance = null;
	}

	public void SortSpottedAreas()
	{
		List<Zone> list = ZoneManager.instance.LockableZones.ToList();
		foreach (Zone lockedZone in ZoneManager.instance.LockedZones)
		{
			if (list.Contains(lockedZone))
			{
				list.Remove(lockedZone);
			}
		}
		SortedSpottedAreas.Clear();
		foreach (Zone item in list)
		{
			if (item.SeenTimes > 0)
			{
				SortedSpottedAreas.Add(item);
			}
		}
		SortedSpottedAreas.Sort(Zone.SortBySeenTimes);
	}

	public void SpawnRandomTrap()
	{
		SortSpottedAreas();
		Zone zone = null;
		if (SortedSpottedAreas.Count != 0)
		{
			int index = Random.Range(0, SortedSpottedAreas.Count);
			while (PlayerController.instance.CurrentZone == SortedSpottedAreas[index] || YandereScript.instance.CurrentZone == SortedSpottedAreas[index] || (TrappedZones.Count == 0 && TrappedZones.Contains(SortedSpottedAreas[index])))
			{
				index = Random.Range(0, SortedSpottedAreas.Count);
			}
			if (TrappedZones.Count == 0 || !TrappedZones.Contains(SortedSpottedAreas[index]))
			{
				zone = SortedSpottedAreas[index];
			}
		}
		else
		{
			List<Zone> list = ZoneManager.instance.LockableZones.ToList();
			foreach (Zone lockedZone in ZoneManager.instance.LockedZones)
			{
				if (list.Contains(lockedZone))
				{
					list.Remove(lockedZone);
				}
			}
			int index2 = Random.Range(0, list.Count);
			while (PlayerController.instance.CurrentZone == list[index2] || YandereScript.instance.CurrentZone == list[index2] || (TrappedZones.Count == 0 && TrappedZones.Contains(list[index2])))
			{
				index2 = Random.Range(0, list.Count);
			}
			if (TrappedZones.Count == 0 || !TrappedZones.Contains(list[index2]))
			{
				zone = list[index2];
			}
		}
		if (zone != null)
		{
			int num = Random.Range(0, zone.Doors.Length);
			(zone.MyTrap = Object.Instantiate(TrapPrefab, zone.Doors[num].transform.position, zone.Doors[num].transform.rotation, TrapParent).GetComponent<Trap>()).MyZone = zone;
			TrappedZones.Add(zone);
		}
	}

	public void SpawnRandomCorpse()
	{
		List<Door> potentialDoors = PotentialDoors;
		foreach (Door jumpscareDoor in JumpscareDoors)
		{
			if (potentialDoors.Contains(jumpscareDoor))
			{
				potentialDoors.Remove(jumpscareDoor);
			}
		}
		Door door = potentialDoors[Random.Range(0, potentialDoors.Count)];
		Object.Instantiate(BodyPrefab, door.transform.position, door.transform.rotation, BodyParent).GetComponent<Body>().MyDoor = door;
		JumpscareDoors.Add(door);
	}

	public void Update()
	{
	}
}
