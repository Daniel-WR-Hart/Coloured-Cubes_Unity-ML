using UnityEngine;
//using UnityEngine.UI;

// Connects to object "Player"
public class TestAgentObject : TestAgent
{
	public const float STRIDE_LENGTH = 0.6f;//0.3f; //FORCE = 0.1f;
	private const float GOOD_PROXIMITY = 0.5f, BAD_PROXIMITY = -0.4f;//-0.45f;
	private const float GOAL_REWARD = 20, 
						KILLER_PUNISHMENT = -40,//-40,
						EDGE_PENALTY = -200;//-200;
	private const float SCALED_MOVEMENT_PENALTY = -0.3f, FLAT_MOVEMENT_PENALTY = -0.05f;

	private float invertedSquareGoodDistance, invertedSquareBadDistance, goodDistancePoints, badDistancePoints;


	// I can remove this by adding a condition to check how small "direction" is
	//void OnCollisionEnter(Collision col)


	// Only called from parent's OnTriggerEnter()
	protected override void UpdateFitnessFromEvent(Collider collisionSource)
	{
		if (collisionSource.tag == "Goal")
			AddReward(GOAL_REWARD);
		else if (collisionSource.tag == "Killer")
			AddReward(KILLER_PUNISHMENT);
		else if (collisionSource.tag == "Boundary")
			AddReward(EDGE_PENALTY);
	}


	// Only called from FixedUpdate()
	protected override void UpdateFitnessFromTime()
	{
		// Calculate the points gained and lost based on the inverse squared distance between the cubes
		InvertedSquareDistance();
		//InvertedLinearDistance();

		AddReward(Mathf.Clamp(goodDistancePoints, 0, GOOD_PROXIMITY));
		AddReward(Mathf.Clamp(badDistancePoints, BAD_PROXIMITY, 0));
	}
	protected void InvertedSquareDistance()
	{
		invertedSquareGoodDistance = (transform.localPosition - instance.goal.transform.localPosition).sqrMagnitude;   // min = 1
		goodDistancePoints = GOOD_PROXIMITY / invertedSquareGoodDistance;
		invertedSquareBadDistance = (transform.localPosition - instance.killer.transform.localPosition).sqrMagnitude;  // min = 1
		badDistancePoints = BAD_PROXIMITY / invertedSquareBadDistance;
	}
	protected void InvertedLinearDistance()
	{
		invertedSquareGoodDistance = (transform.localPosition - instance.goal.transform.localPosition).magnitude;   // min = 1
		goodDistancePoints = GOOD_PROXIMITY / invertedSquareGoodDistance;
		invertedSquareBadDistance = (transform.localPosition - instance.killer.transform.localPosition).magnitude;  // min = 1
		badDistancePoints = BAD_PROXIMITY / invertedSquareBadDistance;
	}


	// Only called from AgentAction() -> ExecuteDecision()
	protected override Vector3 MoveWithTransform(float x, float z)
	{
		Vector3 movement = new Vector3(x, 0, z) * STRIDE_LENGTH;
		transform.localPosition += movement;
		//Debug.Log("movement: " + movement);
		return movement;
	}
	protected override Vector3 MoveWithTransformStepFunction(float x, float z)
	{
		//Debug.Log("x before: " + x);
		//Debug.Log("z before: " + z);
		x = StepFunction(x);
		z = StepFunction(z);

		Vector3 movement = new Vector3(x, 0, z) * STRIDE_LENGTH;
		transform.localPosition += movement;
		//Debug.Log("movement: " + movement);
		return movement;
	}
	protected float StepFunction(float p)
	{
		if (p == -1.0f)
			p = -2.00f;
		else if (p < -0.9f)
			p = -1.750f;
		else if (p < -0.8f)
			p = -1.50f;
		else if (p < -0.7f)
			p = -1.25f;
		else if (p < -0.6f)
			p = -1.00f;
		else if (p < -0.5f)
			p = -0.80f;
		else if (p < 0.5f)
			p = 0.00f;
		else if (p < 0.6f)
			p = 0.80f;
		else if (p < 0.7f)
			p = 1.00f;
		else if (p < 0.8f)
			p = 1.25f;
		else if (p < 0.9f)
			p = 1.50f;
		else if (p == 1.00f)
			p = 2.00f;

		return p;
	}
	protected override Vector3 MoveWithTransform_8Directions(float x, float z)
	{
		//x = (x > 0) ? 1 : -1;
		//z = (z > 0) ? 1 : -1;

		// R
		if (x > 0.6f && Mathf.Abs(z) < 0.6f)
		{
			x = 1;
			z = 0;
		}
		// R-U
		else if (x > 0.6f && z > 0.6f)
		{
			x = 0.5f;
			z = 0.5f;
		}
		// U
		else if (Mathf.Abs(x) < 0.6f && z > 0.6f)
		{
			x = 0;
			z = 1;
		}
		// L-U
		else if (x < -0.6f && z > 0.6f)
		{
			x = -0.5f;
			z = 0.5f;
		}
		// L
		else if (x < -0.6f && Mathf.Abs(z) < 0.6f)
		{
			x = -1;
			z = 0;
		}
		// L-D
		else if (x < -0.6f && z < -0.6f)
		{
			x = -0.5f;
			z = -0.5f;
		}
		// D
		else if (Mathf.Abs(x) < 0.6f && z < -0.6f)
		{
			x = 0;
			z = -1;
		}
		// R-D
		else if (x > 0.6f && z < -0.6f)
		{
			x = 0.5f;
			z = -0.5f;
		}
		// No movement
		else
		{
			x = 0;
			z = 0;
		}

		Vector3 movement = new Vector3(x, 0, z) * STRIDE_LENGTH;
		transform.localPosition += movement;
		return movement;
	}
	protected override void MovementPenalty(Vector3 movement)
	{
		// Calculating the punishment for x and z separately will not reward or punish the player for moving diagonally,
		// Calculating with the magnitude of the movement vector should encourage more diagonal movement,
		// but only when it gives an advantage over strict horizontal or vertical movement.

		AddReward(SCALED_MOVEMENT_PENALTY * movement.magnitude + FLAT_MOVEMENT_PENALTY);

		// Only use flat punishment if the magnitude of the movement is fixed, and if there is an option to not move
		//AddReward(FLAT_MOVEMENT_PENALTY);
	}


}