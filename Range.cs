using System;

[Serializable]
public struct Range
{
	public float min;

	public float max;

	public Range(float Min, float Max)
	{
		min = Min;
		max = Max;
	}
}
