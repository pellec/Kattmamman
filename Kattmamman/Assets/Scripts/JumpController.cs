using UnityEngine;

public class JumpController : MonoBehaviour
{
	private Rigidbody2D _rigidBody2D;
	private const float FallMultiplier = 2.5f;
	private const float LowJumpMultiplier = 2f;

	public void Awake()
	{
		_rigidBody2D = GetComponent<Rigidbody2D>();
	}

	public void Update()
	{
		if (_rigidBody2D.velocity.y < 0)
		{
			_rigidBody2D.velocity += Vector2.up * Physics2D.gravity.y * (FallMultiplier - 1) * Time.deltaTime;
		}
		else if (_rigidBody2D.velocity.y > 0 && !Input.GetButton("Jump"))
		{
			_rigidBody2D.velocity += Vector2.up * Physics2D.gravity.y * (LowJumpMultiplier - 1) * Time.deltaTime;
		}
	}
}
