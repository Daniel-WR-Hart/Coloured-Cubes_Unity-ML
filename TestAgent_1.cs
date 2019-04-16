using UnityEngine;

public abstract partial class TestAgent : Agent
{
	[Header("Specific to TEST")]
	protected static PersistentResources persistent;
	protected InstanceResources instance;
	private const bool FOR_OBSERVATION = true;

	//private Text goalText, killerText, percentText;

	
	// Don't initialize the environment (Start()) until all
	// "instance" & "persistence" references are set (Awake())
	protected void Start()
	{
		DeclareResourceManagers();

		// New environment can only be created after DeclareResourceManagers()
		NewEnvironment();
	}
	private void DeclareResourceManagers()
	{
		persistent = GameObject.Find("Training Grids").GetComponent<PersistentResources>();
		instance = GetComponentInParent<InstanceResources>();
	}

	// Only called from Start() & AgentReset()
	private void NewEnvironment()
	{
		instance.env.NewEnvironment();
	}


	public override void InitializeAgent()
	{

	}
	public override void CollectObservations()
	{
		float[][] allPositions = GetCurrentPositions();
		allPositions = MakePositionsWorldAgnostic(allPositions);
		AddPositionsToObservation(allPositions);
	}
	public override void AgentAction(float[] vectorAction, string textAction)
	{
		//float[] vectorAction is generated inside the Agent class, and its length is set in the Unity editor
		// Create, convert, then execute a decision to move
		float[][] movement = MakeDecision(vectorAction);
		MakeDecisionWorldAgnostic(movement);
		ExecuteDecision(movement);
	}
	public override void AgentReset()
	{
		NewEnvironment();
	}
	public override void AgentOnDone()
	{

	}


	// Only called from CollectObservations()
	private float[][] GetCurrentPositions()
	{
		float[][] allPositions =
		{
			new float[]{ instance.goal.transform.localPosition.x, transform.localPosition.x, instance.killer.transform.localPosition.x },
			new float[]{ instance.goal.transform.localPosition.z, transform.localPosition.z, instance.killer.transform.localPosition.z }
		};

		return allPositions;
	}
	private float[][] MakePositionsWorldAgnostic(float[][] allPositions)
	{
		// Convert goals and positions into World-Agnostic
		Vector3 goalLocation = new Vector3(allPositions[0][0], 0, allPositions[1][0]);
		allPositions = WorldCalculator.ApplySymmetryOptimization(goalLocation, allPositions, FOR_OBSERVATION);

		return allPositions;
	}
	private void AddPositionsToObservation(float[][] allPositions)
	{
		// Add inputs to observations
		for (int c = 0; c < WorldCalculator.POSITION_DIMENSIONS; c++)
			for (int i = 0; i < allPositions[c].Length; i++)
				AddVectorObs(allPositions[c][i]);

		SetTextObs("Testing " + gameObject.GetInstanceID());
	}


