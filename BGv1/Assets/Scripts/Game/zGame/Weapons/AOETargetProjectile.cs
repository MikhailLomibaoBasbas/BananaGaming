using UnityEngine;
using System.Collections;

public class AOETargetProjectile : Projectile {

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

	// Update is called once per frame
	void Update () {
	
	}
}
