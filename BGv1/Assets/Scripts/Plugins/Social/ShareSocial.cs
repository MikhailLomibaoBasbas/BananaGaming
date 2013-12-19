using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

public sealed class ShareSocial {
	
	private const string LISTENER_GAMEOBJECT_NAME = "ShareSocialListener";
	
	public static void CreateListener() {
		GameObject ssListener = GameObject.Find(LISTENER_GAMEOBJECT_NAME);
		if(ssListener == null) {
			ssListener = new GameObject(LISTENER_GAMEOBJECT_NAME);
			ssListener.AddComponent<ShareSocialListener>();
			GameObject.DontDestroyOnLoad(ssListener);
		}
	}
	
	[DllImport("__Internal")]
	private static extern void _socialOpen ();
	
	public static void OpenSocialMenu (string message, string name, string caption, string description, string link, string picture, string appID, string screenshot) {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			Debug.Log("OpenSocialMenu");
			PlayerPrefs.SetString("social_share_message", message);
			PlayerPrefs.SetString("social_share_name", name);
			PlayerPrefs.SetString("social_share_caption", caption);
			PlayerPrefs.SetString("social_share_description", description);
			PlayerPrefs.SetString("social_share_link", link);
			PlayerPrefs.SetString("social_share_picture", picture);
			PlayerPrefs.SetString("social_share_appID", appID);
			PlayerPrefs.SetString("social_share_screenshot", screenshot); // This is the directory of the screenshot that was taken
			
			_socialOpen ();
		}
	}
	
	[DllImport("__Internal")]
	private static extern bool _isActivityViewController ();
	
	public static bool isActivityViewController () {
		return _isActivityViewController();
	}
}
