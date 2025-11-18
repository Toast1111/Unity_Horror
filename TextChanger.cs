using TMPro;
using UnityEngine;

public class TextChanger : MonoBehaviour
{
	public TMP_Text MyText;

	public string KunText;

	public string ChanText;

	public void OnEnable()
	{
		MyText.text = (GameSettings.KunMode ? KunText : ChanText);
	}
}
