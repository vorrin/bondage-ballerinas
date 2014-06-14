using UnityEngine;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour
{
	#region variables
	public bool alive = true;
	GameObject player;
	PlayerScript playerScript;
	GameManager gameManager;
	AudioManager audioManager;
	public Animator animator;
	public float defaultHitPoints = 2, hitPoints = 0, agroRangeInner = 8, agroRangeOuter = 10;
	int hitScore = 10, killScore = 20;
	public int attackPower = 1, attackKnockback = 0;
	float XSpeed = 10f, currentXSpeed = 0,
	currentYSpeed = 0f, YSpeed = 8f, YSpeedAfterUppercut = 18f, YSpeedDeath = 30;
	float timeBetweenAttacks = AudioManager.barLength, timeAttackFinishes = 0;
	public bool attacking = false;
	float attackDuration = AudioManager.barLength;
	public Vector3 knockBackPos = Vector3.zero;
	public bool beingKnockedBack = false;
	float knockBackSpeedMultiplier = 2f;
	public float timeBeingKnockedBackEnds = 0;
	float timeBeingHit = AudioManager.beatLength, timeBeingHitFinishes = 0;
	public float combatModOffset = 2.5f;
	public int timesTapAttackHit = 0;
	public float attackRange = 4, attackHeightRangeBelow = 7.5f, attackHeightRangeAbove = 7.5f;
	float lastHitID = 0;
	public float attackID = 0;
	float timeAttackHitBegins = 0, timeAttackHitEnds = 0;
	List<EnemyScript>colliedEnemies = new List<EnemyScript> ();
	bool hitWithUppercut = false, hitWithMidSwing = true;
	public bool specialKnockUp = false;
	
	
	#endregion
	
	#region setup
	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		playerScript = player.GetComponent<PlayerScript> ();
		
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		audioManager = gameManager.gameObject.GetComponent<AudioManager> ();
		
		animator = gameObject.GetComponent<Animator> ();
		if (animator.layerCount >= 2)
			animator.SetLayerWeight (1, 1);
		
		SetUpEnemyType ();
	}
	
	public void SetUpEnemyType ()
	{
		if (animator.layerCount >= 2)
			animator.SetLayerWeight (1, 1);
		
		alive = true;
		animator.SetBool ("DIE", false);
		hitPoints = defaultHitPoints;
		attacking = false;
		beingKnockedBack = false;
		timesTapAttackHit = 0;
		hitWithUppercut = false;
		hitWithMidSwing = false;
		rigidbody.useGravity = false;
		rigidbody.velocity = Vector3.zero;
		specialKnockUp = false;
		colliedEnemies.Clear ();
	}
	#endregion
	
	// Update is called once per frame
	void Update ()
	{
		if (gameManager.gameState == GameManager.GameState.GamePlay) {
			/// Debug.Log(transform.name.ToString() +"  " + gameManager.closeCombatTarget.ToString());			
			CheckAnimations ();
				
			if (alive) {
				if (Time.time > timeAttackFinishes + timeBetweenAttacks) {				
					attacking = false;
					//try to attack player				
					Attack ();
				}
			}
			
			if ((hitWithUppercut || hitWithMidSwing) && rigidbody.velocity.x > 2) {
				float dist = 2;
				foreach (GameObject enemy in gameManager.worldManager.enemyList) {
					if (Vector3.Distance (this.transform.position, enemy.transform.position) <= dist) {
						EnemyScript enemyScript = enemy.GetComponent<EnemyScript> ();
						if (enemyScript.alive && enemyScript != this && !colliedEnemies.Contains (enemyScript)) {
							colliedEnemies.Add (enemyScript);
									
							gameManager.IncreaseScoreMultiplier ();
							
							if (hitWithUppercut) {
								enemyScript.Die ();
								Die ();
								gameManager.IncreaseScoreMultiplier ();
							} else {
								enemyScript.hitPoints --;									
								enemyScript.animator.SetBool ("KNOCKBACK", true);	
								enemyScript.timeBeingHitFinishes = Time.time + timeBeingHit;
										
								if (enemyScript.hitPoints < 1){
									enemyScript.Die ();
								}
								hitPoints --;
								if (hitPoints < 1) {
									Die ();
								} else {
									hitWithMidSwing = false;
									hitWithUppercut = false;
								}	
							}								
																		
									
							gameManager.sFXManager.PlaySFX (gameManager.sFXManager.enemyHitClip);
						}
					}
				}
			}
			
			
			if (gameManager.closeCombat == true && gameManager.closeCombatTarget == this) {
				DoMovement (true);
			} else {
				DoMovement (false);
			}	
		}
	}
	
	#region movement
	private void CalculateMovement ()
	{
		if (gameManager.closeCombat) {
		} else if (!beingKnockedBack) {
			currentXSpeed = -XSpeed;
		}
	}
	
	void DoMovement (bool closeCombatMode)
	{		
		if (closeCombatMode == false) {
			//DoFollowY();
			CalculateMovement ();
			if (beingKnockedBack) { //Knockback
				if (Time.time > timeBeingKnockedBackEnds) {
					beingKnockedBack = false;
					
					if (alive)
						rigidbody.velocity = Vector3.zero;
				}
				rigidbody.useGravity = true;
			} else if (!alive || specialKnockUp) {
				//DO NOTHING!
			} else if (playerScript.currentEnemy == this && 
					(playerScript.dashing || Time.time <= playerScript.timeDashingFinished + AudioManager.barLength)) {
				//DO NOTHING!
				rigidbody.useGravity = false;
			} else {
				//move the enemy
				if (playerScript.currentEnemy == this &&
					Time.time < playerScript.timeAttackFinishes + (AudioManager.beatLength * 2) &&
					transform.position.y > player.transform.position.y - attackHeightRangeBelow &&
					transform.position.y < player.transform.position.y + attackHeightRangeAbove) {
					/*rigidbody.useGravity = false;
					//.position = new Vector3 (playerScript.distanceFromCurrentEnemy + player.transform.position.x, transform.position.y, transform.position.z);
					Vector3 targetPos = new Vector3 (player.transform.position.x + playerScript.generalCombatDistance, transform.position.y, transform.position.z);
				
					if (Vector3.Distance (transform.position, targetPos) < 0.1f) {
						transform.position = Vector3.Lerp (transform.position, targetPos, audioManager.timeTillNextBeatLerp);
					} else {
						transform.position = targetPos;
					}*/
				} else {
					rigidbody.useGravity = false;
					//move to correct height to attack sirian
					if (player.transform.position.x < transform.position.x) {
						if (transform.position.y < player.transform.position.y + WorldManager.enemySpawnHeightAbovePassive) {
							if (player.transform.position.y < transform.position.y - attackHeightRangeBelow) {
								if (hitWithUppercut) {
									currentYSpeed = -YSpeedAfterUppercut;
								} else
									currentYSpeed = -YSpeed;
							} else if (player.transform.position.y > transform.position.y + attackHeightRangeAbove)
								currentYSpeed = YSpeed;
							else
								currentYSpeed = 0;
						}
					}
					if(!specialKnockUp){
					transform.Translate (new Vector3 (currentXSpeed * Time.deltaTime, currentYSpeed * Time.deltaTime, 0), Space.World);
					}
				}
			}
		} else {
			rigidbody.useGravity = false;
			Vector3 newPos = player.transform.position;
			newPos.x += combatModOffset;
			transform.position = newPos;
		}
		
		//make sure we dont fall through floor	
		if (transform.position.y < WorldManager.floor) {
			transform.position = new Vector3 (transform.position.x, WorldManager.floor, transform.position.z);
			if (!alive)
				rigidbody.drag = 1f;
		}	
	}
	#endregion
	
	
	void Attack ()
	{
		//only attack if we are not currently attacking or in close combat
		if (Time.time > timeAttackFinishes + timeBetweenAttacks && 
			Time.time > timeBeingHitFinishes + AudioManager.beatLength &&
			!beingKnockedBack &&
			!gameManager.closeCombat && 
			!(playerScript.dashing || Time.time <= playerScript.timeDashingFinished + AudioManager.barLength) && 
			player.transform.position.x > transform.position.x - attackRange &&
			player.transform.position.x < transform.position.x &&
			player.transform.position.y > transform.position.y - attackHeightRangeBelow &&
			player.transform.position.y < transform.position.y + attackHeightRangeAbove
			) {
			attackID = Random.value;
			
			playerScript.RecieveAttack (attackPower, attackID);
			
			animator.SetBool ("ATTACK", true);
			attacking = true;
			timeAttackFinishes = Time.time + attackDuration;
		}
	}
	
	#region recieve attack logic	
	public void RecieveAttack ()
	{			
		if (!beingKnockedBack && playerScript.attackTypes [playerScript.currentAttackKey].uniqueID != lastHitID) {
			Debug.Log ("Enemy recieved attack type: " + playerScript.currentAttackKey);
			
			lastHitID = playerScript.attackTypes [playerScript.currentAttackKey].uniqueID;
			
			animator.SetBool ("KNOCKBACK", true);
			
			gameManager.sFXManager.PlaySFX (gameManager.sFXManager.enemyHitClip);
		
			timeBeingHitFinishes = Time.time + timeBeingHit;
			
			hitPoints -= playerScript.attackTypes [playerScript.currentAttackKey].power;
			
			if (playerScript.currentAttackKey == PlayerScript.AttackKeys.UPPERCUT.ToString ()) {
				playerScript.currentEnemy = this;
				playerScript.distanceFromCurrentEnemy = transform.position.x - player.transform.position.x;
			}
			//else if (playerScript.currentEnemy == this){
			//	playerScript.currentEnemy = null;
			//}
			
			if (playerScript.currentAttackKey == PlayerScript.AttackKeys.DOWNSMASH.ToString ()) {
				//TODO: make enemy drop with player
			} else if (playerScript.currentAttackKey == PlayerScript.AttackKeys.JAB.ToString ()) {
				timesTapAttackHit ++;
			} else if (playerScript.currentAttackKey == PlayerScript.AttackKeys.UPPERCUT.ToString ()) {
				hitWithUppercut = true;	
			} else if (playerScript.currentAttackKey == PlayerScript.AttackKeys.MIDSWING.ToString ()) {
				hitWithMidSwing = true;	
			}
			
			/*knockBackPos = new Vector3 (transform.position.x + playerScript.attackTypes [playerScript.currentAttackKey].knockbackX, 
				transform.position.y + playerScript.attackTypes [playerScript.currentAttackKey].knockbackY, transform.position.z);							
			
			
			if (knockBackPos.y < WorldManager.floor)
				knockBackPos.y = WorldManager.floor;
			else if (knockBackPos.y > WorldManager.maxWorldHeight)
				knockBackPos.y = WorldManager.maxWorldHeight;*/
		
			//if (playerScript.currentAttackKey != PlayerScript.AttackKeys.JAB.ToString()) {
			beingKnockedBack = true;
			timeBeingKnockedBackEnds = Time.time + playerScript.attackTypes [playerScript.currentAttackKey].knockbackDuration;
			if(playerScript.currentAttackKey == PlayerScript.AttackKeys.UPPERCUT.ToString()){
				rigidbody.AddForce ( playerScript.currentUpperCutKnockBackPower, ForceMode.Impulse);
			}
			else
				rigidbody.AddForce (playerScript.attackTypes [playerScript.currentAttackKey].knockbackPower, ForceMode.Impulse);
			//}
			
			
			gameManager.IncreaseScore ((int)(hitScore * gameManager.scoreMultiplier));
			
			gameManager.cameraControlScript.SetLookAtTarget (this.transform.gameObject, AudioManager.barLength);
			
			
			if (hitPoints < 1) {
				
				Die ();
				
				if (playerScript.currentAttackKey == PlayerScript.AttackKeys.UPPERCUT.ToString ()) {
					playerScript.EnableDash (this);
				}
			}
		}
	}
	
	public void Die ()
	{
		hitPoints = 0;
		alive = false;
		animator.SetBool ("DIE", true);
		animator.SetLayerWeight (1, 0);
		
		rigidbody.useGravity = true;
				
		if (playerScript.currentEnemy == this)
			playerScript.currentEnemy = null;
				
		if (gameManager.closeCombat && gameManager.closeCombatTarget == this)
			gameManager.EndCloseCombat ();
		
		gameManager.IncreaseScore((int)(killScore * gameManager.scoreMultiplier));
	}
	#endregion
	
	void CheckAnimations ()
	{
		//movement
		animator.SetFloat ("SPEED", currentXSpeed);
		
		//flight
		if (transform.position.y == WorldManager.floor)
			animator.SetBool ("FLY", false);
		else
			animator.SetBool ("FLY", true);
		
		//combat
		animator.SetBool ("ATTACK", false);	
		
		if (Time.time > timeBeingHitFinishes)
			animator.SetBool ("KNOCKBACK", false);						
		
	}
}