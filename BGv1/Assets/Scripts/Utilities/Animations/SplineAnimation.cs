using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplineNode {
	public SplineNode(Vector3 p, Quaternion q, float t, Vector2 io) { Point = p; Rotation = q; Time = t; EaseIO = io; }
	public SplineNode(SplineNode o) { Point = o.Point; Rotation = o.Rotation; Time = o.Time; EaseIO = o.EaseIO; }
	internal Vector3 Point;
	internal Quaternion Rotation;
	internal float Time;
	internal Vector2 EaseIO;

}

public class SplineAnimation : CommonAnimation {

	public static SplineAnimation set(GameObject targetGO, float time, cWrapMode wMode, bool isGlobal,  bool startImmediately, 
	                                  CommonAnimation.OnCommonAnimationFinished callback, params Vector3[] points) {
		SplineAnimation tempAnim = GetComponentCommonAnim<SplineAnimation> (targetGO);
		tempAnim.Initialize ();
		tempAnim.setValues (time, wMode, isGlobal, points);
		if (callback != null)
			tempAnim.addCallbackv2 (callback);
		if(startImmediately)
			tempAnim.start (true);
		return tempAnim;
	}

	public static SplineAnimation set(GameObject targetGO, float time, cWrapMode wMode, bool isGlobal,  bool startImmediately, 
	                                  object classPtr, string callback, object[] param, params Vector3[] points) {
		SplineAnimation tempAnim = GetComponentCommonAnim<SplineAnimation> (targetGO);
		tempAnim.Initialize ();
		tempAnim.setValues (time, wMode, isGlobal, points);
		if (classPtr != null)
			tempAnim.addCallback (classPtr, callback, param, true);
		if(startImmediately)
			tempAnim.start (true);
		return tempAnim;
	}
	
	public static SplineAnimation set(GameObject targetGO, float time, cWrapMode wMode, bool isGlobal,  bool startImmediately, 
	                                  CommonAnimation.OnCommonAnimationFinished callback, List<SplineNode> nodeList) {
		SplineAnimation tempAnim = GetComponentCommonAnim<SplineAnimation> (targetGO);
		tempAnim.Initialize ();
		tempAnim.setValues (nodeList, wMode, isGlobal);
		if (callback != null)
			tempAnim.addCallbackv2 (callback);
		if(startImmediately)
			tempAnim.start (true);
		return tempAnim;
	}

	public static SplineAnimation set(GameObject targetGO, float time, cWrapMode wMode, bool isGlobal,  bool startImmediately, 
	                                  object classPtr, string callback, object[] param, List<SplineNode> nodeList) {
		SplineAnimation tempAnim = GetComponentCommonAnim<SplineAnimation> (targetGO);
		tempAnim.Initialize ();
		tempAnim.setValues (nodeList, wMode, isGlobal);
		if (classPtr != null)
			tempAnim.addCallback (classPtr, callback, param, true);
		if(startImmediately)
			tempAnim.start (true);
		return tempAnim;
	}

	enum SplineType {
		Points,
		Node
	}
	SplineType type = (SplineType)(-1);
	int listCount = 0;
	int currIndex = 0;
	public Vector2 easeInOut;

	List<SplineNode> nodeList = new List<SplineNode>();
	bool withNodeRotations;
	float animationTime2 = 0;

	//For non-complex spline
	public List<Vector3> pointList = new List<Vector3>();
	float timePerPoint = 0;

	List<Vector3> currIndexPointsNodes = new List<Vector3>(){Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};

	public bool isGlobal;
	bool didFinalizedPoints;

	void Awake() {
		Initialize ();
	}

	public override void Initialize () {
		base.Initialize ();
	}

	public override void changeTarget (Transform mtarget) {
		base.changeTarget (mtarget);
	}

	public void setValues (List<SplineNode> mNodeList, cWrapMode wmode, bool misGlobal) { //over write the current.
		if (mNodeList.Count >= 2) {		
			didFinalizedPoints = false;
			withNodeRotations = true;
			type = SplineType.Node;
			pointList = null;
			nodeList = mNodeList;
			listCount = mNodeList.Count;
			wrapMode = wmode;
			isGlobal = misGlobal;
		} else {
			type = (SplineType)(-1);
			throw new System.Exception (mNodeList + "(" + mNodeList.Count + ") must atleast contain 2 points.");
		}
	}


