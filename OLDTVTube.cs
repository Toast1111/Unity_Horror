using UnityEngine;

[ExecuteInEditMode]
public class OLDTVTube : FilterBehavior
{
	public delegate void RepaintAction();

	public Texture scanlinePattern;

	public bool scanlineCountAuto;

	public int scanlineCount = 320;

	public float scanlineBrightMin = 0.75f;

	public float scanlineBrightMax = 1f;

	public Texture mask;

	public Texture reflex;

	public float reflexMagnetude = 0.5f;

	public float radialDistortion = 0.2f;

	public event RepaintAction WantRepaint;

	private void Repaint()
	{
		if (this.WantRepaint != null)
		{
			this.WantRepaint();
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.material.SetTexture("_ScanLine", scanlinePattern);
		if (scanlineCountAuto)
		{
			scanlineCount = Screen.height;
		}
		base.material.SetFloat("_ScanLineCount", scanlineCount / 2);
		base.material.SetFloat("_ScanLineMin", scanlineBrightMin);
		base.material.SetFloat("_ScanLineMax", scanlineBrightMax);
		base.material.SetTexture("_MaskTex", mask);
		base.material.SetTexture("_ReflexTex", reflex);
		base.material.SetFloat("_ReflexMagnetude", reflexMagnetude);
		base.material.SetFloat("_Distortion", radialDistortion);
		Graphics.Blit(source, destination, base.material);
	}
}
