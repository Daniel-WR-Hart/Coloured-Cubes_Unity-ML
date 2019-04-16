using UnityEngine;
using UnityEngine.UI;

public class PersistentResources : MonoBehaviour
{
	[HideInInspector]
	public Text percentText;
	public Text[][] specificText;
	
	public Material red, green;             // Materials in Resources
	[Range(1f,100f)]
	public float playbackSpeed;

	private void Start()
	{
		SetGameSpeed();
		SetUITextReferences();
		//SetColorReferences();
	}

	private void SetGameSpeed()
	{
		Time.fixedDeltaTime = 0.02f / playbackSpeed;
	}
	private void SetUITextReferences()
	{
		// The Text needs to be easily accessible to all prefab instances
		Transform scores = GameObject.Find("Scores").transform;
		const int CATEGORIES = 9, TYPES_OF_END = 3;

		specificText = new Text[CATEGORIES][]; // Difficulties (8) + Total (1)

		for (int label = 0; label < CATEGORIES; label++)
		{
			specificText[label] = new Text[TYPES_OF_END]; // goal, killed, boundary

			for (int score = 0; score < TYPES_OF_END; score++)
				specificText[label][score] = scores.GetChild(label).GetChild(score).GetComponent<Text>();
		}

		percentText = GameObject.Find("Text percent").GetComponent<Text>();
	}
	private void SetColorReferences()
	{
		red = Resources.Load("Bad.mat", typeof(Material)) as Material;
		green = Resources.Load("Good.mat", typeof(Material)) as Material;
	}
}