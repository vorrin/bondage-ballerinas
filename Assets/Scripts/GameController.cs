using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	
	public Camera mainCamera;
	public GameObject onScreenText;
	public Transform playerPrefab;
	
	public float verticalExtent {get; private set;}
	public float horizontalExtent {get; private set;}
	public float gameTime = 0;
	public int victoryScore = 10; 
	public float countDownToStartMax; // seconds
	private float countDownToStart; // seconds

	public List<Player> livingPlayers;

	public bool robotsActive = false;
	public bool aliensActive = false;
	public bool dinosActive = false;

	public int dinoCount = 0;
	public int alienCount = 0;
	public int robotCount = 0;

	public Transform textJoin, textTitle, textGameOver;
	public Transform textRobots, textAliens, textDinos;
	public Transform featherSplotion;

	private int targetCount = 0;

	private float nextCheckTime = 0;

	public AudioSource[] music;
	//public AudioClip startMusic;

	private Player winner;
	
	public static Color[] playerColorList = new Color[]{
		new Color(0.01f, 0.02f, 0.6f, 1f),
		new Color(0.7f, 0.12f, 0.01f, 1f),
		new Color(0.9f, 0.02f, 0.7f, 1f),
		new Color(0.8f, 0.92f, 0.01f, 1f),
		new Color(0.4f, 0.02f, 0.01f, 1f),
		new Color(0.3f, 0.7f, 0.21f, 1f),
		new Color(0.01f, 0.92f, 0.81f, 1f),
		new Color(1f, 0.62f, 0.31f, 1f),
		new Color(0.4f, 0.12f, 0.7f, 1f),
		new Color(0.5f, 0.5f, 0.7f, 1f),
		new Color(0.8f, 0.8f, 0.31f, 1f),
		new Color(0f, 0.82f, 0.31f, 1f),
		new Color(0.1f, 0.22f, 0.51f, 1f),
		new Color(0.4f, 0.5f, 0.6f, 1f),
		new Color(0.11f, 0.2f, 1f, 1f),
		new Color(0.41f, 0.2f, 1f, 1f),
		new Color(0.11f, 0.72f, 1f, 1f),
		new Color(0.8f, 0.82f, 0.8f, 1f)	
	};
	
	public static GameController instance;
	
	void Awake() {
		instance = this;
		Debug.Log("Timescale: " + Time.timeScale + " " + Time.fixedDeltaTime);
		Time.timeScale = 1;
		Time.fixedDeltaTime = 0.02f;
		this.verticalExtent = Camera.main.orthographicSize;
		this.horizontalExtent = this.verticalExtent * Camera.main.aspect;
	}
	
	public static int LOBBY = 0;
	public static int LOBBYREADY = 4;
	public static int PLAYING = 1;
	public static int PAUSED = 2;
	public static int GAMEOVER = 3;	
	
	public int gameState = LOBBY;
	
	public Transform playerButtonPrefab;
	
	public List<PlayerControllerData> connectedControllers = new List<PlayerControllerData>();

	private PlayerControllerData MakeButton(int id, int layout, int pid)
	{		
			//pcd.button = (Transform) Instantiate(playerButtonPrefab, new Vector3( -18f + ((pcd.playerId-1)%4)* 12f, 12.8f - ((pcd.playerId-1) / 4) * 3f, 0), Quaternion.identity);
			Transform button = (Transform) Instantiate(playerButtonPrefab, new Vector3(((pid-1)%8)* 0.13f + 0.07f, 0.94f - ((pid-1) / 8) * 0.1f, 0), Quaternion.identity);
			PlayerControllerData pcd = button.GetComponent<PlayerControllerData>();
			pcd.Init (id, layout, pid);
		
			button.Find ("Texts").Find ("PlayerText").GetComponent<GUIText>().text = "Player "+pcd.playerId;
			pcd.infoText = button.Find ("Texts").Find ("InfoText").GetComponent<GUIText>();
			pcd.SetButtonText("Waiting!");
			pcd.SetButtonColor();
		
			//pcd.GetComponent<AudioSource>().pitch = 1f + pcd.playerId / 10f;
			return pcd;		
	}

	private void SetReady(PlayerControllerData pcd)
	{
		//pcd.GetComponent<AudioSource>().pitch = 1.5f + pcd.playerId / 10f;
		//pcd.GetComponent<AudioSource>().Play();
		pcd.SetButtonText("Ready!");	
	}


	public void TweenText(Transform t)
	{		
		Debug.Log ("Tweening text "+ t.name);
		GoTweenChain chain = new GoTweenChain();
		chain.append(Go.to(t, 0.5f, new GoTweenConfig().position(Vector3.down * 5, true).setEaseType(GoEaseType.QuadInOut)));
		chain.appendDelay(1);
		chain.append(Go.to(t, 0.5f, new GoTweenConfig().position(Vector3.up * 5, true).setEaseType(GoEaseType.QuadInOut)));
		chain.play();
	}


	// Update is called once per frame
	void Update () {
		if (gameState == LOBBY || gameState == LOBBYREADY)
			LobbyUpdate();
		else if (gameState == PLAYING)
			GameUpdate();
		
		if (Input.GetKey("r") || Input.GetButtonDown("StartGame")) {
			Application.LoadLevel("GameScene");	
		}
		//Debug.Log("Screen size: " + verticalExtent + "," + horizontalExtent);
	}
	
	private void GameUpdate()
	{
		if (gameState == PLAYING)
		{
			gameTime += Time.deltaTime;
			//EnemyController.

		}
		
//		if (Input.GetKeyDown(KeyCode.Alpha1)) aliensActive = !aliensActive;
//		if (Input.GetKeyDown(KeyCode.Alpha2)) robotsActive = !robotsActive;
//		if (Input.GetKeyDown(KeyCode.Alpha3)) dinosActive = !dinosActive;
		
	}
	
	public IEnumerator PressRToRestartText()
	{
		ShowOnScreenText("Press R to Restart");
		yield return new WaitForSeconds(1.5f);
		StartCoroutine(ShowPlayerWinsText());
	}
	
	public IEnumerator ShowPlayerWinsText()
	{
		ShowOnScreenText("Player " + this.winner.playerControllerData.playerId + " wins!");
		yield return new WaitForSeconds(1.5f);
		StartCoroutine(PressRToRestartText());
	}
	
	
	public void PlayerWins(Player player)
	{
		if (this.gameState != GAMEOVER) 
		{
			this.gameState = GAMEOVER;
			this.winner = player;
		//	player.isInvulnerable = true;
			
			StartCoroutine(ShowPlayerWinsText());
			
			Go.to(mainCamera.transform.parent, 1, new GoTweenConfig().eulerAngles(new Vector3(-60,0,0), false).setEaseType(GoEaseType.QuartInOut));
			Go.to(winner.transform, 1f, new GoTweenConfig().scale(2f, true));
		}
	}
		
	private void StartGame()
	{

//		this.GetComponent<AudioSource>().Stop();
//		this.GetComponent<AudioSource>().clip = music;
		this.GetComponent<AudioSource>().Play();

		this.livingPlayers = new List<Player>();
		//AudioSource.PlayClipAtPoint(startMusic, this.transform.position);
		foreach(PlayerControllerData pcd in this.connectedControllers) {
			Vector3 ploc = Vector3.left * ((pcd.playerId - 2)*2 - 5);
			GameObject newPlayerObject = GameObject.Instantiate(this.playerPrefab, 
				ploc, 
				Quaternion.Euler(0, 0, 0)) as GameObject;
			
			newPlayerObject.layer = 15 + livingPlayers.Count;
			//foreach (Transform child in newPlayerObject.GetComponent<Player>().transformColliderChildren)
			//{
			//	child.gameObject.layer = 15+ livingPlayers.Count;
			//}
						
			Player newPlayer = newPlayerObject.GetComponent<Player>();
			newPlayer.playerControllerData = pcd;
			this.livingPlayers.Add(newPlayer);
		//	Respawn(newPlayer);
		}
		
		winner = null;
		gameState = PLAYING;
		ShowOnScreenText("");
	}	
	
	public void PlayerDied(Player player) {
		this.livingPlayers.Remove(player);
		if (this.livingPlayers.Count == 1) {
			PlayerWins(this.livingPlayers[0]);
		}
	}
	
	void LobbyUpdate() {
	
		if (gameState == LOBBY)
		{
			int readyPlayers = 0;
			foreach(PlayerControllerData pcd in connectedControllers)
			{
				if (pcd.isReady)
				{
					readyPlayers++;
				}
			}
			if (readyPlayers >= 2 && readyPlayers == connectedControllers.Count) 
			{
				gameState = LOBBYREADY;
				countDownToStart = countDownToStartMax;
				
				//GetComponent<AudioSource>().clip = mainSound;
				//GetComponent<AudioSource>().Play();
				
				//AudioSource.PlayClipAtPoint(scratchSound,Vector3.zero);
			}		
		}
		if (gameState == LOBBYREADY)
		{

			countDownToStart -= Time.deltaTime;
			//Go.to (camRot, countDownToStartMax, new GoTweenConfig().eulerAngles(new Vector3(315,0,0), false).setEaseType(GoEaseType.QuadOut));
			ShowOnScreenText("STARTING IN ... "+countDownToStart.ToString("0.0"));

			Go.to (textJoin, 1, new GoTweenConfig().position(Vector3.down * 5).setEaseType(GoEaseType.QuadInOut));
			Go.to (textTitle, 2, new GoTweenConfig().position(Vector3.up * 5).setEaseType(GoEaseType.QuadInOut));

			if (countDownToStart <= 0)
			{
				//Debug.Log ("Starting game...");
				StartGame();
			}
			return;
		}
		
		
		// keyboard wasd
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			print ("left shift!");
			bool alreadyIn = false;
			foreach (PlayerControllerData cc in connectedControllers)
			{
				if (cc.controllerId == -1)
				{
					cc.isReady = true;
					alreadyIn = true;
					SetReady(cc);
					continue;
				}
			}
			if (!alreadyIn)
			{
				//PlayerControllerData pcd = new PlayerControllerData(-1, PlayerControllerData.ONEPLAYER, connectedControllers.Count + 1);
				PlayerControllerData pcd =  MakeButton(-1, PlayerControllerData.ONEPLAYER, connectedControllers.Count + 1);
				connectedControllers.Add(pcd);
			}
		}
		
		// keyboard arrows
		if (Input.GetKeyDown(KeyCode.RightShift))
		{
			bool alreadyIn = false;
			foreach (PlayerControllerData cc in connectedControllers)
			{
				if (cc.controllerId == 0)
				{
					cc.isReady = true;
					alreadyIn = true;
					SetReady(cc);
					continue;
				}
			}
			if (!alreadyIn)
			{
				//PlayerControllerData pcd = new PlayerControllerData(0, PlayerControllerData.ONEPLAYER, connectedControllers.Count + 1);
				PlayerControllerData pcd =  MakeButton(0, PlayerControllerData.ONEPLAYER, connectedControllers.Count + 1);
				connectedControllers.Add(pcd);
			}
		}
		
		// standard setup
		for (int i = 1; i <= 8; i++)
		{
			if (Input.GetButtonDown("Jump1_g" + i))
			{
				bool alreadyIn = false;
				// check if controller is already used 
				foreach (PlayerControllerData cc in connectedControllers)
				{
					if (cc.controllerId == i)
					{
						if (cc.controllerLayout == PlayerControllerData.ONEPLAYER)
							cc.isReady = true;
						alreadyIn = true;
						SetReady(cc);
						continue;
					}
				}
				if (alreadyIn) continue;
				//PlayerControllerData pcd = new PlayerControllerData(i, PlayerControllerData.ONEPLAYER, connectedControllers.Count + 1);
				
				PlayerControllerData pcd =  MakeButton(i, PlayerControllerData.ONEPLAYER, connectedControllers.Count + 1);
				connectedControllers.Add(pcd);
			
			}			
		}
		/*
		// dual player setup
		for (int i = 1; i <= 16; i++)
		{
			if (Input.GetButtonDown("Fire1_s" + i))
			{
				
				int controllerId = (i+1)/2;
				bool isLeftPlayer = i % 2 == 1;
				int controllerType = isLeftPlayer ? PlayerControllerData.TWOPLAYERLEFT : PlayerControllerData.TWOPLAYERRIGHT;
				
				// check if controller is already used 
				bool alreadyIn = false;
				foreach (PlayerControllerData cc in connectedControllers)
				{
					if (cc.controllerId == controllerId && 
						( cc.controllerLayout == PlayerControllerData.ONEPLAYER
						|| cc.controllerLayout == controllerType))
						
					{
						if (cc.controllerLayout == controllerType)
							cc.isReady = true;
						alreadyIn = true;
						SetReady(cc);
						continue;
					}
				}
				if (alreadyIn) continue;
				PlayerControllerData pcd =  MakeButton(controllerId, controllerType, connectedControllers.Count + 1);
				connectedControllers.Add(pcd);
			}			
		}	
		*/	

	}

	public void GameOver()
	{
		if (gameState == GAMEOVER) return;
		gameState = GAMEOVER;
		//Go.to (Time, 1, new GoTweenConfig().floatProp("timeScale", 0, false).setEaseType(GoEaseType.QuadInOut));
		//textGameOver.transform.position += Vector3.up * 5;
		Time.timeScale = 0.3f;
		Time.fixedDeltaTime = 0.04f;
		Go.to (textGameOver, 0.1f, new GoTweenConfig().position(Vector3.up * 5).setEaseType(GoEaseType.QuadInOut));
		music[0].pitch *= 0.5f;
		
	}
	
	private void ShowOnScreenText(string text)
	{
		onScreenText.GetComponent<GUIText>().text = text;	
	}
}