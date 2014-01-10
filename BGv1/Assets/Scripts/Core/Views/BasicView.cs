using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;



public class BasicView : MonoBehaviour, IView
{
	public enum ViewTransitionType {
		Fade, // For 2D only
		Scale,
		Move, // Buggy for 3DView
		TweenToLeft,
		TweenToRight,
		TweenFromLeft,
		TweenFromRight,
		Default
	}

	protected ViewTransitionType transitionType = ViewTransitionType.Default;

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
			if(parentObj != null)
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

	private static Camera _uICamera;
	public static Camera GetUICamera {
		get {
			if(_uICamera == null)
				_uICamera = Game.instance.gameUiRoot.transform.FindChild("Camera").camera;
			return _uICamera;
		}
	}

	#endregion

	#region Button EventListener Methods

	private Dictionary<GameObject,  UIEventListener.VoidDelegate> _buttonMap = new Dictionary<GameObject, UIEventListener.VoidDelegate>();
	public Dictionary<GameObject, UIEventListener.VoidDelegate> GetButtonMap {get {return _buttonMap;}}

	protected virtual void AddButtonClickListener (string childName, UIEventListener.VoidDelegate listener) {
		GameObject tempGO = GetChild (childName);
		if (tempGO != null) {
			if (tempGO.GetComponent<UIButton> () != null || (tempGO.GetComponent<UIImageButton> () != null) || (tempGO.collider as BoxCollider) != null) {
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

	public virtual void RemoveButtonClickHandlers () {
		List<GameObject> buttonKeysList = new List<GameObject> (GetButtonMap.Keys);
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
			switch(transitionType) {
				//case ViewTransitionType.Default:
				case ViewTransitionType.TweenFromLeft:
				case ViewTransitionType.TweenFromRight:
					if (gameObject.activeSelf) {
						TweenFromRight ();
					} else {
						TweenFromLeft ();
					}
					break;
				default:
					StartTransition (transitionType);
					break;
			}
		}
		transitionType = ViewTransitionType.Default;
		gameObject.SetActive(true);
	}
	
	public virtual void Hide(bool animated) {
		if(animated) {
			switch(transitionType) {
			//case ViewTransitionType.Default:
			case ViewTransitionType.TweenToLeft:
				TweenToLeft("Disable");
				break;
			default:
				StartTransition (transitionType, "Disable");
				break;
			}
		}else{
			Disable();
		}
		transitionType = ViewTransitionType.Default;
	}
	
	public virtual void Close(bool animated) {
		if(animated) {
			switch(transitionType) {
			//case ViewTransitionType.Default:
			case ViewTransitionType.TweenToRight:
				TweenToRight("DelayedDestroy");
				break;
			default:
				StartTransition (transitionType, "DelayedDestroy");
				break;
			}

		}else{
			GameObject.Destroy(gameObject);
		}
		//transitionType = ViewTransitionType.Default;
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
		GameObject.Destroy(gameObject, 0.5f);
	}

	public void Destroy() {
		GameObject.Destroy (gameObject);
	}

	void StartTransition (ViewTransitionType type, string onComplete = null) {
		bool onCompleteNull = (onComplete == null);
		FadeTypeV2 fadeType = (onCompleteNull ? FadeTypeV2.FadeIn : FadeTypeV2.FadeOut);
		switch (type) {
		case ViewTransitionType.Fade:
			Fade (fadeType, onComplete);
			break;
		case ViewTransitionType.Scale:
			Scale (fadeType, onComplete);
			break;
		case ViewTransitionType.Move:
			Move (fadeType, onComplete);
			break;
		case ViewTransitionType.TweenFromLeft:
			break;
		case ViewTransitionType.TweenFromRight:
			break;
		case ViewTransitionType.TweenToLeft:
			break;
		case ViewTransitionType.TweenToRight:
			break;
		default:
			if(onComplete != null)
				Invoke (onComplete, 0.1f);
			break;
		}
	}

	FadeAnimationV2 fadeAnim = null;
	void Fade(FadeTypeV2 type, string onComplete = null) {
		if(fadeAnim == null)
			fadeAnim = FadeAnimationV2.set 
			(gameObject, 0.4f, type, false, cWrapMode.Once);
		if (onComplete != null)
			fadeAnim.addCallback (this, onComplete, null);
		fadeAnim.start (true);
	}

	void Scale (FadeTypeV2 fadeType, string onComplete = null) { //By default, accompanied by fade animation
		Fade (fadeType);
		bool onCompleteNull = (onComplete == null);
		if(!onCompleteNull)
			onComplete = "Destroy";
		Vector3 scaleFrom = transform.localScale * (onCompleteNull ? 0.1f : 1.0f);
		Vector3 scaleTo = transform.localScale * (onCompleteNull ? 1.0f : 0.1f);
		ScaleAnimation.set (gameObject, scaleFrom, scaleTo, 0.3f, cWrapMode.Once, 0, onCompleteNull,
		                             this, onComplete, null, true);
	}

	void Move (FadeTypeV2 fadeType, string onComplete = null) {
		Fade (fadeType);
		bool onCompleteNull = (onComplete == null);
		Vector3 moveFrom =(onCompleteNull ? Vector3.up * Screen.height : Vector3.zero);
		Vector3 MoveTo = (onCompleteNull ?Vector3.zero : Vector3.up * Screen.height);
		MoveAnimationV2 tempAnim = MoveAnimationV2.set (gameObject, moveFrom, MoveTo, 0.6f, true, cWrapMode.Once, false);
		if(onComplete != null)
			tempAnim.addCallback (this, "onComplete", null, true);
		tempAnim.start (true);
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

