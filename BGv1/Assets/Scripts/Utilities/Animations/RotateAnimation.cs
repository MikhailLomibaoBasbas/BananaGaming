using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RotateAnimation : CommonAnimation {

	public static RotateAnimation set (GameObject targetGo, Quaternion rotateFr, Quaternion rotateTo, float time, float delay = 0, bool startImmediately = false,
	                                   bool isGlobal = true, cWrapMode wMode = cWrapMode.Once,
	                                   object classPtr = null, string callbckFcn = null, object[] param = null) {
		RotateAnimation tempAnim = null;
		tempAnim = GetComponentCommonAnim<RotateAnimation> (targetGo);
		tempAnim.Initialize ();
		tempAnim.setValues (rotateFr, rotateTo, time, isGlobal, wMode);
		if (classPtr != null && callbckFcn != null)
			tempAnim.addCallback (classPtr, callbckFcn, param);
		tempAnim.startDelayed (startImmediately, delay);
		return tempAnim;
	}

	public static RotateAnimation set (GameObject targetGo, Vector3 eulerAnglesFr, Vector3 EulerAngleTo, float time, float delay = 0, bool startImmediately = false,
	                                   bool isGlobal = true, cWrapMode wMode = cWrapMode.Once,
	                                   object classPtr = null, string callbckFcn = null, object[] param = null) {
		RotateAnimation tempAnim = null;
		tempAnim = GetComponentCommonAnim<RotateAnimation> (targetGo);
		tempAnim.Initialize ();
		tempAnim.setValues (eulerAnglesFr, EulerAngleTo, time, isGlobal, wMode);
		if (classPtr != null && callbckFcn != null)
			tempAnim.addCallback (classPtr, callbckFcn, param);
		tempAnim.startDelayed (startImmediately, delay);
		return tempAnim;
	}

	enum RotateType {
		Euler,
		Quaternion
	}

	Vector3 _rfEulerAngles;
	Quaternion _rotateFrom;
	public Quaternion rotateFrom {
		set {
			_rotateFrom = value;
			_rfEulerAngles = value.eulerAngles;
		}
		get {
			return _rotateFrom;
		}
	}

	Vector3 _rtEulerAngles;
	Quaternion _rotateTo;
	public Quaternion rotateTo {
		set {
			_rotateTo = value;
			_rtEulerAngles = value.eulerAngles;
		}
		get {
			return _rotateTo;
		}
	}
	public bool isGlobal;

	RotateType rotateType = default(RotateType);
	public List<Vector3> eulerAngleList = new List<Vector3> ();
	public List<Quaternion> quaternionList = new List<Quaternion>();
	int listCount = 0;
	int currIndex = 0;
	float timePerPoint = 0;
	
	private bool _isInitialized;


	// Use this for initialization
	void Awake () {
		Initialize ();
	}

	public override void Initialize () {
		base.Initialize ();
		if (!_isInitialized) {
			_isInitialized = true;
		}
	}

	public void setValues(Vector3 rfEuler, Vector3 rtEuler, float time, bool pIsGlobal = true, cWrapMode wMode = cWrapMode.Once) {
		quaternionList = null;
		eulerAngleList = null;
		listCount = 0;
		_rfEulerAngles = rfEuler;
		_rtEulerAngles = rtEuler;
		_rotateFrom = Quaternion.Euler (rfEuler);
		_rotateTo = Quaternion.Euler (rtEuler);
		animationTime = time;
		isGlobal = pIsGlobal;
		wrapMode = wMode;
		rotateType = RotateType.Euler;
	}

	public void setValues (Quaternion rotateFr, Quaternion rotateT, float time, bool pIsGlobal = true, cWrapMode wMode = cWrapMode.Once) {
		quaternionList = null;
		eulerAngleList = null;
		listCount = 0;
		rotateFrom = rotateFr;
		rotateTo = rotateT;
		animationTime = time;
		isGlobal = pIsGlobal;
		wMode = wrapMode;
		rotateType = RotateType.Quaternion;
	}

	public void setValues (float time, bool pIsGlobal, cWrapMode wMode, params Quaternion[] rotations) {
		if (rotations.Length < 2)
			return;
		animationTime = time;
		isGlobal = pIsGlobal;
		wrapMode = wMode;
		quaternionList = new List<Quaternion> (rotations);
		listCount = quaternionList.Count;
		timePerPoint = animationTime = time / (listCount - 1);
		rotateType = RotateType.Quaternion;
	}

	public void setValues (float time, bool pIsGlobal, cWrapMode wMode, params Vector3[] rotationEulers) {
		if (rotationEulers.Length < 2)
			return;
		animationTime = time;
		isGlobal = pIsGlobal;
		wrapMode = wMode;
		eulerAngleList = new List<Vector3> (rotationEulers);
		listCount = eulerAngleList.Count;
		timePerPoint = animationTime = time / (listCount - 1);
		rotateType = RotateType.Euler;

	}

	public override void start (bool flag) {
		if (listCount >= 2 && flag) {
			setRotationListValues (currIndex = 0);
		}
		target.SetRotation ((flag ? rotateFrom : rotateTo), isGlobal);
		timeAnimating = 0;
		isAnimating = flag;
		ratio = 0;
		if (!flag)
			doCallback ();
	}

	public override void startDelayed (bool flag, float delay = 0.00009f) {
		if (delay > 0)
			static_coroutine.getInstance.DoReflection (this, "start", new object[1] { flag }, delay);
		else
			start (flag);
	}

	public void stop() {
		timeAnimating = 0;
		isAnimating = false;
		ratio = 0;
		doCallback ();
	}

	// Update is called once per frame
	void Update () {
		if (isAnimating) {
			timeAnimating += Time.deltaTime;
			ratio = timeAnimating / animationTime;
			if (timeAnimating < animationTime) {
				Quaternion tempRot = Quaternion.Euler (_rfEulerAngles + ((_rtEulerAngles - _rfEulerAngles) * ratio));
				target.SetRotation (tempRot, isGlobal);
			} else {
				if (listCount > 0) {
					if (currIndex < listCount - 2) {
						ratio = 0;
						timeAnimating = 0;
						isAnimating = false;
						setRotationListValues (++currIndex);
						isAnimating = true;
					} else {
						checkAnimationFinishedAction ();
					}
				} else {
					checkAnimationFinishedAction ();
				}
			}
		}
	}

	void setRotationListValues (int index) {
		switch(rotateType) {
			case RotateType.Euler:
				setEulerListValues (index);
				break;
			case RotateType.Quaternion:
				setQuaternionListValues (index);
				break;
			default:
			break;
		}
	}

	void setQuaternionListValues (int index) {
		if (listCount - 2 < index)
			return;
		rotateFrom = quaternionList [index];
		rotateTo = quaternionList [index+1];
		animationTime = timePerPoint;
	}

	void setEulerListValues (int index) {
		if (listCount - 2 < index)
			return;
		_rotateFrom = Quaternion.Euler (_rfEulerAngles = eulerAngleList [index]);
		_rotateTo = Quaternion.Euler (_rtEulerAngles = eulerAngleList [index+1]);
		
		Debug.LogWarning ("setEulerList " + _rfEulerAngles + " - " + _rtEulerAngles);
		Debug.LogWarning(target + " " + animationTime);
		animationTime = timePerPoint;
	}

	void checkAnimationFinishedAction () {
		switch (wrapMode) {
			case cWrapMode.Once:
			break;
			case cWrapMode.PingPong:
				Vector3 tempRfEulerAngles = _rfEulerAngles;
				_rotateFrom = Quaternion.Euler (_rfEulerAngles = _rtEulerAngles);
				_rotateTo = Quaternion.Euler (_rtEulerAngles = tempRfEulerAngles);
			break;
			case cWrapMode.Loop:
			break;
		}
		start (wrapMode != cWrapMode.Once);
	}

	public override void Clean () {
		base.Clean ();
		quaternionList = null;
		eulerAngleList = null;
		_rfEulerAngles = default(Vector3);
		_rtEulerAngles = default(Vector3);
		_rotateFrom = default(Quaternion);
		_rotateTo = default(Quaternion);
		isGlobal = default(bool);
		wrapMode = default(cWrapMode);
	}
}
