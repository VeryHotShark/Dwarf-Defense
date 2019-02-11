using System.Collections;
using UnityEngine;

public class PopUpController : MonoBehaviour 
{

	static PopUpScript popUpText;
	static PlayerAbilities playerAbilities;
	
	// Use this for initialization
	public static void Init () 
	{
		popUpText = Resources.Load<PopUpScript>("Prefabs/PopUpCanvas");
		playerAbilities = GameObject.FindObjectOfType<PlayerAbilities>();
	}
	
	// Update is called once per frame
	public static void CreateFloatingText(string text, Transform pos,Vector3 playerPos, Vector3 enemyPos)
	{
		PopUpScript instance = Instantiate(popUpText);
		Quaternion lookRotation = Quaternion.LookRotation((enemyPos - playerPos).normalized);
		instance.transform.position = pos.position + (Vector3.right * Random.Range(-0.1f,0.1f)) + (Vector3.up * Random.Range(-0.1f,0.1f));
		instance.transform.rotation = lookRotation;
		//instance.transform.LookAt(new Vector3(0f,playerRot.y,0f));
		//instance.transform.rotation = Quaternion.Euler(0f, lookRotation.y, 0f) ;
		instance.SetText(text, playerAbilities);
	}
}
