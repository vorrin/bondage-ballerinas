using UnityEngine;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour
{	
	#region variables
	GameManager gameManager;
	AudioManager audioManager;
	public bool alive = true;
	public static float xSpeed = 10f,
			zPos = -30f,
			moveToHeightAnimThreshold = 1f;
	
	float glidingSpeedX = 30, glidingSpeedY = 1, glideDashSpeedX = 50;
	bool wasGlidingLastFrame = false;
	float timeglideDashEnds = 0, glideDashDuration = 0.75f;
	
	bool fallDown = false;
	float fallDownSpeedY = 40;
	
	[HideInInspector]
	public string currentAttackKey = null;
	public Dictionary<string,Attack> attackTypes = new Dictionary <string, Attack> ();
	public float timeAttackFinishes = 0, beingHitDuration = 5f, timeBeingHitFinishes = 0;
	public int tapAttacksBeforeUpperCut = 0;
	public int hitPoints = 100;
	float ascendTime = 1f, timeAscendFinishes = 0, timeDescendFinishes = 0;
	public Animator animator;
	public float closeCombatDistance = 4f, generalCombatDistance = 4f;
	public bool canDash = false, dashing = false;
	public float timeDashInputFinishes = 0, dashSpeedMultiplier = 2f;
	float dashInputSlowdownValue = 0.25f, dashInputSlowdownDuration = 3f;
	public float timeDashingFinished = 0;
	
	public EnemyScript currentEnemy;
	
	public EnemyScript specialEnemy;
	bool hasPerformedSpecialAttack = false;
	
	bool downAttack = false;
	float downAttackMoveSpeed = 30f;
	public float xSpeedThisUpdate = 0;
	public float distanceFromCurrentEnemy = 0;
	public string queuedAttackKey = null;
	public float timeAttackHitBegins = 0, timeAttackHitEnds = 0;
	bool jabCombo = false;
	
	public bool specialKnockUp = false;
	
	float lastEnemyAttackID = 0;
	
	public bool chargingUpperCut = false, chargingMidSwing = false, chargingUpperCutSlowDown = false;
	public Vector3 currentUpperCutKnockBackPower = Vector2.zero;
	
	public string lastAttackKey = AttackKeys.JAB.ToString();
	#endregion
	
	// Use this for initialization
	void Start ()
	{
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		audioManager = gameManager.gameObject.GetComponent<AudioManager> ();
		
		animator = GetComponent<Animator> ();
		if (animator.layerCount >= 2)
			animator.SetLayerWeight (1, 1);
		
		DefineAttackTypes ();
		currentUpperCutKnockBackPower = attackTypes[PlayerScript.AttackKeys.UPPERCUT.ToString()].knockbackPower;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (gameManager.gameState == GameManager.GameState.GamePlay) {
			if (alive) {
			
				if(!string.IsNullOrEmpty(currentAttackKey)){
					lastAttackKey = currentAttackKey;	
				}
				
				if (currentEnemy == null) {
					distanceFromCurrentEnemy = 0;
				} else if (currentEnemy.transform.position.x < transform.position.x) {
					currentEnemy = null;
					distanceFromCurrentEnemy = 0;
				}
				
				if(specialKnockUp){
					if(rigidbody.velocity.y < 0){						
						specialKnockUp = false;
						specialEnemy.specialKnockUp = false;	
						Attack(AttackKeys.GLIDEATTACK.ToString());
					}
				}
			
				
				CheckAnimations ();
				if (gameManager.closeCombat) {
					
				}
				else{
					MovePlayer();	
				}
			
		
				if (canDash && Time.realtimeSinceStartup > timeDashInputFinishes) {
					canDash = false;
				}		
				//Delayed attack hit
				if(!string.IsNullOrEmpty(currentAttackKey) && Time.time > timeAttackHitBegins && Time.time < timeAttackHitEnds){
					
					//check if we actually hit something		
					Ray ray = new Ray (transform.position, Vector3.right);
					RaycastHit hit;
					if (Physics.Raycast (ray, out hit)) {
						//is the hit object an enemy?
						if (hit.collider.transform.root.tag == "Enemy") {
							EnemyScript enemyScript = hit.collider.transform.root.gameObject.GetComponent<EnemyScript> ();
						
							//is the enemy alive?
							if (enemyScript.alive) {
								//is the enemy within reach of the current attack?
								if (enemyScript.transform.position.x <= transform.position.x + attackTypes [currentAttackKey].range) {
									//is the enemy within an acceptable hight range?	
									if (enemyScript.transform.position.y >= transform.position.y - attackTypes [currentAttackKey].rangeBelow &&
							   		enemyScript.transform.position.y <= transform.position.y + attackTypes [currentAttackKey].rangeAbove) {
								
										enemyScript.RecieveAttack ();
										
										//SlowDownTimeForAttack(currentAttackKey);
										
										if(currentAttackKey == AttackKeys.UPPERCUT.ToString()){
											Debug.Log("UPPERCUT");
										}
										else if(currentAttackKey == AttackKeys.DOWNSMASH.ToString()){
											Debug.Log("DOWNSMASH");
										}else if(currentAttackKey == AttackKeys.MIDSWING.ToString()){
											Debug.Log("MIDSWING");
										}
									}
								}
							}
						}
					}
				}				
				
				
				if(transform.position.y < WorldManager.floor){
					transform.position = new Vector3(transform.position.x, WorldManager.floor,transform.position.z);	
				}
				
				if(wasGlidingLastFrame && transform.position.y == WorldManager.floor){//no longer gliding
					gameManager.ResetScoreMultiplier();
					gameManager.ResetPowerLevel();
					//hasPerformedSpecialAttack = false;
				}
				wasGlidingLastFrame = false;
				if(transform.position.y > WorldManager.floor){
					wasGlidingLastFrame = true;
				}
				
			} else {			
				animator.SetBool ("DIE", true);
				animator.SetLayerWeight (1, 0);
				
				foreach (KeyValuePair<string, Attack> attackType in attackTypes) {
					animator.SetBool (attackType.Key, false);
				}
				
				if(transform.position.y > WorldManager.floor)
				transform.position = new Vector3 (transform.position.x, transform.position.y - xSpeed * Time.deltaTime, transform.position.z);
			}
		}
	}
	
	#region movement
	void MovePlayer ()
	{	
		if(specialKnockUp){
			//use gravity so behaviour is the same as enemies
			rigidbody.useGravity = true;
		}
		else{
			rigidbody.useGravity = false;
			rigidbody.velocity = Vector3.zero;
			//move the player
			if(fallDown){
				transform.position = new Vector3(transform.position.x,transform.position.y - fallDownSpeedY * Time.deltaTime,transform.position.z);
				if(transform.position.y <= WorldManager.floor)
					fallDown = false;
			}
			else if (downAttack) {			
				xSpeedThisUpdate = 0;
				if (transform.position.y - (downAttackMoveSpeed * Time.deltaTime) < WorldManager.floor) {
					transform.position = new Vector3 (transform.position.x, WorldManager.floor, transform.position.z);
					downAttack = false;
				} else {
					transform.Translate (new Vector3 (0, -downAttackMoveSpeed * Time.deltaTime, 0), Space.World);	
				}
			}
			
			
			else if(transform.position.y > WorldManager.floor){//gliding
				if(Time.time < timeglideDashEnds)					
					transform.position = new Vector3(transform.position.x + (glideDashSpeedX * Time.deltaTime), transform.position.y + (-glidingSpeedY * Time.deltaTime), transform.position.z);
				else
					transform.position = new Vector3(transform.position.x + (glidingSpeedX * Time.deltaTime), transform.position.y + (-glidingSpeedY * Time.deltaTime), transform.position.z);	
			}
			
			
			else if(dashing){
				if(currentEnemy == null || Vector3.Distance(transform.position,new Vector3(currentEnemy.knockBackPos.x-closeCombatDistance,currentEnemy.knockBackPos.y,currentEnemy.knockBackPos.z))<1f)
					DashToAttackCallBack();	
				else
					transform.position = Vector3.Lerp(transform.position,new Vector3(currentEnemy.knockBackPos.x-closeCombatDistance,currentEnemy.knockBackPos.y,currentEnemy.knockBackPos.z), dashSpeedMultiplier * Time.deltaTime);
				
			}else if(currentEnemy != null && Time.time <= timeDashingFinished + AudioManager.barLength){
				//DO NOTHING!!	
			}
			else {
				xSpeedThisUpdate = xSpeed * Time.deltaTime;
				if(chargingUpperCutSlowDown)
					xSpeedThisUpdate = 0;
				
				transform.Translate (new Vector3 (xSpeedThisUpdate, 0, 0), Space.World);
			}
		}
	}
	#endregion
	
	#region Attack logic	
	public void Attack (string attackKey)
	{
		if (!string.IsNullOrEmpty(attackKey)) {
			if (dashing) {
				Debug.Log ("Queued attack: " + attackKey);
				queuedAttackKey = attackKey;
			} else {
					
				if (Time.time > timeAttackFinishes && Time.time > timeBeingHitFinishes) {					
					currentAttackKey = attackKey;
						
					attackTypes[currentAttackKey].uniqueID = Random.value;
						
					timeAttackHitBegins = Time.time + attackTypes[attackKey].hitBegin;		
					timeAttackHitEnds = Time.time + attackTypes[attackKey].hitEnd;				
					timeAttackFinishes = Time.time + attackTypes [currentAttackKey].duration;		
					
					chargingUpperCut = false;
					chargingMidSwing = false;
					
					animator.SetBool (currentAttackKey, true);					
					
					if(currentAttackKey == AttackKeys.GLIDEATTACK.ToString()){
						timeglideDashEnds = Time.time + glideDashDuration;		
					}
					else if(currentAttackKey == AttackKeys.DOWNSMASH.ToString()){
						DownAttack ();	
					}
					
					Debug.Log ("Attack" + attackKey);
					//jabCombo = false;
				}	
				//else if(attackKey == AttackKeys.NEWJAB.ToString){
				//jabCombo = true;
				//	queuedAttackKey = attackKey;
				//}
				
			}
		}
	}
	
	public void ChargeUpperCutStart(){
		chargingUpperCut = true;
		animator.SetBool("CHARGEUPPERCUT",true);
	}
	public void ChargeUpperCutRelease(float chargeTime){
		if(chargeTime < 0.75f)
			chargeTime = 0.75f;
		
		currentUpperCutKnockBackPower = new Vector3(attackTypes[AttackKeys.UPPERCUT.ToString()].knockbackPower.x * chargeTime,
													attackTypes[AttackKeys.UPPERCUT.ToString()].knockbackPower.y * chargeTime,0);
		
		chargingUpperCut = false;
		animator.SetBool("CHARGEUPPERCUT",false);
		Attack(AttackKeys.UPPERCUT.ToString());
	}
	public void ChargeMidSwingStart(){
		chargingMidSwing = true;
		animator.SetBool("CHARGEMIDSWING",true);
	}
	public void ChargeMidSwingRelease(){
		chargingMidSwing = false;
		animator.SetBool("CHARGEMIDSWING",false);
		Attack(AttackKeys.MIDSWING.ToString());
	}
	
	public void SpecialAttack(){
		
		if(!hasPerformedSpecialAttack && gameManager.powerLevelCurrent > 0){
			hasPerformedSpecialAttack = true;
			
			specialEnemy = null;		
			
			foreach(GameObject enemy in gameManager.worldManager.enemyList){
				EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
				if (enemyScript.alive) {
						//is the enemy within reach of the current attack?
					if(enemyScript.transform.position.x >= transform.position.x){
						if (enemyScript.transform.position.x <= transform.position.x + 0.75) {
							//is the enemy within an acceptable hight range?	
							if (enemyScript.transform.position.y >= transform.position.y - attackTypes [AttackKeys.UPPERCUT.ToString()].rangeBelow &&
								enemyScript.transform.position.y <= transform.position.y + attackTypes [AttackKeys.UPPERCUT.ToString()].rangeAbove) {
									specialEnemy = enemyScript;
							}
						}
					}
				}
			}
			if(specialEnemy != null){
			/*specialEnemy.rigidbody.velocity = Vector3.zero;
			specialEnemy.specialKnockUp = true;
			specialEnemy.rigidbody.AddForce(gameManager.powerLevels[gameManager.currentPowerLevel].force,ForceMode.Impulse);*/
			gameManager.BeginCloseCombat (specialEnemy);
			
			specialKnockUp = true;
			rigidbody.velocity = Vector3.zero;
			rigidbody.useGravity = true;
			rigidbody.AddForce(new Vector3(0,60,0),ForceMode.Impulse);
			} else{
				animator.SetBool(AttackKeys.UPPERCUT.ToString(),true);	
			}
		}
	}
	
	void SlowDownTimeForAttack(string attackKey){
		float timeScale = 0.75f;
		float timeSlowdownStarts = Time.realtimeSinceStartup;
		float timeSlowdownEnds = audioManager.timeNextBeatStarts;
		
		if(attackKey != AttackKeys.UPPERCUT.ToString())
			gameManager.SlowDownTime(timeScale,timeSlowdownStarts,timeSlowdownEnds);
		/*//calculate time until hit begins
		float scaleValue = 1;
		
		float timeHitWillOccur = Time.realtimeSinceStartup + attackTypes[attackKey].hitBegin;
		
		float timeSlowdownStarts = timeHitWillOccur - AudioManager.beatLength + audioManager.timeTillNextBeat;
		while(timeSlowdownStarts <= timeHitWillOccur){
			timeSlowdownStarts += AudioManager.beatLength;	
		}
				
		
		float timeHitShouldOccur = audioManager.timeNextBeatStarts;
		//ensure that we will only slow time, not speed up
		while(timeHitShouldOccur <= timeHitWillOccur){
			timeHitShouldOccur += AudioManager.beatLength;	
		}
		
		float difference = timeHitWillOccur - timeHitShouldOccur;
				
		scaleValue = difference/timeHitShouldOccur;
		
		gameManager.SlowDownTime(scaleValue,timeSlowdownStarts,timeHitShouldOccur);		*/		
		
		
		// normal punch until 1 beat before hit point (if < 0.3, double up beat) 
		// half speed until 1 beat after hit point
		// play	
		
		/*float timeNextHitBegins = Time.realtimeSinceStartup + attackTypes[attackKey].hitBegin;
		
		int beatsBeforeHitBegins = 0;
		for (float time = 0; time < timeNextHitBegins; time += AudioManager.beatLength){
			beatsBeforeHitBegins ++;
		}				
		float beatBeforeHit = AudioManager.beatLength * beatsBeforeHitBegins;
		
		float beatAfterHit = beatBeforeHit + (AudioManager.beatLength*2);
		
		if(timeNextHitBegins - beatBeforeHit < 0.3){
			beatBeforeHit -= AudioManager.beatLength;	
		}*/		
		
			
		

	}
	
	public void EnableDash (EnemyScript enemy)
	{
		canDash = true;
		//gameManager.SlowDownTime(dashInputSlowdownValue,Time.realtimeSinceStartup, Time.realtimeSinceStartup + dashInputSlowdownDuration);
		
		timeDashInputFinishes = Time.realtimeSinceStartup + dashInputSlowdownDuration;
	}
	
	public void DashToAttack ()
	{
		if (canDash) {
			gameManager.EndTimeSlowdown();
			dashing = true;
		}
	}
	
	void DashToAttackCallBack ()
	{
		dashing = false;
		timeDashingFinished = Time.time;
		
		if (transform.position.y >= WorldManager.maxWorldHeight -1f) {
			gameManager.BeginCloseCombat (currentEnemy);
			
		} else {
			//Initiate queued attack
			Attack (queuedAttackKey);
			queuedAttackKey = null;
		}
	}
	
	public void DownAttack ()
	{
		downAttack = true;
		//TODO: Enable fancy particle effects here
	}
	#endregion	
	
	public void RecieveAttack (int attackPower, float attackID)
	{		
		if(transform.position.y == WorldManager.floor){// invulnerable if we are in air
			//only react to an attack if we have not recently been hit and not attcking
			if (attackID != lastEnemyAttackID && !dashing && Time.time > timeAttackHitEnds) {
				timeBeingHitFinishes = Time.time + beingHitDuration;
				
				lastEnemyAttackID = attackID;
				
				Debug.Log ("Player recieved attack for: " + attackPower + " damage");
				
				animator.SetBool ("HIT", true);
				gameManager.sFXManager.PlaySFX(gameManager.sFXManager.playerHitClip);		
				
				chargingUpperCut = false;
				chargingMidSwing = false;
					
				if(gameManager.scoreMultiplier<1)
					hitPoints -= attackPower;
				
				//check if player has died
				if (hitPoints < 1) {				
					alive = false;				
				}
				
				gameManager.ResetScoreMultiplier ();
				gameManager.ResetPowerLevel();
			}
		} else{
			if(!specialKnockUp && Time.time > timeglideDashEnds){//invulnerable if flying up or glide dashing
				//fall to floor
				fallDown = true;
				timeBeingHitFinishes = Time.time + beingHitDuration;
				animator.SetBool ("HIT", true);
				gameManager.sFXManager.PlaySFX(gameManager.sFXManager.playerHitClip);
				gameManager.ResetScoreMultiplier();
				gameManager.ResetPowerLevel();
				
				chargingUpperCut = false;
				chargingMidSwing = false;
			}
		}
	}	

	
	void CheckAnimations ()
	{		
		//attacks		
		if (Time.time >= timeAttackFinishes) {		
			foreach (KeyValuePair<string, Attack> attackType in attackTypes) {
				animator.SetBool (attackType.Key, false);
			}
		}
		
		if (!gameManager.closeCombat) {
			animator.SetBool ("COMBO2", false);	
		}
		
		if(!chargingUpperCut){
			animator.SetBool("CHARGEUPPERCUT",false);	
		}
		if(!chargingMidSwing){
			animator.SetBool("CHARGEMIDSWING",false);
		}
		
		//hit
		if (Time.time >= timeBeingHitFinishes) {
			animator.SetBool ("HIT", false);
		}
		
		//flying
		if (transform.position.y > WorldManager.floor)
			animator.SetBool ("FLY", true);
		else
			animator.SetBool ("FLY", false);
		
		if (Time.time > timeDescendFinishes)
			animator.SetBool ("DESCEND", false);
		
		if (Time.time > timeAscendFinishes)
			animator.SetBool ("ASCEND", false);
	}
	
	
	#region attack definition
	void DefineAttackTypes ()
	{
		Attack attack = new Attack ();
		attack.knockbackX = 10;
		attack.duration = 0.1f;
		attack.hitBegin = 0f;
		attack.hitEnd = attack.duration;
		attackTypes.Add (AttackKeys.JAB.ToString (), attack);
		
		attack = new Attack ();
		attack.power = 25;
		attack.knockbackX = 40;		
		attack.hitBegin = 0f;
		attack.hitEnd = 0.65f;
		attack.duration = attack.hitEnd;
		attack.knockbackDuration = 1;
		attack.knockbackPower = new Vector3(50,0,0);
		attackTypes.Add (AttackKeys.MIDSWING.ToString (), attack);
		
		attack = new Attack ();
		attack.power = 1;
		attack.knockbackX = 37;
		attack.knockbackY = 15;		
		attack.hitBegin = 0f;
		attack.hitEnd = 0.5f;
		attack.duration = attack.hitEnd;
		attack.knockbackDuration = 1;
		attack.knockbackPower = new Vector3(40,25,0);		
		attackTypes.Add (AttackKeys.UPPERCUT.ToString (), attack);
		
		attack = new Attack ();
		attack.power = 100;
		attack.knockbackX = 20;
		attack.knockbackY = WorldManager.floor - WorldManager.maxWorldHeight;
		attack.duration = 1.667f;
		attack.hitBegin = 0f;
		attack.hitEnd = attack.duration;
		attackTypes.Add (AttackKeys.DOWNSMASH.ToString (), attack);
		
		attack = new Attack ();
		attack.power = 100;	
		attack.hitBegin = 0f;
		attack.duration = glideDashDuration;
		attack.hitEnd = attack.duration;
		attack.knockbackDuration = 1;
		attack.knockbackPower = new Vector3(40,25,0);		
		attackTypes.Add (AttackKeys.GLIDEATTACK.ToString (), attack);
	}
	public enum AttackKeys
	{
		JAB,
		MIDSWING,
		UPPERCUT,
		DOWNSMASH,
		GLIDEATTACK
	}
	#endregion
}

public class Attack
{
	public int power = 1;
	public float knockbackX = 0, knockbackY = 0;
	public float duration = 0;
	public float range = 5f;
	public float rangeBelow = 7.5f, rangeAbove = 7.5f;
	public float hitBegin = 0, hitEnd = 0;
	public float uniqueID = 0;
	public float knockbackDuration = 0.1f;
	public Vector3 knockbackPower = new Vector3(6,0,0);
}
