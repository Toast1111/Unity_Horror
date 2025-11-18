using System;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public static PlayerController instance;

	[Header("Controller Settings")]
	public CharacterController MyController;

	public Transform Head;

	[SerializeField]
	private float WalkSpeed = 1f;

	[SerializeField]
	private float RunSpeed = 4f;

	[SerializeField]
	private float CrouchWalkSpeed = 0.75f;

	[SerializeField]
	private float CrouchRunSpeed = 2f;

	[Header("Animation Settings")]
	public Animation MyAnimation;

	[SerializeField]
	private string IdleAnim;

	[SerializeField]
	private string WalkAnim;

	[SerializeField]
	private string RunAnim;

	[Header("Camera Settings")]
	public Camera MainCamera;

	[SerializeField]
	private float CameraHeight = 1.64f;

	[SerializeField]
	private float NormalCameraHeight = 1.64f;

	[SerializeField]
	private float CrouchingCameraHeight = 0.752f;

	[SerializeField]
	private float Sensitivity = 3f;

	[SerializeField]
	private float ZoomFOV = 50f;

	[SerializeField]
	private float ZoomSpeed = 10f;

	[SerializeField]
	private Range VerticalRange = new Range(-70f, 70f);

	[Header("Debug Info")]
	[Tooltip("Set true to render extra data via PlayerInputDiagnostics.")]
	public bool EnableDebugStats;

	[NonSerialized]
	public float DebugAxisVertical;

	[NonSerialized]
	public float DebugAxisHorizontal;

	[NonSerialized]
	public Vector3 DebugMoveDirection;

	[NonSerialized]
	public Vector3 DebugPlanarVelocity;

	[NonSerialized]
	public float DebugVerticalVelocity;

	[NonSerialized]
	public bool DebugIsRunning;

	[NonSerialized]
	public bool DebugControllerGrounded;

	[NonSerialized]
	public InputDeviceType DebugDeviceType;

	[NonSerialized]
	public bool DebugUsingController;

	[Header("State Settings")]
	public AudioSource CrumpleSource;

	public Zone CurrentZone;

	public bool LockCamera;

	public bool LockCursor;

	public bool CanMove;

	public bool CanRun;

	public bool CanCrouch;

	private float v;

	private float h = 90f;

	public bool isCrouching;

	private float timer;

	public Vector3 LastRunPosition;

	public Transform LastRunSpot;

	public Collectable CurrentKey;

	public Door CurrentLocker;

	public TMP_Text TripWireLabel;

	private float hurtTimer;

	private bool isHurt;

	private bool isTripped;

	private float verticalVelocity;

	private void OnEnable()
	{
		instance = this;
	}

	private void OnDisable()
	{
		instance = null;
	}

	public void Hurt()
	{
		hurtTimer = 0f;
		isHurt = true;
		CanRun = false;
	}

	public void Trip()
	{
		isTripped = true;
		CanMove = false;
		LockCamera = true;
		CrumpleSource.Play();
	}

	private void Start()
	{
		LastRunSpot = new GameObject("LastRunSpot").transform;
	}

	private void Update()
	{
		if (!LockCamera)
		{
			UpdateCamera();
		}
		else
		{
			CameraFollowHead();
		}
		if (CanMove)
		{
			UpdateMovement();
		}
		if (isTripped)
		{
			UpdateTripping();
		}
		TripWireLabel.color = new Color(1f, 1f, 1f, Mathf.MoveTowards(TripWireLabel.color.a, isTripped ? 1f : 0f, Time.deltaTime * 20f));
	}

	private void UpdateTripping()
	{
		if (YandereScript.instance.CurrentState != YandereScript.State.Murder)
		{
			MyAnimation.CrossFade("trip_00");
			if (MyAnimation["trip_00"].time >= MyAnimation["trip_00"].length)
			{
				isTripped = false;
				CanMove = true;
				LockCamera = false;
			}
		}
		else
		{
			isTripped = false;
		}
	}

	private void CameraFollowHead()
	{
		MainCamera.transform.position = Head.position;
		MainCamera.transform.rotation = Head.rotation;
	}

	private void UpdateMovement()
	{
		if (!(Time.timeScale > 0f))
		{
			return;
		}
		if (MyController.isGrounded && verticalVelocity < 0f)
		{
			verticalVelocity = -2f;
		}
		DebugControllerGrounded = MyController.isGrounded;
		if (isHurt)
		{
			hurtTimer += Time.deltaTime;
			if (hurtTimer >= 5f)
			{
				isHurt = false;
				CanRun = true;
				hurtTimer = 0f;
			}
		}
		float axisRaw = Input.GetAxisRaw("Vertical");
		float axisRaw2 = Input.GetAxisRaw("Horizontal");
		DebugAxisVertical = axisRaw;
		DebugAxisHorizontal = axisRaw2;
		DebugDeviceType = ((InputManager.instance != null) ? InputManager.instance.DeviceType : InputDeviceType.MouseAndKeyboard);
		DebugUsingController = InputManager.instance != null && InputManager.instance.isController;
		Vector3 planarVelocity = Vector3.zero;
		DebugMoveDirection = Vector3.zero;
		DebugPlanarVelocity = Vector3.zero;
		DebugIsRunning = false;
		if (InputManager.instance.RS)
		{
			isCrouching = !isCrouching;
			if (isCrouching)
			{
				MyController.height = 0.9f;
				MyController.center = new Vector3(0f, 0.475f, 0f);
			}
			else
			{
				MyController.height = 1.75f;
				MyController.center = new Vector3(0f, 0.9f, 0f);
			}
		}
		if (axisRaw != 0f || axisRaw2 != 0f)
		{
			bool flag = InputManager.instance.LB && CanRun && axisRaw >= 0f;
			float num = (flag ? RunSpeed : WalkSpeed);
			string animation = (flag ? RunAnim : WalkAnim);
			if (isCrouching)
			{
				num = (flag ? CrouchRunSpeed : CrouchWalkSpeed);
			}
			Vector3 moveDir = base.transform.forward * axisRaw + base.transform.right * axisRaw2;
			if (moveDir.sqrMagnitude > 1f)
			{
				moveDir.Normalize();
			}
			planarVelocity = moveDir * num;
			DebugMoveDirection = moveDir;
			DebugPlanarVelocity = planarVelocity;
			DebugIsRunning = flag;
			if (flag && !isCrouching && !YandereScript.instance.GoToNewDoor)
			{
				LastRunPosition = base.transform.position;
			}
			timer += Time.deltaTime * ((!isCrouching) ? (flag ? 20f : 10f) : (flag ? 15f : 10f));
			MainCamera.transform.position += new Vector3(Mathf.Cos(timer * -0.5f) * 0.05f, Mathf.Sin(timer) * 0.05f, 0f);
			if (!MyAnimation.IsPlaying(animation))
			{
				MyAnimation.CrossFade(animation);
			}
		}
		else if (!MyAnimation.IsPlaying(IdleAnim))
		{
			MyAnimation.CrossFade(IdleAnim);
		}
		verticalVelocity += Physics.gravity.y * Time.deltaTime;
		DebugVerticalVelocity = verticalVelocity;
		Vector3 totalVelocity = planarVelocity;
		totalVelocity.y = verticalVelocity;
		MyController.Move(totalVelocity * Time.deltaTime);
	}

	private void UpdateCamera()
	{
		if (Time.timeScale != 0f)
		{
			if (LockCursor && Cursor.lockState != CursorLockMode.Locked)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			MainCamera.fieldOfView = Mathf.MoveTowards(MainCamera.fieldOfView, InputManager.instance.RT ? ZoomFOV : 60f, Time.deltaTime * ZoomSpeed);
			v += (0f - Input.GetAxis("Mouse Y")) * Sensitivity;
			h += Input.GetAxis("Mouse X") * Sensitivity;
			v = Mathf.Clamp(v, VerticalRange.min, VerticalRange.max);
			CameraHeight = Mathf.Lerp(CameraHeight, isCrouching ? CrouchingCameraHeight : NormalCameraHeight, Time.deltaTime * 15f);
			MainCamera.transform.eulerAngles = new Vector3(v, base.transform.eulerAngles.y, 0f);
			MainCamera.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + CameraHeight, base.transform.position.z);
			base.transform.eulerAngles = new Vector3(0f, h, 0f);
		}
	}
}
