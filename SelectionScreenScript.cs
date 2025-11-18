using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionScreenScript : MonoBehaviour
{
	public RawImage FadeSprite;

	public Transform Selector;

	public AudioSource KunSource;

	public AudioSource ChanSource;

	public Transform VCam;

	public Transform DifficultyCameraSpot;

	public TMP_Text TitleLabel;

	private int phase;

	private bool isSelectingKun;

	private bool isSelectingHard;

	private void Start()
	{
		FadeSprite.color = Color.black;
	}

	private void Update()
	{
		if (phase == 0)
		{
			FadeSprite.color -= new Color(0f, 0f, 0f, Time.deltaTime * 2f);
			if (FadeSprite.color.a <= 0f)
			{
				phase++;
			}
			Selector.localPosition = new Vector3(Selector.localPosition.x, Selector.localPosition.y, Mathf.Lerp(Selector.localPosition.z, isSelectingKun ? 28.61f : (-24.71f), Time.deltaTime * 10f));
		}
		else if (phase == 1)
		{
			if (InputManager.instance.StickLeft || InputManager.instance.Left)
			{
				if (!isSelectingKun)
				{
					KunSource.Play();
				}
				isSelectingKun = true;
			}
			else if (InputManager.instance.StickRight || InputManager.instance.Right)
			{
				if (isSelectingKun)
				{
					ChanSource.Play();
				}
				isSelectingKun = false;
			}
			else if (InputManager.instance.A)
			{
				TitleLabel.text = "SELECT YOUR PREFERED DIFFICULTY";
				phase++;
			}
			Selector.localPosition = new Vector3(Selector.localPosition.x, Selector.localPosition.y, Mathf.Lerp(Selector.localPosition.z, isSelectingKun ? 28.61f : (-24.71f), Time.deltaTime * 10f));
		}
		else if (phase == 2)
		{
			VCam.transform.position = Vector3.Lerp(VCam.transform.position, DifficultyCameraSpot.position, Time.deltaTime * 10f);
			VCam.transform.rotation = Quaternion.Lerp(VCam.transform.rotation, DifficultyCameraSpot.rotation, Time.deltaTime * 10f);
			if (InputManager.instance.StickLeft || InputManager.instance.Left)
			{
				isSelectingHard = false;
			}
			else if (InputManager.instance.StickRight || InputManager.instance.Right)
			{
				isSelectingHard = true;
			}
			else if (InputManager.instance.A)
			{
				phase++;
			}
			Selector.localPosition = new Vector3(Mathf.Lerp(Selector.localPosition.x, 34.2f, Time.deltaTime * 10f), Selector.localPosition.y, Mathf.Lerp(Selector.localPosition.z, isSelectingHard ? (-13.52f) : 13.57f, Time.deltaTime * 10f));
		}
		else
		{
			FadeSprite.color += new Color(0f, 0f, 0f, Time.deltaTime * 2f);
			if (FadeSprite.color.a >= 1f)
			{
				GameSettings.KunMode = isSelectingKun;
				GameSettings.HardMode = isSelectingHard;
				SceneManager.LoadScene("SchoolScene");
			}
			Selector.localPosition = new Vector3(Mathf.Lerp(Selector.localPosition.x, 34.2f, Time.deltaTime * 10f), Selector.localPosition.y, Mathf.Lerp(Selector.localPosition.z, isSelectingHard ? (-13.52f) : 13.57f, Time.deltaTime * 10f));
		}
	}
}
