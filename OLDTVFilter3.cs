using UnityEngine;
using VortexStudios.PostProcessing;

[ExecuteInEditMode]
public class OLDTVFilter3 : MonoBehaviour
{
	[SerializeField]
	private OLDTVPreset _preset;

	[SerializeField]
	private Camera _camera;

	public bool customAspectRatio;

	[SerializeField]
	private Vector2 _aspectRatio = new Vector2(4f, 3f);

	public bool timeScale;

	public OLDTVPreset preset
	{
		get
		{
			return _preset;
		}
		set
		{
			_preset = value;
		}
	}

	public Vector2 aspectRatio
	{
		get
		{
			return _aspectRatio;
		}
		set
		{
			_aspectRatio = value;
		}
	}

	private void Start()
	{
		OnValidate();
	}

	private void Update()
	{
		if (!(_preset == null) && (!timeScale || Time.timeScale != 0f))
		{
			_preset.compositeFilter.OnFixedUpdate();
			_preset.noiseFilter.OnFixedUpdate();
			_preset.staticFilter.OnFixedUpdate();
			_preset.chromaticAberrationFilter.OnFixedUpdate();
			_preset.scanlineFilter.OnFixedUpdate();
			_preset.televisionFilter.OnFixedUpdate();
			_preset.tubeFilter.OnFixedUpdate();
		}
	}

	private void OnValidate()
	{
		aspectRatio = _aspectRatio;
	}

	private void OnPreRender()
	{
		if (!(_camera != null))
		{
			return;
		}
		if (customAspectRatio)
		{
			float num = (float)Screen.width / (float)Screen.height;
			float num2 = _aspectRatio.x / _aspectRatio.y;
			if (num / num2 >= 1f)
			{
				float num3 = (float)Screen.height / _aspectRatio.y * _aspectRatio.x;
				float x = ((float)Screen.width - num3) / 2f;
				_camera.pixelRect = new Rect(x, 0f, num3, Screen.height);
			}
			else
			{
				float num4 = (float)Screen.width / _aspectRatio.x * _aspectRatio.y;
				float y = ((float)Screen.height - num4) / 2f;
				_camera.pixelRect = new Rect(0f, y, Screen.width, num4);
			}
		}
		else
		{
			_camera.rect = new Rect(0f, 0f, 1f, 1f);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (_preset == null)
		{
			Graphics.Blit(source, destination);
			return;
		}
		source.wrapMode = TextureWrapMode.Repeat;
		if (PostProcessingProfile.SOURCEBUFFER == null || !PostProcessingProfile.SOURCEBUFFER.IsCreated() || PostProcessingProfile.SOURCEBUFFER.width != source.width || PostProcessingProfile.SOURCEBUFFER.height != source.height || PostProcessingProfile.SOURCEBUFFER.depth != source.depth)
		{
			if (PostProcessingProfile.SOURCEBUFFER != null && PostProcessingProfile.SOURCEBUFFER.IsCreated())
			{
				PostProcessingProfile.SOURCEBUFFER.DiscardContents();
			}
			PostProcessingProfile.SOURCEBUFFER = new RenderTexture(source.width, source.height, source.depth);
			PostProcessingProfile.SOURCEBUFFER.antiAliasing = 1;
			PostProcessingProfile.SOURCEBUFFER.anisoLevel = 0;
			PostProcessingProfile.SOURCEBUFFER.autoGenerateMips = false;
			PostProcessingProfile.SOURCEBUFFER.filterMode = FilterMode.Bilinear;
		}
		if (PostProcessingProfile.DESTBUFFER == null || !PostProcessingProfile.DESTBUFFER.IsCreated() || PostProcessingProfile.DESTBUFFER.width != source.width || PostProcessingProfile.DESTBUFFER.height != source.height || PostProcessingProfile.DESTBUFFER.depth != source.depth)
		{
			if (PostProcessingProfile.DESTBUFFER != null && PostProcessingProfile.DESTBUFFER.IsCreated())
			{
				PostProcessingProfile.DESTBUFFER.DiscardContents();
			}
			PostProcessingProfile.DESTBUFFER = new RenderTexture(source.width, source.height, source.depth);
			PostProcessingProfile.DESTBUFFER.antiAliasing = 1;
			PostProcessingProfile.DESTBUFFER.anisoLevel = 0;
			PostProcessingProfile.DESTBUFFER.autoGenerateMips = false;
			PostProcessingProfile.DESTBUFFER.filterMode = FilterMode.Bilinear;
		}
		Graphics.Blit(source, PostProcessingProfile.SOURCEBUFFER);
		if (_preset.noiseFilter.enabled)
		{
			_preset.noiseFilter.OnRenderImage(source);
		}
		if (_preset.staticFilter.enabled)
		{
			_preset.staticFilter.OnRenderImage(source);
		}
		if (_preset.compositeFilter.enabled)
		{
			_preset.compositeFilter.OnRenderImage(source);
		}
		if (_preset.chromaticAberrationFilter.enabled)
		{
			_preset.chromaticAberrationFilter.OnRenderImage(source);
		}
		if (_preset.televisionFilter.enabled)
		{
			_preset.televisionFilter.OnRenderImage(source);
		}
		if (_preset.scanlineFilter.enabled)
		{
			_preset.scanlineFilter.OnRenderImage(source);
		}
		if (_preset.tubeFilter.enabled)
		{
			_preset.tubeFilter.OnRenderImage(source);
		}
		Graphics.Blit(PostProcessingProfile.SOURCEBUFFER, destination);
	}
}
