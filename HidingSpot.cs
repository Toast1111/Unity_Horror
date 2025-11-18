using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour
{
	public ZoneType MyType;

	public Zone MyZone;

	public bool Left;

	[HideInInspector]
	public float DistanceToAyano;

	public static List<HidingSpot> ClassroomRegistery;

	public static List<HidingSpot> CorridorRegistery;

	private static Mesh SpyingMesh;

	private void OnEnable()
	{
		if (ClassroomRegistery == null)
		{
			ClassroomRegistery = new List<HidingSpot>();
		}
		if (CorridorRegistery == null)
		{
			CorridorRegistery = new List<HidingSpot>();
		}
		if (MyType == ZoneType.Classroom)
		{
			ClassroomRegistery.Add(this);
		}
		else
		{
			CorridorRegistery.Add(this);
		}
	}

	private void OnDisable()
	{
		if (MyType == ZoneType.Classroom)
		{
			ClassroomRegistery.Remove(this);
		}
		else
		{
			CorridorRegistery.Remove(this);
		}
	}

	private void Update()
	{
		DistanceToAyano = Vector3.Distance(base.transform.position, YandereScript.instance.transform.position);
	}

	public static HidingSpot GetClosest(ZoneType targetType)
	{
		HidingSpot hidingSpot = null;
		switch (targetType)
		{
		case ZoneType.Corridor:
			foreach (HidingSpot item in CorridorRegistery)
			{
				if (item.MyZone == PlayerController.instance.CurrentZone)
				{
					if (hidingSpot == null)
					{
						hidingSpot = item;
					}
					else if (item.DistanceToAyano < hidingSpot.DistanceToAyano && !PlayerController.instance.transform.IsBehind(item.transform))
					{
						hidingSpot = item;
					}
				}
			}
			break;
		case ZoneType.Classroom:
			foreach (HidingSpot item2 in ClassroomRegistery)
			{
				if (hidingSpot == null)
				{
					hidingSpot = item2;
				}
				else if (item2.DistanceToAyano < hidingSpot.DistanceToAyano)
				{
					hidingSpot = item2;
				}
			}
			break;
		}
		return hidingSpot;
	}
}
