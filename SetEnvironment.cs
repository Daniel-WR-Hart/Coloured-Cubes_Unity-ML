//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

// Connects to object "Game Instance"
public class SetEnvironment : MonoBehaviour
{
	private static PersistentResources persistent;
	private InstanceResources instance;
	// Difficulty starts high then adjusts downward from there
	// The reason is to make it easier to resume training highly trained models
	private static float[] difficultyHardest = { 0.110f, 0.225f, 0.345f, 0.465f, 0.595f, 0.725f, 0.860f, 1.000f };
	private static float[] difficultyEasiest = { 0.310f, 0.580f, 0.650f, 0.720f, 0.790f, 0.860f, 0.930f, 1.000f };
	private static float[] difficultyCurrent = difficultyHardest;
	private const float DIFF_INCREMENT = 0.01f;
	private const float DIFF_MAX = 0.7f;
	private const float DEGREES_180 = 3.14f;
	private const float DEGREES_50 = 0.87f;
	private const float DEGREES_40 = 0.70f;
	private const float DEGREES_30 = 0.52f;
	private const float DEGREES_20 = 0.35f;
	private const float DEGREES_10 = 0.17f;
	private const float DEGREES_5 = 0.09f;

	/* Disable AdjustDifficulty() when testing in the editor */
	/* Modify AdjustDifficulty() when making a build for resumed testing */

	private void Awake()
	{
		persistent = GameObject.Find("Training Grids").GetComponent<PersistentResources>();
		instance = GetComponent<InstanceResources>();
	}

	// Completely reset environment
	public void NewEnvironment()
	{
		Vector3[] positions;
		int difficulty;

		NewGoalAndKillerColours();
		// Choose a value in (0, 1) to be used for choosing the difficulty level
		difficulty = FindDifficultyLevelFromRandomPoint(Random.value);
		SendDifficultyToCounterManager(difficulty);
		positions = CreateNewPositionVectors(difficulty);
		AssignNewPositionVectors(positions);
		ResetPlayerPosition();
	}
	// Change cube colours and what is labeled the goal and killer
	private void NewGoalAndKillerColours()
	{
		instance.goal = instance.g1;
		instance.killer = instance.g2;
		//instance.r1.material = persistent.green;
		//instance.r2.material = persistent.red;
		instance.goal.tag = "Goal";
		instance.killer.tag = "Killer";
		/*
		if (Random.value < 0.5f)
		{
			instance.goal = instance.g1;
			instance.killer = instance.g2;
			instance.r1.material = persistent.green;
			instance.r2.material = persistent.red;
			instance.g1.tag = "Goal";
			instance.g2.tag = "Killer";
		}
		else
		{
			instance.killer = instance.g1;
			instance.goal = instance.g2;
			instance.r1.material = persistent.red;
			instance.r2.material = persistent.green;
			instance.g1.tag = "Killer";
			instance.g2.tag = "Goal";
		}
		*/
	}

	private int FindDifficultyLevelFromRandomPoint(float difficultyPoint)
	{
		//Vector3[] positions = { new Vector3(), new Vector3() };
		int difficulty = -1;

		//Cycle through the difficulty gradients to find the appropriate difficulty level
		for (int i = 0; i < difficultyCurrent.Length; i++)
		{
			if (difficultyPoint < difficultyCurrent[i])
			{
				/*positions = CreateNewPositionVectors(i);

				//Update the records for the difficulties
				SendDifficultyToCounterManager(i);*/
				difficulty = i;
				break;
			}
		}

		return difficulty;
	}

	private void SendDifficultyToCounterManager(int difficulty)
	{
		instance.player.GetComponent<CounterManager>().SetDifficultyValueForIndex(difficulty);
	}

