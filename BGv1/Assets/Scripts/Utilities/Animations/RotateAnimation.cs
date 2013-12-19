using UnityEngine;
using System.Collections;

public class RotateAnimation : CommonAnimation {

	public static RotateAnimation set (GameObject targetGo, Quaternion rotateFr, Quaternion rotateTo, float time, float delay = 0, bool startImmediately = false,
	                                   object classPtr = null, string callbckFcn = null, object[] param = null) {
		RotateAnimation tempAnim = null;
		if ((tempAnim = targetGo.GetComponent<RotateAnimation> ()) == null)
			tempAnim = targetGo.AddComponent<RotateAnimation> ();
		tempAnim.Initialize ();
		tempAnim.setValues (rotateFr, rotateTo, time);
		if (classPtr != null && callbckFcn != null)
			tempAnim.addCallBack (classPtr, callbckFcn, param);
		tempAnim.StartAnimation (startImmediately, delay);
		return tempAnim;
	}

	public Quaternion rotateFrom;
	public Quaternion rotateTo;
	
	private bool _isInitialized;


	// Use this for initialization
	void Start () {
		Initialize ();
	}

	public override void Initialize () {
		base.Initialize ();
		if (!_isInitialized) {
			_isInitialized = true;
		}
	}

	public void setValues (Quaternion rotateFr, Quaternion rotateT, float time) {
		rotateFrom = rotateFr;
		rotateTo = rotateT;
		animationTime = time;
	}

	public override void StartAnimation (bool flag, float delay = 0.0f) {
		target.localRotation = flag ? rotateFrom : rotateTo;
		base.StartAnimation (flag, delay);
	}

	void checkStartAnimatingFlag () {
		if (startAnimating) {
			startAnimating = false;
			StartAnimation (true);
		}
	}

	// Update is called once per frame
	void Update () {
		checkStartAnimatingFlag ();
		if (isAnimating) {
			if (!UpdateTimeAnimatingFinished ()) {
				target.localRotation = Quaternion.Lerp (rotateFrom, rotateTo, ratio);
			} else {
				if (isBackAndForth)
					setValues (rotateTo, rotateFrom, animationTime);
				StartAnimation (isRepeating || isBackAndForth);
			}
		}
	}



}
