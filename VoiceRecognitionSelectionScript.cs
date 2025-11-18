using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VoiceRecognitionSelectionScript : MonoBehaviour
{
	public Transform Selector;

	public RawImage FadeSprite;

	public bool isEnablingVoiceRecognition;

	private int phase;

	private void Start()
	{
		if (!SystemInfo.operatingSystem.ToLower().Contains("windows 10"))
		{
			GameSettings.VoiceRecognitionIncompatible = true;
			GameSettings.VoiceRecognition = false;
			SceneManager.LoadScene("TitleScene");
		}
		FadeSprite.color = Color.black;
	}

	public void Update()
	{
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		Selector.localPosition = new Vector3(32.25f, Selector.localPosition.y, Mathf.Lerp(Selector.localPosition.z, (!isEnablingVoiceRecognition) ? (-15.14f) : 12.3f, Time.deltaTime * 10f));
		if (phase == 0)
		{
			FadeSprite.color -= new Color(0f, 0f, 0f, Time.deltaTime * 2f);
			if (FadeSprite.color.a <= 0f)
			{
				phase++;
			}
		}
		else if (phase == 1)
		{
			if (InputManager.instance.StickLeft || InputManager.instance.Left)
			{
				isEnablingVoiceRecognition = true;
			}
			else if (InputManager.instance.StickRight || InputManager.instance.Right)
			{
				isEnablingVoiceRecognition = false;
			}
			else if (InputManager.instance.A)
			{
				phase++;
			}
		}
		else
		{
			FadeSprite.color += new Color(0f, 0f, 0f, Time.deltaTime * 2f);
			if (FadeSprite.color.a >= 1f)
			{
				GameSettings.VoiceRecognition = isEnablingVoiceRecognition;
				SceneManager.LoadScene("TitleScene");
			}
		}
	}
}
