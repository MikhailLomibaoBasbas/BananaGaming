using UnityEngine;
using System.Collections;


public class AnimationsManagerUpdater : MonoBehaviour {
	private static StaticAnimationsManager instance;
	bool isSet = false;

	void Start () {
		instance = StaticAnimationsManager.getInstance;
		isSet = true;
	}

	void Update () {
		if (isSet) {
			if (instance.isUpdateRunning) {
				instance.UpdateManager ();
			}
		}
	}
}
