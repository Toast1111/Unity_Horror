using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

public class InputManager : MonoBehaviour
{
	public static InputManager instance;

	[Header("Buttons")]
	public bool A;

	public bool B;

	public bool X;

	public bool Y;

	public bool Start;

	public bool Select;

	[Header("Buttons Hold")]
	public bool AHold;

	public bool BHold;

	public bool XHold;

	public bool YHold;

	[Header("Sticks Hold")]
	public bool RSHold;

	[Header("Sticks Pressed")]
	public bool RS;

	public bool LS;

	[Header("Sticks Moved")]
	public bool StickUp;

	public bool StickLeft;

	public bool StickDown;

	public bool StickRight;

	[Header("Left Sticks Moved")]
	public bool LStickUp;

	public bool LStickLeft;

	public bool LStickDown;

	public bool LStickRight;

	[Header("Direction Pad")]
	public bool Up;

	public bool Left;

	public bool Down;

	public bool Right;

	[Header("Triggers")]
	public bool LT;

	public bool LB;

	public bool RT;

	public bool RB;

	[Header("Other")]
	public bool isController;

	private float Dpadtimer;

	private float timer;

	public InputDeviceType DeviceType = InputDeviceType.Gamepad;

	private Vector3 MousePrevious;

	private Vector3 MouseDelta;

	private float Horizontal;

	private float Vertical;

#if ENABLE_INPUT_SYSTEM
	private Keyboard m_Keyboard;
	private Mouse m_Mouse;
	private Gamepad m_Gamepad;
#endif

	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public void OnEnable()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public void OnDisable()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	public bool ConvertToInput(KeyCode myButton, bool hold)
	{
		if (!hold)
		{
			switch (myButton)
			{
			case KeyCode.E:
				return instance.A;
			case KeyCode.Q:
				return instance.B;
			case KeyCode.F:
				return instance.X;
			case KeyCode.R:
				return instance.Y;
			case KeyCode.C:
				return instance.RS;
			}
		}
		else
		{
			switch (myButton)
			{
			case KeyCode.E:
				return instance.AHold;
			case KeyCode.Q:
				return instance.BHold;
			case KeyCode.F:
				return instance.XHold;
			case KeyCode.R:
				return instance.YHold;
			case KeyCode.C:
				return instance.RSHold;
			}
		}
		return false;
	}

	public void Update()
	{
#if ENABLE_INPUT_SYSTEM
		if (TryUpdateWithInputSystem())
		{
			return;
		}
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
		UpdateLegacyInput();
#else
		ResetInputs();
#endif
	}

