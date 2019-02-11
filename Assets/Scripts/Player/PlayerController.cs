using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamageable
{
	[Header("Particles")]
	[SerializeField] ParticleSystem manaPickUpVFX;
	[SerializeField] ParticleSystem healthPickUpVFX;

	[Space][Header("General")]
	public float playerMaxHealth, healthPickUp, manaPickUp;
	public float playerHP;

	public float playerMaxMana;
	public float playerMana {get;set;}

	public bool isDead;

	[Space] [Header("UI")]
	public Text healthText;
	public Text manaText;
	public Image healthBar;
	public Image manaBar;

	[Space]
	public Material myMaterial;
	public Texture hitTexture;
	public Texture defaultTexture;

	public static event System.Action OnPlayerDeath;

	void Start () 
	{
		myMaterial.mainTexture = defaultTexture;
		isDead = false;
		playerHP = playerMaxHealth;
		playerMana = playerMaxMana;
		HandleUI ();
	}


	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.CompareTag ("healPickUp"))
		{
			PlayVFX(healthPickUpVFX);
			playerHP += healthPickUp;
			playerHP = Mathf.Clamp(playerHP, 0f, playerMaxHealth);
			HandleUI ();
			Destroy (other.gameObject);
		}
		else if(other.gameObject.CompareTag ("manaPickUp"))
		{
			PlayVFX(manaPickUpVFX);
			playerMana += manaPickUp;
			HandleUI ();
			Destroy (other.gameObject);
		}
		else if(other.gameObject.CompareTag("Axe"))
		{
			TakeDamage(other.GetComponentInParent<Enemy>().damage);
		}
	}

	void PlayVFX(ParticleSystem particleSystem)
	{
		ParticleSystem vfx = Instantiate(particleSystem,transform.position,particleSystem.transform.rotation) as ParticleSystem;
		vfx.transform.parent = transform;
		vfx.Play();
		Destroy(vfx.gameObject,vfx.main.duration + vfx.main.startLifetime.constant);
	}

	public void TakeDamage(float damage)
	{
		StartCoroutine(ChangeTexture());

		playerHP -= damage;
		HandleUI ();
		if(playerHP <=0) 
		{
			Die();
		}
	}

	public void SubstractMana(float amount)
	{
		playerMana -= amount;
		playerMana = Mathf.Clamp(playerMana, 0f, playerMaxMana);
		HandleUI();
	}

	public void Die()
	{
		isDead = true;
		if(OnPlayerDeath != null)
			OnPlayerDeath();
					
		Destroy(gameObject);
	}

	void HandleUI()
	{
		healthText.text = playerHP.ToString ();
		healthBar.fillAmount = playerHP / playerMaxHealth;

		manaText.text = Mathf.Round(playerMana).ToString ();
		manaBar.fillAmount = playerMana / playerMaxMana;
	}

	IEnumerator ChangeTexture()
	{
		myMaterial.mainTexture = hitTexture;
		yield return new WaitForSeconds(0.1f);
		myMaterial.mainTexture = defaultTexture;
	}

}
