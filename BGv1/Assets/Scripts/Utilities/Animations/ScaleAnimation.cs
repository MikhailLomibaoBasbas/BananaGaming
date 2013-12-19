using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScaleAnimation : CommonAnimation {
	public List<ScaleAnimationElements> scaleAnimElemsList = new List<ScaleAnimationElements> ();
	int listCount = 0;
	private int currIndex = 0;

	public Vector3 scaleFrom;
	public Vector3 scaleTo;
	public float delay;
	private Vector3 _origTargetScale;

	private bool _hasOffsetEffect = false; // For a rough Bounce effect

	public static ScaleAnimation set(GameObject targetGO, Vector3 scaleFrom, Vector3 scaleTo, float time, 
	                                          cWrapMode wrapMode = cWrapMode.Once, float delay = 0f, bool startImmidiately = false, 
	                                          object classPtr = null, string callbackFunction = null, object[] parameters = null, bool oneShot = false) {
		ScaleAnimation temp;
		temp = GetComponentCommonAnim<ScaleAnimation>(targetGO);
		temp.setValues (scaleFrom, scaleTo, time, wrapMode);
		if (classPtr != null && callbackFunction != null) {
			temp.addCallback (classPtr, callbackFunction, parameters, oneShot);
		}
		if (startImmidiately) {
			if (delay == 0)
				temp.start (true);
			else
				temp.startDelayed (true, delay);
		}
		return temp;
	}

	/*public static ScaleAnimation set(GameObject targetGO, Vector3 scaleFrom, Vector3 scaleTo, float time, 
	                                 AnimationWrapMode wrapMode = AnimationWrapMode.Once, float delay = 0f, bool startImmidiately = false,
	                                 OnCommonAnimationFinished callback = null){
		ScaleAnimation temp = set (targetGO, scaleFrom, scaleTo, time, wrapMode, delay, startImmidiately, null, null, null, false);
		if(callback != null)
			temp.addCallbackv2 (callback);
		return temp;
	}*/

	public static ScaleAnimation set(GameObject targetGO, List<ScaleAnimationElements> mScaleAnimElemList, cWrapMode wrapMode = cWrapMode.Once, 
	                                 float delay = 0f, bool startimmediately = false, OnCommonAnimationFinished callback = null) {
		ScaleAnimation temp = GetComponentCommonAnim<ScaleAnimation> (targetGO);
		temp.setValues (mScaleAnimElemList, wrapMode, 
		                callback == null? null: callback);
		if (startimmediately) {
			if (delay > 0)
				temp.startDelayed (true, delay);
			else
				temp.start (true);
		}
		return temp;
	}

	// Use this for initialization
	void Awake () {
		Initialize ();
	}

	public override void Initialize () {
		base.Initialize ();
	}

	public void setValues(Vector3 mscaleFr, Vector3 mscaleTo, float time, cWrapMode wMode = cWrapMode.Once, OnCommonAnimationFinished callback = null) {
		scaleAnimElemsList = null;
		listCount = 0;
		currIndex = 0;
		_origTargetScale = scaleFrom = mscaleFr;
		scaleTo = mscaleTo;
		animationTime = time;
		wrapMode = wMode;
		addCallbackv2 (callback);
	}

	public void setValues(List<ScaleAnimationElements> mscaleAnimElementsList, cWrapMode wMode = cWrapMode.Once, OnCommonAnimationFinished callback = null) {
		for (int i = 0; i < mscaleAnimElementsList.Count; i++) {
			//Debug.LogWarning (mscaleAnimElementsList[i].scale);
		}
		if (mscaleAnimElementsList.Count < 2) {
			Debug.LogError (scaleAnimElemsList + " must have atleast 2 points");
			return;
		}
		scaleAnimElemsList = null;
		scaleAnimElemsList = mscaleAnimElementsList;
		listCount = scaleAnimElemsList.Count;
		wrapMode = wMode;
		addCallbackv2 (callback);
	}
	

	public override void start(bool flag){
		if (flag && listCount > 1) {
			setScaleAnimElem (currIndex = 0);
		}
		isAnimating = flag;
		target.localScale = (flag) ? scaleFrom : scaleTo;
		timeAnimating = 0f;

		if (!flag)
			doCallback ();
	}

	public override void startDelayed (bool flag, float delay = 0.00009f) {
		if (delay > 0)
			static_coroutine.getInstance.DoReflection (this, "start", new object[1] { flag }, delay);
		else
			start (flag);
	}

	public void stop () {
		isAnimating = false;
		timeAnimating = 0f;
		ratio = 0;
		doCallback ();
	}

	// Update is called once per frame
	void Update () {
		if (isAnimating) {
			timeAnimating += Time.deltaTime;
			if (timeAnimating < animationTime) {
				ratio = timeAnimating / animationTime;
				if (!_hasOffsetEffect) {
					target.localScale = Vector3.Lerp (scaleFrom, scaleTo, ratio);
				} else {
					target.localScale = GetSmoothOffsetVector (ratio, scaleFrom, scaleTo);
				}
			} else {
				if (listCount > 1) {
					if (currIndex < listCount - 2) {
						timeAnimating = 0f;
						ratio = 0;
						isAnimating = false;
						target.localScale = scaleTo;
						setScaleAnimElem (++currIndex);
						static_coroutine.getInstance.DoReflection(this, "onNextAnimationDelayFinished", null, delay);
					} else {
						checkAnimationFinishedAction ();
					}
				} else {
					checkAnimationFinishedAction ();
				}
			}
		}
	}

	public void onNextAnimationDelayFinished () {
		isAnimating = true;
	}

	void checkAnimationFinishedAction () {
		switch (wrapMode) {
		case cWrapMode.Once:
			start (false);
			break;
		case cWrapMode.PingPong:
			if (listCount < 2) {
				Vector3 tempScaleFrom = scaleFrom;
				scaleFrom = scaleTo;
				scaleTo = tempScaleFrom;
				start (true);
			} else {
				scaleAnimElemsList.Reverse ();
				startDelayed (true, scaleAnimElemsList [0].delay);
			}
			break;
		case cWrapMode.Loop:
			startDelayed (true, scaleAnimElemsList[0].delay);
			break;
		}
	}

	void setScaleAnimElem (int index) {
		if (index > scaleAnimElemsList.Count - 2)
			return;
		animationTime = scaleAnimElemsList [index].time;
		delay = scaleAnimElemsList [index].delay;
		scaleFrom = scaleAnimElemsList [index].scale;
		scaleTo =  scaleAnimElemsList [index + 1].scale;
	}

	public override void Clean () {
		base.Clean ();
		scaleAnimElemsList = null;
		listCount = 0;
		currIndex = 0;
		scaleFrom = default(Vector3);
		scaleTo = default(Vector3);
	}
}


public class ScaleAnimationElements: CommonAnimationElements {
	public ScaleAnimationElements(Vector3 mscale, float mtime, float mdelay) {
		scale = mscale;
		time = mtime;
		delay = mdelay;
	}
	public ScaleAnimationElements(){

	}
	public Vector3 scale;

}
