using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class Fade : MonoBehaviour 
{
	public PlayerAbilities playerAbilities;
	public bool vfx;

	public float focal;
	public float startTemperature,desiredTemperature;

	public enum fade {FadeIn, FadeOut, FadeInHitMark, FadeInMenu, FadeInBlur}
	public fade fadeType;

	public PostProcessingBehaviour postProcessingBehaviour = null;
	public Image background;
	public Text[] texts;

	public float fadeDuration;

	void Start()
	{
		var dofSettings = postProcessingBehaviour.profile.depthOfField.settings;
		dofSettings.focalLength = focal;
		postProcessingBehaviour.profile.depthOfField.settings = dofSettings;
		postProcessingBehaviour.profile.depthOfField.enabled = true;

		var cgSettings = postProcessingBehaviour.profile.colorGrading.settings;
		cgSettings.basic.temperature = startTemperature;
		postProcessingBehaviour.profile.colorGrading.settings = cgSettings;

		var caSettings = postProcessingBehaviour.profile.chromaticAberration.settings;
		caSettings.intensity = 0f;
		postProcessingBehaviour.profile.chromaticAberration.settings = caSettings;

		postProcessingBehaviour.profile.motionBlur.enabled = false;
		postProcessingBehaviour.profile.chromaticAberration.enabled = false;
	}

	// Use this for initialization
	void OnEnable () 
	{
		if(vfx)
			return;

		switch(fadeType)
		{
			case fade.FadeInBlur:
				return;
				break;

			case fade.FadeInMenu:
				return;
				break;

			case fade.FadeIn:
				StartCoroutine(FadeIn());
				break;

			case fade.FadeOut:
				StartCoroutine(FadeOut());
				break;

			case fade.FadeInHitMark:
				StartCoroutine(FadeOut());
				break;
		}
	}

	public void OnGameStart()
	{
			StartCoroutine(FadeInMenu());
			StartCoroutine(FadeInBlur());
	}

	public IEnumerator FadeInRage()
	{
		float fadeSpeed = 1f / fadeDuration;
		float percent = 0f;

		var cgSettings = postProcessingBehaviour.profile.colorGrading.settings;
		var caSettings = postProcessingBehaviour.profile.chromaticAberration.settings;

		postProcessingBehaviour.profile.motionBlur.enabled = true;
		postProcessingBehaviour.profile.chromaticAberration.enabled = true;
		
		while(percent <= 1f)
		{
			percent += Time.deltaTime * fadeSpeed;

			cgSettings.basic.temperature = Mathf.Lerp(startTemperature,desiredTemperature,percent);
			postProcessingBehaviour.profile.colorGrading.settings = cgSettings;

			caSettings.intensity = Mathf.Lerp(0f,1f,percent);
			postProcessingBehaviour.profile.chromaticAberration.settings = caSettings;

			yield return null;
		}
	}

	public IEnumerator FadeOutRage()
	{
		float fadeSpeed = 1f / fadeDuration;
		float percent = 0f;

		var cgSettings = postProcessingBehaviour.profile.colorGrading.settings;
		var caSettings = postProcessingBehaviour.profile.chromaticAberration.settings;

		while(percent <= 1f)
		{
			percent += Time.deltaTime * fadeSpeed;

			cgSettings.basic.temperature = Mathf.Lerp(desiredTemperature,startTemperature,percent);
			postProcessingBehaviour.profile.colorGrading.settings = cgSettings;

			caSettings.intensity = Mathf.Lerp(1f,0f,percent);
			postProcessingBehaviour.profile.chromaticAberration.settings = caSettings;

			yield return null;
		}

		postProcessingBehaviour.profile.motionBlur.enabled = false;
		postProcessingBehaviour.profile.chromaticAberration.enabled = false;
	}

	IEnumerator FadeInMenu()
	{
		float fadeSpeed = 1f / fadeDuration;
		float percent = 0f;

		Color startColor = background.color;

		while(percent <= 1f)
		{
			percent += Time.deltaTime * fadeSpeed;
			background.color = Color.Lerp(startColor,Color.clear,percent);
			if(texts != null)
			{
				foreach (Text text in texts)
				{
					text.color = Color.Lerp(Color.white,Color.clear,percent);
				}
			}
			yield return null;
		}

		Destroy(gameObject);
	}

	IEnumerator FadeInBlur()
	{
		float fadeSpeed = 1f / fadeDuration;
		float percent = 0f;

		float startFocalLength = postProcessingBehaviour.profile.depthOfField.settings.focalLength;

		var dofSettings = postProcessingBehaviour.profile.depthOfField.settings;

		while(percent <= 1f)
		{
			percent += Time.deltaTime * fadeSpeed;
			dofSettings.focalLength = Mathf.Lerp(startFocalLength,0f,percent);
			postProcessingBehaviour.profile.depthOfField.settings = dofSettings;
			yield return null;
		}

		postProcessingBehaviour.profile.depthOfField.enabled = false;
	}

	
	IEnumerator FadeIn()
	{
		float fadeSpeed = 1f / fadeDuration;
		float percent = 0f;

		while(percent <= 1f)
		{
			percent += Time.deltaTime * fadeSpeed;
			background.color = Color.Lerp(Color.black,Color.clear,percent);
			if(texts != null)
			{
				foreach (Text text in texts)
				{
					text.color = Color.Lerp(Color.white,Color.clear,percent);
				}
			}
			yield return null;
		}
	}

	IEnumerator FadeOut ( )
	{
		float fadeSpeed = 1f / fadeDuration ;
		float percent = 0f ;

		while ( percent <= 1f )
		{
			percent += Time.deltaTime * fadeSpeed ;
			background.color = Color.Lerp ( Color.clear , Color.black , percent ) ;
			if ( texts != null )
			{
				foreach ( Text text in texts )
				{
					text.color = Color.Lerp ( Color.clear , Color.white , percent ) ;
				}
			}
			yield return null;
		}
	}
}
