using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	private Transform _onActiveParent;
	private Transform _weaponContainer;
	private float _time;
	private float _currTime;
	private float _distance;
	private Transform _cachedTransform;

	private Vector3 _startPos;
	private Vector3 _endPos;

	private bool _isActive = false;
	public bool isActive {get {return _isActive;}}
	private Animator _animator;
	public int targetTriggerLayer;

	void Awake () {
		Invoke("DoInit", 0.01f);
	}
	private void DelayedSetInactive () {
		_cachedTransform = transform;
		_onActiveParent = Game.instance.game2DRoot.transform;
		_animator = GetComponent<Animator>();
		gameObject.SetActive(false);
	}

	public void SetValues (Transform wc, float time, float dist) {
		_weaponContainer = wc;
		_time = time;
		_distance = dist;
	}

	public void Show (bool isRight = true) {
		gameObject.SetActive(_isActive = true);
		_cachedTransform.parent = _onActiveParent;
		_startPos = _cachedTransform.position;
		float multiplier = isRight ? 1f: -1f;
		_endPos = _startPos;
		_endPos.x = _endPos.x + (_distance * multiplier);
		_currTime = 0;

		Vector3 tempScale = _cachedTransform.localScale;
		tempScale.x = multiplier;
		_cachedTransform.localScale = tempScale;
	}

	public void Hide () {
		gameObject.SetActive(_isActive = false);
		_cachedTransform.parent = _weaponContainer;
		_cachedTransform.localPosition = Vector3.zero;
		_startPos = default(Vector3);
		_endPos = default(Vector3);
		_currTime = 0;
	}

	// Update is called once per frame
	void Update () {
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

	void OntriggerEnter2D (Collider2D collider) {
		GameObject colGO = collider.gameObject;
		Debug.LogError("lols");
		if(colGO.layer == targetTriggerLayer) {
			_animator.SetBool("", true);
		}
	}

}
