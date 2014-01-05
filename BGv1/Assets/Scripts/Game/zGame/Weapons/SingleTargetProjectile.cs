using UnityEngine;
using System.Collections;

public class SingleTargetProjectile : Projectile {
	private Vector3 _startPos;
	private Vector3 _endPos;
	public Vector2 minMaxYDisplacement;

	protected override void Init () {
		base.Init ();
		//Debug.LogError(_cachedTransform.parent.parent + " " +  _cachedTransform);
		//Debug.LogError(_collider2D);
	}

	public override void Show (bool isRight = true) {
		base.Show(isRight);
		float multiplier = isRight ? 1f: -1f;
		_startPos = _cachedTransform.position;
		_endPos = _startPos;
		_endPos.x += (_distance * multiplier);
		_endPos.y += Random.Range(minMaxYDisplacement.x, minMaxYDisplacement.y);
	}

	public override void Hide () {
		base.Hide();
		_startPos = default(Vector3);
		_endPos = default(Vector3);

	}

	public override void OnUpdate () {
		if(_isActive) {
			if(_currTime < _time) {
				_cachedTransform.position = Vector3.Lerp(_startPos, _endPos, 
			                                         _currTime += (Time.deltaTime / _time));
			} else {
				_cachedTransform.position = _endPos;
				Hide();
			}
		}
	}

}
