using UnityEngine;
using System;

/* This script initializes is where you should set-up the State UI, the UIEventListeners for the NGUI Widgets then use/Initialize them to its corresponding State. */

public class SampleView : BasicView
{
	//Note: IN_GAME_PANEL string is the path where the prefab is located.
	// e.g. "Prefabs/GUI/MyGamePanel"
	private const string PANEL_PATH = "Prefabs/GUI/SamplePanel";
	public static SampleView Create() {
		return Create<SampleView>(PANEL_PATH);
	}
	
	// Note: This is the Universal NavigationView for this Framework.
	// By Default: This holds the Back Button UI. It has a UIEventListener ready 
	// for the said UI. which you can add to your state.
	// THIS IS OPTIONAL.
	private NavigationView _navigationView;
	public NavigationView navigationView {
		get {
			if(_navigationView == null)
				_navigationView = NavigationView.Create(gameObject);
			return _navigationView;
		}
	}

	
	void Awake() {
		navigationView.Enable();
	}

	public override void Show (bool animated) {
		base.Show (animated);
	}

	public override void Hide (bool animated) {
		base.Hide (animated);
	} 

	public override void Close (bool animated) {
		//base.Close (animated);
		//FadeAnimation.setAnimation (gameObject, FadeType.FadeOut, 0.8f, Color.clear, true);
	}
}