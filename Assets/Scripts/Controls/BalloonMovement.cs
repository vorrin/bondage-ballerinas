using UnityEngine;

public class BalloonMovement : MonoBehaviour 
{
	bool facingRight = true;							// For determining which way the player is currently facing.

	[SerializeField] float maxSpeed = 10f;				// The fastest the player can travel in the x axis.
	[SerializeField] float jumpForce = 400f;			// Amount of force added when the player jumps.	

	[Range(0, 1)]
	[SerializeField] float crouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	
	[SerializeField] bool airControl = false;			// Whether or not a player can steer while jumping;
	[SerializeField] LayerMask whatIsGround;			// A mask determining what is ground to the character
	
	Transform groundCheck;								// A position marking where to check if the player is grounded.
	float groundedRadius = .2f;							// Radius of the overlap circle to determine if grounded
	public bool grounded = false;								// Whether or not the player is grounded.
	float ceilingRadius = .01f;							// Radius of the overlap circle to determine if the player can stand up
	Animator anim;
	bool jump;
									
    void Awake()
	{
		// Setting up references.
//		groundCheck = transform.Find("GroundCheck");
	}

	void Update ()
	{
		if (Input.GetKey (KeyCode.W)) jump = true;
	}
	
	void FixedUpdate()
	{
		float h = Input.GetAxis("Horizontal");
		
		// Pass all parameters to the character control script.
		Move( h, jump );
		
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
//		grounded = Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsGround);
//		anim.SetBool("Ground", grounded);

		// Set the vertical animation
		
		jump = false;
	}


	public void Move(float move, bool jump)
	{
		//only control the player if grounded or airControl is turned on
		if(grounded || airControl)
		{
			// Reduce the speed if crouching by the crouchSpeed multiplier

			// The Speed animator parameter is set to the absolute value of the horizontal input.

			// Move the character
			rigidbody2D.velocity = new Vector2(move * maxSpeed, rigidbody2D.velocity.y);
			
			// If the input is moving the player right and the player is facing left...
			if(move > 0 && !facingRight)
			{
				
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if(move < 0 && facingRight)
				// ... flip the player.
				
				Flip();
			}
	

        // If the player should jump...
        if (jump) {
			// Add a vertical force to the player.
            rigidbody2D.AddForce(new Vector2(0f, jumpForce));
        }
	}

	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
