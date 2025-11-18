using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[Serializable]
public class Door : MonoBehaviour, IInteractable
{
	private string text = "Open Door";

	public bool Open;

	public bool Locked;

	public bool Swinging;

	public bool Exit;

	public Vector2 SwingingDoorAngles;

	public Transform LeftDoor;

	public Transform RightDoor;

	public BoxCollider LeftDoorCollider;

	public BoxCollider RightDoorCollider;

	public AudioSource SFXSource;

	public GameObject EscapedUI;

	public GameObject HUD;

	public TMP_Text CurrentKeyLabel;

	public Zone MyZone;

	public NavMeshObstacle MyObstacle;

	public bool previouslyLocked;

	private float swingingTimer;

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

	public void Unlock()
	{
		Locked = false;
		previouslyLocked = true;
		if (YandereScript.instance.CurrentState != YandereScript.State.Chase && YandereScript.instance.CurrentState != YandereScript.State.HideKey)
		{
			PlayerController.instance.LastRunPosition = base.transform.position;
			YandereScript.instance.CurrentState = YandereScript.State.Distracted;
			YandereScript.instance.GoToNewDoor = true;
		}
		text = "Open Door";
	}

	public void Start()
	{
		if (!Swinging)
		{
			MyObstacle = GetComponent<NavMeshObstacle>();
		}
		else
		{
			TrapManager.instance.PotentialDoors.Add(this);
		}
		if (Locked)
		{
			text = "Locked";
		}
		else if (MyObstacle != null)
		{
			MyObstacle.enabled = false;
		}
		if (!SFXSource)
		{
			SFXSource = GetComponent<AudioSource>();
		}
		if ((bool)LeftDoor)
		{
			LeftDoorCollider = LeftDoor.GetComponent<BoxCollider>();
		}
		if ((bool)RightDoor)
		{
			RightDoorCollider = RightDoor.GetComponent<BoxCollider>();
		}
	}

	public void Update()
	{
		if (Swinging)
		{
			if (Open)
			{
				if ((bool)LeftDoor)
				{
					LeftDoor.localRotation = Quaternion.Slerp(LeftDoor.localRotation, new Quaternion(0f, SwingingDoorAngles.x, 0f, 1f), Time.deltaTime * 10f);
				}
				if ((bool)RightDoor)
				{
					RightDoor.localRotation = Quaternion.Slerp(RightDoor.localRotation, new Quaternion(0f, SwingingDoorAngles.y, 0f, 1f), Time.deltaTime * 10f);
				}
			}
			else
			{
				if ((bool)LeftDoor)
				{
					LeftDoor.localRotation = Quaternion.Slerp(LeftDoor.localRotation, Quaternion.identity, Time.deltaTime * 10f);
				}
				if ((bool)RightDoor)
				{
					RightDoor.localRotation = Quaternion.Slerp(RightDoor.localRotation, Quaternion.identity, Time.deltaTime * 10f);
				}
			}
		}
		else if (Exit)
		{
			if (Open)
			{
				Time.timeScale = 0f;
				EscapedUI.SetActive(value: true);
				HUD.SetActive(value: false);
				if (InputManager.instance.A)
				{
					SceneManager.LoadScene("CreditsScene");
				}
			}
		}
		else if (Open)
		{
			if ((bool)LeftDoor)
			{
				LeftDoor.localPosition = Vector3.Lerp(LeftDoor.localPosition, Vector3.right, Time.deltaTime * 10f);
			}
			if ((bool)RightDoor)
			{
				RightDoor.localPosition = Vector3.Lerp(RightDoor.localPosition, Vector3.left, Time.deltaTime * 10f);
			}
		}
		else
		{
			if ((bool)LeftDoor)
			{
				LeftDoor.localPosition = Vector3.Lerp(LeftDoor.localPosition, Vector3.zero, Time.deltaTime * 10f);
			}
			if ((bool)RightDoor)
			{
				RightDoor.localPosition = Vector3.Lerp(RightDoor.localPosition, Vector3.zero, Time.deltaTime * 10f);
			}
		}
		if (Exit)
		{
			return;
		}
		if (!Swinging)
		{
			if ((bool)LeftDoor)
			{
				if (Vector3.Distance(LeftDoor.localPosition, Vector3.zero) < 0.01f || Vector3.Distance(LeftDoor.localPosition, Vector3.right) < 0.01f)
				{
					LeftDoorCollider.enabled = true;
				}
				else
				{
					LeftDoorCollider.enabled = false;
				}
			}
			if ((bool)RightDoor)
			{
				if (Vector3.Distance(RightDoor.localPosition, Vector3.zero) < 0.01f || Vector3.Distance(RightDoor.localPosition, Vector3.left) < 0.01f)
				{
					RightDoorCollider.enabled = true;
				}
				else
				{
					RightDoorCollider.enabled = false;
				}
			}
		}
		else
		{
			swingingTimer += Time.deltaTime;
			if ((bool)LeftDoor)
			{
				LeftDoorCollider.enabled = swingingTimer >= 0.8f;
			}
			if ((bool)RightDoor)
			{
				RightDoorCollider.enabled = swingingTimer >= 0.8f;
			}
		}
	}

	public void Interact()
	{
		if (Locked)
		{
			return;
		}
		if (Vector3.Distance(YandereScript.instance.transform.position, base.transform.position) < 5f && YandereScript.instance.CurrentState != YandereScript.State.Chase && YandereScript.instance.CurrentState != YandereScript.State.HideKey)
		{
			PlayerController.instance.LastRunPosition = base.transform.position;
			YandereScript.instance.CurrentState = YandereScript.State.Distracted;
		}
		if (previouslyLocked)
		{
			CurrentKeyLabel.text = string.Empty;
			PlayerController.instance.CurrentKey = null;
			if (!Exit)
			{
				Door[] doors = MyZone.Doors;
				foreach (Door obj in doors)
				{
					obj.previouslyLocked = false;
					if (obj.MyObstacle != null)
					{
						MyObstacle.enabled = false;
					}
				}
			}
			previouslyLocked = false;
		}
		Open = !Open;
		text = (Open ? "Close Door" : "Open Door");
		swingingTimer = 0f;
		if ((bool)SFXSource && !Exit)
		{
			SFXSource.Play();
		}
	}

	public void Interact(bool isOpen)
	{
		if (Locked || previouslyLocked)
		{
			return;
		}
		if (Swinging && PlayerController.instance.CurrentLocker == this)
		{
			Open = isOpen;
			text = (Open ? "Close Door" : "Open Door");
			if ((bool)SFXSource && !Exit)
			{
				SFXSource.Play();
			}
		}
		else if (!Swinging)
		{
			Open = isOpen;
			text = (Open ? "Close Door" : "Open Door");
			swingingTimer = 0f;
			if ((bool)SFXSource && !Exit)
			{
				SFXSource.Play();
			}
		}
	}
}
