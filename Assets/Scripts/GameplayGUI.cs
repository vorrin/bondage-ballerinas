using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameplayGUI : MonoBehaviour
{
	
	//TODO: remove debug vars	
	Rect fpsRect = new Rect (800, 75, 200, 200);
	string fpsString = "FPS: ";
	Rect closeCombatRect = new Rect (800, 50, 200, 200);
	string closeCombatString = "Close Combat: ";
	Rect healthRect = new Rect (800, 100, 200, 200);
	string healthString = "HP: ";
	Rect scoreMultiplierRect = new Rect (800, 125, 200, 200);
	string scoreMultiplierString = "Score Multiplier: ";
	Rect scoreRect = new Rect (800, 150, 200, 200);
	string scoreString = "Score: ";
	Rect currentEnemyStatusRect = new Rect (0, 0, 400, 100);
	string currentEnemyString = "";
	List<Texture2D> multiplierTextures = new List<Texture2D> ();
	List<Texture2D> specialButtonTextures = new List<Texture2D> ();
	GameManager gameManager;
	GameObject player;
	PlayerScript playerScript;
	PlayerControls playerControls;
	Camera camera;
	GUIStyle style;
	
	// Use this for initialization
	void Start ()
	{
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		playerScript = player.GetComponent<PlayerScript> ();
		camera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		playerControls = player.GetComponent<PlayerControls> ();
		
		style = new GUIStyle ();
		style.fontSize = 24;
		style.normal.textColor = Color.red;
		
		//load textures
		for (int i = 0; i < 6; i++)
		{
			multiplierTextures.Add ((Texture2D)Resources.Load ("GUITextures/Multiplier_Gauge_x" + (i + 1)));
		}
		for (int i = 0; i < 5; i++)
		{
			specialButtonTextures.Add ((Texture2D)Resources.Load ("GUITextures/Button" + i));	
		}
	}
	
	void OnGUI ()
	{	
		
		//TODO: Remove debug stuff		
		GUI.Label (fpsRect, fpsString + gameManager.fPS);
		GUI.Label (closeCombatRect, closeCombatString + gameManager.closeCombat.ToString ());
		GUI.Label (scoreMultiplierRect, scoreMultiplierString + gameManager.scoreMultiplier);
		GUI.Label (new Rect (800, 175, 100, 100), "Power level: " + gameManager.powerLevelCurrent);
		if (playerControls.touchGraph.Count > 0)
			GUI.Label (new Rect (800, 200, 100, 100), "Last Touch: " + playerControls.touchGraph [playerControls.touchGraph.Count - 1]);
		
		if (gameManager.gameState == GameManager.GameState.GamePlay)
		{
			//HP
			GUI.Label (healthRect, healthString + playerScript.hitPoints);
			
			//Display current enemy status
			if (playerScript.currentEnemy != null && playerScript.currentEnemy.alive)
			{
				currentEnemyStatusRect.x = camera.WorldToScreenPoint (playerScript.currentEnemy.transform.position).x;
				currentEnemyStatusRect.y = camera.WorldToScreenPoint (playerScript.currentEnemy.transform.position).y;
				
				currentEnemyString = "HP: " + playerScript.currentEnemy.hitPoints + "  ";
				currentEnemyString += playerScript.currentEnemy.timesTapAttackHit + ":" + playerScript.tapAttacksBeforeUpperCut;
				if (playerScript.canDash)
					currentEnemyString += "DASH!";
				
				GUI.Label (currentEnemyStatusRect, currentEnemyString);
			}
			
			//score multiplier
			if (gameManager.scoreMultiplier == 0)
				GUI.DrawTexture (new Rect (50, 50, 472, 92), multiplierTextures [0]);
			else
				GUI.DrawTexture (new Rect (50, 50, 472, 92), multiplierTextures [gameManager.scoreMultiplier - 1]);
			
			//special button
			//GUI.DrawTexture (new Rect (Screen.width - 205, Screen.height - 204, 205, 204), specialButtonTextures [gameManager.powerLevelCurrent]);
			
			
			GUI.Label (new Rect (100, 60, 500, 500), "Score: " + gameManager.displayScore, style);
			
			if (!playerScript.alive)
			{
				GUI.Label (new Rect (Screen.width / 2, Screen.height / 2, 200, 200), "YOU ARE DEAD!", style);	
			}
		}
		else if (gameManager.gameState == GameManager.GameState.Start)
		{
			GUI.Label (new Rect (400, 400, 400, 400), "Press return or tap screen to start");	
		}
	}
}
