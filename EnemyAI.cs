using Godot;
using System;
using System.Reflection;

public partial class EnemyAI : CharacterBody2D
{
	// Singleton instance
	private static EnemyAI instance;
	public int health;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _Ready()
	{
		health = 10;
		// Ensure only one instance exists
		if (instance == null)
		{
			instance = this;
			// Ensure the singleton persists between scenes
		}
	}
	public override void _ExitTree()
	{
		// Reset the instance when exiting the scene tree
		if (instance == this)
		{
			instance = null;
		}
	}

	public static EnemyAI GetInstance()
	{
		// Return the singleton instance
		return instance;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		Velocity = velocity;
		MoveAndSlide();
	}
	public void OnDamage(int damage)
	{
		health -= damage;
		if (health <= 0)
		{
			Die();
		}
	}
	private void Die()
	{
		QueueFree();
	}
}
