using UnityEngine;

[ExecuteInEditMode]
public class OLDTVScreen : FilterBehavior
{
	public float screenSaturation;

	public Texture chromaticAberrationPattern;

	public float chromaticAberrationMagnetude = 0.015f;

	public Texture noisePattern;

	public float noiseMagnetude = 0.1f;

	public Texture staticPattern;

	public Texture staticMask;

	public float staticVertical;

	public float staticMagnetude = 0.015f;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.material.SetFloat("_Saturation", screenSaturation);
		base.material.SetTexture("_ChromaticAberrationTex", chromaticAberrationPattern);
		base.material.SetFloat("_ChromaticAberrationMagnetude", chromaticAberrationMagnetude);
		base.material.SetTexture("_NoiseTex", noisePattern);
		base.material.SetFloat("_NoiseOffsetX", Random.Range(0f, 1f));
		base.material.SetFloat("_NoiseOffsetY", Random.Range(0f, 1f));
		base.material.SetFloat("_NoiseMagnetude", noiseMagnetude);
		base.material.SetTexture("_StaticTex", staticPattern);
		base.material.SetTexture("_StaticMask", staticMask);
		base.material.SetFloat("_StaticVertical", staticVertical);
		base.material.SetFloat("_StaticMagnetude", staticMagnetude);
		Graphics.Blit(source, destination, base.material);
	}
}
