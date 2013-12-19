using UnityEngine;
using System;

public class NavigationView : BasicView
{
	private const string NAVIGATION_PANEL = "Prefabs/GUI/NavigationPanel";
	public static NavigationView Create() {
		return Create<NavigationView>(NAVIGATION_PANEL);
	}
	public static NavigationView Create(GameObject parent) {
		return Create<NavigationView>(NAVIGATION_PANEL, parent);
	}
	
	public UIEventListener.VoidDelegate onClickBack;
	
	private const string BTN_BACK 		= "AnchorTopLeft/BtnBack";
	void Awake() {
		UIEventListener.Get(GetChild(BTN_BACK)).onClick += OnClickBack;
	}
	
	void OnClickBack(GameObject go) {
		if(onClickBack != null)
			onClickBack(go);
	}
}