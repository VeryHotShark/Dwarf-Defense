using UnityEngine;
using UnityEngine.UI;

public class Projectile : MonoBehaviour 
{
	public GameObject hitVFX;

	public RectTransform hitMark;
	public LayerMask layerMask; // layer that our projectille will colllide with
	public float damage;
	public float speed;

	//Components
	Rigidbody myRigid;
	Camera myCamera;

	// Private Variables
	float moveAmountInNextFrame; // variable that we pass in to check if our projectile will hit something in the next frame
	public bool areaShot{get;set;}
	public float areaRadius{get;set;}
	public Vector3 dirToTarget{get;set;}

	public float damageVaration { get; set;}

	// Use this for initialization
	void Start () 
	{
		GetComponents();
		Collider[] colliders = Physics.OverlapSphere(transform.position,0.3f,layerMask); //check if we overlap something when we spawn projectile
		if(colliders.Length > 0)
		{
			EnemyOverlap(colliders[0]); // if yes we call this method passing the first collider we collided with
		}
		Destroy(gameObject,3f);
	}

	void GetComponents()
	{
		myCamera = GameObject.FindObjectOfType<Camera>();
		myRigid = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		moveAmountInNextFrame = Time.fixedDeltaTime * speed;
		transform.Translate( Vector3.forward * moveAmountInNextFrame,Space.Self);
		CheckForCollision(moveAmountInNextFrame);
	}

	void CheckForCollision(float moveAmount)
	{
		Ray ray = new Ray(transform.position,transform.forward);
		RaycastHit raycastHit;
		bool collided = Physics.Raycast(ray,out raycastHit,moveAmount);
		if(collided)
		{

			if(areaShot)
			{
				HandleAreaShot(raycastHit);
			}
			else
			{
				EnemyHit(raycastHit);
			}
			Destroy(gameObject);
		}
	}

	float AddDamageVaration()
	{
		float random = Mathf.Round(Random.Range (-damageVaration, damageVaration));
		return random;
	}

	void EnemyHit(RaycastHit hitObject)
	{
		IDamageable damageableObject = hitObject.collider.GetComponent<IDamageable>();

        if(damageableObject != null && hitObject.collider.tag == "Enemy")
		{
			damageableObject.TakeDamage(damage + AddDamageVaration());
			hitMark.gameObject.SetActive (true);
		}
		else
		{
			Vector3 impactPoint = hitObject.point;
			Vector3 faceNormal = hitObject.normal;
			GameObject vfx = Instantiate(hitVFX,impactPoint,Quaternion.Euler(faceNormal)) as GameObject;
			vfx.GetComponent<ParticleSystem>().Play();
			Destroy(vfx, 2f);
		}
	}

	void EnemyHit(Collider collider)
	{
		IDamageable damageableObject = collider.GetComponent<IDamageable>();
		if(damageableObject != null)
		{
			damageableObject.TakeDamage(damage + AddDamageVaration());
			hitMark.gameObject.SetActive (true);
		}
	}

	void HandleAreaShot(RaycastHit hitObject)
	{
		Collider[] colliders = Physics.OverlapSphere(hitObject.point,areaRadius, layerMask );
		if(colliders.Length > 0)
		{
			foreach ( Collider collider in colliders )
			{
				IDamageable damageableObject = collider.GetComponent<IDamageable>();
				Enemy enemy = collider.GetComponent<Enemy>();
				if(damageableObject != null)
				{
					damageableObject.TakeDamage(damage + AddDamageVaration());
					enemy.dizzy = true;
				}
			}
		}

		areaShot = false;
	}

	void HandleAreaShot(Collider hitObject)
	{
		Collider[] colliders = Physics.OverlapSphere(hitObject.transform.position,areaRadius, layerMask );
		if(colliders.Length > 0)
		{
			foreach ( Collider collider in colliders )
			{
				IDamageable damageableObject = collider.GetComponent<IDamageable>();
				Enemy enemy = collider.GetComponent<Enemy>();
				if(damageableObject != null)
				{
					damageableObject.TakeDamage(damage + AddDamageVaration());
					enemy.dizzy = true;
				}
			}
		}

		areaShot = false;
	}

	void EnemyOverlap(Collider col)
	{
		Destroy(gameObject);
		IDamageable damageableObject = col.GetComponent<IDamageable>();
		if(damageableObject != null)
		{
				if(areaShot)
				{
					HandleAreaShot(col);
				}
				else
				{
					EnemyHit(col);
				}
				Destroy(gameObject);
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawRay( new Ray(transform.position,transform.forward));
	}

	public float SetDamage {get {return damage;} set{ damage = value; }}
	public float SetSpeed {get {return speed;} set{ speed = value; }}
}
