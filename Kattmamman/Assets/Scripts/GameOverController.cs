using UnityEngine;

public class GameOverController : MonoBehaviour
{
	private AudioSource _gameOverSound;
	private LevelController _levelController;

	void Start()
	{
		_gameOverSound = GetComponent<AudioSource>();
		_levelController = FindObjectOfType<LevelController>();
	}

	void Update()
	{

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			Destroy(other.gameObject);
			_gameOverSound.Play();
			_levelController.GameOver();
		}
	}
}
