using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour 
{
	public static ScoreKeeper scoreKeeper;
	public TextMeshProUGUI scoreText;
	public static int score { get; set; }
	public int killScore = 50;
	public Text multiplier;
	//public Text endGameScore;
	//public Text endWinGameScore;
	public TextMeshProUGUI endGameScore;
	public TextMeshProUGUI endWinGameScore;

	float lastEnemyKillTime;
	int streakCount;
	int startKillScore;
	float streakExpiryTime = 5f;

	// Use this for initialization
	void Start () 
	{
		HallController.OnHallDestroyed += displayScore;
		PlayerController.OnPlayerDeath +=  displayScore;
		EnemySpawner.OnLastEnemyDeath += displayScore;
		multiplier.gameObject.SetActive(false);
		startKillScore = killScore;

		if(scoreKeeper == null)
			scoreKeeper = this;
		else
			Destroy(scoreKeeper);
	}

	void Update()
	{
		if(Time.time > lastEnemyKillTime + streakExpiryTime)
		{
			multiplier.gameObject.SetActive (false);
		}
	}

	public void OnEnemyKilled(Enemy enemy)
	{
		if(Time.time < lastEnemyKillTime + streakExpiryTime)
		{
			streakCount ++;
			if(streakCount >= 4 && streakCount < 8)
			{
				multiplier.gameObject.SetActive(true);
				killScore = startKillScore * 2;
				multiplier.text = "x2";
			}
			else if(streakCount >= 8 && streakCount < 16)
			{
				killScore = startKillScore * 4;
				multiplier.text = "x4";
			}
			else if(streakCount >= 16)
			{
				killScore = startKillScore * 8;
				multiplier.text = "x8";
			}
				
		}
		else
		{
			multiplier.gameObject.SetActive(false);
			killScore = startKillScore;
			streakCount = 0;
		}

		lastEnemyKillTime = Time.time;
		score += killScore;
		scoreText.SetText(score.ToString("D6"));
	}

	public void displayScore()
	{
		endGameScore.text = "score : " + score.ToString ();
		endWinGameScore.text = "score : " + score.ToString ();
	}

	public void Restart()
	{
		HallController.OnHallDestroyed -= displayScore;
		PlayerController.OnPlayerDeath -=  displayScore;
		EnemySpawner.OnLastEnemyDeath -= displayScore;
		score = 0;
		streakCount = 0;
	}

}
