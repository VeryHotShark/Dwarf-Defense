using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class PopUpScript : MonoBehaviour 
{
	public Animator animator;
	Text damageText;

	void Start()
	{
		AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
		Destroy(gameObject,clipInfo[0].clip.length);
	}

	public void SetText(string text,PlayerAbilities playerAbilities)
	{
		damageText = animator.GetComponent<Text>();
		damageText.text = text;

		if(playerAbilities.rage)
			damageText.color = Color.red;
	}
}
