using UnityEngine;
using System.Collections;

public class ShotgunProjectile : Projectile {
	PolygonCollider2D _polygonCollider2D;

	protected override void Init () {
		base.Init ();
		_polygonCollider2D = _collider2D as PolygonCollider2D;
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
		//Hide();
		//int tempDamage = _spriteRenderer.bounds.size.x;
		return _damage;
	}
}
