using UnityEngine;

public class KnifeScript : MonoBehaviour
{
	public float delay;

	public Rigidbody myRB;

	public GameObject Mesh;

	public AudioSource MySource;

	public AudioClip[] KnifeClips;

	public AudioClip[] StabClips;

	private float timer;

	private bool hasAccelerated;

	private void Update()
	{
		if (!hasAccelerated)
		{
			timer += Time.deltaTime;
			if (timer >= delay)
			{
				MySource.clip = KnifeClips[0];
				MySource.Play();
				myRB.AddForce(base.transform.forward * 25f, ForceMode.VelocityChange);
				Mesh.SetActive(value: true);
				hasAccelerated = true;
			}
			return;
		}
		if (Vector3.Distance(PlayerController.instance.transform.position, base.transform.position) > 100f)
		{
			Object.Destroy(base.gameObject);
		}
		if (Vector3.Distance(PlayerController.instance.transform.position, base.transform.position) < 15f)
		{
			if (MySource.clip != KnifeClips[1])
			{
				MySource.clip = KnifeClips[1];
				MySource.Play();
			}
			MySource.pitch = Mathf.Abs(1f - Vector3.Distance(PlayerController.instance.transform.position, base.transform.position) / 15f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			PlayerController.instance.Hurt();
			AudioSource audioSource = new GameObject("Stab").AddComponent<AudioSource>();
			audioSource.loop = false;
			audioSource.playOnAwake = false;
			audioSource.clip = StabClips[Random.Range(0, StabClips.Length)];
			audioSource.Play();
			Object.Destroy(audioSource, audioSource.clip.length);
			Object.Destroy(base.gameObject, audioSource.clip.length + 0.01f);
		}
	}
}
