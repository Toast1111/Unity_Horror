using UnityEngine;

public class MapScript : MonoBehaviour
{
	public Camera MapCamera;

	public GameObject Map;

	public Transform Player;

	public RectTransform UIPlayer;

	public RectTransform CanvasRect;

	private int phase;

	public void Update()
	{
		if (phase == 0)
		{
			if (Time.timeScale >= 1f && PlayerController.instance.CanMove && InputManager.instance.Select)
			{
				Map.SetActive(value: true);
				UIPlayer.gameObject.SetActive(value: true);
				MapCamera.enabled = true;
				Time.timeScale = 0f;
				CalculatePlayerPosition();
				phase++;
			}
		}
		else if (phase == 1)
		{
			if (InputManager.instance.Select || InputManager.instance.B)
			{
				phase++;
			}
		}
		else if (phase == 2)
		{
			Map.SetActive(value: false);
			UIPlayer.gameObject.SetActive(value: false);
			MapCamera.enabled = false;
			Time.timeScale = 1f;
			CalculatePlayerPosition();
			phase = 0;
		}
	}

	public void CalculatePlayerPosition()
	{
		Vector2 vector = MapCamera.WorldToViewportPoint(Player.transform.position);
		Vector2 anchoredPosition = new Vector2(vector.x * CanvasRect.sizeDelta.x - CanvasRect.sizeDelta.x * 0.5f, vector.y * CanvasRect.sizeDelta.y - CanvasRect.sizeDelta.y * 0.5f);
		UIPlayer.anchoredPosition = anchoredPosition;
		UIPlayer.transform.eulerAngles = new Vector3(0f, 0f, 360f - Player.transform.eulerAngles.y);
	}
}