	private void ResetInputs()
	{
		A = false;
		B = false;
		X = false;
		Y = false;
		Start = false;
		Select = false;
		AHold = false;
		BHold = false;
		XHold = false;
		YHold = false;
		RS = false;
		LS = false;
		RSHold = false;
		StickUp = false;
		StickDown = false;
		StickLeft = false;
		StickRight = false;
		LStickUp = false;
		LStickDown = false;
		LStickLeft = false;
		LStickRight = false;
		Up = false;
		Down = false;
		Left = false;
		Right = false;
		LT = false;
		LB = false;
		RT = false;
		RB = false;
	}

#if ENABLE_INPUT_SYSTEM
	private bool TryUpdateWithInputSystem()
	{
		m_Keyboard = Keyboard.current;
		m_Mouse = Mouse.current;
		m_Gamepad = Gamepad.current;
		if (m_Keyboard == null && m_Mouse == null && m_Gamepad == null)
		{
			return false;
		}
		UpdateDeviceTypeInputSystem();
		UpdateMouseDataInputSystem();
		A = WasPressedThisFrame(m_Keyboard?.eKey) || WasPressedThisFrame(m_Gamepad?.buttonSouth);
		B = WasPressedThisFrame(m_Keyboard?.qKey) || WasPressedThisFrame(m_Gamepad?.buttonEast);
		X = WasPressedThisFrame(m_Keyboard?.fKey) || WasPressedThisFrame(m_Gamepad?.buttonWest);
		Y = WasPressedThisFrame(m_Keyboard?.rKey) || WasPressedThisFrame(m_Gamepad?.buttonNorth);
		AHold = IsPressed(m_Keyboard?.eKey) || IsPressed(m_Gamepad?.buttonSouth);
		BHold = IsPressed(m_Keyboard?.qKey) || IsPressed(m_Gamepad?.buttonEast);
		XHold = IsPressed(m_Keyboard?.fKey) || IsPressed(m_Gamepad?.buttonWest);
		YHold = IsPressed(m_Keyboard?.rKey) || IsPressed(m_Gamepad?.buttonNorth);
		Start = WasPressedThisFrame(m_Gamepad?.startButton) || WasPressedThisFrame(m_Keyboard?.escapeKey);
		Select = WasPressedThisFrame(m_Gamepad?.selectButton) || WasPressedThisFrame(m_Keyboard?.tabKey);
		RS = WasPressedThisFrame(m_Gamepad?.rightStickButton) || WasPressedThisFrame(m_Keyboard?.cKey);
		LS = WasPressedThisFrame(m_Gamepad?.leftStickButton) || WasPressedThisFrame(m_Keyboard?.leftCtrlKey);
		RSHold = IsPressed(m_Gamepad?.rightStickButton) || IsPressed(m_Keyboard?.cKey);
		LB = IsPressed(m_Gamepad?.leftShoulder) || IsPressed(m_Keyboard?.leftShiftKey);
		RB = IsPressed(m_Gamepad?.rightShoulder) || IsPressed(m_Keyboard?.rightShiftKey);
		LT = TriggerPressed(m_Gamepad?.leftTrigger) || (m_Mouse != null && m_Mouse.leftButton.isPressed);
		RT = TriggerPressed(m_Gamepad?.rightTrigger) || (m_Mouse != null && m_Mouse.rightButton.isPressed);
		if (!isController)
		{
			if (!Down)
			{
				Up = WasPressedThisFrame(m_Keyboard?.upArrowKey);
			}
			if (!Up)
			{
				Down = WasPressedThisFrame(m_Keyboard?.downArrowKey);
			}
			if (!Right)
			{
				Left = WasPressedThisFrame(m_Keyboard?.leftArrowKey);
			}
			if (!Left)
			{
				Right = WasPressedThisFrame(m_Keyboard?.rightArrowKey);
			}
		}
		else
		{
			float axisRaw = ReadAxis(m_Gamepad?.dpad?.x);
			float axisRaw2 = ReadAxis(m_Gamepad?.dpad?.y);
			if (Mathf.Abs(axisRaw) == 0f && Mathf.Abs(axisRaw2) == 0f)
			{
				Up = false;
				Down = false;
				Left = false;
				Right = false;
				Dpadtimer = 0.2f;
			}
			else
			{
				Dpadtimer += Time.unscaledDeltaTime;
				if (Dpadtimer > 0.2f)
				{
					Up = axisRaw2 > 0.5f;
					Down = axisRaw2 < -0.5f;
					Left = axisRaw < -0.5f;
					Right = axisRaw > 0.5f;
					if (Up || Down || Left || Right)
					{
						Dpadtimer = 0f;
					}
				}
				else
				{
					Up = false;
					Down = false;
					Left = false;
					Right = false;
				}
			}
		}
		if (!isController)
		{
			if (!StickDown)
			{
				StickUp = WasPressedThisFrame(m_Keyboard?.wKey);
			}
			if (!StickUp)
			{
				StickDown = WasPressedThisFrame(m_Keyboard?.sKey);
			}
			if (!StickRight)
			{
				StickLeft = WasPressedThisFrame(m_Keyboard?.aKey);
			}
			if (!StickLeft)
			{
				StickRight = WasPressedThisFrame(m_Keyboard?.dKey);
			}
		}
		else
		{
			float axisRaw3 = ReadAxis(m_Gamepad?.leftStick?.x);
			float axisRaw4 = ReadAxis(m_Gamepad?.leftStick?.y);
			if (Mathf.Abs(axisRaw3) < 0.01f && Mathf.Abs(axisRaw4) < 0.01f)
			{
				StickUp = false;
				StickDown = false;
				StickLeft = false;
				StickRight = false;
				timer = 0.2f;
			}
			else
			{
				timer += Time.unscaledDeltaTime;
				if (timer > 0.2f)
				{
					StickUp = axisRaw4 > 0.5f;
					StickDown = axisRaw4 < -0.5f;
					StickLeft = axisRaw3 < -0.5f;
					StickRight = axisRaw3 > 0.5f;
					if (StickUp || StickDown || StickLeft || StickRight)
					{
						timer = 0f;
					}
				}
				else
				{
					StickUp = false;
					StickDown = false;
					StickLeft = false;
					StickRight = false;
				}
			}
		}
		float mouseY = m_Mouse != null ? m_Mouse.delta.ReadValue().y : 0f;
		float mouseX = m_Mouse != null ? m_Mouse.delta.ReadValue().x : 0f;
		Vertical = mouseY;
		Horizontal = mouseX;
		LStickUp = Vertical > 0.25f;
		LStickDown = Vertical < -0.25f;
		LStickLeft = Horizontal < -0.25f;
		LStickRight = Horizontal > 0.25f;
		return true;
	}

