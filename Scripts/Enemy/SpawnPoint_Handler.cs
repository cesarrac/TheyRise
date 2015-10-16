using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpawnPoint_Handler : MonoBehaviour {
	public ResourceGrid resourceGrid;
	[HideInInspector]
	public Vector2[] spawnPositions;
	[HideInInspector]
	public Vector2[] possibleSpawnPositions;

	public List<Node>[] kamikazePaths;
	public List<Node>[] paths;

	public Vector2[] kamikazeDestinations;

	Queue<List<Node>> pathQueue = new Queue<List<Node>> ();
	Queue<List<Node>> kamikazePathQueue = new Queue<List<Node>> ();

	public Map_Generator map_generator;

	int map_width, map_height;

	// Make the MIN Spawn X a 4th of the map's width
	int minSpawnX, minSpawnY;

	public int numberOfSpawnPositions = 5;

	void Awake(){

		if (!map_generator) {
			map_generator = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map_Generator> ();
			map_width = map_generator.width;
			map_height = map_generator.height;
			minSpawnX = map_width / 4;
			minSpawnY = map_height / 10;
		} else {
			map_width = map_generator.width;
			map_height = map_generator.height;
			minSpawnX = map_width / 4;
			minSpawnY = map_height / 10;

		}

		InitializeAllSpawnPositions ();

		
		if (resourceGrid == null)
			resourceGrid = GameObject.FindGameObjectWithTag ("Map").GetComponent<ResourceGrid> ();

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
		GetRandomKamikazeDestinations ();

	

	}

	/* Make this A LOT simpler by just getting the map width and height and assuming that it will always have at least
	 *  a 2 Pixel border. So we have to get all the x positions at y = 2 and y = mapHeight - 2, then all the y positions
	 * at x = 2 and x = mapWidth - 2 */

	void InitializeAllSpawnPositions()
	{
		int calcTotalPositions = ((map_width + map_height) + 20) *5;
		possibleSpawnPositions = new Vector2[calcTotalPositions];
		int index = 0;
		// Bottom
		for (int y = 1; y <= 2; y++) {
			for (int x = minSpawnX; x < map_width - minSpawnX; x++) {
				if (index == 0){
					possibleSpawnPositions[0] = new Vector2(x, y);
				}else{
					possibleSpawnPositions[index] = new Vector2(x, y);
				}
				index++;
			}
		}
		//Top
		for (int y = map_height - 2; y < map_height; y++) {
			for (int x = minSpawnX; x < map_width - minSpawnX; x++) {

				possibleSpawnPositions[index] = new Vector2(x, y);

				index++;
			}
		}
		// Left
		for (int x = minSpawnX; x <= minSpawnX + 4; x++){
			for (int y = 4; y < map_height - 2; y++)  {
				
				possibleSpawnPositions[index] = new Vector2(x, y);
				
				index++;
			}
		}
		// Right
		for (int x = map_width - (minSpawnX + 4); x <= map_width - minSpawnX; x++){
			for (int y = 4; y < map_height - 2; y++)  {
				
				possibleSpawnPositions[index] = new Vector2(x, y);
				
				index++;
			}
		}

		//Clean up the Array, turn all Vector2.zero into a sure position like (5,2)
		for (int i = 0; i < possibleSpawnPositions.Length; i++) {
			if (possibleSpawnPositions[i] == Vector2.zero){
				possibleSpawnPositions[i] = new Vector2(5, 2);
			}
		}

		InitActualSpawnPositions ();
	}

	void InitActualSpawnPositions()
	{
		// Initialize the spawn positions array
		spawnPositions = new Vector2[numberOfSpawnPositions];
		for (int i = 0; i < spawnPositions.Length; i++) {
			spawnPositions[i] = GetRandomSpawnPositions();
			Debug.Log(spawnPositions[i]);
		}
	}
	Vector2 GetRandomSpawnPositions()
	{
		Vector2 returnVector = Vector2.zero;
		int randomPositionPick = Random.Range (2, possibleSpawnPositions.Length - 1);

		return new Vector2 (possibleSpawnPositions [randomPositionPick].x, possibleSpawnPositions [randomPositionPick].y);
		

	}
	void GetRandomKamikazeDestinations()
	{
		kamikazeDestinations = new Vector2[numberOfSpawnPositions];
		// Loop through the Kamikaze Destinations array and fill each with a random X and Y
		for (int x = 0; x < kamikazeDestinations.Length; x++) {
			int randomKamikazeX = Random.Range(minSpawnX, map_width - minSpawnX);
			int randomKamikazeY = Random.Range(minSpawnY, map_height - minSpawnY);
			kamikazeDestinations[x] = new Vector2(randomKamikazeX, randomKamikazeY);
		}
	}

	void Start () {

		
		paths = new List<Node>[spawnPositions.Length];
		kamikazePaths = new List<Node>[spawnPositions.Length]; 



		if (resourceGrid != null) {
			// Get Paths to capital from all Spawn Positions
			for (int x =0; x< spawnPositions.Length; x++) {
				resourceGrid.GenerateWalkPath (resourceGrid.capitalSpawnX, resourceGrid.capitalSpawnY, false, 
			                              (int)spawnPositions [x].x, (int)spawnPositions [x].y);
				if (resourceGrid.pathForEnemy != null)
					FillPath (resourceGrid.pathForEnemy, x, false);
			}

			// Then get Paths to kamikaze destinations
			for (int x =0; x< kamikazeDestinations.Length; x++) {
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
