using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	private Transform _target;
	public Transform target {
		set {
			_target = value;
			//if(value != null)
			//	_cachedTransform.parent = value;
			//else
			//	_cachedTransform.parent = Game.instance.game2DRoot.transform;
		}
		get {
			return _target;
		}
	}
	public float offset;
	public float smoothTime;

	private Transform _cachedTransform;
	private Vector3 _targetLastPos;
	private Vector3 _deltaPosition;
	private Vector3 _currentVelocity;

	private bool _isMoving;
	private bool _isIdling;

	private bool _isActive = false;
	public bool isActive {
		get {
			return _isActive;
		}
		set {
			_isActive = value;
			enabled = value;
		}
	}
	
	void Awake () {
		_cachedTransform = transform;
		isActive = false;
		//_targetLastPos = target.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(isActive) {
			_deltaPosition = target.position - _targetLastPos;
			_isMoving = (_deltaPosition != Vector3.zero);
			_isIdling = (_deltaPosition == Vector3.zero);
			UpdateFollowMove();
			UpdateFollowIdle();
			_targetLastPos = target.position;
		}
	}



	private void UpdateFollowMove () {
		if(_isMoving) {
			Vector3 smoothToPos = Vector3.zero;
			if(_deltaPosition.x != 0)
				smoothToPos.x = _deltaPosition.x > 0 ? 1: -1;
			if(_deltaPosition.y != 0)
				smoothToPos.y = _deltaPosition.y > 0 ? /*1*/ 0.5f: /*-1*/ -0.5f;

			_cachedTransform.localPosition = Vector3.SmoothDamp(_cachedTransform.position,  target.position + smoothToPos * offset,
			                                                    ref _currentVelocity, smoothTime);
			if(Mathf.Abs(Vector2.Distance(_cachedTransform.position, target.position + smoothToPos * offset)) < 10f) {
			}
		}
	}

	private void UpdateFollowIdle () {
		if(_isIdling) {
			_cachedTransform.localPosition = Vector3.SmoothDamp(_cachedTransform.position, target.position,
			                                                    ref _currentVelocity, smoothTime * 0.25f);
			if( Mathf.Abs(Vector2.Distance(_cachedTransform.position, target.position)) < 10f) {
				_isIdling = false;
				_cachedTransform.position = target.position;
			}
		}

	}


}
