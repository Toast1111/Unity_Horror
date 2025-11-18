using UnityEngine;

public class Zone : MonoBehaviour
{
	public ZoneType MyType;

	public BoxCollider BoundingBox;

	[Header("Classroom Properties")]
	public string RoomName;

	public Door[] Doors;

	public Transform KeySpotsParent;

	public Trap MyTrap;

	public int SeenTimes;

	public static int SortBySeenTimes(Zone p1, Zone p2)
	{
		return p1.SeenTimes.CompareTo(p2.SeenTimes);
	}

	public bool Contains(Vector3 point)
	{
		return BoundingBox.bounds.Contains(point);
	}

	public void Start()
	{
		if (MyType == ZoneType.Classroom)
		{
			Door[] doors = Doors;
			for (int i = 0; i < doors.Length; i++)
			{
				doors[i].MyZone = this;
			}
		}
	}

	public void Update()
	{
		if (Contains(PlayerController.instance.transform.position))
		{
			PlayerController.instance.CurrentZone = this;
		}
		if (Contains(YandereScript.instance.transform.position))
		{
			YandereScript.instance.CurrentZone = this;
		}
	}
}
