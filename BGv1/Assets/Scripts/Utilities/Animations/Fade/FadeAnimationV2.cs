using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum FadeTypeV2 {
	FadeIn,
	FadeOut
}

public class FadeAnimationV2 : CommonAnimation {

	public static FadeAnimationV2 set(GameObject targetGO, List<FadeAnimationV2Elements> fadeAnimElemList, bool startImmediately = false,
	                                  cWrapMode wrapMode = cWrapMode.Once, OnCommonAnimationFinished callback = null, float delay = 0) {
		FadeAnimationV2 tempAnim = GetComponentCommonAnim<FadeAnimationV2>(targetGO);
		tempAnim.setValues (fadeAnimElemList, wrapMode, 
		                   callback);
		if (startImmediately) {
			if (delay > 0)
				tempAnim.start (true);
			else
				tempAnim.startDelayed (true, delay);
		}
		return tempAnim;
	}

	public static FadeAnimationV2 set (GameObject targetGO, float time = 0.5f, FadeTypeV2 type = FadeTypeV2.FadeIn,
	                                  bool startImmediately = false, cWrapMode wrapMode = cWrapMode.Once,
	                                   OnCommonAnimationFinished callback = null, float delay = 0)
	{
		FadeAnimationV2 tempAnim = GetComponentCommonAnim<FadeAnimationV2>(targetGO);
		tempAnim.setValues (type, time, wrapMode,
		                    callback);
		if (startImmediately) {
			if (delay > 0)
				tempAnim.start (true);
			else
				tempAnim.startDelayed (true, delay);
		}
		return tempAnim;
	}

	enum FadeUpdateType {
		Classic,
		Elements,
		Parameters,
		None
	}
	FadeUpdateType fUpdateType = FadeUpdateType.None;

	private FadeAnimationV2Components m_commonAnimationComponents = null;
	public FadeTypeV2 fadeTypeV2 = (FadeTypeV2)(-1);
	//public float alphaRatio = 1f;

	List<FadeAnimationV2Elements> fadeAnimElementsList = new List<FadeAnimationV2Elements> ();
	List<float> fadeAnimRatioList = new List<float>();
	float fromRatio = 0.5f;
	float toRatio = 0.5f;
	int listCount = 0;
	int fadeAnimElemListCurIndex = 0;
	float delay = 0;

	public bool paused {
		get {
			return !isAnimating;
		}
		set {
			isAnimating = value;
		}
	}

	void Awake () {
		Initialize ();
	}

	public override void Initialize () {
		if (!isInitialized) {
			target = transform;
			m_commonAnimationComponents = new FadeAnimationV2Components (transform);
			isInitialized = true;
		}
	}

	public override void changeTarget (Transform mtarget) {
		base.changeTarget (mtarget);
		m_commonAnimationComponents = new FadeAnimationV2Components (mtarget);
	}

	public void setValues (float time, cWrapMode wMode, OnCommonAnimationFinished callback, params float[] ratios) {
		if (ratios.Length < 2) {
			Debug.LogError (ratios + " must have atleast 2 elements");
			return;
		}
		fUpdateType = FadeUpdateType.Parameters;
		fadeAnimElementsList = null;
		fadeAnimRatioList = new List<float> (ratios);
		listCount = fadeAnimRatioList.Count;
		animationTime = time / (listCount - 1);
		wrapMode = wMode;
		addCallbackv2 (callback);
	}

	public void setValues (List<FadeAnimationV2Elements> fadeAnimElemList, cWrapMode wMode = cWrapMode.Once,
	                       OnCommonAnimationFinished callback = null) {
		if (fadeAnimElemList.Count < 2) {
			Debug.LogError (fadeAnimElemList + " must have atleast 2 elements");
			return;
		}
		fUpdateType = FadeUpdateType.Elements;
		fadeAnimRatioList = null;
		fadeAnimElementsList = fadeAnimElemList;
		listCount = fadeAnimElemList.Count;
		fadeTypeV2 = (FadeTypeV2)(-1);
		wrapMode = wMode;
		addCallbackv2 (callback);
	}

	public void setValues(float mFromRatio, float mToRatio, float time, cWrapMode wMode = cWrapMode.Once,
	                      OnCommonAnimationFinished callback = null) {
		fUpdateType = FadeUpdateType.Classic;
		fadeAnimElementsList = null;
		listCount = 0;
		fromRatio = mFromRatio;
		toRatio = mToRatio;
		animationTime = time;
		wrapMode = wMode;
		addCallbackv2 (callback);
	}

