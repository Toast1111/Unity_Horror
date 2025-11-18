using UnityEngine;

public class Rotate : MonoBehaviour
{
	public Vector3 rotation = new Vector3(10f, -15f, 7.5f);

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Rotate(rotation * Time.deltaTime);
	}
}
