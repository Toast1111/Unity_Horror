using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ZoneManager : MonoBehaviour
{
	public static ZoneManager instance;

	public Zone[] LockableZones;

	public Collectable[] Keys;

	public List<Zone> LockedZones = new List<Zone>();

	private const int EXIT_KEY = 9;

	private const int START_KEY = 0;

	private void OnEnable()
	{
		instance = this;
	}

	private void OnDisable()
	{
		instance = null;
	}

	private void Start()
	{
		for (int i = 0; i < 9; i++)
		{
			int num = Random.Range(0, LockableZones.Length);
			if (i > 0)
			{
				while (LockedZones.Contains(LockableZones[num]))
				{
					num = Random.Range(0, LockableZones.Length);
				}
			}
			LockedZones.Add(LockableZones[num]);
			Door[] doors = LockedZones[i].Doors;
			foreach (Door door in doors)
			{
				door.Locked = true;
				door.Text = "Locked";
				if (door.MyObstacle != null)
				{
					door.MyObstacle.enabled = true;
				}
			}
		}
		for (int k = 1; k < 10; k++)
		{
			Zone zone = LockedZones[k - 1];
			Collectable collectable = Keys[k];
			int index = Random.Range(0, zone.KeySpotsParent.childCount);
			Transform child = zone.KeySpotsParent.GetChild(index);
			collectable.transform.position = child.position;
			collectable.transform.rotation = child.rotation;
			if (k != 9)
			{
				collectable.Room = LockedZones[k];
				collectable.Text = "Collect (" + collectable.Room.RoomName + ")";
			}
			else
			{
				collectable.Text = "Collect (EXIT)";
			}
		}
		Keys[0].Room = LockedZones[0];
		Keys[0].Text = "Collect (" + LockedZones[0].RoomName + ")";
	}

	public Vector3 GetAccessableHidingSpot(Collectable Key)
	{
		List<Zone> list = LockableZones.ToList();
		foreach (Zone lockedZone in LockedZones)
		{
			list.Remove(lockedZone);
		}
		int index = Random.Range(0, list.Count);
		while (list[index] == null)
		{
			index = Random.Range(0, list.Count);
		}
		Zone zone = list[index];
		index = Random.Range(0, zone.KeySpotsParent.childCount);
		Transform child = zone.KeySpotsParent.GetChild(index);
		Key.transform.position = child.position;
		Key.transform.rotation = child.rotation;
		Key.Collected = false;
		NavMesh.FindClosestEdge(new Vector3(child.position.x, 0f, child.position.z), out var hit, 0);
		return hit.position;
	}
}
