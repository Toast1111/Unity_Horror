using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsCutsceneScript : MonoBehaviour
{
	public GameObject isFinished;

	private void Start()
	{
		Time.timeScale = 1f;
	}

	private void Update()
	{
		if (InputManager.instance.A || isFinished.activeInHierarchy)
		{
			SceneManager.LoadScene("TitleScene");
		}
	}
}
