using UnityEngine;
using System;

public class SocialView : BasicView {
	
	private const string SOCIAL_PANEL = "Prefabs/GUI/SocialPanel";
	public static SocialView Create() {
		return Create<SocialView>(SOCIAL_PANEL);
	}
	
	private NavigationView _navigationView;
	public NavigationView navigationView {
		get {
			if(_navigationView == null)
				_navigationView = NavigationView.Create(gameObject);
			return _navigationView;
		}
	}
	
	private const string LBL_USERNAME = "AnchorTopLeft/LblUsername";
	
	void Awake() {
		navigationView.Enable();
	}
	
	public void SetName(string name) {
		GetComponentFromChild<UILabel>(LBL_USERNAME).text = name;
	}
}