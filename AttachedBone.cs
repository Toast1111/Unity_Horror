using UnityEngine;

public class AttachedBone : MonoBehaviour
{
	public Transform Target;

	public SkinnedMeshRenderer TargetRenderer;

	public SkinnedMeshRenderer BaseRenderer;

	public static void Create(Transform BaseBone, Transform TargetBone)
	{
		BaseBone.gameObject.AddComponent<AttachedBone>().Target = TargetBone;
	}

	public void Start()
	{
		BaseRenderer = GetComponent<SkinnedMeshRenderer>();
		TargetRenderer = Target.GetComponent<SkinnedMeshRenderer>();
	}

	public void Update()
	{
		base.transform.position = Target.position;
		base.transform.rotation = Target.rotation;
		if (!(BaseRenderer != null) || !(TargetRenderer != null) || BaseRenderer.sharedMesh.blendShapeCount == 0 || TargetRenderer.sharedMesh.blendShapeCount == 0)
		{
			return;
		}
		for (int i = 0; i < Mathf.Max(BaseRenderer.sharedMesh.blendShapeCount, TargetRenderer.sharedMesh.blendShapeCount); i++)
		{
			if (i < Mathf.Min(BaseRenderer.sharedMesh.blendShapeCount, TargetRenderer.sharedMesh.blendShapeCount) && i == 2)
			{
				BaseRenderer.SetBlendShapeWeight(i, TargetRenderer.GetBlendShapeWeight(i));
			}
		}
	}
}
