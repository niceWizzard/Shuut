using System.Threading.Tasks;
using Godot;
using Shuut.Scripts;
using Vector2 = Godot.Vector2;

namespace Shuut.World.Weapons.Pistol;

public partial class Pistol : BaseWeapon
{
	[Export] public new RangedWeaponInfo WeaponInfo;
	[Export] private PackedScene _bullet;
	[Export] private Node2D muzzle;
	private bool _canShoot = true;
	private uint mask;

	public override void _Ready()
	{
		base._Ready();
		Position = Vector2.Right * Handler.WeaponDistanceFromHandler;
	}

	public override void SetAttackMask(uint mask)
	{
		this.mask = mask;
	}

	public override async Task Use()
	{
		if (!_canShoot)
		{
			return;
		}
		_canShoot = false;
		await CurrentAnimation.WaitAsync();
		for (int i = 0; i < WeaponInfo.ShootCount; i++)
		{
			var s = _bullet.Instantiate();
			GetTree().Root.AddChild(s);
			if (s is Shuut.World.Weapons.Pistol.Bullet bullet)
			{
				bullet.Setup
				(
					muzzle.GlobalPosition,
					Vector2.Right.Rotated((GlobalRotation)),
					mask,
					WeaponOwner.BaseDamage + WeaponInfo.BaseDamage,
					WeaponOwner
				);
			}
			await Handler.CreateTimer((int)(WeaponInfo.ShootCountInterval * 1000));
		}

		CurrentAnimation.Release();
		GoCooldown();
	}

	private async void GoCooldown()
	{
		await Handler.CreateTimer((int)(WeaponInfo.ShootCooldown * 1000));
		_canShoot = true;
	}

	public override Task OnCancel() => Task.CompletedTask;

}