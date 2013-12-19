using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public sealed class InAppPurchaseManager
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

	public static void Unlock(string methodName, bool success, string message) {
		if(statusMap.ContainsKey(methodName))
			statusMap[methodName] = false;
		else
			statusMap.Add(methodName, false);
		_lastResult.success = success;
		_lastResult.message = message;
		ParseLastResult();
	}

	public static bool IsLocked(string methodName) {
		return statusMap.ContainsKey(methodName) && statusMap[methodName];
	}

	protected static void ParseLastResult() {
		string[] messages = InAppPurchaseManager._lastResult.message.Split('^');
		if(messages.Length >= 1) {
			_productIds = messages[0].Split('|');
			if(messages.Length >= 2)
				_errors = messages[1].Split('|');
			else
				_errors = new string[]{};
		} else {
			_productIds = new string[]{};
			_errors = new string[]{};
		}
	}

	private static string[] _productIds = null;
	public static string[] productIds {
		get {
			return _productIds;
		}
	}

	private static string[] _errors = null;
	public static string[] errors {
		get {
			return _errors;
		}
	}
	
	[DllImport("__Internal")] 
	private static extern void _purchaseProductWithId(string productId);
	
	public static IEnumerator PurchaseItem(string productId) {
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			Lock("_purchaseProductWithId");
			_purchaseProductWithId(productId);
			while(IsLocked("_purchaseProductWithId")) yield return null;
		}else{
			_lastResult.success = false;
		}
	}
}