	public void setValues(float time, cWrapMode wmode, bool misGlobal, params Vector3[] points) {
		if (points.Length >= 2) {
			didFinalizedPoints = false;
			nodeList = null;
			withNodeRotations = false;
			type = SplineType.Points;
			listCount = 0;
			pointList = new List<Vector3> (points);
			listCount = pointList.Count;
			wrapMode = wmode;
			isGlobal = misGlobal;
			timePerPoint = time / (listCount - 1);
			easeInOut = Vector2.up;
		} else {
			type = (SplineType)(-1);
			throw new System.Exception (points + "(" + points.Length + ") must atleast contain 2 points.");
		}
	}

	
	public void setValues (float time, cWrapMode wmode, bool misGlobal, List<Vector3> mpointList) {
		if (mpointList.Count >= 2) {
			didFinalizedPoints = false;
			nodeList = null;
			withNodeRotations = false;
			type = SplineType.Points;
			listCount = 0;
			pointList = mpointList;
			listCount = pointList.Count;
			wrapMode = wmode;
			isGlobal = misGlobal;
			timePerPoint = time / (listCount - 1);
			easeInOut = Vector3.up;
		} else {
			type = (SplineType)(-1);
			throw new System.Exception (mpointList + "(" + mpointList.Count + ") must atleast contain 2 points.");
		}
	}
	

	public override void start(bool flag) {
		currIndex = 1;
		if (flag) {
			finalizeList ();
			setListValuesByIndex (currIndex);
		}
		Vector3 pos = (type == SplineType.Node ? (flag ? nodeList[0].Point : nodeList[nodeList.Count - 2].Point) 
		               : (flag ? pointList[0]: pointList[pointList.Count - 2]));
		target.SetPosition (pos, isGlobal);
		//Debug.LogError (target.position);
		if (type == SplineType.Node) {
			Vector3 rot = (flag ? nodeList [0].Point : nodeList [listCount - 2].Point);
			target.SetRotation (rot, isGlobal);
		}
		timeAnimating = 0;
		isAnimating = flag;
		ratio = 0;

		if (!flag)
			doCallback ();
	}

	public override void startDelayed (bool flag, float delay = 0.00001f) {
		if (delay > 0)
			static_coroutine.getInstance.DoReflection (this, "start", new object[1] { flag }, delay);
		else
			start (flag);
	}

	public void stop() {
		isAnimating = false;
		timeAnimating = 0;
		ratio = 0;
		doCallback ();
	}

	void finalizeList () {
		if (!didFinalizedPoints) {

			switch (type) {
				case SplineType.Points:
					pointList.Insert (0, pointList [0]);
					pointList.Add (pointList [pointList.Count - 1]);
					listCount = pointList.Count;
					break;
				case SplineType.Node:
					if (withNodeRotations) {
						for (int c = 1; c < listCount; c++) {
							SplineNode node = nodeList[c];
							SplineNode prevNode = nodeList[c - 1];

							// Always interpolate using the shortest path -> Selective negation
							if (Quaternion.Dot(node.Rotation, prevNode.Rotation) < 0) {
								node.Rotation.x = -node.Rotation.x;
								node.Rotation.y = -node.Rotation.y;
								node.Rotation.z = -node.Rotation.z;
								node.Rotation.w = -node.Rotation.w;
							}
						}
					}
					nodeList.Insert(0, nodeList[0]);
					nodeList.Add(nodeList[nodeList.Count - 1]);
					listCount = nodeList.Count;
					break;
			}

			didFinalizedPoints = true;
		}


	}
	

	void Update () {
		if (isAnimating) {
			timeAnimating += Time.deltaTime;
			if (timeAnimating < animationTime) {
				//Debug.LogWarning (Time.time);
				if (type == SplineType.Node)
					ratio = (timeAnimating - animationTime) / (animationTime2 - animationTime);
				else
					ratio = timeAnimating / animationTime;
				ratio = MathUtils.Ease(ratio, easeInOut.x, easeInOut.y); //Smooth Param
				target.SetPosition (GetHermiteInternal(ratio), isGlobal);
				if (type == SplineType.Node)
					target.SetRotation (GetSquad (nodeList, currIndex, ratio), isGlobal);
			} else {
				timeAnimating = 0;
				if (currIndex < listCount - 3) {
					setListValuesByIndex (++currIndex);
				} else {
					start (false);
				}
			}
		}
	}

