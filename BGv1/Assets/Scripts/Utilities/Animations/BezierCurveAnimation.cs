

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BezierCurveAnimation : CommonAnimation {

	public static BezierCurveAnimation set (GameObject targetGO, List<Vector2> vectorPoints, float time, float delay = 0, bool startImmediately = false, 
	                                        object classPtr = null, string callbackFcn = null, object[] param = null) 
	{
		BezierCurveAnimation tempAnim;
		if ((tempAnim = targetGO.GetComponent<BezierCurveAnimation> ()) == null)
			tempAnim = targetGO.AddComponent<BezierCurveAnimation> ();
		tempAnim.Initialize();
		tempAnim.setValues (vectorPoints, time);
		if (classPtr != null && callbackFcn != null) {
			tempAnim.addCallBack (classPtr, callbackFcn, param, false);
		}
		if (startImmediately)
			tempAnim.StartAnimation (true, delay);
		return tempAnim;
	}

	List<Vector2> vector_Points = new List<Vector2>();

	// Use this for initialization
	void Start () {
		Initialize();
	}

	public override void Initialize () {
		base.Initialize ();
	}
	
	// Update is called once per frame
	void Update () {
		if(isAnimating) {
			if (!UpdateTimeAnimatingFinished ()) {
				float ratio = timeAnimating / animationTime;
				//Debug.Log (vector_Points [0] + " " + vector_Points [2] + " " + ratio);
				transform.localPosition = apply_bezier_n (ratio, vector_Points);
			} else {
				StartAnimation (false);
				doCallback ();
			}
		}
	}

	public override void StartAnimation (bool flag, float delay = 0) {
		transform.localPosition = flag ? vector_Points[0]: vector_Points[vector_Points.Count - 1];
		base.StartAnimation (flag, delay);
	}

	public void setValues(List<Vector2> vectors, float time) {
		animationTime = time;
		vector_Points = vectors;
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
