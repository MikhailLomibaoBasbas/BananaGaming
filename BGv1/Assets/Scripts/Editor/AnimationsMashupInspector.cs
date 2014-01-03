using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[CustomEditor(typeof(AnimationsMashUp))]
public class AnimationsMashupInspector : Editor {
	protected AnimationsMashUp mAnimationsMashUp;
	protected Transform mTarget;
	protected float mAnimationTime;
	protected cWrapMode mWrapMode;
	protected bool isInitialized = false;

	const float PCOUNT_WIDTH = 160f;
	const float ISGLOBAL_WIDTH = 140f;

	const float VECTOR3_WIDTH = 195f;
	const float LISTELEM_HEIGHT = 38f;

	static float DEF_LABEL_WIDTH = 130f;
	static float DEF_FIELD_WIDTH = 60;


	const int MINI_BZR_PCOUNT = 3;
	
	//Variables for Move Animation
	bool mvIsInspectorShown = false;
	bool mvIsGlobal = false;
	int mvNewPointsCount = 0;
	int mvOldPointsCount = 0;
	List<Vector3> mvPointsList = new List<Vector3> ();

	//Variables for Rotate Animation
	bool rtIsInspectorShown = false;
	bool rtIsGlobal = false;
	int rtNewPointsCount = 0;
	int rtOldPointsCount = 0;
	List<Vector3> rtPointsList = new List<Vector3> ();

	//Variables for Spline Animation
	bool spIsInspectorShown = false;
	bool spIsGlobal = false;
	int spNewPointsCount = 0;
	int spOldPointsCount = 0;
	List<Vector3> spPointsList = new List<Vector3> ();

	//Variables for Scale Animation
	bool scIsInspectorShown = false;
	int scNewPointsCount = 0;
	int scOldPointsCount = 0;
	List<Vector3> scPointsList = new List<Vector3> ();

	//Variables for Bezier Animation
	bool bzIsInspectorShown = false;
	bool bzIsGlobal = false;
	int bzNewCurvesCount = 0;
	int bzOldCurvesCount = 0;
	List<List<Vector2>> bzCurvesList = new List<List<Vector2>> ();

	//Variables for Change Color Animation
	bool ccIsInspectorShown = false;
	int ccNewColorsCount = 0;
	int ccOldColorsCount = 0;
	List<Color> ccColorList = new List<Color> ();

	//Variables for fade Color Animation
	bool faIsInspectorShown = false;
	int faNewOpacityCount = 0;
	int faOldOpacityCount = 0;
	List<float> faOpacityList = new List<float> ();

	private GUIStyle _internalGuiStyle = null;
	protected GUIStyle getInternalGuiStyle {
		get {
			if (_internalGuiStyle == null) {
				_internalGuiStyle = new GUIStyle ();
				_internalGuiStyle.fontSize = 11;
				_internalGuiStyle.fontStyle = FontStyle.Bold;
			}
			return _internalGuiStyle;
		}
	}


	public override void OnInspectorGUI () {
		if (!isInitialized) {
			isInitialized = true;
			OnInit ();
			Debug.Log ("init");
		}	

		DrawCommonProperties ();
		OnDrawProperties ();
		DrawPostCommonProperties ();

	}

	protected virtual void OnInit (){
		mAnimationsMashUp = target as AnimationsMashUp;
		mAnimationsMashUp.getFadeAnim.hideFlags = HideFlags.HideInInspector;
		mAnimationsMashUp.getMoveAnim.hideFlags = HideFlags.HideInInspector;
		mAnimationsMashUp.getRotateAnim.hideFlags = HideFlags.HideInInspector;
		mAnimationsMashUp.getScaleAnim.hideFlags = HideFlags.HideInInspector;
		mAnimationsMashUp.getChangeColorAnim.hideFlags = HideFlags.HideInInspector;
		mAnimationsMashUp.getSplineAnim.hideFlags = HideFlags.HideInInspector;
		mAnimationsMashUp.getbezierAnim.hideFlags = HideFlags.HideInInspector;

	 	mTarget = mAnimationsMashUp.target;
		mAnimationTime = mAnimationsMashUp.animationTime;
		mWrapMode = mAnimationsMashUp.wrapMode;

		mvOldPointsCount = mvNewPointsCount = mAnimationsMashUp.getMoveAnim.pointsList.Count;
		mvPointsList = mAnimationsMashUp.getMoveAnim.pointsList;
	}

