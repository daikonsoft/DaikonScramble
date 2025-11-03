using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


[RequireComponent(typeof(Controller2D))]


public class Player : MonoBehaviour
{

	[Header("Horizontal Movement")]
	[Tooltip("(float) How fast the player moves horizontally.")]
	public float moveSpeed = 6f;
	private float horizontalInput;
	float velocityXSmoothing;

	[Header("Jumping")]
	[Tooltip("(int) Maximum number of jumps the player can perform.")]
	public int maxJumps = 2;
	private int jumpsRemaining;
	[Tooltip("(float) The maximum height of a jump.")]
	public float maxJumpHeight = 4;
	[Tooltip("(float) The minimum height of a jump.")]
	public float minJumpHeight = 1;
	[Tooltip("(float) How long in seconds it takes to reach the peak of a jump.")]
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	float maxJumpVelocity;
	float minJumpVelocity;


	[Header("Gravity")]
	float gravity;

	Controller2D controller;
	Vector3 velocity;



	void Start()
	{
		controller = GetComponent<Controller2D>();
		gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight); 
		print("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);
		jumpsRemaining = maxJumps;
		print(jumpsRemaining);
	}

	void Update()
	{
		ProcessMovement();
		ProcessGravity();
		ProcessGroundCheck();
	}


	public void Move(InputAction.CallbackContext context)
	{
		horizontalInput = context.ReadValue<Vector2>().x;
	}

	private void ProcessMovement()
	{
		float targetVelocityX = horizontalInput * moveSpeed;
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
		controller.Move(velocity * Time.deltaTime);
	}



	private void ProcessGravity()
	{
		velocity.y += gravity * Time.deltaTime;
		if (controller.collisions.above || controller.collisions.below)
		{
			velocity.y = 0;
		}

	}

	public void Jump(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			if (jumpsRemaining > 0)
			{
				velocity.y = maxJumpVelocity;
				jumpsRemaining--;
			}
		}
		if (context.canceled)
		{
			if (velocity.y > minJumpVelocity)
			{
				velocity.y = minJumpVelocity;
			}
		}

	}

	private void ProcessGroundCheck()
	{
		if (controller.collisions.below)
		{
			jumpsRemaining = maxJumps;
		}

	}

}