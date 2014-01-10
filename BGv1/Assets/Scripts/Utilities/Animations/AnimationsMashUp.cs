using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MoveAnimationV2))]
[RequireComponent(typeof(RotateAnimation))]
[RequireComponent(typeof(ScaleAnimation))]
[RequireComponent(typeof(FadeAnimationV2))]
[RequireComponent(typeof(SplineAnimation))]
[RequireComponent(typeof(BezierCurveAnimation))]
public class AnimationsMashUp : MonoBehaviour {

	void Awake () {
		target = null;
		animationTime = 0.5f;
		wrapMode = cWrapMode.Once;
	}

	private float _animationTime = 2.0f;
	public float animationTime {
		get {
			return _animationTime;
		}
		set {
			_animationTime = value;
			getMoveAnim.animationTime = value;
			getFadeAnim.animationTime = value;
			getScaleAnim.animationTime = value;
			getRotateAnim.animationTime = value;
			getChangeColorAnim.animationTime = value;
			getSplineAnim.animationTime = value;
			getbezierAnim.animationTime = value;
		}
	}

	private cWrapMode _wrapMode = cWrapMode.Once;
	public cWrapMode wrapMode {
		set {
			_wrapMode = value;
			getScaleAnim.wrapMode = value;
			getFadeAnim.wrapMode = value;
			getRotateAnim.wrapMode = value;
			getMoveAnim.wrapMode = value;
			getChangeColorAnim.wrapMode = value;
			getSplineAnim.wrapMode = value;
			getbezierAnim.wrapMode = value;
		}
		get {
			return _wrapMode;
		}
	}
	Transform _target = null;
	public Transform target {
		set {
			getScaleAnim.changeTarget (value);
			getFadeAnim.changeTarget (value);
			getRotateAnim.changeTarget (value);
			getMoveAnim.changeTarget (value);
			getChangeColorAnim.changeTarget (value);
			getSplineAnim.changeTarget (value);
			getbezierAnim.changeTarget (value);
			_target = value;
		}
		get {
			return _target;
		}
	}

	public bool isAnimating {
		get {
			return (getScaleAnim.isAnimating || getFadeAnim.isAnimating || 
			        getMoveAnim.isAnimating || getRotateAnim.isAnimating || getbezierAnim.isAnimating );
		}
	}
	
	public bool isActive {
		get {
			return isChangeColorActive || isScaleActive || isFadeActive || isMoveActive || isRotateActive
				|| isBezierActive || isSplineActive;
		} 
	}

	public void stop (bool willGoToLastPosition = false) {
		if (!willGoToLastPosition) {
			stopScaleAnim ();
			stopFadeAnim ();
			stopMoveAnim();
			stopRotateAnim ();
			stopChangeColorAnim ();
			stopSplineAnim ();
		} else {
			startScalenim (false);
			startFadenim (false);
			startMoveAnim (false);
			startRotatenim (false);
			startChangeColorAnim (false);
			startSplineAnim (false);
			startBezierAnim (false);
		}
	}

	public void Clean () {
		isChangeColorActive = isScaleActive = isFadeActive = isMoveActive = isRotateActive = isBezierActive = false;
		target = null;
		Debug.LogWarning (name);
		//getScaleAnim.Clean ();
		//getFadeAnim.Clean ();
		//getSplineAnim.Clean ();
		//getMoveAnim.Clean ();
		//getChangeColorAnim.Clean ();
		//getbezierAnim.Clean ();
		//getRotateAnim.Clean ();
	}

