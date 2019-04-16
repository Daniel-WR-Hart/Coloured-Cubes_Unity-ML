using UnityEngine;

public abstract partial class TestAgent : Agent
{
	// This is here just in case the player moves too quickly to be detected when it enters the goal's collider
	protected void OnTriggerStay(Collider collisionSource)
	{
		if (collisionSource.tag != "Platform")
		{
			UpdateFitnessFromEvent(collisionSource);
			Done();
		}
	}
	protected void OnTriggerEnter(Collider collisionSource)
	{
		if (collisionSource.tag != "Platform")
		{
			UpdateFitnessFromEvent(collisionSource);
			Done();
		}
	}

	// Only called from OnTriggerEnter()
	protected abstract void UpdateFitnessFromEvent(Collider collisionSource);


	protected void FixedUpdate()
	{
		// Under a certain condition, have a decision be made
		//RequestDecision();
		UpdateFitnessFromTime();
	}

	// Only called from FixedUpdate()
	protected abstract void UpdateFitnessFromTime();
}