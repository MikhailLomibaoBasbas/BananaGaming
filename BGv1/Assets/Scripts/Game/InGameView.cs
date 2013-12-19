using UnityEngine;
using System;

public class InGameView : BasicView
{
	private const string IN_GAME_PANEL = "Prefabs/GUI/GamePanel";
	public static InGameView Create() {
		return Create<InGameView>(IN_GAME_PANEL);
	}
	
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
}

