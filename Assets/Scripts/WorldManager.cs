using UnityEngine;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{	
	List<Stage> stages = new List<Stage> ();
	int currentStage = -1;
	
	List<GameObject> blockList = new List<GameObject> ();
	GameObject startingBlock;
	int blockSize = 120;
	List<int> recentBlocks = new List<int> ();
	int recentBlocksCount = 4;
	Vector3 blockPlacementPos = Vector3.zero;
	
	List<GameObject> skyList = new List<GameObject>();
	GameObject startingSky;
	int skySize = 240, skyIndex = 0;
	Vector3 skyPlacementPos = new Vector3(0,30,130);
	
	List<GameObject> silhouetteList = new List<GameObject>();
	GameObject startingSilhouette;
	int silhouetteSize = 240, silhouetteIndex = 0;
	Vector3 silhouettePlacementPos = new Vector3(0,30,100);
	
	public List<GameObject> enemyList = new List<GameObject> ();
	public List<GameObject> onScreenEnemies = new List<GameObject>();
	public static int enemySpawnDistance = 50, enemySpawnHeightAbove = 20, enemySpawnHeightAbovePassive = 21;
	float enemySpawnTime = 5, enemySpawnTimeMin = 1, enemySpawnTimeMax = 2;
	float enemyAboveSpawnTime = 10, enemyAboveSpawnTimeMin = 4, enemyAboveSpawnTimeMax = 10;
	float enemyAbovePassiveSpawnTime = 7, enemyAbovePassiveSpawnTimeMin = 4, enemyAbovePassiveSpawnTimeMax = 10;
	int enemyIndex = 0, maxEnemies = 10;
	Vector3 enemyPlacementPos = new Vector3(enemySpawnDistance,0,-30);
	
	GameManager gameManager;
	GameObject player;
	PlayerScript playerScript;
	GameObject camera;
	ForceGPULoad levelGPULoader;
	Vector3 initialBlockPos = new Vector3 (0, -1000, 0);
	public static float maxWorldHeight = 60, floor = 0;
	
	public bool canSpawnEnemy = true;
	
	void Start ()
	{		
		DefineStages ();
		
		gameManager = GetComponent<GameManager> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		playerScript = player.GetComponent<PlayerScript> ();
		camera = GameObject.FindGameObjectWithTag ("MainCamera");
		levelGPULoader = GameObject.Find ("ForceGPULoadCamera").GetComponent<ForceGPULoad> ();
		
		TransitionStage ();
	}
	
	public void TransitionStage ()
	{
		if (currentStage < stages.Count - 1) {
			currentStage++;
		
		
			
			//set camera filter
			GameObject filter = (GameObject)(Instantiate (Resources.Load (stages [currentStage].filterName), Vector3.zero, Quaternion.Euler (Vector3.zero)));
			filter.transform.parent = camera.transform;
			
			//remove old blocks from recent list
			recentBlocks.Clear ();
			
			//add new assets
			//blocks
			//fill block list
			for (int i = 0; i < stages[currentStage].numberOfBlocks; i++) {
				blockList.Add ((GameObject)Instantiate (Resources.Load (stages [currentStage].blockResourceName + (i + 1)), initialBlockPos, transform.rotation));	
			}
			startingBlock = (GameObject)Instantiate (Resources.Load (stages [currentStage].startingBlock), blockPlacementPos, transform.rotation);
			blockPlacementPos.x += blockSize;
			
			//skies
			//fill sky list
			skyList.Add((GameObject)Instantiate(Resources.Load(stages[currentStage].background),initialBlockPos,Quaternion.Euler(new Vector3(90,180,0))));
			skyList.Add((GameObject)Instantiate(Resources.Load(stages[currentStage].background),initialBlockPos,Quaternion.Euler(new Vector3(90,180,0))));
			skyList.Add((GameObject)Instantiate(Resources.Load(stages[currentStage].background),initialBlockPos,Quaternion.Euler(new Vector3(90,180,0))));
			startingSky = (GameObject)Instantiate(Resources.Load(stages[currentStage].background),skyPlacementPos,Quaternion.Euler(new Vector3(90,180,0)));
			skyPlacementPos.x += skySize;
			
			//fill silhouette list
			silhouetteList.Add((GameObject)Instantiate(Resources.Load(stages[currentStage].silhouette),initialBlockPos,Quaternion.Euler(new Vector3(0,180,0))));
			silhouetteList.Add((GameObject)Instantiate(Resources.Load(stages[currentStage].silhouette),initialBlockPos,Quaternion.Euler(new Vector3(0,180,0))));
			silhouetteList.Add((GameObject)Instantiate(Resources.Load(stages[currentStage].silhouette),initialBlockPos,Quaternion.Euler(new Vector3(0,180,0))));
			startingSilhouette = (GameObject)Instantiate(Resources.Load(stages[currentStage].silhouette),initialBlockPos,Quaternion.Euler(new Vector3(0,180,0)));
			
			//fill enemy list
			for (int i = 0; i < maxEnemies; i++) {
				enemyList.Add((GameObject) Instantiate(Resources.Load("GREMLIN"), initialBlockPos, Quaternion.Euler(0,270,0)));
			}
			//Force assets to load into GPU RAM
			levelGPULoader.ForceLoad (initialBlockPos);
			
		}
	}
	
	void Update ()
	{
		if (gameManager.gameState == GameManager.GameState.GamePlay) {
			//do block checking
			CheckBlocks (player.transform.position);	
			//check enemies
			CheckEnemies(player.transform.position);
		}
	}
	
	void CheckEnemies (Vector3 playerPosition)
	{		
		
		if(canSpawnEnemy){
			
			//move the next enemy into position and reset it
			if(Time.time > enemySpawnTime){
				enemyList[enemyIndex].transform.position = new Vector3(player.transform.position.x + enemySpawnDistance,player.transform.position.y,player.transform.position.z);
				enemyList[enemyIndex].GetComponent<EnemyScript>().SetUpEnemyType();
				//increment the index
				enemyIndex++;
				if(enemyIndex >=enemyList.Count)
					enemyIndex = 0;
				
				enemySpawnTime = Time.time + Random.Range(enemySpawnTimeMin,enemySpawnTimeMax);
			}
			
			if(Time.time > enemyAboveSpawnTime){
				enemyList[enemyIndex].transform.position = new Vector3(player.transform.position.x + enemySpawnDistance,player.transform.position.y + enemySpawnHeightAbove ,player.transform.position.z);				
				enemyList[enemyIndex].GetComponent<EnemyScript>().SetUpEnemyType();
				//increment the index
				enemyIndex++;
				if(enemyIndex >=enemyList.Count)
					enemyIndex = 0;
				
				enemyAboveSpawnTime = Time.time + Random.Range(enemyAboveSpawnTimeMin,enemyAboveSpawnTimeMax);
			}
			if(false)//(Time.time > enemyAbovePassiveSpawnTime)
			{
				enemyList[enemyIndex].transform.position = new Vector3(player.transform.position.x + enemySpawnDistance,player.transform.position.y + enemySpawnHeightAbovePassive ,player.transform.position.z);
				enemyAboveSpawnTime = Time.time + Random.Range(enemyAboveSpawnTimeMin,enemySpawnTimeMax);
				enemyList[enemyIndex].GetComponent<EnemyScript>().SetUpEnemyType();
				//increment the index
				enemyIndex++;
				if(enemyIndex >=enemyList.Count)
					enemyIndex = 0;
				
				enemyAbovePassiveSpawnTime = Time.time + Random.Range(enemyAbovePassiveSpawnTime,enemyAbovePassiveSpawnTime);
			}		
		}
	}
	
	void CheckBlocks (Vector3 playerPosition)
	{
		CreateNewBlocks ();
		RemoveOldBlocks (playerPosition);
	}

	void CreateNewBlocks ()
	{
		//now check the blocks
		if (player.transform.position.x > blockPlacementPos.x - blockSize) {		
			
			//pick the next block
			int nextBlockIndex = 0;
			for (int i = 0; i <= 30; i++) {
				nextBlockIndex = Random.Range (0, stages [currentStage].numberOfBlocks);
				if (!recentBlocks.Contains (nextBlockIndex)) {
					break;
				} else if (i == 30)
					nextBlockIndex = recentBlocks [0];
			}
			
			//move the next block into position
			blockList [nextBlockIndex].transform.position = blockPlacementPos;
			recentBlocks.Add (nextBlockIndex);
			
			//remove oldest block from recent list
			if (recentBlocks.Count >= recentBlocksCount) {
				recentBlocks.RemoveAt (0);	
			}
			//move the placement pos, ready for the next block
			blockPlacementPos.x += blockSize;
		}
		
		//check the skies
		if(player.transform.position.x > skyPlacementPos.x - skySize){
			//move the next sky into position
			skyList[skyIndex].transform.position = skyPlacementPos;
			
			skyPlacementPos.x += skySize;
			skyIndex++;
			if(skyIndex >= skyList.Count)
				skyIndex = 0;
		}
		
		//check the silhouettes
		if(player.transform.position.x > silhouettePlacementPos.x - silhouetteSize){
			//move the next sky into position
			silhouetteList[silhouetteIndex].transform.position = silhouettePlacementPos;
			
			silhouettePlacementPos.x += silhouetteSize;
			silhouetteIndex++;
			if(silhouetteIndex >= silhouetteList.Count)
				silhouetteIndex = 0;
		}
	}

	void RemoveOldBlocks (Vector3 playerPosition)
	{
		//TODO: Remove unused blocks after level transition
	}
	
	void DefineStages ()
	{
		//Stage 1 City
		Stage stage = new Stage ();
		stage.background = "Block Prefabs/City_NewSky_Plane";
		stage.startingBackground = "Block Prefabs/City_NewSky_Plane";
		stage.startingBlock = "Block Prefabs/CityBlock_001";
		stage.blockResourceName = "Block Prefabs/CityBlock_00";
		stage.silhouette = "Block Prefabs/City_Silhouette";
		stage.filterName = "CityLights";
		stage.numberOfBlocks = 5;
		stage.preloadResource = "City";
		stages.Add (stage);
		
		//Stage 2 Wasteland
		stage = new Stage ();
		stage.background = "Wasteland_Sky_Plane";
		stage.startingBackground = "City_Trans_Wasteland_Sky_Plane";
		stage.startingBlock = "WastelandBlock_001";
		stage.blockResourceName = "WastelandBlock_00";
		stage.silhouette = "Wasteland_Silhouette";
		stage.filterName = "WastelandLights";
		stage.numberOfBlocks = 5;
		stages.Add (stage);
		
		//Stage 3 Hell
		stage = new Stage ();
		stage.background = "Hell_Sky_Plane";
		stage.startingBackground = "Wasteland_Trans_Hell_Sky_Plane";
		stage.startingBlock = "HellTransitionBlock_001";
		stage.blockResourceName = "HellBlock_00";
		stage.silhouette = "Hell_Silhouette";
		stage.filterName = "HellLights";
		stage.numberOfBlocks = 5;
		stages.Add (stage);
	}
	
	struct Stage
	{
		public string background, startingBackground, startingBlock, blockResourceName, silhouette, filterName, preloadResource;
		public int numberOfBlocks;
	}
}