using UnityEngine;
using System;

public class MainMenuView : BasicView
{
	private const string MAIN_MENU_PANEL = "Prefabs/GUI/MainMenuPanel";
	public static MainMenuView Create() {
		return Create<MainMenuView>(MAIN_MENU_PANEL);
	}
	
	public UIEventListener.VoidDelegate onClickPlay = null;
	public UIEventListener.VoidDelegate onClickOption = null;

	private const string BTN_PLAY 		= "Anchor/BtnPlay";
	private const string BTN_OPTION 	= "Anchor/BtnOption";

	
	void Awake() {
		UIEventListener.Get(GetChild(BTN_PLAY)).onClick += OnClickPlay;
		UIEventListener.Get(GetChild(BTN_OPTION)).onClick += OnClickOption;
	}
	
	void OnClickPlay(GameObject go) {
		if(onClickPlay != null)
			onClickPlay(go);
	}
	
	void OnClickOption(GameObject go) {
		if(onClickOption != null)
			onClickOption(go);
	}
}