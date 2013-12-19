using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISpriteGIFAnimation : CommonAnimation {

	public UISprite targetSprite;
	public List<string> spriteNameList;
	public int spriteNameListCount = 0;
	int currIndex = 0;

	public override void Initialize () {
		if (!isInitialized) {
			target = transform;
			targetSprite = target.GetComponent<UISprite> ();
			isInitialized = true;
		}
	}

	public override void changeTarget (Transform mtarget) {
		UISprite tempSprite = mtarget.GetComponent<UISprite> ();
		if (tempSprite != null) {
			targetSprite = tempSprite;
			target = mtarget;
		}
	}

	public void setValues(float time, cWrapMode wmode, List<string> mspriteList) {
		spriteNameList = null;
		animationTime = time;
		wrapMode = wmode;
		spriteNameList = mspriteList;
		spriteNameListCount = spriteNameList.Count;
	}

	public void setValues(float time, cWrapMode wmode, params string[] sprites) {
		spriteNameList = null;
		animationTime = time;
		wrapMode = wmode;
		spriteNameList = new List<string> (sprites);
		spriteNameListCount = spriteNameList.Count;
	}

	public override void start (bool flag) {
		isAnimating = flag;
		targetSprite.spriteName = flag ? spriteNameList [0] : spriteNameList [spriteNameListCount - 1];
		ratio = 0;
		timeAnimating = 0;
		currIndex = 0;

		if (!flag)
			doCallback ();
	}
	

	public override void startDelayed (bool flag, float delay) {
		if (delay > 0)
			static_coroutine.getInstance.DoReflection (this, "start", new object[1]{flag}, delay);
		else
			start (flag);
	}

	void Update () {
		if (isAnimating) {
			timeAnimating += Time.deltaTime;
			ratio = timeAnimating / animationTime;
			if (timeAnimating < animationTime) {
				int index = Mathf.RoundToInt (ratio * (spriteNameListCount-1));
				if (currIndex < index) {
					currIndex = index;
					targetSprite.spriteName = spriteNameList [index];
					targetSprite.MakePixelPerfect ();
				}
			} else {
				CheckAnimationFinished ();
			}
		}
	}

	void CheckAnimationFinished () {
		switch (wrapMode) {
		case cWrapMode.Once:
			break;
		case cWrapMode.Loop:
			break;
		case cWrapMode.PingPong:
			spriteNameList.Reverse ();
			break;
		}
		start (wrapMode != cWrapMode.Once);
	}

}
