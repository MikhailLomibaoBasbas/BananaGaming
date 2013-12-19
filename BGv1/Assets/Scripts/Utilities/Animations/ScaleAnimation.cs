using UnityEngine;
using System.Collections;

public class ScaleAnimation : CommonAnimation {
	
	public bool _isRepeating;

	private Vector3 _scaleFrom;
	private Vector3 _scaleTo;
	private float _mult;
	private Vector3 _scaleDifference;

	private bool _isInitialized;

	private Vector3 _origTargetScale;

	private float _endDelay = 0.0f; // For a rough Bounce effect

	public static ScaleAnimation setAnimation(GameObject targetGO, Vector3 scaleFrom, Vector3 scaleTo, float time, float delay = 0f, float endDelay = 0f,  bool startImmidiately = false,  object classPtr = null, string callbackFunction = null, object[] parameters = null, bool oneShot = false, bool hidden = false) {
		ScaleAnimation temp;
		if((temp = targetGO.GetComponent<ScaleAnimation>()) == null) {
			temp = targetGO.AddComponent<ScaleAnimation> ();
		}
		temp.Initialize ();
		temp.setValues (scaleFrom, scaleTo, time, endDelay);
		if (classPtr != null && callbackFunction != null) {
			temp.addCallBack (classPtr, callbackFunction, parameters, oneShot);
		}
		if(startImmidiately)
			temp.StartAnimation (true, delay, hidden);
		return temp;
	}

	// Use this for initialization
	void Start () {
		Initialize ();
	}

	public override void Initialize () {
		base.Initialize ();
		if (!_isInitialized) {
			_isInitialized = true;
		}
	}

	public void setValues(Vector3 scaleFrom, Vector3 scaleTo, float time, float endDelay = 0) {
		_endDelay = endDelay;
		target.localScale = _origTargetScale = _scaleFrom = scaleFrom;
		_scaleTo = scaleTo;
		_scaleDifference = (_scaleFrom - _scaleTo);
		_scaleDifference.x = Mathf.Abs (_scaleDifference.x);
		_scaleDifference.y = Mathf.Abs (_scaleDifference.y);
		_scaleDifference.z = Mathf.Abs (_scaleDifference.z);
		animationTime = time;
		_mult = (scaleTo.x * scaleTo.y < scaleFrom.x * scaleFrom.y )? -1f: 1f;
		//Debug.Log ((scaleTo.x * scaleTo.y) + " " + scaleTo.magnitude + " " + scaleFrom.magnitude + " " + (scaleFrom.x * scaleFrom.y) + " " + _mult);
	}

	public void StartAnimation(bool flag, float delay = 0, bool hidden = false){
		if (!flag)
			doCallback ();
		isAnimating = (delay > 0) ? false: flag;
		timeAnimating = 0f;
		target.localScale = (flag)? _scaleFrom: _scaleTo;
		if(delay > 0) {
			target.gameObject.SetActive (!hidden);
			object[] par = new object[2];
			par[0] = flag;
			par[1] = hidden;
			static_coroutine.getInstance.DoReflection(this, "setIsAnimatingEnabled", par, delay);
		}
	}

	public void setIsAnimatingEnabled(bool flag, bool unhid = false){
		isAnimating = flag;
		if(unhid)
			target.gameObject.SetActive (unhid);
	}

	// Update is called once per frame
	void Update () {
		if (isAnimating) {
			timeAnimating += (Time.timeScale > 0) ? (Time.deltaTime / Time.timeScale): Time.fixedDeltaTime;
			Vector3 tempLocalScale = _origTargetScale;
			float tempR = _mult * ((timeAnimating) / (animationTime));
			if (timeAnimating < animationTime + _endDelay) {
				//tempLocalScale.x += tempR * _scaleDifference.x;
				//tempLocalScale.y += tempR * _scaleDifference.y;
				tempLocalScale += tempR * _scaleDifference;
				target.localScale = tempLocalScale;
			} else { // End of animation
				StartAnimation (false, 0);
			}
		}

	}
}
