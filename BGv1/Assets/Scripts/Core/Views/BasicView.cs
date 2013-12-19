using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class BasicView : MonoBehaviour, IView
{
	#region Factory 
	public static T Create<T>(string resourceName) where T : Component {
		GameObject gameUiRoot = GetGameRootObject();
		return Create<T>(resourceName, gameUiRoot);
	}
	
	public static T Create<T>(string resourceName, GameObject parentObj) where T : Component {
		GameObject panelPrefab = Resources.Load(resourceName) as GameObject;
		T view = default(T);
		if(panelPrefab != null) {
			GameObject panel = GameObject.Instantiate(panelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			panel.transform.parent = parentObj.transform;
			panel.transform.localScale = Vector3.one;
			view = panel.GetComponent<T>();
			if(view == null) view = panel.AddComponent<T>();
		}else{
			Debug.LogWarning("Can't find prefab \"" + resourceName + "\" when creating " + typeof(T).Name);
		}
		return view;
	}
	
	public static GameObject GetGameRootObject() {
		return Game.instance.gameUiRoot;
		//return null;
	}

	#endregion

	#region Button EventListener Methods

	private Dictionary<GameObject,  UIEventListener.VoidDelegate> _buttonMap = new Dictionary<GameObject, UIEventListener.VoidDelegate>();
	public Dictionary<GameObject, UIEventListener.VoidDelegate> getButtonMap {get {return _buttonMap;}}

	protected void AddButtonClickListener (string childName, UIEventListener.VoidDelegate listener) {
		GameObject tempGO = GetChild (childName);
		if (tempGO != null) {
			if (tempGO.GetComponent<UIButton> () != null && (BoxCollider)tempGO.collider != null) {
				if(_buttonMap.ContainsKey(tempGO))
				   return;
				UIEventListener.Get (tempGO).onClick += listener;
				_buttonMap.Add(tempGO, listener);
			} else {
				Debug.LogWarning (tempGO.name + " has neither " + typeof(UIButton).ToString () + " nor " + typeof(BoxCollider).ToString ()); 
			}
		} else {
			Debug.LogWarning ("There is no Child named " + childName);
		}

	}

	public void RemoveButtonClickHandlers () {
		List<GameObject> buttonKeysList = new List<GameObject> (getButtonMap.Keys);
		for (int i = 0; i < buttonKeysList.Count; i++) {
			GameObject tempGO = buttonKeysList [i];
			UIEventListener.Get (tempGO).onClick -= _buttonMap [tempGO];
			_buttonMap.Remove (tempGO);
		}
	}

	#endregion
	
	private Transform _transform;
	protected Transform cachedTransform {
		get {
			if(_transform == null)
				_transform = transform;
			return _transform;
		}
	}
	
	public void AddChild(GameObject child) {
		child.transform.parent = cachedTransform;
	}
	
	public void RemoveChild(string name) {
		GameObject child = GetChild(name);
		if(child != null) {
			child.transform.parent = null;
		}
	}
	
	public GameObject GetChild(string name) {
		Transform child = cachedTransform.FindChild(name);
		if(child != null) {
			return child.gameObject;
		}else{
			Debug.LogWarning(gameObject.name + " has no child \"" + name + "\"");
			return null;
		}
	}
	
	public T GetComponentFromChild<T>(string name) where T : Component {
		GameObject child = GetChild(name);
		if(child == null) return default(T);
		
		T component = child.GetComponent<T>();
		if(component == null) {
			Debug.LogWarning(name + " does not contain a " + typeof(T).Name);
			return default(T);
		}
		
		return component;
	}
	
	public virtual void Show(bool animated) {
		if(animated) {
			if(gameObject.activeSelf) {
				TweenFromRight();
			}else{
				TweenFromLeft();
			}
		}
		gameObject.SetActive(true);
	}
	
	public virtual void Hide(bool animated) {
		if(animated) {
			TweenToLeft("Disable");
		}else{
			Disable();
		}
	}
	
	public virtual void Close(bool animated) {
		if(animated) {
			TweenToRight("DelayedDestroy");
		}else{
			GameObject.Destroy(gameObject);
		}
	}
	
	public void Enable() {
		gameObject.SetActive(true);
	}
	
	public void Disable() {
		gameObject.SetActive(false);
	}
	
	public void ResetPosition() {
		gameObject.transform.localPosition = Vector3.zero;
	}
	
	public void DelayedDestroy() {
		GameObject.Destroy(gameObject, 1f);
	}
	

	void TweenFromRight() {
		ResetPosition();
		iTween.MoveFrom(gameObject, iTween.Hash(
			"position", new Vector3(Screen.width*2f, 0, 0),
			"islocal", true,
			"time", 1f
		));
	}
	
	void TweenFromLeft() {
		ResetPosition();
		iTween.MoveFrom(gameObject, iTween.Hash(
			"position", new Vector3(-Screen.width*2f, 0, 0),
			"islocal", true,
			"time", 1f
		));
	}
	
	void TweenToRight(string onComplete) {
		ResetPosition();
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", new Vector3(Screen.width*2f, 0, 0),
			"islocal", true,
			"time", 1f,
			"oncomplete", onComplete
		));
	}
	
	void TweenToLeft(string onComplete) {
		ResetPosition();
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", new Vector3(-Screen.width*2f, 0, 0),
			"islocal", true,
			"time", 1f,
			"oncomplete", onComplete
		));
	}
}

