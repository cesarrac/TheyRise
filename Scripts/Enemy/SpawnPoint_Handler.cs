using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpawnPoint_Handler : MonoBehaviour {
	public ResourceGrid resourceGrid;
	public Vector2[] spawnPositions;

	public List<Node>[] kamikazePaths;
	public List<Node>[] paths;

	public Vector2[] kamikazeDestinations;

	[Header("Kamikaze Min. and Max. X/Y positions")]
	public float minKamiX, maxKamiX, minKamiY, maxKamiY;

	private float randomKamikazeX;
	private float randomKamikazeY;

	Queue<List<Node>> pathQueue = new Queue<List<Node>> ();
	Queue<List<Node>> kamikazePathQueue = new Queue<List<Node>> ();

	void Awake(){
		paths = new List<Node>[spawnPositions.Length];
		kamikazePaths = new List<Node>[spawnPositions.Length]; 

		// Store the paths (List<Node>) in a queue 
//
//		foreach (List<Node> path in paths) {
//			pathQueue.Enqueue(path);
//		}
//
//
//		foreach (List<Node> path in paths) {
//			kamikazePathQueue.Enqueue(path);
//		}

		// Create some random Kamikaze positions
		RandomKamikaze ();
	}

	void RandomKamikaze()
	{
		// Loop through the Kamikaze Destinations array and fill each with a random X and Y
		for (int x = 0; x < kamikazeDestinations.Length; x++) {
			randomKamikazeX = Random.Range(minKamiX, maxKamiX);
			randomKamikazeY = Random.Range(minKamiY, maxKamiY);
			kamikazeDestinations[x] = new Vector2(Mathf.Round(randomKamikazeX), Mathf.Round(randomKamikazeY));
		}
	}

	void Start () {

		if (resourceGrid == null)
			resourceGrid = GameObject.FindGameObjectWithTag ("Map").GetComponent<ResourceGrid> ();

		if (resourceGrid != null) {
			// Get Paths to capital from all Spawn Positions
			for (int x =0; x< spawnPositions.Length; x++) {
				resourceGrid.GenerateWalkPath (resourceGrid.capitalSpawnX, resourceGrid.capitalSpawnY, false, 
			                              (int)spawnPositions [x].x, (int)spawnPositions [x].y);
				if (resourceGrid.pathForEnemy != null)
					FillPath (resourceGrid.pathForEnemy, x, false);
			}

			// Then get Paths to kamikaze destinations
			for (int x =0; x< spawnPositions.Length; x++) {
				resourceGrid.GenerateWalkPath ((int)kamikazeDestinations[x].x, (int)kamikazeDestinations[x].y, false, 
				                               (int)spawnPositions [x].x, (int)spawnPositions [x].y);
				if (resourceGrid.pathForEnemy != null)
					FillPath (resourceGrid.pathForEnemy, x, true);
			}
		}


	}

	void GetNextInQueue ()
	{

	}

	void GetPathFromGrid(int spawnPosX, int spawnPosY)
	{
		
	}
	void FillPath(List<Node> currPath, int i, bool trueIfKamikaze){

		if (!trueIfKamikaze) {
			paths [i] = new List<Node> ();
			for (int y = 0; y < currPath.Count; y++) {
				paths [i].Add (currPath [y]);
			}

//			Debug.Log ("PATH TO CAPITAL: " + i + " From: " + paths [i] [0].x + " " + paths [i] [0].y + " To: " + paths [i] [paths [i].Count - 1].x + " " + paths [i] [paths [i].Count - 1].y);

		} else {
			kamikazePaths [i] = new List<Node> ();
			for (int y = 0; y < currPath.Count; y++) {
				kamikazePaths [i].Add (currPath [y]);
			}
//			Debug.Log ("KAMIKAZE PATH: " + i + " From: " + kamikazePaths [i] [0].x + " " + kamikazePaths [i] [0].y + " To: " + kamikazePaths [i] [kamikazePaths [i].Count - 1].x + " " + kamikazePaths [i] [kamikazePaths [i].Count - 1].y);

		}
	}
	
}
