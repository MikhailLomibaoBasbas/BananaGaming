using UnityEngine;
using System.Collections;

public class PointAnimation : CommonAnimation {

	public int repetition;
	private int _currReps;

	private Vector3 _targetOriginalPos;
	private Vector3 _targetOffsetPos;
	private Vector3 _targetcurrPos;
	private float mult = 1f; //1 or -1

	private bool p_isInitialized;

	private float _cachedDeltaTime;

	public static PointAnimation setAnimation (GameObject holder, float time, Vector3 offset, int reps) {
		PointAnimation tempPointAnim;
		if((tempPointAnim = holder.GetComponent<PointAnimation>()) == null) {
			tempPointAnim = holder.AddComponent<PointAnimation> ();
		}
		tempPointAnim.Initialize ();
		tempPointAnim.setValues (time, offset, reps);
		tempPointAnim.startAnimation (true);
		return tempPointAnim;
	}

	// Use this for initialization
	void Start () {
		Initialize ();
	}


	public override void Initialize () {
		base.Initialize ();
		if (!p_isInitialized) {
			p_isInitialized = true;
			_targetOriginalPos = target.localPosition;
		}
	}
	
	public void setValues(float time, Vector3 offsetPos, int reps) {
		animationTime = time;
		_targetOffsetPos = offsetPos;
		repetition = reps;
	}

	public void startAnimation (bool flag) {
		startAnimation (flag, 0);
	}

	public void startAnimation (bool flag, int currReps) {
		isAnimating = flag;
		_targetcurrPos = target.localPosition = _targetOriginalPos;
		mult = 1f;
		timeAnimating = 0f;
		_currReps = currReps;
	}

	// Update is called once per frame
	void Update () {
		if (isAnimating) {
			_cachedDeltaTime = Time.deltaTime;
			timeAnimating += _cachedDeltaTime;
			float tempR = mult * _cachedDeltaTime * 2f / animationTime;
			if (timeAnimating < animationTime) {
				Vector3 tempPos = _targetcurrPos;
				tempPos.x += tempR * _targetOffsetPos.x;
				tempPos.y += tempR * _targetOffsetPos.y;
				_targetcurrPos = target.localPosition = tempPos;
				if (timeAnimating > animationTime / 2f && mult > 0) {
					mult = -mult;
					//target.localPosition = _targetOriginalPos + _targetOffsetPos;
				}
			} else {
				if (_currReps < repetition - 1) {
					startAnimation (true, ++_currReps);
				} else {
					startAnimation (false);
					doCallback ();
				}
			}
		}
	}
}