	protected virtual void DrawCommonProperties () {
		//EditorGUIUtility.LookLikeControls (100f);
		EditorGUIUtility.LookLikeInspector ();
		mTarget = EditorGUILayout.ObjectField ("Target", mTarget, typeof(Transform), true) as Transform;
		if (mTarget != mAnimationsMashUp.target) mAnimationsMashUp.target = mTarget;
		mAnimationTime = EditorGUILayout.FloatField ("Animation Time", mAnimationTime);
		if (mAnimationTime != mAnimationsMashUp.animationTime) mAnimationsMashUp.animationTime = mAnimationTime;
		mWrapMode = (cWrapMode)EditorGUILayout.EnumPopup ("cWrap Mode", mWrapMode);
		if (mWrapMode != mAnimationsMashUp.wrapMode) mAnimationsMashUp.wrapMode = mWrapMode;
	}

	protected virtual void OnDrawProperties () {
		EditorGUIUtility.LookLikeControls (DEF_LABEL_WIDTH, DEF_FIELD_WIDTH);
		OnDrawMoveAnimation ();
		OnDrawRotateAnimation ();
		OnDrawScaleAnimation ();
		OnDrawSplineAnimation ();
		OnDrawBezierCurveAnimation ();
		OnDrawChangeColorAnimation ();
		OnDrawFadeAnimation ();
	}

	void DrawPostCommonProperties () {
		NGUIEditorTools.DrawSeparator ();
		GUILayout.BeginHorizontal ();
		{
			if (GUILayout.Button ("Start Animation")) {
				Debug.Log (bzCurvesList.Count);
				if (!mAnimationsMashUp.isAnimating && mTarget != null) {
					mAnimationsMashUp.target = mTarget;
					mAnimationsMashUp.animationTime = mAnimationTime;
					mAnimationsMashUp.wrapMode = mWrapMode;
					mAnimationsMashUp.start (true);
				}
			}

			if (GUILayout.Button ("Stop Animation")) {
				if (mAnimationsMashUp.isAnimating && mTarget != null) {
					mAnimationsMashUp.stop ();
				}
			}
		}
		GUILayout.EndHorizontal ();
	}

	protected void OnDrawMoveAnimation () {
		NGUIEditorTools.DrawSeparator ();
		setIndentLevel (0);
		mvIsInspectorShown = EditorGUILayout.Foldout (mvIsInspectorShown, "Move Animation");
		//EditorGUIUtility.LookLikeControls ();
		if (mvIsInspectorShown) {
			incrementIndentLevel ();
			mvIsGlobal = EditorGUILayout.Toggle ("Is Global", mvIsGlobal, MaxWidth(ISGLOBAL_WIDTH));
			mvNewPointsCount = EditorGUILayout.IntField ("Points Count", mvNewPointsCount, MaxWidth(PCOUNT_WIDTH));
			incrementIndentLevel ();
			OnUpdateList<Vector3> (mvPointsList, ref mvOldPointsCount, ref mvNewPointsCount);
			mAnimationsMashUp.getMoveAnim.pointsList = GetNewListByComparing<Vector3> (mAnimationsMashUp.getMoveAnim.pointsList,
			                                                                                   mvPointsList);
			if (mvOldPointsCount > 0) {
				mvPointsList [0] = EditorGUILayout.Vector3Field ("Start Point", mvPointsList [0], MaxWidth (VECTOR3_WIDTH), MaxHeight (LISTELEM_HEIGHT));
				for (int i = 1; i < mvOldPointsCount; i++) {
					mvPointsList [i] = EditorGUILayout.Vector3Field ("Point " + i, mvPointsList [i], MaxWidth (VECTOR3_WIDTH), MaxHeight (LISTELEM_HEIGHT));
				}
			}
		}
	}