	// Only called from AgentAction()
	private float[][] MakeDecision(float[] vectorAction)
	{
		float[][] movement = new float[WorldCalculator.POSITION_DIMENSIONS][];
		// x movement
		movement[0] = new float[1];
		// z movement
		movement[1] = new float[1];

		//float[] action = new float[] { 0, 0 };

		//Minor reward for getting closer to the goal and further from the killer
		if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
		{

			/* NOTE ABOUT CONTINUOUS VS DISCRETE */
			/* 
			Continuous: "Space Size" gives the dimension of "float[] vectorAction". It says nothing about the possible
			values of each element.
			Discrete: "Space Size" gives the size of the lowest subset of non-negative integers. This set is the
			action space of a 1-dimensional "float vectorAction[]". This is most useful for actions that should
			exist on a discrete spectrum, like the magnitude and direction of a finite set of horizontal movements.

			I'm not sure why or how GridWorld managed to use discrete actions. I'm guessing it worked because there
			are many different ways to reach a goal

			*/

			/* NOTE ABOUT CHOSEN ACTIONS */
			/* vectorAction[#] needs to be clamped since any range of float values can be returned by it.
			I tested this with Debug.Log(vectorAction[0] on an internal model and got values mostly in (-3, 3), with
			several instances of values close to 0.

			A clamp in [-1, 1], multiplied by a constant, will only determine the maximum stirde per cycle. The AI seems
			to have a hard time learning to move perfectly diagonally and minimizing wasted movement when it makes very frequent decisions.
			It seems to prefer curved paths over diagonal ones, but that might have to do with maximizing its points by moving towards
			green and away from red. Increasing MOVEMENT_PENALTY may be enough to encourage paths that are closer to diagonal.

			The only way to see it move perfectly diagonally is to reduce the decision frequency and watch it repeat its last action,
			which will most likely have a non-zero x and z component.

			When STRIDE_LENGTH is high it's hard to notice when it takes small steps on purpose, but one good example is to watch how
			small it can move horizontally when trying to sneak vertically between a red and the edge.
			*/
			//Controls

			// clamp movement values in [-1, 1]
			movement[0][0] = Mathf.Clamp(vectorAction[0], -1, 1);
			movement[1][0] = Mathf.Clamp(vectorAction[1], -1, 1);

			//default movement range will naturally be [-1, 1], since vectorAction range is [-10, 10]
			//movement[0][0] = vectorAction[0]/10;
			//movement[1][0] = vectorAction[1]/10;

			//movement[0][0] = vectorAction[0] / 50;
			//movement[1][0] = vectorAction[1] / 50;

			/*int moveScale = 10;
			if (CounterManager.percentResult > 60f)
				moveScale = 20;
			else if (CounterManager.percentResult > 65f)
				moveScale = 30;
			else if (CounterManager.percentResult > 70f)
				moveScale = 40;
			else if (CounterManager.percentResult > 80f)
				moveScale = 50;
			else if (CounterManager.percentResult > 85f)
				moveScale = 60;
			else if (CounterManager.percentResult > 90f)
				moveScale = 70;
			else if (CounterManager.percentResult > 95f)
				moveScale = 80;
			else if (CounterManager.percentResult > 98f)
				moveScale = 90;
			else if (CounterManager.percentResult > 99f)
				moveScale = 100;
			
			movement[0][0] = vectorAction[0] / moveScale;
			movement[1][0] = vectorAction[1] / moveScale;*/

			//Debug.Log("vectorAction[0 & 1]" + vectorAction[0] + " " + vectorAction[1]);


			//			MoveForce(x, z);

			/* NOTE ABOUT MOVEMENT POINTS */
			/* To some degree the player will learn that going towards goal is better than 
			
			The reward for moving towards green should be greater than the punishment for moving towards red so that the player
			won't be afraid to approach a blocked green. There should also be a flat punishment for movement in general to discourage
			the player from taking needlessly long paths, or from hovering around green to rack up proximity points. 

			Keep in mind that making this value larger will make everything else look smaller by comparison.*/
		}
		else
		{
			int discreteAction = Mathf.FloorToInt(vectorAction[0]);

			Vector3 targetPos = transform.position;
			if (discreteAction == 0)
			{
				movement[0][0] = -1;
			}

			if (discreteAction == 1)
			{
				movement[1][0] = -1;
			}

			if (discreteAction == 2)
			{
				movement[0][0] = 1;
			}

			if (discreteAction == 3)
			{
				movement[1][0] = 1;
			}
		}

		return movement;
	}
	private float[][] MakeDecisionWorldAgnostic(float[][] movement)
	{
		movement = WorldCalculator.ApplySymmetryOptimization(instance.goal.transform.localPosition, movement, !FOR_OBSERVATION);
		return movement;
	}
	private void ExecuteDecision(float[][] movement)
	{
		Vector3 movementV3 = MoveWithTransform(movement[0][0], movement[1][0]);
		//Vector3 movementV3 = MoveWithTransformStepFunction(movement[0][0], movement[1][0]);
		//Vector3 movementV3 = MoveWithTransform_8Directions(movement[0][0], movement[1][0]);
		MovementPenalty(movementV3);
	}
	

	// Only called from ExecuteDecision()
	protected abstract Vector3 MoveWithTransform(float x, float z);
	protected abstract Vector3 MoveWithTransformStepFunction(float x, float z);
	protected abstract Vector3 MoveWithTransform_8Directions(float x, float z);
	protected abstract void MovementPenalty(Vector3 movement);
}