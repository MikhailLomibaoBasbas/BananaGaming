using UnityEngine;
using System.Collections;

public class ConstantRotation : MonoBehaviour {
	
	public bool isClockWise = true;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(!isClockWise)
			transform.Rotate (0,0,50*Time.deltaTime);
		else
			transform.Rotate (0,0,-50*Time.deltaTime);
	}
}
