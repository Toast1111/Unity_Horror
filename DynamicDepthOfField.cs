using UnityEngine;
using UnityEngine.PostProcessing;

public class DynamicDepthOfField : MonoBehaviour
{
	public float LerpSpeed = 10f;

	public float MaxDistance = 5f;

	public PostProcessingBehaviour MyBehaviour;

	public LayerMask Layers;

	private void Update()
	{
		if (!GameSettings.LowQuality)
		{
			if (Physics.Raycast(base.transform.position, base.transform.forward * MaxDistance, out var hitInfo, MaxDistance, Layers))
			{
				Debug.DrawRay(base.transform.position, hitInfo.point);
				DepthOfFieldModel.Settings settings = MyBehaviour.profile.depthOfField.settings;
				settings.focusDistance = Mathf.Lerp(settings.focusDistance, Vector3.Distance(base.transform.position, hitInfo.point), Time.deltaTime * LerpSpeed);
				MyBehaviour.profile.depthOfField.settings = settings;
			}
			else
			{
				DepthOfFieldModel.Settings settings2 = MyBehaviour.profile.depthOfField.settings;
				settings2.focusDistance = Mathf.Lerp(settings2.focusDistance, MaxDistance, Time.deltaTime * LerpSpeed);
				MyBehaviour.profile.depthOfField.settings = settings2;
			}
		}
	}
}
