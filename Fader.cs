using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
	public RawImage MyRawImage;

	public float Speed;

	public bool FadeOut;

	private void Start()
	{
		if (!MyRawImage)
		{
			MyRawImage = GetComponent<RawImage>();
		}
		if ((bool)MyRawImage)
		{
			MyRawImage.color = new Color(MyRawImage.color.r, MyRawImage.color.g, MyRawImage.color.b, FadeOut ? 1f : 0f);
		}
	}

	private void Update()
	{
		if ((bool)MyRawImage)
		{
			MyRawImage.color = new Color(MyRawImage.color.r, MyRawImage.color.g, MyRawImage.color.b, Mathf.MoveTowards(MyRawImage.color.a, FadeOut ? 0f : 1f, Time.unscaledDeltaTime * Speed));
			if ((MyRawImage.color.a <= 0f && FadeOut) || (MyRawImage.color.a >= 1f && !FadeOut))
			{
				Object.Destroy(this);
			}
		}
	}
}