	public void start(bool cachedAnimations = false, float delay = 0.0f, object classPtr = null, string callbackFcnName = null, object[] parameters = null,
	                  bool saveCallback = false) {

		if (isScaleActive) {
			getScaleAnim.startDelayed(true, delay);
			//isScaleActive = cachedAnimations;
		}

		if (isFadeActive) {
			//Debug.LogError ("sss" + getFadeAnim.target);
			getFadeAnim.startDelayed (true, delay);
			//isFadeActive = cachedAnimations;
		}

		if (isMoveActive) {
			getMoveAnim.startDelayed (true, delay);
			isMoveActive = cachedAnimations;
		}

		if (isRotateActive) {
			getRotateAnim.startDelayed (true, delay);
			//isRotateActive = cachedAnimations;
		}

		if (isChangeColorActive) {
			getChangeColorAnim.startDelayed (true, delay);
			//isChangeColorActive = cachedAnimations;
		}

		if (isSplineActive) {

			getSplineAnim.startDelayed (true, delay);
			//isSplineActive = cachedAnimations;
		}

		if (isBezierActive) {
			//Debug.LogError(gameObject + " " + target);
			getbezierAnim.startDelayed (true, delay);
			//isBezierActive = cachedAnimations;
		}

		if (wrapMode == cWrapMode.Once) {
			if (classPtr != null)
				static_coroutine.getInstance.DoReflection (classPtr, callbackFcnName, parameters, delay + animationTime);
			StartCoroutine (cachedAnimationsActiveCoroutine (cachedAnimations, delay + animationTime));
		}
	}

	private IEnumerator cachedAnimationsActiveCoroutine (bool cachedAnimations, float delayTime) {
		yield return new WaitForSeconds (delayTime);
		//Debug.LogWarning (target.name + " CAched? " + cachedAnimations + " " + this.name);
		if (!cachedAnimations) {
			//Clean ();
			StaticAnimationsManager.getInstance.removeFromStack (this);
		}
	}

	ScaleAnimation scaleAnim = null;
	bool _isScaleActive = false;
	bool isScaleActive {
		set {
			getScaleAnim.enabled = value;
			_isScaleActive = value;
		}
		get {
			return _isScaleActive;
		}
	}
	public ScaleAnimation getScaleAnim {
		get {
			if (scaleAnim == null) {
				scaleAnim = CommonAnimation.GetComponentCommonAnim<ScaleAnimation> (gameObject);
			}
			return scaleAnim;
		}
	}

	FadeAnimationV2 fadeAnim = null;
	bool _isFadeActive = false;
	bool isFadeActive {
		set {
			getFadeAnim.enabled = value;
			_isFadeActive = value;
		}
		get {
			return _isFadeActive;
		}
	}
	public FadeAnimationV2 getFadeAnim {
		get {
			if (fadeAnim == null) {
				fadeAnim = CommonAnimation.GetComponentCommonAnim<FadeAnimationV2> (gameObject);
			}
			return fadeAnim;
		}
	}

	RotateAnimation rotateAnim = null;
	bool _isRotateActive = false;
	bool isRotateActive {
		set {
			getRotateAnim.enabled = value;
			_isRotateActive = value;
		}
		get {
			return _isRotateActive;
		}
	}
	public RotateAnimation getRotateAnim {
		get {
			if (rotateAnim == null) {
				rotateAnim = CommonAnimation.GetComponentCommonAnim<RotateAnimation> (gameObject);
			}
			return rotateAnim;
		}
	}

	MoveAnimationV2 moveAnim = null;
	bool _isMoveActive = false;
	bool isMoveActive {
		set {
			getMoveAnim.enabled = value;
			_isMoveActive = value;
		}
		get {
			return _isMoveActive;
		}
	}
	public MoveAnimationV2 getMoveAnim {
		get {
			if (moveAnim  == null) {
				moveAnim = CommonAnimation.GetComponentCommonAnim<MoveAnimationV2> (gameObject);
			}
			return moveAnim;
		}
	}

	ChangeColorAnimation changeColorAnim = null;
	bool _isChangeColorActive = false;
	bool isChangeColorActive {
		set {
			getChangeColorAnim.enabled = value;
			_isChangeColorActive = value;
		}
		get {
			return _isChangeColorActive;
		}
	}
	public ChangeColorAnimation getChangeColorAnim {
		get {
			if (changeColorAnim == null) {
				changeColorAnim = CommonAnimation.GetComponentCommonAnim<ChangeColorAnimation> (gameObject);
			}
			return changeColorAnim;
		}
	}

