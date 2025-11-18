using UnityEngine;

public class CameraReflexNew : MonoBehaviour
{
	private OLDTVFilter3 _oldtvfilter;

	private void Start()
	{
		_oldtvfilter = GetComponent<OLDTVFilter3>();
		string text = WebCamTexture.devices[0].name;
		WebCamDevice[] devices = WebCamTexture.devices;
		for (int i = 0; i < devices.Length; i++)
		{
			WebCamDevice webCamDevice = devices[i];
			if (webCamDevice.isFrontFacing)
			{
				text = webCamDevice.name;
				break;
			}
		}
		Debug.Log("Using " + text + " as webcam");
		WebCamTexture webCamTexture = new WebCamTexture(text);
		_oldtvfilter.preset.tubeFilter.reflexPattern = webCamTexture;
		webCamTexture.Play();
	}
}
