using UnityEngine;
using UnityEngine.AI;

public class FootstepScript : MonoBehaviour
{
	public NavMeshAgent myAgent;

	public AudioSource MyAudio;

	public AudioClip[] WalkFootsteps;

	public AudioClip[] RunFootsteps;

	public float DownThreshold = 0.02f;

	public float UpThreshold = 0.025f;

	public bool FootUp;

	private void Update()
	{
		if (myAgent != null)
		{
			if (!(YandereScript.instance == null) && YandereScript.instance.isAggressive)
			{
				return;
			}
			if (!FootUp)
			{
				if (base.transform.position.y > myAgent.transform.position.y + UpThreshold)
				{
					FootUp = true;
				}
			}
			else
			{
				if (!(base.transform.position.y < myAgent.transform.position.y + DownThreshold))
				{
					return;
				}
				if (FootUp)
				{
					if (myAgent.speed > 1f)
					{
						MyAudio.clip = RunFootsteps[Random.Range(0, RunFootsteps.Length)];
						MyAudio.volume = 0.5f;
					}
					else
					{
						MyAudio.clip = WalkFootsteps[Random.Range(0, WalkFootsteps.Length)];
						MyAudio.volume = 0.3f;
					}
					MyAudio.Play();
				}
				FootUp = false;
			}
		}
		else if (!FootUp)
		{
			if (base.transform.position.y > PlayerController.instance.transform.position.y + UpThreshold)
			{
				FootUp = true;
			}
		}
		else
		{
			if (!(base.transform.position.y < PlayerController.instance.transform.position.y + DownThreshold))
			{
				return;
			}
			if (FootUp && !PlayerController.instance.isCrouching)
			{
				if (PlayerController.instance.MyController.velocity.z > 1f)
				{
					MyAudio.clip = RunFootsteps[Random.Range(0, RunFootsteps.Length)];
					MyAudio.volume = 0.5f;
				}
				else
				{
					MyAudio.clip = WalkFootsteps[Random.Range(0, WalkFootsteps.Length)];
					MyAudio.volume = 0.3f;
				}
				MyAudio.Play();
			}
			FootUp = false;
		}
	}
}
