using System.Text;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerInputDiagnostics : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private bool startVisible;

    [SerializeField]
    private KeyCode toggleKey = KeyCode.F1;

    [SerializeField]
    private Vector2 panelOffset = new Vector2(20f, 20f);

    [SerializeField]
    private float panelWidth = 340f;

    private bool isVisible;

    private readonly StringBuilder builder = new StringBuilder(512);

    private void Awake()
    {
        if (player == null)
        {
            player = GetComponent<PlayerController>();
        }
        isVisible = startVisible;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isVisible = !isVisible;
        }
    }

    private void OnGUI()
    {
        if (!isVisible || player == null || InputManager.instance == null || !player.EnableDebugStats)
        {
            return;
        }
        var rect = new Rect(panelOffset.x, panelOffset.y, panelWidth, Screen.height * 0.5f);
        GUILayout.BeginArea(rect, GUI.skin.box);
        GUILayout.BeginVertical();
        builder.Length = 0;
        builder.AppendLine("PLAYER INPUT DIAGNOSTICS");
        builder.AppendLine($"Device: {InputManager.instance.DeviceType} {(InputManager.instance.isController ? "(Controller)" : "(KBM)")}");
        builder.AppendLine($"Axis Raw V/H: {player.DebugAxisVertical:+0.00;-0.00;+0.00} / {player.DebugAxisHorizontal:+0.00;-0.00;+0.00}");
        builder.AppendLine($"Move Dir: {FormatVector(player.DebugMoveDirection)}");
        builder.AppendLine($"Planar Velocity: {FormatVector(player.DebugPlanarVelocity)}");
        builder.AppendLine($"Vertical Velocity: {player.DebugVerticalVelocity:+0.00;-0.00;+0.00}");
        builder.AppendLine($"Running: {player.DebugIsRunning} | Crouching: {player.isCrouching} | Grounded: {player.DebugControllerGrounded}");
        builder.AppendLine($"LB (Run Held): {InputManager.instance.LB} | RS (Crouch Toggle): {InputManager.instance.RS}");
        builder.AppendLine($"CharacterController Velocity: {FormatVector(player.MyController != null ? player.MyController.velocity : Vector3.zero)}");
        builder.AppendLine($"Position: {FormatVector(player.transform.position)}");
        GUILayout.Label(builder.ToString());
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private static string FormatVector(Vector3 value)
    {
        return $"({value.x:+0.00;-0.00;+0.00}, {value.y:+0.00;-0.00;+0.00}, {value.z:+0.00;-0.00;+0.00})";
    }
}
