//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

// Connects to object "Player"
public class Controls : MonoBehaviour
{
	//public Rigidbody rb;
	private const float STRIDE_LENGTH = TestAgentObject.STRIDE_LENGTH; // This might not get the value after it's been set
	private float[] vectorAction;
	private bool actionPending;

	
	void Update ()
	{
		if (DetectPlayerInput())
			InterpretPlayerInput();
	}
	void FixedUpdate()
	{
		if (IsActionPending())
			InitiateAction();
	}


	private bool DetectPlayerInput()
	{
		return (Pressed("a") || Pressed("s") || Pressed("d") || Pressed("w"));
	}
	private void InterpretPlayerInput()
	{
		//This action will be executed at the next frame in FixedUpdate()
		actionPending = true;

		if (Pressed("a"))
			SetVectorActionToLeft();
		else if (Pressed("s"))
			SetVectorActionToBackward();
		else if (Pressed("d"))
			SetVectorActionToRight();
		else if (Pressed("w"))
			SetVectorActionToForward();
	}
	private bool Pressed(string key)
	{
		return Input.GetKeyDown(key);
	}
	private bool IsActionPending()
	{
		return actionPending;
	}
	private void InitiateAction()
	{
		// The action is no longer waiting
		actionPending = false;

		//rb.AddForce(-thrust, 0, 0, ForceMode.Impulse);
		float x = Mathf.Clamp(vectorAction[0], -1, 1);
		float z = Mathf.Clamp(vectorAction[1], -1, 1);

		Vector3 movement = new Vector3(x, 0, z) * STRIDE_LENGTH;
		transform.localPosition += movement;
	}

	private void SetVectorActionToLeft()
	{
		vectorAction = new float[] { -1, 0 };
	}
	private void SetVectorActionToBackward()
	{
		vectorAction = new float[] { 0, -1 };
	}
	private void SetVectorActionToRight()
	{
		vectorAction = new float[] { 1, 0 };
	}
	private void SetVectorActionToForward()
	{
		vectorAction = new float[] { 0, 1 };
	}
}