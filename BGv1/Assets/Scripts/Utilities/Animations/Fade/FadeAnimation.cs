using UnityEngine;
using System.Collections;
using System.Reflection;

/* This class is NGUI recommended, it requires the one of the following components of the Target: 
 UISlicedSprite, UIPanelAlpha and UILabel */

public class FadeAnimation : CommonAnimation {
	
	public Color targetColor; // For UISlicedSprite / Non-UIPanelAlpha
	
	private float _alphaFrom;
	private float _alphaTo;

	private Color _colorFrom = Color.clear;
	private Color _colorTo = Color.clear;
	
	private float _timeAnimating = 0f;

	private FadeType fadeType = FadeType.FadeIn;

	private bool _isInitialized = false;

	private float delayErr = 0.00f;

	// The purpose bool startimmediately is for animating without the 
	public static FadeAnimation setAnimation (GameObject target, FadeType type, float time, Color fadeTo, 
	                                          bool startImmidiately = false,  object classPtr = null, 
	                                          string callbackFunction = null, object[] parameters = null, bool oneShot = false, float delay = 0) {
		//Debug.LogError (Time.time);
		FadeAnimation tempFadeAnimation;
		if((tempFadeAnimation = target.GetComponent<FadeAnimation>()) == null) {
			tempFadeAnimation = target.AddComponent<FadeAnimation> ();
		}
		tempFadeAnimation.Initialize ();
		tempFadeAnimation.setValues (type, time, fadeTo);
		if (classPtr != null && callbackFunction != null) {
			tempFadeAnimation.addCallBack (classPtr, callbackFunction, parameters, oneShot);
		}

		if(startImmidiately)
			tempFadeAnimation.StartAnimation (true, delay);

		return tempFadeAnimation;
	}

	// Use this for initialization
	private void Start () {
		Initialize ();
	}

	// Update is called once per frame
	private void Update () {
		if (isAnimating) {
			_timeAnimating += (Time.timeScale != 0)? (Time.deltaTime / Time.timeScale): Time.fixedDeltaTime;

			if (_timeAnimating < animationTime + delayErr) {
				updateFadeAnimation ();
			} else {
				//Debug.Log (gameObject.name + "'s Fade Animation is Finished");
				StartAnimation (false, 0);
				doCallback ();
			}
		}
	}

	public override void Initialize () {
		base.Initialize ();
	}

	private void updateFadeAnimation () {
		float tempR = _timeAnimating / (animationTime + delayErr);
		switch (animComponentType) {
			case AnimationComponentType.UIPanelAlpha: { 
				targetPanelAlpha.alpha = Mathf.Lerp (_alphaFrom, _alphaTo, tempR);
				break;
			}
			case AnimationComponentType.UISlicedSprite: {
				targetSlicedSprite.color = Color.Lerp (_colorFrom, _colorTo, tempR);
				break;
			}
			case AnimationComponentType.UILabel: { 
				targetLabel.color = Color.Lerp (_colorFrom, _colorTo, tempR);
				break;
			}
			case AnimationComponentType.UISprite: { 
				targetSprite.color = Color.Lerp (_colorFrom, _colorTo, tempR);
				break;
			}
		}
	}

	public void StartAnimation (bool flag, float delay = 0) {
		//Debug.Log ("FadeAnimation Started" + _alphaTo);
		Debug.Log (gameObject.name + "'s Animation is " + (flag ? "started":"finished"));
		if (delay > 0) {
			object[] par = new object[1];
			par [0] = flag;
			static_coroutine.getInstance.DoReflection (this, "setIsAnimating", par, delay);
		} else {
			isAnimating = flag;
		}
		_timeAnimating = 0f;
		if (AnimationComponentType.UIPanelAlpha == animComponentType) {
			targetPanelAlpha.alpha = (flag) ? _alphaFrom: _alphaTo;
		} else if (AnimationComponentType.UISlicedSprite == animComponentType) {
			targetSlicedSprite.color = (flag) ? _colorFrom: _colorTo;
		} else if (AnimationComponentType.UISprite == animComponentType) {
			targetSprite.color = (flag) ? _colorFrom: _colorTo;
		} else if (AnimationComponentType.UILabel == animComponentType) {
			targetLabel.color = (flag) ? _colorFrom : _colorTo;
		}

	}

	public void hideTarget () {

	}

	public void setIsAnimatingEnabled(bool flag){
		isAnimating = flag;
	}

	public void setValues (FadeType type, float time) {
		setValues (type, time, Color.clear);
	}

	public void setValues (FadeType type, float time, Color colorTo) {
		fadeType = type;
		animationTime = time;
		if(FadeType.FadeToColor == type)
			_colorTo = colorTo;
		//Debug.Log ("ColorTo " + _colorTo);
		setFadeValues ();
	}


	public void setFadeType (FadeType type) {
		fadeType = type;
		setFadeValues ();
	}

	public void setTime (float time) {
		animationTime = time;
		setFadeValues ();
	}
	


 private void setFadeValues () {
		//Debug.LogError (fadeType);
		//Debug.LogError (fadeComponentType);
		switch (fadeType) {
			case FadeType.FadeIn: {
			if(animComponentType != AnimationComponentType.UIPanelAlpha) {
				_colorFrom = Color.clear;
				_colorTo = targetColor;
			}
			switch (animComponentType) {
				case AnimationComponentType.UIPanelAlpha:
					targetPanelAlpha.alpha = _alphaFrom = 0f;
					_alphaTo = 1f;
					break;
				case AnimationComponentType.UISlicedSprite:
					targetSlicedSprite.color = _colorFrom;
					break;
				case AnimationComponentType.UILabel:
					targetLabel.color = _colorFrom;
					break;
				case AnimationComponentType.UISprite:
					targetSprite.color = _colorFrom;
					break;
			}
			break;
		}
			case FadeType.FadeOut: {
				if(animComponentType != AnimationComponentType.UIPanelAlpha) {
					_colorFrom = targetColor;
					_colorTo = Color.clear;
				}	
				switch (animComponentType) {
					case AnimationComponentType.UIPanelAlpha:
						targetPanelAlpha.alpha = _alphaFrom = 1f;
						_alphaTo = 0f;
						break;
					case AnimationComponentType.UISlicedSprite:
						targetSlicedSprite.color = _colorFrom;
						break;
					case AnimationComponentType.UILabel:
						targetLabel.color = _colorFrom;
						break;
					case AnimationComponentType.UISprite:
						targetSprite.color = _colorFrom;
						break;
				}
				break;
			}

		case FadeType.FadeToColor: { // This is like fade out if you make the _colorTo null
			if(AnimationComponentType.UIPanelAlpha != animComponentType)
				_colorFrom = targetColor;
			switch (animComponentType) {
					case AnimationComponentType.UIPanelAlpha:
						Debug.LogWarning (gameObject.name + " has a UIPanelAlpha. Does not have a color Component");
						GameObject.Destroy (this);
						break;
					case AnimationComponentType.UISlicedSprite:
						targetSlicedSprite.color = _colorFrom;
						break;
					case AnimationComponentType.UILabel:
						targetLabel.color = _colorFrom;
						break;
					case AnimationComponentType.UISprite:
						targetSprite.color = _colorFrom;
						break;
			}
				break;
			}
		}
	}

}
