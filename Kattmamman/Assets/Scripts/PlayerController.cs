using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	private Animator _animator;
	private Rigidbody2D _rigidBody2D;
	private LevelController _levelController;

	private bool _grounded;
	private float _groundRadius = 0.2f;

	public Transform GroundCheck;
	public LayerMask WhatIsGround;
	public Text Score;

	void Start()
	{
		_animator = transform.GetComponent<Animator>();
		_rigidBody2D = GetComponent<Rigidbody2D>();
		_levelController = FindObjectOfType<LevelController>();
	}

	void FixedUpdate()
	{
		_grounded = Physics2D.OverlapCircle(GroundCheck.position, _groundRadius, WhatIsGround);
		_animator.SetBool("Grounded", _grounded);
	}

	void Update()
	{
		if (_grounded && Input.GetButtonDown("Jump"))
		{
			GetComponent<AudioSource>().Play();

			_animator.SetBool("Grounded", false);

			if (_levelController.GameSpeed < 4f)
			{
				_rigidBody2D.AddForce(Vector2.up * 800);
			}
			else
			{
				_rigidBody2D.AddForce(Vector2.up * 1000);
			}
		}

		if (_levelController.GameSpeed > 4f &&  _levelController.GameSpeed < 10f)
		{
			_animator.speed += 0.0005f;
		}
		else if(_levelController.GameSpeed > 10f)
		{
			_animator.speed += 0.0001f;
		}
	}
}
