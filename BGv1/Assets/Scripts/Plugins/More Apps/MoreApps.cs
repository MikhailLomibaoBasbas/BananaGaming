using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

public class MoreApps : MonoBehaviour {
	
	private const string LISTENER_GAMEOBJECT_NAME = "MoreAppsListener";
	private const string MORE_APPS_URL = "http://appinfo.bizdev-klab.com/en";

	[DllImport("__Internal")]
	private static extern void _moreAppsOpenLink (string url);
	public static void OpenWindow() {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			_moreAppsOpenLink(MORE_APPS_URL);
		}
	}
	
}