	public void setValues(FadeTypeV2 ptype, float time, cWrapMode wMode = cWrapMode.Once,
	                      OnCommonAnimationFinished callback = null) {
		fadeTypeV2 = ptype;
		setValues ((FadeTypeV2.FadeIn == ptype ? 0f : 1f),
		          (FadeTypeV2.FadeIn == ptype ? 1f : 0f), time, wMode, callback);
	}
	
	public override void start(bool flag) {
		if (fUpdateType == FadeUpdateType.None) {
			Debug.LogError ("No values are yet set.");
			return;
		}
		if (flag && listCount > 1) {
			switch(fUpdateType) {
			case FadeUpdateType.Elements:
				setFadeAnimV2Elems (fadeAnimElemListCurIndex = 0);
				break;
			case FadeUpdateType.Parameters:
				setFadeAnimV2Params (fadeAnimElemListCurIndex = 0);
				break;
			}
		}
		m_commonAnimationComponents.setComponentsAlphaRatioTo (flag ? fromRatio: toRatio);
		timeAnimating = 0;
		ratio = 0;
		isAnimating = flag;
		if (!flag)
			doCallback ();
	}

	public override void startDelayed(bool flag, float delay = 0.001f) {
		if (delay > 0)
			static_coroutine.getInstance.DoReflection (this, "start", new object[1] { flag }, delay);
		else
			start (flag);
	}

	public void stop () {
		timeAnimating = 0;
		ratio = 0;
		isAnimating = false;
		doCallback ();
	}

	public void stopDelayed ( float delay = 0.001f ) {
		if (delay > 0)
			static_coroutine.getInstance.DoReflection (this, "stop", null, delay);
		else
			stop ();
	}


	void Update () {
		if (isAnimating) {
			timeAnimating += Time.deltaTime;
			ratio = timeAnimating / animationTime;
			ratio = fromRatio + (ratio * (toRatio - fromRatio));
			switch (fUpdateType) {
			case FadeUpdateType.Elements:
				UpdateListFadeAnim ();
				break;
			case FadeUpdateType.Parameters:
				UpdateParamsFadeAnim ();
				break;
			case FadeUpdateType.Classic:
				UpdateSingleFadeAnim ();
				break;
			}
		}
	}

	void UpdateParamsFadeAnim () {
		if (animationTime > timeAnimating) {
			m_commonAnimationComponents.setComponentsAlphaRatioTo (ratio);
		} else {
			timeAnimating = 0;
			ratio = 0;
			isAnimating = false;
			if (fadeAnimElemListCurIndex < listCount - 2) {
				setFadeAnimV2Params (++fadeAnimElemListCurIndex);
				isAnimating = true;
			} else {
				checkAnimationFinishedAction ();
			}
		}
	} 

	void UpdateSingleFadeAnim () {
		if (animationTime > timeAnimating) {
			m_commonAnimationComponents.setComponentsAlphaRatioTo (ratio);
		} else {
			checkAnimationFinishedAction ();
		}
	}

	void UpdateListFadeAnim () {
		if (fadeAnimElemListCurIndex < listCount - 1) {
			if (animationTime > timeAnimating) {
				m_commonAnimationComponents.setComponentsAlphaRatioTo (ratio);
			} else {
				timeAnimating = 0;
				ratio = 0;
				isAnimating = false;
				m_commonAnimationComponents.setComponentsAlphaRatioTo (toRatio);
				setFadeAnimV2Elems (++fadeAnimElemListCurIndex);
				static_coroutine.getInstance.DoReflection(this, "onNextAnimationDelayFinished", null, delay);
			}
		} else {
			checkAnimationFinishedAction ();
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
				if (listCount > 1) {
					fadeAnimElementsList.Reverse ();
				} else {
					setFromToRatioByType (fadeTypeV2 = (FadeTypeV2.FadeIn == fadeTypeV2 ? 
				                                   FadeTypeV2.FadeOut : FadeTypeV2.FadeIn));
				}
				startDelayed (true, fadeAnimElementsList [0].delay);
				break;
			case cWrapMode.Loop:
				startDelayed(true, fadeAnimElementsList[0].delay);
				break;
		}
	}

	public void setAlpha (float alpha) {
		m_commonAnimationComponents.setComponentsAlphaRatioTo(alpha);
	}

	public float getAlpha {
		get {
			return m_commonAnimationComponents.getComponentsAlphaRatio;
		}
	}

