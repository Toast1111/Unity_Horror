using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WarningSceneScript : MonoBehaviour
{
	public RawImage FadeSprite;

	private int phase;

	private void Start()
	{
		FadeSprite.color = Color.black;
	}

	private void Update()
	{
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
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
			if (InputManager.instance.A || InputManager.instance.B || InputManager.instance.X || InputManager.instance.Y || InputManager.instance.Start || InputManager.instance.Select || Input.anyKeyDown)
			{
				phase++;
			}
		}
		else
		{
			FadeSprite.color += new Color(0f, 0f, 0f, Time.deltaTime * 2f);
			if (FadeSprite.color.a >= 1f)
			{
				SceneManager.LoadScene("VoiceRecognitionScene");
			}
		}
	}
}
