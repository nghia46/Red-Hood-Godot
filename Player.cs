// Define global using Godot here so using does not need to be defined in any other script
global using Godot;

// Lets use namespace "Redhood" in every script. If your code gets too complex, for example
// maybe you're making a multiplayer game and you want to keep the client code separate
// from the server code. Then you could use the namespace Redhood.Client and Redhood.Server
namespace Redhood;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 100.0f;
	[Export] public float JumpVelocity = -400.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	// Everything is private by default so there's no need to prefix private to everything
	AnimatedSprite2D animatedSprite2D;
	bool isFacingRight;

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		isFacingRight = true;
	}

	public override void _Process(double delta)
	{
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector2 velocity = Velocity;
		// Add the gravity. ]
		//note: if the player is on the floor reset the velocity
		velocity.Y += !IsOnFloor() ? gravity * (float)delta : 0;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
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
		// Handle Jump.
		velocity.Y = Input.IsActionJustPressed("ui_accept") && IsOnFloor() ? JumpVelocity : velocity.Y;
		//Set velocity
		Velocity = velocity;
	}


	private void FlipPLayer(float dir)
	{
		// I prefer to only use 'var' when the type is extremely long. For example
		// "Dictionary<SuperLongList<EntityList<Entity>>>". This is super long so
		// I would use var here. For this just write out Vector2, it's easier to
		// read to the programmer.
		Vector2 scale = Scale;

		if (dir < 0 && isFacingRight || dir > 0 && !isFacingRight)
		{
			scale.X *= -1;
			isFacingRight = !isFacingRight;
		}

		Scale = scale;
	}
}