	SplineAnimation splineAnim = null;
	bool _isSplineActive = false;
	bool isSplineActive {
		set {
			getbezierAnim.enabled = value;
			_isSplineActive = value;
		}
		get {
			return _isSplineActive;
		}
	}
	public SplineAnimation getSplineAnim {
		get {
			if (splineAnim == null) {
				splineAnim = CommonAnimation.GetComponentCommonAnim<SplineAnimation> (gameObject);
			}
			return splineAnim;
		}
	}

	BezierCurveAnimation bezierAnim = null;
	bool _isBezierActive = false;
	bool isBezierActive {
		set {
			getbezierAnim.enabled = value;
			_isBezierActive = value;
		}
		get {
			return _isBezierActive;
		}
	}
	public BezierCurveAnimation getbezierAnim {
		get {
			if (bezierAnim == null) {
				bezierAnim = CommonAnimation.GetComponentCommonAnim<BezierCurveAnimation> (gameObject);
			}
			return bezierAnim;
		}
	}


	public virtual void setSplineAnim(bool isGlobal, params Vector3[] points) {
		isSplineActive = true;
		getSplineAnim.setValues (animationTime, wrapMode, isGlobal, points);
	}
	public virtual void setSplineAnim(bool isGlobal, List<Vector3> pointList) {
		isSplineActive = true;
		getSplineAnim.setValues (animationTime, wrapMode, isGlobal, pointList);
	}
	public virtual void setMoveAnim(Vector3 mFrom, Vector3 mTo, bool isGlobal) {
		isMoveActive = true;
		getMoveAnim.setValues (mFrom, mTo, animationTime, isGlobal, wrapMode);
	}
	public virtual void setMoveAnim(List<MoveAnimationV2Elements> mav2ElemList, bool isGlobal) {
		isMoveActive = true;
		getMoveAnim.setValues (mav2ElemList, isGlobal, wrapMode);
	}
	public virtual void setScaleAnim(Vector3 sFrom, Vector3 sTo) {
		isScaleActive = true;
		getScaleAnim.setValues (sFrom, sTo, animationTime, wrapMode);
	}
	public virtual void setScaleAnim(List<ScaleAnimationElements> scaleAnimElemsList) {
		isScaleActive = true;
		getScaleAnim.setValues (scaleAnimElemsList, wrapMode);
	}
	public virtual void setRotateAnim(Vector3 rFrom, Vector3 rTo, bool isGlobal) {
		isRotateActive = true;
		getRotateAnim.setValues (rFrom, rTo, animationTime, isGlobal);
	}
	public virtual void setRotateAnim(Quaternion rFrom, Quaternion rTo, bool isGlobal) {
		isRotateActive = true;
		getRotateAnim.setValues (rFrom, rTo, animationTime, isGlobal);
	}
	public virtual void setRotateAnim(bool isGlobal, params Quaternion[] rotations) {
		isRotateActive = true;
		getRotateAnim.setValues (animationTime, isGlobal, wrapMode, rotations);
	}
	public virtual void setRotateAnim(bool isGlobal, params Vector3[] rotationEulers) {
		isRotateActive = true;
		getRotateAnim.setValues (animationTime, isGlobal, wrapMode, rotationEulers);
	}
	public virtual void setFadeAnim(FadeTypeV2 type) {
		isFadeActive = true;
		getFadeAnim.setValues (type, animationTime, wrapMode);
	}
	public virtual void setFadeAnim(List<FadeAnimationV2Elements> fav2eList) {
		isFadeActive = true;
		getFadeAnim.setValues (fav2eList, wrapMode);
	}
	public virtual void setFadeAnim(params float[] ratios) {
		isFadeActive = true;
		getFadeAnim.setValues (animationTime, wrapMode, null, ratios);
	}
	public virtual void setChangeColorAnim(params Color[] colors) {
		isChangeColorActive = true;
		getChangeColorAnim.setValues (animationTime, wrapMode, colors);
	}
	
