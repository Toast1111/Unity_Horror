using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class PowerControl : MonoBehaviour, IInteractable
{
	private string text = "Activate";

	public bool isActive;

	public Light[] Lights;

	public AudioSource SFXSource;

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
		StartCoroutine(Switch(PowerOn: true));
		if (YandereScript.instance.CurrentState != YandereScript.State.Chase && YandereScript.instance.CurrentState != YandereScript.State.HideKey)
		{
			PlayerController.instance.LastRunPosition = YandereScript.instance.PowerControlSpot.position;
			YandereScript.instance.CurrentState = YandereScript.State.Distracted;
		}
		isActive = true;
	}

	public IEnumerator Switch(bool PowerOn)
	{
		ToggleLights(PowerOn);
		yield return new WaitForSeconds(0.1f);
		ToggleLights(!PowerOn);
		yield return new WaitForSeconds(0.1f);
		ToggleLights(PowerOn);
		isActive = PowerOn;
		if (!PowerOn)
		{
			SFXSource.Play();
		}
		yield return null;
	}

	private void ToggleLights(bool LightsOn)
	{
		Light[] lights = Lights;
		for (int i = 0; i < lights.Length; i++)
		{
			lights[i].enabled = LightsOn;
		}
	}
}
