using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputController : MonoBehaviour {
	public Camera uiCamera;

	private ArrayList trackers;
	private TouchManager touchManager;
	private GameObject navGO;

		private Dictionary<TouchTracker,GameObject> copyTouch = new Dictionary<TouchTracker, GameObject>();
	public delegate void OnNavMove (Vector3 normalizedDisplacement, bool pressed);
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

			if (!touch.isEnded && Physics.Raycast (uiCamera.ScreenPointToRay (touch.selfTouch.position), out hit, 20, uiLayer)) 
			{
					Debug.Log ("Name :" + hit.transform.gameObject.name);
					navGO = hit.transform.gameObject;
					if (!copyTouch.ContainsKey(touch))
						copyTouch.Add (touch, navGO);

					tapped (hit.transform.gameObject, touch);
			} else {
					if (copyTouch.ContainsKey (touch)) {									
							tapped (copyTouch [touch], touch);
							copyTouch.Remove (touch);
					}
			}
		}

		//Debug.LogError (trackers.Count);
		touchManager.EndTrackers ();
	}

	private void tapped(GameObject go, TouchTracker touch){
		switch(go.name){
			case "btnNav":
				movePlayer (go, touch);
			break;
		}
	}

	private void tappedFinished(GameObject go, TouchTracker touch) {
		switch(go.name){
		case "btnNav":
			movePlayerFinished (go, touch);
			break;
		}
	}

	private void movePlayer(GameObject go, TouchTracker touch){
				if (!touch.isEnded) {

						Vector3 pos = uiCamera.ScreenToWorldPoint (touch.selfTouch.position);
						go.transform.position = pos;
						Vector3 posGO = go.transform.localPosition;
						//posGO.x = Mathf.Clamp (posGO.x, -40f, 40f);
						//posGO.y = Mathf.Clamp (posGO.y, -40f, 40f);
						Vector3 dir = posGO - Vector3.zero;
						if (dir.magnitude > 100)
								posGO = dir.normalized * 100;


						go.transform.localPosition = posGO;


						if (onNavMove != null) {
				onNavMove ((go.transform.localPosition - Vector3.zero).normalized, true);
						}
				} else
						go.transform.localPosition = Vector3.zero;
	}

	private void movePlayerFinished (GameObject go, TouchTracker touch) {
		onNavMove ((go.transform.localPosition - Vector3.zero).normalized, false);
	}
}
