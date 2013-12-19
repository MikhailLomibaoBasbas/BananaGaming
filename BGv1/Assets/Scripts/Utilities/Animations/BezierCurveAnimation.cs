using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BezierCurveAnimation : CommonAnimation {


	public static BezierCurveAnimation set (GameObject targetGO, List<Vector2> vectorPoints, float time, bool mIsGlobel, float delay = 0, cWrapMode wmode = cWrapMode.Once,
	                                        bool startImmediately = false, object classPtr = null, string callbackFcn = null, object[] param = null) 
	{
		BezierCurveAnimation tempAnim;
		if ((tempAnim = targetGO.GetComponent<BezierCurveAnimation> ()) == null)
			tempAnim = targetGO.AddComponent<BezierCurveAnimation> ();
		tempAnim.Initialize();
		tempAnim.setValues (vectorPoints, time, mIsGlobel, wmode );
		if (classPtr != null && callbackFcn != null) {
			tempAnim.addCallback (classPtr, callbackFcn, param,true);
		}
		if (startImmediately)
			tempAnim.startDelayed (true, delay);
		return tempAnim;
	}

	public List<Vector2> vectorPointsList = new List<Vector2>();
	List<List<Vector2>> listOfVectorPointsList = new List<List<Vector2>>();
	int listCount = 0;
	int listCurrIndex = 0;
	float timePerPoint = 0f;
	bool isGlobal;

	// Use this for initialization
	void Start () {
		Initialize();
	}

	public override void Initialize () {
		base.Initialize ();
	}

	public override void changeTarget (Transform mtarget) {
		base.changeTarget (mtarget);
	}

	public override void start (bool flag) {
		if (flag && listOfVectorPointsList != null) {
			setVariablesByIndex (listCurrIndex = 0);
		}
		target.SetPosition ( flag ? vectorPointsList[0]: vectorPointsList[vectorPointsList.Count - 1], isGlobal);
		timeAnimating = 0f;
		isAnimating = flag;

		if(!flag)
			doCallback ();
		//base.start (flag, delay);
	}

	public override void startDelayed (bool flag, float delay) {
		if (delay > 0)
			static_coroutine.getInstance.DoReflection (this, "start", new object[1] { flag }, delay);
		else
			start (flag);
	}

	public void stop () {
		isAnimating = false;
		doCallback ();
		timeAnimating = 0f;
		ratio = 0f;
	}

	public void setValues(List<Vector2> vectors, float time, bool misGlobal, cWrapMode wMode = cWrapMode.Once) {
		listOfVectorPointsList = null;
		listCount = 0;
		isGlobal = misGlobal;
		wrapMode = wMode;
		timePerPoint = animationTime = time;
		vectorPointsList = vectors;
	}

	public void setValues(List<List<Vector2>> mListofVectorPointList, float time, bool misGlobal, cWrapMode wMode = cWrapMode.Once) {
		if (mListofVectorPointList.Count < 2) {
			Debug.LogError (mListofVectorPointList + " must atleast have 2 items");
			return;
		}
		wrapMode = wMode;
		listOfVectorPointsList = mListofVectorPointList;
		listCount = listOfVectorPointsList.Count;
		timePerPoint = animationTime = time;
		isGlobal = misGlobal;
	}

	public void setValues(float time, cWrapMode wMode, bool misGlobal, params List<Vector2>[] vectorsList) {
		if (vectorsList.Length < 2) {
			Debug.LogError (vectorsList + " must atleast have 2 items");
			return;
		}
		wrapMode = wMode;
		listOfVectorPointsList = new List<List<Vector2>> (vectorsList);
		listCount = listOfVectorPointsList.Count;
		timePerPoint = animationTime = time;
		isGlobal = misGlobal;
	}

	// Update is called once per frame
	void Update () {
		if(isAnimating) {
			timeAnimating += Time.deltaTime;
			if (timeAnimating < timePerPoint) {
				//Debug.LogWarning (target.name + " " + timeAnimating);
				float ratio = timeAnimating / timePerPoint;
				target.SetPosition( apply_bezier_n (ratio, vectorPointsList), isGlobal);
			} else {
				if(listCount > 0) {
					if (listCurrIndex < listCount - 1) {
						isAnimating = false;
						ratio = 0;
						timeAnimating = 0;
						setVariablesByIndex (++listCurrIndex);
						isAnimating = true;
					} else {
						checkAnimationFinished ();
					}
				} else {
					checkAnimationFinished();
				}
			}
		}
	}

	void setVariablesByIndex (int index) {
		vectorPointsList = listOfVectorPointsList [index];
		timePerPoint = animationTime / listCount;
	}

	void checkAnimationFinished () {
		switch (wrapMode) {
		case cWrapMode.Once:
			start (false);
			break;
		case cWrapMode.Loop:
			start (true);
			break;
		case cWrapMode.PingPong:
			if (listCount > 0)
				listOfVectorPointsList.Reverse ();
			else
				vectorPointsList.Reverse ();
			start (true);
			break;
		}
	}
	
	public override void Clean () {
		base.Clean ();
		vectorPointsList = null;
		listOfVectorPointsList = null;
		listCount = 0;
		listCurrIndex = 0;
	}

	/*Logic comes from the DEV GOD, JEFF ORTIGA! -Khail*/

	Dictionary<int,List<int>> pascal_triangles = new Dictionary<int,List<int>>();

	public Vector2 apply_bezier_n(float ratio, List<Vector2> vectors) {

		populate_pascal_triangle(vectors.Count-1);

		List<int> ptriangle = pascal_triangles[vectors.Count-1];
		Vector2 coord = new Vector2(0, 0);

		// applying summation
		float sum = 0.0f;
		for (int x = 0; x < ptriangle.Count; x++)
		{
			int pow = ptriangle.Count - 1 - x;
			sum += ptriangle[x] * (Mathf.Pow((1 - ratio), pow)) * (Mathf.Pow(ratio, x)) * vectors[x].x;
		}
		coord.x = sum;
		sum = 0.0f;
		for (int x = 0; x < ptriangle.Count; x++)
		{
			int pow = ptriangle.Count - 1 - x;
			sum += ptriangle[x] * (Mathf.Pow((1 - ratio), pow)) * (Mathf.Pow(ratio, x)) * vectors[x].y;
		}
		coord.y = sum;
		return coord;
	}

	public void populate_pascal_triangle(int i) {
		for (int x = 1 ; x <= i; x++) {
			if (!pascal_triangles.ContainsKey(x))
				pascal_triangles.Add(x,get_pascal_triangle(x));
		}
	}

	public List<int> get_pascal_triangle(int n) {
		int tx;
		int r = 1;

		List<int> fvalues = new List<int>();
		fvalues.Add(1);

		List<int> values = new List<int>();     // inside loop count
		for (tx = 1; tx <= (n / 2); tx++) {
			r *= n - tx + 1;
			r /= tx;
			values.Add(r);
			fvalues.Add(r);
		}

		if ((n + 1) % 2 == 0)		
			for (int x = values.Count - 1; x >= 0; x--)		  
				fvalues.Add(values[x]);		 
		else
			for (int x = values.Count - 2; x >= 0; x--)		  
				fvalues.Add(values[x]);

		fvalues.Add(1);		
		return fvalues;	
	}
}
