using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public sealed class Facebook {
	
	private static ObjCResult _lastResult = new ObjCResult();
	public static ObjCResult lastResult {
		get {
			return _lastResult;
		}
	}
	
	private static Dictionary<string, bool> _statusMap;
	private static Dictionary<string, bool> statusMap {
		get{
			if(_statusMap == null) {
				_statusMap = new Dictionary<string, bool>();
			}
			return _statusMap;
		}
	}
	
	public static void Lock(string methodName) {
		if(statusMap.ContainsKey(methodName))
			statusMap[methodName] = true;
		else
			statusMap.Add(methodName, true);
	}
	
	public static void Unlock(string methodName, bool result, string message) {
		if(statusMap.ContainsKey(methodName))
			statusMap[methodName] = false;
		else
			statusMap.Add(methodName, false);

		_lastResult.success = result;
		_lastResult.message = message;
	}
	
	public static bool IsLocked(string methodName) {
		return statusMap.ContainsKey(methodName) && statusMap[methodName];
	}
	
	[DllImport("__Internal")] 
	private static extern void _fbOpenSession(string fbAppId, ref ObjCResult result);
	public static IEnumerator OpenSession(string fbAppId) {
		if(Application.platform == RuntimePlatform.IPhonePlayer && !IsLocked("_fbOpenSession")) {
			Lock("_fbOpenSession");
			_fbOpenSession(fbAppId, ref _lastResult);
			while(IsLocked("_fbOpenSession")) yield return null;
			Debug.Log(_lastResult.success + " HERALKDSA");
		}else{
			_lastResult.success = false;
		}
	}
	
	[DllImport("__Internal")]
	private static extern void _fbCloseSession();
	public static void CloseSession() {
		if(Application.platform == RuntimePlatform.IPhonePlayer)
			_fbCloseSession();
	}
	
	[DllImport("__Internal")]
	private static extern bool _fbHasOpenSession();
	public static bool HasOpenSession() {
		if(Application.platform == RuntimePlatform.IPhonePlayer)
			return _fbHasOpenSession();
		return false;
	}
	
	
	[DllImport("__Internal")]
	private static extern bool _fbGetUserInfo(ref ObjCResult result);
	public static IEnumerator GetUserInfo() {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			Lock("_fbGetUserInfo");
			_fbGetUserInfo(ref _lastResult);
			while(IsLocked("_fbGetUserInfo")) yield return null;
		}else{
			_lastResult.success = false;
		}
	}

	[DllImport("__Internal")]
	private static extern string _fbGetUserId();
	public static string GetUserId() {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			return _fbGetUserId();
		}else{
			return "__id";
		}
	}

	[DllImport("__Internal")]
	private static extern string _fbGetUserName();
	public static string GetUserName() {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			return _fbGetUserName();
		}else{
			return "__name";
		}
	}

	[DllImport("__Internal")]
	private static extern bool _fbGetUserFriends (ref ObjCResult result);
	public static IEnumerator GetUserFriends() {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			Lock ("_fbGetUserFriends");
			_fbGetUserFriends (ref _lastResult);
			while (IsLocked("_fbGetUserFriends")) yield return null;
		} else {
			_lastResult.success = false;
		}
	}

	[DllImport("__Internal")]
	private static extern int _fbGetUserFriendsCount();
	public static int GetUserFriendsCount() {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			return _fbGetUserFriendsCount();
		}else{
			return 0;
		}
	}

	[DllImport("__Internal")]
	private static extern string _fbGetUserFriendIdAtIndex(int index);
	public static string GetUserFriendIdAtIndex(int index) {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			return _fbGetUserFriendIdAtIndex(index);
		}else{
			return "_id";
		}
	}

	[DllImport("__Internal")]
	private static extern string _fbGetUserFriendNameAtIndex(int index);
	public static string GetUserFriendNameAtIndex(int index) {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			return _fbGetUserFriendNameAtIndex(index);
		}else{
			return "_name";
		}
	}
	
	[DllImport("__Internal")]
	private static extern void _fbPostUserFeed(string message, string name, string caption, string description, string link, string picture, ref ObjCResult result);
	public static IEnumerator PostUserFeed(string message, string name, string caption, string description, string link, string picture) {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			Lock("_fbPostUserFeed");
			_fbPostUserFeed(message, name, caption, description, link, picture, ref _lastResult);
			while (IsLocked("_fbPostUserFeed")) yield return null;
		}else{
			_lastResult.success = false;
		}
	}

	[DllImport("__Internal")]
	private static extern void _fbPostUserFeedWithActions(string message, string name, string caption, string description, string link, string picture, string actions, ref ObjCResult result);
	public static IEnumerator PostUserFeed(string message, string name, string caption, string description, string link, string picture, List<Dictionary<string, string>> actions) {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			string actionString = "";
			foreach(Dictionary<string, string> action in actions) {
				string actionName = action["name"];
				string actionLink = action["link"];
				actionString += actionName + "|" + actionLink + ",";
			}
			// remove last delimiter at the end of the string
			if(actionString.Length > 0) actionString = actionString.Substring(0, actionString.Length-1);
			Lock("_fbPostUserFeedWithActions");
			_fbPostUserFeedWithActions(message, name, caption, description, link, picture, actionString, ref _lastResult);
			while (IsLocked("_fbPostUserFeedWithActions")) yield return null;
		}else{
			_lastResult.success = false;
		}
	}
	
	[DllImport("__Internal")]
	private static extern void _fbPostStatusUpdate();
	public static void PostStatusUpdate(string message) {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			PlayerPrefs.SetString("fb_status_message", message);
			_fbPostStatusUpdate();
		}
	}
	
	[DllImport("__Internal")]
	private static extern void _fbRequestGraphPath();
	public static void RequestGraphPath(string path) {
		RequestGraphPath(path, null, null);
	}
	
	public static void RequestGraphPath(string path, Hashtable parameters, string method) {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			PlayerPrefs.SetString("fb_request_path", path);
			if(parameters != null)
				PlayerPrefs.SetString("fb_request_params", ""/*JSON.Instance.ToJSON(parameters)*/);
			else
				PlayerPrefs.DeleteKey("fb_request_params");
			if(method != null)
				PlayerPrefs.SetString("fb_request_method", method);
			else
				PlayerPrefs.DeleteKey("fb_request_method");
			_fbRequestGraphPath();
		}
	}
	
}
