using UnityEngine;
using System.Collections;
using System.Reflection;

public class CommonAnimation : CommonCallBack {
	

	public bool startAnimating;
	public bool isAnimating = false;
	public bool isBackAndForth = false;
	public bool isRepeating = false;
	public Transform target;
	public float speed = 1f;
	protected UISprite targetSprite;
	protected UISlicedSprite targetSlicedSprite;
	protected UILabel targetLabel;
	protected UIPanelAlpha targetPanelAlpha;
	protected UIFilledSprite targetFilledSprite;
	protected Rigidbody targetRigidBody;
	public float animationTime = 0.5f;
	public float timeAnimating = 0f;
	protected float ratio = 0;

	protected bool isInitialized;

	protected float cachedDeltaTime;

	public AnimationComponentType animComponentType;

	public virtual void Initialize () {
		if (!isInitialized) {
			target = transform;
			if ((targetPanelAlpha = transform.GetComponent<UIPanelAlpha> ()) != null) {
				animComponentType = AnimationComponentType.UIPanelAlpha;
			} else if ((targetSlicedSprite = transform.GetComponent<UISlicedSprite> ()) != null) {
				animComponentType = AnimationComponentType.UISlicedSprite;
			} else if ((targetLabel = transform.GetComponent<UILabel> ()) != null) {
				animComponentType = AnimationComponentType.UILabel;
			} else if ((targetSprite = transform.GetComponent<UISprite> ()) != null) { 
				animComponentType = AnimationComponentType.UISprite;
			}  else if ((targetFilledSprite = transform.GetComponent<UIFilledSprite> ()) != null) { 
				animComponentType = AnimationComponentType.UIFilledSprite;
			}else if ((targetRigidBody = transform.GetComponent<Rigidbody> ()) != null) { 
				animComponentType = AnimationComponentType.RigidBody;
			} else {
				Debug.LogWarning ("None of the supported components were found");
				//Destroy (this);
			}
			isInitialized = true;
		}
	}


	public bool UpdateTimeAnimatingFinished (bool timeScaleAffected = true) {
		if (isAnimating) {
			 cachedDeltaTime = Time.deltaTime;
			float tempTimeThisFrame = 0;
			if (!timeScaleAffected) {
				float tempTimeScale = Time.timeScale;
				tempTimeThisFrame = (tempTimeScale != 0) ? cachedDeltaTime / tempTimeScale : Time.fixedDeltaTime;
			} else {
				tempTimeThisFrame = cachedDeltaTime;
			}
			ratio = timeAnimating / animationTime;
			timeAnimating += tempTimeThisFrame;
			return timeAnimating > animationTime;
		}
		return false;
	}

	public virtual void StartAnimation (bool flag, float delay = 0) {
		if (delay > 0) {
			object[] par = new object[1];
			par[0] = flag;
			static_coroutine.getInstance.DoReflection(this, "setIsAnimating", par, delay);
		} else {
			timeAnimating = 0f;
			isAnimating = flag;
			doCallback();
		}
	}

	public virtual void setIsAnimating (bool flag) {
		timeAnimating = 0f;
		isAnimating = flag;
		doCallback();
	}

	protected void SetComponentActive (bool flag) {
		switch (animComponentType) {
			case AnimationComponentType.UISprite:
			targetSprite.enabled = flag;
			break;
			case AnimationComponentType.UIPanelAlpha:
			targetPanelAlpha.alpha = flag ? 1f : 0f;
			break;
			case AnimationComponentType.UILabel:
			targetLabel.enabled = flag;
			break;
			case AnimationComponentType.UISlicedSprite:
			targetSlicedSprite.enabled = flag;
			break;
			case AnimationComponentType.UIFilledSprite:
			targetFilledSprite.enabled = flag;
			break;
			default:
			break;
		}
	}

}