	protected void OnDrawRotateAnimation () {
		NGUIEditorTools.DrawSeparator ();
		setIndentLevel (0);
		rtIsInspectorShown = EditorGUILayout.Foldout (rtIsInspectorShown, "Rotate Animation");
		//EditorGUIUtility.LookLikeControls ();
		if (rtIsInspectorShown) {
			incrementIndentLevel ();
			rtIsGlobal = EditorGUILayout.Toggle ("Is Global", rtIsGlobal, MaxWidth (ISGLOBAL_WIDTH));
			rtNewPointsCount = EditorGUILayout.IntField ("Points Count", rtNewPointsCount, MaxWidth (PCOUNT_WIDTH));
			incrementIndentLevel ();
			OnUpdateList<Vector3> (rtPointsList, ref rtOldPointsCount, ref rtNewPointsCount); 
			if (rtOldPointsCount > 0) {
				rtPointsList [0] = EditorGUILayout.Vector3Field ("Start Point", rtPointsList [0], MaxWidth (VECTOR3_WIDTH), MaxHeight (LISTELEM_HEIGHT));
				for (int i = 1; i < rtOldPointsCount; i++) {
					rtPointsList [i] = EditorGUILayout.Vector3Field ("Point " + i, rtPointsList [i], MaxWidth (VECTOR3_WIDTH), MaxHeight (LISTELEM_HEIGHT));
				}
			}
		}
	}

	protected void OnDrawScaleAnimation () {
		NGUIEditorTools.DrawSeparator ();
		setIndentLevel (0);
		scIsInspectorShown = EditorGUILayout.Foldout (scIsInspectorShown, "Scale Animation");
		//EditorGUIUtility.LookLikeControls ();
		if (scIsInspectorShown) {
			incrementIndentLevel ();
			scNewPointsCount = EditorGUILayout.IntField ("Points Count", scNewPointsCount, MaxWidth (PCOUNT_WIDTH));
			incrementIndentLevel ();
			OnUpdateList<Vector3> (scPointsList, ref scOldPointsCount, ref scNewPointsCount); 
			if (scOldPointsCount > 0) {
				scPointsList [0] = EditorGUILayout.Vector3Field ("Start Point", scPointsList [0], MaxWidth (200f), MaxHeight (38f));
				for (int i = 1; i < scOldPointsCount; i++) {
					scPointsList [i] = EditorGUILayout.Vector3Field ("Point " + i, scPointsList [i], MaxWidth (200f), MaxHeight (38f));
				}
			}
		}
	}

	protected void OnDrawSplineAnimation () {
		NGUIEditorTools.DrawSeparator ();
		setIndentLevel (0);
		spIsInspectorShown = EditorGUILayout.Foldout (spIsInspectorShown, "Spline Animation");
		//EditorGUIUtility.LookLikeControls ();
		if (spIsInspectorShown) {
			incrementIndentLevel ();
			spIsGlobal = EditorGUILayout.Toggle ("Is Global", spIsGlobal, MaxWidth(ISGLOBAL_WIDTH));
			spNewPointsCount = EditorGUILayout.IntField ("Points Count", spNewPointsCount, MaxWidth (PCOUNT_WIDTH));
			incrementIndentLevel ();
			OnUpdateList<Vector3> (spPointsList, ref spOldPointsCount, ref spNewPointsCount);
			if (spOldPointsCount > 0) {
				spPointsList [0] = EditorGUILayout.Vector3Field ("Start Point", spPointsList [0], MaxWidth (VECTOR3_WIDTH), MaxHeight (LISTELEM_HEIGHT));
				for (int i = 1; i < spOldPointsCount; i++) {
					spPointsList [i] = EditorGUILayout.Vector3Field ("Point " + i, spPointsList [i], MaxWidth (VECTOR3_WIDTH), MaxHeight (LISTELEM_HEIGHT));
				}
			}
		}
	}

