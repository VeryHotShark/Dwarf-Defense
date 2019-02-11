using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour 
{
	// GameObjects & Components
	public Transform camStartPos;
	public Transform ZoomPos;
	public GameObject playerToFollow;
	public Transform pivot; // empty gameObject which we use to rotate our camera on X axis
	public Camera myCam {get;set;}
	PlayerMovement playerMovement;

	// Smooth variables
	Vector3 velocity;  // Ref variable
	public float smoothTime;
	public float zoomTime;

	// Private variables
	Vector3 offset;
	Vector3 desiredPos;
	Vector3 zoomVelocity; // ref variable 

	// Mouse Rotation variables
	public bool invertAxis = false;
	public float mouseSensitivity;
	public float smoothRotateTime;
	public Vector2 pitchMinMax;
	float yaw = 180f, pitch;

	Coroutine zoomIn;
	Coroutine zoomOut;

	void OnEnable()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	// Use this for initialization
	void Start () 
	{
		offset = playerToFollow.transform.position - transform.position;
		myCam = GetComponentInChildren<Camera>();
		playerMovement = GameObject.FindObjectOfType<PlayerMovement>();

	}

	void Update()
	{
		HandleAim();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		FollowPlayer();
		RotateCamera();
	}

	void FollowPlayer()
	{
		if(playerToFollow != null)
		{
			desiredPos = playerToFollow.transform.position + offset;
			transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);
		}
	}

	void RotateCamera()
	{
		yaw += Input.GetAxis("Mouse X") * mouseSensitivity;

		if(invertAxis)
			pitch += Input.GetAxis("Mouse Y") * mouseSensitivity;
		else
			pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

		pitch = Mathf.Clamp(pitch,pitchMinMax.x,pitchMinMax.y); // clamp x rotation so we can't rotate undere the ground or upside down the camera

		Quaternion desiredPitchRot = Quaternion.Euler(pitch,0f,0f);
		pivot.localRotation = Quaternion.Lerp(pivot.localRotation,desiredPitchRot,Time.fixedDeltaTime * smoothRotateTime);

		Quaternion desiredYawRot = Quaternion.Euler(0f,yaw,0f);
		transform.rotation = Quaternion.Lerp(transform.rotation,desiredYawRot,Time.fixedDeltaTime * smoothRotateTime);

	}

	void HandleAim()
	{
		if(Input.GetMouseButtonDown(1))
		{
			if(zoomOut != null)
				StopCoroutine(zoomOut);

			if(playerMovement.run)
				playerMovement.run = false;

			zoomIn = StartCoroutine(CameraZoom());
			playerMovement.isAiming = true;
		}

		if(Input.GetMouseButtonUp(1))
		{
			if(zoomIn != null)
				StopCoroutine(zoomIn);

			zoomOut = StartCoroutine(ZoomOut());
			playerMovement.isAiming = false;
		}
	}

	public IEnumerator CameraZoom()
	{
		float zoomSpeed =  1f / zoomTime;
		float percent = 0f;
		while(percent < 1f)
		{
			percent += Time.deltaTime * zoomSpeed;
			myCam.transform.localPosition = Vector3.Lerp(myCam.transform.localPosition, ZoomPos.localPosition,percent);
			yield return null;
		}
	}

	public IEnumerator ZoomOut()
	{
		float zoomSpeed =  1f / zoomTime;
		float percent = 0f;
		while(percent < 1f)
		{
			percent += Time.deltaTime * zoomSpeed;
			myCam.transform.localPosition = Vector3.Lerp(myCam.transform.localPosition, camStartPos.localPosition,percent);
			yield return null;
		}
	}

	public void ZoomOutIfRunning()
	{
		if(zoomIn != null)
			StopCoroutine(zoomIn);

		zoomOut = StartCoroutine(ZoomOut());
		playerMovement.isAiming = false;
	}

	public void ChangeMouseSensitivity(float sensitivity)
	{
		mouseSensitivity = sensitivity;
	}
}
