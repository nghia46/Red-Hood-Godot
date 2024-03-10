using Godot;
namespace Redhood;
public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 100.0f;
	[Export] public float JumpVelocity = -350.0f;
	[Export] public float CoyoteJumpTime = 0.2f;
	bool canCoyoteJump = false;
	float coyoteJumpTimer = 0.0f;
	bool isFacingRight;
	AnimatedSprite2D animatedSprite2D;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		isFacingRight = true;
	}

	public override void _Process(double delta)
	{
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector2 velocity = Velocity;
		// Add the gravity.
		//note: if the player is on the floor reset the velocity
		velocity.Y += !IsOnFloor() ? gravity * (float)delta : 0;
		//Action
		CheckCoyoteJump(delta);
		PlayerMoveAndJump(direction, velocity);
		PlayerAnimation(velocity);
		FlipPLayer(velocity.X);
		MoveAndSlide();
	}

	private void PlayerAnimation(Vector2 velocity)
	{
		if (velocity.Y != 0)
		{
			animatedSprite2D.Play(velocity.Y < 0 ? "Jumping" : "Falling");
		}
		else
		{
			animatedSprite2D.Play(Mathf.Abs(velocity.X) > 0 ? "Run" : "Idle");
		}
	}

	private void PlayerMoveAndJump(Vector2 direction, Vector2 velocity)
	{

		// Handle Move.
		velocity.X = direction != Vector2.Zero ? direction.X * Speed : Mathf.MoveToward(velocity.X, 0, Speed);
		// Handle regular Jump and Coyote Jump
		if (Input.IsActionJustPressed("ui_accept"))
		{
			if (IsOnFloor() || canCoyoteJump)
			{
				// Perform regular jump if on the floor or allowed coyote jump
				velocity.Y = JumpVelocity;
				canCoyoteJump = false;
			}
		}

		//Set velocity
		Velocity = velocity;
	}
	private void CheckCoyoteJump(double delta)
	{
		// Check for coyote jump
		if (IsOnFloor())
		{
			canCoyoteJump = true;
		}
		else
		{
			coyoteJumpTimer += (float)delta;
			if (coyoteJumpTimer >= CoyoteJumpTime)
			{
				canCoyoteJump = false;
				coyoteJumpTimer = 0.0f;
			}
		}
	}

	private void FlipPLayer(float dir)
	{
		Vector2 scale = Scale;
		if (dir < 0 && isFacingRight || dir > 0 && !isFacingRight)
		{
			scale.X *= -1;
			isFacingRight = !isFacingRight;
		}

		Scale = scale;
	}
}
