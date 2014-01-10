using UnityEngine;
using System.Collections;

public class ShotgunProjectile : Projectile {

	private Transform[] _leadBalls; // ammos
	private int _leadBallsLength;


	protected override void Init () {
		base.Init ();


		Transform[] tempLeadBalls = GetComponentsInChildren<Transform>(true); 
		_leadBalls = new Transform[tempLeadBalls.Length - 1];
		int j = 0;
		for(int i = 0; i < tempLeadBalls.Length; i++) {
			if(tempLeadBalls[i].name != name) {
				_leadBalls[j] = tempLeadBalls[i];
				j++;
			}
		}
		_leadBallsLength = _leadBalls.Length;
	}

	public override void Show (bool isRight) {
		base.Show(isRight);
		for(int i = 0; i < _leadBallsLength; i++) {
			_leadBalls[i].SetGameObjectActive(true);
		}
		_cachedTransform.localScale = Vector3.one;
		//Invoke("Hide", _time);
	}

	public override void Hide () {
		base.Hide ();
		for(int i = 0; i < _leadBallsLength; i++) {
			_leadBalls[i].localPosition = Vector3.zero;
		}
	}

	public override void OnUpdate () {
		if(_updateEnabled && _isActive) {

			float tDeltaTime = Time.deltaTime;
			_currTime += tDeltaTime;
			if(_currTime < _time) {
				for(int i = 0; i < _leadBallsLength; i++) {
					Transform tlb = _leadBalls[i];
					//Debug.LogError(i + " " + tlb.forward);
					if(tlb.gameObject.activeSelf)
						tlb.position += (_isRight ? 1f: -1f) * tlb.right * (_distance / _time) * tDeltaTime * (((float)i+1)/_leadBallsLength);
				}
			} else {
				Hide();
			}
		}
	}

	public override int GetDamage (Transform trans) { // YOU MUST FIRST DO THIS
		//CancelInvoke("Hide");
		if(trans != null)
			trans.SetGameObjectActive(false);

		//_updateEnabled = false;
		//Hide();
		//int tempDamage = _spriteRenderer.bounds.size.x;
		return _damage;
	}
}
