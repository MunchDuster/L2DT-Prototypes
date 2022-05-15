using UnityEngine;

public abstract class Minigame : Interactable
{
	public Transform cameraPoint;

	public delegate void MinigameEvent();
	public event MinigameEvent OnPlayerJoin;
	public event MinigameEvent OnPlayerLeave;
	public event MinigameEvent OnGameUpdate;
	public event MinigameEvent OnGameFixedUpdate;

	protected Player player = null;

	private Vector3 initialLocalPosition;
	private Quaternion initialLocalRotation;

	//Interact is called when the player clicks on this
	public override InteractionInfo Interact(Player player)
	{
		//Player already using,then clicking on it for another reason
		if (this.player != null) return InteractionInfo.Success();

		InteractionInfo info = CheckRequirements(player);

		if (info.success)
		{
			this.player = player;
			StartGame();
		}

		return info;
	}

	//CheckInteract to be filled by child
	protected abstract InteractionInfo CheckRequirements(Player player);

	//Checks whether the player is leaving the game
	private void CheckLeave()
	{
		//Move player camera to cameraPoint
		player.camera.transform.position = cameraPoint.position;
		player.camera.transform.rotation = cameraPoint.rotation;

		if (Input.GetKeyDown(KeyCode.Escape)) EndGame();
	}

	//Handles basic admin starting of game
	private void StartGame()
	{
		//Save the current player camera transform
		initialLocalPosition = player.camera.transform.localPosition;
		initialLocalRotation = player.camera.transform.localRotation;

		//Move player camera to cameraPoint
		player.camera.transform.position = cameraPoint.position;
		player.camera.transform.rotation = cameraPoint.rotation;

		//Unlock the cursor
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		//Disable player movement
		player.movement.enabled = false;

		//Disable the player sense
		player.sensor.TurnOff();

		//Add check for if plaer is trying to leave game
		OnGameUpdate += CheckLeave;

		//Call delegate
		if (OnPlayerJoin != null) OnPlayerJoin();
	}

	//Handles basic admin of finishing a game
	protected void EndGame()
	{
		//Reset camera
		player.camera.transform.localPosition = initialLocalPosition;
		player.camera.transform.localRotation = initialLocalRotation;

		//Lock the cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		//Enable player movement
		player.movement.enabled = true;

		//Eable the player sense
		player.sensor.TurnOn();

		//Remove check for if plaer is trying to leave game
		OnGameUpdate -= CheckLeave;

		//Call delegate
		OnPlayerLeave();

		//Reset player var
		player = null;
	}

	// Update is called every frame
	private void Update()
	{
		if (player != null) OnGameUpdate();
	}

	// FixedUpdate is called every physics update
	private void FixedUpdate()
	{
		if (player != null && OnGameFixedUpdate != null) OnGameFixedUpdate.Invoke();
	}
}