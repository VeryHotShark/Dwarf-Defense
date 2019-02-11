using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable 
{
	//[Header("SlowDown")]
	float fadeDuration;

	[Header("Texture")]
	public Renderer myRenderer;
	public MeshCollider axeCollider;
	public Texture hitTexture;
	Texture defaultTexture;
	Material myMaterial;

	[Range(0,100)] public int dropPercent;
	public GameObject[] drops; 

	public Transform tower;
	public bool attackEnabled;

	[Space] [Header("Attack Variables")]
	public float chaseRadius;
	public float attackRadius;
	public float timeBetweenAttacks;
	public float damage;

	[Space][Header("Properties")]
	public float startSpeed;
	public float startAccelaration;
	public float startRotationSpeed;
	public float maxHealth;
	public int killScore;
	float health;
	float timer;

	[Space][Header("Enemy UI")]
	public Transform UIsocket;
	public GameObject healthBarGO;
	public Image healthBar;

	public Animator animator;
	float updateSearch = 0.2f;
	Transform targetPos;
	NavMeshAgent myAgent;
	CapsuleCollider collider;
	PlayerMovement player;
	PlayerAbilities playerAbilities;
	GameObject parent;
	bool attacking;
	bool isDizzy;
	bool executing;
	bool isDead;
	bool slowDown;

	IEnumerator attackCoroutine;

	public bool dizzy {get;set;}

	public event System.Action<Enemy> OnEnemyDeath;

	void Start()
	{
		PopUpController.Init(); 

        //Tutaj dodalem pobieranie najblizszej wiezy z managera
      	tower = TowersManager.Instance.GetClosestTower(transform); 

      	myMaterial = myRenderer.material;
      	defaultTexture = myMaterial.mainTexture;

		parent = GameObject.Find("Enemies"); 
		if(!parent)
		{
			parent = new GameObject("Enemies");
			parent.transform.position = Vector3.zero;
		}

		transform.parent = parent.transform;
		//SetHealth(maxHealth);
		axeCollider.enabled = false;

		GetComponents();


		myAgent.speed = startSpeed;
		myAgent.acceleration = startAccelaration;
		myAgent.angularSpeed = startRotationSpeed;
		fadeDuration = playerAbilities.rageFade.fadeDuration;

		StartCoroutine (UpdateTarget ()); //Tutaj się wywołuje Korutyna, która odpowiada za Updatowanie Cel, który przeciwnik ma gonić
	}

	void GetComponents()
	{
		myAgent = GetComponent<NavMeshAgent>();
		player = GameObject.FindObjectOfType<PlayerMovement>();
		playerAbilities = player.GetComponent<PlayerAbilities>();
		collider = GetComponent<CapsuleCollider> ();
	}

	void Update()
	{
		if(playerAbilities.rage && !slowDown)
			StartCoroutine(Slowdown());
		else if(slowDown && !playerAbilities.rage)
			StartCoroutine(UnSlowdown());


		timer += Time.deltaTime; // Tutaj jest zmienna, która jest stoperem tak jakby i ona jest po to żeby przeciwnik atakował co określoną ilość sekund
		if( targetPos != null && player != null && Vector3.Distance(transform.position, targetPos.position) < attackRadius && timer > timeBetweenAttacks) // tutaj chyba rozumiesz
		{
			axeCollider.enabled = true;
			if(!executing)
			{
				attackCoroutine = Attack (targetPos);

				if(attackCoroutine != null)
					StartCoroutine(attackCoroutine); // tutaj wywołuje metodę do atakowania
			}
		}
		
		if(!isDead)
		{
			if(!dizzy && targetPos != null && player != null && Vector3.Distance(transform.position, targetPos.position) > attackRadius ) 
			{
				executing = false;
				axeCollider.enabled = false;
				attacking = false;
				animator.SetBool("Attack", attacking);
			}

			if(dizzy && isDizzy == false)
				StartCoroutine(HandleDizzy());
		}

		if(attacking)
			RotateTowardsTarget();
	}

	public void TakeDamage(float damage) 
	{
		PopUpController.CreateFloatingText(damage.ToString(), UIsocket, player.transform.position, transform.position);

		health -= damage;
		UpdateHealth ();

		StartCoroutine(ChangeTexture());

		if(health<=0)
		{
			isDead = true;
			StartCoroutine(DieAnimation());
			Die();
		}
	}

	void RotateTowardsTarget()
	{
		if(!targetPos)
			return;

		Vector3 dir = (targetPos.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(dir);
		transform.rotation = Quaternion.Slerp(transform.rotation,lookRotation, Time.deltaTime * 10f);
	}

	IEnumerator Slowdown()
	{
		slowDown = true;
		float speed = 1f/ fadeDuration;
		float percent = 0f;

		while(percent < 1f)
		{
			percent += Time.deltaTime * speed;

			animator.speed = Mathf.Lerp(1f,0.5f,percent);
			myAgent.speed = Mathf.Lerp(startSpeed , startSpeed/2f, percent);
			myAgent.acceleration = Mathf.Lerp(startAccelaration , startAccelaration/2f, percent);
			myAgent.angularSpeed = Mathf.Lerp(startRotationSpeed , startRotationSpeed/2f, percent);

			yield return null;
		}
	}

	IEnumerator UnSlowdown()
	{
		float speed = 1f/ fadeDuration;
		float percent = 0f;

		while(percent < 1f)
		{
			percent += Time.deltaTime * speed;

			animator.speed = Mathf.Lerp(0.5f,1f,percent);
			myAgent.speed = Mathf.Lerp(startSpeed/2f ,startSpeed, percent);
			myAgent.acceleration = Mathf.Lerp(startAccelaration /2f, startAccelaration, percent);
			myAgent.angularSpeed = Mathf.Lerp(startRotationSpeed /2f , startRotationSpeed, percent);

			yield return null;
		}

		slowDown = false;
	}

	IEnumerator ChangeTexture()
	{
		myMaterial.mainTexture = hitTexture;
		yield return new WaitForSeconds(0.1f);
		myMaterial.mainTexture = defaultTexture;
	}

	IEnumerator HandleDizzy()
	{
		isDizzy = true;

		if(attackCoroutine != null)
			StopCoroutine(attackCoroutine); // tutaj wywołuje metodę do atakowania

		myAgent.isStopped = true;
		animator.SetBool("Dizzy",true);
		yield return new WaitForSeconds(3f);
		myAgent.isStopped = false;
		isDizzy = false;
		dizzy = false;
		animator.SetBool("Dizzy",false);
	}

	IEnumerator DieAnimation()
	{
		myAgent.isStopped = true;
		animator.SetTrigger("Die");

		yield return new WaitForSeconds(slowDown ? 2f : 1f);
			
		Destroy(gameObject);
	}

	public void Die()
	{
		DropLoot();
		if(OnEnemyDeath != null)
		{
			OnEnemyDeath(this);
		}
	}

	public void DropLoot()
	{
		int random = Random.Range(0,101);

		if(random <= dropPercent)
		{
			int randomDropIndex = Random.Range (0, drops.Length);

			Instantiate(drops[randomDropIndex],new Vector3(transform.position.x, 0.3f, transform.position.z) ,Quaternion.identity);
		}	
	}

	IEnumerator UpdateTarget()
	{
        //chyba wszystkie te rzeczy powinienes miec w petli while i to takiej while(true)
		if(player != null) // jeśli gracz jescze żyje
		{
			targetPos = (Vector3.Distance(transform.position, player.transform.position) <= chaseRadius) ? player.transform : tower; // w żależności czy gracz jest w zasięgu ścigania przez przeciwnika jeśli tak to wtedy celem jest gracz a jeśli nie to celem jest wieża
		}
		else
		{
			targetPos = tower;
		}

		float colliderRadius = collider.radius; // tutaj pobieram promień collidera 
		Vector3 dirToTarget = (targetPos.position - transform.position).normalized; // tu wyliczam Wektor, który wskazuje kierunek do celu
		Vector3 targetPosition = targetPos.position - dirToTarget * colliderRadius ; // a tutaj robię tak że odejmuje od docelowego celu promień collidera przeciwnika żeby np nie wchodził w gracza tylko zatrzymał się przed nim
		while(targetPos != null) // dopóki mamy cel 
		{

			if(player != null) // i gracz wciąż żyje 
			{
				targetPos = (Vector3.Distance(transform.position, player.transform.position) <= chaseRadius) ? player.transform : tower; // to powtarzamy znowu wyszukiwanie kto jest naszym celem, bo to nie sprawdza co klatkę tylko co 1/4 sekundy bodajże jeśli dobrze pamiętam
			}
			else
			{
				targetPos = tower;
			}

			myAgent.SetDestination (targetPos.position); // a tutaj mówimy mu co jest naszym celem i on będzie iść do tego celu
			yield return new WaitForSeconds(updateSearch); // no i tu właśnie jest to odświeżanie
		}
	}

	IEnumerator Attack(Transform target)
	{
		executing = true;
		attacking = true;
		while(attacking && target != null)
		{
					
			timer = 0f;
			myAgent.isStopped = true;
			IDamageable damageable = target.GetComponentInParent<IDamageable> ();
			if (damageable == null)
			{
				Debug.LogError (target.name);
			}
			else
			{
				//damageable.TakeDamage(damage);
			}
			animator.SetBool("Attack", attacking);

			yield return new WaitForSeconds(1.08f);
		}

		myAgent.isStopped = false;
	}

	void UpdateHealth()
	{
		healthBar.fillAmount = health / maxHealth;
	}

	public void SetDamage(float damageToSet)
	{
		damage = damageToSet;
	}

	public void SetHealth(float healthToSet)
	{
		maxHealth = healthToSet;
		health = maxHealth;
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position,chaseRadius);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position,attackRadius);
	}
}
