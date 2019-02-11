using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour 
{
	[Header("Jump")]
	public float fallMultiplier = 2.5f;
	public float lowJumpMultiplier = 2f;
	public float jumpPower = 4f;

	[Space] [Header("Movement")]
	public float runSpeed;
	public float moveSpeed;
	public float backSpeed;
	float targetSpeed;
	Vector3 moveVelocity;

	//Smooth Movement
	public float smoothTime;
	float smoothInput;
	float smoothVelocity;

	//Run variables
	public float sideRunAngle;
	public float runSmoothTime;
	float desiredRot;
	float velocity;

	[Space][Header("Components")]
	public Animator anim;
	Rigidbody myRigidbody;
	Collider myCollider;
	Transform thirdPersonCam;

	//private
	public float vertical {get; private set;}
	public float horizontal {get; private set;}
	int terrainLayer; // ground layer so we can check if we are on Ground
	bool onGround;
	public bool run { get; set; }
	public bool canRun {get ; set;}
	public bool jump { get; set; } //TODO remove this variable, we already have onGround Bool
	public bool isAiming {get; set;}
	public bool isMoving { get; set; }

	[Space][Header("Linecast")]
	public Transform startPoint;
	public Transform endPoint;

	// Use this for initialization
	void Start () 
	{
		Initialize ();
		GetComponents();
	}

	void Initialize ()
	{
		isAiming = false;
		canRun = true;
		run = false;
		jump = false;
		terrainLayer = 1 << 10;
		targetSpeed = moveSpeed;
	}

	void GetComponents()
	{
		anim = GetComponentInChildren<Animator>();
		myRigidbody = GetComponent<Rigidbody>();
		myCollider = GetComponent<Collider>();
		thirdPersonCam = GameObject.FindObjectOfType<ThirdPersonCamera>().transform;
	}

	void Update()
	{
		RunInput();
	}

	void FixedUpdate () 
	{
		CheckIfOnGround();
		RigidBasedMovement(); // move our player based on camera direction
		Jumping(); // handle jumping
	}

	void FaceCameraDirection() //nie wiem po co to, ale na razie to wyłączyłem /horhe
	{	
		//float camYRot = thirdPersonCam.eulerAngles.y; // gets camera rotation on Y Axis
		//Debug.Log(camYRot);
		//transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x,camYRot,transform.rotation.z)); // rotate our player on Y Axis to be the same as Camera Y Axis
	}

	void RigidBasedMovement()
	{
		// Get our input
		vertical = Input.GetAxis("Vertical"); 
		horizontal = Input.GetAxis("Horizontal");

		//	Translate our movement from global coordinates to local coordinations eg. when moving right player will move right in his local space not iright in global space
		Vector3 v = vertical * thirdPersonCam.forward;
		Vector3 h = horizontal * thirdPersonCam.right;
		Vector3 moveDir = (v+h).normalized; // adding vertical and horizontal Vectors so we have move direction Vector, then we normalized so every  direction will be equal


		if(moveDir != Vector3.zero)
			isMoving = true;
		else
			isMoving = false;

		//Debug.Log(isMoving);

		if(vertical <= 0 || Mathf.Abs(transform.InverseTransformDirection(moveDir).x) >= 0.8f) // if there is no vertical input or player is moving only horizontal we disable his ability to run
		{
			run = false;
		}

		targetSpeed = (vertical < 0) ? backSpeed : (run == true) ? runSpeed : moveSpeed; // we assign our targetSpeed based on vertical input we check if player is moving backward, forward or forward + running 

		//this is for making our player stop almost instantly after we release our input 
		float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
		float moveAmount = Mathf.Clamp01(m);

		Vector3 move =  moveDir * (targetSpeed  * moveAmount);
	
		myRigidbody.velocity = new Vector3 (move.x, myRigidbody.velocity.y , move.z); // move our player based on calculations we make above

		HandleFacingDirection(moveDir, moveAmount);
		HandleAnimations(horizontal, vertical);
	}

	void RunInput ()
	{
		if ( Input.GetKeyDown ( KeyCode.LeftShift ) && canRun && vertical >= 0.4f )// we allow our player to run only if we are already moving forward and we can run
		{
			canRun = false;
			// this variable is for disabling ability to run after we shoot, player has to release shift button and press it again to run again 
			if ( isAiming )
			{
				thirdPersonCam.GetComponent<ThirdPersonCamera> ().ZoomOutIfRunning ();
			}
			if(GetComponent<PlayerAbilities>().charging)
				GetComponent<PlayerAbilities>().charging = false;

			run = true;
		}
		else if ( Input.GetKeyUp ( KeyCode.LeftShift ) )
		{
			canRun = true;
			run = false;
		}

		if(run)
			GetComponent<PlayerAbilities>().charging = false;
	}

	void HandleAnimations(float hor, float ver)
	{	
		if(vertical >= 0.4f)
			anim.SetBool("run", run);
		else
			anim.SetBool("run", false);

		anim.SetFloat("MoveZ",ver);
		anim.SetFloat("MoveX",hor);

		anim.SetBool("isAiming",isAiming);

		if(isAiming)
			anim.SetBool("isMoving", isMoving);
		else
			anim.SetBool("isMoving", false);
	}

	void CheckIfOnGround()
	{
		bool isGrounded = Physics.Linecast(startPoint.position,endPoint.position); 

		if(isGrounded)
		{
			onGround = true;
			jump = false;
		}
		else
		{
			onGround = false;
			jump = true;
		}

		anim.SetBool("jump",jump);
	}

	void Jumping()
	{
		if(Input.GetButtonDown("Jump") && onGround == true && jump == false)
		{
			myRigidbody.velocity = Vector3.up * jumpPower;
			onGround = false;
			jump = true;
		}

		if(myRigidbody.velocity.y  < 0) // if we are falling add extra gravity so we fall faster
		{
			myRigidbody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier -1f) * Time.fixedDeltaTime;
		}
		else if(myRigidbody.velocity.y > 0 && !Input.GetButton("Jump")) // if we are going up in air but we released our jump button add extra gravity downward so we fall faster, this code makes that if we hold our jump button we will jump higher;
		{
			myRigidbody.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier -1f) * Time.fixedDeltaTime;
		}
	}

	void HandleFacingDirection(Vector3 moveDir, float moveAmount) // Doesn't work yet but this is responsible for facing cemra direction while we run using one run animation
	{
		if(run)
		{
			moveDir.x = Mathf.Clamp (moveDir.x, -0.4f, 0.4f);
			Quaternion tr = Quaternion.LookRotation(moveDir);
			Quaternion targetRotation = Quaternion.Slerp(transform.rotation,tr,Time.deltaTime * moveAmount * runSmoothTime);
			transform.rotation = targetRotation;
		}
		else
		{
			float camYRot = thirdPersonCam.eulerAngles.y;
			transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x,camYRot,transform.rotation.z));
		}
	}

	public Rigidbody rigid
	{
		get
		{
			return myRigidbody;
		}
		set
		{
			myRigidbody = value;
		}
	}

}
