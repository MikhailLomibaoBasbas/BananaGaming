using UnityEngine;
using System.Collections;

public class SplineTest : MonoBehaviour {

	// Use this for initialization
	SplineAnimation SplineAnim = null;
	void Awake () {
		delayedAwake ();
		//Invoke ("delayedAwake", 0.1f);
	}

	void delayedAwake () {
		SplineAnim = gameObject.GetComponent<SplineAnimation> ();
		SplineAnim.Initialize ();
		SplineAnim.setValues (4.0f, cWrapMode.Once, false, Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.up * 2f);
		SplineAnim.start (true);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
