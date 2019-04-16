using UnityEngine;

// Connects to object "Game Instance"
public class InstanceResources : MonoBehaviour
{
	public SetEnvironment env;				// Other child from the same parent
	public GameObject player, g1, g2;       // Cubes
	public GameObject platform;

	[HideInInspector]
	public GameObject goal, killer;         // Goal and killer, selected at runtime
	[HideInInspector]
	public Renderer r1, r2;					// Renderer of cubes, contains materials of cubes
	[HideInInspector]
	public Rigidbody playerRigidbody;       // Player's rigidbody
	[HideInInspector]
	public Transform playerTransform;       // Player's transform
	[HideInInspector]
	public Controls playerControls;         // Player's control script

	void Awake()
	{
		SetPlayerReferences();
	}

	private void SetPlayerReferences()
	{
		r1 = g1.GetComponent<Renderer>();
		r2 = g2.GetComponent<Renderer>();
		playerRigidbody = player.GetComponent<Rigidbody>();
		playerTransform = player.GetComponent<Transform>();
		playerControls = player.GetComponent<Controls>();
	}
}