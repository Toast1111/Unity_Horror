using System;
using TMPro;
using UnityEngine;

[Serializable]
public class Collectable : MonoBehaviour, IInteractable
{
	private string text = "Collect";

	public Zone Room;

	public TMP_Text CurrentKeyLabel;

	public bool Collected;

	public Door ExitDoor;

	public bool triggersAggressiveMode;

	private bool wasPickedUpOnce;

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
		}
	}

	public void Interact()
	{
		Collected = true;
		PlayerController.instance.CurrentKey = this;
		if (!wasPickedUpOnce)
		{
			TrapManager.instance.SpawnRandomTrap();
			TrapManager.instance.SpawnRandomCorpse();
			QuickMenuScript.CollectedKeys++;
			wasPickedUpOnce = true;
		}
		if (!ExitDoor)
		{
			Door[] doors = Room.Doors;
			for (int i = 0; i < doors.Length; i++)
			{
				doors[i].Unlock();
			}
			CurrentKeyLabel.text = "CURRENT KEY: " + Room.RoomName;
			if (triggersAggressiveMode)
			{
				YandereScript.instance.TurnAggressive();
			}
		}
		else
		{
			ExitDoor.Unlock();
			CurrentKeyLabel.text = "CURRENT KEY: EXIT";
		}
		base.gameObject.SetActive(value: false);
	}
}
