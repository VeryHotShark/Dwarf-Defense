using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPos : MonoBehaviour 
{
	PlayerAbilities playerAbilities;
	PlayerMovement playerMovement;
	public Transform rotInIdle;
	public Transform initTransfrom;
	public Transform rotInRun;
	public Transform leftHand;

	bool rotChanged;
	bool rotReset;
	// Use this for initialization
	void Start () 
	{
		playerMovement = GetComponentInParent<PlayerMovement>();
		playerAbilities = playerMovement.GetComponent<PlayerAbilities>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(playerMovement.run)
			ChangeRotInRun();
		else if((playerMovement.vertical >= 0.4f && playerMovement.horizontal <= -0.4f && !playerMovement.isAiming) || (playerMovement.vertical <= -0.4f && playerMovement.horizontal >= 0.4f && !playerMovement.isAiming))
			ChangeRotToLeftHand();
		else if((playerMovement.vertical >= 0.4f && playerMovement.horizontal >= 0.4f) || (playerMovement.vertical <= -0.4f && playerMovement.horizontal <= -0.4f) || (Mathf.Abs(playerMovement.vertical) >= 0.4f) || (Mathf.Abs(playerMovement.horizontal) >= 0.4f) || playerMovement.isAiming)
			ChangeRotToInit();
		else if(playerAbilities.shootingInIdle && !playerMovement.isMoving)
			ChangeRotToShootIdle();
		else if((Mathf.Abs(playerMovement.vertical) < 0.4f && Mathf.Abs(playerMovement.horizontal) < 0.4f)  && !playerMovement.isAiming)
			ChangeRotInIdle();
	}

	void ChangeRotInIdle()
	{
		transform.localRotation = Quaternion.Lerp(transform.localRotation,rotInIdle.localRotation, Time.deltaTime * 10f);
		transform.localPosition = Vector3.Lerp(transform.localPosition,initTransfrom.localPosition,Time.deltaTime * 10f);
	}

	void ChangeRotToInit()
	{
		transform.localRotation = Quaternion.Lerp(transform.localRotation,initTransfrom.localRotation, Time.deltaTime * 10f);
		transform.localPosition = Vector3.Lerp(transform.localPosition,initTransfrom.localPosition,Time.deltaTime * 10f);
	}

	void ChangeRotInRun()
	{
		transform.localRotation = Quaternion.Lerp(transform.localRotation,rotInRun.localRotation, Time.deltaTime * 10f);
		transform.localPosition = Vector3.Lerp(transform.localPosition,initTransfrom.localPosition,Time.deltaTime * 10f);
	}

	void ChangeRotToLeftHand()
	{
		transform.localRotation = Quaternion.Lerp(transform.localRotation,leftHand.localRotation,Time.deltaTime * 10f);
		transform.localPosition = Vector3.Lerp(transform.localPosition,leftHand.localPosition,Time.deltaTime * 10f);
	}

	void ChangeRotToShootIdle()
	{
		transform.localRotation = Quaternion.Lerp(transform.localRotation,initTransfrom.localRotation, Time.deltaTime * 100f);
		transform.localPosition = Vector3.Lerp(transform.localPosition,initTransfrom.localPosition,Time.deltaTime * 100f);
	}
}
