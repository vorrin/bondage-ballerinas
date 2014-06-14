using UnityEngine;
using System.Collections.Generic;

public class PlayerControls : MonoBehaviour
{
	PlayerScript playerScript;
	GameManager gameManager;
	GameManager.GameState lastGameState = GameManager.GameState.Start;
	GuitarScript guitarScript;
	Camera mainCamera;
	Rect debugRect = new Rect (0, 0, 100, 100);
	Rect specialRect = new Rect (Screen.width - 205, 0, 205, 204);
	
	//Touch input
	public List<Vector2>touchGraph = new List<Vector2> ();
	bool touchedPlayer = false, touchedEnemy = false;
	float tapDurationMax = 1f, tapStartTime = 0;
	float swipeSpeedTotal = 0, swipeSpeedMin = 5, swipeDistanceMin = 20, swipeSpeedPoints = 0, swipeDistanceCurrent = 0;
	public LayerMask rayLayerMask;
	
	//TODO:Remove this var
	float swipeVelocityThreshold = 100f;
	float swipeAngle = 0;
	float deltaX = 0, deltaY = 0;
	
	void Start ()
	{		
		playerScript = GetComponent<PlayerScript> ();
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		//guitarScript = GameObject.FindGameObjectWithTag("Guitar").GetComponent<GuitarScript>();
	}

	void Update ()
	{
		if (gameManager.gameState == GameManager.GameState.GamePlay)
		{
			//RawInputUpdate ();
			SplitScreenControls ();
			
			//Debug inputs
			//if (Input.touchCount > 0 && debugRect.Contains (Input.touches [0].position))
			if (Input.GetKeyDown (KeyCode.LeftArrow))
			{
				if (playerScript.transform.position.y >= WorldManager.floor && gameManager.powerLevelCurrent > 0)
				{
					playerScript.Attack (PlayerScript.AttackKeys.GLIDEATTACK.ToString ());
					gameManager.DecreasePowerLevel ();
				}
				else
					playerScript.Attack (PlayerScript.AttackKeys.JAB.ToString ());
			}
			if (Input.GetKeyDown (KeyCode.RightArrow))
				playerScript.Attack (PlayerScript.AttackKeys.MIDSWING.ToString ());
			if (Input.GetKeyDown (KeyCode.UpArrow))
				playerScript.Attack (PlayerScript.AttackKeys.UPPERCUT.ToString ());
			if (Input.GetKeyDown (KeyCode.DownArrow))
				playerScript.Attack (PlayerScript.AttackKeys.DOWNSMASH.ToString ());
			if (Input.GetKeyDown (KeyCode.Space))
				playerScript.DashToAttack ();
			if (Input.GetKeyDown (KeyCode.RightShift))
				playerScript.SpecialAttack ();
		}
	}
	
	float tapLength = 0.25f;
	float fingerDownTime = 0;
	bool leftSide = false;
	float holdUpperCutLength = 0.25f;

	void SplitScreenControls ()
	{
		if (Input.touchCount > 0)//finger down
		{
			if (Input.touches [0].position.x <= Screen.width / 2)
			{
				leftSide = true;	
			}
			else
			{
				leftSide = false;	
			}
			
			if (fingerDownTime == 0)
			{
				fingerDownTime = Time.realtimeSinceStartup;	
			}
			
			if(leftSide){
				playerScript.ChargeUpperCutStart();	
				if(Time.realtimeSinceStartup > fingerDownTime + holdUpperCutLength){
				playerScript.chargingUpperCutSlowDown = true;	
			}
			}
			else{
				playerScript.ChargeMidSwingStart();	
			}			
		}
		else//finger released
		{
			if (fingerDownTime > 0)
			{
				if(leftSide){
					playerScript.ChargeUpperCutRelease(Time.realtimeSinceStartup - fingerDownTime);	
				}
				else
				{
					if (Time.realtimeSinceStartup <= fingerDownTime + tapLength)
					{//screen tapped
						playerScript.Attack(PlayerScript.AttackKeys.JAB.ToString());
					}
					else
					{//screen pressed
						playerScript.ChargeMidSwingRelease();					
					}
				}
			}
			
			//reset vars
			fingerDownTime = 0;
			playerScript.chargingUpperCutSlowDown = false;
		}		
	}
	
