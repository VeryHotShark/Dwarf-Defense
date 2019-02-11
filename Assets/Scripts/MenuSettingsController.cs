using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuSettingsController : MonoBehaviour 
{
    public float AnimLength;
	public GameObject spawningEnemies, PlayerUI, dwarf , PauseUI, pauseBG, PauseSettings, Menu, MenuBG;
	public ThirdPersonCamera thirdPersonCam;
    public Animator camAnim;
    
    public TextMeshProUGUI sensitivityText;
	public TextMeshProUGUI sensitivityTextPause;
    public Slider sensitivitySlider;
	public Slider sensitivitySliderPause;

  	public bool gamePaused;
  	bool inMenu;

	void Start ()
	{
		Menu.SetActive(true);	
		MenuBG.SetActive(true);
		camAnim = GetComponent<Animator>();
		sensitivityText.SetText(thirdPersonCam.mouseSensitivity.ToString());
		inMenu = true;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
	public void StartGame () 
	{
		Cursor.visible = false;

        camAnim.enabled = true;
        StartCoroutine (StartGameplay());
	}

    IEnumerator StartGameplay()
    {
    	inMenu = false;
        yield return new WaitForSeconds(AnimLength); //animacja
        //pokaż odliczanie
        //yield return new WaitForSeconds(5.0f);
        PlayerUI.SetActive(true);
        spawningEnemies.GetComponent<EnemySpawner>().enabled = true;
        dwarf.GetComponent<PlayerAbilities>().enabled = true;
        dwarf.GetComponent<PlayerMovement>().enabled = true;
 		thirdPersonCam.enabled = true;
        Destroy(camAnim);
    }

    void Update()
    {
		if((Input.GetKeyDown(KeyCode.P) ||Input.GetKeyDown(KeyCode.Escape)) && !GameManager.instance.isOver && !inMenu)
		{
			if(gamePaused)
				Resume();
			else
				PauseGame();
		}
    }

    public void Resume()
    {
    	if(PauseSettings.activeSelf)
    		PauseSettings.SetActive(false);
    	
		pauseBG.SetActive(false);
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		PauseUI.SetActive(false);
    	gamePaused = false;
    	Time.timeScale = 1f;
    }

    void PauseGame()
    {
    	pauseBG.SetActive(true);
    	Cursor.visible = true;
    	Cursor.lockState = CursorLockMode.None;
    	PauseUI.SetActive(true);
    	gamePaused = true;
    	Time.timeScale = 0f;
    }

	public void SetQuality(int qualityIndex)
	{
		QualitySettings.SetQualityLevel(qualityIndex);
	}
	 
	public void DisplayMouseSensitivity()
	{
		sensitivityText.SetText(thirdPersonCam.mouseSensitivity.ToString());
		sensitivityTextPause.SetText(thirdPersonCam.mouseSensitivity.ToString());
	}

    public void SetMouseSensitivity(float sensitivity)
    {
    	thirdPersonCam.mouseSensitivity = sensitivity;
    	sensitivityText.SetText(sensitivity.ToString());
		sensitivityTextPause.SetText(sensitivity.ToString());
    }

	public void SetMouseSensitivity(string sensitivityString)
    {
    	float sensitivity = float.Parse(sensitivityString);

		if(sensitivity > sensitivitySlider.maxValue)
			sensitivity = sensitivitySlider.maxValue;

    	thirdPersonCam.mouseSensitivity = sensitivity;
    	sensitivitySlider.value = sensitivity;
		sensitivitySliderPause.value = sensitivity;
    }

    public void InvertAxis(bool statement)
    {
    	thirdPersonCam.invertAxis = statement;
    }

	public void QuitGame()
	{
		Debug.Log("Quit Game");
		Application.Quit();
	}
}
