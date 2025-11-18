using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class YandereSFXSpawner : MonoBehaviour
{
	public static YandereSFXSpawner instance;

	public Dictionary<string, Action> actions = new Dictionary<string, Action>();

	public KeywordRecognizer keywordRecognizer;

	public AudioSource SFXSource;

	public AudioClip[] ChanNormalClips;

	public AudioClip[] KunNormalClips;

	public AudioClip[] YesClips;

	public AudioClip ChanMurderClip;

	public AudioClip KunMurderClip;

	private AudioClip[] TargetClips;

	private AudioClip TargetMurderClip;

	public AudioClip ChanDodgeClip;

	public SkinnedMeshRenderer MyRenderer;

	public bool ShouldBeMute;

	private float timer;

	private float delayTime;

	public int TriggerCount;

	private bool murder;

	public void OnEnable()
	{
		delayTime = UnityEngine.Random.Range(20f, 25f);
		TargetClips = (GameSettings.KunMode ? KunNormalClips : ChanNormalClips);
		TargetMurderClip = (GameSettings.KunMode ? KunMurderClip : ChanMurderClip);
		if (GameSettings.KunMode && YandereScript.instance != null)
		{
			MyRenderer = YandereScript.instance.KunRenderer;
		}
		instance = this;
	}

	public void OnDisable()
	{
		instance = null;
	}

	public void Start()
	{
		if (PhraseRecognitionSystem.isSupported && GameSettings.VoiceRecognition)
		{
			actions.Add("ayano aishi", null);
			actions.Add("ayato aishi", null);
			actions.Add("ayato", null);
			actions.Add("ayano", null);
			actions.Add("where is she", null);
			actions.Add("are you there", null);
			actions.Add("can you hear me", null);
			actions.Add("where are you", null);
			actions.Add("where is the key", null);
			keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray(), ConfidenceLevel.Low);
			keywordRecognizer.OnPhraseRecognized += HasRecognizedSpeech;
			keywordRecognizer.Start();
		}
	}

	public void PlayDodgeClip()
	{
		timer = 0f;
		SFXSource.clip = ChanDodgeClip;
		SFXSource.Play();
		delayTime = UnityEngine.Random.Range(20f, 25f);
	}

	private void HasRecognizedSpeech(PhraseRecognizedEventArgs speech)
	{
		if (murder || SFXSource.isPlaying || !(Time.timeScale > 0f) || ShouldBeMute)
		{
			return;
		}
		switch (speech.text)
		{
		case "ayato aishi":
		case "ayano aishi":
		case "ayano":
		case "ayato":
		case "can you hear me":
		case "are you there":
		case "where is she":
		case "where are you":
		case "where is the key":
			if (GameSettings.KunMode)
			{
				timer = delayTime;
			}
			else
			{
				AudioClip audioClip = null;
				switch (speech.text)
				{
				case "where is she":
				case "where are you":
				case "where is the key":
				{
					int num = UnityEngine.Random.Range(17, 19);
					audioClip = TargetClips[num];
					SFXSource.clip = audioClip;
					SFXSource.Play();
					delayTime = UnityEngine.Random.Range(20f, 25f);
					timer = 0f;
					break;
				}
				case "ayano aishi":
				case "ayano":
				case "can you hear me":
				case "are you there":
					audioClip = YesClips[UnityEngine.Random.Range(0, YesClips.Length)];
					SFXSource.clip = audioClip;
					SFXSource.Play();
					delayTime = UnityEngine.Random.Range(20f, 25f);
					timer = 0f;
					break;
				}
			}
			TriggerCount++;
			if (!YandereScript.instance.IsStatic && YandereScript.instance.CurrentState != YandereScript.State.Chase && YandereScript.instance.CurrentState != YandereScript.State.HideKey)
			{
				PlayerController.instance.LastRunPosition = PlayerController.instance.transform.position;
				YandereScript.instance.CurrentState = YandereScript.State.Distracted;
			}
			if (!YandereScript.instance.IsStatic && TriggerCount > 3 && !YandereScript.instance.isAggressive)
			{
				YandereScript.instance.TurnAggressive();
			}
			break;
		}
	}

	public void Update()
	{
		if (!murder)
		{
			timer += Time.deltaTime;
			if (!ShouldBeMute && timer >= delayTime)
			{
				timer = 0f;
				SFXSource.clip = TargetClips[UnityEngine.Random.Range(0, TargetClips.Length)];
				SFXSource.Play();
				delayTime = UnityEngine.Random.Range(20f, 25f);
			}
			MyRenderer.SetBlendShapeWeight(2, Mathf.Clamp(Mathf.Lerp(MyRenderer.GetBlendShapeWeight(2), GetCurrentVolume(SFXSource, 2500f), Time.deltaTime * 20f), 0f, 100f));
		}
	}

	public float GetCurrentVolume(AudioSource source, float sensitivity)
	{
		float[] array = new float[256];
		float num = 0f;
		source.GetOutputData(array, 0);
		float[] array2 = array;
		foreach (float f in array2)
		{
			num += Mathf.Abs(f);
		}
		return num / 256f * sensitivity;
	}

	public void Murder()
	{
		if (!murder)
		{
			SFXSource.clip = TargetMurderClip;
			SFXSource.Play();
			murder = true;
		}
	}
}