	private Vector3[] CreateNewPositionVectors(int difficulty)
	{
		Vector3[] positions;

		switch (difficulty)
		{
			// Cases where the goal needs to be closer to the player than the killer
			case 0:
				positions = CalculatePositions(DEGREES_50, DEGREES_180, true); // over 50 degree CW or CCW
				break;
			case 1:
				positions = CalculatePositions(DEGREES_40, DEGREES_50, true);
				break;
			// Cases where the killer needs to be closer to the player than the goal
			case 2:
				positions = CalculatePositions(DEGREES_40, DEGREES_50, false);
				break;
			case 3:
				positions = CalculatePositions(DEGREES_30, DEGREES_40, false);
				break;
			// Cases where the killer and goal are fairly aligned
			case 4:
				positions = CalculatePositions(DEGREES_20, DEGREES_30, false);
				break;
			case 5:
				positions = CalculatePositions(DEGREES_10, DEGREES_20, false);
				break;
			case 6:
				positions = CalculatePositions(DEGREES_5, DEGREES_10, false);
				break;
			case 7:
				positions = CalculatePositions(0, DEGREES_5, false);
				break;
			default: // error case
				positions = CalculatePositions(0, 0, false);
				Debug.Log("SELECTED DIFFICULTY OUT OF BOUNDS");
				break;
		}

		return positions;
	}
	private Vector3[] CalculatePositions(float angleGapMin, float angleGapMax, bool easierDifficulty)
	{
		/*
		RNG the angle of v1, then RNG the gap between v1 and v2
		repeat with the magnitude, but the max value can vary between 9 and 13 (1 less than 10 and 14) depending on the angle
		The min and max are both inclusive for the variation of Random.Range() that returns a float

		after generating the x and y coordinates, adjust the magnitudes to prevent them from going out of bounds
		e.g. (13, 8) becomes (10, 6) by dividing by 1.3
		e.g. (13, 2) becomes (10, 2) since 2 needs to be the minimum

		degrees *pi/180 = radians
		*/

		// Angle between goal and killer
		float goalAngle = Random.Range(0.00f, 6.28f);
		float angleGap = Random.Range(angleGapMin, angleGapMax);
		float killerAngle = goalAngle + (Sign() * angleGap);

		// Goal or killer is closer to player
		float goalMag;
		float killerMag;
		float goalX, goalZ, killerX, killerZ;


		// Make sure coordinates stay at least 1 unit inside of the boundary edge
		if (easierDifficulty == true)
		{
			goalMag = Random.Range(3, 6);
			killerMag = Random.Range(goalMag + 1, 13);

			goalX = Mathf.Cos(goalAngle) * goalMag;
			goalZ = Mathf.Sin(goalAngle) * goalMag;
			killerX = Mathf.Cos(killerAngle) * killerMag;
			killerZ = Mathf.Sin(killerAngle) * killerMag;
			//Debug.Log("killer X and Z = " + killerX + " " + killerZ);
			if (killerX > 9)
			{
				killerZ = killerZ * 9 / killerX;
				killerX = 9;
				//Debug.Log("CHANGED1 killer X and Z = " + killerX + " " + killerZ);
			}
			else if (killerX < -9)
			{
				killerZ = killerZ * -9 / killerX;
				killerX = -9;
				//Debug.Log("CHANGED1 killer X and Z = " + killerX + " " + killerZ);
			}
			else if (killerZ > 9)
			{
				killerX = killerX * 9 / killerZ;
				killerZ = 9;
				//Debug.Log("CHANGED2 killer X and Z = " + killerX + " " + killerZ);
			}
			else if (killerZ < -9)
			{
				killerX = killerX * -9 / killerZ;
				killerZ = -9;
				//Debug.Log("CHANGED2 killer X and Z = " + killerX + " " + killerZ);
			}
		}
		else
		{
			killerMag = Random.Range(3, 6);
			goalMag = Random.Range(killerMag + 1, 13);

			goalX = Mathf.Cos(goalAngle) * goalMag;
			goalZ = Mathf.Sin(goalAngle) * goalMag;
			killerX = Mathf.Cos(killerAngle) * killerMag;
			killerZ = Mathf.Sin(killerAngle) * killerMag;

			if (goalX > 9)
			{
				goalZ = goalZ * 9 / goalX;
				goalX = 9;
			}
			else if (goalX < -9)
			{
				goalZ = goalZ * -9 / goalX;
				goalX = -9;
			}
			else if (goalZ > 9)
			{
				goalX = goalX * 9 / goalZ;
				goalZ = 9;
			}
			else if (goalZ < -9)
			{
				goalX = goalX * -9 / goalZ;
				goalZ = -9;
			}
		}





		Vector3[] positions =
		{
			new Vector3(goalX, 1, goalZ),
			new Vector3(killerX, 1, killerZ)
		};

		return positions;
	}

	private void AssignNewPositionVectors(Vector3[] positions)
	{
		instance.goal.transform.localPosition = positions[0];
		instance.killer.transform.localPosition = positions[1];
		/*
		// Set the new locations of the goal and killer
		if (instance.g1 == instance.goal)
		{
			instance.g1.transform.localPosition = positions[0];
			instance.g2.transform.localPosition = positions[1];
		}
		else
		{
			instance.g1.transform.localPosition = positions[1];
			instance.g2.transform.localPosition = positions[0];
		}*/
	}

	private void ResetPlayerPosition()
	{
		instance.playerRigidbody.velocity = new Vector3(0, 0, 0);
		instance.playerTransform.localPosition = new Vector3(0, 1, 0);
	}

	private static int Sign() // Return -1 or 1 with equal probability
	{
		return (Random.Range(0, 2) * 2) - 1;
	}


	private void ReduceDifficulty()
	{
		for (int i = 0; i < 3; i++)
			difficultyCurrent[i] = (difficultyCurrent[i] * 0.985f) + (difficultyEasiest[i] * 0.015f);
	}

	private void IncreaseDifficulty()
	{
		for (int i = 0; i < 3; i++)
			difficultyCurrent[i] = (difficultyCurrent[i] * 0.995f) + (difficultyHardest[i] * 0.005f);
	}
}