	protected void OnDrawBezierCurveAnimation () {
		NGUIEditorTools.DrawSeparator ();
		setIndentLevel (0);
		bzIsInspectorShown = EditorGUILayout.Foldout (bzIsInspectorShown, "Bezier Animation");
		if (bzIsInspectorShown) {
			setIndentLevel (1);
			bzIsGlobal = EditorGUILayout.Toggle ("Is Global", bzIsGlobal, MaxWidth(ISGLOBAL_WIDTH));
			bzNewCurvesCount = EditorGUILayout.IntField ("Curves Count", bzNewCurvesCount, MaxWidth (PCOUNT_WIDTH));
			setIndentLevel (2);
			if (bzNewCurvesCount < 0) {
				bzOldCurvesCount = bzNewCurvesCount = 0;
				return;
			}
			if (bzNewCurvesCount != bzOldCurvesCount) {
				if (bzNewCurvesCount > bzOldCurvesCount) {
					List<Vector2>[] tempArray = new List<Vector2>[bzNewCurvesCount - bzOldCurvesCount];
					for (int i = 0; i < tempArray.Length; i++) {
						tempArray [i] = new List<Vector2> (new Vector2[3]);
					}
					bzCurvesList.AddRange (tempArray);
					//bzCurvesList.AddRange (new List<Vector2>[bzNewCurvesCount - bzOldCurvesCount]);
				} else {
					if (bzNewCurvesCount > 0) {
						bzCurvesList.RemoveRange (bzNewCurvesCount, bzOldCurvesCount - bzNewCurvesCount); 
					} else
						bzCurvesList.Clear ();
				}
				bzOldCurvesCount = bzNewCurvesCount;
			}
			for (int j = 0; j < bzOldCurvesCount; j++) {
				setIndentLevel (3);
				List<Vector2> tempBzpList = bzCurvesList[j];
				int tempOldBzpCount = tempBzpList.Count;
				int tempNewBzpCount = 0;
				tempNewBzpCount = EditorGUILayout.IntField ("Curve " + (j + 1), tempOldBzpCount, MaxWidth (PCOUNT_WIDTH));
				if (tempNewBzpCount < MINI_BZR_PCOUNT)
					tempNewBzpCount = tempOldBzpCount = MINI_BZR_PCOUNT;
				OnUpdateList<Vector2> (tempBzpList, ref tempOldBzpCount, ref tempNewBzpCount);
				if (tempOldBzpCount > 0) {
					setIndentLevel (4);
					tempBzpList [0] = EditorGUILayout.Vector2Field ("Start Point", tempBzpList [0], MaxWidth (VECTOR3_WIDTH), MaxHeight (LISTELEM_HEIGHT));
					for (int i = 1; i < tempOldBzpCount; i++) {
						tempBzpList [i] = EditorGUILayout.Vector2Field ("Point " + i, tempBzpList [i], MaxWidth (VECTOR3_WIDTH), MaxHeight (LISTELEM_HEIGHT));
					}
				}
				EditorGUILayout.Space ();
			}
		}
	}

