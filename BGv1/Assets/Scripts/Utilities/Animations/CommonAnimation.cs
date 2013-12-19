using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public enum cWrapMode {
	Once,
	PingPong,
	Loop
}

public class CommonAnimation : CommonCallback {

	//public bool startAnimating;
	public bool isAnimating = false;
	public Transform target;
	public float animationTime = 0.5f;
	public float timeAnimating = 0f;
	protected float ratio = 0;
	public cWrapMode wrapMode;

	protected bool isInitialized;

	protected float cachedDeltaTime;

	public static T GetComponentCommonAnim<T>(GameObject go) where T: CommonAnimation {
		T animComponent = default(T);
		if ((animComponent = go.GetComponent<T> ()) == null) {
			animComponent = go.AddComponent<T> ();
			animComponent.Initialize ();
		}
		return animComponent;
	}

	public virtual void Initialize () {
		if (!isInitialized) {
			target = transform;
			isInitialized = true;
		}
	}

	public virtual void changeTarget (Transform mtarget) {
		target = mtarget;
	}

	public virtual void start (bool flag) {
	}

	public virtual void startDelayed (bool flag, float delay) {
	}

	public virtual void Clean () {
		target = null;
		animationTime = 0;
		timeAnimating = 0;
		ratio = 0;
		isAnimating = false;
		wrapMode = cWrapMode.Once;
	}

	#region Smooth Offset Effect

	public Vector3 GetSmoothOffsetVector (float ratio, Vector3 p1, Vector3 p2) {
		float newRatio = GetRiseBounceConversion (ratio);
		return VectorInterpolate (newRatio, p1, p2);
	}

	float GetRiseBounceConversion(float ratio) {
		return (2.5f * (1 - ratio) * ratio * 1.2f + ratio * ratio);
	}

	Vector3 VectorInterpolate (float ratio, Vector3 p1, Vector3 p2) {
		return p1 + ratio * (p2 - p1);
	}

	float floatInterpolate(float ratio, float p1, float p2) { // For Bounce effect
		float toReturn = p1 + ratio * (p2 - p1);
		return toReturn;
	}
	#endregion

}

public class CommonAnimationElements {
	public float time;
	public float delay;
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 rotationEuler;
}
