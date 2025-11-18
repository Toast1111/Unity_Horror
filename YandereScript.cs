using FIMSpace.FLook;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class YandereScript : MonoBehaviour
{
	public enum State
	{
		Patrol = 0,
		Distracted = 1,
		Chase = 2,
		DisableElectricity = 3,
		HideKey = 4,
		Murder = 5
	}

	public static YandereScript instance;

	public bool IsStatic;

	[Header("Animation Settings")]
	public YandereSFXSpawner SFXSpawner;

	public FLookAnimator LookAt;

	public SkinnedMeshRenderer MyRenderer;

	public SkinnedMeshRenderer KunRenderer;

	public Animation MyAnimation;

	public Transform RightShoulder;

	public GameObject Knife;

	public string YandereAnim = "f02_yanderePose_00";

	public string HoldKnifeAnim = "f02_holdKnife_00";

	public string IdleAnim = "f02_idle_00";

	public string WalkAnim = "f02_walk_00";

	public string RunAnim = "f02_sprint_00";

	public string SpyAnim = "f02_spying_00";

	public string PeekAnim = "f02_cornerPeek_00";

	[Header("Pathfinding Settings")]
	public NavMeshAgent Pathfinding;

	public State CurrentState;

	public float LerpingSpeed = 10f;

	public float WalkSpeed = 1f;

	public float RunSpeed = 4f;

	[Space(20f)]
	public int PatrolPhase;

	public Transform[] PatrolSpots;

	public float lastSeenTimer;

	[Space(20f)]
	public HidingSpot CurrentHidingSpot;

	public PowerControl PowerController;

	public Transform PowerControlSpot;

	[Space(20f)]
	public TMP_Text CurrentKeyLabel;

	public Vector3 KeyHidingSpot;

	[Header("Witness Properties")]
	public Transform Eyes;

	public LayerMask TargetLayers;

	public bool canSee = true;

	public float VisionDistance = 20f;

	public float FOV = 90f;

	[Header("Other Properties")]
	public AudioSource HeartbeatSource;

	public AudioSource CreepySFXSource;

	public Texture2D ChanUniformWNoiseLessShoes;

	public Texture2D KunUniformWNoiseLessShoes;

	public NotificationShower StolenShower;

	public NotificationShower KnifeShower;

	[Header("Caught Properties")]
	public GameObject GameOverCamera;

	public RawImage GameOverCameraSprite;

	public RawImage FadeSprite;

	public TMP_Text GameOverLabel;

	public TMP_Text RestartLabel;

	public TMP_Text TitleScreen;

	[Header("Runtime Properties")]
	public bool canMove;

	public bool isPlayingOverlayAnimations;

	public bool GoToLastSeen;

	public bool GoToNewDoor;

	public bool HasPlayedSFX;

	public bool isAggressive;

	public Collectable currentKey;

	public Zone CurrentZone;

	public float boredomTimer;

	[Header("Knife Properties")]
	public GameObject KnifePrefab;

	public Transform[] KnifeSpawnSpots;

	private float KnifeTimer;

	private void OnEnable()
	{
		instance = this;
	}

	private void OnDisable()
	{
		instance = null;
	}

	private void Start()
	{
		if (!IsStatic)
		{
			EnableOverlayAnimation(flag: true);
			if (GameSettings.HardMode)
			{
				RunSpeed = 4.85f;
			}
			if (GameSettings.KunMode)
			{
				MyRenderer = KunRenderer;
			}
		}
	}

	private void EnableOverlayAnimation(bool flag)
	{
		if (flag)
		{
			MyAnimation[YandereAnim].layer = 2;
			MyAnimation.Play(YandereAnim);
			MyAnimation[YandereAnim].weight = 1f;
			MyAnimation[HoldKnifeAnim].layer = 3;
			MyAnimation[HoldKnifeAnim].AddMixingTransform(RightShoulder);
			MyAnimation.Play(HoldKnifeAnim);
			Knife.SetActive(value: true);
			isPlayingOverlayAnimations = true;
		}
		else
		{
			MyAnimation.Stop(YandereAnim);
			MyAnimation[YandereAnim].weight = 0f;
			MyAnimation[HoldKnifeAnim].RemoveMixingTransform(RightShoulder);
			MyAnimation.Stop(HoldKnifeAnim);
			Knife.SetActive(value: false);
			isPlayingOverlayAnimations = false;
		}
	}

	public void TurnAggressive()
	{
		isAggressive = true;
		if (GameSettings.KunMode)
		{
			MyRenderer.materials[0].SetTexture("_MainTex", KunUniformWNoiseLessShoes);
			return;
		}
		MyRenderer.materials[0].SetTexture("_MainTex", ChanUniformWNoiseLessShoes);
		MyRenderer.materials[1].SetTexture("_MainTex", ChanUniformWNoiseLessShoes);
	}

	public bool InSight(Vector3 point)
	{
		Vector3 to = point - Eyes.position;
		return Vector3.Angle(Eyes.forward, to) <= FOV;
	}

	public bool CanSee(GameObject obj, Vector3 targetPoint, bool debug = false)
	{
		if (canSee)
		{
			Debug.DrawLine(Eyes.position, targetPoint, Color.green);
			Vector3 position = Eyes.position;
			Vector3 vector = targetPoint - position;
			float num = Mathf.Pow(VisionDistance, 2f);
			bool num2 = InSight(targetPoint);
			bool flag = vector.sqrMagnitude <= num;
			if (num2 && flag && Physics.Linecast(position, targetPoint, out var hitInfo, TargetLayers))
			{
				if (debug)
				{
					Debug.Log(hitInfo.collider.gameObject.name);
				}
				if (hitInfo.collider.gameObject == obj)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CanSee(GameObject obj, Vector3 targetPoint, LayerMask targetLayers, bool debug = false)
	{
		if (canSee)
		{
			Debug.DrawLine(Eyes.position, targetPoint, Color.green);
			Vector3 position = Eyes.position;
			Vector3 vector = targetPoint - position;
			float num = Mathf.Pow(VisionDistance, 2f);
			bool num2 = InSight(targetPoint);
			bool flag = vector.sqrMagnitude <= num;
			if (num2 && flag && Physics.Linecast(position, targetPoint, out var hitInfo, targetLayers))
			{
				if (debug)
				{
					Debug.Log(hitInfo.collider.gameObject.name);
				}
				if (hitInfo.collider.gameObject == obj)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void Update()
	{
		if (!IsStatic)
		{
			UpdateRoutine();
		}
	}

	private void UpdateRoutine()
	{
		Pathfinding.isStopped = !canMove;
		if (!Pathfinding.isStopped)
		{
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(0f, Quaternion.LookRotation(Pathfinding.velocity.normalized).eulerAngles.y, 0f), 0.1f);
		}
		switch (CurrentState)
		{
		case State.Patrol:
			Patrol();
			break;
		case State.Distracted:
			Distracted();
			break;
		case State.Chase:
			Chase();
			break;
		case State.DisableElectricity:
			DisableElectricity();
			break;
		case State.HideKey:
			HideKey();
			break;
		case State.Murder:
			Murder();
			break;
		}
		if (Vector3.Distance(PlayerController.instance.transform.position, base.transform.position) < 0.2f || (Vector3.Distance(PlayerController.instance.transform.position, base.transform.position) < 0.75f && CanSee(PlayerController.instance.MainCamera.gameObject, PlayerController.instance.MainCamera.transform.position)))
		{
			PlayerController.instance.LockCamera = true;
			PlayerController.instance.CanMove = false;
			canMove = false;
			CurrentState = State.Murder;
		}
		MyAnimation[WalkAnim].speed = WalkSpeed;
		if (CurrentState == State.Murder || CurrentState == State.HideKey)
		{
			return;
		}
		if (CanSee(PlayerController.instance.MainCamera.gameObject, PlayerController.instance.MainCamera.transform.position) || (Mathf.Abs(PlayerController.instance.MyController.velocity.z) > 1f && !PlayerController.instance.isCrouching && Vector3.Distance(base.transform.position, PlayerController.instance.transform.position) < 5f) || ((isAggressive || GameSettings.HardMode) && Mathf.Abs(PlayerController.instance.MyController.velocity.z) > 1f && !PlayerController.instance.isCrouching && Vector3.Distance(base.transform.position, PlayerController.instance.transform.position) < 10f) || (Vector3.Distance(base.transform.position, PlayerController.instance.transform.position) < 3f && Mathf.Abs(PlayerController.instance.MyController.velocity.z) >= 1f && !PlayerController.instance.isCrouching) || ((isAggressive || GameSettings.HardMode) && Vector3.Distance(base.transform.position, PlayerController.instance.transform.position) < 5f && Mathf.Abs(PlayerController.instance.MyController.velocity.z) >= 1f && !PlayerController.instance.isCrouching))
		{
			if (CurrentState != State.Chase && CurrentState != State.HideKey)
			{
				if (PlayerController.instance.CurrentZone.MyType == ZoneType.Classroom)
				{
					PlayerController.instance.CurrentZone.SeenTimes++;
					if (TrapManager.instance != null)
					{
						TrapManager.instance.SortSpottedAreas();
					}
				}
				CurrentState = State.Chase;
			}
			else
			{
				PlayerController.instance.LastRunPosition = PlayerController.instance.transform.position;
			}
		}
		else if (CurrentState != State.HideKey && CurrentHidingSpot == null)
		{
			if (CurrentState == State.Chase)
			{
				CurrentState = State.Distracted;
				GoToLastSeen = true;
			}
			if (Mathf.Abs(PlayerController.instance.MyController.velocity.z) > 1f && !PlayerController.instance.isCrouching && CurrentState != State.Distracted)
			{
				CurrentState = State.Distracted;
			}
		}
	}

	private void HideKey()
	{
		if (WalkSpeed != 1.45f)
		{
			WalkSpeed = 1.45f;
		}
		else if (Pathfinding.speed != RunSpeed)
		{
			Pathfinding.speed = RunSpeed;
		}
		if (KeyHidingSpot == Vector3.zero)
		{
			KeyHidingSpot = ZoneManager.instance.GetAccessableHidingSpot(currentKey);
		}
		if (!isPlayingOverlayAnimations)
		{
			EnableOverlayAnimation(flag: true);
		}
		if (HeartbeatSource.isPlaying)
		{
			HeartbeatSource.Stop();
		}
		KnifeTimer = 0f;
		boredomTimer = 0f;
		if (Vector3.Distance(base.transform.position, KeyHidingSpot) > 1f)
		{
			if (Pathfinding.destination != KeyHidingSpot)
			{
				Pathfinding.SetDestination(KeyHidingSpot);
			}
			if (Pathfinding.velocity == Vector3.zero)
			{
				Pathfinding.enabled = false;
				Pathfinding.enabled = true;
			}
			else if (!MyAnimation.IsPlaying(RunAnim))
			{
				MyAnimation.CrossFade(RunAnim);
			}
			if (!canMove)
			{
				canMove = true;
			}
		}
		else
		{
			currentKey.gameObject.SetActive(value: true);
			KeyHidingSpot = Vector3.zero;
			if (isAggressive || GameSettings.HardMode)
			{
				PlayerController.instance.LastRunPosition = PlayerController.instance.transform.position;
			}
			CurrentState = ((isAggressive || GameSettings.HardMode) ? State.Distracted : State.Patrol);
		}
	}

	private void DisableElectricity()
	{
		if (WalkSpeed != 1.45f)
		{
			WalkSpeed = 1.45f;
		}
		if (Pathfinding.speed != WalkSpeed && !isAggressive && !GameSettings.HardMode)
		{
			Pathfinding.speed = WalkSpeed;
		}
		else if (Pathfinding.speed != RunSpeed && (isAggressive || GameSettings.HardMode))
		{
			Pathfinding.speed = RunSpeed;
		}
		if (!isPlayingOverlayAnimations)
		{
			EnableOverlayAnimation(flag: true);
		}
		if (HeartbeatSource.isPlaying)
		{
			HeartbeatSource.Stop();
		}
		KnifeTimer = 0f;
		boredomTimer = 0f;
		if (Vector3.Distance(base.transform.position, PowerControlSpot.position) > 1f)
		{
			if (Pathfinding.destination != PowerControlSpot.position)
			{
				Pathfinding.SetDestination(PowerControlSpot.position);
			}
			if (!MyAnimation.IsPlaying(WalkAnim) && !isAggressive && !GameSettings.HardMode)
			{
				MyAnimation.CrossFade(WalkAnim);
			}
			else if (!MyAnimation.IsPlaying(RunAnim) && (isAggressive || GameSettings.HardMode))
			{
				MyAnimation.CrossFade(RunAnim);
			}
			if (!canMove)
			{
				canMove = true;
			}
			return;
		}
		if (!MyAnimation.IsPlaying(IdleAnim))
		{
			MyAnimation[IdleAnim].time = 0f;
			MyAnimation.CrossFade(IdleAnim);
			StartCoroutine(PowerController.Switch(PowerOn: false));
		}
		if (canMove)
		{
			canMove = false;
		}
		if (MyAnimation[IdleAnim].time >= MyAnimation[IdleAnim].length)
		{
			if (isAggressive || GameSettings.HardMode)
			{
				PlayerController.instance.LastRunPosition = PlayerController.instance.transform.position;
			}
			CurrentState = ((isAggressive || GameSettings.HardMode) ? State.Distracted : State.Patrol);
		}
	}

	private void Murder()
	{
		PlayerController.instance.transform.LookAt(base.transform.position);
		base.transform.LookAt(PlayerController.instance.transform.position);
		base.transform.position = PlayerController.instance.transform.position + -base.transform.forward * 1.1f;
		if (LookAt.enabled)
		{
			LookAt.enabled = false;
		}
		if (isPlayingOverlayAnimations)
		{
			EnableOverlayAnimation(flag: false);
		}
		if (HeartbeatSource.isPlaying)
		{
			HeartbeatSource.Stop();
		}
		if (!GameOverCamera.activeInHierarchy)
		{
			GameOverCamera.SetActive(value: true);
		}
		GameOverCameraSprite.color = new Color(1f, 1f, 1f, Mathf.MoveTowards(GameOverCameraSprite.color.a, 1f, Time.deltaTime * 5f));
		Knife.SetActive(value: true);
		PlayerController.instance.MyAnimation.CrossFade("snapDie_00");
		MyAnimation.CrossFade("f02_snapKill_00");
		SFXSpawner.Murder();
		if (PlayerController.instance.MyAnimation["snapDie_00"].time >= 9.12f)
		{
			FadeSprite.color = new Color(0f, 0f, 0f, Mathf.MoveTowards(FadeSprite.color.a, 1f, Time.deltaTime * 1f));
			if (PlayerController.instance.MyAnimation["snapDie_00"].time >= PlayerController.instance.MyAnimation["snapDie_00"].length)
			{
				SceneManager.LoadScene("SchoolScene");
			}
		}
	}

	private void Patrol()
	{
		if (Pathfinding.speed != WalkSpeed)
		{
			Pathfinding.speed = WalkSpeed;
		}
		if (WalkSpeed != 1.2f && !isAggressive && !GameSettings.HardMode)
		{
			WalkSpeed = 1.2f;
		}
		else if (WalkSpeed != 1.5f && (isAggressive || GameSettings.HardMode))
		{
			WalkSpeed = 1.5f;
		}
		if (!isPlayingOverlayAnimations)
		{
			EnableOverlayAnimation(flag: true);
		}
		if (HeartbeatSource.isPlaying)
		{
			HeartbeatSource.Stop();
		}
		if (HasPlayedSFX)
		{
			HasPlayedSFX = false;
		}
		KnifeTimer = 0f;
		boredomTimer = 0f;
		lastSeenTimer += Time.deltaTime;
		if (lastSeenTimer > 60f || ((isAggressive || GameSettings.HardMode) && lastSeenTimer > 30f))
		{
			PlayerController.instance.LastRunPosition = PlayerController.instance.transform.position;
			CurrentState = State.Distracted;
			lastSeenTimer = 0f;
		}
		else if (Vector3.Distance(base.transform.position, PatrolSpots[PatrolPhase].position) > 1f)
		{
			if (Pathfinding.destination != PatrolSpots[PatrolPhase].position)
			{
				Pathfinding.SetDestination(PatrolSpots[PatrolPhase].position);
			}
			if (!MyAnimation.IsPlaying(WalkAnim))
			{
				MyAnimation.CrossFade(WalkAnim);
			}
			if (!canMove)
			{
				canMove = true;
			}
		}
		else
		{
			PatrolPhase++;
			if (PatrolPhase >= PatrolSpots.Length)
			{
				PatrolPhase = 0;
			}
		}
	}

	private void Distracted()
	{
		if (WalkSpeed != 1.45f)
		{
			WalkSpeed = 1.45f;
		}
		if (Pathfinding.speed != RunSpeed)
		{
			Pathfinding.speed = RunSpeed;
		}
		if (!isPlayingOverlayAnimations)
		{
			EnableOverlayAnimation(flag: true);
		}
		if (HeartbeatSource.isPlaying)
		{
			HeartbeatSource.Stop();
		}
		KnifeTimer = 0f;
		if (Vector3.Distance(base.transform.position, PlayerController.instance.LastRunPosition) > 4f || (GoToLastSeen && Vector3.Distance(base.transform.position, PlayerController.instance.LastRunPosition) > 1f))
		{
			if (Pathfinding.destination != PlayerController.instance.LastRunPosition)
			{
				Pathfinding.SetDestination(PlayerController.instance.LastRunPosition);
			}
			if (!MyAnimation.IsPlaying(RunAnim))
			{
				MyAnimation.CrossFade(RunAnim);
			}
			if (!canMove)
			{
				canMove = true;
			}
			return;
		}
		if (!MyAnimation.IsPlaying(IdleAnim))
		{
			MyAnimation[IdleAnim].time = 0f;
			MyAnimation.CrossFade(IdleAnim);
		}
		if (canMove)
		{
			canMove = false;
		}
		if (MyAnimation[IdleAnim].time >= MyAnimation[IdleAnim].length * (GoToNewDoor ? 3f : 1f))
		{
			CurrentState = (PowerController.isActive ? State.DisableElectricity : State.Patrol);
			GoToLastSeen = false;
			GoToNewDoor = false;
		}
		boredomTimer = 0f;
	}

	private void Chase()
	{
		float num = Vector3.Distance(base.transform.position, PlayerController.instance.transform.position);
		if (num <= 4f || (boredomTimer >= 15f && num <= 20f))
		{
			if (Pathfinding.destination != PlayerController.instance.transform.position)
			{
				Pathfinding.SetDestination(PlayerController.instance.transform.position);
			}
			if (!MyAnimation.IsPlaying(RunAnim))
			{
				MyAnimation.CrossFade(RunAnim);
			}
			if (!canMove)
			{
				canMove = true;
			}
			if (!isPlayingOverlayAnimations)
			{
				EnableOverlayAnimation(flag: true);
			}
			if (CurrentHidingSpot != null)
			{
				CurrentHidingSpot = null;
			}
			if (Pathfinding.speed != RunSpeed)
			{
				Pathfinding.speed = RunSpeed;
			}
			if (!HeartbeatSource.isPlaying)
			{
				HeartbeatSource.Play();
			}
			HeartbeatSource.volume = Mathf.Abs(1f - num / 20f);
			if (!PlayerController.instance.CanRun || MyRenderer.isVisible || !(num > 8f))
			{
				return;
			}
			KnifeTimer += Time.deltaTime;
			if (KnifeTimer >= 8f)
			{
				KnifeShower.Show(4f);
				Object.Instantiate(KnifePrefab, KnifeSpawnSpots[0].position, KnifeSpawnSpots[0].rotation).GetComponent<KnifeScript>().delay = 0f;
				Object.Instantiate(KnifePrefab, KnifeSpawnSpots[1].position, KnifeSpawnSpots[1].rotation).GetComponent<KnifeScript>().delay = 0.1f;
				Object.Instantiate(KnifePrefab, KnifeSpawnSpots[2].position, KnifeSpawnSpots[2].rotation).GetComponent<KnifeScript>().delay = 0.2f;
				if (!GameSettings.KunMode)
				{
					YandereSFXSpawner.instance.PlayDodgeClip();
				}
				KnifeTimer = 0f;
			}
			return;
		}
		if (num <= 5f)
		{
			if (Pathfinding.destination != PlayerController.instance.transform.position)
			{
				Pathfinding.SetDestination(PlayerController.instance.transform.position);
			}
			if (!MyAnimation.IsPlaying(WalkAnim) && !isAggressive && !GameSettings.HardMode)
			{
				MyAnimation.CrossFade(WalkAnim);
			}
			else if (!MyAnimation.IsPlaying(RunAnim) && (isAggressive || GameSettings.HardMode))
			{
				MyAnimation.CrossFade(RunAnim);
			}
			if (!canMove)
			{
				canMove = true;
			}
			if (!isPlayingOverlayAnimations)
			{
				EnableOverlayAnimation(flag: true);
			}
			if (CurrentHidingSpot != null)
			{
				CurrentHidingSpot = null;
			}
			if (WalkSpeed != 1.45f)
			{
				WalkSpeed = 1.45f;
			}
			if (Pathfinding.speed != WalkSpeed && !isAggressive && !GameSettings.HardMode)
			{
				Pathfinding.speed = WalkSpeed;
			}
			else if (Pathfinding.speed != RunSpeed && (isAggressive || GameSettings.HardMode))
			{
				Pathfinding.speed = RunSpeed;
			}
			if (!HeartbeatSource.isPlaying)
			{
				HeartbeatSource.Play();
			}
			HeartbeatSource.volume = Mathf.Abs(1f - num / 20f);
			KnifeTimer = 0f;
			return;
		}
		if (num <= 20f || CurrentHidingSpot != null)
		{
			if (MyRenderer.isVisible)
			{
				if (!HasPlayedSFX)
				{
					CreepySFXSource.Play();
					HasPlayedSFX = true;
				}
				if (!CurrentHidingSpot)
				{
					HidingSpot closest = HidingSpot.GetClosest(PlayerController.instance.CurrentZone.MyType);
					if (closest != null && closest.DistanceToAyano <= 5f && !PlayerController.instance.transform.IsBehind(closest.transform))
					{
						CurrentHidingSpot = closest;
					}
					else
					{
						CurrentHidingSpot = null;
					}
					if (Pathfinding.destination != PlayerController.instance.transform.position)
					{
						Pathfinding.SetDestination(PlayerController.instance.transform.position);
					}
					if (!MyAnimation.IsPlaying(WalkAnim) && !GameSettings.HardMode)
					{
						MyAnimation.CrossFade(WalkAnim);
					}
					else if (!MyAnimation.IsPlaying(RunAnim) && GameSettings.HardMode)
					{
						MyAnimation.CrossFade(RunAnim);
					}
					if (!canMove)
					{
						canMove = true;
					}
					if (!isPlayingOverlayAnimations)
					{
						EnableOverlayAnimation(flag: true);
					}
					if (WalkSpeed != 1.45f)
					{
						WalkSpeed = 1.45f;
					}
					if (Pathfinding.speed != WalkSpeed && !GameSettings.HardMode)
					{
						Pathfinding.speed = WalkSpeed;
					}
					else if (Pathfinding.speed != RunSpeed && GameSettings.HardMode)
					{
						Pathfinding.speed = RunSpeed;
					}
				}
				else
				{
					if (Pathfinding.destination != CurrentHidingSpot.transform.position)
					{
						Pathfinding.SetDestination(CurrentHidingSpot.transform.position);
					}
					if (Vector3.Distance(base.transform.position, CurrentHidingSpot.transform.position) < 1f)
					{
						string animation = ((CurrentHidingSpot.MyType == ZoneType.Classroom) ? SpyAnim : (PeekAnim + (CurrentHidingSpot.Left ? "_L" : "_R")));
						if (!MyAnimation.IsPlaying(animation))
						{
							MyAnimation.CrossFade(animation);
						}
						if (canMove)
						{
							canMove = false;
						}
						if (isPlayingOverlayAnimations)
						{
							EnableOverlayAnimation(flag: false);
						}
						base.transform.position = Vector3.Lerp(base.transform.position, CurrentHidingSpot.transform.position, Time.deltaTime * LerpingSpeed);
						base.transform.eulerAngles = Vector3.Lerp(base.transform.eulerAngles, CurrentHidingSpot.transform.eulerAngles, Time.deltaTime * LerpingSpeed);
						boredomTimer += Time.deltaTime;
					}
					else
					{
						if (!MyAnimation.IsPlaying(RunAnim))
						{
							MyAnimation.CrossFade(RunAnim);
						}
						if (!canMove)
						{
							canMove = true;
						}
						if (!isPlayingOverlayAnimations)
						{
							EnableOverlayAnimation(flag: true);
						}
						if (Pathfinding.speed != RunSpeed)
						{
							Pathfinding.speed = RunSpeed;
						}
					}
					if (CurrentHidingSpot.MyType != PlayerController.instance.CurrentZone.MyType)
					{
						CurrentHidingSpot = null;
					}
					else if (CurrentHidingSpot.MyZone != PlayerController.instance.CurrentZone)
					{
						CurrentHidingSpot = null;
					}
					else if (Vector3.Distance(base.transform.position, PlayerController.instance.transform.position) >= 25f)
					{
						CurrentHidingSpot = null;
					}
					else if (PlayerController.instance.CurrentLocker != null && !CanSee(PlayerController.instance.MainCamera.gameObject, PlayerController.instance.MainCamera.transform.position))
					{
						CurrentHidingSpot = null;
					}
					else if (!PlayerController.instance.transform.IsBehind(CurrentHidingSpot.transform))
					{
						KnifeTimer = 0f;
					}
				}
			}
			else
			{
				if (Pathfinding.destination != PlayerController.instance.transform.position)
				{
					Pathfinding.SetDestination(PlayerController.instance.transform.position);
				}
				if (!MyAnimation.IsPlaying(WalkAnim) && !GameSettings.HardMode)
				{
					MyAnimation.CrossFade(WalkAnim);
				}
				else if (!MyAnimation.IsPlaying(RunAnim) && GameSettings.HardMode)
				{
					MyAnimation.CrossFade(RunAnim);
				}
				if (!canMove)
				{
					canMove = true;
				}
				if (!isPlayingOverlayAnimations)
				{
					EnableOverlayAnimation(flag: true);
				}
				if (CurrentHidingSpot != null)
				{
					CurrentHidingSpot = null;
				}
				if (Pathfinding.speed != WalkSpeed && !isAggressive && !GameSettings.HardMode)
				{
					Pathfinding.speed = WalkSpeed;
				}
				else if (Pathfinding.speed != RunSpeed && (isAggressive || GameSettings.HardMode))
				{
					Pathfinding.speed = RunSpeed;
				}
				if (PlayerController.instance.CanRun)
				{
					KnifeTimer += Time.deltaTime;
					if (KnifeTimer >= 8f)
					{
						KnifeShower.Show(3f);
						Object.Instantiate(KnifePrefab, KnifeSpawnSpots[0].position, KnifeSpawnSpots[0].rotation).GetComponent<KnifeScript>().delay = 0f;
						Object.Instantiate(KnifePrefab, KnifeSpawnSpots[1].position, KnifeSpawnSpots[1].rotation).GetComponent<KnifeScript>().delay = 0.4f;
						Object.Instantiate(KnifePrefab, KnifeSpawnSpots[2].position, KnifeSpawnSpots[2].rotation).GetComponent<KnifeScript>().delay = 0.8f;
						if (!GameSettings.KunMode)
						{
							YandereSFXSpawner.instance.PlayDodgeClip();
						}
						KnifeTimer = 0f;
					}
				}
				boredomTimer = 0f;
			}
			if (!HeartbeatSource.isPlaying && HasPlayedSFX)
			{
				HeartbeatSource.Play();
			}
			HeartbeatSource.volume = Mathf.Abs(1f - num / 20f);
			return;
		}
		if (Pathfinding.destination != PlayerController.instance.transform.position)
		{
			Pathfinding.SetDestination(PlayerController.instance.transform.position);
		}
		if (!MyAnimation.IsPlaying(RunAnim))
		{
			MyAnimation.CrossFade(RunAnim);
		}
		if (!canMove)
		{
			canMove = true;
		}
		if (CurrentHidingSpot != null)
		{
			CurrentHidingSpot = null;
		}
		if (!isPlayingOverlayAnimations)
		{
			EnableOverlayAnimation(flag: true);
		}
		if (Pathfinding.speed != RunSpeed)
		{
			Pathfinding.speed = RunSpeed;
		}
		if (HeartbeatSource.isPlaying)
		{
			HeartbeatSource.Stop();
		}
		if (PlayerController.instance.CanRun)
		{
			KnifeTimer += Time.deltaTime;
			if (KnifeTimer >= 10f)
			{
				KnifeShower.Show(4f);
				Object.Instantiate(KnifePrefab, KnifeSpawnSpots[0].position, KnifeSpawnSpots[0].rotation).GetComponent<KnifeScript>().delay = 0f;
				Object.Instantiate(KnifePrefab, KnifeSpawnSpots[1].position, KnifeSpawnSpots[1].rotation).GetComponent<KnifeScript>().delay = 0.1f;
				Object.Instantiate(KnifePrefab, KnifeSpawnSpots[2].position, KnifeSpawnSpots[2].rotation).GetComponent<KnifeScript>().delay = 0.2f;
				if (!GameSettings.KunMode)
				{
					YandereSFXSpawner.instance.PlayDodgeClip();
				}
				KnifeTimer = 0f;
			}
		}
		boredomTimer = 0f;
	}
}
