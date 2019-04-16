using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// Demonstrate the platforms in a spherical arrangement
// Attach to shape base
public class SpawnAtVertices : MonoBehaviour
{
	private Mesh mesh;
	//private Vector3[] normalSet;

	/*public GameObject prefab;
	private GameObject[] prefabSet;*/
	public Transform cam;

	private List<Vector3> vertexSet;
	private List<GameObject> platformSet;

	void Start ()
	{
		//platformSet = new List<GameObject>();
		MakeListOfVertices();
		MakeListOfPlatforms();
		MovePlatformsToVertices();
	}

	// Create an invisible mesh
	void MakeListOfVertices ()
	{
		//mesh.CombineMeshes(0, true, false, false);
		//mesh.Clear();
		/*mesh.vertices = new Vector3[] { new Vector3(0, 0, 0),
										new Vector3(1, 0, 0),
										new Vector3(0, 1, 0),
										new Vector3(0, 0, 1),
										new Vector3(1, 1, 0),
										new Vector3(1, 0, 1),
										new Vector3(0, 1, 1),
										new Vector3(1, 1, 1)};*/
		mesh = GetComponent<MeshFilter>().mesh;
		//normalSet = mesh.normals;
		vertexSet = mesh.vertices.Distinct().ToList();
	}

	// Create a list of references to every platform (tag the platform prefab with "Player")
	void MakeListOfPlatforms ()
	{
		platformSet = GameObject.FindGameObjectsWithTag("Player").ToList<GameObject>();
	}

	// Going through the list of platforms, map each platform to a unique vertex on the created mesh
	void MovePlatformsToVertices ()
	{
		for (int i = 0; i < vertexSet.ToArray().Length; i++)
		{
			platformSet[i].transform.position = transform.position + (vertexSet[i] * 0.7f);
			platformSet[i].transform.LookAt(cam);
			platformSet[i].transform.eulerAngles += new Vector3(90, 0, 0);
		}
	}
	
	/*void SpawnPrefabs ()
	{
		foreach (Vector3 vert in reducedSet)
		{
			GameObject clone = Instantiate(prefab, transform.position + (vert * 0.5f), new Quaternion()) as GameObject;
			clone.transform.LookAt(cam);
		}
	}*/
}