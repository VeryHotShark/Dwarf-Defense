using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class HallController : MonoBehaviour, IDamageable
{
	public Transform[] points;

	public Image fillBar;
	public Text healthText;

	public float maxHealth;
	float health;

	public static event System.Action OnHallDestroyed;

	// Use this for initialization
	void Start () {
		health = maxHealth;
        //wieza dodaje sie do managera wiez
		UpdateHealth();
        TowersManager.Instance.AddToTowersManager(this.transform);

		/*  foreach(Transform point in points)
        {
        	TowersManager.Instance.AddToTowersManager(point);
        }*/
	}
	
	public void TakeDamage(float damage)
	{
		health -= damage;
		UpdateHealth ();
		if(health <= 0)
		{
			Die ();
		}
	}

	public void Die()
	{
        //przy zniszczeniu wiezy usuwamy wieze z managera
        TowersManager.Instance.RemoveFromTowersManager(this.transform);

		/*foreach(Transform point in points)
        {
        	TowersManager.Instance.RemoveFromTowersManager(point);
        }*/

        if(OnHallDestroyed != null)
			OnHallDestroyed();

		Destroy (gameObject);
	}

	public void  UpdateHealth()
	{
		fillBar.fillAmount = health / maxHealth;
		healthText.text = health.ToString ();
	}

	void OnTriggerEnter (Collider other)
	{
		if(other.gameObject.CompareTag("Axe"))
		{
			TakeDamage(other.GetComponentInParent<Enemy>().damage);
			UpdateHealth();
		}
	}


}
