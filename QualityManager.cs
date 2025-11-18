using UnityEngine;
using UnityEngine.PostProcessing;

public class QualityManager : MonoBehaviour
{
	public MonoBehaviour[] MyBehaviours;

	public DynamicDepthOfField[] DynamicDOF;

	private void Update()
	{
		MonoBehaviour[] myBehaviours = MyBehaviours;
		for (int i = 0; i < myBehaviours.Length; i++)
		{
			PostProcessingBehaviour postProcessingBehaviour = (PostProcessingBehaviour)myBehaviours[i];
			if (GameSettings.LowQuality)
			{
				postProcessingBehaviour.enabled = false;
			}
			else
			{
				postProcessingBehaviour.enabled = true;
			}
		}
		DynamicDepthOfField[] dynamicDOF = DynamicDOF;
		foreach (DynamicDepthOfField dynamicDepthOfField in dynamicDOF)
		{
			if (GameSettings.LowQuality)
			{
				dynamicDepthOfField.enabled = false;
			}
			else
			{
				dynamicDepthOfField.enabled = true;
			}
		}
	}
}
