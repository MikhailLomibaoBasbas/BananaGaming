using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ScaleAnimation))]
public class CountAnimation : CommonAnimation {

	public UILabel targetLabel = null;
	Vector3 originalScale = Vector3.zero;

	bool hasTargetLabel = false;
	int fromValInt = 0, toValInt = 0, intDiff = 0;
	float fromValFl = 0f, toValFl = 0f, flDiff = 0;	
	int flDecimalPlaces = 1;
	bool currencyMode = false;
	bool withScaleAnim = false;

	ScaleAnimation mScaleAnim;
	List<ScaleAnimationElements> mScaleAnimElementsList = new List<ScaleAnimationElements>();

	// Use this for initialization
	float getFloatCountValue { get { return fromValFl + (flDiff * ratio); } }
	int getIntCountValue { get { return fromValInt + Mathf.RoundToInt((float)intDiff * ratio); } }
	public override void changeTarget (Transform mtarget)
	{
		if ((targetLabel = mtarget.GetComponent<UILabel> ()) != null) {
			target = mtarget;
			originalScale = target.localScale;
		} else {
			Debug.LogWarning (mtarget + " does not have UILabel component");
		}
	}

	void Awake () {
		Initialize ();
	}

	public override void Initialize () {
		if (!isInitialized) {
			if ((targetLabel = GetComponent<UILabel> ()) == null)
				hasTargetLabel = false;
			mScaleAnim = GetComponent<ScaleAnimation> ();
			mScaleAnim.Initialize ();
			isInitialized = true;
		}
	}
	

	// Update is called once per frame
	void Update () {
		updateCountLabelCached ();
	}
	
	public void setValues (UILabel mtargetLabel, bool mwithScaleAnim = true, bool mcurrencyMode = false, float time = 1.0f) {
		if (mtargetLabel != null) {
			targetLabel = mtargetLabel;
			currencyMode = mcurrencyMode;
			if(withScaleAnim = mwithScaleAnim)
				setScaleAnimElements (targetLabel.cachedTransform, animationTime = time);
			hasTargetLabel = true;
		}
	}

	void setScaleAnimElements (Transform mTarget, float time) {
		float tempRevised = time * 0.4f;
		Vector3 mtargetScale = mTarget.localScale;
		mtargetScale.z = 1;
		mScaleAnimElementsList.Clear ();
		mScaleAnimElementsList.Add(new ScaleAnimationElements(mtargetScale, tempRevised / 2f, 0f));
		mScaleAnimElementsList.Add(new ScaleAnimationElements(mtargetScale * 1.4f, tempRevised / 2f, 1f - tempRevised));
		mScaleAnimElementsList.Add(new ScaleAnimationElements(mtargetScale, tempRevised / 2f, 0f));
		mScaleAnim.changeTarget (mTarget);
		mScaleAnim.setValues (mScaleAnimElementsList);
	}

	public void startCountInt (int fromVal, int toVal) {
		preStartCount<int> (fromVal, toVal);
		fromValFl = toValFl = 0f;
		fromValInt = fromVal;
		toValInt = toVal;
		intDiff = toVal - fromVal;
		targetLabel.text = fromVal.ToString ();
		if(withScaleAnim)
			mScaleAnim.start (true);
	}

	public void startCountFloat (float fromVal, float toVal, int mDecimalPlaces = 1) {
		preStartCount <float> (fromVal, toVal);
		flDecimalPlaces = 1;
		fromValInt = toValInt = 0;
		fromValFl = fromVal;
		toValFl = toVal;
		flDiff = toVal - fromVal;
		targetLabel.text = fromVal.ToString ();
		if(withScaleAnim)
			mScaleAnim.start (true);
	}

	void stopCount (bool willDisplayCountToValue = true) {
		if(willDisplayCountToValue)
			targetLabel.text = ((fromValInt == toValInt) ? toValFl: toValInt).ToString(currencyMode ? "N0":"" );
		isAnimating = false;
		//targetLabel.cachedTransform.localScale = originalScale;
		timeAnimating = 0;
	}

	void preStartCount<T>(object mFrom, object mTo) {
		Type type = typeof(T);
		if (type == typeof(int)) {
			if ((int)mFrom == (int)mTo) {
					Debug.LogWarning ("From and To Value ( " + (int) mTo  + " ) must not be equal ");
				return;
			}
		}
		if (type == typeof(float)) {
			if ((float)mFrom == (float)mTo) {
					Debug.LogWarning ("From and To Value ( " + (float) mTo  + " ) must not be equal ");
				return;
			}
		}
		if (targetLabel == null) {
			Debug.LogWarning ("No Assigned Label to be used");
			return;
		}
		timeAnimating = 0;
		isAnimating = true;
	}

	void updateCountLabelCached() {
		if (isAnimating && hasTargetLabel) {
			//Debug.LogError (fromValInt + " " + toValInt);
			timeAnimating += Time.deltaTime;
			ratio = timeAnimating / animationTime;
			if (timeAnimating <= animationTime) {
				targetLabel.text = ((fromValInt == toValInt) ? getFloatCountValue: getIntCountValue).ToString(currencyMode ? "N0":"" );
			} else {
				stopCount ();
			}
		}
	}
}
