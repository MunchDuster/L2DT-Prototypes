using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerSense : MonoBehaviour
{
	public float raycastDist = 5f;
	public LayerMask layerMask;
	public Transform raycastPoint;

	public delegate void OnEvent(Interactable item);
	public OnEvent OnHoverItem;

	[Space(10)]
	[HideInInspector] public Player player;

	public HoverInfo hoverInfo;
	public PlayerUI playerUI;

	private Interactable curHover = null; //Interactable hovering this frame
	private Interactable lastHover = null; //Interactabl hovering last frame
	private GameObject lastHoverObject = null; //Gameobject of lastHover (for quick check)
	private bool isOn = true;

	//Update is called every frame.
	private void Update()
	{
		if (!isOn) return;

		GetCurrentHoverItem();
		UpdateHoverInfo();
		CheckInteract();

		lastHover = curHover;
		lastHoverObject = (curHover != null) ? curHover.gameObject : null;
	}

	//To disable/enable sensing and also disable hover over any curent ineractable."
	public void TurnOn()
	{
		isOn = true;
	}
	public void TurnOff()
	{
		isOn = false;

		if (curHover != null) curHover.EndHover(hoverInfo);

		lastHover = curHover;
		lastHoverObject = curHover.gameObject;

		curHover = null;
	}

	private void UpdateHoverInfo()
	{
		//Skip hover hasn't changed
		if (curHover == lastHover) return;

		//Call event
		if (OnHoverItem != null) OnHoverItem(curHover);

		//Call functions
		if (lastHover != null) lastHover.EndHover(hoverInfo);
		if (curHover != null) curHover.StartHover(hoverInfo);
	}

	private void CheckInteract()
	{
		//Interact if key pressed
		if (Input.GetMouseButtonDown(0) && curHover != null)
		{
			InteractionInfo info = curHover.Interact(player);

			//Display error if fail
			if (!info.success) playerUI.ShowError(info.info);
		}
	}

	private void GetCurrentHoverItem()
	{
		if (Physics.Raycast(raycastPoint.position, raycastPoint.forward, out RaycastHit hit, raycastDist, layerMask))
		{
			Debug.DrawRay(raycastPoint.position, raycastPoint.forward * hit.distance, Color.red);

			if (hit.collider.gameObject != lastHoverObject)
			{
				curHover = hit.collider.gameObject.GetComponentInParent<Interactable>();
			}
		}
		else
		{
			Debug.DrawRay(raycastPoint.position, raycastPoint.forward * raycastDist, Color.green);
			curHover = null;
		}
	}
}