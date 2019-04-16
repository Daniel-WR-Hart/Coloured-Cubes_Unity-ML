using UnityEngine;


public static class WorldCalculator
{
	public const int POSITION_DIMENSIONS = 2;
	// TAKES INPUT OF FORMAT: (x,z) or (x,z, repeat) 
	// Converts a game-state or action into an equivalent where green is located in angles 0 - 45 degrees, or undoes that
	public static float[][] ApplySymmetryOptimization(Vector3 goal, float[][] data, bool forObservation)
	{
		// returns between [-pi, pi]
		float radians = Mathf.Atan2(goal.z, goal.x);

		// Debug.Log("BEFORE " + data.ToString() + " AFTER " + RotateCW_180(data).ToString());
		if (radians < -2.35619449019234)		{ return RotateCW_180(data); }				// Case 5: 180 - 225 degrees
		else if (radians < -1.57079632679490)	{ return ReflectNegativeDiagonal(data); }	// Case 6: 225 - 270 degrees
		else if (radians < -0.78539816339745)												// Case 7: 270 - 315 degrees
		{
			if (forObservation)
				return RotateCW_270(data);
			else
				return RotateCW_90(data);
		}

		else if (radians < 0.000000000000000)	{ return ReflectOverHorizontalAxis(data); } // Case 8: 315 - 360 degrees
		else if (radians < 0.78539816339745)	{ return data; }                            // Case 1: 0 - 45 degrees
		else if (radians < 1.57079632679490)	{ return ReflectPositiveDiagonal(data); }   // Case 2: 45 - 90 degrees
		else if (radians < 2.35619449019234)                                                // Case 3: 90 - 135 degrees
		{
			if (forObservation)
				return RotateCW_90(data);
			else
				return RotateCW_270(data);
		}
		else									{ return ReflectOverVerticalAxis(data); }   // Case 4: 135 - 180 degrees

		/*Debug.Log("1,0 " + Mathf.Atan2(0, 1));
		Debug.Log("1,1 " + Mathf.Atan2(1, 1));
		Debug.Log("0,1 " + Mathf.Atan2(1, 0));
		Debug.Log("-1,1 " + Mathf.Atan2(1, -1));
		Debug.Log("-1,0 " + Mathf.Atan2(0, -1));
		Debug.Log("-1,-1 " + Mathf.Atan2(-1,-1 ));
		Debug.Log("0,-1 " + Mathf.Atan2(-1, 0));
		Debug.Log("1, -1" + Mathf.Atan2(-1,1 ));*/

		/*
		How to make the inputs World-Direction Agnostic
		1 -> no change
		2 -> reflect over y=x
		3 -> rotate 90 CW
		4 -> reflect x=0
		5 -> rotate 180
		6 -> reflect over y=-x
		7 -> rotate 270 CW
		8 -> reflect over y=0

		-----------------
		|\      |      /|
		|  \ 3  |  2 /	|
		| 4  \	|  /  1	|
		|      \|/		|
		-----------------
		|      /|\		|
		| 5  /  |  \  8 |
		|  / 6  |  7 \	|
		|/      |      \|
		-----------------
		*/
	}

	public static float[][] ReflectPositiveDiagonal(float[][] inputs)
	{
		float[] saveZ = inputs[1];
		inputs[1] = inputs[0];
		inputs[0] = saveZ;

		return inputs;
	}
	public static float[][] ReflectNegativeDiagonal(float[][] inputs)
	{
		float[] saveZ = inputs[1];
		inputs[1] = inputs[0];
		inputs[0] = saveZ;

		for (int c = 0; c < POSITION_DIMENSIONS; c++)
			for (int i = 0; i < inputs[c].Length; i++)
				inputs[c][i] = -inputs[c][i];

		return inputs;
	}
	public static float[][] ReflectOverVerticalAxis(float[][] inputs)
	{
		for (int i = 0; i < inputs[0].Length; i++)
			inputs[0][i] = -inputs[0][i];

		return inputs;
	}
	public static float[][] ReflectOverHorizontalAxis(float[][] inputs)
	{
		for (int i = 0; i < inputs[0].Length; i++)
			inputs[1][i] = -inputs[1][i];

		return inputs;
	}
	public static float[][] RotateCW_90(float[][] inputs)
	{
		return ReflectPositiveDiagonal(ReflectOverVerticalAxis(inputs));
	}
	public static float[][] RotateCW_180(float[][] inputs)
	{
		return ReflectOverHorizontalAxis(ReflectOverVerticalAxis(inputs));
	}
	public static float[][] RotateCW_270(float[][] inputs)
	{
		return ReflectOverVerticalAxis(ReflectPositiveDiagonal(inputs));
	}



		//Positions of left and right cubes, and their respective colours
		//The AI can't figure out which one is the goal or killer just based on their colour
		/*AddVectorObs(g1.transform.localPosition.x);
		AddVectorObs(g1.transform.localPosition.z);
		AddVectorObs(g2.transform.localPosition.x);
		AddVectorObs(g2.transform.localPosition.z);
		AddVectorObs(m1.material.color.r);
		AddVectorObs(m2.material.color.r);
		AddVectorObs(m1.material.color.g);
		AddVectorObs(m2.material.color.g);*/

		//Instead of velocity, use the "Stacked Vector" setting to see historical game-states
		//AddVectorObs(rb.velocity);

}
