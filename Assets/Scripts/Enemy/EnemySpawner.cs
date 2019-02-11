using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour {

	public GameObject countToNextWave;
	public Wave[] Waves;
	
	public Transform[] spawnPositions;
	public Enemy[] enemiesToSpawn;
	float timeBetweenEnemiesSpawns;
	float counter;

	int waveIndex;
	int enemiesLeftToSpawn;
	int enemiesAlive;

	Wave currentWave;
	bool lastWave;
	public static event System.Action OnLastEnemyDeath;

	//UI
	public TextMeshProUGUI waveText;
	public TextMeshProUGUI waveTimeBreak;
	public TextMeshProUGUI waveLasted;
	public TextMeshProUGUI waveLastedWin;

	// Use this for initialization
	void Start ()
	{
		HallController.OnHallDestroyed += displayWave;
		PlayerController.OnPlayerDeath += displayWave;
		countToNextWave.SetActive (false);
		currentWave = Waves[0];
		NextWave();
	}
	
	// Update is called once per frame
	void Update () 
	{
		counter += Time.deltaTime;
		if(counter >=  currentWave.timeBetweenSpawns && enemiesLeftToSpawn > 0 && enemiesAlive != 0 )
		{
			spawnEnemy();
		}
	}

	void NextWave()
	{
		if(waveIndex <= Waves.Length - 1)
		{
			waveIndex ++;
			if(waveIndex == Waves.Length )
			{
				lastWave = true;
				Debug.Log (lastWave);
			}
		}
		else
		{
			return;
		}

		currentWave = Waves[waveIndex -1];
		UpdateWaveCounter();

		enemiesLeftToSpawn = currentWave.enemyCount;
		enemiesAlive = enemiesLeftToSpawn;
		timeBetweenEnemiesSpawns = currentWave.timeBetweenSpawns;

	}

	void UpdateWaveCounter()
	{
		waveText.SetText(waveIndex.ToString());
	}

	void decreaseEnemiesLeftToSpawn(Enemy enemy)
	{
		enemy.OnEnemyDeath -= ScoreKeeper.scoreKeeper.OnEnemyKilled;
		enemy.OnEnemyDeath -= decreaseEnemiesLeftToSpawn;
		enemiesAlive--;
		//Debug.Log ("Enemies Alive: " + enemiesAlive);
		if (enemiesAlive <= 0 && !lastWave)
		{
			StartCoroutine (WaitBeforeNextWave ());
		}
		else if(lastWave && enemiesAlive <= 0)
		{
			if(OnLastEnemyDeath != null)
			{
				GameManager.instance.playerWon = true;
				OnLastEnemyDeath ();
			}
		}
	}

	IEnumerator WaitBeforeNextWave()
	{
		countToNextWave.SetActive (true);
		float waitTime = currentWave.timeToNextWave;
		while(waitTime > 0f)
		{
			waitTime -= Time.deltaTime;
			waveTimeBreak.SetText (Mathf.CeilToInt (waitTime).ToString ());
			yield return null;
		}
		countToNextWave.SetActive (false);
		//yield return new WaitForSeconds(currentWave.timeToNextWave);
		NextWave();
	}

	void spawnEnemy()
	{
		enemiesLeftToSpawn --;
		//Debug.Log("Enemies to spawn: " + enemiesLeftToSpawn);
		counter = 0;

		int randomPos = Random.Range (0,spawnPositions.Length);
		int randomEnemy = Random.Range (0, enemiesToSpawn.Length);

		Vector3 random = new Vector3 (Random.Range (-2f, 2f), 0f, Random.Range (-2f,2f));

		Enemy enemy = Instantiate (enemiesToSpawn [randomEnemy], spawnPositions [randomPos].position + random, Quaternion.identity) as Enemy;

		enemy.SetDamage(currentWave.enemyDamage);
		enemy.SetHealth(currentWave.enemyHealth);

		enemy.OnEnemyDeath += ScoreKeeper.scoreKeeper.OnEnemyKilled;
		enemy.OnEnemyDeath += decreaseEnemiesLeftToSpawn;
	}

	public void displayWave()
	{
		waveLasted.text = "wave : " + waveIndex.ToString ();
		waveLastedWin.text = "wave : " + waveIndex.ToString ();
	}

	public void Restart()
	{
		HallController.OnHallDestroyed -= displayWave;
		PlayerController.OnPlayerDeath -=  displayWave;
		waveIndex = 0;
	}

	[System.Serializable]
	public class Wave
	{
		public int enemyCount;
		public float timeBetweenSpawns;
		public float timeToNextWave;
		public float enemyHealth;
		public float enemyDamage;
	}
}
