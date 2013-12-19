using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		tryTest ();
	}

	void tryTest () {
		Vector3 p1 = new Vector3 (10, 2, 0);
		Vector3 p2 = new Vector3 (0, 0, 0);
		Vector3 pComp = p1 - p2;
		Debug.LogError (Mathf.Atan2 (pComp.x, pComp.y) * Mathf.Rad2Deg);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
