using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour
{
	private GameObject _tilePosition;
	private GameObject _cloudPosition;
	private GameObject _bgPosition;
	private float _startUpPositionY;
	private const float TileWidth = 1.25f;
	private int _heightLevel;
	private GameObject _tempTile;
	private GameObject _tempBg;
	private GameObject _tempCloud;

	private GameObject _collectedTiles;
	private GameObject _gameLayer;
	private GameObject _backgroundLayer;
	private GameObject _cloudLayer;
	private GameObject _collectedClouds;
	private GameObject _collectedBackgrounds;

	private float _gameSpeed = 2.0f;
	private float _outOfBoundX;

	private int _blankCounter;
	private int _middleCounter;

	private int _blankBgCounter;

	private Tile _lastTile = Tile.Right;
	private Background _lastBg = Background.Default;

	private const string GLeft = "gLeft";
	private const string GMiddle = "gMiddle";
	private const string GRight = "gRight";
	private const string GBlank = "gBlank";

	private const string BgDefault = "bgDefault";
	private const string BgBlank = "bgBlank";

	private const string Cloud1 = "Cloud1";
	private const string Cloud2 = "Cloud2";
	private const string Cloud3 = "Cloud3";

	private float _startTime;
	private int _score;

	private bool _gameOver;

	public Text ScoreText;
	public Text GameOverText;
	public Text RestartText;

	private const string _gameOverText = "Game Over :/";
	private const string _restartText = "Press 'R' to restart";

	public float GameSpeed
	{
		get { return _gameSpeed; }
	}

	public void GameOver()
	{
		_gameOver = true;
		GameOverText.text = _gameOverText;
		RestartText.text = _restartText;
	}

	void Awake()
	{
		Application.targetFrameRate = 60;

		GameOverText.text = "";
		RestartText.text = "";
	}

	void Start()
	{
		_gameLayer = GameObject.Find("GameLayer");
		_backgroundLayer = GameObject.Find("BackgroundLayer");
		_cloudLayer = GameObject.Find("CloudLayer");
		_collectedTiles = GameObject.Find("Tiles");
		_collectedClouds = GameObject.Find("Clouds");
		_collectedBackgrounds = GameObject.Find("Backgrounds");

		LoadTiles();
		LoadClouds();
		LoadBackgrounds();

		_collectedTiles.transform.position = new Vector2(-60.0f, -20.0f);
		_collectedClouds.transform.position = new Vector2(-60.0f, -20.0f);
		_collectedBackgrounds.transform.position = new Vector2(-60.0f, -20.0f);

		_tilePosition = GameObject.Find("StartTilePosition");
		_cloudPosition = GameObject.Find("StartCloudPosition");
		_bgPosition = GameObject.Find("StartBackgroundPosition");

		_startUpPositionY = _tilePosition.transform.position.y;
		_outOfBoundX = _tilePosition.transform.position.x - 5.0f;

		FillScene();
		SetScore(0);

		_startTime = Time.timeSinceLevelLoad;
	}

	private void LoadBackgrounds()
	{
		for (int i = 0; i < 10; i++)
		{
			var tmpB1 = (GameObject)Instantiate(Resources.Load("BgDefault", typeof(GameObject)));
			tmpB1.transform.parent = _collectedBackgrounds.transform.Find(BgDefault).transform;

			var tmpB2 = (GameObject)Instantiate(Resources.Load("Blank", typeof(GameObject)));
			tmpB2.transform.parent = _collectedBackgrounds.transform.Find(BgBlank).transform;
		}
	}

	private void LoadTiles()
	{
		for (int i = 0; i < 21; i++)
		{
			var tmpG1 = (GameObject)Instantiate(Resources.Load("GroundLeft", typeof(GameObject)));
			tmpG1.transform.parent = _collectedTiles.transform.Find(GLeft).transform;

			var tmpG2 = (GameObject)Instantiate(Resources.Load("GroundMiddle", typeof(GameObject)));
			tmpG2.transform.parent = _collectedTiles.transform.Find(GMiddle).transform;

			var tmpG3 = (GameObject)Instantiate(Resources.Load("GroundRight", typeof(GameObject)));
			tmpG3.transform.parent = _collectedTiles.transform.Find(GRight).transform;

			var tmpG4 = (GameObject)Instantiate(Resources.Load("Blank", typeof(GameObject)));
			tmpG4.transform.parent = _collectedTiles.transform.Find(GBlank).transform;
		}
	}

	private void LoadClouds()
	{
		for (int i = 0; i < 20; i++)
		{
			var tmpC1 = (GameObject)Instantiate(Resources.Load("Cloud1", typeof(GameObject)));
			tmpC1.transform.parent = _collectedClouds.transform.Find(Cloud1).transform;

			var tmpC2 = (GameObject)Instantiate(Resources.Load("Cloud2", typeof(GameObject)));
			tmpC2.transform.parent = _collectedClouds.transform.Find(Cloud2).transform;

			var tmpC3 = (GameObject)Instantiate(Resources.Load("Cloud3", typeof(GameObject)));
			tmpC3.transform.parent = _collectedClouds.transform.Find(Cloud3).transform;
		}
	}

	void Update()
	{
		if (_gameOver && Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	void FixedUpdate()
	{
		var fiveSecondsHasPassed = _startTime - Time.time % 5;
		if (fiveSecondsHasPassed == 0)
		{
			_gameSpeed += 0.5f;
		}

		_gameLayer.transform.position = new Vector2(_gameLayer.transform.position.x - _gameSpeed * Time.deltaTime, 0);
		_backgroundLayer.transform.position = new Vector2(_backgroundLayer.transform.position.x - _gameSpeed / 4 * Time.deltaTime, 0);
		_cloudLayer.transform.position = new Vector2(_cloudLayer.transform.position.x - _gameSpeed / 6 * Time.deltaTime, 0);

		HandleTiles();
		HandleClouds();
		HandleBackgrounds();
	}

	private void HandleBackgrounds()
	{
		foreach (var child in _backgroundLayer.transform.Cast<Transform>())
		{
			if (child.position.x >= _outOfBoundX - 5f)
			{
				continue;
			}

			switch (child.gameObject.name)
			{
				case "BgDefault(Clone)":
				{
					child.gameObject.transform.position = _collectedBackgrounds.transform.Find(BgDefault).transform.position;
					child.gameObject.transform.parent = _collectedBackgrounds.transform.Find(BgDefault).transform;
					break;
				}
				case "Blank(Clone)":
				{
					child.gameObject.transform.position = _collectedBackgrounds.transform.Find(BgBlank).transform.position;
					child.gameObject.transform.parent = _collectedBackgrounds.transform.Find(BgBlank).transform;
					break;
				}
				case "StartBackgroundPosition":
				{
					break;
				}
				default:
				{
					Destroy(child.gameObject);
					break;
				}
			}
		}

		if (_backgroundLayer.transform.childCount < 16)
		{
			SpawnBackground();
		}
	}

	private void HandleClouds()
	{
		foreach (var child in _cloudLayer.transform.Cast<Transform>())
		{
			if (child.position.x >= _outOfBoundX)
			{
				continue;
			}

			switch (child.gameObject.name)
			{
				case "Cloud1(Clone)":
					{
						child.gameObject.transform.position = _collectedClouds.transform.Find(Cloud1).transform.position;
						child.gameObject.transform.parent = _collectedClouds.transform.Find(Cloud1).transform;
						break;
					}
				case "Cloud2(Clone)":
					{
						child.gameObject.transform.position = _collectedClouds.transform.Find(Cloud2).transform.position;
						child.gameObject.transform.parent = _collectedClouds.transform.Find(Cloud2).transform;
						break;
					}
				case "Cloud3(Clone)":
					{
						child.gameObject.transform.position = _collectedClouds.transform.Find(Cloud3).transform.position;
						child.gameObject.transform.parent = _collectedClouds.transform.Find(Cloud3).transform;
						break;
					}
				case "StartCloudPosition":
					{
						break;
					}
				default:
					{
						Destroy(child.gameObject);
						break;
					}
			}
		}

		if (_cloudLayer.transform.childCount < 8)
		{
			SpawnCloud();
		}
	}

	private void HandleTiles()
	{
		foreach (var child in _gameLayer.transform.Cast<Transform>())
		{
			if (child.position.x >= _outOfBoundX)
			{
				continue;
			}

			switch (child.gameObject.name)
			{
				case "GroundLeft(Clone)":
					{
						child.gameObject.transform.position = _collectedTiles.transform.Find(GLeft).transform.position;
						child.gameObject.transform.parent = _collectedTiles.transform.Find(GLeft).transform;
						break;
					}
				case "GroundMiddle(Clone)":
					{
						child.gameObject.transform.position =
							_collectedTiles.transform.Find(GMiddle).transform.position;
						child.gameObject.transform.parent = _collectedTiles.transform.Find(GMiddle).transform;
						break;
					}
				case "GroundRight(Clone)":
					{
						child.gameObject.transform.position = _collectedTiles.transform.Find(GRight).transform.position;
						child.gameObject.transform.parent = _collectedTiles.transform.Find(GRight).transform;
						break;
					}
				case "Blank(Clone)":
					{
						child.gameObject.transform.position = _collectedTiles.transform.Find(GBlank).transform.position;
						child.gameObject.transform.parent = _collectedTiles.transform.Find(GBlank).transform;
						break;
					}
				case "StartTilePosition":
					{
						break;
					}
				default:
					{
						Destroy(child.gameObject);
						break;
					}
			}
		}

		if (_gameLayer.transform.childCount < 25)
		{
			SpawnTile();
		}
	}

	private void SpawnTile()
	{
		if (_blankCounter > 0)
		{
			SetTile(Tile.Blank);
			_blankCounter--;
			return;
		}

		if (_middleCounter > 0)
		{
			SetTile(Tile.Middle);
			_middleCounter--;
			return;
		}

		if (_lastTile == Tile.Blank)
		{
			ChangeHeight();
			SetTile(Tile.Left);
			_middleCounter = Random.Range(1, 8);
		}
		else if (_lastTile == Tile.Right)
		{
			UpdateScore();
			if (_gameSpeed > 10f)
			{
				_blankCounter = Random.Range(1, 3);
			}
			else if(_gameSpeed > 6f)
			{
				_blankCounter = Random.Range(1, 4);
			}
			else if (_gameSpeed > 3f)
			{
				_blankCounter = Random.Range(1, 2);
			}
			else
			{
				_blankCounter = 1;
			}
		}
		else if (_lastTile == Tile.Middle)
		{
			SetTile(Tile.Right);
		}
	}

	private void ChangeHeight()
	{
		int newHeightLevel = Random.Range(0, 4);
		if (newHeightLevel < _heightLevel)
		{
			_heightLevel--;
		}
		else if (newHeightLevel > _heightLevel)
		{
			_heightLevel++;
		}
	}

	private void FillScene()
	{
		FillTiles();
		FillClouds();
		FillBackgrounds();
	}

	private void FillBackgrounds()
	{
		SetBackground(Background.Default);
		SetBackground(Background.Default);
		SetBackground(Background.Default);
		SetBackground(Background.Default);
		SetBackground(Background.Default);
		SetBackground(Background.Default);
	}

	private void SetBackground(Background type)
	{
		if (type == Background.Default)
		{
			_tempBg = _collectedBackgrounds.transform.Find(BgDefault).transform.GetChild(0).gameObject;
		}
		else if (type == Background.Blank)
		{
			_tempBg = _collectedBackgrounds.transform.Find(BgBlank).transform.GetChild(0).gameObject;
		}

		_tempBg.transform.parent = _backgroundLayer.transform;
		_tempBg.transform.position = new Vector3(_bgPosition.transform.position.x + Random.Range(8f, 14f),
			_startUpPositionY + Random.Range(1f, 4f), Random.Range(20, 30));

		_bgPosition = _tempBg;
		_lastBg = type;
	}

	private void SpawnBackground()
	{
		if (_blankBgCounter > 0)
		{
			SetBackground(Background.Blank);
			_blankBgCounter--;
			return;
		}

		if (_lastBg == Background.Blank)
		{
			SetBackground(Background.Default);
		}
		else if (_lastBg == Background.Default)
		{
			_blankBgCounter = Random.Range(1, 2);
		}
	}

	private void FillTiles()
	{
		for (var i = 0; i < 15; i++)
		{
			SetTile(Tile.Middle);
		}

		SetTile(Tile.Right);
	}

	private void SetTile(Tile type)
	{
		if (type == Tile.Left)
		{
			_tempTile = _collectedTiles.transform.Find(GLeft).transform.GetChild(0).gameObject;
		}
		else if (type == Tile.Right)
		{
			_tempTile = _collectedTiles.transform.Find(GRight).transform.GetChild(0).gameObject;
		}
		else if (type == Tile.Middle)
		{
			_tempTile = _collectedTiles.transform.Find(GMiddle).transform.GetChild(0).gameObject;
		}
		else if (type == Tile.Blank)
		{
			_tempTile = _collectedTiles.transform.Find(GBlank).transform.GetChild(0).gameObject;
		}

		_tempTile.transform.parent = _gameLayer.transform;
		_tempTile.transform.position = new Vector2(_tilePosition.transform.position.x + TileWidth, _startUpPositionY + _heightLevel * TileWidth);

		_tilePosition = _tempTile;
		_lastTile = type;
	}

	private void SpawnCloud()
	{
		SetCloud((Cloud)Random.Range(1, 4));
	}

	private void FillClouds()
	{
		SetCloud(Cloud.Second);
		SetCloud(Cloud.Third);
	}

	private void SetCloud(Cloud type)
	{
		if (type == Cloud.First)
		{
			_tempCloud = _collectedClouds.transform.Find(Cloud1).transform.GetChild(0).gameObject;
		}
		else if (type == Cloud.Second)
		{
			_tempCloud = _collectedClouds.transform.Find(Cloud2).transform.GetChild(0).gameObject;
		}
		else if (type == Cloud.Third)
		{
			_tempCloud = _collectedClouds.transform.Find(Cloud3).transform.GetChild(0).gameObject;
		}

		_tempCloud.transform.parent = _cloudLayer.transform;
		_tempCloud.transform.position = new Vector2(_cloudPosition.transform.position.x + Random.Range(8f, 12f), _startUpPositionY + Random.Range(8f, 9.5f));

		_cloudPosition = _tempCloud;
	}

	private void UpdateScore()
	{
		if (_gameOver)
		{
			return;
		}

		_score += 1;
		SetScore(_score);
	}

	private void SetScore(int score)
	{
		ScoreText.text = string.Format("Score: {0}", score);
	}
}