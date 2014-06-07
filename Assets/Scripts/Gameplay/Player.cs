using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	[HideInInspector] public PlayerControllerData playerControllerData;
	
//	public GameObject modelContainer;
//	[HideInInspector] public bool isInvulnerable = false;
//	
//	public float speed = 17f;
//	public tk2dSpriteAnimator animator;
//	public Transform umbrellaRotator;
//	public tk2dSprite talkToTheHand;
//	//public Pillow pillowPrefab;
//	public AudioClip grabPillow;
//	public tk2dSpriteCollectionData otherArmData;
//
//	private bool onGround = false;
//	private bool onGroundLastTick = false;
//	private bool walkingRight = true;
//	private int onGroundCounter = 0;
//	private float jumpTimer = 0;
//	private float extraJumpEnergy = 1;
//	//private Pillow myPillow;
//	private float knockOutTimer = 0;
//	private bool knockedOut = false;
//	private bool inFortArea = false;
//	private float pillowCoolDown = 0;
//
//	//private Pillow pillow;
//
//	void Awake() {
//	}
//	
//	void Start() {
//		SetText();
//		Physics.IgnoreLayerCollision(this.gameObject.layer, 12, false);
//		
//		//tk2dSprite s = animator.GetComponent<tk2dSprite>();
//		//s.color = playerControllerData.playerColor * 8f;
//		int pid = ((playerControllerData.playerId % 4) + 1);
//		if (pid == 1)
//			talkToTheHand.SetSprite("Player_0" + 1 + "_PointerArm");
//		else
//			talkToTheHand.SetSprite(otherArmData, "Player_0" + pid + "_PointerArm");
//	}
//	
//	void FixedUpdate() {
//
//		onGround = onGroundLastTick;
//		onGroundLastTick = false;
//
//		if (this.playerControllerData == null) return;
//		//Physics.IgnoreLayerCollision(this.gameObject.layer, 12, (this.rigidbody.velocity.y > 0.0f));
//		
//		jumpTimer += Time.deltaTime;
//
//		Vector3 inputDirection = GetDirection();
//		Vector3 lookDirection = GetAimDirection();
//		
//		//if (lookDirection.magnitude > 0.5f)
//		if (knockOutTimer > 0)
//		{
//			if (!knockedOut)
//			{
//				knockedOut = true;
//				Go.to (animator.transform, 0.4f, new GoTweenConfig().eulerAngles(new Vector3(0,0,90), true).setEaseType(GoEaseType.BounceOut));
//			}
//			knockOutTimer -= Time.deltaTime;
//			return;
//		}
//		else if (knockedOut)
//		{
//			knockedOut = false;
//			Go.to (animator.transform, 0.4f, new GoTweenConfig().eulerAngles(new Vector3(0,0,-90), true).setEaseType(GoEaseType.BounceOut));
//		}
//		pillowCoolDown -= Time.deltaTime;
//		if (pillowCoolDown < 0 && inFortArea)
//			umbrellaRotator.gameObject.SetActive(true);
//		bool shooting = ShootButtonPressed();
//		//if (shooting && pillowCoolDown < 0 && GameController.instance.pillowFort.currentHealth > 0)
//		//{
//		//	ThrowPillow();
//		//	pillowCoolDown = 1;
//		//	umbrellaRotator.gameObject.SetActive(false);
//		//}
//
//		bool jumped = JumpButtonPressed();
//
//		if (inputDirection.magnitude > 0.6f || jumped) {
//			if (onGround) 
//				animator.Play("PlayerWalk" + ((playerControllerData.playerId % 4) + 1));
//			LookInDirection(inputDirection);
//
//			float currentSpeed = this.rigidbody.velocity.magnitude;
//			Vector3 leftRightDir = Vector3.right * inputDirection.x;
//			//if (umbrellaOpen) leftRightDir *= 0.75f;
//			if (!onGround) leftRightDir *= 0.75f;
//			float currentLeftRight = Mathf.Abs(this.rigidbody.velocity.x);
//			if (currentLeftRight < this.speed * Time.deltaTime)
//				this.rigidbody.velocity = Vector3.up * this.rigidbody.velocity.y + leftRightDir * this.speed * Time.deltaTime;
//			
//			if (onGround && jumped && jumpTimer > 0.1f)
//			{
//				//animator.Play("PlayerJumpUp");
//				jumpTimer = 0;
//				onGround = false;
//				Debug.Log("Jump!");
//				extraJumpEnergy = 0.5f;
//				// start jump
//				float jumpForce = 500 * this.rigidbody.mass;
//				//if (!umbrellaOpen) jumpForce = jumpForce * 2f;
//				this.rigidbody.AddExplosionForce(jumpForce, this.transform.position + Vector3.down, 0, 1);
//			}
//			else if (!onGround && jumped && extraJumpEnergy > 0)
//			{
//				extraJumpEnergy -= Time.deltaTime;
//				this.rigidbody.AddForce(Vector3.up * 7 * this.rigidbody.mass);
//			}				
//
//			
//			if (!onGround && !jumped )
//			{
//				extraJumpEnergy = 0;
//			}	
//
//			
//			if (!onGround )
//			{
//				this.rigidbody.AddForce(Vector3.down * 10 * this.rigidbody.mass);
//				
//			}
//
//
//			
//			// lock position to inside window
//			this.transform.position = new Vector3(Mathf.Clamp(this.transform.position.x, -GameController.instance.horizontalExtent, GameController.instance.horizontalExtent), this.transform.position.y, this.transform.position.z);
//			
//		} else {
////			this.playerControllerData.SetButtonText("Respawn in " + ((int)(respawnInterval - this.timeSinceDying)).ToString());
////			if (this.timeSinceDying > respawnInterval) {
////				this.timeSinceDying = 0f;
////				this.isDead = false;
////				this.modelContainer.SetActive(true);
////				GameController.instance.Respawn(this);
////			}
//		}
//
//		if (walkingRight == (this.rigidbody.velocity.x > 0) && this.rigidbody.velocity.magnitude > 0.1f)
//		{
//			// flip direction
//			walkingRight = !walkingRight;
//			tk2dSprite s = animator.GetComponent<tk2dSprite>();
//			s.scale = new Vector3(s.scale.x * -1, 1, 1);
//		}
//
//		if (onGround)
//		{
//			if (this.rigidbody.velocity.magnitude < 0.1f)
//				animator.Play("PlayerIdle" + ((playerControllerData.playerId % 4) + 1));
//			else
//				animator.Play("PlayerWalk" + ((playerControllerData.playerId % 4) + 1));				
//		}
//		else
//		{			
//			if (this.rigidbody.velocity.y < -0.1f)
//				animator.Play("PlayerJumpDown" + ((playerControllerData.playerId % 4) + 1));
//			else if (this.rigidbody.velocity.y > 0.1f)
//			{
//				animator.Play("PlayerJumpUp" + ((playerControllerData.playerId % 4) + 1));
//			}
//			else
//				animator.Play("PlayerIdle" + ((playerControllerData.playerId % 4) + 1));
//				
//		}
//
//		// limit velocity
//		float maxVelocity = 10f;
//		if (this.rigidbody.velocity.magnitude > maxVelocity)
//		{
//			this.rigidbody.velocity = this.rigidbody.velocity.normalized * maxVelocity;
//		}
//
////		if (Input.GetKey(KeyCode.K))
////		{
////			Debug.Log ("Knocking out!");
////			KnockOut(4);
////		}
//
//	}
//	
//	public void SetText() {
//		if (this.playerControllerData != null)
//			this.playerControllerData.SetButtonText("Play!");
//		
//	}
//	
//		
//	public void IncrementScore(int inc) 
//	{
//
//	}
//
//	
//	public void SetControllerData(PlayerControllerData pcd)
//	{
//		this.playerControllerData = pcd;		
//	}
//	
//	private Vector3 GetAimDirection()
//	{
//		Vector3 v = new Vector3(0,0,0);
//		if (this.playerControllerData.controllerId < 1) return v;
//		v.x = Input.GetAxis("Aim_x" + this.playerControllerData.controllerId);
//		v.y = Input.GetAxis("Aim_y" + this.playerControllerData.controllerId);			
//		
//		return v;
//	}
//	
//	private Vector3 GetDirection()
//	{
//		Vector3 v = new Vector3(0,0,0);
//		
//		if (this.playerControllerData.controllerId == 0) 
//		{
//			// Keyboard arrow keys
//			v.x = (Input.GetKey("left")?-1:0) + (Input.GetKey("right")?1:0);
//			v.y = (Input.GetKey("up")?1:0) + (Input.GetKey("down")?-1:0);
//		}
//		else if (this.playerControllerData.controllerId == -1) 
//		{
//			// Keyboard wasd
//			v.x = (Input.GetKey("a")?-1:0) + (Input.GetKey("d")?1:0);
//			v.y = (Input.GetKey("w")?1:0) + (Input.GetKey("s")?-1:0);
//		}
//		else 
//		{
//			if (this.playerControllerData.controllerLayout == PlayerControllerData.ONEPLAYER 
//			|| this.playerControllerData.controllerLayout == PlayerControllerData.TWOPLAYERLEFT )
//			{
//				v.x = Input.GetAxis("Move_x" + this.playerControllerData.controllerId);
//				v.y = Input.GetAxis("Move_y" + this.playerControllerData.controllerId);
//			}
//			else
//			{
//				v.x = Input.GetAxis("Aim_x" + this.playerControllerData.controllerId);
//				v.y = Input.GetAxis("Aim_y" + this.playerControllerData.controllerId);						
//			}
//		}
//		
//		return v;
//	}
//	
//	// returns true if the fire button has been pressed
//	private bool ShootButtonPressed()
//	{
//		if (!inFortArea) return false;
//		if (playerControllerData.controllerId == 0) 
//		{
//			// Keyboard arrow keys
//			return Input.GetKeyDown(KeyCode.RightShift);
//		}
//		else if (playerControllerData.controllerId == -1) 
//		{
//			// Keyboard wasd
//			return Input.GetKeyDown(KeyCode.LeftShift);
//		}
//		else 
//		{
//			if (playerControllerData.controllerLayout == PlayerControllerData.ONEPLAYER )
//			{
//				return Input.GetButton("Fire1_g" + playerControllerData.controllerId);
//				
//			}
//			else if (playerControllerData.controllerLayout == PlayerControllerData.TWOPLAYERLEFT)
//			{
//				return Input.GetButtonDown("Fire1_s" + ((playerControllerData.controllerId * 2) - 1));
//			}
//			else
//			{
//				return Input.GetButtonDown("Fire1_s" + (playerControllerData.controllerId * 2));					
//			}
//		}
//	}
//
//	// returns true if the fire button has been pressed
//	private bool JumpButtonPressed()
//	{
//		if (playerControllerData.controllerId == 0) 
//		{
//			// Keyboard arrow keys
//			return Input.GetKeyDown(KeyCode.Return);
//		}
//		else if (playerControllerData.controllerId == -1) 
//		{
//			// Keyboard wasd
//			return Input.GetKeyDown(KeyCode.CapsLock);
//		}
//		else 
//		{
//			if (playerControllerData.controllerLayout == PlayerControllerData.ONEPLAYER )
//			{
//				return Input.GetButton("Jump1_g" + playerControllerData.controllerId);
//				
//			}
//			else if (playerControllerData.controllerLayout == PlayerControllerData.TWOPLAYERLEFT)
//			{
//				return Input.GetButtonDown("Fire1_s" + ((playerControllerData.controllerId * 2) - 1));
//			}
//			else
//			{
//				return Input.GetButtonDown("Fire1_s" + (playerControllerData.controllerId * 2));					
//			}
//		}
//	}
//
//	bool CheckOnGround(Collision c)
//	{
//		float threshold = 0.2f;
//		foreach (ContactPoint cp in c.contacts)
//		{
//			//Debug.Log("Collision: " + cp.point + " Normal: " +cp.normal + " dot: " + Vector3.Dot(cp.normal, Vector3.up));
//			if (Vector3.Dot((cp.point - this.transform.position).normalized, Vector3.up) < threshold)
//			{
//				return true;
//			}
//		}
//		return false;
//	}
//
//
//	public void OnCollisionStay(Collision c)
//	{
//		OnCollisionEnter(c);
//	}
//
//
//	public void OnCollisionEnter(Collision c)
//	{
//		Collider other = c.collider;
//
//
//		if (CheckOnGround(c) && (other.tag.Equals("floor") || other.tag.Equals("Player")))
//		{		
//			//onGroundCounter++;
//			onGroundLastTick = true;
//			onGround = true;
//			//animator.Play("Idle");
//			
//		}
//	}
//	
//	public void OnTriggerEnter(Collider other)
//	{
//		if (other.tag.Equals("floor") || other.tag.Equals("Player"))
//		{
//			
//			
//			//onGroundCounter++;
//			//onGround = true;
//			animator.Play("PlayerIdle"+ ((playerControllerData.playerId % 4) + 1));
//			
//		}
//
//		if (other.name.Equals("FortArea") )
//		{
//			inFortArea = true;
//			//if (myPillow != null)
//			umbrellaRotator.gameObject.SetActive(true);
//		}
//
////		if (myPillow == null && other.tag.StartsWith("pillow"))
////		{
////			// it's a pillow
////			PickupPillow(other.gameObject);
////
////		}
//
////		if (other.name.StartsWith("Robot") && GameController.instance.robotsActive)
////		{
////			if (myPillow != null)
////				DropMyPillow(true);
////			KnockOut(1);
////
////		}
//
//	}
//
////	void ThrowPillow()
////	{
////		//Debug.Log("Trying to throw pillow ... aim dir: " + GetDirection());
////		// instantiate pillow
////
////		Pillow p = Instantiate(pillowPrefab, this.transform.position + GetDirection(), Quaternion.identity) as Pillow;
////		p.lastOwnerTime = GameController.instance.gameTime;
////		p.lastOwner = this;
////		LookInDirection(GetDirection());
////		DropMyPillow(false);
////		Vector3 dir = GetDirection();
////		if (dir.y < 0) dir = dir - Vector3.up * dir.y;
////		p.rigidbody.AddForce(dir * 600);
////		p.rigidbody.AddTorque(Vector3.back * Random.Range(-10, 10));
////		//Debug.Log("Thrown Pillow!");
////		GameController.instance.pillowFort.SubstractOne();
////		//p.Explode();
////
////	}
//
//	public void KnockOut(float sec)
//	{
//		//knockedOut = false;
//		knockOutTimer = sec;
//	}
//
////	public void DropMyPillow(bool explode)
////	{
////		if (myPillow != null)
////		{
////			myPillow.lastOwner = this;
////			myPillow.lastOwnerTime = GameController.instance.gameTime;
////			myPillow.owner = null;
////			myPillow.transform.parent = null;
////			myPillow.rigidbody.isKinematic = false;
////			myPillow.transform.position +=  Vector3.forward * 2;
////			Pillow p = myPillow;
////			myPillow = null;
////			if (explode) p.Explode();
////			
////			umbrellaRotator.gameObject.SetActive(false);
////		}
////	}
//
////	void PickupPillow(GameObject pillowObject)
////	{
////		Pillow p = pillowObject.transform.parent.GetComponent<Pillow>();
////		AudioSource.PlayClipAtPoint(grabPillow, this.transform.position);
////		if (p == null)
////		{
////			Debug.Log ("ERROR: NO PILLOW!");
////			return;
////		}
////		if (p.owner == null)
////		{
////			if (p.lastOwner == this && (GameController.instance.gameTime - p.lastOwnerTime < 0.5f) || p.IsSuperDangerous())
////				return;
////			// pick up!
////			p.owner = this;
////			myPillow = p;
////			p.transform.parent = this.transform;
////			p.transform.position = this.transform.position + this.GetDirection() + Vector3.back * 2;
////			p.rigidbody.isKinematic = true;
////			if (inFortArea) 
////				umbrellaRotator.gameObject.SetActive(true);
////		}
////	}
//
//	public void OnTriggerExit(Collider other)
//	{
//		
//		if (other.name.Equals("FortArea"))
//		{
//			inFortArea = false;
//			umbrellaRotator.gameObject.SetActive(false);
//			
//		}
//
//		
//	}
//
////	public void LookInDirection(Vector3 direction) {
////		float desiredAngle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
////		umbrellaRotator.transform.rotation = Quaternion.Euler(0, 0, desiredAngle);
////		if (myPillow != null)
////		{
////			myPillow.transform.position = this.transform.position + direction * 0.3f  + Vector3.back * 2;
////		}
////	}
//	
//	private void BoundTransformLocation() {
//		float xBound = GameController.instance.horizontalExtent - this.collider.bounds.extents.x;
//		float yBound = GameController.instance.verticalExtent - this.collider.bounds.extents.y;
//
//		this.transform.position = new Vector3(Mathf.Clamp(transform.position.x, -xBound, xBound),
//			Mathf.Clamp(transform.position.y, -yBound, yBound),
//			0f);
//	}
//	
//	void LateUpdate() {
//		//BoundTransformLocation();
//	}
}
