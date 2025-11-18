using TMPro;
using UnityEngine;

public class NotificationShower : MonoBehaviour
{
	public TMP_Text Label;

	public float LifeTime;

	public float lifeTimer;

	public bool canStart;

	public int phase;

	public void Show(float lifetime)
	{
		lifeTimer = 0f;
		LifeTime = lifetime;
		canStart = true;
	}

	private void Update()
	{
		if (!canStart)
		{
			return;
		}
		lifeTimer += Time.deltaTime;
		switch (phase)
		{
		case 0:
			Label.color = new Color(Label.color.r, Label.color.g, Label.color.b, Mathf.MoveTowards(Label.color.a, 1f, Time.deltaTime * (Mathf.Pow(LifeTime, 1f) * 4f)));
			if (Label.color.a >= 1f)
			{
				phase++;
			}
			break;
		case 1:
			if (lifeTimer >= 2f * LifeTime / 4f)
			{
				phase++;
			}
			break;
		case 2:
			if (lifeTimer >= 3f * LifeTime / 4f)
			{
				phase++;
			}
			break;
		case 3:
			Label.color = new Color(Label.color.r, Label.color.g, Label.color.b, Mathf.MoveTowards(Label.color.a, 0f, Time.deltaTime * (Mathf.Pow(LifeTime, 1f) * 4f)));
			if (Label.color.a <= 0f)
			{
				phase++;
			}
			break;
		case 4:
			Label.color = new Color(Label.color.r, Label.color.g, Label.color.b, 0f);
			LifeTime = 0f;
			lifeTimer = 0f;
			phase = 0;
			canStart = false;
			break;
		}
		if (YandereScript.instance.CurrentState == YandereScript.State.Murder)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
