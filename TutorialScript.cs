using UnityEngine;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour
{
	public RawImage MyImage;

	public Texture2D KunTexture;

	public Texture2D ChanTexture;

	public void Start()
	{
		Time.timeScale = 0f;
		MyImage.enabled = true;
		MyImage.texture = (GameSettings.KunMode ? KunTexture : ChanTexture);
	}

	public void Update()
	{
		if (InputManager.instance.A)
		{
			Time.timeScale = 1f;
			MyImage.enabled = false;
			base.gameObject.SetActive(value: false);
		}
	}
}