	void setFromToRatioByType (FadeTypeV2 type) {
		fadeTypeV2 = type;
		fromRatio = (type == FadeTypeV2.FadeIn ? 0f : 1f);
		toRatio = (type == FadeTypeV2.FadeIn ? 1f : 0f);
	}

	void setFadeAnimV2Elems (int index) {
		if (index > listCount - 2)
			return;
		animationTime = fadeAnimElementsList [index].time;
		fromRatio = fadeAnimElementsList [index].alphaRatio;
		toRatio =  fadeAnimElementsList [index + 1].alphaRatio;
		delay =  fadeAnimElementsList [index].delay;
	}

	void setFadeAnimV2Params (int index) {
		if (index > listCount - 2)
			return;
		fromRatio = fadeAnimRatioList [index];
		toRatio = fadeAnimRatioList [index + 1];
	}

	public override void Clean () {
		base.Clean ();
		fadeAnimElementsList = null;	
		fadeAnimRatioList = null;
		fromRatio = 0;
		toRatio = 0;
	}
}

public class FadeAnimationV2Elements: CommonAnimationElements {
	public FadeAnimationV2Elements(){}
	public FadeAnimationV2Elements(FadeTypeV2 mtype, float mAlphaRatio, float mTime, float mDelay = 0) {
		type = mtype;
		alphaRatio = mAlphaRatio;
		time = mTime;
		delay = mDelay;
	}

	public FadeTypeV2 type;
	public float alphaRatio;
}

public class ChangeColorAnimation: CommonAnimation {
	private FadeAnimationV2Components m_commonAnimationComponents = null;

	int colorListCount = 0;
	int colorListCurrIndex = 0;
	List<Color> _colorList = new List<Color>();
	List<Color> colorList {
		set {
			_colorList = value;
			if (value != null) {
				if (value.Count >= 2) {
					colorFrom = value [0];
					colorTo = value [1];
					colorListCount = value.Count;
					colorListCurrIndex = 0;
				} else {
					Debug.LogError (value + " must have at least 2 value.");
				}
			}
		}
		get {
			return _colorList;
		}
	}
	Color colorFrom;
	Color colorTo;

	public override void Initialize () {
		if (!isInitialized) {
			m_commonAnimationComponents = new FadeAnimationV2Components (target = transform);
			isInitialized = true;
		}
	}

	public override void changeTarget (Transform mtarget) {
		if (mtarget != null) {
			m_commonAnimationComponents = new FadeAnimationV2Components (target = mtarget);
		} else
			Debug.LogWarning (this.name + "Target is null");
	}

	public void setColor (Color color) {
		m_commonAnimationComponents.setComponentsColorTo (color);

	}

	public void setValues(float time, cWrapMode wMode, params Color[] colors) {
		if (colors.Length < 2) {
			Debug.LogError (colors + "must have atleast 2 items");
			return;
		}
		colorList = new List<Color> (colors);
		animationTime = time / (colorListCount - 1);
		wrapMode = wMode;
	}

	public void setValues (Color mcolorFrom, Color mcolorTo, float time = 0.5f, cWrapMode wMode = 0) {
		colorList = null;
		colorListCount = 0;
		colorFrom = mcolorFrom;
		colorTo = mcolorTo;
		animationTime = time;
		wrapMode = wMode;
	}

	public void start (bool flag) {
		if (colorListCount >= 2 && flag) {
			setChangeColorElements (colorListCurrIndex = 0);
		}

		m_commonAnimationComponents.setComponentsColorTo (flag ? colorFrom : colorTo);
		ratio = 0;
		timeAnimating = 0;
		isAnimating = flag;
		if (!flag)
			doCallback ();
	}

	public void startDelayed(bool flag, float delay = 0.000009f) {
		if (delay > 0)
			static_coroutine.getInstance.DoReflection (this, "start", new object[1] { flag }, delay);
		else
			start (flag);
	}

	public void stop () {
		ratio = 0;
		timeAnimating = 0;
		isAnimating = false;
		doCallback ();
	}

	void Update () {
		if (isAnimating) {
			timeAnimating += Time.deltaTime;
			ratio = timeAnimating / animationTime;
			if (timeAnimating < animationTime) {
				Color tempColor = Color.Lerp (colorFrom, colorTo, ratio);
				m_commonAnimationComponents.setComponentsColorTo (tempColor);
			} else {
				if (colorListCount >= 2) {
					timeAnimating = 0;
					ratio = 0;

					if (colorListCurrIndex < colorListCount - 2) {
						setChangeColorElements (++colorListCurrIndex);
						m_commonAnimationComponents.setComponentsColorTo (colorFrom);
					} else {
						checkAnimationFinishedAction ();
					}
				} else {
					checkAnimationFinishedAction ();
				}
			}
		}
	}

