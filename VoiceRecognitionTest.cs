using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceRecognitionTest : MonoBehaviour
{
	public TMP_Text MyText;

	public Dictionary<string, Action> actions = new Dictionary<string, Action>();

	private KeywordRecognizer keywordRecognizer;

	private void Start()
	{
		if (PhraseRecognitionSystem.isSupported)
		{
			actions.Add("ayano aishi", null);
			actions.Add("senpai", null);
			actions.Add("osana", null);
			actions.Add("najimi", null);
			actions.Add("taro", null);
			actions.Add("yamada", null);
			actions.Add("where are you", null);
			actions.Add("i am here", null);
			actions.Add("how are you", null);
			actions.Add("test", null);
			keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray(), ConfidenceLevel.Low);
			keywordRecognizer.OnPhraseRecognized += HasRecognizedSpeech;
			keywordRecognizer.Start();
		}
	}

	private void HasRecognizedSpeech(PhraseRecognizedEventArgs speech)
	{
		Debug.Log(speech.text);
		MyText.text = speech.text;
	}
}
