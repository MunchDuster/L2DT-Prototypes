using UnityEngine;

public class Player : MonoBehaviour
{
	public string playerName;

	[Header("Attachments")]
	public new Camera camera;
	public PlayerSense sensor;
	public PlayerPickup pickuper;
	public FirstPersonController movement;

	// Start is called before the first frame update
	private void Start()
	{
		sensor.player = this;
		pickuper.player = this;
	}
}