	private void UpdateDeviceTypeInputSystem()
	{
		bool keyboardActivity = m_Keyboard != null && (m_Keyboard.anyKey.wasPressedThisFrame || m_Keyboard.anyKey.isPressed);
		if (m_Mouse != null)
		{
			keyboardActivity |= m_Mouse.leftButton.wasPressedThisFrame || m_Mouse.rightButton.wasPressedThisFrame || m_Mouse.middleButton.wasPressedThisFrame || m_Mouse.delta.ReadValue() != Vector2.zero;
		}
		bool gamepadActivity = m_Gamepad != null && (
			m_Gamepad.wasUpdatedThisFrame ||
			m_Gamepad.buttonSouth.wasPressedThisFrame ||
			m_Gamepad.buttonNorth.wasPressedThisFrame ||
			m_Gamepad.buttonEast.wasPressedThisFrame ||
			m_Gamepad.buttonWest.wasPressedThisFrame ||
			m_Gamepad.leftStick.ReadValue().sqrMagnitude > 0.001f ||
			m_Gamepad.dpad.ReadValue().sqrMagnitude > 0.001f ||
			m_Gamepad.startButton.wasPressedThisFrame ||
			m_Gamepad.selectButton.wasPressedThisFrame);
		if (gamepadActivity)
		{
			DeviceType = InputDeviceType.Gamepad;
		}
		else if (keyboardActivity)
		{
			DeviceType = InputDeviceType.MouseAndKeyboard;
		}
		isController = DeviceType == InputDeviceType.Gamepad;
	}

	private void UpdateMouseDataInputSystem()
	{
		Vector3 previous = MousePrevious;
		Vector2 mousePosition = (m_Mouse != null) ? m_Mouse.position.ReadValue() : Vector2.zero;
		MousePrevious = new Vector3(mousePosition.x, mousePosition.y, 0f);
		MouseDelta = MousePrevious - previous;
	}

	private static bool WasPressedThisFrame(KeyControl key)
	{
		return key != null && key.wasPressedThisFrame;
	}

	private static bool WasPressedThisFrame(ButtonControl button)
	{
		return button != null && button.wasPressedThisFrame;
	}

	private static bool IsPressed(KeyControl key)
	{
		return key != null && key.isPressed;
	}

	private static bool IsPressed(ButtonControl button)
	{
		return button != null && button.isPressed;
	}

	private static bool TriggerPressed(AxisControl axis)
	{
		return axis != null && axis.ReadValue() > 0.25f;
	}

