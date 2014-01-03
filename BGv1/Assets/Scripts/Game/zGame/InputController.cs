﻿using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {
	public Camera uiCamera;

	private ArrayList trackers;
	private TouchManager touchManager;
	private GameObject navGO;

	public delegate void OnNavMove (Vector3 normalizedDisplacement);
	public event OnNavMove onNavMove;
	public static InputController instance = null;

	// Use this for initialization
	void Start () {
		instance = this;
		touchManager = TouchManager.getInstance;
	}
	
	// Update is called once per frame
	void Update () {
		touchManager.Update ();
		trackers = touchManager.getTrackers;

		RaycastHit hit;
		int uiLayer = 1 << 8;
		for (int i = 0; i < trackers.Count; i++) {
			TouchTracker touch = (TouchTracker)trackers [i];
			Debug.Log ("TOUCH : " + trackers.Count);
			if (Physics.Raycast (uiCamera.ScreenPointToRay (touch.selfTouch.position), out hit, 20, uiLayer)) {
				Debug.Log ("Name :" + hit.transform.gameObject.name);
				tapped (hit.transform.gameObject, touch.selfTouch);
			}
		}
	}

	private void tapped(GameObject go, Touch touch){
		switch(go.name){
			case "btnNav":
				movePlayer (go, touch);
			break;
		}
	}

	private void movePlayer(GameObject go, Touch touch){
		Vector3 pos = uiCamera.ScreenToWorldPoint(touch.position);
		go.transform.position = pos;
		if (onNavMove != null) {
			onNavMove ((pos - Vector3.zero).normalized);
		}
	}
}
