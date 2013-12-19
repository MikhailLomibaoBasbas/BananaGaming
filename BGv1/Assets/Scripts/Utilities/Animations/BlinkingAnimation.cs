using UnityEngine;
using System.Collections;

// ON THE PROCESS

public class BlinkingAnimation : CommonAnimation {

	public Color targetColor;
	public float speed = 1f;

	private float _lerpMult;
	private float _lerpVal;
	
	private bool b_IsInitialized = false;

	public static void setAnimation (GameObject target, float speed) {
		BlinkingAnimation tempBlinkAnim;
		if ((tempBlinkAnim = target.AddComponent<BlinkingAnimation> ()) == null) {
			tempBlinkAnim = target.GetComponent<BlinkingAnimation> ();
		}
		tempBlinkAnim.Initialize ();
		tempBlinkAnim.setSpeed (speed);
		tempBlinkAnim.StartAnimation (true);
	}

	// Use this for initialization
	void Start () {
		Initialize ();
	}

	public override void Initialize () {
		base.Initialize ();
		if (!b_IsInitialized) {
			b_IsInitialized = true;
			switch (animComponentType) {
				case AnimationComponentType.UILabel:
					targetColor = targetLabel.color;
					break;
				case AnimationComponentType.UISlicedSprite:
					targetColor = targetSlicedSprite.color;
					break;
				case AnimationComponentType.UISprite:
					targetColor = targetSprite.color;
					break;
				case AnimationComponentType.UIFilledSprite:
					targetColor = targetFilledSprite.color;
				break;
				default:
					break;
			}
		}
	}

	public void setSpeed (float val) {
		speed = val;
	}


	public void StartAnimation (bool flag) {
		_lerpVal = 0f;
		_lerpMult = 1f;
		isAnimating = flag;
		Color tempColor =  (flag)? Color.clear: targetColor;
		switch (animComponentType) {
			case AnimationComponentType.UILabel:
				targetLabel.color = tempColor;
				break;
			case AnimationComponentType.UISlicedSprite:
				targetSlicedSprite.color =  tempColor;
				break;
			case AnimationComponentType.UISprite:
				targetSprite.color = tempColor;
				break;
			case AnimationComponentType.UIPanelAlpha:
			targetPanelAlpha.alpha =  (flag)? 0f: 1f;
				break;
			default:
				break;
		}
	}

	// Update is called once per frame
	void Update () {
		if(isAnimating) {
			if (_lerpVal > 1.5f || _lerpVal < 0f) {
				_lerpMult = -_lerpMult;
				_lerpVal = (_lerpVal < 0f)? 0f: 1f;
			}
			_lerpVal += _lerpMult * Time.deltaTime * speed;
			float tempLerpValue = Mathf.Clamp (_lerpVal, 0f, 1f);
			updateBlinkingAnimation (tempLerpValue);
		}
	}

	private void updateBlinkingAnimation (float lerpval) {
		if(isAnimating) {
			Color tempColor = Color.Lerp(Color.clear, targetColor, lerpval);
			switch (animComponentType) {
				case AnimationComponentType.UILabel:
					targetLabel.color = tempColor;
					break;
				case AnimationComponentType.UISlicedSprite:
					targetSlicedSprite.color = tempColor;
					break;
				case AnimationComponentType.UISprite:
					targetSprite.color = tempColor;
					break;
				case AnimationComponentType.UIPanelAlpha:
					targetPanelAlpha.alpha = Mathf.Lerp(0f, 1f, lerpval);
					break;
				case AnimationComponentType.UIFilledSprite:
					targetFilledSprite.color = tempColor;
					break;
				default:
					break;
			}
		}
	}

}