	void RawInputUpdate ()
	{
		if (Input.touchCount > 0 || Input.GetMouseButtonDown (0))
		{			
			//add current touch pos to graph
			if (Input.GetMouseButtonDown (0))
				touchGraph.Add (new Vector2 (Input.mousePosition.x, Input.mousePosition.y));
			else
				touchGraph.Add (Input.touches [0].position);			
			
			//shoot ray from current touch pos to check what we are touching
			Vector3 working = new Vector3 (touchGraph [touchGraph.Count - 1].x, touchGraph [touchGraph.Count - 1].y, 0);			
			Ray ray = mainCamera.ScreenPointToRay (working);
			RaycastHit hit2;
			if (Physics.Raycast (ray, out hit2))
			{
				//is the hit object an enemy?
				if (hit2.collider.transform.root.tag == "Player")
				{
					touchedPlayer = true;
				}
				else if (hit2.collider.transform.root.tag == "Enemy")
				{
					touchedEnemy = true;	
				}
			}
			if (touchGraph.Count > 1)
			{//add current finger movement speed to total and increase point count
				swipeSpeedTotal = Vector2.Distance (touchGraph [touchGraph.Count - 2], touchGraph [touchGraph.Count - 1]) / Time.deltaTime;
				swipeSpeedPoints++;
				
				
				//Check if we are in a swipe upwards
				swipeDistanceCurrent = Vector2.Distance (touchGraph [0], touchGraph [touchGraph.Count - 1]);
				deltaY = touchGraph [touchGraph.Count - 1].y - touchGraph [0].y;
				deltaX = touchGraph [touchGraph.Count - 1].x - touchGraph [0].x;
				swipeAngle = (Mathf.Atan2 (deltaY, deltaX) * 180 / Mathf.PI) + 180;//Top of portrait screen is 0, increasing anticlockwise						
				Debug.Log ("swipe angle" + swipeAngle);
				
				if (swipeDistanceCurrent > swipeDistanceMin &&
					swipeSpeedTotal / swipeSpeedPoints > swipeSpeedMin)
				{//a swipe occured
					//check swipe direction	
					if (swipeAngle > 225 && swipeAngle < 315)
					{
						if (playerScript.transform.position.y <= WorldManager.floor)
						{
								
							playerScript.ChargeUpperCutStart ();
						}
					}
				}
			}
			else
			{//first frame of finger press
				tapStartTime = Time.realtimeSinceStartup;
			}
		}
		else
		{//no fingers down
			if (!gameManager.closeCombat)
			{
				if (touchGraph.Count >= 2)
				{
					
					//check special button
					if (specialRect.Contains (touchGraph [touchGraph.Count - 1]))
					{
						playerScript.SpecialAttack ();
					}
					else if (playerScript.chargingUpperCut)
					{//charge attack was released
						playerScript.ChargeUpperCutRelease (0);
					}
					else
					{//dont allow other input if we pressed the button
					
						//find distance between start and end points
						swipeDistanceCurrent = Vector2.Distance (touchGraph [0], touchGraph [touchGraph.Count - 1]);
						deltaY = touchGraph [touchGraph.Count - 1].y - touchGraph [0].y;
						deltaX = touchGraph [touchGraph.Count - 1].x - touchGraph [0].x;
						swipeAngle = (Mathf.Atan2 (deltaY, deltaX) * 180 / Mathf.PI) + 180;//Top of portrait screen is 0, increasing anticlockwise						
						Debug.Log ("swipe angle" + swipeAngle);
					
						//gliding dash
						if (playerScript.transform.position.y >= WorldManager.floor && gameManager.powerLevelCurrent > 0)
						{
							if (Time.realtimeSinceStartup < tapStartTime + tapDurationMax &&
								swipeDistanceCurrent < swipeDistanceMin)
							{//screen was tapped						
								playerScript.Attack (PlayerScript.AttackKeys.GLIDEATTACK.ToString ());
								gameManager.DecreasePowerLevel ();
							}
							else if (swipeAngle > 45 && swipeAngle < 135)
							{//swipe down
								playerScript.Attack (PlayerScript.AttackKeys.DOWNSMASH.ToString ());
							}
						}
					
						if (playerScript.canDash && touchedPlayer && touchedEnemy)
						{ //we must dash to enemy
							playerScript.DashToAttack ();
						}
						else if (Time.realtimeSinceStartup < tapStartTime + tapDurationMax &&
								swipeDistanceCurrent < swipeDistanceMin)
						{//screen was tapped
							//TODO: once items are added, shoot ray to collect them here
							Debug.Log ("Screen tapped at: " + touchGraph [touchGraph.Count - 1]);
							playerScript.Attack (PlayerScript.AttackKeys.JAB.ToString ());
						}
						else if (swipeDistanceCurrent > swipeDistanceMin &&
								swipeSpeedTotal / swipeSpeedPoints > swipeSpeedMin)
						{//a swipe occured
							//check swipe direction						
							//see which direction matches the angle
							if (swipeAngle > 45 && swipeAngle < 135)
							{//swipe down
								if (playerScript.transform.position.y != WorldManager.floor)
								{//dont downswipe if on floor
									//					if (playerScript.currentEnemy != null && playerScript.currentEnemy.alive && 
									//				playerScript.currentEnemy.timesTapAttackHit >= playerScript.tapAttacksBeforeUpperCut) {//Only allow attack if we hit the enemy enough
									playerScript.Attack (PlayerScript.AttackKeys.DOWNSMASH.ToString ());	
									//					}
								}
							}
							else if (swipeAngle > 225 && swipeAngle < 315)
							{//swipe up
								if (playerScript.transform.position.y != WorldManager.maxWorldHeight)
								{//dont uppercut if at world height limit
									//			if (playerScript.currentEnemy != null && playerScript.currentEnemy.alive && 
									//		playerScript.currentEnemy.timesTapAttackHit >= playerScript.tapAttacksBeforeUpperCut) {//Only allow attack if we hit the enemy enough
									playerScript.Attack (PlayerScript.AttackKeys.UPPERCUT.ToString ());
									//			}
								}
							}
							else
							{//the angle was not up or down, so it must be horizontal
								playerScript.Attack (PlayerScript.AttackKeys.MIDSWING.ToString ());	
							}
						}
					}
				}
			}
					
			//no fingers down, so reset values
			swipeSpeedTotal = 0;
			swipeSpeedPoints = 0;
			swipeDistanceCurrent = 0;
			touchedPlayer = false;
			touchedEnemy = false;
			tapStartTime = 0;
			touchGraph.Clear ();
		}
	}
}