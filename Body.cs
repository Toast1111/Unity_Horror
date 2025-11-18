using UnityEngine;

public class Body : MonoBehaviour
{
	public ParticleSystem[] MyParticles;

	public Rigidbody[] MyRBs;

	public Door MyDoor;

	public AudioClip MyClip;

	public Transform Blood;

	public bool WasTriggered;

	public void Update()
	{
		Blood.transform.localScale = Vector3.MoveTowards(Blood.transform.localScale, Vector3.one, Time.deltaTime * 5f);
		if (!WasTriggered && MyDoor.Open)
		{
			base.transform.LookAt(PlayerController.instance.transform);
			if (Vector3.Distance(PlayerController.instance.transform.position, base.transform.position) < 5f)
			{
				AudioSource audioSource = new GameObject("Jumpscare Effect").AddComponent<AudioSource>();
				audioSource.playOnAwake = false;
				audioSource.loop = false;
				audioSource.clip = MyClip;
				audioSource.Play();
				Object.Destroy(audioSource, MyClip.length);
			}
			ParticleSystem[] myParticles = MyParticles;
			for (int i = 0; i < myParticles.Length; i++)
			{
				myParticles[i].Play();
			}
			Rigidbody[] myRBs = MyRBs;
			foreach (Rigidbody obj in myRBs)
			{
				obj.isKinematic = false;
				obj.AddForce(base.transform.forward * 10f + Vector3.up * 5f, ForceMode.Impulse);
			}
			WasTriggered = true;
		}
	}
}
