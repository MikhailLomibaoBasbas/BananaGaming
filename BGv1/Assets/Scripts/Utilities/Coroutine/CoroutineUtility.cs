using UnityEngine;
using System;
using System.Collections;

public class CoroutineUtility : MonoBehaviour
{
	private static CoroutineUtility _instance;
	private static CoroutineUtility instance{
		get{
			if(_instance == null){
				GameObject go = new GameObject("CoroutineHandler");
				_instance = go.AddComponent<CoroutineUtility>();
				GameObject.DontDestroyOnLoad(_instance);
			}
			return _instance;
		}
	}
	
	public static Coroutine StartCoroutineStatic(IEnumerator routine){
		return instance.StartCoroutine(routine);
	}
}

