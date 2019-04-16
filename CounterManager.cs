using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Connects to object "Player"
public class CounterManager : MonoBehaviour
{
	private static PersistentResources persistent;
	private InstanceResources instance;

	private int currentDiff;
	private static int[] totalCounts; //goal, killed, boundary
	private static int[][] detailedCounts; // Subtotals categorized by difficulty, then type of end. Saved value is a counter for that category
	private const int CHAIN_LENGTH = 100000;
	public static float  percentResult;


	// TYPE_OF_END does not count time expiration, since that type of end does not trigger a change in score.
	// It only checks for: Success, death by trap, and death by out of bounds
	private const int DIFFICULTIES = 8, TYPES_OF_END = 3;
	private const int UNDEFINED = -1, GOAL = 0, KILLED = 1, BOUNDARY = 2;

	private static Node head, tail;


	private void Start()
	{
		DeclareResourceManagers();
		InitializeNodeChain();
		InitializeCountingArrays();
	}
	private void DeclareResourceManagers()
	{
		persistent = GameObject.Find("Training Grids").GetComponent<PersistentResources>();
		instance = GetComponentInParent<InstanceResources>();
	}
	private static void InitializeNodeChain()
	{
		tail = new Node(-1, -1, null);
		head = new Node(-1, -1, tail);

		for (int i = 0; i < CHAIN_LENGTH-2; i++)
			head = new Node(-1, -1, head);
	}
	private static void InitializeCountingArrays()
	{
		totalCounts = new int[TYPES_OF_END];
		detailedCounts = new int[DIFFICULTIES][];

		for (int i = 0; i < DIFFICULTIES; i++)
			detailedCounts[i] = new int[TYPES_OF_END];
	}
	/*private void InitializeCounterArrays()
	{
		counters = new int[DIFFICULTIES][][];

		for (int i = 0; i < DIFFICULTIES; i++)
		{
			counters[i] = new int[TYPE_OF_END][];

			for (int j = 0; j < TYPE_OF_END; j++)
				counters[i][j] = new int[TIMELINE];
		}

		totalCounts = new int[TYPE_OF_END];

		historyTracker = new int[TIMELINE][];

		for (int i = 0; i < TIMELINE; i++)
			historyTracker[i] = new int[2];
	}*/

	// Only gets called from SetEnvironment.CreatePositionsFromRandomDifficulty(),
	// When the difficulty is selected for this instance
	public void SetDifficultyValueForIndex(int diff)
	{
		currentDiff = diff;
	}



	protected void OnTriggerEnter(Collider collisionSource)
	{
		// If the agent collides with anything that isn't the platform,
		// reset the points if they are too high to read easily
		// update the counts, give the agent feedback for its performance
		// update the UI displaying the current counts
		// and indictate that this agent finished a round
		if (collisionSource.tag != "Platform")
		{
			// Subtract the stats from the old head
			if (head.typeOfEnd != UNDEFINED) // Case where the node is not empty
			{
				ReduceCounters(head.diff, head.typeOfEnd);
				UpdateUICounter(head.diff, head.typeOfEnd);
			}

			RemoveOldHeadNode();
			AddNewTailNode(collisionSource);

			// Add the stats from the new tail
			IncreaseCounters(tail.diff, tail.typeOfEnd);
			UpdateUICounter(tail.diff, tail.typeOfEnd);


			if (tail.typeOfEnd != GOAL)
			{
				/*if (Vector3.Magnitude(instance.player.transform.localPosition - instance.killer.transform.localPosition) <= 1.5f)
				{
					Debug.Log("CORRECT: Player: " + instance.player.transform.localPosition
						+ "   Goal: " + instance.goal.transform.localPosition
						+ "   Killer: " + instance.killer.transform.localPosition);
				}*/
				/*if (Vector3.Magnitude(instance.player.transform.localPosition - instance.goal.transform.localPosition) <= 1.5f)
				{
					Debug.Log("WRONG: Player: " + instance.player.transform.localPosition
						+ "   Goal: " + instance.goal.transform.localPosition
						+ "   Killer: " + instance.killer.transform.localPosition);
				}*/
			}
			
		}
	}
	// Only called from OnTriggerEnter
	private void RemoveOldHeadNode()
	{
		head = head.next;
	}
	private void AddNewTailNode(Collider collisionSource)
	{
		int typeOfEnd = -1;

		if (collisionSource.tag == "Goal")
			typeOfEnd = GOAL;
		else if (collisionSource.tag == "Killer")
			typeOfEnd = KILLED;
		else if (collisionSource.tag == "Boundary")
			typeOfEnd = BOUNDARY;

		tail.next = new Node(currentDiff, typeOfEnd, null);
		tail = tail.next;
	}
	private void ReduceCounters(int diff, int typeOfEnd)
	{
		totalCounts[typeOfEnd]--;
		detailedCounts[diff][typeOfEnd]--;

	}
	private void IncreaseCounters(int diff, int typeOfEnd)
	{
		totalCounts[typeOfEnd]++;
		detailedCounts[diff][typeOfEnd]++;
	}
	private void UpdateUICounter(int diff, int typeOfEnd) // Update the most recently changed metric only
	{
		
		persistent.specificText[DIFFICULTIES][typeOfEnd].text = totalCounts[typeOfEnd].ToString();

		int total = totalCounts[GOAL] + totalCounts[KILLED] + totalCounts[BOUNDARY];

		persistent.specificText[diff][typeOfEnd].text
			= detailedCounts[diff][typeOfEnd].ToString("0");

		float numerator = totalCounts[GOAL] * 100;
		float denominator = total;
		percentResult = (numerator / denominator);

		persistent.percentText.text = percentResult.ToString("0.000") + "%";
		//Debug.Log(totalCounts[GOAL] + " | " + totalCounts[KILLED] + " | " + totalCounts[BOUNDARY]);
		//Debug.Log(numerator + " / " + denominator + " = " + result);
	}
}