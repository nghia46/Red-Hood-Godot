// Declaring the namespace for the script
namespace Redhood;
// Declaring the partial class PlayerController, which extends CharacterBody2D

public partial class PlayerController : CharacterBody2D
{
	// Declaring and exporting variables for speed and jump velocity
	[Export] public float Speed = 100f;

	[Export] public float JumpVelocity = -250f;

	// Boolean to keep track of the player's facing direction
	bool _isFacingRight = true;
	// Reference to the AnimatedSprite2D node
	AnimatedSprite2D _animatedSprite2D;

	CollisionShape2D _collisionShape2D;

	// Gravity variable obtained from the project settings
	float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	// Called when the node enters the scene tree for the first time
	public override void _Ready()
	{
		// Getting the reference to the AnimatedSprite2D node
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		//get the box collider
		_collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame
	public override void _Process(double delta)
	{
		// Getting the player's velocity
		Vector2 velocity = Velocity;
		// Getting the input direction and handling the movement/deceleration
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		//Velocity = velocity;

		// Handling move action
		GetMoveAction(direction, velocity, IsOnFloor());
		// Adding gravity if the player is not on the floor
		GetGravity(velocity, delta, IsOnFloor());
		// Flipping the player
		FlipPLayer(velocity.X);
		// Handling player animations
		PlayerAnimation(velocity);
		// Moving the player with sliding
		MoveAndSlide();
	}

	private void GetMoveAction(Vector2 direction, Vector2 velocity, bool isOnFloor)
	{
		// Handle jump
		if (Input.IsActionJustPressed("ui_accept") && isOnFloor)
		{
			velocity.Y = JumpVelocity;
		}

		// Handle horizontal movement
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;

		}
		else if (isOnFloor) // Only apply deceleration if the player is on the floor
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		// Update the player's velocity
		Velocity = velocity;
	}

	void GetGravity(Vector2 velocity, double delta, bool isOnFloor)
	{
		if (isOnFloor) 
			return;

		velocity.Y += _gravity * (float)delta;
		Velocity = velocity;
	}

	// Method to handle player animations based on velocity
	void PlayerAnimation(Vector2 velocity)
	{
		if (velocity.Y != 0)
		{
			_animatedSprite2D.Play(velocity.Y < 0 ? "Jumping" : "Falling");
		}
		else
		{
			_animatedSprite2D.Play(Mathf.Abs(velocity.X) > 0 ? "Run" : "Idle");
		}
	}

	// Method to flip the player's direction based on movement
	void FlipPLayer(float dir)
	{
		Vector2 scale = Scale;

		if (dir < 0 && _isFacingRight || dir > 0 && !_isFacingRight)
		{
			scale.X *= -1;
			_isFacingRight = !_isFacingRight;
		}

		Scale = scale;
	}
}
