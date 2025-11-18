using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenScript : MonoBehaviour
{
	public RawImage FadeSprite;

	public Transform Selector;

	public Transform WarningLabel;

	public TMP_Text[] MenuLabels;

	public TMP_Text GameTitle;

	public GameObject EPrompt;

	public GameObject QPrompt;

	public GameObject LeftRightLabel;

	public AudioSource MusicBox;

	[Header("Graphical Settings")]
	public Vector2Int[] Resolutions;

	private int resolutionSelection;

	private int microphoneSelection;

	private int Phase;

	private int CurrentSelection;

	public void Start()
	{
		Time.timeScale = 1f;
		FadeSprite.color = Color.black;
		if (!SystemInfo.operatingSystem.ToLower().Contains("windows 10"))
		{
			GameSettings.VoiceRecognitionIncompatible = true;
			GameSettings.VoiceRecognition = false;
		}
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	public void Update()
	{
		if (Phase == 0)
		{
			FadeSprite.color -= new Color(0f, 0f, 0f, Time.deltaTime * 2f);
			if (FadeSprite.color.a <= 0f)
			{
				FadeSprite.color = new Color(0f, 0f, 0f, 0f);
				Phase++;
			}
		}
		else if (Phase == 1)
		{
			CurrentSelection += ((InputManager.instance.StickUp || InputManager.instance.Up) ? (-1) : ((InputManager.instance.StickDown || InputManager.instance.Down) ? 1 : 0));
			CurrentSelection = Mathf.Clamp(CurrentSelection, 0, MenuLabels.Length - 1);
			Selector.localPosition = Vector3.Lerp(Selector.localPosition, new Vector3(0f, MenuLabels[CurrentSelection].transform.localPosition.y, 0f), Time.deltaTime * 10f);
			if (InputManager.instance.A)
			{
				Phase++;
			}
		}
		else if (Phase == 2)
		{
			switch (CurrentSelection)
			{
			case 0:
				FadeSprite.color += new Color(0f, 0f, 0f, Time.deltaTime * 2f);
				MusicBox.volume -= Time.deltaTime * 2f;
				if (FadeSprite.color.a >= 1f)
				{
					FadeSprite.color = new Color(0f, 0f, 0f, 1f);
					SceneManager.LoadScene("SelectionScene");
				}
				break;
			case 1:
				GameTitle.text = "SETTINGS";
				MenuLabels[0].text = "RESOLUTION: " + Screen.width + "X" + Screen.height;
				MenuLabels[1].text = "FULL SCREEN: " + (Screen.fullScreen ? "ON" : "OFF");
				MenuLabels[2].text = "HIGH QUALITY: " + ((QualitySettings.GetQualityLevel() == 6) ? "ON" : "OFF");
				MenuLabels[3].text = (GameSettings.VoiceRecognitionIncompatible ? "" : ("VOICE RECOGNITION: " + (GameSettings.VoiceRecognition ? "ON" : "OFF")));
				EPrompt.SetActive(value: false);
				QPrompt.SetActive(value: true);
				LeftRightLabel.SetActive(value: true);
				Phase = 3;
				break;
			case 3:
				FadeSprite.color += new Color(0f, 0f, 0f, Time.deltaTime * 2f);
				MusicBox.volume -= Time.deltaTime * 2f;
				if (FadeSprite.color.a >= 1f)
				{
					FadeSprite.color = new Color(0f, 0f, 0f, 1f);
					Application.Quit();
				}
				break;
			default:
				FadeSprite.color += new Color(0f, 0f, 0f, Time.deltaTime * 2f);
				MusicBox.volume -= Time.deltaTime * 2f;
				if (FadeSprite.color.a >= 1f)
				{
					FadeSprite.color = new Color(0f, 0f, 0f, 1f);
					SceneManager.LoadScene("CreditsScene");
				}
				break;
			}
		}
		else if (Phase == 3)
		{
			CurrentSelection += ((InputManager.instance.StickUp || InputManager.instance.Up) ? (-1) : ((InputManager.instance.StickDown || InputManager.instance.Down) ? 1 : 0));
			CurrentSelection = Mathf.Clamp(CurrentSelection, 0, 3 - (GameSettings.VoiceRecognitionIncompatible ? 1 : 0));
			Selector.localPosition = Vector3.Lerp(Selector.localPosition, new Vector3(0f, MenuLabels[CurrentSelection].transform.localPosition.y, 0f), Time.deltaTime * 10f);
			if (InputManager.instance.StickLeft || InputManager.instance.StickRight || InputManager.instance.Left || InputManager.instance.Right)
			{
				if (CurrentSelection == 0)
				{
					if (InputManager.instance.Left || InputManager.instance.StickLeft)
					{
						resolutionSelection--;
					}
					else if (InputManager.instance.Right || InputManager.instance.StickRight)
					{
						resolutionSelection++;
					}
					resolutionSelection = Mathf.Clamp(resolutionSelection, 0, Resolutions.Length - 1);
					Screen.SetResolution(Resolutions[resolutionSelection].x, Resolutions[resolutionSelection].y, Screen.fullScreen);
					MenuLabels[0].text = "RESOLUTION: " + Resolutions[resolutionSelection].x + "X" + Resolutions[resolutionSelection].y;
				}
				if (CurrentSelection == 1)
				{
					Screen.fullScreen = !Screen.fullScreen;
					MenuLabels[1].text = "FULL SCREEN: " + (Screen.fullScreen ? "ON" : "OFF");
				}
				else if (CurrentSelection == 2)
				{
					QualitySettings.SetQualityLevel((QualitySettings.GetQualityLevel() == 6) ? 5 : 6);
					MenuLabels[2].text = "HIGH QUALITY: " + ((QualitySettings.GetQualityLevel() == 6) ? "ON" : "OFF");
					GameSettings.LowQuality = QualitySettings.GetQualityLevel() == 5;
				}
				else if (CurrentSelection == 3)
				{
					GameSettings.VoiceRecognition = !GameSettings.VoiceRecognition;
					if (!GameSettings.VoiceRecognition)
					{
						YandereSFXSpawner.instance.keywordRecognizer.Stop();
					}
					else
					{
						YandereSFXSpawner.instance.keywordRecognizer.Start();
					}
					MenuLabels[3].text = "VOICE RECOGNITION: " + (GameSettings.VoiceRecognition ? "ON" : "OFF");
				}
			}
			if (InputManager.instance.B)
			{
				Phase = 4;
			}
		}
		else if (Phase == 4)
		{
			GameTitle.text = "YANDERE NO SUTOKA";
			MenuLabels[0].text = "START";
			MenuLabels[1].text = "SETTINGS";
			MenuLabels[2].text = "CREDITS";
			MenuLabels[3].text = "QUIT";
			EPrompt.SetActive(value: true);
			QPrompt.SetActive(value: false);
			LeftRightLabel.SetActive(value: false);
			Phase = 0;
		}
		WarningLabel.localPosition = new Vector3(Mathf.Lerp(WarningLabel.localPosition.x, (Phase == 3 && CurrentSelection == 3 && !GameSettings.VoiceRecognitionIncompatible) ? 0f : 600f, Time.deltaTime * 20f), 0f, 0f);
	}
}
