using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("")]
public class FilterBehavior : MonoBehaviour
{
	public Shader shader;

	private Material _Material;

	protected Material material
	{
		get
		{
			if (_Material == null)
			{
				_Material = new Material(shader);
				_Material.hideFlags = HideFlags.HideAndDontSave;
			}
			return _Material;
		}
	}

	protected virtual void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			base.enabled = false;
		}
		else if (!shader || !shader.isSupported)
		{
			base.enabled = false;
		}
	}

	protected virtual void OnDisable()
	{
		if ((bool)_Material)
		{
			Object.DestroyImmediate(_Material);
		}
	}
}
