using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractableManager : MonoBehaviour
{
	[Header("Interactable References")]
	public TMP_Text InteractableLabel;

	public RawImage InteractableKeyPrompt;

	[Header("Interactable Settings")]
	public LayerMask TargetLayers;

	public float ViewDistance;

	public void Update()
	{
		if (!PlayerController.instance.LockCamera)
		{
			Debug.DrawLine(base.transform.position, base.transform.position + base.transform.forward * ViewDistance, Color.green);
			if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.forward * ViewDistance, out var hitInfo, TargetLayers) && hitInfo.collider.gameObject.layer == 10)
			{
				IInteractable component = hitInfo.collider.gameObject.GetComponent<IInteractable>();
				if (component != null)
				{
					if (component as PowerControl != null && (component as PowerControl).isActive)
					{
						Visible(isVisible: false);
						return;
					}
					InteractableLabel.text = component.Text;
					Visible(isVisible: true);
					if (InputManager.instance.A)
					{
						component.Interact();
					}
					return;
				}
			}
		}
		Visible(isVisible: false);
	}

	private void Visible(bool isVisible)
	{
		InteractableKeyPrompt.color = new Color(1f, 1f, 1f, isVisible ? 1 : 0);
		InteractableLabel.color = new Color(1f, 1f, 1f, isVisible ? 1 : 0);
	}
}
