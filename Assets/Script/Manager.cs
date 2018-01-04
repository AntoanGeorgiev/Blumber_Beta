using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour {
	private List<Food> food = new List<Food>();
	public GameObject foodPrefab;
	public GameObject pauseMenu;
	private Collider2D[] foodCols;
	private float deltaSpawn=1.0f;
	private float lastSpawn;
	private const float Sliceforce = 300.0f; 
	private int scoreCount;
	private int highScore;
	private Vector3 LastMousePosition;
	public Transform trail;
	private int lifePoint;
	public Text scoreText;
	public Text highscoreText;
	public Image[] points;
	public static Manager Instance;
	private bool isPaused = false;
	public GameObject deathMenu;
	public GameObject Sound;


	private void Awake()
	{
		Instance = this;
	}
		
	private void Start()
	{
		foodCols = new Collider2D[0];
		NewGame ();
	}

	public void NewGame()
	{
		scoreCount = 0;
		lifePoint = 3;
		pauseMenu.SetActive (false);
		scoreText.text = scoreCount.ToString ();
		highScore = PlayerPrefs.GetInt ("Score");
		highscoreText.text = "Best: " + highScore.ToString (); 
		Time.timeScale = 1;
		isPaused = false;
		deathMenu.SetActive (false);
		foreach (Image i in points)
			i.enabled = true;
		foreach (Food f in food) {
			Destroy (f.gameObject);
			food.Clear ();
		}
			
	
	}	


	private void Update()
	{
		if (isPaused)
			return;

		if (Time.time - lastSpawn > deltaSpawn) {
			Food f = getFood ();
			float randomX = Random.Range (-1.65f, 1.65f);

			f.FoodLauncher (Random.Range (1.85f, 2.75f), randomX, -randomX);
			lastSpawn = Time.time;
		}
		if (Input.GetMouseButton (0)) {			
			Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			pos.z = -1;
			trail.position = pos;

			Collider2D[] thisFramesFood = Physics2D.OverlapPointAll (new Vector2 (pos.x, pos.y), LayerMask.GetMask ("Banana"));
			if ((Input.mousePosition - LastMousePosition).sqrMagnitude > Sliceforce) { 
				foreach (Collider2D c2 in thisFramesFood) {
					for (int i = 0; i < foodCols.Length; i++) {
						if (c2 == foodCols [i]) {
							c2.GetComponent<Food> ().Slice ();
							Sound.GetComponent<AudioSource>().Play ();
						}
					}
				}
			}
			LastMousePosition = Input.mousePosition;
			foodCols = thisFramesFood;
		}
	}

	private Food getFood() // Seeking food
	{
		Food f = food.Find (x => !x.Active);
		if (f == null) {
			f = Instantiate (foodPrefab).GetComponent<Food> ();
			food.Add (f);
		}
		return f;
	}

	public void IncrementScore (int scoreAmount)
	{
		scoreCount += scoreAmount;
		scoreText.text = scoreCount.ToString ();

		if (scoreCount > highScore) {
			highScore = scoreCount;
			highscoreText.text = "BEST:" + highScore.ToString (); 
			PlayerPrefs.SetInt ("Score", highScore);
		}
		}

	public void LoseLP()
	{
		if (lifePoint == 0) {
			return;
		}
		lifePoint--;
		points[lifePoint].enabled = false;
		if (lifePoint == 0)
			Death ();
	}

	public void Death()
	{
		isPaused = true;
		deathMenu.SetActive (true);
	}
	 
	public void PauseGame()
	{
		pauseMenu.SetActive (!pauseMenu.activeSelf);
		isPaused = pauseMenu.activeSelf;
		Time.timeScale = (Time.timeScale == 0) ? 1: 0 ;
	}

	public void ToMain()
	{
		SceneManager.LoadScene ("main");
	}



}
