using UnityEngine;
using System.Collections;

public enum SlideType {
	Right,
	Left,
	Down
}

public class SlideAnimation : CommonAnimation {

	public SlideType slideType = SlideType.Down;

	private Vector3 _targetOriginalPosition;
	private Vector3 _tempPosition = Vector3.zero;
	private Vector3 _targetInitSlidePos = Vector3.zero;

	private bool _slideIn = true; //False means sliding out 

	public float animDelay = 0.2f;

	public static void setAnimation (GameObject target, SlideType type, float time, bool slideIn, float delay, object classPtr, string callbackFunction) {
		SlideAnimation tempSlideAnim;
		if ((tempSlideAnim = target.GetComponent<SlideAnimation> ()) == null) {
			tempSlideAnim = target.AddComponent<SlideAnimation> ();
		}
		tempSlideAnim.Initialize ();
		tempSlideAnim.setValues (type, time);
		if (classPtr != null && callbackFunction != null) {
			tempSlideAnim.addCallBack (classPtr, callbackFunction, null);
		}
		tempSlideAnim.startAnimation (true, slideIn, delay);
	}

	private void Start() {
		Initialize ();
	}

	public override void Initialize () {
		base.Initialize ();
		if (!isInitialized) {
			_targetOriginalPosition = target.localPosition;
		}
		//Debug.Log (gameObject.name + " " + _targetOriginalPosition);
	}
	
	private void Update () {
		if (isAnimating) {
			timeAnimating += (Time.timeScale != 0)? Time.deltaTime: Time.fixedDeltaTime;
			if (timeAnimating < animationTime) {
				updateSlideAnimation (slideType);
			} else {
				startAnimation (false, _slideIn);
				doCallback ();
			}
		}
	}

	public void setValues(SlideType type, float animTime) {
		slideType = type;
		animationTime = animTime;
		_targetOriginalPosition = target.localPosition;
	}

	public void startAnimation (bool flag, bool slideIn) {
		startAnimation (flag, slideIn, 0f);
	}

	public void startAnimation (bool flag, bool slideIn, float delay) {
		Debug.Log (target.name + "'s slide Animation is " + (flag ? "started.": "finished."));
		if(slideIn)
			_targetInitSlidePos = _tempPosition = _targetOriginalPosition;
		if (flag == true) {
			switch (slideType) {
				case SlideType.Down: 
				{
					_targetInitSlidePos.y = Screen.height;
					break;
				}
				case SlideType.Left:
				{
					_targetInitSlidePos.x  = -Screen.width;
					break;
				}
				case SlideType.Right:
				{
					_targetInitSlidePos.x  = Screen.width;
					break;
				}
				default:
					break;
			}
		}
		if(flag)
			target.localPosition = _targetInitSlidePos;
		timeAnimating = 0f;
		_slideIn = slideIn;

		object[] par = new object[1];
		par[0] = flag;
		static_coroutine.getInstance.DoReflection(this, "setIsAnimating", par, delay);
		//Invoke ("toggleIsAnimating", delay);
	}

	private void toggleIsAnimating () {
		isAnimating = !isAnimating;
		//Debug.Log (gameObject.name + " " + isAnimating);
	}

	public void setIsAnimatingEnabled (bool flag) {
		isAnimating = flag;
	}
	                      

	private void updateSlideAnimation(SlideType type) {
		float r =(_slideIn)?(timeAnimating / animationTime): 1 - (timeAnimating / animationTime);
		r = GetRiseBounceConversion (r);
		switch (type) {
			case SlideType.Down: {
			_tempPosition.y = FloatInterpolate (r,_targetInitSlidePos.y,_targetOriginalPosition.y);
				break;
			}
			case SlideType.Left: {
				_tempPosition.x = FloatInterpolate (r,_targetInitSlidePos.x,_targetOriginalPosition.x);
				break;
			}	
			case SlideType.Right: {
				_tempPosition.x =  FloatInterpolate (r,_targetInitSlidePos.x,_targetOriginalPosition.x);
				break;
			}
			default:
				break;
		}
		target.localPosition = _tempPosition;
	}	

	public  float GetRiseBounceConversion(float ratio) {
		return (2 * (1 - ratio) * ratio * 1.2f + ratio * ratio);
	}

	public float FloatInterpolate(float ratio, float p1, float p2) {
		float toReturn;
		toReturn = p1 + ratio * (p2 - p1);
		return toReturn;
	}
}
