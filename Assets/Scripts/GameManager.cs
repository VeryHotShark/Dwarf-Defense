using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
	public float speedMultiplier;
	public GameObject GameOverUI, GameWinUI;
	public GameObject PlayerUI;
	public static GameManager instance;
	public bool isOver = false, playerWon;

	// Use this for initialization
	void Awake () 
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(instance);
		}

		//DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		PlayerController.OnPlayerDeath += HandleUI;
		HallController.OnHallDestroyed += HandleUI;
		EnemySpawner.OnLastEnemyDeath += HandleUI;
	}

	void Update()
	{
		RenderSettings.skybox.SetFloat("_Rotation", Time.time * speedMultiplier);
		RestartLevel();
	}

	void RestartLevel()
	{
		if(Input.GetKeyDown(KeyCode.R) && isOver)
		{
			HandleResetting();
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}

	public void Restart()
	{
		HandleResetting();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	void HandleResetting()
	{
		isOver = false;
		playerWon = false;
		GameOverUI.SetActive(false);
		GameWinUI.SetActive (false);	
		PlayerUI.SetActive(true);
		ScoreKeeper.scoreKeeper.Restart();
		PlayerController.OnPlayerDeath -= HandleUI;
		HallController.OnHallDestroyed -= HandleUI;
		EnemySpawner.OnLastEnemyDeath -= HandleUI;
	}

	void HandleUI()
	{
		isOver = true;
		if(playerWon)
		{
			GameWinUI.SetActive (true);	
		}
		else
		{
			GameOverUI.SetActive(true);
		}
		PlayerUI.SetActive(false);
	}
}