	private static float ReadAxis(InputControl control)
	{
		if (control is AxisControl axisControl)
		{
			return axisControl.ReadValue();
		}
		return 0f;
	}
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
	private void UpdateLegacyInput()
	{
		UpdateDeviceTypeLegacy();
		A = Input.GetButtonDown("A");
		B = Input.GetButtonDown("B");
		X = Input.GetButtonDown("X");
		Y = Input.GetButtonDown("Y");
		AHold = Input.GetButton("A");
		BHold = Input.GetButton("B");
		XHold = Input.GetButton("X");
		YHold = Input.GetButton("Y");
		Start = Input.GetButtonDown("Start") || Input.GetKeyDown(KeyCode.Escape);
		Select = Input.GetButtonDown("Select");
		RS = Input.GetButtonDown("RS");
		LS = Input.GetButtonDown("LS");
		RSHold = Input.GetButton("RS");
		LB = Input.GetButton("LB");
		RB = Input.GetButton("RB");
		LT = Input.GetAxis("LT") > 0.25f || Input.GetMouseButton(0);
		RT = Input.GetAxis("RT") > 0.25f || Input.GetMouseButton(1);
		if (!isController)
		{
			if (!Down)
			{
				Up = Input.GetKeyDown(KeyCode.UpArrow);
			}
			if (!Up)
			{
				Down = Input.GetKeyDown(KeyCode.DownArrow);
			}
			if (!Right)
			{
				Left = Input.GetKeyDown(KeyCode.LeftArrow);
			}
			if (!Left)
			{
				Right = Input.GetKeyDown(KeyCode.RightArrow);
			}
		}
		else
		{
			float axisRaw = Input.GetAxisRaw("DpadX");
			float axisRaw2 = Input.GetAxisRaw("DpadY");
			if (Mathf.Abs(axisRaw) == 0f && Mathf.Abs(axisRaw2) == 0f)
			{
				Up = false;
				Down = false;
				Left = false;
				Right = false;
				Dpadtimer = 0.2f;
			}
			else
			{
				Dpadtimer += Time.unscaledDeltaTime;
				if (Dpadtimer > 0.2f)
				{
					Up = axisRaw2 == 1f;
					Down = axisRaw2 == -1f;
					Left = axisRaw == -1f;
					Right = axisRaw == 1f;
					if (Up || Down || Left || Right)
					{
						Dpadtimer = 0f;
					}
				}
				else
				{
					Up = false;
					Down = false;
					Left = false;
					Right = false;
				}
			}
		}
		if (!isController)
		{
			if (!StickDown)
			{
				StickUp = Input.GetKeyDown(KeyCode.W);
			}
			if (!StickUp)
			{
				StickDown = Input.GetKeyDown(KeyCode.S);
			}
			if (!StickRight)
			{
				StickLeft = Input.GetKeyDown(KeyCode.A);
			}
			if (!StickLeft)
			{
				StickRight = Input.GetKeyDown(KeyCode.D);
			}
		}
		else
		{
			float axisRaw3 = Input.GetAxisRaw("Horizontal");
			float axisRaw4 = Input.GetAxisRaw("Vertical");
			if (Mathf.Abs(axisRaw3) == 0f && Mathf.Abs(axisRaw4) == 0f)
			{
				StickUp = false;
				StickDown = false;
				StickLeft = false;
				StickRight = false;
				timer = 0.2f;
			}
			else
			{
				timer += Time.unscaledDeltaTime;
				if (timer > 0.2f)
				{
					StickUp = axisRaw4 == 1f;
					StickDown = axisRaw4 == -1f;
					StickLeft = axisRaw3 == -1f;
					StickRight = axisRaw3 == 1f;
					if (StickUp || StickDown || StickLeft || StickRight)
					{
						timer = 0f;
					}
				}
				else
				{
					StickUp = false;
					StickDown = false;
					StickLeft = false;
					StickRight = false;
				}
			}
		}
		Vertical = Input.GetAxis("Mouse Y");
		Horizontal = Input.GetAxis("Mouse X");
		LStickUp = Vertical > 0.25f;
		LStickDown = Vertical < -0.25f;
		LStickLeft = Horizontal < -0.25f;
		LStickRight = Horizontal > 0.25f;
		isController = DeviceType == InputDeviceType.Gamepad;
	}
#endif

	#if ENABLE_LEGACY_INPUT_MANAGER
	private void UpdateDeviceTypeLegacy()
	{
		MouseDelta = Input.mousePosition - MousePrevious;
		MousePrevious = Input.mousePosition;
		_ = DeviceType;
		if ((Input.GetJoystickNames().Length == 0 && Input.anyKey) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) || MouseDelta != Vector3.zero)
		{
			DeviceType = InputDeviceType.MouseAndKeyboard;
		}
		else
		{
			bool flag = false;
			for (int i = 0; i < 20; i++)
			{
				if (Input.GetKey("joystick 1 button " + i))
				{
					flag = true;
					break;
				}
			}
			bool flag2 = Mathf.Abs(Input.GetAxis("DpadX")) > 0.5f || Mathf.Abs(Input.GetAxis("DpadY")) > 0.5f;
			bool flag3 = Mathf.Abs(Input.GetAxis("Vertical")) == 1f || Mathf.Abs(Input.GetAxis("Horizontal")) == 1f;
			if (flag || flag2 || flag3)
			{
				DeviceType = InputDeviceType.Gamepad;
			}
		}
		Horizontal = Input.GetAxis("Horizontal");
		Vertical = Input.GetAxis("Vertical");
	}
	#endif
}