	public virtual void setChangeColorAnim(Color colorFr, Color colorTo) {
		isChangeColorActive = true;
		getChangeColorAnim.setValues (colorFr, colorTo, animationTime, wrapMode);
	}
	public void setBezierAnim(bool isGlobal, params List<Vector2>[] vectorsList) {
		isBezierActive = true;
		getbezierAnim.setValues (animationTime, wrapMode, isGlobal, vectorsList);
	}
	public void setBezierAnim(bool isGlobal, List<Vector2> vectors) {
		isBezierActive = true;
		getbezierAnim.setValues (vectors, animationTime, isGlobal, wrapMode);
	}
	public void setBezierAnim(bool isGlobal, List<List<Vector2>> vectorsList) {
		isBezierActive = true;
		getbezierAnim.setValues (vectorsList, animationTime, isGlobal, wrapMode);
	}


	#region Animations Start and Stop methods

	void startMoveAnim (bool flag, float delay = 0.0f, bool cachedAnimations = false) {
		if (isMoveActive) {
			getMoveAnim.startDelayed(flag, delay);
		}
	}
	void stopMoveAnim () {
		if (getMoveAnim.isAnimating) {
			getMoveAnim.stop ();
		}
	}
	void startScalenim (bool flag, float delay = 0.0f, bool cachedAnimations = false) {
		if (isScaleActive) {
			getScaleAnim.startDelayed(flag, delay);
		}
	}
	void stopScaleAnim () {
		if (getScaleAnim.isAnimating)
			getScaleAnim.stop ();
	}
	void startFadenim (bool flag, float delay = 0.0f, bool cachedAnimations = false) {
		if (isFadeActive) {
			getFadeAnim.startDelayed(flag, delay);
		}
	}
	void stopFadeAnim () {
		if (getFadeAnim.isAnimating)
			getFadeAnim.stop ();
	}
	void startRotatenim (bool flag, float delay = 0.0f, bool cachedAnimations = false) {
		if (isRotateActive) {
			getRotateAnim.startDelayed(flag, delay);
		}
	}
	void stopRotateAnim () {
		if (getRotateAnim.isAnimating)
			getRotateAnim.stop ();
	}
	void startSplineAnim (bool flag, float delay = 0.0f, bool cachedAnimations = false) {
		if (isSplineActive) {
			getSplineAnim.startDelayed(flag, delay);
		}
	}
	void stopSplineAnim () {
		if (getSplineAnim.isAnimating)
			getSplineAnim.stop ();
	}
	void startChangeColorAnim (bool flag, float delay = 0.0f, bool cachedAnimations = false) {
		if (isChangeColorActive) {
			getChangeColorAnim.startDelayed(flag, delay);
		}
	}
	void stopChangeColorAnim () {
		if (getChangeColorAnim.isAnimating)
			getChangeColorAnim.stop ();
	}
	void startBezierAnim (bool flag, float delay = 0.0f, bool cachedAnimations = false) {
		if (isBezierActive) {
			getbezierAnim.startDelayed(flag, delay);
		}
	}
	void stopBezierAnim () {
		if (getbezierAnim.isAnimating)
			getbezierAnim.stop ();
	}
	#endregion

	#region Clean Specific Animations method
	public void cleanMoveAnim() {getMoveAnim.Clean ();}
	public void cleanRotateAnim() {getRotateAnim.Clean ();}
	public void cleanScaleAnim() {getScaleAnim.Clean ();}
	public void cleanSplineAnim() {getSplineAnim.Clean ();}
	public void cleanBezierAnim() {getbezierAnim.Clean ();}
	public void cleanChangeColorAnim() {getChangeColorAnim.Clean ();}
	public void cleanFadeAnim() {getFadeAnim.Clean ();}
	#endregion
}
