using UnityEngine;
using System.Collections;

public class EnemyProjectile : Projectile {
	protected override void Init () {
		base.Init ();
	}

	public override void Show (bool isRight) {
		//Debug.LogError("fds");
		SetActiveInScene(true);
		Invoke("Hide", _time);
	}

	public override void Hide () {
		SetActiveInScene(false);
	}
}
