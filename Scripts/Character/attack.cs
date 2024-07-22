using Godot;
using System;

public partial class attack : Node
{
	public bool CanAttack = true;
	public float AttackTimer = 0.3f;
	public async void HandleAttack(CharacterBody2D character, float attackCooldown, bool attackPressed)
	{
		if (CanAttack && attackPressed)
		{
			Area2D attackHitbox = character.GetNode<Area2D>("BasicAttackCollider");
			attackHitbox.Monitorable = true;
			attackHitbox.Monitoring = true;
			CanAttack = false;
			await ToSignal(character.GetTree().CreateTimer(AttackTimer), "timeout");
			attackHitbox.Monitorable = false;
			attackHitbox.Monitoring = false;
			await ToSignal(character.GetTree().CreateTimer(attackCooldown), "timeout");
			CanAttack = true;
		}
		
	}
}