	void setListValuesByIndex(int index) {
		switch (type) {
		case SplineType.Node:
			setNodeValuesByIndex (index);
			break;
		case SplineType.Points:
			setPointValuesByIndex (index);
			break;
		default:
			break;
		}
	}

	void setNodeValuesByIndex (int index) {
		if (index == 0 || index >= listCount - 2)
			return;
		animationTime = nodeList [index].Time;
		animationTime2 = nodeList [index + 1].Time;
		easeInOut = nodeList [index].EaseIO;
		setPointsNodesByIndex (index);
	}

	void setPointValuesByIndex(int index) {
		if (index == 0 || index >= listCount - 2)
			return;
		animationTime = timePerPoint;
		setPointsNodesByIndex (index);
	}

	void setPointsNodesByIndex (int index) {
		Debug.LogWarning ("SplineIndex: " + index);
		for (int i = 0; i < 4; i++) {
			currIndexPointsNodes [i] = (type == SplineType.Points ? pointList [index + (i-1)] : nodeList [index + (i-1)].Point);
		}
	}

	void checkAnimationFinishedAction () {
		switch (wrapMode) {
			case cWrapMode.Once:
				start (false);
				break;
			case cWrapMode.PingPong:
				nodeList.Reverse ();
				break;
			case cWrapMode.Loop:
				break;
		}
	}

	public override void Clean () {
		base.Clean ();
		type = (SplineType)(-1);
		listCount = 0;
		currIndex = 0;
		easeInOut = default(Vector2);
		nodeList = null;
		withNodeRotations = false;
		animationTime2 = 0;
		List<Vector3> pointList = new List<Vector3>();
		timePerPoint = 0;
		currIndexPointsNodes = new List<Vector3>(){Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
		isGlobal = default(bool);
		didFinalizedPoints = default(bool);
	}


	#region Computations
	Quaternion GetSquad(List<SplineNode> mNodes, int idxFirstPoint, float t) {
		Quaternion Q0 = mNodes[idxFirstPoint - 1].Rotation;
		Quaternion Q1 = mNodes[idxFirstPoint].Rotation;
		Quaternion Q2 = mNodes[idxFirstPoint + 1].Rotation;
		Quaternion Q3 = mNodes[idxFirstPoint + 2].Rotation;

		Quaternion T1 = MathUtils.GetSquadIntermediate(Q0, Q1, Q2);
		Quaternion T2 = MathUtils.GetSquadIntermediate(Q1, Q2, Q3);

		return MathUtils.GetQuatSquad(t, Q1, Q2, T1, T2);
	}

	public Vector3 GetHermiteInternal(float t) {
		Vector3 P0 = currIndexPointsNodes[0];
		Vector3 P1 = currIndexPointsNodes [1];
		Vector3 P2 = currIndexPointsNodes [2];
		Vector3 P3 = currIndexPointsNodes [3];

		float t2 = t * t;
		float t3 = t2 * t;
		float tension = 0.5f;	// 0.5 equivale a catmull-rom

		Vector3 T1 = tension * (P2 - P0);
		Vector3 T2 = tension * (P3 - P1);

		float Blend1 = 2 * t3 - 3 * t2 + 1;
		float Blend2 = -2 * t3 + 3 * t2;
		float Blend3 = t3 - 2 * t2 + t;
		float Blend4 = t3 - t2;

		return Blend1 * P1 + Blend2 * P2 + Blend3 * T1 + Blend4 * T2;
	}

	public Vector3 GetHermiteInternal(List<SplineNode> mNodes, int idxFirstPoint, float t) {
		Vector3 P0 = mNodes[idxFirstPoint - 1].Point;
		Vector3 P1 = mNodes[idxFirstPoint].Point;
		Vector3 P2 = mNodes[idxFirstPoint + 1].Point;
		Vector3 P3 = mNodes[idxFirstPoint + 2].Point;

		float t2 = t * t;
		float t3 = t2 * t;
		float tension = 0.5f;	// 0.5 equivale a catmull-rom

		Vector3 T1 = tension * (P2 - P0);
		Vector3 T2 = tension * (P3 - P1);

		float Blend1 = 2 * t3 - 3 * t2 + 1;
		float Blend2 = -2 * t3 + 3 * t2;
		float Blend3 = t3 - 2 * t2 + t;
		float Blend4 = t3 - t2;

		return Blend1 * P1 + Blend2 * P2 + Blend3 * T1 + Blend4 * T2;
	}
	#endregion

}