	void checkAnimationFinishedAction () {
		switch (wrapMode) {
			case cWrapMode.Once:
			break;
			case cWrapMode.PingPong:
			if (colorListCount >= 2) {
				colorList.Reverse ();
			} else {
				Color tempcolor = colorFrom;
				colorFrom = colorTo;
				colorTo = tempcolor;
			}
			break;
			case cWrapMode.Loop:
			break;
		}
		start (wrapMode != cWrapMode.Once);
	}

	void setChangeColorElements (int index) {
		if (index >= colorListCount - 1)
			return;
		colorFrom = colorList [index];
		colorTo = colorList [index + 1];
	}

	public override void Clean () {
		base.Clean ();
		colorFrom = default(Color);
		colorTo = default(Color);
		colorList = null;
	}
	
}

internal class FadeAnimationV2Components {
	private Transform target;

	private UIWidget[] widgets = null;
	private Color[] originalWidgetColorList;
	private int widgetCount = 0;

	private List<Material> materials = new List<Material>();
	private Color[] originalMaterialColorList;
	private int materialListCount = 0;

	private float initialAlpha = 0;
	public float getInitialAlpha {
		get {
			if (initialAlpha >= 0) {
				if (widgetCount > 0)
					initialAlpha = widgets [0].alpha;
				else if (materialListCount > 0)
					initialAlpha = materials [0].color.a;
			}
			return initialAlpha;
		}
	}

	public void resetInitialAlpha() {
		initialAlpha = -1;
	}

	public FadeAnimationV2Components (Transform m_target) {
		Initialize (m_target);
	}


	public void setComponentsColorTo(Color color) {
		if (widgetCount > 0)
			setWidgetComponentsColorTo (color);
		if (materialListCount > 0)
			setMaterialColorTo (color);
	}

	public void setComponentsAlphaRatioTo(float ratio) {
		if (widgetCount > 0)
			setWidgetComponentsAlphaTo (ratio);
		if (materialListCount > 0)
			setMaterialAlphaTo (ratio);
		componentsAlphaRatio = ratio;
	}

	float componentsAlphaRatio = -1;
	public float getComponentsAlphaRatio {
		get {
			if (componentsAlphaRatio > 0)
				return componentsAlphaRatio;
			if (widgetCount > 0)
				return getWidgetComponentsAlpha;
			if (materialListCount > 0)
				return getMateriaComponentsAlpha;
			return 1;
		}
	}

	private FadeAnimationV2Components() {

	}

	private void Initialize(Transform m_target) {
		target = m_target;
		findWidgets ();
		findMaterials ();
	}

	void setWidgetComponentsAlphaTo(float ratio) {
		for (int i = 0; i < widgetCount; i++)
			widgets [i].alpha = ratio;
	}

	float getWidgetComponentsAlpha {
		get {
			return (componentsAlphaRatio = widgets [0].alpha);
		}
	}

	void setMaterialAlphaTo(float ratio) {
		for (int i = 0; i < materialListCount; i++)
			materials [i].SetAlpha (ratio);
	}

	float getMateriaComponentsAlpha {
		get {

			return (componentsAlphaRatio = materials [0].color.a);
		}
	}

	void setWidgetComponentsColorTo(Color color) {
		for (int i = 0; i < widgetCount; i++)
			widgets [i].color = color;
	}

	void setMaterialColorTo(Color color) {
		for (int i = 0; i < materialListCount; i++)
			materials [i].color = color;
	}

	void findWidgets () {
		if (target != null) {
			widgets = target.GetComponentsInChildren<UIWidget> (true);
			widgetCount = widgets.Length;
		}
	}

	void findMaterials () {
		if (target != null) {
			Renderer[] renderers = target.GetComponentsInChildren<Renderer> (true);
			if (renderers.Length > 0) {
				for (int i = 0; i < renderers.Length; i++) {
					Material tempMat = renderers [i].material;
					if (tempMat != null) {
						if(tempMat.HasProperty("_Color")) {
							materials.Add (tempMat);
						materialListCount = materials.Count;
						}
					}
				}
			}
		}
	}
	
}
