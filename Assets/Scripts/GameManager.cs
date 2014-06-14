using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
		public enum GameState
        {
            Start, Pause, End, GamePlay, GuitarMinigame
        }
	
	
	public GameState gameState = GameState.Start;
	
	public WorldManager worldManager;
	GameObject camera;
	private GameObject player;
	
	public SFXManager sFXManager;
	
	public int score = 0, displayScore = 0;
	public int scoreMultiplier = 0, scoreMultiplierMax = 6;
	
	public int powerLevelCurrent = 4, powerLevelMax = 4;
	
	public bool closeCombat = false;
	public EnemyScript closeCombatTarget;
	
	bool transitionedToWasteland = false;
	
	private PlayerScript playerScript;
	
	float updateInterval = 0.5F; 
	float accum   = 0; // FPS accumulated over the interval
	int   frames  = 0; // Frames drawn over the interval
	float timeleft; // Left time for current interval
	public float fPS = 0; // FPS to display
	
	public float timeSlowDownFinishes = 0, timeSlowdownStarts = 0, timeScaleValue = 1;
	
	LookAtTargetObj lookAtObject;
	public CameraControl cameraControlScript;
	// Use this for initialization
	void Start ()
	{		
		//allow 60 fps on ipad
		Application.targetFrameRate = 60;
		
		worldManager = GetComponent<WorldManager>();
		lookAtObject = GameObject.Find ("LookAt").GetComponent<LookAtTargetObj>();		
		player = GameObject.FindGameObjectWithTag ("Player");
		playerScript = player.GetComponent<PlayerScript>();
		camera = GameObject.Find ("Main Camera");
		cameraControlScript = camera.GetComponent<CameraControl>();
		sFXManager = GetComponent<SFXManager>();
	}
	
	// Update is called once per frame
	void Update ()
	{		
		//check if we should begin gameplay
		if(gameState == GameState.Start &&
			(Input.GetKeyDown(KeyCode.Return) ||
			 Input.touches.Length > 0)){
				
			
			gameState = GameState.GamePlay;	
		}
		
		if(gameState == GameState.GamePlay){
		
		CheckTimeScale();
			
		//check close combat status
		if(closeCombat && (!closeCombatTarget.alive || closeCombatTarget == null || !playerScript.specialKnockUp)){
			EndCloseCombat();	
		}
		
			
		//calculate FPS
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		frames++;
		if(timeleft <= 0){
			fPS = accum/frames;
			timeleft = updateInterval;
			accum = 0;
			frames = 0;
		}	
		
		//Debug.Log(displayScore);
		/*if(Time.time > 10){
			if(!transitionedToWasteland){
				worldManager.TransitionStage();
				transitionedToWasteland = true;
			}
		}
		else if(Time.time > 90)
			worldManager.TransitionStage();*/
			
			if(!playerScript.alive && (Input.touches.Length>0 || Input.GetKeyDown(KeyCode.Return))){
				Application.LoadLevel(Application.loadedLevel);
			}	
				
		}
	}
	
	void CheckTimeScale(){
		if(Time.realtimeSinceStartup < timeSlowDownFinishes &&
			Time.realtimeSinceStartup > timeSlowdownStarts){
			Time.timeScale = timeScaleValue;
		}
		//set timescale to 0 if enough real time has passed
		else if (Time.realtimeSinceStartup > timeSlowDownFinishes) {
			Time.timeScale = 1f;
		}	
	}
	
	public void SlowDownTime (float timeScaleValue, float realTimeSlowdownStarts, float realTimeSlowdownEnds)
	{
		timeSlowdownStarts = realTimeSlowdownStarts;
		timeSlowDownFinishes = realTimeSlowdownEnds;
		this.timeScaleValue = timeScaleValue;
	}
	
	public void EndTimeSlowdown(){
		timeSlowDownFinishes = 0;	
	}
	
	public void BeginCloseCombat(EnemyScript combatTarget){
		if(combatTarget != null){
		lookAtObject.target = combatTarget.transform;
		closeCombat = true;	
			closeCombatTarget = combatTarget;
		
		//GetComponent<AudioManager>().TriggerSolo();
		}
	}
	
	public void EndCloseCombat(){
		lookAtObject.target = null;
		closeCombat = false;
		//playerScript.DownAttack();
	}
	
	public void IncreaseScore(int amount){
		score += amount;
		this.displayScore = score;
		Debug.Log("Score increased by: " + amount + " New score is: " + score);		
	}
	
	public void IncreaseScoreMultiplier(){
		if(scoreMultiplier == 0)
			scoreMultiplier = 2;
		else
			scoreMultiplier ++;
		
		if(scoreMultiplier > scoreMultiplierMax){
			IncreasePowerLevel();
			ResetScoreMultiplier();
		}
	}
	
	public void ResetScoreMultiplier(){
		scoreMultiplier = 0;
		Debug.Log("scoreMultiplier reset");
	}
	
	public void IncreasePowerLevel(){
		if(powerLevelCurrent < powerLevelMax){
			powerLevelCurrent++;
		}
	}
	
	public void DecreasePowerLevel(){
		if(powerLevelCurrent > 0)
			powerLevelCurrent--;
	}
	
	public void ResetPowerLevel(){
		powerLevelCurrent = 0;
		Debug.Log("Power level reset");
	}
}