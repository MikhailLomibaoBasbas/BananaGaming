using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MiniJSON;

public sealed class Twitter
{
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
	private static extern void _twitterGetAccountInfo(ref ObjCResult result);

	public static IEnumerator GetAccountInfo() {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			Lock("_twitterGetAccountInfo");
			_twitterGetAccountInfo(ref _lastResult);
			while(IsLocked("_twitterGetAccountInfo")) yield return null;
		}else{
			_lastResult.success = false;
		}
	}

	[DllImport("__Internal")]
	private static extern string _twitterGetAccountId();

	public static string GetAccountId() {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			return _twitterGetAccountId();
		}else{
			return "__id";
		}
	}

	[DllImport("__Internal")]
	private static extern string _twitterGetAccountName();

	public static string GetAccountName() {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			return _twitterGetAccountName();
		}else{
			return "__name";
		}
	}
	
	[DllImport("__Internal")]
	private static extern void _twitterGetRequest(string url, string parameters, ref ObjCResult result);
	
	public static IEnumerator GetRequest(string url, Dictionary<string, object> parameters) {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			Lock("_twitterGetRequest");
			_twitterGetRequest(url, Json.Serialize(parameters), ref _lastResult);
			while(IsLocked("_twitterGetRequest")) yield return null;
		}else{
			_lastResult.success = false;
		}
	}
	
	[DllImport("__Internal")]
	private static extern void _twitterPostRequest(string url, string parameters, ref ObjCResult result);
	
	public static IEnumerator PostRequest(string url, Dictionary<string, object> parameters) {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			Lock("_twitterPostRequest");
			_twitterPostRequest(url, Json.Serialize(parameters), ref _lastResult);
			while(IsLocked("_twitterPostRequest")) yield return null;
		}else{
			_lastResult.success = false;
		}
	}
	
	[DllImport("__Internal")]
	private static extern void _twitterDeleteRequest(string url, string parameters, ref ObjCResult result);
	
	public static IEnumerator DeleteRequest(string url, Dictionary<string, object> parameters) {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			Lock("_twitterDeleteRequest");
			_twitterDeleteRequest(url, Json.Serialize(parameters), ref _lastResult);
			while(IsLocked("_twitterDeleteRequest")) yield return null;
		}else{
			_lastResult.success = false;
		}
	}
}