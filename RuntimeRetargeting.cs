using UnityEngine;

public class RuntimeRetargeting : MonoBehaviour
{
	public GameObject Base;

	public GameObject Target;

	private bool Initialized;

	private Transform[] BaseBoneset;

	private Transform[] TargetBoneset;

	private void OnEnable()
	{
		if (Initialized)
		{
			return;
		}
		BaseBoneset = Base.GetComponentsInChildren<Transform>(includeInactive: false);
		TargetBoneset = Target.GetComponentsInChildren<Transform>(includeInactive: false);
		Transform[] baseBoneset = BaseBoneset;
		foreach (Transform transform in baseBoneset)
		{
			Transform[] targetBoneset = TargetBoneset;
			foreach (Transform transform2 in targetBoneset)
			{
				if (transform.name == transform2.name)
				{
					AttachedBone.Create(transform, transform2);
				}
			}
		}
	}

	private void Update()
	{
		Base.transform.position = Target.transform.position;
		Base.transform.rotation = Target.transform.rotation;
	}
}
