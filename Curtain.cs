using System;
using UnityEngine;

[Serializable]
public class Curtain : MonoBehaviour, IInteractable
{
	private string text = "Close Curtains";

	public bool Open = true;

	public SkinnedMeshRenderer MyRenderer;

	public BoxCollider MyCollider;

	public AudioSource ClothSource;

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
		if (Vector3.Distance(YandereScript.instance.transform.position, base.transform.position) < 3.5f && YandereScript.instance.CurrentState != YandereScript.State.Chase && YandereScript.instance.CurrentState != YandereScript.State.HideKey)
		{
			PlayerController.instance.LastRunPosition = base.transform.position;
			YandereScript.instance.CurrentState = YandereScript.State.Distracted;
		}
		Open = !Open;
		Text = (Open ? "Close Curtains" : "Open Curtains");
		MyCollider.enabled = !Open;
		ClothSource.Play();
	}

	public void Update()
	{
		MyRenderer.SetBlendShapeWeight(0, Mathf.Lerp(MyRenderer.GetBlendShapeWeight(0), Open ? 100f : 0f, Time.deltaTime * 15f));
	}
}
