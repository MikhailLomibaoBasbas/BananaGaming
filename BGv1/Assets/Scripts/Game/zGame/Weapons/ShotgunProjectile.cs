using UnityEngine;
using System.Collections;

public class ShotgunProjectile : Projectile {

	protected override void Init () {
		base.Init ();
	}

	public override void Show (bool isRight) {
		base.Show(isRight);
		Invoke("Hide", _time);
	}

	public override void Hide () {
		base.Hide ();
	}

	public override void OnUpdate () {
	}

	public override int GetDamage () { // YOU MUST FIRST DO THIS
		CancelInvoke("Hide");
		_updateEnabled = false;
		Invoke("Hide", _time);
		return _damage;
	}
}
