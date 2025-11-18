using UnityEngine;

public class AnimationSpeedChanger : MonoBehaviour
{
	public Animation MyAnimation;

	public string AnimationName;

	public float AnimationSpeed;

	public void Update()
	{
		MyAnimation[AnimationName].speed = AnimationSpeed;
	}
}
