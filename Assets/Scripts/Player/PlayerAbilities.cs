using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilities : MonoBehaviour 
{
	public Fade rageFade;
	public float timeBetweenShots;
	public float chargeDuration;
	//public float rewindDuration;
	public Projectile projectile;
	public float projectileDamageVaration = 2f;
	public Transform projectileSpawnPos;

	Transform camera; //TODO remove this varaiable
	PlayerMovement player;
	PlayerController playerController;
	ThirdPersonCamera myCam;

	public RectTransform crosshairRect;

	//List<PosRot> transformPoints;
	//public bool rewinding {get;set;}

	public bool rage { get; set;}
	[Space] [Header("Abilities")]
	[SerializeField] float rageMultiplier = 1.5f;
	[SerializeField] float rageDuration;
	[SerializeField] float rageMana = 30f;
	[SerializeField] bool areaShot;
	[SerializeField] float areaRadius;
	[SerializeField] float areaMana = 40f;

	public float manaPerSec = 1f;

	private float timeSinceLastShot;
	public bool charging;
	float chargeAmount;

	public bool shootingInIdle {get;set;}

	// Use this for initialization
	void Start () 
	{
		//transformPoints = new List<PosRot>();
		player = GetComponent<PlayerMovement>();
		playerController = GetComponent<PlayerController>();
		camera = Camera.main.transform;
		myCam = GameObject.FindObjectOfType<ThirdPersonCamera>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		RecoverMana();
		Rage();
		AreaShot();
		HandleShooting ();
	}

	void RecoverMana()
	{
		playerController.SubstractMana(-manaPerSec * Time.deltaTime);
	}

	void Rage ( )
	{
		if ( Input.GetKeyDown ( KeyCode.Q ) && rage == false && rageMana <= playerController.playerMana)
		{
			playerController.SubstractMana(rageMana);
			StartCoroutine ( Raging ( ) ) ;
		}
	}

	IEnumerator Raging()
	{
		rage = true;

		StartCoroutine(rageFade.FadeInRage());

		yield return new WaitForSeconds(rageDuration);

		rage = false;

		StartCoroutine(rageFade.FadeOutRage());
	}

	void AreaShot()
	{
		if(Input.GetKeyDown(KeyCode.E) && areaShot == false && areaMana <= playerController.playerMana)
		{
			playerController.SubstractMana(areaMana);
			areaShot = true;

			StopRunIfShoot();
			CalculateProjectileRotation ();
		}
	}

	void StopRunIfShoot ()
	{
		if ( player.run )// if we shoot while running we make our player to cancel run 
		{
			player.run = false;
			player.canRun = false;
			player.anim.SetBool ( "run" , player.run );
		}
		player.anim.SetBool ( "shooting" , shootingInIdle );
	}


	void HandleShooting ()
	{
		if(Input.GetMouseButtonDown(0) && !player.jump && Time.time > timeSinceLastShot + timeBetweenShots )
		{
			charging = true;
		}

		if (charging)
		{
			float chargeSpeed = 1f / chargeDuration;
			chargeAmount += Time.deltaTime * chargeSpeed;
			chargeAmount = Mathf.Clamp01 (chargeAmount);
			AnimateCrossHair ();
			//Debug.Log (chargeAmount);
		}
		else
		{
			StopAnimateCrossHair ();
			chargeAmount = 0f;
		}
			

		if ( Input.GetMouseButtonUp ( 0 ) && !player.jump )// we can only shoot if we are not jumping
		{
			if(!player.isMoving)
				shootingInIdle = true;
			else
				shootingInIdle = false;

			CalculateProjectileRotation ();
			timeSinceLastShot = Time.time;
			StopRunIfShoot ();
		}

		if ( Time.time > timeSinceLastShot + 0.4f )
		{
				player.anim.SetBool ( "shooting" , shootingInIdle );
				shootingInIdle = false;
		}
	}

	void AnimateCrossHair()
	{
		if(charging)
		{
			crosshairRect.localScale = Vector3.Lerp (crosshairRect.localScale, Vector3.one / 2f, chargeAmount);
		}
	}

	void StopAnimateCrossHair()
	{
		crosshairRect.localScale = Vector3.Lerp (crosshairRect.localScale, Vector3.one, Time.deltaTime * 50f);
	}

	void CalculateProjectileRotation()
	{
		Vector3 dirToTarget;
		Quaternion rotToTarget;

		Ray ray = myCam.myCam.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0f));
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit,1000f))
		{
			dirToTarget = (hit.point - projectileSpawnPos.position).normalized;
			rotToTarget = Quaternion.FromToRotation (projectile.transform.forward, dirToTarget);
		}
		else
		{
			dirToTarget = ray.direction;
			rotToTarget = myCam.myCam.transform.rotation;
		}

		SpawnProjectile (dirToTarget, rotToTarget);
	}

	void SpawnProjectile(Vector3 dir, Quaternion rot)
	{
		Projectile myProjectile  = Instantiate(projectile,projectileSpawnPos.position,rot) as Projectile;
		myProjectile.damageVaration = projectileDamageVaration;

		if(rage)
			myProjectile.SetDamage = Mathf.Round(myProjectile.SetDamage * rageMultiplier);

		if(areaShot)
		{
			myProjectile.areaShot = true;
			myProjectile.areaRadius = areaRadius;
			areaShot = false;
		}

		if (charging && chargeAmount >= 0.6f)
		{
			//CameraShake.isShaking = true;
			myProjectile.SetSpeed = 40;
			myProjectile.SetDamage = 50f;
		}
		charging = false;
	}

}