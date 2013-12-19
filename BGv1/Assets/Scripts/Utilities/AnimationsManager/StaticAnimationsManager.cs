using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class StaticAnimationsManager {

	private static StaticAnimationsManager instance = null;
	public static StaticAnimationsManager getInstance {
		get {
			if (instance == null)
				instance = new StaticAnimationsManager ();
			return instance;
		}
	}

	const string ANIM_MASHUP_NAME = "AnimationMashUpNode";
	public GameObject container = null;
	public bool isUpdateRunning = false;
	private Dictionary<string, AnimationsMashUp> _activeAnimMashUpMap = new Dictionary<string, AnimationsMashUp> ();
	private List<AnimationsMashUp> _animMashUpList = new List<AnimationsMashUp>();
	private int mashUpCount = 20;
	private float cachedDeltaTime = 0f;
	private StaticAnimationsManager () {
		container = new GameObject ("z_animMashUp_manager");
		container.AddComponent<AnimationsManagerUpdater> ();
		for (int i = 0; i < mashUpCount; i++) {
			GameObject tempGO = StaticManager_Helper.CreateChild (container, ANIM_MASHUP_NAME + (i + 1));
			_animMashUpList.Add(tempGO.AddComponent<AnimationsMashUp>());
		}
	}
	public AnimationsMashUp getAvailableAnimMashUp {
		get {
			AnimationsMashUp tempMashup = null;
			for (int i = 0; i < mashUpCount; i++) {
				tempMashup = _animMashUpList [i];
				if (!tempMashup.isAnimating && !_activeAnimMashUpMap.ContainsKey(tempMashup.name)) {
					tempMashup.Clean ();
					_activeAnimMashUpMap.Add (tempMashup.name, tempMashup);
					break;
				}
				tempMashup = null;
			}
			if (tempMashup == null) {
				GameObject tempGO = StaticManager_Helper.CreateChild (container, ANIM_MASHUP_NAME + (_animMashUpList.Count + 1));
				_animMashUpList.Add(tempMashup = tempGO.AddComponent<AnimationsMashUp>());
			}
			return tempMashup;//_availableAnimMashUp;
		}
	}
	//PARANG RELEASE
	const string REMOVE_ITEM_STR = "removeFromStack"; 
	public void removeFromStack (AnimationsMashUp mashup, bool willStopAnimation = false) { // Being called after temporary animMashup is played.
		string name = mashup.name;
		if (_activeAnimMashUpMap.ContainsKey (name)) {
			if (willStopAnimation)
				mashup.stop ();
			_activeAnimMashUpMap.Remove (name);
		}
	}
	const string REMOVE_ITEM_STR_BY_NAME = "removeByNameFromStack";
	public void removeByNameFromStack (string name, bool willStopAnimation = false) { // Being called after temporary animMashup is played.
		if (_activeAnimMashUpMap.ContainsKey (name)) {
			if (willStopAnimation)
				_activeAnimMashUpMap [name].stop ();
			_activeAnimMashUpMap.Remove (name);
		}
	}
	public void resetStack() {
		_activeAnimMashUpMap.Clear ();
	}
	public void stopAnimation(AnimationsMashUp mashup, bool goToFinal = false) {
		mashup.stop (goToFinal);
	}
	public void stopAnimationByName(string name, bool goToFinal = false) {
		if (_activeAnimMashUpMap.ContainsKey (name)) {
			_activeAnimMashUpMap [name].stop (goToFinal);
		}
	}


	#region General Methods
	object classPointer = null;
	string callbackFunction = null;
	object[] parameters = null;

	void setCallback( object classPtr = null, string callbackFcn = null, object[] param = null) {
		if (classPtr == null)
			return;
		classPointer = classPtr;
		callbackFunction = callbackFcn;
		parameters = param;
	}

	public void StartAnimation(AnimationsMashUp animMashup, float delay = 0.0f, 
	                    object classPtr = null, string callbackFcn = null, object[] param = null) {
		animMashup.start (true, delay, classPtr, callbackFcn, param);
	}

	public void StartAnimationByName(string animMashupName, float delay = 0.0f, 
	                                 object classPtr = null, string callbackFcn = null, object[] param = null) {
		if(_activeAnimMashUpMap.ContainsKey(animMashupName)) {
			_activeAnimMashUpMap[animMashupName].start(true, delay, classPtr, callbackFcn, param);
		}
	}
	#endregion

	#region Generic Animations

	List<ScaleAnimationElements> bsiAnimList = new List<ScaleAnimationElements> (){
		new ScaleAnimationElements(Vector3.one * 0.0f, time * 0.5f, 0.00f),
		new ScaleAnimationElements(Vector3.one * 1.4f, time * 0.25f, 0.00f),
		new ScaleAnimationElements(Vector3.one * 0.8f, time * 0.13f, 0.00f),
		new ScaleAnimationElements(Vector3.one * 1.2f, time * 0.12f, 0.00f),
		new ScaleAnimationElements(Vector3.one * 1.0f, time * 0.5f, 0.00f)
	};
	const float time = 0.5f;
	public AnimationsMashUp setBouncyScaleInAnimation(bool cached, Transform target,
	                                                  object classPtr = null, string callbackFcn = null, object[] param = null) {
		Vector3 targetScale = target.localScale;
		List<ScaleAnimationElements> tempAnimList = new List<ScaleAnimationElements> (bsiAnimList);
		int count = tempAnimList.Count;
		for (int i = 0; i < count; i++) {
			Vector3 tempScale =	tempAnimList [i].scale;
			tempScale.x *= targetScale.x;
			tempScale.y *= targetScale.y;
			tempScale.z *= targetScale.z;
			tempAnimList [i].scale = tempScale;
		}
		AnimationsMashUp tempMashup = getAvailableAnimMashUp;
		tempMashup.target = target;
		tempMashup.animationTime = time;
		tempMashup.setScaleAnim (tempAnimList);
		if (!cached) {
			tempMashup.start (false, 0.0f, classPtr, callbackFcn, param);
			static_coroutine.getInstance.DoReflection (this, REMOVE_ITEM_STR, new object[2] { tempMashup, false }, 0.1f);
		} else
			return tempMashup;
		return null;
	}

	public AnimationsMashUp setBouncyScaleOutAnimation(bool cached, Transform target,
	                                                   object classPtr = null, string callbackFcn = null, object[] param = null) {
		Vector3 targetScale = target.localScale;
		List<ScaleAnimationElements> tempAnimList = new List<ScaleAnimationElements> (bsiAnimList);
		tempAnimList.Reverse ();
		int count = tempAnimList.Count;
		for (int i = 0; i < count; i++) {
			Vector3 tempScale =	tempAnimList [i].scale;
			tempScale.x *= targetScale.x;
			tempScale.y *= targetScale.y;
			tempScale.z *= targetScale.z;
			tempAnimList [i].scale = tempScale;
		}
		AnimationsMashUp tempMashup = getAvailableAnimMashUp;
		tempMashup.target = target;
		tempMashup.setScaleAnim (tempAnimList);
		if (!cached) {
			tempMashup.start (false, 0.0f, classPtr, callbackFcn, param);
			static_coroutine.getInstance.DoReflection (this, REMOVE_ITEM_STR, new object[2] { tempMashup, false }, 0.1f);
		} else
			return tempMashup;
		return null;
	}

	public string setMoveAnimation(bool cached, Transform target, Vector3 destination, float time, bool isGlobal = true, 
	                              object classPtr = null, string callbackFcn = null, object[] param = null) {
		AnimationsMashUp tempMashup = getAvailableAnimMashUp;
		string tempMashupName = null;
		tempMashup.target = target;
		tempMashup.animationTime = time;
		tempMashup.wrapMode = cWrapMode.Once;
		Vector3 fromPos = isGlobal ? target.position : target.localPosition;
		tempMashup.setMoveAnim (fromPos, destination, isGlobal);
		if (!cached) {
			tempMashup.start (false, 0.0f, classPtr, callbackFcn, param);
			static_coroutine.getInstance.DoReflection (this, REMOVE_ITEM_STR, new object[2] { tempMashup, false }, 0.1f);
		} else {
			tempMashupName = tempMashup.name;
			//_activeAnimMashUpMap.Add (tempMashupName = tempMashup.name, tempMashup);

		}
		return tempMashupName;
	}

	public AnimationsMashUp setRandomSpline(bool cached, Transform target, Vector3 moveFrom, Vector3 moveTo, float time, int randPtsCount = 1, bool isGlobal = true,
	                            object classPtr = null, string callbackFcn = null, object[] param = null) {
		//Debug.LogError (target.GetPosition (isGlobal) + " " + destination);
		AnimationsMashUp tempMashup = getAvailableAnimMashUp;
		tempMashup.target = target;
		tempMashup.animationTime = time;
		tempMashup.wrapMode = cWrapMode.Once;
		List<Vector3> splinePointsList = new List<Vector3> ();
		splinePointsList.Add (moveFrom);
		for (int i = 0; i < randPtsCount; i++)
			splinePointsList.Add (StaticManager_Helper.GetRandomInBetweenPoint (moveFrom, moveTo, (moveTo - moveFrom) * 0.2f));
		splinePointsList.Add (moveTo);
		tempMashup.setSplineAnim (isGlobal, splinePointsList);

		if (!cached) {
			tempMashup.start (false, 0.0f, classPtr, callbackFcn, param);
			static_coroutine.getInstance.DoReflection (this, REMOVE_ITEM_STR, new object[2] { tempMashup, false }, 0.1f);
		}
		return tempMashup;
	}

	// Teleport Anim
	public void setTeleportAnimation (Transform target, Vector3 destination, float time, bool isGlobal = true, object classPtr = null, string callbackFcn = null, object[] param = null) {
		AnimationsMashUp tempMashup = getAvailableAnimMashUp;
		tempMashup.target = target;
		tempMashup.animationTime = time;
		tempMashup.setFadeAnim (1.0f, 0.0f, 1.0f);
		tempMashup.start (false, 0.0f, classPtr, callbackFcn, param);
		static_coroutine.getInstance.DoReflection (this, REMOVE_ITEM_STR, new object[2] { tempMashup, false }, 0.1f);
		static_coroutine.getInstance.DoReflection (this, "onTeleportAnimation2", new object[3] { target, destination, isGlobal }, time / 2f);
	}

	public void onTeleportAnimation2 (Transform target, Vector3 destination, bool isGlobal) {
		target.SetPosition (destination, isGlobal);
	}

	//Smash Anim
	Transform tsTarget;
	object tsClassPointer = null;
	string tsCallbackFunction = null;
	object[] tsParameters = null;
	public string setSmashAnim (bool cached, Transform target, float time, object classPtr = null, string callbackFcn = null, object[] param = null) {
		Vector3 originalScale = target.localScale;
		List<ScaleAnimationElements> scaleAnimElemList = new List<ScaleAnimationElements> () {
			new ScaleAnimationElements (originalScale * 5f, time  *  0.60f, 0.0f),
			new ScaleAnimationElements (originalScale * 0.7f, time *  0.30f,time * 0.1f),
			new ScaleAnimationElements (originalScale, time  *  0.60f, 0.0f)
		};
		AnimationsMashUp tempMashup = getAvailableAnimMashUp;
		string tempMashupName = null;
		tempMashup.wrapMode = cWrapMode.Once;
		tempMashup.target = target;
		tempMashup.animationTime = time;
		tempMashup.setScaleAnim(scaleAnimElemList);
		if (!cached) {
			tempMashup.start (false, 0.0f, classPtr, callbackFcn, param);
			static_coroutine.getInstance.DoReflection (this, REMOVE_ITEM_STR, new object[2] { tempMashup, false },  0.1f);
		} else {
			tempMashupName = tempMashup.name;
			//tsTarget = target;
			//tsAnimMashUp = tempMashup;
			//tsClassPointer = classPtr;
			//tsCallbackFunction = callbackFcn;
			//tsParameters = param;
		}
		return tempMashupName;
	}
	//end Smash animation

	//Blinking animation - Requires material that has color component.
	AnimationsMashUp baAnimMashup = null;
	object baClassPointer = null;
	string baCallbackFunction = null;
	object[] baParameters = null;
	public string setBlinkingAnimation(bool cached, Transform targetTrans, float time, cWrapMode wMode,
	                                 object classPtr = null, string callbackFcn = null, object[] param = null) {
		AnimationsMashUp tempMashup = getAvailableAnimMashUp;
		string tempMashupName = null;
		tempMashup.target = targetTrans;
		tempMashup.wrapMode = wMode;
		tempMashup.animationTime = time / 2f;
		List<FadeAnimationV2Elements> baFaeList = new List<FadeAnimationV2Elements> () {
			new FadeAnimationV2Elements(FadeTypeV2.FadeOut, 1.0f, time * 0.45f, 0),
			new FadeAnimationV2Elements(FadeTypeV2.FadeOut, 0.0f, time * 0.45f, time * 0.1f),
			new FadeAnimationV2Elements(FadeTypeV2.FadeOut, 1.0f, time * 0.45f, 0)
		};
		tempMashup.setFadeAnim (baFaeList);
		if (!cached) {
			tempMashup.start (false, 0.0f, classPtr, callbackFcn, param);
			static_coroutine.getInstance.DoReflection (this, REMOVE_ITEM_STR, new object[2] { tempMashup, false }, 0.1f);
		} else {
			tempMashupName = tempMashup.name;
			//baAnimMashup = tempMashup;
			//baClassPointer = classPtr;
			//baCallbackFunction = callbackFcn;
			//baParameters = param;
			//baOriginalRatio = baAnimMashup.getFadeAnim.getAlpha;
		}
		return tempMashupName;
	}
	//end Blinking animation

	//Spinning Headline Animation
	AnimationsMashUp shaMashup = null;
	Transform shaTarget = null;
	object shaClassPointer = null;
	string shaCallbackFunction = null;
	object[] shaParameters = null;
	public void setSpinningHeadlineAnimation(bool cached, Transform target, float time,
	                                         object classPtr = null, string callbackFcn = null, object[] param = null) {
		AnimationsMashUp tempMashup = getAvailableAnimMashUp;
		tempMashup.target = target;
		tempMashup.wrapMode = cWrapMode.Once;
		tempMashup.animationTime = time;
		tempMashup.setScaleAnim (Vector3.zero, target.localScale);
		tempMashup.setRotateAnim (Vector3.zero, Vector3.back * 1440f * time, false);
		if (!cached) {
			//Debug.LogError (tempMashup == null);
			tempMashup.start (false, 0.0f, classPtr, callbackFcn, param);
			static_coroutine.getInstance.DoReflection (this, REMOVE_ITEM_STR, new object[2] { tempMashup, false },  0.1f);
		} else {
			shaMashup = tempMashup;
			shaTarget = target;
			shaClassPointer = classPtr;
			shaCallbackFunction = callbackFcn;
			shaParameters = param;
		}
	}
	public void startSpinningHeadlineAnimation() {
		if (shaMashup != null) {
			shaMashup.start (true);
		}
	}
	public void onSpinningHeadlineAnimationFinished() {
		if (shaClassPointer != null)
			static_coroutine.getInstance.DoReflection (shaClassPointer, shaCallbackFunction, shaParameters, 0.0f);

	}
	//End Spinning Headline Animation

	//Consecutive Smash Animation
	AnimationsMashUp csaAnimMashup = null;
	List<Transform> csaTransformList = new List<Transform> ();
	float csaTime;
	object csaClassPointer = null;
	string csaCallbackFunction = null;
	object[] csaParameters = null;
	public void setConsecutiveSmashAnimation(bool cached, List<Transform> transformList, float time,
	                                         object classPtr = null, string callbackFcn = null, object[] param = null) {
		float itemTime = time / transformList.Count;
		for (int i = 0; i < transformList.Count; i++) {
			AnimationsMashUp tempAnimMashUp = getAvailableAnimMashUp;
			Transform target = transformList [i];
			tempAnimMashUp.target = transformList [i];
			tempAnimMashUp.wrapMode = cWrapMode.Once;
			tempAnimMashUp.animationTime = itemTime;
			tempAnimMashUp.setScaleAnim (target.localScale * 12f, target.localScale);
			tempAnimMashUp.start (true, (itemTime * i) * 0.91f);

		}
		if (!cached) {
			startConsecutiveSmashAnimation (transformList, time, classPtr, callbackFcn, param);
		} else {
			csaTransformList = transformList;
			csaTime = time;
			shaClassPointer = classPtr;
			shaCallbackFunction = callbackFcn;
			shaParameters = param;
		}
	}
	public void startCachedConsecutiveSmashAnimation() {
		if(csaTransformList != null || csaTransformList.Count > 0) {
			startConsecutiveSmashAnimation (csaTransformList, csaTime, csaClassPointer, csaCallbackFunction, csaParameters);
        }
	}
	void startConsecutiveSmashAnimation (List<Transform> transformList, float time,
	                                           object classPtr = null, string callbackFcn = null, object[] param = null) {
		float itemTime = time / transformList.Count;
		for (int i = 0; i < transformList.Count; i++) {
			AnimationsMashUp tempAnimMashUp = getAvailableAnimMashUp;
			Transform target = transformList [i];
			tempAnimMashUp.target = transformList [i];
			tempAnimMashUp.wrapMode = cWrapMode.Once;
			tempAnimMashUp.animationTime = itemTime;
			tempAnimMashUp.setScaleAnim (target.localScale * 12f, target.localScale);
			tempAnimMashUp.start (true, (itemTime * i) * 0.91f);
			static_coroutine.getInstance.DoReflection (this, REMOVE_ITEM_STR, new object[2] { tempAnimMashUp, false },  0.1f);
		}
		if (classPtr != null)
			static_coroutine.getInstance.DoReflection (classPtr, callbackFcn, param, time);
	}
	//End Consecutive Smash Animation

	#endregion

	#region Specific Animations

	//Coin Increase
	Transform pmaTarget = null;
	Vector3 pmaOriginalTargetPos = Vector3.zero;
	Transform pmaRotateTarget = null;
	const string PMA_ROTATE_TARGET = "PMARotateTarget";
	AnimationsMashUp pmaAnimMashUp = null;
	AnimationsMashUp pmaRotateMashUpAnim = null;
	ParticleEmitter pmaTargetEmitter = null;
	UILabel pmaLabel = null;
	bool pmaIsGlobal = true;
	int pmaCountFrom = 0;
	int pmaCountTo = 0;
	object pmaClassPointer = null;
	string pmaCallbackFunction = null;
	object[] pmaParameters = null;
	public void setCoinIncreaseAnimation (Transform targetTrans, UILabel labelToAnimate,
	                                         bool isGlobal = true, float time = 1.5f,
	                                      object classPtr = null, string callbackFcn = null, object[] param = null) {
		pmaTarget = targetTrans;
		pmaIsGlobal = isGlobal;
		pmaOriginalTargetPos = pmaTarget.position;
		pmaRotateTarget = pmaTarget.FindChild (PMA_ROTATE_TARGET);
		pmaTargetEmitter = pmaTarget.GetComponentInChildren<ParticleEmitter> ();
		pmaLabel = labelToAnimate;
		pmaAnimMashUp = getAvailableAnimMashUp;
		pmaAnimMashUp.target = pmaTarget;
		pmaAnimMashUp.wrapMode = cWrapMode.Once;
		pmaAnimMashUp.animationTime = time;
		pmaAnimMashUp.setMoveAnim (targetTrans.GetPosition(isGlobal), labelToAnimate.cachedTransform.GetPosition(isGlobal), isGlobal); // Move First

		pmaRotateMashUpAnim = getAvailableAnimMashUp;
		pmaRotateMashUpAnim.target = pmaRotateTarget;
		pmaRotateMashUpAnim.setRotateAnim (Vector3.zero, Vector3.forward * 720f, false);
		pmaRotateMashUpAnim.animationTime = 0.8f;

		pmaClassPointer = classPtr;
		pmaCallbackFunction = callbackFcn;
		pmaParameters = param;
	}
	public void startCoinIncreaseAnimation (Vector3 mFrom, Vector3 mTo, int countFrom, int countTo) {
		if (pmaAnimMashUp != null) {
			pmaCountFrom = countFrom;
			pmaCountTo = countTo;
			pmaTarget.position = pmaOriginalTargetPos;
			pmaTargetEmitter.emit = true;
			pmaAnimMashUp.setMoveAnim (mFrom, mTo, pmaIsGlobal);
			pmaAnimMashUp.start (true, 0.0f, this, "onCoinIncreaseAnimation2");
		} else
			Debug.LogError ("Animation is not yet set");
	}
	public void onCoinIncreaseAnimation2 () {
		pmaRotateMashUpAnim.start (true, 0.0f, this, "onPMARotateAnimFinished", null);
		setCountLabelAnimation (true, pmaLabel, pmaCountFrom, pmaCountTo, 1.5f, pmaClassPointer, pmaCallbackFunction, pmaParameters);
		startCountLabelAnimMashUp ();
	}
	public void onPMARotateAnimFinished() {
		if (pmaTargetEmitter != null)
			pmaTargetEmitter.emit = false;
	}
	public void resetCoinIncrease () {
		pmaTarget = null;
		pmaRotateTarget = null;
		pmaAnimMashUp = null;
		pmaTargetEmitter = null;
		pmaLabel = null;
		pmaCountFrom = 0;
		pmaCountTo = 0;
		pmaClassPointer = null;
		pmaCallbackFunction = null;
		pmaParameters = null;
	}


	float wltime = 0f;
	float wlcountTime = 0;
	float wltimeAnimating = 0f;
	float wlFromToDiff = 0;
	UILabel wlLabel = null;
	Vector3 wlOriginalLabelScale = Vector3.one;
	Vector3 wlOriginalPos = Vector3.zero;
	List<ScaleAnimationElements> wlSaeList = new List<ScaleAnimationElements>();

	AnimationsMashUp wlAnimMashup = null;
	int wlFromVal = 0;
	int wlToVal = 0;
	object wlClassPointer = null;
	string wlCallbackFunction = null;
	object[] wlParameters = null;
	public void setCountLabelAnimation(bool cached, UILabel label, int fromVal, int toVal, float time = 0.3f, 
	                                   object classPtr = null, string callbackFcn = null, object[] param = null) {
		label.text = fromVal.ToString ("N0");
		//label.text = fromVal.ConvertIntToCurrency ();
		wlOriginalLabelScale = label.cachedTransform.localScale;
		wlOriginalPos = label.cachedTransform.localPosition;
		wltimeAnimating = 0f;
		wlLabel = label;
		wlFromVal = fromVal;
		wlToVal = toVal;
		wltime = time;
		wlFromToDiff = (float)(wlToVal - wlFromVal);
		wlAnimMashup = getAvailableAnimMashUp;
		wlAnimMashup.target = wlLabel.cachedTransform;
		wlAnimMashup.wrapMode = cWrapMode.Once;
		wlAnimMashup.animationTime = wltime;

		wlSaeList.Clear ();
		wlcountTime = (float)time * 0.4f;
		wlSaeList.Add(new ScaleAnimationElements(wlOriginalLabelScale, wlcountTime / 2, 0f));
		wlSaeList.Add(new ScaleAnimationElements(wlOriginalLabelScale * 1.8f, wlcountTime / 2, 1 - wlcountTime));
		wlSaeList.Add(new ScaleAnimationElements(wlOriginalLabelScale, wlcountTime / 2, 0f));
		wlAnimMashup.setScaleAnim (wlSaeList);
		//wlAnimMashup.setFadeAnim (FadeTypeV2.FadeIn);
		wlClassPointer = classPtr;
		wlCallbackFunction = callbackFcn;
		wlParameters = param;

	}
	public void startCountLabelAnimMashUp() {
		if (wlAnimMashup != null) {
			wlAnimMashup.start (false, 0.0f, this, "onCountAnimFinished");
			isUpdateRunning = true;
		}
	}
	void updateCountLabelUncached () {
	}
	void updateCountLabelCached() {
		wltimeAnimating += cachedDeltaTime;
		if (wltimeAnimating <= wlcountTime) {
			wlLabel.text = (wlFromVal 
			                + Mathf.RoundToInt((wlFromToDiff * wltimeAnimating / wlcountTime))).ToString ("N0");
			//Debug.LogWarning (Mathf.RoundToInt((wlFromToDiff * wltimeAnimating / wlcountTime)));
		} else {
			wlLabel.text = wlToVal.ToString ("N0");
			wlAnimMashup.target.localPosition = wlOriginalPos;
			wlAnimMashup.target.rotation = Quaternion.identity;
			isUpdateRunning = false;
		}
	}
	public void onCountAnimFinished () {
		wlLabel.cachedTransform.localScale = wlOriginalLabelScale;
		wlToVal = 0;
		wlFromVal = 0;
		wltimeAnimating = 0;
		wltime = 0;
		if (wlClassPointer != null)
			static_coroutine.getInstance.DoReflection (wlClassPointer, wlCallbackFunction, wlParameters, 0);
		if(pmaTargetEmitter != null)
			pmaTargetEmitter.emit = false; 
	}
	public void resetCountAnim () {
		wltime = 0f;
		wltimeAnimating = 0f;
		wlFromToDiff = 0;
		wlLabel = null;
		wlOriginalLabelScale = Vector3.one;
		wlOriginalPos = Vector3.zero;
		wlAnimMashup = null;
		int wlFromVal = 0;
		int wlToVal = 0;
		object wlClassPointer = null;
		string wlCallbackFunction = null;
		object[] wlParameters = null;
	}

	// End Coin Increase

	#endregion

	public void UpdateManager () {
		if (isUpdateRunning) {
			cachedDeltaTime = Time.deltaTime;
			updateCountLabelCached ();
		}
	}

}

