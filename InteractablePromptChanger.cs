using UnityEngine;
using UnityEngine.UI;

public class InteractablePromptChanger : MonoBehaviour
{
	private InputDeviceType DeviceType;

	public RawImage MyTexture;

	public Texture2D KeyboardPrompt;

	public Texture2D ControllerPrompt;

	private void Update()
	{
		if (DeviceType != InputManager.instance.DeviceType)
		{
			MyTexture.texture = ((InputManager.instance.DeviceType == InputDeviceType.Gamepad) ? ControllerPrompt : KeyboardPrompt);
			DeviceType = InputManager.instance.DeviceType;
		}
	}
}
