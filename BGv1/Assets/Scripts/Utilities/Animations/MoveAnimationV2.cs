using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveAnimationV2 : CommonAnimation {

	public static MoveAnimationV2 set(GameObject target, List<MoveAnimationV2Elements> elemList, bool isGlobal = true, cWrapMode wMode = cWrapMode.Once,
	                                  bool startImmediately = false,
	                                  OnCommonAnimationFinished callBack = null) {
		MoveAnimationV2 tempAnim = GetComponentCommonAnim<MoveAnimationV2>(target);
		tempAnim.setValues(elemList, isGlobal, wMode, callBack);
		if (startImmediately) {
			if (tempAnim.delay > 0)
				tempAnim.startDelayed (true, tempAnim.delay);
			else
				tempAnim.start (true);
		}
		return tempAnim;
	}

	public static MoveAnimationV2 set(GameObject target, Vector3 moveFr, Vector3 moveTo, float time, bool isGlobal = true,
	                                  cWrapMode wMode = cWrapMode.Once, bool startImmediately = false,
	                                  OnCommonAnimationFinished callBack = null, float delay = 0) {

		MoveAnimationV2 tempAnim = GetComponentCommonAnim<MoveAnimationV2> (target);
		tempAnim.setValues (moveFr, moveTo, time, isGlobal, wMode, callBack);
		if (startImmediately) {
			if (delay > 0)
				tempAnim.startDelayed (true, delay);
			else
				tempAnim.start (true);

		}
		return tempAnim;
	}

	public List<MoveAnimationV2Elements> elementsList = new List<MoveAnimationV2Elements> ();
	public List<Vector3> pointsList = new List<Vector3>();
	int listCount = 0;
	private int moveAnimV2CurIndex = 0;

	public Vector3 moveFrom = Vector3.zero;
	public Vector3 moveTo = Vector3.zero;
	public float delay = 0;
	public bool isGlobal = true;

	void Awake () {
		Initialize ();
	}

	public override void Initialize() {
		base.Initialize ();
	}

	public void setValues(Vector3 moveFr, Vector3 mTo, float time, bool mIsGlobal = true, cWrapMode wMode = cWrapMode.Once, 
	                      OnCommonAnimationFinished callback = null) {
		elementsList = null;
		listCount = 0;
		moveAnimV2CurIndex = 0;
		moveFrom = moveFr;
		moveTo = mTo;
		animationTime = time;
		wrapMode = wMode;
		isGlobal = mIsGlobal;
		addCallbackv2 (callback);
	}

	public void setValues (List<MoveAnimationV2Elements> moveAnimV2ElemList, bool mIsGlobal = true, cWrapMode wMode = cWrapMode.Once, 
	                       OnCommonAnimationFinished callback = null) {
		if (moveAnimV2ElemList.Count < 2) {
			Debug.LogError (moveAnimV2ElemList + " must have atleast 2 points");
			return;
		}
		wrapMode = wMode;
		elementsList = moveAnimV2ElemList;
		listCount = elementsList.Count;
		isGlobal = mIsGlobal;
		addCallbackv2 (callback);
	}

	public void setValues(bool mIsGlobal, cWrapMode wMode, params Vector3[] points) {
		if (points.Length < 2) {
			Debug.LogError (points + " must have atleast 2 points");
		}
		wrapMode = wMode;
		pointsList = new List<Vector3> (points);
		listCount = pointsList.Count;
		isGlobal = mIsGlobal;
	}

	public override void start(bool flag) {
		if (flag && listCount > 1) {
			setMoveAnimV2Elem (moveAnimV2CurIndex = 0);
		}
		timeAnimating = 0;
		target.SetPosition (flag ? moveFrom : moveTo, isGlobal);
		isAnimating = flag;

		if(!flag)
			doCallback ();
	}

	public override void startDelayed(bool flag, float delay) {
		if (delay > 0)
			static_coroutine.getInstance.DoReflection (this, "start", new object[1] { flag }, delay);
		else
			start (flag);
	}

	public void stop () { // mainly for PingPong / Loop
		isAnimating = false;
		timeAnimating = 0;
		ratio = 0;
		isAnimating = false;
		doCallback ();
	}

	void Update () {
		if (isAnimating) {
			cachedDeltaTime = Time.deltaTime;
			timeAnimating += cachedDeltaTime;
			if (timeAnimating < animationTime) {
				ratio = timeAnimating / animationTime;
				if(isGlobal)
					target.position = Vector3.Lerp (moveFrom, moveTo, ratio);
				else
					target.localPosition = Vector3.Lerp (moveFrom, moveTo, ratio);
			} else {
				if (listCount > 1) {
					timeAnimating = 0;
					ratio = 0;
					isAnimating = false;
					if (moveAnimV2CurIndex < listCount - 2) {
						setMoveAnimV2Elem (++moveAnimV2CurIndex);
						if(isGlobal)
							target.position = Vector3.Lerp (moveFrom, moveTo, ratio);
						else
							target.localPosition = Vector3.Lerp (moveFrom, moveTo, ratio);
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
			break;
		case cWrapMode.PingPong:
			if (listCount > 1) {
				elementsList.Reverse ();
			} else {
				Vector3 tempFrom = moveFrom;
				moveFrom = moveTo;
				moveTo = tempFrom;
			}
			break;
		case cWrapMode.Loop:
			break;
		}
		start (wrapMode != cWrapMode.Once);
	}

	void setMoveAnimV2Elem (int index) {
		if (index > listCount - 2)
			return;
		animationTime = elementsList [index].time;
		moveFrom = elementsList [index].position;
		delay = elementsList [index].delay;
		moveTo =  elementsList [index + 1].position;
	}

	public override void Clean () {
		base.Clean ();
		elementsList = null;
	}
}

public class MoveAnimationV2Elements: CommonAnimationElements {
	public Vector3 position;
	public bool isGlobal;
}
