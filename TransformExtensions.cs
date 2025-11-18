using UnityEngine;

public static class TransformExtensions
{
	public static bool IsBehind(this Transform Target, Transform Base)
	{
		return Vector3.Dot(Target.position - Base.position, Base.transform.forward) <= 0f;
	}
}
