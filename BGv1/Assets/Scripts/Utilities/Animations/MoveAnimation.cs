using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MoveAnimType {
	Linear,
	Sine,
	Cosine
}

public class MoveAnimation : CommonAnimation {

	public Vector3 moveFrom;
	public Vector3 moveTo;
	bool _hasBounceEffect;
	public MoveAnimType moveAnimType = MoveAnimType.Linear;

	List<Vector3> _positionsList = new List<Vector3>();

	public float amplitude = 2f;
	public float centerAmplitude = 0;
	public float frequency = 2f;
	public float phase = 0;

	public static MoveAnimation setAnimation (GameObject go, Vector3 mFrom, Vector3 mTo, float time, MoveAnimType type = MoveAnimType.Linear, float delay = 0, 
	                                          bool hasBounceEffect = false, bool backAndForth = false, bool startImmediately = false, 
	                                          object classPtr = null, string callbackFcn = null, object[] args = null,
	                                          bool oneShot = false, AnimationComponentType animComponentType = AnimationComponentType.None)
	{
		MoveAnimation tempAnim;
		if ((tempAnim = go.GetComponent<MoveAnimation> ()) == null) {
			tempAnim = go.AddComponent<MoveAnimation> ();
		}
		tempAnim.Initialize ();
		tempAnim.setValues (mFrom, mTo, time);
		tempAnim.moveAnimType = type;
		if(classPtr != null && callbackFcn != null)
			tempAnim.addCallBack (classPtr, callbackFcn, args, oneShot);
		if (startImmediately)
			tempAnim.StartAnimation (true, delay, hasBounceEffect);

		return tempAnim;
	}

	// Use this for initialization
	void Start () {
		Initialize ();
	}

	public override void Initialize () {
		base.Initialize ();

	}

	public void setValues(Vector3 mFrom, Vector3 mTo, float time, bool backAndForth = false, bool repeating = false) {
		moveFrom = mFrom;
		moveTo = mTo;
		isRepeating = repeating;
		isBackAndForth = backAndForth;
		animationTime = time;
	}

	public void setValues (List<Vector3> posList, float timePerPos, bool backAndForth = false, bool repeating = false) {
		_positionsList = posList;
		isRepeating = repeating;
		isBackAndForth = backAndForth;
		animationTime = timePerPos;
	}

	public void setWaveFormulaValues (float ampl, float centerAmp, float freq, float pha) {
		amplitude = ampl;
		centerAmplitude = centerAmp;
		frequency = freq;
		phase = pha;
	}


	public void checkStartAnimationFlag() {
		if (startAnimating) {
			StartAnimation (true);
			startAnimating = false;
		}
	}


	public void StartAnimation(bool flag, float delay = 0, bool hasBounceEffect = false) {
		target.localPosition = flag ? moveFrom: moveTo;
		_hasBounceEffect = hasBounceEffect;
		base.StartAnimation(flag, delay);
	}

	
	// Update is called once per frame
	void Update () {
		checkStartAnimationFlag ();
		if (isAnimating) {
			//timeAnimating += (Time.timeScale > 0) ? (Time.deltaTime / Time.timeScale): Time.fixedDeltaTime;
			if (!UpdateTimeAnimatingFinished()) {
				ratio = (timeAnimating / animationTime);
				if (targetRigidBody == null) {
					switch(moveAnimType) {
					case MoveAnimType.Linear:
						target.localPosition = getLinearVector (moveFrom, moveTo, ratio);
								break;
						case MoveAnimType.Sine:
							target.localPosition = getSineWaveVector (moveFrom, moveTo, ratio, amplitude, centerAmplitude, frequency, phase);
							break;
						case MoveAnimType.Cosine:
							target.localPosition = getCosineWaveVector (moveFrom, moveTo, ratio, amplitude, centerAmplitude, frequency, phase);
							break;
					}
				} else { 
					//Debug.LogError ("HMM");
					targetRigidBody.MovePosition (Vector3.Lerp (moveFrom, moveTo, ratio));
				}
			} else {
				//Debug.LogError (target.localPosition + "w");
				if (isBackAndForth)
					setValues (moveTo, moveFrom, animationTime, isBackAndForth);
				//target.localPosition = getSineWaveVector (moveFrom, moveTo, 0.01f);
				StartAnimation (isBackAndForth || isRepeating, 0, _hasBounceEffect);
			}
		}
	}
	

	Vector3 getSineWaveVector (Vector3 moveFr, Vector3 moveT, float ratio, 
	                       float ampli = 50f, float centerAmpli = 0f, float frequency = 1f, float phase = 0f) {
		Vector3 tempPos = Vector3.zero;
		if (Mathf.Abs (moveFr.x - moveT.x) > Mathf.Abs (moveFr.y - moveT.y)) {
			tempPos.x = Mathf.Lerp (moveFr.x, moveT.x, ratio);
			tempPos.y = Mathf.Lerp (moveFr.y, moveT.y, ratio) + (ampli * Mathf.Sin ((((Mathf.PI * 2f) * frequency) * ratio) - phase) - centerAmpli);
		} else {
			tempPos.x = Mathf.Lerp (moveFr.x, moveT.x, ratio) + (ampli * Mathf.Sin ((((Mathf.PI * 2f) * frequency) * ratio) - phase) - centerAmpli);
			tempPos.y = Mathf.Lerp (moveFr.y, moveT.y, ratio);
		}
		return tempPos;
	}

	Vector3 getCosineWaveVector (Vector3 moveFr, Vector3 moveT, float ratio, 
	                        float ampli = 50f, float centerAmpli = 0f, float frequency = 1f, float phase = 0f) {
		Vector3 tempPos = Vector3.zero;
		if (Mathf.Abs (moveFr.x - moveT.x) > Mathf.Abs (moveFr.y - moveT.y)) {
			tempPos.x = Mathf.Lerp (moveFr.x, moveT.x, ratio);
			tempPos.y = Mathf.Lerp (moveFr.y, moveT.y, ratio) + (ampli * Mathf.Cos ((((Mathf.PI * 2f) * frequency) * ratio) - phase) - centerAmpli);
		} else {
			tempPos.x = Mathf.Lerp (moveFr.x, moveT.x, ratio) + (ampli * Mathf.Cos ((((Mathf.PI * 2f) * frequency) * ratio) - phase) - centerAmpli);
			tempPos.y = Mathf.Lerp (moveFr.y, moveT.y, ratio);
		}
		return tempPos;
	}

	Vector3 getLinearVector (Vector3 moveFr, Vector3 moveT, float ratio) {
		if (!_hasBounceEffect) {
			return Vector3.Lerp (moveFr, moveT, ratio);
		} else {

			Vector3 bouncePos = Vector3.zero;
			ratio = GetRiseBounceConversion(ratio);
			bouncePos.x = floatInterpolate(ratio, moveFr.x, moveT.x);
			bouncePos.y = floatInterpolate (ratio, moveFr.y, moveT.y);
			bouncePos.z = floatInterpolate (ratio, moveFr.z, moveT.z);
			return bouncePos;
		}
		return Vector3.zero;
	}

	public  float GetRiseBounceConversion(float ratio) {
		return (2.5f * (1 - ratio) * ratio * 1.2f + ratio * ratio);
	}

	float floatInterpolate(float ratio, float p1, float p2) { // For Bounce effect
		float toReturn = p1 + ratio * (p2 - p1);
		return toReturn;
	}
}
