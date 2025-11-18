using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuickMenuScript : MonoBehaviour
{
	public static int CollectedKeys;

	public Transform Selector;

	public GameObject Menu;

	public TMP_Text KeyLabel;

	private int phase;

	private bool shouldLeave;

	public void Start()
	{
		CollectedKeys = 0;
	}

	public void Update()
	{
		if (phase == 0)
		{
			if (Time.timeScale >= 1f && PlayerController.instance.CanMove && InputManager.instance.Start)
			{
				shouldLeave = false;
				Menu.SetActive(value: true);
				Time.timeScale = 0f;
				phase++;
			}
		}
		else if (phase == 1)
		{
			if (InputManager.instance.StickDown || InputManager.instance.StickUp || InputManager.instance.Up || InputManager.instance.Down)
			{
				shouldLeave = !shouldLeave;
			}
			else if (InputManager.instance.A)
			{
				phase++;
			}
			else if (InputManager.instance.Start)
			{
				shouldLeave = false;
				phase++;
			}
		}
		else if (phase == 2)
		{
			if (shouldLeave)
			{
				SceneManager.LoadScene("TitleScene");
			}
			else
			{
				Menu.SetActive(value: false);
				Time.timeScale = 1f;
				phase = 0;
			}
		}
		Selector.localPosition = new Vector3(Selector.localPosition.x, Mathf.Lerp(Selector.localPosition.y, shouldLeave ? 16f : 124f, Time.unscaledDeltaTime * 10f), Selector.localPosition.z);
		KeyLabel.text = CollectedKeys + "/10 keys collected";
	}
}