	protected void OnDrawChangeColorAnimation () {
		NGUIEditorTools.DrawSeparator ();
		setIndentLevel (0);
		ccIsInspectorShown = EditorGUILayout.Foldout (ccIsInspectorShown, "Color Animation");
		//EditorGUIUtility.LookLikeControls ();
		if (ccIsInspectorShown) {
			incrementIndentLevel ();
			ccNewColorsCount = EditorGUILayout.IntField ("Colors Count", ccNewColorsCount, MaxWidth (PCOUNT_WIDTH));
			incrementIndentLevel ();
			OnUpdateList<Color> (ccColorList, ref ccOldColorsCount, ref ccNewColorsCount);
			if (ccOldColorsCount > 0) {
				ccColorList [0] = EditorGUILayout.ColorField ("Starting Color", ccColorList [0], MaxWidth (VECTOR3_WIDTH), MaxHeight (LISTELEM_HEIGHT));
				for (int i = 1; i < ccOldColorsCount; i++) {
					ccColorList [i] = EditorGUILayout.ColorField ("Color " + i, ccColorList [i], MaxWidth (VECTOR3_WIDTH), MaxHeight (LISTELEM_HEIGHT));
				}
			}
		}
	}

	protected void OnDrawFadeAnimation () {
		NGUIEditorTools.DrawSeparator ();
		setIndentLevel (0);
		faIsInspectorShown = EditorGUILayout.Foldout (faIsInspectorShown, "Fade Animation");
		//EditorGUIUtility.LookLikeControls ();
		if (faIsInspectorShown) {
			incrementIndentLevel ();
			faNewOpacityCount = EditorGUILayout.IntField ("Opacities Count", faNewOpacityCount, MaxWidth (PCOUNT_WIDTH));
			incrementIndentLevel ();
			OnUpdateList<float> (faOpacityList, ref faOldOpacityCount, ref faNewOpacityCount);
			if (faOldOpacityCount > 0) {
				faOpacityList [0] = EditorGUILayout.FloatField ("Starting Opacity", faOpacityList [0], MaxWidth (PCOUNT_WIDTH - 10), MaxHeight (LISTELEM_HEIGHT));
				for (int i = 1; i < faOldOpacityCount; i++) {
					faOpacityList [i] = EditorGUILayout.FloatField ("Opac " + i, faOpacityList [i],  MaxWidth (PCOUNT_WIDTH - 10), MaxHeight (LISTELEM_HEIGHT));
				}
			}
		}
	}


	#region Utility Methods
	void SetRevised (string name, Object[] objs) {
		if (objs != null) {
			int objsLength = objs.Length;
			for (int i = 0; i < objsLength; i++) {
				Undo.RegisterUndo (objs [i], name);
				EditorUtility.SetDirty (objs [i]);
			}
		} else {
			Undo.RegisterSceneUndo (name);
		}
	}

	void OnUpdateList<T>(List<T> gList, ref int oldCount, ref int newCount) {
		if (newCount < 0) {
			oldCount = newCount = 0;
			return;
		}
		if (newCount != oldCount) {
			if (newCount > oldCount) {
				gList.AddRange (new T[newCount - oldCount]);
			} else {
				if (newCount > 0) {
					gList.RemoveRange (newCount, oldCount - newCount); 
				} else
					gList.Clear ();
				Debug.Log ("lols " + newCount + " " + oldCount + " " + gList.Count);
			}
			oldCount = newCount;
		}
	}

	List<T> GetNewListByComparing<T>(List<T> oldList, List<T> newList) {
		if (!oldList.SequenceEqual (newList))
			return newList;
		return oldList;
	}


	//Still Not Working
	void OnDrawListOfVector3(List<Vector3> gList, int count) {
		if (count > 0) {
			gList [0] = EditorGUILayout.Vector3Field ("Start Point", gList [0], MaxWidth (200f), MaxHeight (38f));
			for (int i = 1; i < scOldPointsCount; i++) {
				gList [i] = EditorGUILayout.Vector3Field ("Point " + i, gList [i], MaxWidth (200f), MaxHeight (38f));
			}
		}
	}

	void setIndentLevel(int val) {EditorGUI.indentLevel = val;}
	void incrementIndentLevel(){EditorGUI.indentLevel++;}
	void decrementIndentLevel(){EditorGUI.indentLevel--;}
	GUILayoutOption MaxWidth(float val){return GUILayout.MaxWidth (val);}
	GUILayoutOption MaxHeight(float val){return GUILayout.MaxHeight (val);}
	#endregion
}
