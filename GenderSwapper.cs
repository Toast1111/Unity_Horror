using UnityEngine;

public class GenderSwapper : MonoBehaviour
{
	[SerializeField]
	private Renderer[] KunRenderers;

	[SerializeField]
	private Renderer[] ChanRenderers;

	private void Start()
	{
		Swap();
	}

	private void Swap()
	{
		Renderer[] kunRenderers = KunRenderers;
		for (int i = 0; i < kunRenderers.Length; i++)
		{
			kunRenderers[i].enabled = GameSettings.KunMode;
		}
		kunRenderers = ChanRenderers;
		for (int i = 0; i < kunRenderers.Length; i++)
		{
			kunRenderers[i].enabled = !GameSettings.KunMode;
		}
	}
}
